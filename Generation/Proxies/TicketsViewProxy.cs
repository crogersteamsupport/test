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
  [KnownType(typeof(TicketsViewItemProxy))]
  public class TicketsViewItemProxy
  {
    public TicketsViewItemProxy() {}
    [DataMember] public int TicketID { get; set; }
    [DataMember] public string ProductName { get; set; }
    [DataMember] public string ReportedVersion { get; set; }
    [DataMember] public string SolvedVersion { get; set; }
    [DataMember] public string GroupName { get; set; }
    [DataMember] public string TicketTypeName { get; set; }
    [DataMember] public string UserName { get; set; }
    [DataMember] public string Status { get; set; }
    [DataMember] public int? StatusPosition { get; set; }
    [DataMember] public int? SeverityPosition { get; set; }
    [DataMember] public bool IsClosed { get; set; }
    [DataMember] public string Severity { get; set; }
    [DataMember] public int TicketNumber { get; set; }
    [DataMember] public bool IsVisibleOnPortal { get; set; }
    [DataMember] public bool IsKnowledgeBase { get; set; }
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
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime? DateClosed { get; set; }
    [DataMember] public int? CloserID { get; set; }
    [DataMember] public int DaysClosed { get; set; }
    [DataMember] public int? DaysOpened { get; set; }
    [DataMember] public string CloserName { get; set; }
    [DataMember] public string CreatorName { get; set; }
    [DataMember] public string ModifierName { get; set; }
    [DataMember] public decimal? HoursSpent { get; set; }
    [DataMember] public string Tags { get; set; }
    [DataMember] public int? SlaViolationTime { get; set; }
    [DataMember] public int? SlaWarningTime { get; set; }
    [DataMember] public decimal? SlaViolationHours { get; set; }
    [DataMember] public decimal? SlaWarningHours { get; set; }
    [DataMember] public int? MinsSinceCreated { get; set; }
    [DataMember] public int? DaysSinceCreated { get; set; }
    [DataMember] public int? MinsSinceModified { get; set; }
    [DataMember] public int? DaysSinceModified { get; set; }
    [DataMember] public string Contacts { get; set; }
    [DataMember] public string Customers { get; set; }
    [DataMember] public DateTime? SlaViolationTimeClosed { get; set; }
    [DataMember] public DateTime? SlaViolationLastAction { get; set; }
    [DataMember] public DateTime? SlaViolationInitialResponse { get; set; }
    [DataMember] public DateTime? SlaWarningTimeClosed { get; set; }
    [DataMember] public DateTime? SlaWarningLastAction { get; set; }
    [DataMember] public DateTime? SlaWarningInitialResponse { get; set; }
          
  }
  
  public partial class TicketsViewItem : BaseItem
  {
    public TicketsViewItemProxy GetProxy()
    {
      TicketsViewItemProxy result = new TicketsViewItemProxy();
      result.Customers = this.Customers;
      result.Contacts = this.Contacts;
      result.DaysSinceModified = this.DaysSinceModified;
      result.MinsSinceModified = this.MinsSinceModified;
      result.DaysSinceCreated = this.DaysSinceCreated;
      result.MinsSinceCreated = this.MinsSinceCreated;
      result.SlaWarningHours = this.SlaWarningHours;
      result.SlaViolationHours = this.SlaViolationHours;
      result.SlaWarningTime = this.SlaWarningTime;
      result.SlaViolationTime = this.SlaViolationTime;
      result.Tags = this.Tags;
      result.HoursSpent = this.HoursSpent;
      result.ModifierName = this.ModifierName;
      result.CreatorName = this.CreatorName;
      result.CloserName = this.CloserName;
      result.DaysOpened = this.DaysOpened;
      result.DaysClosed = this.DaysClosed;
      result.CloserID = this.CloserID;
      result.CreatorID = this.CreatorID;
      result.ModifierID = this.ModifierID;
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
      result.IsKnowledgeBase = this.IsKnowledgeBase;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.TicketNumber = this.TicketNumber;
      result.Severity = this.Severity;
      result.IsClosed = this.IsClosed;
      result.SeverityPosition = this.SeverityPosition;
      result.StatusPosition = this.StatusPosition;
      result.Status = this.Status;
      result.UserName = this.UserName;
      result.TicketTypeName = this.TicketTypeName;
      result.GroupName = this.GroupName;
      result.SolvedVersion = this.SolvedVersion;
      result.ReportedVersion = this.ReportedVersion;
      result.ProductName = this.ProductName;
      result.TicketID = this.TicketID;
       
      result.DateModified = DateTime.SpecifyKind(this.DateModified, DateTimeKind.Local);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreated, DateTimeKind.Local);
       
      result.SlaWarningInitialResponse = this.SlaWarningInitialResponse == null ? this.SlaWarningInitialResponse : DateTime.SpecifyKind((DateTime)this.SlaWarningInitialResponse, DateTimeKind.Local); 
      result.SlaWarningLastAction = this.SlaWarningLastAction == null ? this.SlaWarningLastAction : DateTime.SpecifyKind((DateTime)this.SlaWarningLastAction, DateTimeKind.Local); 
      result.SlaWarningTimeClosed = this.SlaWarningTimeClosed == null ? this.SlaWarningTimeClosed : DateTime.SpecifyKind((DateTime)this.SlaWarningTimeClosed, DateTimeKind.Local); 
      result.SlaViolationInitialResponse = this.SlaViolationInitialResponse == null ? this.SlaViolationInitialResponse : DateTime.SpecifyKind((DateTime)this.SlaViolationInitialResponse, DateTimeKind.Local); 
      result.SlaViolationLastAction = this.SlaViolationLastAction == null ? this.SlaViolationLastAction : DateTime.SpecifyKind((DateTime)this.SlaViolationLastAction, DateTimeKind.Local); 
      result.SlaViolationTimeClosed = this.SlaViolationTimeClosed == null ? this.SlaViolationTimeClosed : DateTime.SpecifyKind((DateTime)this.SlaViolationTimeClosed, DateTimeKind.Local); 
      result.DateClosed = this.DateClosed == null ? this.DateClosed : DateTime.SpecifyKind((DateTime)this.DateClosed, DateTimeKind.Local); 
       
      return result;
    }	
  }
}
