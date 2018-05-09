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
        public DateTime _timeStamp; // date time for this data

        // new tickets
        public int _newCount; // new (this interval)
        public int _openCount;    // currently Open (since start)
        public double _medianDaysOpen; // median of currently open tickets (days)

        // closed tickets
        public int _closedCount;  // closed (this interval)
        public double? _medianDaysToClose;  // average time to close (this interval)
        public double? _averageActionCount;   // how many actions do the closed ticket have?
        public double? _averageSentimentScore;

        public int? CDI { get; set; }    // CDI !!

        public IntervalData() { }

        public IntervalData(DateTime nextDay, HashSet<TicketJoin> openTickets, HashSet<TicketJoin> closedTickets, int ticketsCreated)
        {
            _timeStamp = nextDay;
            _newCount = ticketsCreated;
            _openCount = openTickets.Count;
            _medianDaysOpen = openTickets.Count == 0 ? 0 : MedianTotalDaysOpen(openTickets).Value;
            _closedCount = closedTickets.Count;

            if (closedTickets.Count > 0)
            {
                _medianDaysToClose = MedianTotalDaysOpen(closedTickets);
                _averageActionCount = closedTickets.Average(x => x.ActionsCount);
                _averageSentimentScore = closedTickets.Average(x => x.TicketSentimentScore);
            }
        }

        /// <summary>used by a normalized instance of Interval Data - See CDIPercentileStrategy</summary>
        public void UpdateCDI()
        {
            HashSet<double> contribution = new HashSet<double> { _newCount, _openCount, _medianDaysOpen, (100 - _closedCount) };

            if (_medianDaysToClose.HasValue)
                contribution.Add(_medianDaysToClose.Value);

            if (_averageActionCount.HasValue)
                contribution.Add(_averageActionCount.Value);

            if (_averageSentimentScore.HasValue)
                contribution.Add(100 - _averageSentimentScore.Value / 10);  // [0, 1000] where low is in distress

            CDI = (int)Math.Round(contribution.Average());
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
            Debug.WriteLine("Date\tNew\tOpen\tMedianDaysOpen\tClosed\tMedianDaysToClose\tAvgActions\tAvgSentiment\tCDI");
            foreach (IntervalData interval in intervals)
                Debug.WriteLine(interval.ToString());

        }

        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}\t{3:0.00}\t{4}\t{5:0.00}\t{6:0.00}\t{7:0.00}\t{8}",
                _timeStamp.ToShortDateString(), _newCount, _openCount, _medianDaysOpen, _closedCount, _medianDaysToClose, _averageActionCount, _averageSentimentScore, CDI);
        }
    }

}
