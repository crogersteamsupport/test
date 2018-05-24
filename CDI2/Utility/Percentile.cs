using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Collect percentiles to determine where the current value lies in the distribution
    /// </summary>
    class Percentile //where T : IComparable<T>
    {
        int _intervalCount;
        SortedDictionary<double, int> percentiles;

        public Percentile(List<IntervalData> intervals, Func<IntervalData, double> getFunc)
        {
            _intervalCount = intervals.Count();
            intervals.Sort((lhs, rhs) => getFunc(lhs).CompareTo(getFunc(rhs)));
            percentiles = new SortedDictionary<double, int>();

            foreach(IntervalData interval in intervals)
            {
                double value = getFunc(interval);
                if (percentiles.ContainsKey(value))
                    ++percentiles[value];
                else
                    percentiles[value] = 1;
            }
        }

        /// <summary>
        /// https://web.stanford.edu/class/archive/anthsci/anthsci192/anthsci192.1064/handouts/calculating%20percentiles.pdf
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int AsPercentile(double value)
        {
            double result;
            int index = 1;
            foreach(KeyValuePair<double, int> pair in percentiles)
            {
                if (value <= pair.Key)  // use the lower bound of percentile (TODO - invert negative correlated metrics!)
                    break;
                index += pair.Value;
            }
            result = 100 * (index - 0.5) / _intervalCount;
            if (result < 0)
                return 0;
            if (result > 99)
                return 99;
            return (int)Math.Round(result);
        }
    }
}
