using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Use percentiles of the interval distribution
    /// </summary>
    public class CDIPercentileStrategy : ICDIStrategy
    {
        Percentile<int> _newCount;  // new (this interval)
        Percentile<int> _openCount; // currently Open (since startDate)
        Percentile<double> _medianDaysOpen;   // days open of currently open
        Percentile<int> _closedCount;   // closed (this interval)
        Percentile<double> _daysToClose;    // average time to close (this interval)
        Percentile<double> _averageActionCount;   // actions per ticket

        public CDIPercentileStrategy(List<IntervalData> intervalData)
        {
            // counts
            _newCount = new Percentile<int>(intervalData, delegate (IntervalData x) { return x._newCount; });
            _openCount = new Percentile<int>(intervalData, delegate (IntervalData x) { return x._openCount; });
            _closedCount = new Percentile<int>(intervalData, delegate (IntervalData x) { return x._closedCount; });

            // open tickets
            _medianDaysOpen = new Percentile<double>(intervalData, delegate (IntervalData x) { return x._medianDaysOpen; });

            // closed tickets
            List<IntervalData> closedTickets = intervalData.Where(t => t._closedCount > 0).ToList();
            if (closedTickets.Count > 0)
            {
                _averageActionCount = new Percentile<double>(closedTickets, delegate (IntervalData x) { return x._averageActionCount.Value; });
                _daysToClose = new Percentile<double>(closedTickets, delegate (IntervalData x) { return x._medianDaysToClose.Value; });
            }
        }

        // TODO
        //  * time to respond (first response from customer service)
        //  * count of actions (long ticket = bad)
        public void CalculateCDI(IntervalData intervalData)
        {
            List<double> metrics = new List<double>();
            metrics.Add(_newCount.AsPercentile(intervalData._newCount));
            metrics.Add(_openCount.AsPercentile(intervalData._openCount));
            metrics.Add(100 - _closedCount.AsPercentile(intervalData._closedCount));    // the higher the better
            metrics.Add(_medianDaysOpen.AsPercentile(intervalData._medianDaysOpen));

            // closed tickets?
            if (intervalData._medianDaysToClose.HasValue)
                metrics.Add(_daysToClose.AsPercentile(intervalData._medianDaysToClose.Value));
            if (intervalData._averageActionCount.HasValue)
                metrics.Add(_averageActionCount.AsPercentile(intervalData._averageActionCount.Value));

            intervalData.CDI = (int)Math.Round(metrics.Average() * 10);  // [0, 1000]
        }
    }

}
