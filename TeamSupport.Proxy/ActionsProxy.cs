using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Data.Linq.Mapping;

namespace TeamSupport.Data
{
    [DataContract(Namespace = "http://teamsupport.com/")]
    [KnownType(typeof(ActionProxy))]
    [Table(Name = "Actions")]
    public class ActionProxy
    {
        public ActionProxy() { }
        [DataMember, Column(DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)] public int ActionID { get; set; }
        [DataMember, Column] public int? ActionTypeID { get; set; }
        [DataMember, Column] public SystemActionType SystemActionTypeID { get; set; }
        [DataMember, Column] public string Name { get; set; }
        [DataMember, Column] public int? TimeSpent { get; set; }
        [DataMember, Column] public DateTime? DateStarted { get; set; }
        [DataMember, Column] public bool IsVisibleOnPortal { get; set; }
        [DataMember, Column] public bool IsKnowledgeBase { get; set; }
        [DataMember, Column] public string ImportID { get; set; }
        [DataMember, Column] public DateTime? DateCreated { get; set; }
        [DataMember, Column] public DateTime? DateModified { get; set; }
        [DataMember, Column] public int? CreatorID { get; set; }
        [DataMember, Column] public int? ModifierID { get; set; }
        [DataMember, Column] public int TicketID { get; set; }
        [DataMember, Column] public string Description { get; set; }
        [DataMember] public string DisplayName { get; set; }
        [DataMember, Column] public string SalesForceID { get; set; }
        [DataMember, Column] public DateTime? DateModifiedBySalesForceSync { get; set; }
        [DataMember, Column] public bool Pinned { get; set; }
        [DataMember, Column] public int? ImportFileID { get; set; }
        public string ActionSource{ get; set; }

    }

}
