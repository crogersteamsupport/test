using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace WatsonToneAnalyzer
{
    [Table(Name = "TicketSentiments")]
    class TicketSentiment
    {
        const string EVENT_SOURCE = "Application";

        int _ticketSentimentID;
        [Column(Storage = "_ticketSentimentID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int TicketSentimentID { get { return _ticketSentimentID; } }

        [Column]
        public int TicketID;
        [Column]
        public int OrganizationID;
        [Column]
        public bool IsAgent;
        [Column]
        public int TicketSentimentScore;
        [Column]
        public bool Sad;
        [Column]
        public bool Frustrated;
        [Column]
        public bool Satisfied;
        [Column]
        public bool Excited;
        [Column]
        public bool Polite;
        [Column]
        public bool Impolite;
        [Column]
        public bool Sympathetic;

        public void SetSentimentID(int sentimentID)
        {
            switch (sentimentID)
            {
                case 1:
                    Sad = true;
                    break;
                case 2:
                    Frustrated = true;
                    break;
                case 3:
                    Satisfied = true;
                    break;
                case 4:
                    Excited = true;
                    break;
                case 5:
                    Polite = true;
                    break;
                case 6:
                    Impolite = true;
                    break;
                case 7:
                    Sympathetic = true;
                    break;
            }
        }

        static int OrganizationSentiment(int organizationID)
        {
            double result = 0;
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    Table<TicketSentiment> ticketSentimentTable = db.GetTable<TicketSentiment>();
                    result = (from sentiment in ticketSentimentTable where (sentiment.OrganizationID == organizationID) select sentiment.TicketSentimentScore).Average();
                }
            }
            catch (Exception e)
            {
                EventLog.WriteEntry(EVENT_SOURCE, "Exception caught at OrganizationSentiment:" + e.Message + " ----- STACK: " + e.StackTrace.ToString());
                Console.WriteLine(e.ToString());
            }
            return (int)Math.Round(result);
        }

        /// <summary>
        /// Record used in rolling up the sentiment scores for the ticket
        /// </summary>
        class MaxActionSentimentScore
        {
            public int ActionID;
            public int SentimentID;
            public decimal MaxSentimentScore;
            public decimal SentimentMultiplier;
        }

        static Mutex _mutex = new Mutex(false);

        /// <summary>
        /// Callback on insert of Watson results - using the transaction submitted but NOT committed
        /// 
        /// For each action, pick the most likely sentiment (highest Sentimentscore)
        /// Use Net Promoter score (promoters - detractors), normalized to [0, 1000] where 500 is neutral
        /// </summary>
        /// <param name="transaction">data associated with the watson transaction</param>
        public static void TicketSentimentStrategy(int ticketID, int organizationID, bool isAgent)
        {
            try
            {
                _mutex.WaitOne();
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    string query = @"SELECT m.ActionID, s.SentimentID, m.MaxSentimentScore, t.SentimentMultiplier
                                        FROM (
                                            SELECT a.ActionID, a.ActionSentimentID, MAX(s.SentimentScore) AS MaxSentimentScore
                                            FROM ActionSentiments a
                                            INNER JOIN ActionSentimentScores s ON a.ActionSentimentID=s.ActionSentimentID
                                            WHERE TicketID={0} AND a.IsAgent='{1}'
                                            GROUP BY a.ActionID, a.ActionSentimentID
                                        ) AS m
                                        INNER JOIN ActionSentimentScores AS s ON m.ActionSentimentID=s.ActionSentimentID AND m.MaxSentimentScore=s.SentimentScore
                                        INNER JOIN ToneSentiments AS t ON t.SentimentID=s.SentimentID
                                        GROUP BY m.ActionID, s.SentimentID, m.MaxSentimentScore, t.SentimentMultiplier";
                    string fullQuery = string.Format(query, ticketID, isAgent ? "1" : "0");
                    var result = db.ExecuteQuery<MaxActionSentimentScore>(fullQuery);
                    //var result = db.ExecuteQuery<MaxActionSentimentScore>(query, ticketID, isAgent ? "1" : "0");  // this throws an exception on converstion of isAgent to a bit?

                    // attach to the ticket score
                    Table<TicketSentiment> ticketScoresTable = db.GetTable<TicketSentiment>();
                    TicketSentiment ticketSentimentScore = (from u in ticketScoresTable where u.TicketID == ticketID select u).FirstOrDefault();
                    if (ticketSentimentScore == null)
                    {
                        ticketSentimentScore = new TicketSentiment()
                        {
                            TicketID = ticketID,
                            OrganizationID = organizationID,
                            IsAgent = isAgent,
                            TicketSentimentScore = 0,
                            Sad = false,
                            Frustrated = false,
                            Satisfied = false,
                            Excited = false,
                            Polite = false,
                            Impolite = false,
                            Sympathetic = false
                        };
                        ticketScoresTable.InsertOnSubmit(ticketSentimentScore);
                    }

                    // calculate a normalized ticket sentiment
                    double ticketSentiment = 0;
                    {
                        int count = 0;
                        List<int> sentiments = new List<int>();
                        foreach (MaxActionSentimentScore record in result)
                        {
                            ++count;
                            if (record.SentimentID == 0)    // no sentiment found
                                continue;

                            ticketSentiment += Convert.ToDouble(record.SentimentMultiplier) * Convert.ToDouble(record.MaxSentimentScore);
                            ticketSentimentScore.SetSentimentID(record.SentimentID);
                        }

                        if (count != 0)
                            ticketSentiment /= count;  // normalize to +- 100%
                        ticketSentiment = 500 * ticketSentiment + 500;  // normalize to [0, 1000]
                    }

                    // submit record
                    ticketSentimentScore.TicketSentimentScore = (int)Math.Round(ticketSentiment);
                    db.SubmitChanges();
                }
            }
            catch (SqlException e1)
            {
                EventLog.WriteEntry(EVENT_SOURCE, "There was an issues with the sql server:" + e1.ToString() + " ----- STACK: " + e1.StackTrace.ToString());
                Console.WriteLine(e1.ToString());
            }
            catch (Exception e2)
            {
                EventLog.WriteEntry(EVENT_SOURCE, "Exception caught at select from ACtionsToAnalyze or HttpPOST:" + e2.Message + " ----- STACK: " + e2.StackTrace.ToString());
                Console.WriteLine(e2.ToString());
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }


    }
}
