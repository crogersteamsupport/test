using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDI2
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Simple_linear_regression
    /// </summary>
    class TrendLine
    {
        LinearRegression _source;

        public double Variance { get; private set; }
        public double Covariance { get; private set; }
        public double Slope { get; private set; }
        public double YIntercept { get; private set; }

        public TrendLine(LinearRegression source)
        {
            _source = source;
            VarianceCoVariance();
            SlopeIntercept();
        }

        void VarianceCoVariance()
        {
            List<DateTime> x = _source.X;
            List<int> y = _source.Y;

            double numerator = 0;
            double denominator = 0;
            int n = x.Count;
            for (int i = 0; i < n; ++i)
            {
                long xdiff = x[i].Ticks - _source.XMean;
                double ydiff = y[i] - _source.YMean;
                numerator += xdiff * ydiff;
                denominator += xdiff * xdiff;
            }

            Covariance = numerator /= n;
            Variance = denominator /= n;
        }

        public void SlopeIntercept()
        {
            Slope = Covariance / Variance;
            YIntercept = _source.YMean - Slope * _source.XMean;
        }
    }
}
