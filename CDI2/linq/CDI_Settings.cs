using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace TeamSupport.CDI.linq
{
    [Table(Name = "CDI_Settings")]
    public class CDI_Settings
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _organizationID;
        [Column(Storage = "_organizationID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int OrganizationID { get { return _organizationID; } }

        [Column]
        public float? TotalTicketsWeight;
        [Column]
        public float? OpenTicketsWeight;
        [Column]
        public float? Last30Weight;
        [Column]
        public float? AvgDaysOpenWeight;
        [Column]
        public float? AvgDaysToCloseWeight;
        [Column]
        public DateTime? LastCompute;
        [Column]
        public bool NeedCompute;
#pragma warning restore CS0649

        public void Normalize()
        {
            float total = TotalTicketsWeight.Value + OpenTicketsWeight.Value + Last30Weight.Value + AvgDaysOpenWeight.Value + AvgDaysToCloseWeight.Value;
            float scalar = 1 / total;

            TotalTicketsWeight *= scalar;
            OpenTicketsWeight *= scalar;
            Last30Weight *= scalar;
            AvgDaysOpenWeight *= scalar;
            AvgDaysToCloseWeight *= scalar;

        }

        public double? Get(Metrics metric)
        {
            double? result = null;
            switch (metric)
            {
                case Metrics.ActionCount:
                    //result = _averageActionCount;
                    break;
                case Metrics.Closed:
                    //result = _closedCount;
                    break;
                case Metrics.DaysOpen:
                    result = AvgDaysOpenWeight;
                    break;
                case Metrics.DaysToClose:
                    result = AvgDaysToCloseWeight;
                    break;
                case Metrics.New:
                    result = Last30Weight;
                    break;
                case Metrics.Open:
                    result = OpenTicketsWeight;
                    break;
                case Metrics.Sentiment:
                    //result = _averageSentimentScore;
                    break;
                case Metrics.Severity:
                    //result = _averageSeverity;
                    break;
                case Metrics.TotalTickets:
                    result = TotalTicketsWeight;
                    break;
            }
            return result;
        }
    }
}
