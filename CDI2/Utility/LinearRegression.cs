using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CDI2
{

    /// <summary>
    /// https://en.wikipedia.org/wiki/Simple_linear_regression
    /// </summary>
    class LinearRegression
    {
        // raw data
        public List<DateTime> X { get; private set; }
        public List<int> Y { get; private set; }
        public long XMean { get; private set; }
        public double YMean { get; private set; }

        public LinearRegression(List<DateTime> x, List<int> y)
        {
            X = x;
            Y = y;
            XMean = (long)X.Select(d => d.Ticks).Average();
            YMean = Y.Average();
        }

        // Trend line
        TrendLine _trendLine = null;
        public double Slope()
        {
            if (_trendLine == null)
                _trendLine = new TrendLine(this);

            return _trendLine.Slope;
        }

        // Correlation
        double? _rSquared = null;
        public double CoefficientOfDetermination()
        {
            if (_rSquared.HasValue)
                return _rSquared.Value;

            double sumXY = 0;
            double sumXX = 0;
            double sumYY = 0;
            int n = X.Count;
            for(int i = 0; i < n; ++i)
            {
                long x = X[i].Ticks;
                sumXY += x * Y[i];
                sumXX += x * x;
                sumYY += Y[i] * Y[i];
            }

            double numerator = sumXY / n - XMean * YMean;
            numerator *= numerator;
            double denominator = (sumXX / n - XMean * XMean) * (sumYY / n - YMean * YMean);
            _rSquared = numerator / denominator;
            return _rSquared.Value;
        }
    }
}
