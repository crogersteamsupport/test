using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Generate CDI metrics for an individual organization
    /// </summary>
    class Organization
    {
        public int OrganizationID { get; private set; }
        TicketJoin[] _tickets;
        DateRange _dateRange;
        IntervalStrategy _intervalStrategy;
        public List<IntervalData> IntervalData { get; private set; }
        ICDIStrategy _cdiStrategy;

        public Organization(int organizationID, TicketJoin[] tickets, DateRange analysisInterval)
        {
            OrganizationID = organizationID;
            _dateRange = analysisInterval;
            _tickets = tickets;

            // collect metrics for each interval
            _intervalStrategy = new IntervalStrategy(_tickets);
            IntervalData = _intervalStrategy.GenerateIntervalData(_dateRange);
            _cdiStrategy = new CDIPercentileStrategy(IntervalData);

            // calculate the CDI using normalized data
            foreach (IntervalData intervalData in IntervalData)
                _cdiStrategy.CalculateCDI(intervalData);
        }

    }
}
