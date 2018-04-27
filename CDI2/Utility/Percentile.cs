using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Collect and calculate percentiles [0%, 5%, 10%, 15%...] to determine where the current value lies in the distribution
    /// </summary>
    class Percentile<T> where T : IComparable<T>
    {
        const int PercentileBucketCount = 20;   // 0%, 5%, 10%, 15%...
        T[] percentiles;

        public Percentile(List<IntervalData> intervalData, Func<IntervalData, T> getFunc)
        {
            // only consider the intervals containing data
            IntervalData[] nonNull = intervalData.Where(x => x.HasData).ToArray();

            // sort
            Array.Sort(nonNull, (lhs, rhs) => getFunc(lhs).CompareTo(getFunc(rhs)));

            // collect percentile thresholds
            percentiles = new T[PercentileBucketCount + 1];    // 0%, 10%, 20%,... 100%
            for (int i = 0; i <= PercentileBucketCount; ++i)
                percentiles[i] = getFunc(nonNull[(int)Math.Round((double)(nonNull.Length - 1) * i / PercentileBucketCount)]);
        }

        public int AsPercentile(T value, bool highIsBad = true)
        {
            int index = 0;
            while (percentiles[index].CompareTo(value) < 0)
                index++;
            int percentile = index * 100 / PercentileBucketCount;
            return highIsBad ? 100 - percentile : percentile;
        }
    }
}
