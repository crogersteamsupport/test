using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TeamSupport.CDI
{
    public enum Metrics
    {
        New,    // _newCount
        Open,   // _openCount
        DaysOpen,   // _medianDaysOpen
        TotalTickets,   // _totalTicketsCreated
        Closed, // _closedCount
        DaysToClose,    // _medianDaysToClose
        ActionCount,    // _averageActionCount
        Severity    // _averageSeverity
    };

    /// <summary>
    /// Use percentiles of the interval distribution
    /// </summary>
    public class IntervalPercentiles
    {
        Dictionary<Metrics, Percentile> _percentiles;

        public IntervalPercentiles(List<IntervalData> intervals)
        {
            _percentiles = new Dictionary<Metrics, Percentile>();

            _percentiles[Metrics.New] = new Percentile(intervals, x => x._newCount);
            _percentiles[Metrics.Open] = new Percentile(intervals, x => x._openCount);
            _percentiles[Metrics.DaysOpen] = new Percentile(intervals, x => x._medianDaysOpen);
            _percentiles[Metrics.TotalTickets] = new Percentile(intervals, x => x._totalTicketsCreated);
            _percentiles[Metrics.Closed] = new Percentile(intervals, x => x._closedCount);

            List<IntervalData> tmp = intervals.Where(m => m._medianDaysToClose.HasValue).ToList();
            _percentiles[Metrics.DaysToClose] = new Percentile(tmp, t => t._medianDaysToClose.Value);

            tmp = intervals.Where(t => t._averageActionCount.HasValue).ToList();
            _percentiles[Metrics.ActionCount] = new Percentile(tmp, x => x._averageActionCount.Value);

            tmp = intervals.Where(t => t._averageSeverity.HasValue).ToList();
            _percentiles[Metrics.Severity] = new Percentile(tmp, x => x._averageSeverity.Value);
        }

        public bool CDI(IntervalData interval, linq.CDI_Settings weights, ICDIStrategy iCDIStrategy)
        {
            // Create the CDI from the normalized fields
            IntervalData normalized = Normalize(interval);
            interval.CDI = iCDIStrategy.GetCDI(interval, normalized, weights, _percentiles);
            return true;
        }

        public IntervalData Normalize(IntervalData interval)
        {
            IntervalData normalized = new IntervalData()
            {
                _timeStamp = interval._timeStamp,
                _newCount = _percentiles[Metrics.New].AsPercentile(interval._newCount),  // 2. Tickets Created in Last 30
                _openCount = _percentiles[Metrics.Open].AsPercentile(interval._openCount),   // 3. Number of Tickets Currently Open
                _medianDaysOpen = _percentiles[Metrics.DaysOpen].AsPercentile(interval._medianDaysOpen),    // 4. Average Time Tickets have been open
                _totalTicketsCreated = _percentiles[Metrics.TotalTickets].AsPercentile(interval._totalTicketsCreated),  // 1. Total Tickets Created
                _closedCount = _percentiles[Metrics.Closed].AsPercentile(interval._closedCount),
            };

            if ((_percentiles[Metrics.DaysToClose] != null) && interval._medianDaysToClose.HasValue)
                normalized._medianDaysToClose = _percentiles[Metrics.DaysToClose].AsPercentile(interval._medianDaysToClose.Value);  // 5. Average Time tickets took to close

            if ((_percentiles[Metrics.ActionCount] != null) && interval._averageActionCount.HasValue)
                normalized._averageActionCount = _percentiles[Metrics.ActionCount].AsPercentile(interval._averageActionCount.Value);

            if ((_percentiles[Metrics.Severity] != null) && interval._averageSeverity.HasValue)
                normalized._averageSeverity = _percentiles[Metrics.Severity].AsPercentile(interval._averageSeverity.Value);

            //if (interval._averageSentimentScore.HasValue)
            //    normalized._averageSentimentScore = interval._averageSentimentScore.Value / 10; // [0, 1000] => [0, 100]

            return normalized;
        }

    }

}
