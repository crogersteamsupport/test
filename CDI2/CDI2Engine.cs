using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Configuration;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Data analysis engine 
    ///     load tickets
    ///     collect metrics across time intervals
    ///     calculate CDI based on distribution of time intervals (worse than usual?)
    ///     detect trends
    /// </summary>
    public class CDI2Engine
    {
        DateRange _dateRange;
        TicketReader _ticketReader;    // cache the tickets for analysis
        HashSet<Customer> _customers;  // raw analysis by organization
        bool _verboseLog;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="timeSpan">interval to sample and average across</param>
        /// <param name="intervalCount">Use an even number of intervals</param>
        public CDI2Engine(TimeSpan timeSpan, int intervalCount)
        {
            _dateRange = new DateRange(timeSpan, intervalCount);
            _ticketReader = new TicketReader(_dateRange);
            _verboseLog = false;
        }

        void VerboseLog(string text)
        {
            if (_verboseLog)
                CDIEventLog.WriteEntry(text);
        }

        public void Run(string[] args)
        {
            CDIEventLog.WriteEntry("CDI Update started...");
            if (args.Contains("verbose"))
                _verboseLog = true;

            // load all the tickets
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (args.Contains("ForceCDIUpdate"))
                _ticketReader.LoadNeedComputeOrganizationTickets();
            else
                _ticketReader.LoadAllTickets();

            VerboseLog(String.Format("{0} Tickets loaded {1} in {2:0.00} sec",
                 _ticketReader.AllTickets.Length, _dateRange, stopwatch.ElapsedMilliseconds / 1000));

            // analyze by organization
            stopwatch.Restart();
            LoadCustomers();
            VerboseLog(String.Format("Customers and Clients loaded in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000));

            // generate the interval data
            stopwatch.Restart();
            foreach (Customer customer in _customers)
                customer.GenerateIntervals();
            VerboseLog(String.Format("Interval data generated in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000));

            // Do the CDI dirty work
            stopwatch.Restart();
            foreach (Customer customer in _customers)
                customer.InvokeCDIStrategy();
            CDIEventLog.WriteEntry(String.Format("Client CDI generated in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000));

            // Save
            CDIEventLog.WriteEntry("Saving CDI data...");
            stopwatch.Restart();
            //SaveToCDITable();
            SaveToOrganizationsTable();
            VerboseLog(String.Format("Client CDI Saved in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000));

            //CDIEventLog.WriteEntry(String.Format("CDI complete"));
            DumpTestResults(args);
        }

        /// <summary>
        /// ParentID = Customer, OrganizationID = Client
        /// </summary>
        void LoadCustomers()
        {
            TicketJoin[] allTickets = _ticketReader.AllTickets;
            Array.Sort(allTickets, (lhs, rhs) => lhs.ParentID.Value.CompareTo(rhs.ParentID.Value));
            _customers = new HashSet<Customer>();

            // spin through each organization
            int startIndex = 0;
            int startId = allTickets[startIndex].ParentID.Value;
            for (int i = 1; i < allTickets.Length; ++i)
            {
                if(allTickets[i].ParentID != startId)
                {
                    _customers.Add(new Customer(new OrganizationAnalysis(_dateRange, allTickets, startIndex, i)));
                    startIndex = i;
                    startId = allTickets[startIndex].ParentID.Value;
                }
            }

            _customers.Add(new Customer(new OrganizationAnalysis(_dateRange, allTickets, startIndex, allTickets.Length)));
        }

        public void SaveToCDITable()
        {
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    Table<linq.CDI> table = db.GetTable<linq.CDI>();
                    linq.CDI[] CDIs = Enumerable.ToArray(table);
                    Dictionary<int, linq.CDI> lookup = new Dictionary<int, linq.CDI>();
                    foreach (linq.CDI cdi in CDIs)
                        lookup[cdi.OrganizationID] = cdi;

                    foreach (Customer customer in _customers)
                        customer.Save(lookup, table);
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("Save failed", e);
            }
        }

        public void SaveToOrganizationsTable()
        {
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    Table<linq.Organization> table = db.GetTable<linq.Organization>();
                    linq.Organization[] organizations = Enumerable.ToArray(table);
                    Dictionary<int, linq.Organization> lookup = new Dictionary<int, linq.Organization>();
                    foreach (linq.Organization organization in organizations)
                        lookup[organization.OrganizationID] = organization;

                    foreach (Customer customer in _customers)
                        customer.Save(lookup, table);
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("Save failed", e);
            }
        }


        bool TryGetCustomer(int id, out Customer customer)
        {
            customer = _customers.Where(c => c.OrganizationID == id).FirstOrDefault();
            return customer != null;
        }

        void DumpTestResults(string[] args)
        {
            // test output
            Customer customer;
            for (int i = 0; i < args.Length; ++i)
            {
                string arg = args[i];
                CDIEventLog.WriteLine(arg);
                switch (arg)
                {
                    case "customer":
                        if (TryGetCustomer(int.Parse(args[++i]), out customer))
                            customer.WriteCdiByOrganization();
                        break;
                    case "customerIntervals":
                        if (TryGetCustomer(int.Parse(args[++i]), out customer))
                            customer.WriteIntervals();
                        break;
                    case "clients":
                        if (TryGetCustomer(int.Parse(args[++i]), out customer))
                            customer.WriteClients();
                        break;
                }
            }
        }
    }
}
