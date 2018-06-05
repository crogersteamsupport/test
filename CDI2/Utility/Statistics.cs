using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.CDI
{
    class Statistics
    {
        HashSet<Client> _clients;
        double[][] _rawData;
        double[] _cdi;
        IntervalPercentiles _percentiles;

        const int MetricCount = 5;

        public Statistics(Customer customer)
        {
            _clients = customer._clients;
            _rawData = new double[MetricCount][];
            for (int i = 0; i < MetricCount; ++i)
                _rawData[i] = new double[_clients.Count];
            _cdi = new double[_clients.Count];
        }

        public void CalculatePercentiles()
        {
            List<IntervalData> clientIntervals = new List<IntervalData>();
            int i = 0;
            foreach (Client client in _clients)
            {
                IntervalData interval = client._iCdiStrategy.RawMetrics;
                if (interval == null)
                    continue;

                Insert(i, interval);
                if (++i >= _clients.Count)
                    break;

                clientIntervals.Add(interval);
            }

            _percentiles = new IntervalPercentiles(clientIntervals);
        }

        void Insert(int i, IntervalData interval)
        {
            _rawData[0][i] = interval._totalTicketsCreated; // TotalTicketsCreated
            _rawData[1][i] = interval._openCount;   // TicketsOpen
            _rawData[2][i] = interval._newCount;    // CreatedLast30
            _rawData[3][i] = interval._medianDaysOpen;  // AvgTimeOpen
            if (interval._medianDaysToClose.HasValue)
                _rawData[4][i] = interval._medianDaysToClose.Value; // AvgTimeToClose
        }

        public void FindOptimalMix()
        {
            //"TotalTicketsCreated\tTicketsOpen\tCreatedLast30\tAvgTimeOpen\tAvgTimeToClose"
            int maxCombination = 0;
            double maxCorrelation = 0;
            for (int combination = 1; combination < 32; ++combination)
            {
                linq.CDI_Settings weights = new linq.CDI_Settings()
                {
                    TotalTicketsWeight = (combination & 0x1) != 0 ? 1 : 0,
                    OpenTicketsWeight = (combination & 0x10) != 0 ? 1 : 0,
                    Last30Weight = (combination & 0x100) != 0 ? 1 : 0,
                    AvgDaysOpenWeight = (combination & 0x1000) != 0 ? 1 : 0,
                    AvgDaysToCloseWeight = (combination & 0x10000) != 0 ? 1 : 0
                };
                weights.Normalize();

                int i = 0;
                foreach (Client client in _clients)
                {
                    client.InvokeCDIStrategy(_percentiles, weights);
                    _cdi[i++] = client._iCdiStrategy.RawMetrics.CDI.Value;
                }

                double sumAbsCorrelation = CheckCorrelation(combination);
                //CDIEventLog.WriteLine("{0} : {1}", combination, sumAbsCorrelation);
                if (sumAbsCorrelation > maxCorrelation)
                {
                    maxCombination = combination;
                    maxCorrelation = sumAbsCorrelation;
                }
            }
            CDIEventLog.WriteLine("{0}\t{1}", maxCombination, maxCorrelation);
        }

        double CheckCorrelation(int combination)
        {
            double result = 0;
            for (int i = 0; i < MetricCount; ++i)
                result += RSquared(_rawData[i], _cdi);
            return result;
        }

        /// <summary> Standard Deviation </summary>
        double StDev(double[] X, double xMean)
        {
            double sum = 0;
            foreach(double x in X)
            {
                double value = x - xMean;
                sum += value * value;
            }
            return Math.Sqrt(sum / (X.Length - 1));
        }

        /// <summary> Covariance of X and Y </summary>
        double Covariance(double[] X, double[] Y, double xMean, double yMean)
        {
            double sum = 0;
            double n = X.Length;
            for (int i = 0; i < n; ++i)
                sum += (X[i] - xMean) * (Y[i] - yMean);

            return sum / (n - 1);
        }


        /// <summary> Pearson Correlation Coefficient </summary>
        double PearsonCorrelationCoefficient(double[] X, double[] Y)
        {
            if ((X.Length == 0) || (X.Length != Y.Length))
                return 0;

            double xMean = X.Average();
            double yMean = Y.Average();

            double covariance = Covariance(X, Y, xMean, yMean);

            double xStdDev = StDev(X, xMean);
            double yStdDev = StDev(Y, yMean);
            return covariance / (xStdDev * yStdDev);
        }

        /// <summary> RSquared = PearsonCorrelationCoefficient^2 /// </summary>
        double RSquared(double[] X, double[] Y)
        {
            if ((X.Length == 0) || (X.Length != Y.Length))
                return 0;

            double n = X.Length;
            double sumX = 0, sumY = 0, sumXX = 0, sumYY = 0, sumXY = 0;
            for(int i = 0; i < n; ++i)
            {
                sumX += X[i];
                sumY += Y[i];
                sumXX += X[i] * X[i];
                sumYY += Y[i] * Y[i];
                sumXY += X[i] * Y[i];
            }

            double XMean = sumX / n;
            double YMean = sumY / n;
            double numerator = sumXY / n - XMean * YMean;
            numerator *= numerator;
            double denominator = (sumXX / n - XMean * XMean) * (sumYY / n - YMean * YMean);
            return numerator / denominator;
        }

    }
}
