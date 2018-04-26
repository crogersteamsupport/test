using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TeamSupport.CDI
{
    interface ICDIStrategy
    {
        void CalculateCDI(IntervalData intervalData);
    }

    /// <summary>
    /// Average data for IntervalData
    /// 
    /// High = good, low = bad
    /// </summary>
    public class CDIPercentileStrategy : ICDIStrategy
    {
        int[] _ticketsCreated; // new (this interval)
        int[] _ticketsOpen;    // currently Open (since start)
        double[] _medianDaysOpen; // average of still open
        int[] _ticketsClosed;  // closed (this interval)
        double[] _medianDaysToClose;  // average time to close (this interval)

        const int PercentileBucketCount = 20;

        public int TicketsCreated(int value)
        {
            int index = Array.BinarySearch(_ticketsCreated, value);
            if (index < 0)
                index = ~index - 1;
            return 100 - (index * 100/PercentileBucketCount);   // high is bad
        }

        public int TicketsOpen(int value)
        {
            int index = Array.BinarySearch(_ticketsOpen, value);
            if (index < 0)
                index = ~index - 1;
            return 100 - (index * 100/PercentileBucketCount);   // high is bad
        }

        public int AverageDaysOpen(double value)
        {
            int index = Array.BinarySearch(_medianDaysOpen, value);
            if (index < 0)
                index = ~index - 1;
            return 100 - (index * 100 / PercentileBucketCount);   // high is bad
        }

        public int TicketsClosed(int value)
        {
            int index = Array.BinarySearch(_ticketsClosed, value);
            if (index < 0)
                index = ~index - 1;
            return index * 100 / PercentileBucketCount;
        }

        public int MedianDaysToClose(double value)
        {
            int index = Array.BinarySearch(_medianDaysToClose, value);
            if (index < 0)
                index = ~index - 1;
            return 100 - (index * 100 / PercentileBucketCount);   // high is bad
        }

        public CDIPercentileStrategy(List<IntervalData> organization)
        {
            // _ticketsCreated
            organization.Sort((lhs, rhs) => lhs._newTicketsCount.CompareTo(rhs._newTicketsCount));
            _ticketsCreated = new int[PercentileBucketCount + 1];    // 0%, 10%, 20%,... 100%
            for (int i = 0; i <= PercentileBucketCount; ++i)
                _ticketsCreated[i] = organization[(int)Math.Round((double)(organization.Count - 1) * i / PercentileBucketCount)]._newTicketsCount;

            // _ticketsOpen
            organization.Sort((lhs, rhs) => lhs._openTicketsCount.CompareTo(rhs._openTicketsCount));
            _ticketsOpen = new int[PercentileBucketCount + 1];
            for (int i = 0; i <= PercentileBucketCount; ++i)
                _ticketsOpen[i] = organization[(int)Math.Round((double)(organization.Count - 1) * i / PercentileBucketCount)]._openTicketsCount;

            // _medianDaysOpen
            organization.Sort((lhs, rhs) => lhs._medianOpenTicketsDaysOpen.CompareTo(rhs._medianOpenTicketsDaysOpen));
            _medianDaysOpen = new double[PercentileBucketCount + 1];   // defaults to 0.0
            for (int i = 0; i <= PercentileBucketCount; ++i)
                _medianDaysOpen[i] = organization[(int)Math.Round((double)(_medianDaysOpen.Length - 1) * i / PercentileBucketCount)]._medianOpenTicketsDaysOpen;

            // _ticketsClosed
            organization.Sort((lhs, rhs) => lhs._closedTicketsCount.CompareTo(rhs._closedTicketsCount));
            _ticketsClosed = new int[PercentileBucketCount + 1];
            for (int i = 0; i <= PercentileBucketCount; ++i)
                _ticketsClosed[i] = organization[(int)Math.Round((double)(organization.Count - 1) * i / PercentileBucketCount)]._closedTicketsCount;

            // _medianDaysToClose
            _medianDaysToClose = new double[PercentileBucketCount + 1];
            var tmp1 = organization.Where(t => t._medianDaysToClose.HasValue).ToArray();
            if(tmp1.Length > 0)
            {
                Array.Sort(tmp1, (lhs, rhs) => lhs._medianDaysToClose.Value.CompareTo(rhs._medianDaysToClose.Value));
                for (int i = 0; i <= PercentileBucketCount; ++i)
                    _medianDaysToClose[i] = tmp1[(int)Math.Round((double)(tmp1.Length - 1) * i / PercentileBucketCount)]._medianDaysToClose.Value;
            }

            // leave sorted by date
            organization.Sort((lhs, rhs) => lhs._intervalEndTimeStamp.CompareTo(rhs._intervalEndTimeStamp));

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

        /// <summary> Average days open and StDev </summary>
        public static double StandardDeviationTotalDaysOpen(TicketJoin[] _tickets)
        {
            // average
            double AvgTimeToClose = _tickets.Average(t => t.TotalDaysOpen);

            // standard deviation
            double denominator = 0;
            foreach (TicketJoin t in _tickets)
            {
                double xdiff = t.TotalDaysOpen - AvgTimeToClose;
                denominator += xdiff * xdiff;
            }
            return Math.Sqrt(denominator / _tickets.Length);
        }

        /// <summary>
        /// get a +-% from the average
        /// </summary>
        /// <param name="percentile"></param>
        public void CalculateCDI(IntervalData intervalData)
        {
            // TODO
            //  * time to respond (first response from customer service)
            //  * Happy face
            //  * count of actions (long ticket = bad)

            //double[] metrics = new double[(int)Metric.DaysToClose + 1];
            //metrics[(int)Metric.Created] = TicketsCreated(_ticketsCreated);
            //metrics[(int)Metric.Open] = TicketsOpen(_ticketsOpen);
            //metrics[(int)Metric.DaysOpen] = AverageDaysOpen(_averageDaysOpen);
            //metrics[(int)Metric.Closed] = TicketsClosed(_ticketsClosed);
            //metrics[(int)Metric.DaysToClose] = MedianDaysToClose(_medianDaysToClose);

            double cdi = (TicketsCreated(intervalData._newTicketsCount) + TicketsClosed(intervalData._closedTicketsCount)) / 2;
            intervalData.CDI = (int)Math.Round(cdi * 10);  // [0, 1000]
            //Write();
        }
    }

}
