using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Interface to CDI strategy
    /// </summary>
    interface ICDIStrategy
    {
        bool CalculateCDI();
    }


    /// <summary>
    /// Generate CDI metrics for an individual organization
    /// </summary>
    class OrganizationAnalysis
    {
        public DateRange _dateRange { get; private set; }
        public TicketJoin[] Tickets { get; private set; }
        public int OrganizationID { get; private set; }
        IntervalStrategy _intervalStrategy;
        public List<IntervalData> Intervals { get; private set; }
        public int CreatorIDCount { get; private set; }
        public int TicketCount { get; private set; }

        public int? ClientOrganizationID
        {
            get { return (Tickets.Length > 0) ? Tickets[0].ClientOrganizationID : null; }
        }

        /// <summary>
        /// Create from subset of all tickets (faster than using linq to query)
        /// </summary>
        /// <param name="analysisInterval">date range and intervals to analyze</param>
        /// <param name="allTickets">complete set for date range, all organizations</param>
        /// <param name="startIndex">start index into all tickets for this organization</param>
        /// <param name="endIndex">end index into all tickets for this organization</param>
        public OrganizationAnalysis(DateRange analysisInterval, TicketJoin[] allTickets, int startIndex, int endIndex)
        {
            try
            {
                _dateRange = analysisInterval;

                // pull out the range for this organization
                TicketCount = endIndex - startIndex;
                Tickets = new TicketJoin[endIndex - startIndex];
                Array.Copy(allTickets, startIndex, Tickets, 0, Tickets.Length);
                OrganizationID = Tickets[0].OrganizationID;
                CreatorIDCount = Tickets.Select(t => t.CreatorID).Distinct().Count();

                // collect metrics for each interval
                _intervalStrategy = new IntervalStrategy(Tickets);
                Intervals = _intervalStrategy.GenerateIntervalData(_dateRange);
            }
            catch(Exception ex)
            {
                CDIEventLog.WriteEntry("New organization failed", ex);
            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1}({2})", OrganizationID, Intervals.Count, Tickets.Length);
        }

        public string CDIValuesToString()
        {
            Intervals.Sort((lhs, rhs) => lhs._timeStamp.CompareTo(rhs._timeStamp));
            int intervalIndex = 0;
            StringBuilder str = new StringBuilder();
            str.Append(OrganizationID);
            foreach (DateTime time in _dateRange)
            {
                str.Append("\t");
                if((intervalIndex < Intervals.Count()) && (Intervals[intervalIndex]._timeStamp == time))
                    str.Append(Intervals[intervalIndex++].CDI);
            }

            //if (intervalIndex != IntervalData.Count() - 1)
            //    Debugger.Break();

            return str.ToString();
        }
    }
}
