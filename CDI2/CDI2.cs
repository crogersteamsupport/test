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

        void Log(string text)
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
            Log(String.Format("{0} Tickets loaded {1} in {2:0.00} sec",
                 _ticketReader.AllTickets.Length, _dateRange, stopwatch.ElapsedMilliseconds / 1000));


            // analyze by organization
            stopwatch.Restart();
            LoadCustomers();
            //EntireDatabase();
            Log(String.Format("Customers and Clients loaded in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000));

            // analyze by organization
            stopwatch.Restart();
            GenerateIntervals();
            Log(String.Format("Interval data generated in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000));

            // Do the dirty work
            stopwatch.Restart();
            InvokeCDIStrategy();
            Log(String.Format("Client CDI generated in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000));

            // test output
            for (int i = 0; i < args.Length; ++i)
            {
                string arg = args[i];
                Console.WriteLine(arg);
                switch (arg)
                {
                    case "customer":
                        {
                            int organizationID = int.Parse(args[++i]);
                            Customer customer = _customers.Where(c => c.OrganizationID == organizationID).FirstOrDefault();
                            if (customer != null)
                                customer.WriteCdiByOrganization();
                            else
                                Console.WriteLine("Customer ID not found");
                        }
                        break;
                    case "customerIntervals":
                        {
                            int organizationID = int.Parse(args[++i]);
                            Customer customer = _customers.Where(c => c.OrganizationID == organizationID).FirstOrDefault();
                            if (customer != null)
                                customer.WriteIntervals();
                            else
                                Console.WriteLine("Customer ID not found");
                        }
                        break;
                }
            }

            // Store the results
            //stopwatch.Restart();
            //CDIEventLog.WriteEntry(String.Format("Results saved. {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000));
            CDIEventLog.WriteEntry(String.Format("CDI Update complete"));
        }

        public void EntireDatabase()
        {
            OrganizationAnalysis all = new OrganizationAnalysis(_dateRange, _ticketReader.AllTickets, 0, _ticketReader.AllTickets.Length);
            all.GenerateIntervals();
            IntervalData.WriteHeader();
            foreach (IntervalData interval in all.Intervals)
                Console.WriteLine(interval.ToString());
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
            Customer customer = _customers.Where(c => c.OrganizationID == customerID).FirstOrDefault();
            if(customer != null)
                customer.WriteIntervals(clientID);
        }
    }
}
