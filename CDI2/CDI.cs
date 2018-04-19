using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CDI2
{
    public class CDI
    {
        int _calendarDaysToAnalyze;  // time span to analyze (days)
        TimeSpan _analysisInterval;
        bool _isClosed; // read only closed or only open records
        TicketReader _ticketReader;    // cache the tickets for analysis
        List<IntervalData> _all;    // raw analysis of all tickets
        Dictionary<int, List<IntervalData>> _organization;  // raw analysis by organization

        public CDI(int calendarDaysToAnalyze, TimeSpan analysisInterval, bool isClosed)
        {
            _calendarDaysToAnalyze = calendarDaysToAnalyze;
            _analysisInterval = analysisInterval;
            _isClosed = isClosed;
            _ticketReader = new TicketReader(_calendarDaysToAnalyze, _isClosed);
            _all = null;
            _organization = new Dictionary<int, List<IntervalData>>();
        }

        public void Run()
        {
            // load the data to be analyzed
            Ticket[] tickets = _ticketReader.Read();
            DateTime firstDayMidnight = tickets[0].DateCreated;
            firstDayMidnight = firstDayMidnight.AddTicks(-(firstDayMidnight.Ticks % _analysisInterval.Ticks));
            DateTime lastDayMidnight = firstDayMidnight.AddDays(_calendarDaysToAnalyze);

            // analyze the data
            ClosedTicketAnalysis ticketAnalysis = new ClosedTicketAnalysis(tickets);
            _all = ticketAnalysis.AnalyzeDaysOpen(firstDayMidnight, lastDayMidnight, _analysisInterval);

            // analyze by organization
            int[] organizationIDs = _ticketReader.ReadOrganizationIDs();
            foreach (int organizationID in organizationIDs)
            {
                tickets = _ticketReader.Read(organizationID); // test cases: Walmart 1085741, Ecsi 536001
                ClosedTicketAnalysis organizationAnalysis = new ClosedTicketAnalysis(tickets);
                _organization[organizationID] = organizationAnalysis.AnalyzeDaysOpen(firstDayMidnight, lastDayMidnight, _analysisInterval);
            }

            CalculateCDI();
        }

        void CalculateCDI()
        {
            for (int i = 0; i < _all.Count; ++i)
            {
                IntervalData average = new IntervalData()
                {
                    _timeStamp = _all[i]._timeStamp,
                    _ticketsCreated = _all[i]._ticketsCreated / _organization.Count,
                    _ticketsOpen = _all[i]._ticketsOpen / _organization.Count,
                    _averageDaysOpen = _all[i]._averageDaysOpen,
                    _averageDaysToClose = _all[i]._averageDaysToClose
                };

                foreach (KeyValuePair<int, List<IntervalData>> pair in _organization)
                {
                    pair.Value[i].CalculateCDI(average);

                    if (pair.Key == 536001) // test cases: Walmart 1085741, Ecsi 536001
                        pair.Value[i].Write();
                }
            }
        }

    }
}
