using System;
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
        Ticket[] _tickets;
        public double AvgTimeToClose { get; private set; }
        public double StdevTimeToClose { get; private set; }   // days
        List<Tuple<DateTime, Ticket>> _chronological;   // chronological listing of each open and close

        public ClosedTicketAnalysis(Ticket[] tickets)
        {
            _tickets = tickets;
            try
            {
                InitStatistics();
                InitChronological();
            }
            catch (Exception e)
            {
                Debugger.Break();
            }
        }

        /// <summary> Average days open and StDev </summary>
        void InitStatistics()
        {
            // average
            AvgTimeToClose = _tickets.Where(t => t.DateClosed.HasValue).Average(t => t.TotalDaysOpen);

            // standard deviation
            double denominator = 0;
            foreach (Ticket t in _tickets)
            {
                if (!t.DateClosed.HasValue)
                    continue;

                double xdiff = t.TotalDaysOpen - AvgTimeToClose;
                denominator += xdiff * xdiff;
            }
            StdevTimeToClose = Math.Sqrt(denominator / _tickets.Length);
        }

        /// <summary> organize tickets for each open and close </summary>
        void InitChronological()
        {
            // TODO - use array double the size and sort?

            // insert each ticket by DateCreated and DateClosed (duplicates allowed)
            _chronological = new List<Tuple<DateTime, Ticket>>();   // allow duplicates for DateTime
            foreach (Ticket t in _tickets)
            {
                if ((t.DateCreated == t.DateClosed) || !t.DateClosed.HasValue)
                    continue;

                _chronological.Add(new Tuple<DateTime, Ticket>(t.DateCreated, t));  // opened
                _chronological.Add(new Tuple<DateTime, Ticket>(t.DateClosed.Value, t));  // closed
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
            //Debug.WriteLine("Date\tOpenCount\tAvgOpenTime(days)\tAvgTimeToClose(days)");
            try
            {
                DateTime nextInterval = firstDayMidnight + interval;

                // tickets open at that moment in time
                HashSet<Ticket> openTickets = new HashSet<Ticket>();    // HashSet is faster than List for big data
                HashSet<Ticket> closedTickets = new HashSet<Ticket>();
                int ticketsCreated = 0;
                results = new List<IntervalData>();
                foreach (Tuple<DateTime, Ticket> pair in _chronological)
                {
                    if (pair.Item1 > nextInterval)
                    {
                        // sample the data at this time
                        results.Add(GetIntervalData(nextInterval, openTickets, closedTickets, ticketsCreated));
                        closedTickets.Clear();
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
                Debugger.Break();
            }

            return results;
        }

        private IntervalData GetIntervalData(DateTime nextDay, HashSet<Ticket> openTickets, HashSet<Ticket> closedTickets, int ticketsCreated)
        {
            // TODO - optimize by previous time open and how long its been since then
            IntervalData sample = new IntervalData()
            {
                _timeStamp = nextDay,
                _ticketsCreated = ticketsCreated,
                _ticketsOpen = openTickets.Count,
                _averageDaysOpen = (openTickets.Count <= 0) ? 0 : openTickets.Average(ticket => ticket.TotalDaysOpen),
                _averageDaysToClose = (closedTickets.Count <= 0) ? 0 : closedTickets.Average(ticket => ticket.TotalDaysOpen)
            };

            //sample.Write();
            return sample;
        }

        public void TimeOpenHistogram(TimeScale timeScale)
        {
            TimeOpenHistogram(timeScale, _tickets);
        }

        /// <summary> Count the days open </summary>
        void TimeOpenHistogram(TimeScale timeScale, Ticket[] tickets)
        {
            Debug.WriteLine("TimeOpen({0})	TicketCount", timeScale);
            TallyDictionary open = new TallyDictionary();
            //int droppedRecords = 0;
            foreach (Ticket t in tickets)
            {
                // no data?
                if (!t.DateClosed.HasValue || (t.TotalDaysOpen <= 0.0))
                {
                    //++droppedRecords;
                    continue;
                }

                int timeOpen = (int)Math.Round(t.ScaledTimeOpen(timeScale));
                open.Increment(timeOpen);
            }

            open.Write();
        }

        /// <summary> Count the days open </summary>
        void TicketTypeHistogram(TimeScale timeScale, Ticket[] tickets)
        {
            Debug.WriteLine("TicketType({0})	TicketCount", timeScale);
            TallyDictionary open = new TallyDictionary();
            foreach (Ticket t in tickets)
                open.Increment(t.TicketTypeID);

            open.Write();
        }
    }
}

