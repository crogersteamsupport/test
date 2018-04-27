using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Use percentiles of the current value in this organization's distribution 
    /// </summary>
    public class CDIPercentileStrategy : ICDIStrategy
    {
        Percentile<int> _newCount;  // new (this interval)
        Percentile<int> _openCount; // currently Open (since start)
        Percentile<double> _daysOpen;   // average of still open
        Percentile<int> _closedCount;   // closed (this interval)
        Percentile<double> _daysToClose;    // average time to close (this interval)
        Percentile<double> _closedActionsCount;   // count of actions per ticket

        public CDIPercentileStrategy(List<IntervalData> intervalData)
        {
            _newCount = new Percentile<int>(intervalData, delegate (IntervalData x) { return x._newTicketsCount; });
            _openCount = new Percentile<int>(intervalData, delegate (IntervalData x) { return x._openTicketsCount; });
            _daysOpen = new Percentile<double>(intervalData, delegate (IntervalData x) { return x._medianOpenTicketsDaysOpen; });
            _closedCount = new Percentile<int>(intervalData, delegate (IntervalData x) { return x._closedTicketsCount; });

            // only consider closed tickets which have 
            var closedTickets = intervalData.Where(t => (t != null) && t._closedTicketsActionsCount.HasValue).ToList();
            _closedActionsCount = new Percentile<double>(closedTickets, delegate (IntervalData x) { return x._closedTicketsActionsCount.Value; });

            // only consider the intervals that have closed data
            closedTickets = intervalData.Where(t => (t != null) && t._medianDaysToClose.HasValue).ToList();
            _closedActionsCount = new Percentile<double>(closedTickets, delegate (IntervalData x) { return x._closedTicketsActionsCount.Value; });
            _daysToClose = new Percentile<double>(closedTickets, delegate (IntervalData x) { return x._medianDaysToClose.Value; });
        }

        // TODO
        //  * time to respond (first response from customer service)
        //  * count of actions (long ticket = bad)
        public void CalculateCDI(IntervalData intervalData)
        {
            // don't calculate the CDI if it is an interval with no data
            // (customer hasn't started contract, or customer has discontinued)
            if(!intervalData.HasData)
                return;

            List<double> metrics = new List<double>();
            metrics.Add(_newCount.AsPercentile(intervalData._newTicketsCount));
            metrics.Add(_openCount.AsPercentile(intervalData._openTicketsCount));
            metrics.Add(_closedCount.AsPercentile(intervalData._closedTicketsCount, false));    // the higher the better
            metrics.Add(_daysOpen.AsPercentile(intervalData._medianOpenTicketsDaysOpen));

            // closed tickets
            if (intervalData._medianDaysToClose.HasValue)
                metrics.Add(_daysToClose.AsPercentile(intervalData._medianDaysToClose.Value));
            if (intervalData._closedTicketsActionsCount.HasValue)
                metrics.Add(_daysOpen.AsPercentile(intervalData._closedTicketsActionsCount.Value));

            intervalData.CDI = (int)Math.Round(metrics.Average() * 10);  // [0, 1000]
        }
    }

}
