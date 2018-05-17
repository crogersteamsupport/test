using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.CDI
{
    class ClientPercentileStrategy : ICDIStrategy
    {
        List<IntervalData> _intervals;
        int _ticketCount;
        double _intervalScalar;
        public CustomerPercentileStrategy _parentStrategy { get; set; }

        public ClientPercentileStrategy(List<IntervalData> intervals, ICDIStrategy iCdiStrategy, int ticketCount)
        {
            _intervals = intervals;
            _parentStrategy = iCdiStrategy as CustomerPercentileStrategy;
            _ticketCount = ticketCount;
            _intervalScalar = _parentStrategy.TicketCount / _ticketCount;
        }

        // scale relative to Customer
        IntervalData ScaleInterval(IntervalData interval)
        {
            interval._closedCount = (int)Math.Round(interval._closedCount * _intervalScalar);
            interval._newCount = (int)Math.Round(interval._newCount * _intervalScalar);
            interval._openCount = (int)Math.Round(interval._openCount * _intervalScalar);
            return interval;
        }

        public bool CalculateCDI()
        {
            DateTime timestamp = _parentStrategy.IntervalTimestamp;
            IntervalData interval = _intervals.Where(i => i._timeStamp == timestamp).FirstOrDefault();
            return (interval != null) ? _parentStrategy.CalculateCDI(ScaleInterval(interval)) : false;
        }

    }
}
