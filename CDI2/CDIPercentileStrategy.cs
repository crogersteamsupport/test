using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.CDI
{
    /// <summary>calculate the CDI using a rolling percentile lookup</summary>
    class CDIPercentileStrategy : ICDIStrategy
    {
        List<IntervalData> _intervals;
        IntervalPercentiles _cdiPercentile;

        public CDIPercentileStrategy(List<IntervalData> intervals)
        {
            _intervals = intervals;
        }

        /// <summary>calculate the CDI using a rolling percentile lookup</summary>
        public void CalculateCDI()
        {
            //Debug.WriteLine("Date\tNew\tOpen\tClosed\tDaysOpen\tDaysToClose\tActions\tSentiment\tCDI");
            _intervals.Sort((lhs, rhs) => lhs._timeStamp.CompareTo(rhs._timeStamp));
            DateTime begin = _intervals.First()._timeStamp;
            DateTime end = begin.AddDays(365);  // 1 year rolling average

            // get a rolling year into the queue
            List<IntervalData> rollingYear = _intervals.Where(t => t._timeStamp < end).ToList();
            _cdiPercentile = new IntervalPercentiles(rollingYear);  // mixes up rollingYear...
            foreach (IntervalData intervalData in _intervals)
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

                if (update)
                    _cdiPercentile = new IntervalPercentiles(rollingYear);  // updated percentile strategy
                _cdiPercentile.CalculateCDI(intervalData);

                // do the same for all clients with this intervalData
            }
        }
    }
}
