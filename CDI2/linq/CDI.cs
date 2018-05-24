using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace TeamSupport.CDI.linq
{
    [Table(Name = "CDI")]
    public class CDI
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _cdiID;
        [Column(Storage = "_cdiID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int CDI2ID { get { return _cdiID; } }

        [Column]
        public int OrganizationID;
        [Column]
        public int ParentID;
        public DateTime Timestamp;
        [Column]
        public int TotalTicketsCreated;
        [Column]
        public int TicketsOpen;
        [Column]
        public int CreatedLast30;
        [Column]
        public int AvgTimeOpen;
        [Column]
        public int AvgTimeToClose;
        [Column]
        public int CustDisIndex;
#pragma warning restore CS0649
    }
}
