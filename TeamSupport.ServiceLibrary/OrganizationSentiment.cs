using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Configuration;

namespace WatsonToneAnalyzer
{
    [Table(Name = "OrganizationSentiments")]
    class OrganizationSentiment
    {
        const string EVENT_SOURCE = "Application";

#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _organizationSentimentID;
        [Column(Storage = "_organizationSentimentID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int OrganizationSentimentID { get { return _organizationSentimentID; } }

        [Column]
        public int OrganizationID;
        [Column]
        public bool IsAgent;
        [Column]
        public double OrganizationSentimentScore;
        [Column]
        public int TicketSentimentCount;
#pragma warning restore CS0649
        /// <summary>
        /// Raw calculation
        /// </summary>
        /// <param name="organizationID"></param>
        /// <returns></returns>
        public static int GetOrganizationSentiment(int organizationID)
        {
            double result = 0;
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    Table<TicketSentiment> ticketSentimentTable = db.GetTable<TicketSentiment>();
                    //result = ticketSentimentTable.Where(s => s.OrganizationID == organizationID).Select(s => s.TicketSentimentScore).Average();
                    result = (from sentiment in ticketSentimentTable where (sentiment.OrganizationID == organizationID) select sentiment.TicketSentimentScore).Average();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.EventLog.WriteEntry(EVENT_SOURCE, "Exception caught at OrganizationSentiment:" + e.Message + " ----- STACK: " + e.StackTrace.ToString());
                Console.WriteLine(e.ToString());
            }
            return (int)Math.Round(result);
        }

        // new ticket for organization
        public static void AddTicket(TicketSentiment sentiment, DataContext db)
        {
            try
            {
                Table<OrganizationSentiment> table = db.GetTable<OrganizationSentiment>();
                var results = from u in table where (u.OrganizationID == sentiment.OrganizationID) && (u.IsAgent == sentiment.IsAgent) select u;
                OrganizationSentiment score = results.FirstOrDefault();
                if (score == null)
                {
                    score = new OrganizationSentiment()
                    {
                        OrganizationID = sentiment.OrganizationID,
                        IsAgent = sentiment.IsAgent,
                        OrganizationSentimentScore = sentiment.TicketSentimentScore,
                        TicketSentimentCount = 1
                    };
                    table.InsertOnSubmit(score);
                }
                else
                {
                    int count = score.TicketSentimentCount;
                    score.OrganizationSentimentScore = ((count * score.OrganizationSentimentScore) + sentiment.TicketSentimentScore) / (count + 1);
                    score.TicketSentimentCount = count + 1;
                }
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.EventLog.WriteEntry(EVENT_SOURCE, "Unable to update ticket on Organization" + e.Message + " ----- STACK: " + e.StackTrace.ToString());
                Console.WriteLine(e.ToString());
            }
        }

        // ticket has new action - recalculate
        public static void UpdateTicket(TicketSentiment sentiment, int oldScore, DataContext db)
        {
            try
            {
                // new ticket score == old ticket score
                if (sentiment.TicketSentimentScore == oldScore)
                    return;

                Table<OrganizationSentiment> table = db.GetTable<OrganizationSentiment>();
                var results = from u in table where (u.OrganizationID == sentiment.OrganizationID) && (u.IsAgent == sentiment.IsAgent) select u;
                OrganizationSentiment score = results.FirstOrDefault();

                int count = score.TicketSentimentCount;
                score.OrganizationSentimentScore = score.OrganizationSentimentScore + (sentiment.TicketSentimentScore - oldScore) / count;
                db.SubmitChanges();
            }
            catch(Exception e)
            {
                string message = String.Format("Unable to update ticket {0} on Organization {1} ", sentiment.TicketID, sentiment.OrganizationID);
                System.Diagnostics.EventLog.WriteEntry(EVENT_SOURCE, message + e.Message + " ----- STACK: " + e.StackTrace.ToString());
                Console.WriteLine(e.ToString());
            }
        }

    }

}
