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
        public bool IsClosed { get; private set; }
        public int DaysToLoad { get; private set; }
        private Ticket[] _tickets;  // tickets for organization in the last year

        const int _TicketBlockSize = 500000; // load in blocks?

        /// <summary> Constructor </summary>
        /// <param name="daysToLoad">How many days prior to today do we load?</param>
        /// <param name="isClosed">Load only open tickets or closed tickets</param>
        public TicketReader(int daysToLoad, bool isClosed)
        {
            DaysToLoad = daysToLoad;
            IsClosed = isClosed;
        }

        /// <summary> Load the tickets since the start date </summary>
        public Ticket[] Read()
        {
            if (_tickets != null)
                return _tickets;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    _tickets = LoadFromDatabase(db);
                }
            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("Ticket Read failed", e);
            }

            long ellapsed = stopwatch.ElapsedMilliseconds;

            return _tickets;
        }

        public Ticket[] Read(int organizationID)
        {
            Ticket[] tickets = Read();
            var query = tickets.Where(t => t.OrganizationID == organizationID).OrderBy(t => t.DateCreated);
            return query.ToArray();
        }

        public int[] ReadOrganizationIDs()
        {
            var query = _tickets.Select(t => t.OrganizationID).Distinct();
            return query.ToArray();
        }

        /// <summary> big data - Load the tickets in pages </summary>
        /// <param name="startDate"></param>
        /// <param name="db"></param>
        Ticket[] LoadFromDatabase(DataContext db)
        {
            Ticket[] tickets = null;

            // tables
            Table<Ticket> ticketTable = db.GetTable<Ticket>();
            Table<TicketStatus> ticketStatusTable = db.GetTable<TicketStatus>();
            Table<TicketType> ticketTypeTable = db.GetTable<TicketType>();

            // loop through loading blocks
            DateTime now = DateTime.UtcNow;
            DateTime startDate = now.AddDays(-1 * DaysToLoad);
            while (startDate < now)
            {
                var query = (from t in ticketTable
                             join tt in ticketTypeTable on t.TicketTypeID equals tt.TicketTypeID
                             join ts in ticketStatusTable on t.TicketStatusID equals ts.TicketStatusID
                             where (t.DateCreated > startDate) &&
                                 (t.DateClosed.HasValue) &&
                                 (t.DateCreated != t.DateClosed.Value) &&
                                 (ts.IsClosed == IsClosed) &&
                                 (!tt.ExcludeFromCDI) &&
                                 (!ts.ExcludeFromCDI)
                             orderby t.DateCreated
                             select t).Take(_TicketBlockSize);

                // execute the query into the array
                Ticket[] queryResults = query.ToArray();
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
                startDate = tickets[tickets.Length - 1].DateCreated;
            }
            return tickets;
        }
    }
}
