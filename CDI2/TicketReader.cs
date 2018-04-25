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

namespace CDI2
{
    /// <summary>
    /// Load the tickets from the database
    /// </summary>
    class TicketReader
    {
        DateTime _startDate;
        DateTime _endDate;
        public TicketJoin[] Tickets { get; private set; }  // tickets for organization in the last year

        /// <summary> Constructor </summary>
        /// <param name="daysToLoad">How many days prior to today do we load?</param>
        public TicketReader(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate = endDate;
            Tickets = null;
        }

        /// <summary> Load the tickets since the start date </summary>
        public void LoadTickets()
        {
            if (Tickets != null)
                return;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    Tickets = LoadTicketsHelper(db);
                }
            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("Ticket Read failed", e);
            }
            long ellapsed = stopwatch.ElapsedMilliseconds;
        }

        public TicketJoin[] Read(int organizationID)
        {
            LoadTickets(); // make sure we are initilized
            var query = Tickets.Where(t => t.OrganizationID == organizationID);//.OrderBy(t => t.DateCreated);
            return query.ToArray();
        }

        public int[] ReadOrganizationIDs()
        {
            LoadTickets(); // make sure we are initilized
            var query = Tickets.Select(t => t.OrganizationID).Distinct();
            return query.ToArray();
        }

        /// <summary> big data - Load the tickets in pages </summary>
        /// <param name="startDate"></param>
        /// <param name="db"></param>
        TicketJoin[] LoadTicketsHelper(DataContext db)
        {
            TicketJoin[] tickets = null;

            // tables
            Table<Ticket> ticketsTable = db.GetTable<Ticket>();
            Table<TicketStatus> ticketStatusesTable = db.GetTable<TicketStatus>();
            Table<TicketType> ticketTypesTable = db.GetTable<TicketType>();
            Table<Action> actionsTable = db.GetTable<Action>();

            // loop through loading blocks
            DateTime queryDate = _startDate;
            while (queryDate < _endDate)
            {
                var query = (from t in ticketsTable
                             join tt in ticketTypesTable on t.TicketTypeID equals tt.TicketTypeID
                             join ts in ticketStatusesTable on t.TicketStatusID equals ts.TicketStatusID
                             join a in actionsTable on t.TicketID equals a.TicketID into actionJoin // count actions
                             where (t.DateCreated > queryDate) &&
                                 (ts.IsClosed == true) &&   // only closed tickets
                                 (t.DateClosed.HasValue) &&
                                 (t.DateClosed.Value > t.DateCreated.AddSeconds(1)) &&  // ignore those open for less than a second
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
                                 ActionCount = actionJoin.Count(),
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
