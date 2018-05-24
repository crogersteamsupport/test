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
            VerboseLog(String.Format("Client CDI generated in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000));

            // Save
            stopwatch.Restart();
            Save();
            VerboseLog(String.Format("Client CDI Saved in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000));

            CDIEventLog.WriteEntry(String.Format("CDI Update complete"));
            DumpTestResults(args);
        }

        /// <summary>
        /// ParentID = Customer, OrganizationID = Client
        /// </summary>
        void LoadCustomers()
        {
            TicketJoin[] allTickets = _ticketReader.AllTickets;
            Array.Sort(allTickets, (lhs, rhs) => lhs.ParentID.CompareTo(rhs.ParentID));
            _customers = new HashSet<Customer>();

            // spin through each organization
            int startIndex = 0;
            int startId = allTickets[startIndex].ParentID;
            for (int i = 1; i < allTickets.Length; ++i)
            {
                if(allTickets[i].ParentID != startId)
                {
                    _customers.Add(new Customer(new OrganizationAnalysis(_dateRange, allTickets, startIndex, i)));
                    startIndex = i;
                    startId = allTickets[startIndex].ParentID;
                }
            }

            _customers.Add(new Customer(new OrganizationAnalysis(_dateRange, allTickets, startIndex, allTickets.Length)));
        }

        public void Save()
        {
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    Table<linq.CDI> table = db.GetTable<linq.CDI>();
                    linq.CDI[] stuff = Enumerable.ToArray(table);
                    foreach (Customer customer in _customers)
                        customer.Save(stuff.Where(c => c.ParentID == customer.OrganizationID).ToArray(), table);
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
