using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CDI2
{
    /// <summary>
    /// Average data for IntervalData
    /// 
    /// High = good, low = bad
    /// </summary>
    public class IntervalDataDistribution
    {
        int[] _ticketsCreated; // new (this interval)
        int[] _ticketsOpen;    // currently Open (since start)
        double[] _averageOfMedianDaysOpen; // average of still open
        int[] _ticketsClosed;  // closed (this interval)
        double[] _averageOfMedianDaysToClose;  // average time to close (this interval)

        const int PercentileBuckets = 20;

        public int TicketsCreated(int value)
        {
            int index = Array.BinarySearch(_ticketsCreated, value);
            if (index < 0)
                index = ~index - 1;
            return 100 - (index * 100/PercentileBuckets);   // high is bad
        }

        public int TicketsOpen(int value)
        {
            int index = Array.BinarySearch(_ticketsOpen, value);
            if (index < 0)
                index = ~index - 1;
            return 100 - (index * 100/PercentileBuckets);   // high is bad
        }

        public int AverageDaysOpen(double value)
        {
            int index = Array.BinarySearch(_averageOfMedianDaysOpen, value);
            if (index < 0)
                index = ~index - 1;
            return 100 - (index * 100 / PercentileBuckets);   // high is bad
        }

        public int TicketsClosed(int value)
        {
            int index = Array.BinarySearch(_ticketsClosed, value);
            if (index < 0)
                index = ~index - 1;
            return index * 100 / PercentileBuckets;
        }

        public int MedianDaysToClose(double value)
        {
            int index = Array.BinarySearch(_averageOfMedianDaysToClose, value);
            if (index < 0)
                index = ~index - 1;
            return 100 - (index * 100 / PercentileBuckets);   // high is bad
        }

        public IntervalDataDistribution(List<IntervalData> organization)
        {
            // _ticketsCreated
            organization.Sort((lhs, rhs) => lhs._ticketsCreated.CompareTo(rhs._ticketsCreated));
            _ticketsCreated = new int[PercentileBuckets + 1];    // 0%, 10%, 20%,... 100%
            for (int i = 0; i <= PercentileBuckets; ++i)
                _ticketsCreated[i] = organization[(int)Math.Round((double)(organization.Count - 1) * i / PercentileBuckets)]._ticketsCreated;

            // _ticketsOpen
            organization.Sort((lhs, rhs) => lhs._ticketsOpen.CompareTo(rhs._ticketsOpen));
            _ticketsOpen = new int[PercentileBuckets + 1];
            for (int i = 0; i <= PercentileBuckets; ++i)
                _ticketsOpen[i] = organization[(int)Math.Round((double)(organization.Count - 1) * i / PercentileBuckets)]._ticketsOpen;

            // _medianDaysOpen
            _averageOfMedianDaysOpen = new double[PercentileBuckets + 1];
            var tmp = organization.Where(t => t._medianDaysOpen.HasValue).ToArray();
            if (tmp.Length > 0)
            {
                Array.Sort(tmp, (lhs, rhs) => lhs._medianDaysOpen.Value.CompareTo(rhs._medianDaysOpen.Value));
                for (int i = 0; i <= PercentileBuckets; ++i)
                    _averageOfMedianDaysOpen[i] = tmp[(int)Math.Round((double)(tmp.Length - 1) * i / PercentileBuckets)]._medianDaysOpen.Value;
            }

            // _ticketsClosed
            organization.Sort((lhs, rhs) => lhs._ticketsClosed.CompareTo(rhs._ticketsClosed));
            _ticketsClosed = new int[PercentileBuckets + 1];
            for (int i = 0; i <= PercentileBuckets; ++i)
                _ticketsClosed[i] = organization[(int)Math.Round((double)(organization.Count - 1) * i / PercentileBuckets)]._ticketsClosed;

            // _medianDaysToClose
            _averageOfMedianDaysToClose = new double[PercentileBuckets + 1];
            var tmp1 = organization.Where(t => t._medianDaysToClose.HasValue).ToArray();
            if(tmp1.Length > 0)
            {
                Array.Sort(tmp1, (lhs, rhs) => lhs._medianDaysToClose.Value.CompareTo(rhs._medianDaysToClose.Value));
                for (int i = 0; i <= PercentileBuckets; ++i)
                    _averageOfMedianDaysToClose[i] = tmp1[(int)Math.Round((double)(tmp1.Length - 1) * i / PercentileBuckets)]._medianDaysToClose.Value;
            }

            // leave sorted by date
            organization.Sort((lhs, rhs) => lhs._timeStamp.CompareTo(rhs._timeStamp));

            //Write();
        }

        //void Write()
        //{
        //    Debug.WriteLine("Percentile\tTicketsCreated\tTicketsOpen\tAverageDaysOpen\tTicketsClosed\tAverageDaysToClose")
        //    for(int i = 0; i <= 100; i += 100 / PercentileBuckets)
        //    {
        //        Debug.Write(i)
        //    }
        //}
    }

}
