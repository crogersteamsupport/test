using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TeamSupport.CDI.linq;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Data analysis engine 
    ///     load tickets
    ///     collect metrics across time intervals
    ///     calculate CDI based on distribution of time intervals (worse than usual?)
    ///     detect trends
    /// </summary>
    public class CDI2
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
        public CDI2(TimeSpan timeSpan, int intervalCount)
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
            if (args.Contains("verbose"))
                _verboseLog = true;
            CDIEventLog.WriteEntry("CDI Update started...");

            // load all the tickets
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            _ticketReader.LoadAllTickets();
            VerboseLog(String.Format("{0} Tickets loaded {1} in {2:0.00} sec",
                 _ticketReader.AllTickets.Length, _dateRange, stopwatch.ElapsedMilliseconds / 1000));

            // analyze by organization
            stopwatch.Restart();
            LoadCustomers();
            //EntireDatabase();
            VerboseLog(String.Format("Customers and Clients loaded in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000));

            // generate the interval data
            stopwatch.Restart();
            GenerateIntervals();
            VerboseLog(String.Format("Interval data generated in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000));

            // Do the CDI dirty work
            stopwatch.Restart();
            InvokeCDIStrategy();
            VerboseLog(String.Format("Client CDI generated in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000));

            // test output
            for (int i = 0; i < args.Length; ++i)
            {
                string arg = args[i];
                CDIEventLog.WriteLine(arg);
                switch (arg)
                {
                    case "customer":
                        {
                            Customer customer = GetCustomer(int.Parse(args[++i]));
                            if (customer != null)
                                customer.WriteCdiByOrganization();
                        }
                        break;
                    case "customerIntervals":
                        {
                            Customer customer = GetCustomer(int.Parse(args[++i]));
                            if (customer != null)
                                customer.WriteIntervals();
                        }
                        break;
                    case "clients":
                        {
                            Customer customer = GetCustomer(int.Parse(args[++i]));
                            if (customer != null)
                                customer.WriteClients();
                        }
                        break;
                }
            }

            // Store the results
            //stopwatch.Restart();
            //CDIEventLog.WriteEntry(String.Format("Results saved. {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000));
            CDIEventLog.WriteEntry(String.Format("CDI Update complete"));
        }

        Customer GetCustomer(int id)
        {
            Customer customer = _customers.Where(c => c._organizationAnalysis.OrganizationID == id).FirstOrDefault();
            if (customer == null)
                CDIEventLog.WriteLine("Customer ID not found");

            return customer;
        }

        public void EntireDatabase()
        {
            OrganizationAnalysis all = new OrganizationAnalysis(_dateRange, _ticketReader.AllTickets, 0, _ticketReader.AllTickets.Length);
            all.GenerateIntervals();
            IntervalData.WriteHeader();
            foreach (IntervalData interval in all.Intervals)
                CDIEventLog.WriteLine(interval.ToString());
        }

        void LoadCustomers()
        {
            TicketJoin[] allTickets = _ticketReader.AllTickets;
            Array.Sort(allTickets, (lhs, rhs) => lhs.OrganizationID.CompareTo(rhs.OrganizationID));
            _customers = new HashSet<Customer>();

            // spin through each organization
            int startIndex = 0;
            int startId = allTickets[startIndex].OrganizationID;
            for (int i = 1; i < allTickets.Length; ++i)
            {
                if(allTickets[i].OrganizationID != startId)
                {
                    _customers.Add(new Customer(new OrganizationAnalysis(_dateRange, allTickets, startIndex, i)));
                    startIndex = i;
                    startId = allTickets[startIndex].OrganizationID;
                }
            }

            _customers.Add(new Customer(new OrganizationAnalysis(_dateRange, allTickets, startIndex, allTickets.Length)));
        }

        public void GenerateIntervals()
        {
            foreach (Customer customer in _customers)
                customer.GenerateIntervals();
        }

        void InvokeCDIStrategy()
        {
            //IntervalData.WriteHeader();
            foreach (Customer customer in _customers)
                customer.InvokeCDIStrategy();
        }

        /// <summary>
        /// Complete summary of CDI by date for all organizations
        /// </summary>
        public void WriteCdiByOrganization()
        {
            foreach (Customer customer in _customers)
                customer.WriteCdiByOrganization();
        }

        public void WriteIntervals(int customerID, int clientID)
        {
            Customer customer = _customers.Where(c => c._organizationAnalysis.OrganizationID == customerID).FirstOrDefault();
            if(customer != null)
                customer.WriteIntervals(clientID);
        }
    }
}
