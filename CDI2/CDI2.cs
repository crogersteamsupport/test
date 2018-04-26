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
            int[] organizationIDs = _ticketReader.ReadOrganizationIDs();
            foreach (int organizationID in organizationIDs)
            {
                TicketJoin[] tickets = _ticketReader.Read(organizationID);
                if (tickets.Length < _dateRange.IntervalCount * 2)
                    continue;

                _organizations.Add(new Organization(organizationID, tickets, _dateRange));
            }

            //WriteCdiByOrganization();
            CDIEventLog.WriteEntry("CDI Update complete.");
        }

        /// <summary>
        /// Complete summary of CDI by date for all organizations
        /// </summary>
        //public void WriteCdiByOrganization()
        //{
        //    // DateTime org1 org2 org3...
        //    StringBuilder str = new StringBuilder();
        //    str.Append("DateTime");
        //    foreach (Organization organization in _organizations)
        //        str.Append("\t" + organization.OrganizationID);
        //    Debug.WriteLine(str.ToString());

        //    for (int i = 0; i < _analysisInterval.IntervalCount; ++i)
        //    {
        //        str.Clear();
        //        str.Append(_organizations.First().AnalysisIntervalData[i]._intervalEndTimeStamp.ToShortDateString() + "\t");
        //        foreach (Organization organization in _organizations)
        //        {
        //            str.Append(organization.AnalysisIntervalData[i].CDI.Value);
        //            str.Append('\t');
        //        }

        //        Debug.WriteLine(str.ToString());
        //    }
        //}

    }
}
