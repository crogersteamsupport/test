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
    public class IntervalPercentiles
    {
        Percentiles _newCountPercentiles;  // new (this interval)
        Percentiles _openCountPercentiles; // currently Open (since startDate)
        Percentiles _medianDaysOpenPercentiles;   // days open of currently open
        Percentiles _totalTicketsCreatedPercentiles;   // total tickets created across all the intervals
        Percentiles _closedCountPercentiles;   // closed (this interval)
        Percentiles _medianDaysToClosePercentiles;    // average time to close (this interval)
        Percentiles _averageActionCountPercentiles;   // actions per ticket
        Percentiles _averageSeverityPercentiles;   // ticket severity

        public IntervalPercentiles(List<IntervalData> intervals)
        {
            // counts
            _newCountPercentiles = new Percentiles(intervals, x => x._newCount);
            _openCountPercentiles = new Percentiles(intervals, x => x._openCount);
            _closedCountPercentiles = new Percentiles(intervals, x => x._closedCount);

            // open tickets
            _medianDaysOpenPercentiles = new Percentiles(intervals, x => x._medianDaysOpen);
            _totalTicketsCreatedPercentiles = new Percentiles(intervals, x => x._totalTicketsCreated);

            // closed tickets
            List<IntervalData> closedTickets = intervals.Where(t => t._closedCount > 0).ToList();
            if (closedTickets.Count > 0)
            {
                _averageActionCountPercentiles = new Percentiles(closedTickets, x => x._averageActionCount.Value);
                _medianDaysToClosePercentiles = new Percentiles(closedTickets, x => x._medianDaysToClose.Value);
                try
                {
                    _averageSeverityPercentiles = new Percentiles(closedTickets, x => x._averageSeverity.Value);
                }
                catch(Exception)
                {
                    _averageSeverityPercentiles = null;
                }
            }
        }

        public bool CalculateCDI(IntervalData interval)
        {
            // Create the CDI from the normalized fields
            IntervalData normalized = Normalize(interval);

            /// <summary>used by a normalized instance of Interval Data - See ICDIStrategy</summary>
            CalculateNormalizedCDI(normalized); // keep this in the CDI strategy
            interval.CDI = normalized.CDI;

            //CDIEventLog.Write(normalized.ToString());
            //CDIEventLog.WriteLine(interval.ToString());
            return true;
        }

        public IntervalData Normalize(IntervalData interval)
        {
            // 1. Total Tickets Created
            // 5. Average Time tickets took to close

            IntervalData normalized = new IntervalData()
            {
                _timeStamp = interval._timeStamp,
                _newCount = _newCountPercentiles.AsPercentile(interval._newCount),  // 2. Tickets Created in Last 30
                _openCount = _openCountPercentiles.AsPercentile(interval._openCount),   // 3. Number of Tickets Currently Open
                _medianDaysOpen = _medianDaysOpenPercentiles.AsPercentile(interval._medianDaysOpen),    // 4. Average Time Tickets have been open
                _totalTicketsCreated = _totalTicketsCreatedPercentiles.AsPercentile(interval._totalTicketsCreated),
                //_closedCount = _closedCountPercentiles.AsPercentile(interval._closedCount),
            };

            if ((_medianDaysToClosePercentiles != null) && interval._medianDaysToClose.HasValue)
                normalized._medianDaysToClose = _medianDaysToClosePercentiles.AsPercentile(interval._medianDaysToClose.Value);

            //if (interval._averageActionCount.HasValue)
            //    normalized._averageActionCount = _averageActionCountPercentiles.AsPercentile(interval._averageActionCount.Value);

            //if (interval._averageSentimentScore.HasValue)
            //    normalized._averageSentimentScore = interval._averageSentimentScore.Value / 10; // [0, 1000] => [0, 100]

            //if ((interval._averageSeverity.HasValue) && (_averageSeverityPercentiles != null))
            //    normalized._averageSeverity = _averageSeverityPercentiles.AsPercentile(interval._averageSeverity.Value);

            return normalized;
        }

        void CalculateNormalizedCDI(IntervalData normalized)
        {
            normalized.CDI = normalized._newCount;// (int)Math.Round(normalized._newCount);
        }

        void CalculateNormalizedCDI1(IntervalData normalized)
        {
            HashSet<double> contribution = new HashSet<double>
            {
                normalized._newCount,
                normalized._openCount,
                normalized._medianDaysOpen,
                (100 - normalized._closedCount),    // less distress when tickets are closed
            };

            if (normalized._medianDaysToClose.HasValue)
                contribution.Add(normalized._medianDaysToClose.Value);

            if (normalized._averageActionCount.HasValue)
                contribution.Add(normalized._averageActionCount.Value);

            if (normalized._averageSentimentScore.HasValue)
                contribution.Add(100 - normalized._averageSentimentScore.Value);  // [0, 1000] where low is in distress

            if (normalized._averageSeverity.HasValue)
                contribution.Add(100 - normalized._averageSeverity.Value);

            normalized.CDI = (int)Math.Round(contribution.Average());
        }

    }

}
