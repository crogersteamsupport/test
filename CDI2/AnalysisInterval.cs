using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDI2
{
    public class AnalysisInterval
    {
        public int IntervalCount { get; private set; }
        public TimeSpan Duration { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public AnalysisInterval(TimeSpan analysisInterval, int intervalCount)
        {
            IntervalCount = intervalCount;
            Duration = analysisInterval;
            EndDate = PreviousMidnight(DateTime.UtcNow);
            StartDate = PreviousMidnight(EndDate - TimeSpan.FromDays(analysisInterval.TotalDays * intervalCount));
        }

        DateTime PreviousMidnight(DateTime value)
        {
            return value.AddTicks(-(value.Ticks % Duration.Ticks));
        }
    }
}
