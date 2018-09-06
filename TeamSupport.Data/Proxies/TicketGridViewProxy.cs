using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{

  public partial class TicketGridViewItem : BaseItem
  {
    public TicketGridViewItemProxy GetProxy()
    {
      TicketGridViewItemProxy result = new TicketGridViewItemProxy();
      result.SlaWarningHours = this.SlaWarningHours;
      result.SlaViolationHours = this.SlaViolationHours;
      result.SlaWarningTime = this.SlaWarningTime;
      result.SlaViolationTime = this.SlaViolationTime;
      result.ModifierName = this.ModifierName;
      result.CreatorName = this.CreatorName;
      result.CloserName = this.CloserName;
      result.DaysOpened = this.DaysOpened;
      result.DaysClosed = this.DaysClosed;
      result.CloserID = this.CloserID;
      result.CreatorID = this.CreatorID;
      result.ModifierID = this.ModifierID;
      result.ParentID = this.ParentID;
      result.Name = (this.Name);
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
      result.UserName = (this.UserName);
      result.TicketTypeName = (this.TicketTypeName);
      result.GroupName = (this.GroupName);
      result.SolvedVersion = this.SolvedVersion;
      result.ReportedVersion = this.ReportedVersion;
      result.ProductName = (this.ProductName);
      result.TicketID = this.TicketID;
       
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
      result.DueDate = this.DueDateUtc == null ? this.DueDateUtc : DateTime.SpecifyKind((DateTime)this.DueDateUtc, DateTimeKind.Utc); 
      result.DateClosed = this.DateClosedUtc == null ? this.DateClosedUtc : DateTime.SpecifyKind((DateTime)this.DateClosedUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
