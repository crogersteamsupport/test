using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Keep the results for the analysis of closed tickets for the given time interval
    /// </summary>
    public class IntervalData
    {
        public DateTime _intervalEndTimeStamp; // date time for this data
        public bool HasData { get; private set; }

        // new tickets
        public int _newTicketsCount; // new (this interval)
        public int _openTicketsCount;    // currently Open (since start)
        public double _medianOpenTicketsDaysOpen; // median of currently open tickets (days)

        // closed tickets
        public int _closedTicketsCount;  // closed (this interval)
        public double? _medianDaysToClose;  // average time to close (this interval)
        public double? _closedTicketsActionsCount;   // how many actions do the closed ticket have?

        public int? CDI { get; set; }    // CDI !!

        public IntervalData(DateTime nextDay)
        {
            _intervalEndTimeStamp = nextDay;
            HasData = false;
        }

        public IntervalData(DateTime nextDay, HashSet<TicketJoin> openTickets, HashSet<TicketJoin> closedTickets, int ticketsCreated)
        {
            HasData = true;
            _intervalEndTimeStamp = nextDay;
            _newTicketsCount = ticketsCreated;
            _openTicketsCount = openTickets.Count;
            _medianOpenTicketsDaysOpen = openTickets.Count == 0 ? 0 : MedianTotalDaysOpen(openTickets).Value;
            _closedTicketsCount = closedTickets.Count;

            if (closedTickets.Count > 0)
            {
                _medianDaysToClose = MedianTotalDaysOpen(closedTickets);
                _closedTicketsActionsCount = closedTickets.Average(x => x.ActionsCount);
            }
        }

        private double? MedianTotalDaysOpen(HashSet<TicketJoin> tickets)
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
            return String.Format("{0} {1} {2} {3:0.00} {4} {5:0.00} {6}",
                _intervalEndTimeStamp.ToShortDateString(), _newTicketsCount, _openTicketsCount, _medianOpenTicketsDaysOpen, _closedTicketsCount, _medianDaysToClose, CDI);
        }
    }

}
