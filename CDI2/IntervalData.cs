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
        public double? _medianDaysOpen; // average of still open

        // closed tickets
        public int _ticketsClosed;  // closed (this interval)
        public double? _medianDaysToClose;  // average time to close (this interval)

        public int? CDI { get; private set; }    // CDI !!

        public static void Write(List<IntervalData> intervals)
        {
            Debug.WriteLine("Date\tNewTickets\tOpenCount\tAvgOpenTime(days)\tClosed\tAvgTimeToClose(days)\tCDI");
            foreach (IntervalData interval in intervals)
                Debug.WriteLine(interval.ToString());

        }
        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                _timeStamp, _ticketsCreated, _ticketsOpen, _medianDaysOpen, _ticketsClosed, _medianDaysToClose, CDI);
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
            // TODO
            //  * time to respond (first response from customer service)
            //  * Happy face

            //double[] metrics = new double[(int)Metric.DaysToClose + 1];
            //metrics[(int)Metric.Created] = percentile.TicketsCreated(_ticketsCreated);
            //metrics[(int)Metric.Open] = percentile.TicketsOpen(_ticketsOpen);
            //metrics[(int)Metric.DaysOpen] = percentile.AverageDaysOpen(_averageDaysOpen);
            //metrics[(int)Metric.Closed] = percentile.TicketsClosed(_ticketsClosed);
            //metrics[(int)Metric.DaysToClose] = percentile.MedianDaysToClose(_medianDaysToClose);

            CDI = (percentile.TicketsCreated(_ticketsCreated) + percentile.TicketsClosed(_ticketsClosed)) / 2;
            CDI *= 10;  // [0, 1000]
            //Write();
        }
    }

}
