using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Linq;

namespace TeamSupport.CDI
{
    public struct CDI1
    {
        public int TotalTicketsCreated;
        public int TicketsOpen;
        public int CreatedLast30;
        public int AvgTimeOpen;
        public int AvgTimeToClose;
        public int CustDisIndex;
    };

    class CDI1Strategy : ICDIStrategy
    {
        OrganizationAnalysis _organizationAnalysis;
        IntervalData _rawMetrics;
        IntervalData _normalizedMetrics;

        public CDI1Strategy(OrganizationAnalysis organizationAnalysis)
        {
            _organizationAnalysis = organizationAnalysis;
        }

        public IntervalData RawMetrics { get { return _rawMetrics; } }
        public IntervalData NormalizedMetrics { get { return _normalizedMetrics; } }

        public IntervalData CalculateRawMetrics()
        {
            IntervalData end = _organizationAnalysis.Current();
            if (end == null)
            {
                IntervalData last = _organizationAnalysis.Intervals.Last();
                _rawMetrics = new IntervalData()
                {
                    _timeStamp = _organizationAnalysis._dateRange.EndDate,
                    _totalTicketsCreated = last._totalTicketsCreated,
                    _newCount = 0,
                    _openCount = last._openCount,
                    _closedCount = 0,
                    _medianDaysOpen = _organizationAnalysis.MedianDaysOpen(),
                    _averageSeverity = last._averageSeverity,
                    _averageActionCount = last._averageActionCount,
                    _averageSentimentScore = last._averageSentimentScore
                };
            }
            else
            {
                _rawMetrics = new IntervalData()
                {
                    _timeStamp = end._timeStamp,
                    _totalTicketsCreated = end._totalTicketsCreated, // 1. TotalTicketsCreated
                    _newCount = end._newCount, // 2. CreatedLast30
                    _openCount = end._openCount,   // 3. TicketsOpen
                    _closedCount = end._closedCount,
                    _medianDaysOpen = end._medianDaysOpen, // 4. AvgTimeOpen
                    _averageSeverity = end._averageSeverity,
                    _averageActionCount = end._averageActionCount,
                    _averageSentimentScore = end._averageSentimentScore
                };
            }

            _rawMetrics._medianDaysToClose = IntervalData.MedianTotalDaysToClose(_organizationAnalysis.Tickets);  // 5. AvgTimeToClose
            return _rawMetrics;
        }

        /// <summary> 
        /// CDI2 
        /// temporarily turn off weights...
        /// </summary>
        public void InvokeCDIStrategy2(IntervalPercentiles clientPercentiles, linq.CDI_Settings weights)
        {
            _normalizedMetrics = clientPercentiles.Normalize(_rawMetrics);

            List<double> result = new List<double>();
            foreach (Metrics metric in Enum.GetValues(typeof(Metrics)))
            {
                double? value = _normalizedMetrics.Get(metric); // * weights.Get(metric)
                double? weight = weights.Get(metric);

                if (value.HasValue)
                    result.Add(value.Value);
            }
            _normalizedMetrics.CDI = _rawMetrics.CDI = (int)Math.Round(result.Average());
        }

        /// <summary> CDI1 </summary>
        public void InvokeCDIStrategy(IntervalPercentiles clientPercentiles, linq.CDI_Settings weights)
        {
            _normalizedMetrics = clientPercentiles.Normalize(_rawMetrics);

            double result = 0;
            foreach (Metrics metric in Enum.GetValues(typeof(Metrics)))
            {
                double? value = _normalizedMetrics.Get(metric);
                double? weight = weights.Get(metric);
                if (value.HasValue && weight.HasValue)
                    result += value.Value * weight.Value;
            }

            // square it to get a better distribution?
            _normalizedMetrics.CDI = (int)Math.Round(result);
            CDIEventLog.WriteLine("4\t{0}", _normalizedMetrics.CDI);
        }

        public string ToStringCDI1()
        {
            //return _organizationAnalysis.ToString();
            if (_rawMetrics == null)
                return string.Empty;
            return _rawMetrics.ToStringCDI1();
        }

        /// <summary>
        /// The linq to sql save is very slow, including a WHERE clause for an exact record match.
        /// Thus use a hard-coded update
        /// </summary>
        /// <param name="db"></param>
        public void Save(DataContext db)
        {
            // Organization
            string updateQuery = String.Format(@"UPDATE Organizations SET TotalTicketsCreated = {0}, TicketsOpen = {1}, CreatedLast30 = {2}, AvgTimeOpen = {3}, AvgTimeToClose = {4}, CustDisIndex = {5}, CustDistIndexTrend = {6} WHERE OrganizationID = {7}",
                _rawMetrics._totalTicketsCreated,  // TotalTicketsCreated
                _rawMetrics._openCount,    // TicketsOpen
                _rawMetrics._newCount, // CreatedLast30
                (int)Math.Round(_rawMetrics._medianDaysOpen),  // AvgTimeOpen
                _rawMetrics._medianDaysToClose.HasValue ? (int)Math.Round(_rawMetrics._medianDaysToClose.Value) : 0,  // AvgTimeToClose
                _rawMetrics.CDI.Value, // CustDisIndex
                0,  // CustDistIndexTrend
                _organizationAnalysis.OrganizationID);    // OrganizationID
            db.ExecuteCommand(updateQuery);
        }

        public void Save(Table<linq.CustDistHistory> table)
        {
            try
            {
                linq.CustDistHistory history = new linq.CustDistHistory()
                {
                    ParentOrganizationID = _organizationAnalysis.ParentID,
                    OrganizationID = _organizationAnalysis.OrganizationID,
                    CDIDate = DateRange.EndTimeNow,
                    CDIValue = _rawMetrics.CDI.Value
                };
                table.InsertOnSubmit(history);
            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("Save failed", e);
            }
        }

        public double GetCDI(CDI1 metrics, linq.CDI_Settings settings)
        {
            return metrics.TotalTicketsCreated * settings.TotalTicketsWeight.Value +
                        metrics.TicketsOpen * settings.OpenTicketsWeight.Value +
                        metrics.CreatedLast30 * settings.Last30Weight.Value +
                        metrics.AvgTimeOpen * settings.AvgDaysOpenWeight.Value +
                        metrics.AvgTimeToClose * settings.AvgDaysToCloseWeight.Value;
        }

        public int GetCDI1(IntervalData interval, IntervalData normalized, linq.CDI_Settings weights, Dictionary<Metrics, Percentile> _percentiles)
        {
            // Create the CDI from the normalized fields
            CDI1 cdi = new CDI1()
            {
                TotalTicketsCreated = _percentiles[Metrics.TotalTickets].AsPercentile(interval._totalTicketsCreated),
                TicketsOpen = _percentiles[Metrics.Open].AsPercentile(interval._openCount),
                CreatedLast30 = _percentiles[Metrics.New].AsPercentile(interval._newCount),
                AvgTimeOpen = _percentiles[Metrics.DaysOpen].AsPercentile(interval._medianDaysOpen),
            };

            if ((_percentiles[Metrics.DaysToClose] != null) && interval._medianDaysToClose.HasValue)
                cdi.AvgTimeToClose = _percentiles[Metrics.DaysToClose].AsPercentile(interval._medianDaysToClose.Value);

            double CustDisIndex = GetCDI(cdi, weights);
            normalized.CDI = (int)Math.Round(CustDisIndex);
            return normalized.CDI.Value;
        }

    }
}
