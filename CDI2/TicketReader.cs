using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Linq;
using System.IO;

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
        public TicketJoin[] AllTickets { get; private set; }  // tickets for organization in the last year

        /// <summary>Time frame to analyze the ticket data</summary>
        /// <param name="analysisInterval"></param>
        public TicketReader(DateRange analysisInterval)
        {
            _dateRange = analysisInterval;
            AllTickets = null;
        }

        /// <summary> Load the tickets since the start date </summary>
        public void LoadAllTickets()
        {
            if (AllTickets != null)
                return;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    AllTickets = LoadAllTicketsHelper(db);
                }
            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("Ticket Read failed", e);
            }
            long ellapsed = stopwatch.ElapsedMilliseconds;
        }

        /// <summary>Extract the tickets for this organization</summary>
        /// <param name="organizationID"></param>
        /// <returns></returns>
        public TicketJoin[] Read(int organizationID)
        {
            LoadAllTickets(); // make sure we are initilized
            var query = AllTickets.Where(t => t.OrganizationID == organizationID);//.OrderBy(t => t.DateCreated);
            return query.ToArray();
        }

        public int[] ReadOrganizationIDs()
        {
            LoadAllTickets(); // make sure we are initilized
            var query = AllTickets.Select(t => t.OrganizationID).Distinct();
            return query.ToArray();
        }

        /// <summary> big data - Load the tickets in pages </summary>
        /// <param name="startDate"></param>
        /// <param name="db"></param>
        TicketJoin[] LoadAllTicketsHelper(DataContext db)
        {
            TicketJoin[] tickets = null;

            // tables
            Table<Ticket> ticketsTable = db.GetTable<Ticket>();
            Table<TicketStatus> ticketStatusesTable = db.GetTable<TicketStatus>();
            Table<TicketType> ticketTypesTable = db.GetTable<TicketType>();
            Table<Action> actionsTable = db.GetTable<Action>();

            // loop through loading blocks
            DateTime queryDate = _dateRange.StartDate;
            while (queryDate < _dateRange.EndDate)
            {
                var query = (from t in ticketsTable
                             join tt in ticketTypesTable on t.TicketTypeID equals tt.TicketTypeID
                             join ts in ticketStatusesTable on t.TicketStatusID equals ts.TicketStatusID
                             join a in actionsTable on t.TicketID equals a.TicketID into actionJoin // count actions
                             where (t.DateCreated > queryDate) &&
                                 //(ts.IsClosed == true) &&   // only closed tickets
                                 //(t.DateClosed.HasValue) &&
                                 //(t.DateClosed.Value > t.DateCreated.AddSeconds(1)) &&  // ignore those open for less than a second
                                 ((ts.IsClosed == false) || ((t.DateClosed.HasValue) && (t.DateClosed.Value > t.DateCreated.AddSeconds(1)))) && // open for more than 1 second
                                 (t.TicketSource != "SalesForce") &&    // ignore imported tickets
                                 (!tt.ExcludeFromCDI) && 
                                 (!ts.ExcludeFromCDI)
                             orderby t.DateCreated
                             select new TicketJoin()
                             {
                                 TicketID = t.TicketID,
                                 TicketStatusName = ts.Name,
                                 TicketTypeName = tt.Name,
                                 OrganizationID = t.OrganizationID,
                                 DateClosed = t.DateClosed,
                                 TicketSource = t.TicketSource,
                                 DateCreated = t.DateCreated,
                                 ActionsCount = actionJoin.Count(),
                                 IsClosed = ts.IsClosed
                             }).Distinct();

                // execute the query into the array
                TicketJoin[] queryResults = query.ToArray();
                if (queryResults.Length == 0)
                    break;  // done - no more records

                if (tickets == null)
                    tickets = queryResults;    // first call
                else
                {
                    int previousLength = tickets.Length;
                    Array.Resize(ref tickets, tickets.Length + queryResults.Length);
                    queryResults.CopyTo(tickets, previousLength);
                }
                queryDate = tickets[tickets.Length - 1].DateCreated;
            }
            return tickets;
        }
    }
}
