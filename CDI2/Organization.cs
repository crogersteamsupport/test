using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDI2
{
    class Organization
    {
        public int OrganizationID { get; private set; }
        TicketJoin[] _tickets;
        AnalysisInterval _interval;
        public List<IntervalData> AnalysisIntervalData { get; private set; }
        IntervalDataDistribution _distribution;

        public Organization(TicketJoin[] tickets, int organizationID, AnalysisInterval interval)
        {
            OrganizationID = organizationID;
            _interval = interval;
            _tickets = tickets;

            // collect metrics for each interval
            ClosedTicketAnalysis analysis = new ClosedTicketAnalysis(_tickets);
            AnalysisIntervalData = analysis.AnalyzeDaysOpen(interval);
 
            // calculate the CDI using normalized data
            _distribution = new IntervalDataDistribution(AnalysisIntervalData);
            foreach (IntervalData intervalData in AnalysisIntervalData)
                intervalData.CalculateCDI(_distribution);
        }

    }
}
