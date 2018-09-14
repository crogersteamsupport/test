using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(TicketProxy))]
  public class TicketProxy
  {
    public TicketProxy() {}
    [DataMember] public int TicketID { get; set; }
    [DataMember] public int? ReportedVersionID { get; set; }
    [DataMember] public int? SolvedVersionID { get; set; }
    [DataMember] public int? ProductID { get; set; }
    [DataMember] public int? GroupID { get; set; }
    [DataMember] public int? UserID { get; set; }
    [DataMember] public int TicketStatusID { get; set; }
    [DataMember] public int TicketTypeID { get; set; }
    [DataMember] public int TicketSeverityID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public int? ParentID { get; set; }
    [DataMember] public int TicketNumber { get; set; }
    [DataMember] public bool IsVisibleOnPortal { get; set; }
    [DataMember] public bool IsKnowledgeBase { get; set; }
    [DataMember] public DateTime? DateClosed { get; set; }
    [DataMember] public int? CloserID { get; set; }
    [DataMember] public string ImportID { get; set; }
    [DataMember] public DateTime? LastViolationTime { get; set; }
    [DataMember] public DateTime? LastWarningTime { get; set; }
    [DataMember] public string TicketSource { get; set; }
    [DataMember] public string PortalEmail { get; set; }
    [DataMember] public DateTime? SlaViolationTimeClosed { get; set; }
    [DataMember] public DateTime? SlaViolationLastAction { get; set; }
    [DataMember] public DateTime? SlaViolationInitialResponse { get; set; }
    [DataMember] public DateTime? SlaWarningTimeClosed { get; set; }
    [DataMember] public DateTime? SlaWarningLastAction { get; set; }
    [DataMember] public DateTime? SlaWarningInitialResponse { get; set; }
    [DataMember] public bool NeedsIndexing { get; set; }
    [DataMember] public int? DocID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public int? KnowledgeBaseCategoryID { get; set; }
    [DataMember] public string SalesForceID { get; set; }
    [DataMember] public DateTime? DateModifiedBySalesForceSync { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
          
  }
  
}
