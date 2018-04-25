using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CDI2
{
    public class CDIEngine
    {
        AnalysisInterval _interval;
        TicketReader _ticketReader;    // cache the tickets for analysis
        HashSet<Organization> _organizations;  // raw analysis by organization

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="timeSpan">interval to sample and average across</param>
        /// <param name="intervalCount">Use an even number of intervals</param>
        public CDIEngine(TimeSpan timeSpan, int intervalCount)
        {
            _interval = new AnalysisInterval(timeSpan, intervalCount);
            _ticketReader = new TicketReader(_interval.StartDate, _interval.EndDate);
            _organizations = new HashSet<Organization>();
        }

        public void Run()
        {
            // load all the tickets
            CDIEventLog.WriteEntry("CDI Update started...");
            _ticketReader.LoadTickets();

            // analyze by organization
            int[] organizationIDs = _ticketReader.ReadOrganizationIDs();
            foreach (int organizationID in organizationIDs)
            {
                TicketJoin[] tickets = _ticketReader.Read(organizationID);
                if (tickets.Length < _interval.IntervalCount * 2)
                    continue;

                _organizations.Add(new Organization(tickets, organizationID, _interval));
            }

            WriteCdiByOrganization();
            CDIEventLog.WriteEntry("CDI Update complete.");
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

            for (int i = 0; i < _interval.IntervalCount; ++i)
            {
                str.Clear();
                str.Append(_organizations.First().AnalysisIntervalData[i]._timeStamp.ToShortDateString() + "\t");
                foreach (Organization organization in _organizations)
                {
                    str.Append(organization.AnalysisIntervalData[i].CDI.Value);
                    str.Append('\t');
                }

                Debug.WriteLine(str.ToString());
            }
        }

    }
}
