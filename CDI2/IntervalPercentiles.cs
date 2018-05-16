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
        Percentiles<int> _newCountPercentiles;  // new (this interval)
        Percentiles<int> _openCountPercentiles; // currently Open (since startDate)
        Percentiles<double> _medianDaysOpenPercentiles;   // days open of currently open
        Percentiles<int> _closedCountPercentiles;   // closed (this interval)
        Percentiles<double> _medianDaysToClosePercentiles;    // average time to close (this interval)
        Percentiles<double> _averageActionCountPercentiles;   // actions per ticket
        Percentiles<double> _averageSeverityPercentiles;   // ticket severity

        public IntervalPercentiles(List<IntervalData> intervals)
        {
            // counts
            _newCountPercentiles = new Percentiles<int>(intervals, x => x._newCount);
            _openCountPercentiles = new Percentiles<int>(intervals, x => x._openCount);
            _closedCountPercentiles = new Percentiles<int>(intervals, x => x._closedCount);

            // open tickets
            _medianDaysOpenPercentiles = new Percentiles<double>(intervals, x => x._medianDaysOpen);

            // closed tickets
            List<IntervalData> closedTickets = intervals.Where(t => t._closedCount > 0).ToList();
            if (closedTickets.Count > 0)
            {
                _averageActionCountPercentiles = new Percentiles<double>(closedTickets, x => x._averageActionCount.Value);
                _medianDaysToClosePercentiles = new Percentiles<double>(closedTickets, x => x._medianDaysToClose.Value);
                try
                {
                    _averageSeverityPercentiles = new Percentiles<double>(closedTickets, x => x._averageSeverity.Value);
                }
                catch(Exception)
                {
                    _averageSeverityPercentiles = null;
                }
            }
        }

        //static bool _writeHeader = true;
        public void CalculateCDI(IntervalData interval)
        {
            // Create the CDI from the normalized fields
            IntervalData normalized = Normalize(interval);

            /// <summary>used by a normalized instance of Interval Data - See ICDIStrategy</summary>
            CalculateNormalizedCDI(normalized); // keep this in the CDI strategy
            interval.CDI = normalized.CDI;

            //if (_writeHeader)
            //{
            //    _writeHeader = false;
            //    IntervalData.WriteHeader();
            //}
            //Debug.WriteLine(normalized.ToString());
            //Debug.WriteLine(interval.ToString());
        }

        public IntervalData Normalize(IntervalData interval)
        {
            IntervalData normalized = new IntervalData()
            {
                _timeStamp = interval._timeStamp,
                _newCount = _newCountPercentiles.AsPercentile(interval._newCount),
                _openCount = _openCountPercentiles.AsPercentile(interval._openCount),
                _medianDaysOpen = _medianDaysOpenPercentiles.AsPercentile(interval._medianDaysOpen),
                _closedCount = _closedCountPercentiles.AsPercentile(interval._closedCount),
            };

            if (interval._medianDaysToClose.HasValue)
                normalized._medianDaysToClose = _medianDaysToClosePercentiles.AsPercentile(interval._medianDaysToClose.Value);

            if (interval._averageActionCount.HasValue)
                normalized._averageActionCount = _averageActionCountPercentiles.AsPercentile(interval._averageActionCount.Value);

            if (interval._averageSentimentScore.HasValue)
                normalized._averageSentimentScore = interval._averageSentimentScore.Value / 10; // [0, 1000] => [0, 100]

            if ((interval._averageSeverity.HasValue) && (_averageSeverityPercentiles != null))
                normalized._averageSeverity = _averageSeverityPercentiles.AsPercentile(interval._averageSeverity.Value);

            return normalized;
        }

        void CalculateNormalizedCDI(IntervalData normalized)
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
