using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CDI2
{
    /// <summary>
    /// Keep the results for the analysis of closed tickets for the given time interval
    /// </summary>
    public struct IntervalData
    {
        public DateTime _timeStamp; // date time for this data
        public int _ticketsCreated;    // Tickets Created in time interval
        public int _ticketsOpen;   // Number of Tickets Currently Open
        public double _averageDaysOpen;    // Average Time Tickets have been open in time interval
        public double _averageDaysToClose; // Average Time tickets took to close in time interval
        private double _cdi;

        public void Write()
        {
            Debug.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
                _timeStamp, _ticketsCreated, _ticketsOpen, _averageDaysOpen, _averageDaysToClose, _cdi));
        }

        public override string ToString()
        {
            return _timeStamp.ToString();
        }

        // are we above or below average?
        public void CalculateCDI(IntervalData average)
        {
            if (average._timeStamp != _timeStamp)
                Debugger.Break();

            double created = _ticketsCreated / average._ticketsCreated;
            double open = _ticketsOpen / average._ticketsOpen;
            double daysOpen = _averageDaysOpen / average._averageDaysOpen;
            double daysToClose = _averageDaysToClose / average._averageDaysToClose;
            _cdi = (created + open + daysOpen + daysToClose) / 4;
        }
    }

}
