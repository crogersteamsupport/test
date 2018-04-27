using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Analyze the last year of tickets
    /// Do calculations in days to prevent Ticks overflow
    /// </summary>
    class IntervalStrategy
    {
        TicketJoin[] _tickets;

        public IntervalStrategy(TicketJoin[] tickets)
        {
            _tickets = tickets;
        }

        /// <summary> organize tickets for each open and close </summary>
        List<Tuple<DateTime, TicketJoin>> GetAsChronological()
        {
            // insert each ticket by DateCreated and DateClosed (duplicates allowed)
            List<Tuple<DateTime, TicketJoin>> chronological = new List<Tuple<DateTime, TicketJoin>>();   // allow duplicates for DateTime
            foreach (TicketJoin t in _tickets)
            {
                chronological.Add(new Tuple<DateTime, TicketJoin>(t.DateCreated, t));  // opened
                if(t.DateClosed.HasValue)
                    chronological.Add(new Tuple<DateTime, TicketJoin>(t.DateClosed.Value, t));  // closed
            }

            // sort by date and TicketID
            chronological.Sort((x, y) =>
            {
                int result = x.Item1.CompareTo(y.Item1);
                return result == 0 ? x.Item2.CompareTo(y.Item2) : result;
            });

            return chronological;
        }

        /// <summary>
        /// Sample the days open at the specified interval
        /// </summary>
        /// <param name="dateRange">how often do we sample - daily, weekly,... </param>
        public List<IntervalData> GenerateIntervalData(DateRange dateRange)
        {
            List<IntervalData> results = new List<IntervalData>();
            try
            {
                // tickets open at that moment in time
                HashSet<TicketJoin> currentlyOpenTickets = new HashSet<TicketJoin>();    // tickets open at that moment in time
                HashSet<TicketJoin> intervalClosedTickets = new HashSet<TicketJoin>();  // tickets closed in that time interval
                int newTicketsCount = 0;

                // spin through all the create/close times and keep a running tally
                List<Tuple<DateTime, TicketJoin>> chronological = GetAsChronological();
                DateTime nextInterval = dateRange.StartDate + dateRange.IntervalTimeSpan;

                // pad while we have no data
                while (nextInterval < chronological[0].Item1)
                {
                    results.Add(new IntervalData(nextInterval));
                    nextInterval += dateRange.IntervalTimeSpan;
                }

                foreach (Tuple<DateTime, TicketJoin> pair in chronological)
                {
                    if (pair.Item1 > nextInterval)
                    {
                        // sample the data at this time
                        results.Add(new IntervalData(nextInterval, currentlyOpenTickets, intervalClosedTickets, newTicketsCount));
                        intervalClosedTickets.Clear();
                        newTicketsCount = 0;
                        nextInterval += dateRange.IntervalTimeSpan;
                    }

                    // running list of open and closed tickets
                    if (pair.Item1 == pair.Item2.DateCreated)
                    {
                        currentlyOpenTickets.Add(pair.Item2);    // new ticket opened
                        ++newTicketsCount;
                    }
                    else
                    {
                        currentlyOpenTickets.Remove(pair.Item2); // ticket closed
                        intervalClosedTickets.Add(pair.Item2);
                    }
                }

                // pad where we have no data
                while(nextInterval <= dateRange.EndDate)
                {
                    results.Add(new IntervalData(nextInterval));
                    nextInterval += dateRange.IntervalTimeSpan;
                }
            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("AnalyzeDaysOpen failed", e);
            }

            return results;
        }

        /// <summary> Count the days open </summary>
        void TimeOpenHistogram(TimeScale timeScale)
        {
            Debug.WriteLine("TimeOpen({0})	TicketCount", timeScale);
            TallyDictionary<int> open = new TallyDictionary<int>();
            foreach (TicketJoin t in _tickets)
            {
                int timeOpen = (int)Math.Round(t.ScaledTimeOpen(timeScale));
                open.Increment(timeOpen);
            }

            open.Write();
        }

        /// <summary> Count the ticket types </summary>
        void TicketTypeHistogram(TimeScale timeScale)
        {
            Debug.WriteLine("TicketType({0})	TicketCount", timeScale);
            TallyDictionary<string> open = new TallyDictionary<string>();
            foreach (TicketJoin t in _tickets)
                open.Increment(t.TicketTypeName);

            open.Write();
        }
    }
}

