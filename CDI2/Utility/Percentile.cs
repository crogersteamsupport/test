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
    class Percentiles<T> where T : IComparable<T>
    {
        const int PercentileBucketCount = 20;   // 0%, 5%, 10%, 15%...
        const int PercentIncrement = 100 / PercentileBucketCount;  // 5% increments
        T[] percentiles;

        public Percentiles(List<IntervalData> intervals, Func<IntervalData, T> getFunc)
        {
            // sort
            intervals.Sort((lhs, rhs) => getFunc(lhs).CompareTo(getFunc(rhs)));

            // collect percentile thresholds
            percentiles = new T[PercentileBucketCount + 1];
            for (int i = 0; i <= PercentileBucketCount; ++i)
                percentiles[i] = getFunc(intervals[(int)Math.Round((double)(intervals.Count - 1) * i / PercentileBucketCount)]);
        }

        public int AsPercentile(T value)
        {
            // where is std::lower_bound when you need it?
            int i = 0;
            for(; i < percentiles.Length - 1; ++i)
            {
                // found a match
                if (percentiles[i + 1].CompareTo(value) > 0)
                    break;
            }

            return i * PercentIncrement;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (T percentile in percentiles)
            {
                if(typeof(T) == typeof(double))
                    builder.Append(String.Format("{0:0.0}", percentile));
                else
                    builder.Append(percentile);
                builder.Append("  ");
            }
            return builder.ToString();
        }

    }
}
