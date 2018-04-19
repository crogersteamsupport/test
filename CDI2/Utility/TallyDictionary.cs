using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CDI2
{
    /// <summary>
    /// Utility class for histogram - counter by bin
    /// </summary>
    class TallyDictionary
    {
        SortedDictionary<int, int> _sum = new SortedDictionary<int, int>();

        /// <summary> add an item to the bin </summary>
        /// <param name="binIndex">which bin does this record land in</param>
        public void Increment(int binIndex)
        {
            if (!_sum.ContainsKey(binIndex))
                _sum[binIndex] = 1;
            else
                _sum[binIndex]++;
        }

        /// <summary> remove an item from the bin </summary>
        /// <param name="binIndex">which bin does this record land in</param>
        public void Decrement(int binIndex)
        {
            if (!_sum.ContainsKey(binIndex))
                _sum[binIndex] = -1;
            else
                _sum[binIndex]--;
        }

        public void Write()
        {
            foreach (KeyValuePair<int, int> pair in _sum)
                Debug.WriteLine("{0}\t{1}", pair.Key, pair.Value);
        }
    }
}
