using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CDI2
{
    public class CDI
    {
        TimeSpan _analysisInterval;
        DateTime _startDate;
        DateTime _endDate;

        TicketReader _ticketReader;    // cache the tickets for analysis
        Dictionary<int, List<IntervalData>> _organizations;  // raw analysis by organization

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="analysisInterval">interval to sample and average across</param>
        /// <param name="intervalCount">Use an even number of intervals</param>
        /// <param name="isClosed"></param>
        public CDI(TimeSpan analysisInterval, int intervalCount)
        {
            _analysisInterval = analysisInterval;
            _endDate = PreviousMidnight(DateTime.UtcNow);
            _startDate = PreviousMidnight(_endDate - TimeSpan.FromDays(analysisInterval.TotalDays * intervalCount));

            _ticketReader = new TicketReader(_startDate, _endDate);
            _organizations = new Dictionary<int, List<IntervalData>>();
        }

        DateTime PreviousMidnight(DateTime value)
        {
            return value.AddTicks(-(value.Ticks % _analysisInterval.Ticks));
        }

        public void Run()
        {
            CDIEventLog.WriteEntry("CDI Update started...");

            // load all the tickets
            Ticket[] tickets = _ticketReader.Read();
            //List<IntervalData> all = Analyze(tickets);    // complete analysis of all tickets

            // analyze by organization
            int[] organizationIDs = _ticketReader.ReadOrganizationIDs();
            foreach (int organizationID in organizationIDs)
            {
                // extract this organization's tickets from the complete set
                tickets = _ticketReader.Read(organizationID);
                _organizations[organizationID] = Analyze(tickets);
            }
            Write();

            CDIEventLog.WriteEntry("CDI Update complete.");
        }

        /// <summary>
        /// Analyze the tickets and calculate the CDI for each interval
        /// </summary>
        /// <param name="tickets"></param>
        /// <returns></returns>
        List<IntervalData> Analyze(Ticket[] tickets)
        {
            // collect metrics for each interval
            ClosedTicketAnalysis analysis = new ClosedTicketAnalysis(tickets);
            List<IntervalData> analysisIntervals = analysis.AnalyzeDaysOpen(_startDate, _endDate, _analysisInterval);

            // calculate the CDI using normalized data
            IntervalDataDistribution intervalDistribution = new IntervalDataDistribution(analysisIntervals);
            foreach (IntervalData intervalData in analysisIntervals)
                intervalData.CalculateCDI(intervalDistribution);

            return analysisIntervals;
        }

        /// <summary>
        /// Complete summary of CDI by date for all organizations
        /// </summary>
        public void Write()
        {
            // DateTime org1 org2 org3...
            StringBuilder str = new StringBuilder();
            str.Append("DateTime");
            foreach (KeyValuePair<int, List<IntervalData>> pair in _organizations)
                str.Append("\t" + pair.Key);
            Debug.WriteLine(str.ToString());

            for (int i = 0; i < _organizations.First().Value.Count; ++i)
            {
                str.Clear();
                str.Append(_organizations.First().Value[i]._timeStamp.ToShortDateString() + "\t");
                foreach (KeyValuePair<int, List<IntervalData>> pair in _organizations)
                    str.Append(((int)Math.Round(pair.Value[i].CDI)).ToString() + "\t");

                Debug.WriteLine(str.ToString());
            }
        }

    }
}
