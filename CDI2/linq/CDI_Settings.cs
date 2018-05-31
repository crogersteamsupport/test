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

        public double GetCDI(CDI metrics)
        {
            return metrics.TotalTicketsCreated * TotalTicketsWeight.Value +
                        metrics.TicketsOpen * OpenTicketsWeight.Value +
                        metrics.CreatedLast30 * Last30Weight.Value +
                        metrics.AvgTimeOpen * AvgDaysOpenWeight.Value +
                        metrics.AvgTimeToClose * AvgDaysToCloseWeight.Value;
        }
    }
}
