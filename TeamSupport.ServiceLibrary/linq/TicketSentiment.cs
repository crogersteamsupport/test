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
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
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
        [Column]
        public double AverageActionSentiment;
        [Column]
        public int ActionSentimentCount;
        [Column]
        public DateTime TicketDateCreated;
#pragma warning restore CS0649

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

        static Mutex _mutex = new Mutex(false);
        static ToneSentiment[] _toneSentiment = null;

        /// <summary>
        /// Callback on insert of Watson results - using the transaction submitted but NOT committed
        /// 
        /// For each action, use the most likely sentiment (highest SentimentScore)
        /// Use Net Promoter score (promoters - detractors), normalized to [0, 1000] where 500 is neutral
        /// </summary>
        /// <param name="transaction">data associated with the watson transaction</param>
        public static void TicketSentimentStrategy(DataContext db, ActionToAnalyze actionToAnalyze, ActionSentimentScore maxScore)
        {
            try
            {
                // we need the multipliers from ToneSentiment (frustrated = -1, satisfied = +1...)
                if (_toneSentiment == null)
                {
                    Table<ToneSentiment> tones = db.GetTable<ToneSentiment>();
                    _toneSentiment = (from tone in tones select tone).ToArray();
                }

                // normalize to [0, 1000]
                double actionScore = Convert.ToDouble(_toneSentiment[maxScore.SentimentID].SentimentMultiplier.Value) * Convert.ToDouble(maxScore.SentimentScore);
                actionScore = 500 * actionScore + 500;

                // submit TicketSentiment and update OrganizationSentiment
                TicketSentiment score = null;
                if (CreateTicketSentiment(db, actionToAnalyze, actionScore, out score))
                {
                    // new ticket
                    OrganizationSentiment.AddTicket(score, db);
                }
                else
                {
                    UpdateAverageSentiment(db, actionScore, score);
                }

                score.TicketSentimentScore = (int)Math.Round(score.AverageActionSentiment);
                score.SetSentimentID(maxScore.SentimentID);
                db.SubmitChanges();
            }
            catch (Exception e2)
            {
                WatsonEventLog.WriteEntry("Exception caught at select from ACtionsToAnalyze or HttpPOST:", e2);
                Console.WriteLine(e2.ToString());
            }
        }

        private static void UpdateAverageSentiment(DataContext db, double actionScore, TicketSentiment score)
        {
            // new action on existing ticket
            int count = score.ActionSentimentCount;
            double oldScore = score.AverageActionSentiment;
            score.AverageActionSentiment = (count * score.AverageActionSentiment + actionScore) / (count + 1);
            score.ActionSentimentCount = count + 1;
            OrganizationSentiment.UpdateTicket(score, oldScore, db);
        }

        static bool CreateTicketSentiment(DataContext db, ActionToAnalyze actionToAnalyze, double actionScore, out TicketSentiment score)
        {
            Table<TicketSentiment> table = db.GetTable<TicketSentiment>();
            score = table.Where(t => t.TicketID == actionToAnalyze.TicketID).FirstOrDefault();
            if (score == null)
            {
                score = new TicketSentiment()
                {
                    TicketID = actionToAnalyze.TicketID,
                    OrganizationID = actionToAnalyze.OrganizationID,
                    IsAgent = actionToAnalyze.IsAgent,
                    TicketDateCreated = actionToAnalyze.DateCreated,
                    TicketSentimentScore = 0,
                    Sad = false,
                    Frustrated = false,
                    Satisfied = false,
                    Excited = false,
                    Polite = false,
                    Impolite = false,
                    Sympathetic = false,
                    AverageActionSentiment = actionScore,
                    ActionSentimentCount = 1
                };
                table.InsertOnSubmit(score);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Record used in rolling up the sentiment scores for the ticket
        /// </summary>
        class MaxActionSentiment
        {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
            public int TicketID;
            public int OrganizationID;
            public int ActionID;
            public int SentimentID;
            public decimal MaxSentimentScore;
            public bool IsAgent;
            public DateTime DateCreated;
#pragma warning restore CS0649
        }

        /// <summary>
        /// If needed we can recreate the TicketSentiments table from ActionSentiments
        /// 
        /// WARNING - Must first call TRUNCATE TABLE [dbo].[TicketSentiments]
        /// </summary>
        public static void RecreateTableFromActionSentiments()
        {
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    // we need the multipliers from ToneSentiment (frustrated = -1, satisfied = +1...)
                    if (_toneSentiment == null)
                    {
                        Table<ToneSentiment> tones = db.GetTable<ToneSentiment>();
                        _toneSentiment = (from tone in tones select tone).ToArray();
                    }

                    // load the most likely sentiment for each action
                    string query = @"SELECT m.TicketID, m.OrganizationID, m.ActionID, s.SentimentID, m.MaxSentimentScore, m.IsAgent, t.DateCreated
                        FROM (
	                        SELECT a.TicketID, a.OrganizationID, a.ActionID, a.IsAgent, a.ActionSentimentID, MAX(s.SentimentScore) AS MaxSentimentScore
	                        FROM ActionSentiments as a
	                        INNER JOIN ActionSentimentScores s ON a.ActionSentimentID=s.ActionSentimentID
	                        GROUP BY a.TicketID, a.OrganizationID, a.ActionID, a.IsAgent, a.ActionSentimentID
                        ) AS m
                        INNER JOIN ActionSentimentScores AS s ON m.ActionSentimentID=s.ActionSentimentID AND m.MaxSentimentScore=s.SentimentScore
                        JOIN Tickets AS t ON m.TicketID=t.TicketID
                        GROUP BY m.TicketID, m.OrganizationID, m.ActionID, s.SentimentID, m.MaxSentimentScore, m.IsAgent, t.DateCreated";
                    MaxActionSentiment[] maxActionSentiments = db.ExecuteQuery<MaxActionSentiment>(query).ToArray();

                    // load the ticket sentimentts
                    Table<TicketSentiment> table = db.GetTable<TicketSentiment>();
                    TicketSentiment[] ticketSentiments = table.ToArray();
                    Dictionary<int, TicketSentiment> avgTicketSentiments = new Dictionary<int, TicketSentiment>();
                    foreach (TicketSentiment ticketSentiment in ticketSentiments)
                        avgTicketSentiments[ticketSentiment.TicketID] = ticketSentiment;

                    foreach (MaxActionSentiment maxActionSentiment in maxActionSentiments)
                    {
                        // normalize to [0, 1000]
                        double ticketSentimentScore = Convert.ToDouble(_toneSentiment[maxActionSentiment.SentimentID].SentimentMultiplier.Value) * Convert.ToDouble(maxActionSentiment.MaxSentimentScore);
                        ticketSentimentScore = 500 * ticketSentimentScore + 500;

                        TicketSentiment sentiment;
                        if (avgTicketSentiments.ContainsKey(maxActionSentiment.TicketID))
                        {
                            sentiment = avgTicketSentiments[maxActionSentiment.TicketID];
                            int count = sentiment.ActionSentimentCount;
                            sentiment.AverageActionSentiment = (count * sentiment.AverageActionSentiment + ticketSentimentScore) / (count + 1);
                            sentiment.ActionSentimentCount = count + 1;
                            sentiment.SetSentimentID(maxActionSentiment.SentimentID);
                        }
                        else
                        {
                            sentiment = new TicketSentiment()
                            {
                                TicketID = maxActionSentiment.TicketID,
                                OrganizationID = maxActionSentiment.OrganizationID,
                                IsAgent = maxActionSentiment.IsAgent,
                                TicketSentimentScore = (int)Math.Round(ticketSentimentScore),
                                Sad = false,
                                Frustrated = false,
                                Satisfied = false,
                                Excited = false,
                                Polite = false,
                                Impolite = false,
                                Sympathetic = false,
                                AverageActionSentiment = ticketSentimentScore,
                                ActionSentimentCount = 1,
                                TicketDateCreated = maxActionSentiment.DateCreated
                            };
                            avgTicketSentiments[sentiment.TicketID] = sentiment;
                            sentiment.SetSentimentID(maxActionSentiment.SentimentID);
                            table.InsertOnSubmit(sentiment);
                        }
                    }
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                WatsonEventLog.WriteEntry("Exception while creating ticket sentiments table:", e);
                Console.WriteLine(e.ToString());
            }
        }
    }
}
