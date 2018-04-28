using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Collect percentiles to determine where the current value lies in the distribution
    /// </summary>
    class Percentile<T> where T : IComparable<T>
    {
        const int PercentileBucketCount = 20;   // 0%, 5%, 10%, 15%...
        T[] percentiles;

        public Percentile(List<IntervalData> intervalData, Func<IntervalData, T> getFunc)
        {
            // sort
            intervalData.Sort((lhs, rhs) => getFunc(lhs).CompareTo(getFunc(rhs)));

            // collect percentile thresholds
            percentiles = new T[PercentileBucketCount + 1];
            for (int i = 0; i <= PercentileBucketCount; ++i)
                percentiles[i] = getFunc(intervalData[(int)Math.Round((double)(intervalData.Count - 1) * i / PercentileBucketCount)]);
        }

        public int AsPercentile(T value)
        {
            // where is std::lower_bound when you need it?
            int index = 0;
            while (percentiles[index].CompareTo(value) < 0)
                index++;
            return index * 100 / PercentileBucketCount;
        }
    }
}
