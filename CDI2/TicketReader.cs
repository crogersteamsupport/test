﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Linq;
using System.IO;
using TeamSupport.CDI.linq;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Load the tickets from the database from AnalysisInterval StartDate to EndDate
    /// * ignore tickets closed in less than a second
    /// * ignore tickets imported from SalesForce
    /// * ignore ExcludeFromCDI ticket types and ticket statuses
    /// </summary>
    class TicketReader
    {
        DateRange _dateRange;
        public TicketJoin[] AllTickets { get; private set; }

        /// <summary>Time frame to analyze the ticket data</summary>
        /// <param name="analysisInterval"></param>
        public TicketReader(DateRange analysisInterval)
        {
            _dateRange = analysisInterval;
        }

        /// <summary> 
        /// Load the tickets since the start date 
        /// 
        /// Verified counts by the following query:
        /// SELECT DISTINCT Count([TicketID])
        ///  FROM [dbo].[Tickets] as t
        ///  JOIN [dbo].[TicketTypes] as tt on t.TicketTypeID=tt.TicketTypeID
        ///  JOIN [dbo].[TicketStatuses] as ts on t.TicketStatusID=ts.TicketStatusID
        ///  WHERE t.DateCreated > '2013-04-29 00:00:00' AND (t.TicketSource != 'SalesForce') AND
        ///  ((ts.IsClosed=0) OR (t.[DateClosed] > t.[DateCreated]))
        /// </summary>
        public void LoadAllTickets()
        {
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    // define the tables we will reference in the query
                    // - each table class only contains the fields we care about
                    Table<Ticket> ticketsTable = db.GetTable<Ticket>();
                    Table<TicketStatus> ticketStatusesTable = db.GetTable<TicketStatus>();
                    Table<TicketType> ticketTypesTable = db.GetTable<TicketType>();
                    Table<TeamSupport.CDI.linq.Action> actionsTable = db.GetTable<TeamSupport.CDI.linq.Action>();
                    Table<TicketSentiment> ticketSentimentsTable = db.GetTable<TicketSentiment>();
                    Table<User> userTable = db.GetTable<User>();
                    Table<TicketSeverity> severityTable = db.GetTable<TicketSeverity>();

                    var query = (from t in ticketsTable
                                 join tt in ticketTypesTable on t.TicketTypeID equals tt.TicketTypeID
                                 join ts in ticketStatusesTable on t.TicketStatusID equals ts.TicketStatusID
                                 where (t.DateCreated > _dateRange.StartDate) &&
                                     (!ts.IsClosed || (t.DateClosed.Value > t.DateCreated)) &&
                                     (t.TicketSource != "SalesForce") &&    // ignore imported tickets
                                     //(t.OrganizationID == 1078) &&
                                     (!tt.ExcludeFromCDI) &&
                                     (!ts.ExcludeFromCDI)
                                 select new TicketJoin()
                                 {
                                     OrganizationID = t.OrganizationID,
                                     DateClosed = t.DateClosed,
                                     DateCreated = t.DateCreated,
                                     IsClosed = ts.IsClosed,
                                     //CreatorID = t.CreatorID,
                                     ActionsCount = (from a in actionsTable where a.TicketID == t.TicketID select a.ActionID).Count(),
                                     AverageActionSentiment = (from tst in ticketSentimentsTable where t.TicketID == tst.TicketID select tst.AverageActionSentiment).First(),  // for some reason Min is faster than First()
                                     ClientOrganizationID = (from u in userTable where u.UserID == t.CreatorID select u.OrganizationID).First(),
                                     Severity = (from s in severityTable where t.TicketSeverityID == s.TicketSeverityID select s.Severity).First()
                                 });

                    // run the query
                    AllTickets = query.ToArray();
                }
            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("Ticket Read failed", e);
            }
        }

    }
}
