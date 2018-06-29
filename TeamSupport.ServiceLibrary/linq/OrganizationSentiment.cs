﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Configuration;
using System.Diagnostics;

namespace WatsonToneAnalyzer
{
    [Table(Name = "OrganizationSentiments")]
    class OrganizationSentiment
    {
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

        // first ticket for organization
        static bool CreateOrganizationSentiment(TicketSentiment sentiment, DataContext db, out OrganizationSentiment score)
        {
            Table<OrganizationSentiment> table = db.GetTable<OrganizationSentiment>();
            var results = from u in table where (u.OrganizationID == sentiment.OrganizationID) && (u.IsAgent == sentiment.IsAgent) select u;
            score = results.FirstOrDefault();
            if (score == null)
            {
                score = new OrganizationSentiment()
                {
                    OrganizationID = sentiment.OrganizationID,
                    IsAgent = sentiment.IsAgent,
                    OrganizationSentimentScore = sentiment.AverageActionSentiment,
                    TicketSentimentCount = 1
                };
                table.InsertOnSubmit(score);
                return true;
            }
            return false;
        }

        // new ticket for organization
        public static void AddTicket(TicketSentiment sentiment, DataContext db)
        {
            if (sentiment.IsAgent)  // only client data
                return;

            try
            {
                OrganizationSentiment score;
                if (!CreateOrganizationSentiment(sentiment, db, out score))
                {
                    int count = score.TicketSentimentCount;
                    score.OrganizationSentimentScore = ((count * score.OrganizationSentimentScore) + sentiment.AverageActionSentiment) / (count + 1);
                    score.TicketSentimentCount = count + 1;
                }
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                WatsonEventLog.WriteEntry("Unable to update ticket on Organization", e);
                Console.WriteLine(e.ToString());
            }
        }

        // ticket has new action - recalculate
        public static void UpdateTicket(TicketSentiment sentiment, double oldScore, DataContext db)
        {
            if (sentiment.IsAgent)  // only client data
                return;

            try
            {
                // new ticket score == old ticket score
                if (sentiment.AverageActionSentiment == oldScore)
                    return;

                OrganizationSentiment score;
                if (!CreateOrganizationSentiment(sentiment, db, out score))
                {
                    int count = score.TicketSentimentCount;
                    score.OrganizationSentimentScore = score.OrganizationSentimentScore + (sentiment.AverageActionSentiment - oldScore) / count;
                }
            }
            catch(Exception e)
            {
                string message = String.Format("Unable to update ticket {0} on Organization {1} ", sentiment.TicketID, sentiment.OrganizationID);
                WatsonEventLog.WriteEntry(message, e);
                Console.WriteLine(e.ToString());
            }
        }

    }

}
