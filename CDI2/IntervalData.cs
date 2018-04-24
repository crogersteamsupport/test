using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CDI2
{
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

        public double CDI { get; private set; }    // CDI !!

        static bool _headerWritten = false;
        public void Write()
        {
            if (!_headerWritten)
            {
                Debug.WriteLine("Date\tNewTickets\tOpenCount\tAvgOpenTime(days)\tClosed\tAvgTimeToClose(days)\tCDI");
                _headerWritten = true;
            }

            Debug.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                _timeStamp, _ticketsCreated, _ticketsOpen, _averageDaysOpen, _ticketsClosed, _averageDaysToClose, CDI));
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
        /// <param name="percentile"></param>
        public void CalculateCDI(IntervalDataDistribution percentile)
        {
            double[] metrics = new double[(int)Metric.DaysToClose + 1];
            metrics[(int)Metric.Created] = percentile.TicketsCreated(_ticketsCreated);
            metrics[(int)Metric.Open] = percentile.TicketsOpen(_ticketsOpen);
            metrics[(int)Metric.DaysOpen] = percentile.AverageDaysOpen(_averageDaysOpen);
            metrics[(int)Metric.Closed] = percentile.TicketsClosed(_ticketsClosed);
            metrics[(int)Metric.DaysToClose] = percentile.AverageDaysToClose(_averageDaysToClose);

            CDI = 10 * metrics.Average();
            //Write();
        }
    }

}
