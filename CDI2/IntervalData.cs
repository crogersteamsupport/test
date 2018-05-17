﻿using System;
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
        public double? _averageSeverity;
        public int _CreatorIDCount;  // how many users created the closed tickets

        public int? CDI { get; set; }    // CDI !!

        public IntervalData() { }

        public IntervalData(DateTime nextDay, HashSet<TicketJoin> openTickets, HashSet<TicketJoin> closedTickets, int ticketsCreated)
        {
            _timeStamp = nextDay;
            _newCount = ticketsCreated;
            _openCount = openTickets.Count;
            _medianDaysOpen = openTickets.Count == 0 ? 0 : MedianTotalDaysOpenToTimestamp(openTickets, _timeStamp).Value;
            _closedCount = closedTickets.Count;

            if (closedTickets.Count > 0)
            {
                _medianDaysToClose = MedianTotalDaysToClose(closedTickets);
                _averageActionCount = closedTickets.Average(x => x.ActionsCount);
                _averageSentimentScore = closedTickets.Average(x => x.AverageActionSentiment);
                _averageSeverity = closedTickets.Average(x => x.Severity);
                _CreatorIDCount = closedTickets.Select(t => t.CreatorID).Distinct().Count();
            }
        }

        private double Median(double[] values)
        {
            Array.Sort(values);
            int centerIndex = values.Length / 2;
            double result = (values.Length % 2 == 1) ? values[centerIndex] : (values[centerIndex - 1] + values[centerIndex]) / 2;
            return result;
        }

        private double? MedianTotalDaysToClose(HashSet<TicketJoin> tickets)
        {
            if (tickets.Count == 0)
                return null;

            double[] totalDays = tickets.Select(ticket => ticket.TotalDaysToClose).ToArray();
            return Median(totalDays);
        }

        private double? MedianTotalDaysOpenToTimestamp(HashSet<TicketJoin> tickets, DateTime timestamp)
        {
            if (tickets.Count == 0)
                return null;

            double[] totalDays = tickets.Select(ticket => ticket.TotalDaysOpenToTimestamp(timestamp)).ToArray();
            return Median(totalDays);
        }

        public static void WriteHeader()
        {
            Debug.WriteLine("Date\tNew\tOpen\tMedianDaysOpen\tClosed\tMedianDaysToClose\tAvgActions\tAvgSentiment\tAverageSeverity\tCDI\tClientCustomerID");
        }

        public static void Write(List<IntervalData> intervals)
        {
            WriteHeader();
            foreach (IntervalData interval in intervals)
                Debug.WriteLine(interval.ToString());
        }

        public override string ToString()
        {
            return String.Format("{0} \t{1} \t{2} \t{3:0.00} \t{4} \t{5:0.00} \t{6:0.00} \t{7:0.00} \t{8:0.00} \t{9}",
                _timeStamp.ToShortDateString(), _newCount, _openCount, _medianDaysOpen, _closedCount, _medianDaysToClose, _averageActionCount, _averageSentimentScore, _averageSeverity, CDI);
        }
    }

}
