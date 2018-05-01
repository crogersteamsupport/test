using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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
        HashSet<Organization> _organizations;  // raw analysis by organization

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="timeSpan">interval to sample and average across</param>
        /// <param name="intervalCount">Use an even number of intervals</param>
        public CDI2(TimeSpan timeSpan, int intervalCount)
        {
            _dateRange = new DateRange(timeSpan, intervalCount);
            _ticketReader = new TicketReader(_dateRange);
            _organizations = new HashSet<Organization>();
        }

        public void Run()
        {
            // load all the tickets
            CDIEventLog.WriteEntry("CDI Update started...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            _ticketReader.LoadAllTickets();
            CDIEventLog.WriteEntry(String.Format("{0} Tickets loaded {1} in {2:0.00} sec",
                _ticketReader.AllTickets.Length, _dateRange, stopwatch.ElapsedMilliseconds / 1000));

            //_organizations.Add(new Organization(_dateRange, _ticketReader.AllTickets, 0, _ticketReader.AllTickets.Length));
            //WriteCdiByOrganization();

            // analyze by organization
            stopwatch.Restart();
            LoadOrganizations();
            CDIEventLog.WriteEntry(String.Format("Organization analysis completed in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000));

            // Store the results
            WriteCdiByOrganization();
            CDIEventLog.WriteEntry("CDI Update complete.");
        }

        void LoadOrganizations()
        {
            TicketJoin[] allTickets = _ticketReader.AllTickets;
            Array.Sort(allTickets, (lhs, rhs) => lhs.OrganizationID.CompareTo(rhs.OrganizationID));

            // spin through each organization
            int startIndex = 0;
            int startId = allTickets[startIndex].OrganizationID;
            for (int i = 1; i < allTickets.Length; ++i)
            {
                if(allTickets[i].OrganizationID != startId)
                {
                    Organization org = new Organization(_dateRange, allTickets, startIndex, i);
                    _organizations.Add(org);
                    startIndex = i;
                    startId = allTickets[startIndex].OrganizationID;
                }
            }

            _organizations.Add(new Organization(_dateRange, allTickets, startIndex, allTickets.Length));
        }

        /// <summary>
        /// Complete summary of CDI by date for all organizations
        /// </summary>
        public void WriteCdiByOrganization()
        {
            // header
            StringBuilder str = new StringBuilder();
            str.Append("OrganizationID");
            foreach (DateTime time in _dateRange)
                str.Append("\t" + time.ToShortDateString());
            Debug.WriteLine(str.ToString());

            // by organization
            foreach (Organization organization in _organizations)
                Debug.WriteLine(organization.CDIValues());
        }

    }
}
