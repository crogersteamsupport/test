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
    public class CDIPercentile
    {
        Percentiles<int> _newCountPercentiles;  // new (this interval)
        Percentiles<int> _openCountPercentiles; // currently Open (since startDate)
        Percentiles<double> _medianDaysOpenPercentiles;   // days open of currently open
        Percentiles<int> _closedCountPercentiles;   // closed (this interval)
        Percentiles<double> _medianDaysToClosePercentiles;    // average time to close (this interval)
        Percentiles<double> _averageActionCountPercentiles;   // actions per ticket

        public CDIPercentile(List<IntervalData> intervalData)
        {
            // counts
            _newCountPercentiles = new Percentiles<int>(intervalData, delegate (IntervalData x) { return x._newCount; });
            _openCountPercentiles = new Percentiles<int>(intervalData, delegate (IntervalData x) { return x._openCount; });
            _closedCountPercentiles = new Percentiles<int>(intervalData, delegate (IntervalData x) { return x._closedCount; });

            // open tickets
            _medianDaysOpenPercentiles = new Percentiles<double>(intervalData, delegate (IntervalData x) { return x._medianDaysOpen; });

            // closed tickets
            List<IntervalData> closedTickets = intervalData.Where(t => t._closedCount > 0).ToList();
            if (closedTickets.Count > 0)
            {
                _averageActionCountPercentiles = new Percentiles<double>(closedTickets, delegate (IntervalData x) { return x._averageActionCount.Value; });
                _medianDaysToClosePercentiles = new Percentiles<double>(closedTickets, delegate (IntervalData x) { return x._medianDaysToClose.Value; });
            }
        }

        //static bool _writeHeader = true;
        public void CalculateCDI(IntervalData intervalData)
        {
            // Create the CDI from the normalized fields
            IntervalData normalized = Normalize(intervalData);
            normalized.UpdateCDI();
            intervalData.CDI = normalized.CDI;

            //if (_writeHeader)
            //{
            //    _writeHeader = false;
            //    Debug.WriteLine("Date\tNew\tOpen\tMedianDaysOpen\tClosed\tMedianDaysToClose\tAvgActions\tAvgSentiment\tCDI");
            //}
            //Debug.WriteLine(normalized.ToString());
            //Debug.WriteLine(intervalData.ToString());
        }

        // TODO
        //  * time to respond (first response from customer service)
        public IntervalData Normalize(IntervalData intervalData)
        {
            IntervalData normalized = new IntervalData()
            {
                _timeStamp = intervalData._timeStamp,
                _newCount = _newCountPercentiles.AsPercentile(intervalData._newCount),
                _openCount = _openCountPercentiles.AsPercentile(intervalData._openCount),
                _medianDaysOpen = _medianDaysOpenPercentiles.AsPercentile(intervalData._medianDaysOpen),
                _closedCount = _closedCountPercentiles.AsPercentile(intervalData._closedCount),
            };

            if (intervalData._medianDaysToClose.HasValue)
                normalized._medianDaysToClose = _medianDaysToClosePercentiles.AsPercentile(intervalData._medianDaysToClose.Value);

            if (intervalData._averageActionCount.HasValue)
                normalized._averageActionCount = _averageActionCountPercentiles.AsPercentile(intervalData._averageActionCount.Value);

            if (intervalData._averageSentimentScore.HasValue)
                normalized._averageSentimentScore = intervalData._averageSentimentScore.Value;

            return normalized;
        }
    }

}
