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
    class Percentiles //where T : IComparable<T>
    {
        const int PercentileBucketCount = 20;   // 0%, 5%, 10%, 15%...
        const int PercentIncrement = 100 / PercentileBucketCount;  // 5% increments
        double[] percentiles;

        public Percentiles(List<IntervalData> intervals, Func<IntervalData, double> getFunc)
        {
            // sort
            intervals.Sort((lhs, rhs) => getFunc(lhs).CompareTo(getFunc(rhs)));

            // collect percentile thresholds
            percentiles = new double[PercentileBucketCount + 1];
            for (int i = 0; i <= PercentileBucketCount; ++i)
            {
                double interpolate = (double)(intervals.Count - 1) * i / (double)PercentileBucketCount;
                int index = (int)interpolate;
                if ((interpolate - index != 0) && (index + 1 < intervals.Count()))
                {
                    double value = getFunc(intervals[index]) + (interpolate - index) * (getFunc(intervals[index + 1]) - getFunc(intervals[index]));
                    percentiles[i] = value;
                }
                else
                {
                    percentiles[i] = getFunc(intervals[(int)Math.Round(interpolate)]);
                }
            }
        }

        public int AsPercentile(double value)
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
            foreach (double percentile in percentiles)
            {
                if(typeof(double) == typeof(double))
                    builder.Append(String.Format("{0:0.0}", percentile));
                else
                    builder.Append(percentile);
                builder.Append("  ");
            }
            return builder.ToString();
        }

    }
}
