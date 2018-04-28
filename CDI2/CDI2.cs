using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TeamSupport.CDI
{
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
            _ticketReader.LoadAllTickets();

            // analyze by organization
            LoadOrganizations();
            WriteCdiByOrganization();
            CDIEventLog.WriteEntry("CDI Update complete.");
        }

        void LoadOrganizations()
        {
            TicketJoin[] allTickets = _ticketReader.AllTickets;

            // already sorted
            //Array.Sort(allTickets, (lhs, rhs) => lhs.OrganizationID.CompareTo(rhs.OrganizationID));

            // spin through each organization
            int startIndex = 0;
            int startId = allTickets[startIndex].OrganizationID;
            int count = 0;
            for (int i = 1; i < allTickets.Length; ++i)
            {
                if(allTickets[i].OrganizationID != startId)
                {
                    // insufficient tickets to get good metrics?
                    if (i - startIndex > _dateRange.IntervalCount * 2)
                        _organizations.Add(new Organization(_dateRange, allTickets, startIndex, i));
                    else
                        ++count;
                    startIndex = i;
                    startId = allTickets[startIndex].OrganizationID;
                }
            }

            if (allTickets.Length - startIndex > _dateRange.IntervalCount * 2)
                _organizations.Add(new Organization(_dateRange, allTickets, startIndex, allTickets.Length));
        }

        /// <summary>
        /// Complete summary of CDI by date for all organizations
        /// </summary>
        public void WriteCdiByOrganization()
        {
            // DateTime org1 org2 org3...
            StringBuilder str = new StringBuilder();
            str.Append("DateTime");
            foreach (Organization organization in _organizations)
                str.Append("\t" + organization.OrganizationID);
            Debug.WriteLine(str.ToString());

            for (int i = 0; i < _dateRange.IntervalCount; ++i)
            {
                str.Clear();
                str.Append(_organizations.First().IntervalData[i]._intervalEndTimeStamp.ToShortDateString() + "\t");
                foreach (Organization organization in _organizations)
                {
                    if(organization.IntervalData[i].CDI.HasValue)
                        str.Append(organization.IntervalData[i].CDI.Value);
                    str.Append('\t');
                }

                Debug.WriteLine(str.ToString());
            }
        }

    }
}
