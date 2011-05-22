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
    [DataMember] public int? DocID { get; set; }
    [DataMember] public bool NeedsIndexing { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
          
  }
  
  public partial class Ticket : BaseItem
  {
    public TicketProxy GetProxy()
    {
      TicketProxy result = new TicketProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.NeedsIndexing = this.NeedsIndexing;
      result.DocID = this.DocID;
      result.PortalEmail = this.PortalEmail;
      result.TicketSource = this.TicketSource;
      result.ImportID = this.ImportID;
      result.CloserID = this.CloserID;
      result.IsKnowledgeBase = this.IsKnowledgeBase;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.TicketNumber = this.TicketNumber;
      result.ParentID = this.ParentID;
      result.Name = this.Name;
      result.OrganizationID = this.OrganizationID;
      result.TicketSeverityID = this.TicketSeverityID;
      result.TicketTypeID = this.TicketTypeID;
      result.TicketStatusID = this.TicketStatusID;
      result.UserID = this.UserID;
      result.GroupID = this.GroupID;
      result.ProductID = this.ProductID;
      result.SolvedVersionID = this.SolvedVersionID;
      result.ReportedVersionID = this.ReportedVersionID;
      result.TicketID = this.TicketID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreated, DateTimeKind.Local);
      result.DateModified = DateTime.SpecifyKind(this.DateModified, DateTimeKind.Local);
       
      result.SlaWarningInitialResponse = this.SlaWarningInitialResponse == null ? this.SlaWarningInitialResponse : DateTime.SpecifyKind((DateTime)this.SlaWarningInitialResponse, DateTimeKind.Local); 
      result.SlaWarningLastAction = this.SlaWarningLastAction == null ? this.SlaWarningLastAction : DateTime.SpecifyKind((DateTime)this.SlaWarningLastAction, DateTimeKind.Local); 
      result.SlaWarningTimeClosed = this.SlaWarningTimeClosed == null ? this.SlaWarningTimeClosed : DateTime.SpecifyKind((DateTime)this.SlaWarningTimeClosed, DateTimeKind.Local); 
      result.SlaViolationInitialResponse = this.SlaViolationInitialResponse == null ? this.SlaViolationInitialResponse : DateTime.SpecifyKind((DateTime)this.SlaViolationInitialResponse, DateTimeKind.Local); 
      result.SlaViolationLastAction = this.SlaViolationLastAction == null ? this.SlaViolationLastAction : DateTime.SpecifyKind((DateTime)this.SlaViolationLastAction, DateTimeKind.Local); 
      result.SlaViolationTimeClosed = this.SlaViolationTimeClosed == null ? this.SlaViolationTimeClosed : DateTime.SpecifyKind((DateTime)this.SlaViolationTimeClosed, DateTimeKind.Local); 
      result.LastWarningTime = this.LastWarningTime == null ? this.LastWarningTime : DateTime.SpecifyKind((DateTime)this.LastWarningTime, DateTimeKind.Local); 
      result.LastViolationTime = this.LastViolationTime == null ? this.LastViolationTime : DateTime.SpecifyKind((DateTime)this.LastViolationTime, DateTimeKind.Local); 
      result.DateClosed = this.DateClosed == null ? this.DateClosed : DateTime.SpecifyKind((DateTime)this.DateClosed, DateTimeKind.Local); 
       
      return result;
    }	
  }
}
