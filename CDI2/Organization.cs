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
        void CalculateCDI(IntervalData intervalData);
    }


    /// <summary>
    /// Generate CDI metrics for an individual organization
    /// </summary>
    class Organization
    {
        DateRange _dateRange;
        TicketJoin[] _tickets;
        public int OrganizationID { get; private set; }
        IntervalStrategy _intervalStrategy;
        public List<IntervalData> IntervalData { get; private set; }
        ICDIStrategy _cdiStrategy;

        /// <summary>
        /// Create from subset of all tickets (faster than using linq to query)
        /// </summary>
        /// <param name="analysisInterval">date range and intervals to analyze</param>
        /// <param name="allTickets">complete set for date range, all organizations</param>
        /// <param name="startIndex">start index into all tickets for this organization</param>
        /// <param name="endIndex">end index into all tickets for this organization</param>
        public Organization(DateRange analysisInterval, TicketJoin[] allTickets, int startIndex, int endIndex)
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
                IntervalData = _intervalStrategy.GenerateIntervalData(_dateRange);

                //Debug.WriteLine("Date\tNew\tOpen\tClosed\tDaysOpen\tDaysToClose\tActions\tSentiment\tCDI");
                CalculateCDIRollingYear();
            }
            catch(Exception ex)
            {
                CDIEventLog.WriteEntry("New organization failed", ex);
            }
        }

        /// <summary>calculate the CDI normalized by a single percentile lookup</summary>
        void CalculateCDIFullTimeline()
        {
            _cdiStrategy = new CDIPercentileStrategy(IntervalData);
            IntervalData.Sort((lhs, rhs) => lhs._timeStamp.CompareTo(rhs._timeStamp));
            foreach (IntervalData intervalData in IntervalData)
                _cdiStrategy.CalculateCDI(intervalData);
        }

        /// <summary>calculate the CDI using a rolling percentile lookup</summary>
        void CalculateCDIRollingYear()
        {
            IntervalData.Sort((lhs, rhs) => lhs._timeStamp.CompareTo(rhs._timeStamp));
            DateTime begin = IntervalData.First()._timeStamp;
            DateTime end = begin.AddDays(365);  // 1 year rolling average

            // get a rolling year into the queue
            List<IntervalData> rollingYear = IntervalData.Where(t => t._timeStamp < end).ToList();
            _cdiStrategy = new CDIPercentileStrategy(rollingYear);  // mixes up rollingYear...
            foreach (IntervalData intervalData in IntervalData)
            {
                // rolling year for percentile
                bool update = false;
                rollingYear.Sort((lhs, rhs) => lhs._timeStamp.CompareTo(rhs._timeStamp));
                if (intervalData._timeStamp > rollingYear.Last()._timeStamp)
                {
                    rollingYear.Add(intervalData);  // add future data
                    update = true;
                }
                while (rollingYear.First()._timeStamp < intervalData._timeStamp.AddDays(-365))
                {
                    rollingYear.RemoveAt(0);    // drop old data
                    update = true;
                }

                if(update)
                    _cdiStrategy = new CDIPercentileStrategy(rollingYear);  // mixes up rollingYear...
                _cdiStrategy.CalculateCDI(intervalData);
            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1}({2})", OrganizationID, IntervalData.Count, _tickets.Length);
        }

        public string CDIValues()
        {
            IntervalData.Sort((lhs, rhs) => lhs._timeStamp.CompareTo(rhs._timeStamp));
            int intervalIndex = 0;
            StringBuilder str = new StringBuilder();
            str.Append(OrganizationID);
            foreach (DateTime time in _dateRange)
            {
                str.Append("\t");
                if((intervalIndex < IntervalData.Count()) && (IntervalData[intervalIndex]._timeStamp == time))
                    str.Append(IntervalData[intervalIndex++].CDI);
            }

            //if (intervalIndex != IntervalData.Count() - 1)
            //    Debugger.Break();

            return str.ToString();
        }
    }
}
