using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
    [DataContract(Namespace = "http://teamsupport.com/")]
    [KnownType(typeof(ActionProxy))]
    public class ActionProxy
    {
        public ActionProxy() { }
        [DataMember] public int ActionID { get; set; }
        [DataMember] public int? ActionTypeID { get; set; }
        [DataMember] public SystemActionType SystemActionTypeID { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public int? TimeSpent { get; set; }
        [DataMember] public DateTime? DateStarted { get; set; }
        [DataMember] public bool IsVisibleOnPortal { get; set; }
        [DataMember] public bool IsKnowledgeBase { get; set; }
        [DataMember] public string ImportID { get; set; }
        [DataMember] public DateTime? DateCreated { get; set; }
        [DataMember] public DateTime? DateModified { get; set; }
        [DataMember] public int? CreatorID { get; set; }
        [DataMember] public int? ModifierID { get; set; }
        [DataMember] public int TicketID { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] public string DisplayName { get; set; }
        [DataMember] public string SalesForceID { get; set; }
        [DataMember] public DateTime? DateModifiedBySalesForceSync { get; set; }
        [DataMember] public bool Pinned { get; set; }
        [DataMember] public int? ImportFileID { get; set; }
        public string ActionSource{ get; set; }

    }

}
