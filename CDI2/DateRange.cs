using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.CDI
{
    /// <summary>
    /// What time scale do we want to use to analyze the data
    /// </summary>
    public class DateRange
    {
        public static DateTime EndTime = DateTime.UtcNow;

        /// <summary>Count of intervals to analyze</summary>
        public int IntervalCount { get; private set; }
        /// <summary>time span of a single interval</summary>
        public TimeSpan IntervalTimeSpan { get; private set; }
        /// <summary>Only analyze tickets created after start date</summary>
        public DateTime StartDate { get; private set; }
        /// <summary>Only analyze tickets before end date</summary>
        public DateTime EndDate { get; private set; }

        public DateRange(TimeSpan analysisInterval, int intervalCount)
        {
            IntervalCount = intervalCount;
            IntervalTimeSpan = analysisInterval;
            EndDate = PreviousMidnight(EndTime);
            StartDate = PreviousMidnight(EndDate - TimeSpan.FromDays(analysisInterval.TotalDays * intervalCount));
        }

        /// <summary>Round to the previous midnight</summary>
        DateTime PreviousMidnight(DateTime value)
        {
            return value.AddTicks(-(value.Ticks % IntervalTimeSpan.Ticks));
        }
    }
}
