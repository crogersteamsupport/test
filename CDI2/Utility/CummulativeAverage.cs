using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDI2
{
    class CummulativeAverage<T> where T : struct
    {
        dynamic _sum;
        int _count;

        public void Add(T value)
        {
            _sum += value;
            ++_count;
        }

        public void Subtract(T value)
        {
            _sum -= value;
            --_count;
        }

        public double Average
        {
            get { return _sum / (double)_count; }
        }

    }
}
