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
        //List<IntervalData> _all;    // raw analysis of all tickets
        Dictionary<int, List<IntervalData>> _organizations;  // raw analysis by organization

        public CDI(int calendarDaysToAnalyze, TimeSpan analysisInterval, bool isClosed)
        {
            _calendarDaysToAnalyze = calendarDaysToAnalyze;
            _analysisInterval = analysisInterval;
            _isClosed = isClosed;
            _ticketReader = new TicketReader(_calendarDaysToAnalyze, _isClosed);
            //_all = null;
            _organizations = new Dictionary<int, List<IntervalData>>();
        }

        public void Run()
        {
            CDIEventLog.WriteEntry("CDI Update started...");

            // load the data to be analyzed
            Ticket[] tickets = _ticketReader.Read();
            DateTime firstDayMidnight = tickets[0].DateCreated;
            firstDayMidnight = firstDayMidnight.AddTicks(-(firstDayMidnight.Ticks % _analysisInterval.Ticks));
            DateTime lastDayMidnight = firstDayMidnight.AddDays(_calendarDaysToAnalyze);

            // analyze the data
            ClosedTicketAnalysis ticketAnalysis = new ClosedTicketAnalysis(tickets);
            //_all = ticketAnalysis.AnalyzeDaysOpen(firstDayMidnight, lastDayMidnight, _analysisInterval);

            // analyze by organization
            int[] organizationIDs = _ticketReader.ReadOrganizationIDs();
            foreach (int organizationID in organizationIDs)
            {
                tickets = _ticketReader.Read(organizationID);
                ClosedTicketAnalysis organizationAnalysis = new ClosedTicketAnalysis(tickets);
                _organizations[organizationID] = organizationAnalysis.AnalyzeDaysOpen(firstDayMidnight, lastDayMidnight, _analysisInterval);
            }

            CalculateCDI();
            CDIEventLog.WriteEntry("CDI Update complete.");
        }

        void CalculateCDI()
        {
            // each interval for each organization
            foreach (KeyValuePair<int, List<IntervalData>> pair in _organizations)
            {
                IntervalDataAverage organizationAverage = new IntervalDataAverage(pair.Value);
                foreach (IntervalData intervalData in pair.Value)
                {
                    intervalData.CalculateCDI(organizationAverage);

                    //if (pair.Key == 536001) // test cases: Walmart 1085741, Ecsi 536001
                    //    intervalData.Write();
                }
            }
        }

    }
}
