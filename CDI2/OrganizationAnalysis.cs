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
        void CalculateCDI();
    }


    /// <summary>
    /// Generate CDI metrics for an individual organization
    /// </summary>
    class OrganizationAnalysis
    {
        DateRange _dateRange;
        TicketJoin[] _tickets;
        public int OrganizationID { get; private set; }
        IntervalStrategy _intervalStrategy;
        public List<IntervalData> Intervals { get; private set; }

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
                _tickets = new TicketJoin[endIndex - startIndex];
                Array.Copy(allTickets, startIndex, _tickets, 0, _tickets.Length);
                OrganizationID = _tickets[0].OrganizationID;

                // collect metrics for each interval
                _intervalStrategy = new IntervalStrategy(_tickets);
                Intervals = _intervalStrategy.GenerateIntervalData(_dateRange);
            }
            catch(Exception ex)
            {
                CDIEventLog.WriteEntry("New organization failed", ex);
            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1}({2})", OrganizationID, Intervals.Count, _tickets.Length);
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
