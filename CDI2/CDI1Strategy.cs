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
        Metrics _rawMetrics;
        Metrics _normalizedMetrics;

        public CDI1Strategy(OrganizationAnalysis organizationAnalysis)
        {
            _organizationAnalysis = organizationAnalysis;
        }

        public Metrics RawMetrics { get { return _rawMetrics; } }
        public Metrics NormalizedMetrics { get { return _normalizedMetrics; } }
        public int? CDI { get { return RawMetrics.CDI; } }  // note that _rawMetrics.CDI == _normalizedMetrics.CDI

        public Metrics CalculateRawMetrics()
        {
            Metrics end = _organizationAnalysis.Current();
            if (end == null)
            {
                Metrics last = _organizationAnalysis.Intervals.Last();
                _rawMetrics = new Metrics()
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
                _rawMetrics = new Metrics()
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

            _rawMetrics._medianDaysToClose = Metrics.MedianTotalDaysToClose(_organizationAnalysis.Tickets);  // 5. AvgTimeToClose
            return _rawMetrics;
        }

        /// <summary> 
        /// CDI2 
        /// temporarily turn off weights...
        /// </summary>
        public void InvokeCDIStrategy2(MetricPercentiles clientPercentiles, linq.CDI_Settings weights)
        {
            _normalizedMetrics = clientPercentiles.Normalize(_rawMetrics);

            List<double> result = new List<double>();
            foreach (EMetrics metric in Enum.GetValues(typeof(EMetrics)))
            {
                double? value = _normalizedMetrics.GetAsCDIPercentile(metric);
                double? weight = weights.Get(metric);

                if (value.HasValue)
                    result.Add(value.Value);
            }
            _normalizedMetrics.CDI = _rawMetrics.CDI = (int)Math.Round(result.Average());
        }

        /// <summary> CDI1 </summary>
        public void InvokeCDIStrategy(MetricPercentiles clientPercentiles, linq.CDI_Settings weights)
        {
            _normalizedMetrics = clientPercentiles.Normalize(_rawMetrics);

            double result = 0;
            foreach (EMetrics metric in Enum.GetValues(typeof(EMetrics)))
            {
                double? value = _normalizedMetrics.GetAsCDIPercentile(metric);
                double? weight = weights.Get(metric);
                if (value.HasValue && weight.HasValue)
                    result += value.Value * weight.Value;
            }

            // square it to get a better distribution?
            _normalizedMetrics.CDI = _rawMetrics.CDI = (int)Math.Round(result);
        }

        public string ToStringCDI1()
        {
            //return _organizationAnalysis.ToString();
            if (_rawMetrics == null)
                return string.Empty;
            return _rawMetrics.ToStringCDI1();
        }

        /// <summary>
        /// The linq to sql update compares object tracking as well as uses parameterization to protect
        /// against sql-injection attackes. Thus use a hard-coded update here...
        /// </summary>
        /// <param name="db"></param>
        public void Save(DataContext db)
        {
            db.ExecuteCommand(linq.Organization.RawUpdateQuery(_rawMetrics, _organizationAnalysis.OrganizationID));
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
                CDIEventLog.Instance.WriteEntry("Save failed", e);
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

        public int GetCDI1(Metrics interval, Metrics normalized, linq.CDI_Settings weights, Dictionary<EMetrics, Percentile> _percentiles)
        {
            // Create the CDI from the normalized fields
            CDI1 cdi = new CDI1()
            {
                TotalTicketsCreated = _percentiles[EMetrics.TotalTickets].AsPercentile(interval._totalTicketsCreated),
                TicketsOpen = _percentiles[EMetrics.Open].AsPercentile(interval._openCount),
                CreatedLast30 = _percentiles[EMetrics.New30].AsPercentile(interval._newCount),
                AvgTimeOpen = _percentiles[EMetrics.DaysOpen].AsPercentile(interval._medianDaysOpen),
            };

            if ((_percentiles[EMetrics.DaysToClose] != null) && interval._medianDaysToClose.HasValue)
                cdi.AvgTimeToClose = _percentiles[EMetrics.DaysToClose].AsPercentile(interval._medianDaysToClose.Value);

            double CustDisIndex = GetCDI(cdi, weights);
            normalized.CDI = (int)Math.Round(CustDisIndex);
            return normalized.CDI.Value;
        }

    }
}
