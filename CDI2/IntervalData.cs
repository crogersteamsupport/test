using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Keep the results for the analysis of closed tickets for the given time interval
    /// </summary>
    public class IntervalData
    {
        public DateTime _intervalEndTimeStamp; // date time for this data

        // new tickets
        public int _newTicketsCount; // new (this interval)
        public int _openTicketsCount;    // currently Open (since start)
        public double _medianOpenTicketsDaysOpen; // median of currently open tickets (days)

        // closed tickets
        public int _closedTicketsCount;  // closed (this interval)
        public double? _medianDaysToClose;  // average time to close (this interval)

        public int? CDI { get; set; }    // CDI !!

        public IntervalData(DateTime nextDay, HashSet<TicketJoin> openTickets, HashSet<TicketJoin> closedTickets, int ticketsCreated)
        {
            _intervalEndTimeStamp = nextDay;
            _newTicketsCount = ticketsCreated;
            _openTicketsCount = openTickets.Count;
            _medianOpenTicketsDaysOpen = openTickets.Count == 0 ? 0 : Median(openTickets).Value;
            _closedTicketsCount = closedTickets.Count;
            _medianDaysToClose = Median(closedTickets);
        }

        private double? Median(HashSet<TicketJoin> tickets)
        {
            if (tickets.Count == 0)
                return null;

            double[] totalDays = tickets.Select(ticket => ticket.TotalDaysOpen).ToArray();
            Array.Sort(totalDays);
            int centerIndex = totalDays.Length / 2;
            double result = (totalDays.Length % 2 == 1) ? totalDays[centerIndex] : (totalDays[centerIndex - 1] + totalDays[centerIndex]) / 2;
            return result;
        }


        public static void Write(List<IntervalData> intervals)
        {
            Debug.WriteLine("Date\tNewTickets\tOpenCount\tAvgOpenTime(days)\tClosed\tAvgTimeToClose(days)\tCDI");
            foreach (IntervalData interval in intervals)
                Debug.WriteLine(interval.ToString());

        }
        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                _intervalEndTimeStamp, _newTicketsCount, _openTicketsCount, _medianOpenTicketsDaysOpen, _closedTicketsCount, _medianDaysToClose, CDI);
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
    }

}
