using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Linq;

namespace TeamSupport.CDI
{
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

        /// <summary> CDI1 </summary>
        public void InvokeCDIStrategy(MetricPercentiles clientPercentiles, linq.CDI_Settings weights)
        {
            _normalizedMetrics = clientPercentiles.Normalize(_rawMetrics);

            double result = 0;
            foreach (EMetrics metric in Enum.GetValues(typeof(EMetrics)))
            {
                double? value = _normalizedMetrics.GetAsCDIPercentile(metric);
                double? weight = weights.GetWeight(metric);
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
    }
}
