﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

namespace CDI2
{
    /// <summary>
    /// Analyze the last year of tickets
    /// Do calculations in days to prevent Ticks overflow
    /// </summary>
    class ClosedTicketAnalysis
    {
        TicketJoin[] _tickets;
        public double AvgTimeToClose { get; private set; }
        public double StdevTimeToClose { get; private set; }   // days
        List<Tuple<DateTime, TicketJoin>> _chronological;   // chronological listing of each open and close

        public ClosedTicketAnalysis(TicketJoin[] tickets)
        {
            _tickets = tickets;
            try
            {
                InitStatistics();
            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("InitStatistics failed", e);
            }
        }

        /// <summary> Average days open and StDev </summary>
        void InitStatistics()
        {
            // average
            AvgTimeToClose = _tickets.Average(t => t.TotalDaysOpen);

            // standard deviation
            double denominator = 0;
            foreach (TicketJoin t in _tickets)
            {
                double xdiff = t.TotalDaysOpen - AvgTimeToClose;
                denominator += xdiff * xdiff;
            }
            StdevTimeToClose = Math.Sqrt(denominator / _tickets.Length);
        }

        /// <summary> organize tickets for each open and close </summary>
        void InitChronological()
        {
            // can be called from AnalyzeDaysOpen if analyzing multiple time spans
            if (_chronological != null)
                return;

            // insert each ticket by DateCreated and DateClosed (duplicates allowed)
            _chronological = new List<Tuple<DateTime, TicketJoin>>();   // allow duplicates for DateTime
            foreach (TicketJoin t in _tickets)
            {
                _chronological.Add(new Tuple<DateTime, TicketJoin>(t.DateCreated, t));  // opened
                _chronological.Add(new Tuple<DateTime, TicketJoin>(t.DateClosed.Value, t));  // closed
            }

            // sort by date and TicketID
            _chronological.Sort((x, y) =>
            {
                int result = x.Item1.CompareTo(y.Item1);
                return result == 0 ? x.Item2.CompareTo(y.Item2) : result;
            });
        }

        /// <summary>
        /// Sample the days open at the specified interval
        /// </summary>
        /// <param name="interval">how often do we sample - daily, weekly,... </param>
        public List<IntervalData> AnalyzeDaysOpen(DateTime firstDayMidnight, DateTime lastDayMidnight, TimeSpan interval)
        {
            List<IntervalData> results = null;
            try
            {
                DateTime nextInterval = firstDayMidnight + interval;

                // tickets open at that moment in time
                HashSet<TicketJoin> openTickets = new HashSet<TicketJoin>();    // HashSet is faster than List for big data
                HashSet<TicketJoin> closedTickets = new HashSet<TicketJoin>();
                int ticketsCreated = 0;
                results = new List<IntervalData>();

                // spin through all the create/close times and keep a running tally of 
                // all currently open/closed
                InitChronological();
                foreach (Tuple<DateTime, TicketJoin> pair in _chronological)
                {
                    if (pair.Item1 > nextInterval)
                    {
                        // sample the data at this time
                        results.Add(GetIntervalData(nextInterval, openTickets, closedTickets, ticketsCreated));
                        closedTickets.Clear();
                        ticketsCreated = 0;
                        nextInterval += interval;
                    }

                    // running list of open and closed tickets
                    if (pair.Item1 == pair.Item2.DateCreated)
                    {
                        openTickets.Add(pair.Item2);    // new ticket opened
                        ++ticketsCreated;
                    }
                    else
                    {
                        openTickets.Remove(pair.Item2); // ticket closed
                        closedTickets.Add(pair.Item2);
                    }
                }

                // pad any remaining time intervals
                while(nextInterval <= lastDayMidnight)
                {
                    results.Add(GetIntervalData(nextInterval, openTickets, closedTickets, ticketsCreated));
                    nextInterval += interval;
                }

            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("AnalyzeDaysOpen failed", e);
            }

            return results;
        }

        private double? Median(HashSet<TicketJoin> tickets)
        {
            try
            {
                double[] totalDays = tickets.Select(ticket => ticket.TotalDaysOpen).ToArray();
                if (totalDays.Length == 0)
                    return null;

                Array.Sort(totalDays);
                int centerIndex = totalDays.Length / 2;
                if (totalDays.Length % 2 == 1)
                    return totalDays[centerIndex];

                return (totalDays[centerIndex - 1] + totalDays[centerIndex]) / 2;
            }
            catch(Exception e)
            {
                CDIEventLog.WriteEntry("Failed Median", e);
            }

            return null;
        }

        private IntervalData GetIntervalData(DateTime nextDay, HashSet<TicketJoin> openTickets, HashSet<TicketJoin> closedTickets, int ticketsCreated)
        {
            // TODO - optimize by previous time open and how long its been since then
            IntervalData sample = new IntervalData()
            {
                _timeStamp = nextDay,
                _ticketsCreated = ticketsCreated,
                _ticketsOpen = openTickets.Count,
                _medianDaysOpen = Median(openTickets),
                _ticketsClosed = closedTickets.Count,
                _medianDaysToClose = Median(closedTickets)
            };

            //sample.Write();
            return sample;
        }

        public void TimeOpenHistogram(TimeScale timeScale)
        {
            TimeOpenHistogram(timeScale, _tickets);
        }

        /// <summary> Count the days open </summary>
        void TimeOpenHistogram(TimeScale timeScale, TicketJoin[] tickets)
        {
            Debug.WriteLine("TimeOpen({0})	TicketCount", timeScale);
            TallyDictionary<int> open = new TallyDictionary<int>();
            foreach (TicketJoin t in tickets)
            {
                int timeOpen = (int)Math.Round(t.ScaledTimeOpen(timeScale));
                open.Increment(timeOpen);
            }

            open.Write();
        }

        /// <summary> Count the ticket types </summary>
        void TicketTypeHistogram(TimeScale timeScale, TicketJoin[] tickets)
        {
            Debug.WriteLine("TicketType({0})	TicketCount", timeScale);
            TallyDictionary<string> open = new TallyDictionary<string>();
            foreach (TicketJoin t in tickets)
                open.Increment(t.TicketTypeName);

            open.Write();
        }
    }
}

