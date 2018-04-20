using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CDI2
{
    /// <summary>
    /// Average data for IntervalData
    /// </summary>
    public class IntervalDataAverage
    {
        public double _ticketsCreated; // new (this interval)
        public double _ticketsOpen;    // currently Open (since start)
        public double _averageDaysOpen; // average of still open
        public double _ticketsClosed;  // closed (this interval)
        public double _averageDaysToClose;  // average time to close (this interval)

        public IntervalDataAverage(List<IntervalData> organization)
        {
            _ticketsCreated = organization.Average(o => o._ticketsCreated);
            _ticketsOpen = organization.Average(o => o._ticketsOpen);
            _averageDaysOpen = organization.Average(o => o._averageDaysOpen);
            _ticketsClosed = organization.Average(o => o._ticketsClosed);
            _averageDaysToClose = organization.Average(o => o._averageDaysToClose);
        }
    }

    /// <summary>
    /// Keep the results for the analysis of closed tickets for the given time interval
    /// </summary>
    public class IntervalData
    {
        public DateTime _timeStamp; // date time for this data

        // new tickets
        public int _ticketsCreated; // new (this interval)
        public int _ticketsOpen;    // currently Open (since start)
        public double _averageDaysOpen; // average of still open

        // closed tickets
        public int _ticketsClosed;  // closed (this interval)
        public double _averageDaysToClose;  // average time to close (this interval)

        private double _cdi;    // CDI !!

        static bool _headerWritten = false;
        public void Write()
        {
            if (!_headerWritten)
            {
                Debug.WriteLine("Date\tNewTickets\tOpenCount\tAvgOpenTime(days)\tClosed\tAvgTimeToClose(days)\tCDI");
                _headerWritten = true;
            }

            Debug.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                _timeStamp, _ticketsCreated, _ticketsOpen, _averageDaysOpen, _ticketsClosed, _averageDaysToClose, _cdi));
        }

        public override string ToString()
        {
            return _timeStamp.ToString();
        }

        double Normalize(double numerator, double denominator)
        {
            if (denominator == 0)
                return 0;
            return numerator / denominator;
        }

        // are we above or below average?
        enum Metric
        {
            Created,
            Open,
            DaysOpen,
            Closed,
            DaysToClose
        }

        /// <summary>
        /// get a +-% from the average
        /// </summary>
        /// <param name="average"></param>
        public void CalculateCDI(IntervalDataAverage average)
        {
            double[] metrics = new double[(int)Metric.DaysToClose + 1];
            metrics[(int)Metric.Created] = Normalize(_ticketsCreated, average._ticketsCreated);
            metrics[(int)Metric.Open] = Normalize(_ticketsOpen, average._ticketsOpen);
            metrics[(int)Metric.DaysOpen] = Normalize(_averageDaysOpen, average._averageDaysOpen);
            metrics[(int)Metric.Closed] = Normalize(_ticketsClosed, average._ticketsClosed);
            metrics[(int)Metric.DaysToClose] = Normalize(_averageDaysToClose, average._averageDaysToClose);

            _cdi = metrics.Average();
        }
    }

}
