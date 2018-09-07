using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ReportTicketsViewItem : BaseItem
  {
    public ReportTicketsViewItemProxy GetProxy()
    {
      ReportTicketsViewItemProxy result = new ReportTicketsViewItemProxy();
      result.HourOfDayCreated = this.HourOfDayCreated;
      result.DayOfWeekCreated = this.DayOfWeekCreated;
      result.MinutesToInitialResponse = this.MinutesToInitialResponse;
      result.RelatedTicketCount = this.RelatedTicketCount;
      result.ChildTicketCount = this.ChildTicketCount;
      result.JiraStatus = this.JiraStatus;
      result.JiraLinkURL = this.JiraLinkURL;
      result.JiraKey = this.JiraKey;
      result.SyncWithJira = this.SyncWithJira;
      result.JiraID = this.JiraID;
      result.SalesForceID = this.SalesForceID;
      result.KnowledgeBaseParentCategoryName = (this.KnowledgeBaseParentCategoryName);
      result.KnowledgeBaseCategoryName = (this.KnowledgeBaseCategoryName);
      result.KnowledgeBaseCategoryID = this.KnowledgeBaseCategoryID;
      result.ModifierEmail = this.ModifierEmail;
      result.CreatorEmail = this.CreatorEmail;
      result.CategoryName = (this.CategoryName);
      result.ForumCategory = this.ForumCategory;
      result.TicketSource = this.TicketSource;
      result.NeedsIndexing = this.NeedsIndexing;
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
      result.MinutesOpened = this.MinutesOpened;
      result.DaysOpened = this.DaysOpened;
      result.MinutesClosed = this.MinutesClosed;
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
       
      result.DateModifiedByJiraSync = this.DateModifiedByJiraSyncUtc == null ? this.DateModifiedByJiraSyncUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedByJiraSyncUtc, DateTimeKind.Utc); 
      result.DateModifiedBySalesForceSync = this.DateModifiedBySalesForceSyncUtc == null ? this.DateModifiedBySalesForceSyncUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedBySalesForceSyncUtc, DateTimeKind.Utc); 
      result.DueDate = this.DueDateUtc == null ? this.DueDateUtc : DateTime.SpecifyKind((DateTime)this.DueDateUtc, DateTimeKind.Utc); 
      result.SlaWarningDate = this.SlaWarningDateUtc == null ? this.SlaWarningDateUtc : DateTime.SpecifyKind((DateTime)this.SlaWarningDateUtc, DateTimeKind.Utc); 
      result.SlaViolationDate = this.SlaViolationDateUtc == null ? this.SlaViolationDateUtc : DateTime.SpecifyKind((DateTime)this.SlaViolationDateUtc, DateTimeKind.Utc); 
      result.SlaWarningInitialResponse = this.SlaWarningInitialResponseUtc == null ? this.SlaWarningInitialResponseUtc : DateTime.SpecifyKind((DateTime)this.SlaWarningInitialResponseUtc, DateTimeKind.Utc); 
      result.SlaWarningLastAction = this.SlaWarningLastActionUtc == null ? this.SlaWarningLastActionUtc : DateTime.SpecifyKind((DateTime)this.SlaWarningLastActionUtc, DateTimeKind.Utc); 
      result.SlaWarningTimeClosed = this.SlaWarningTimeClosedUtc == null ? this.SlaWarningTimeClosedUtc : DateTime.SpecifyKind((DateTime)this.SlaWarningTimeClosedUtc, DateTimeKind.Utc); 
      result.SlaViolationInitialResponse = this.SlaViolationInitialResponseUtc == null ? this.SlaViolationInitialResponseUtc : DateTime.SpecifyKind((DateTime)this.SlaViolationInitialResponseUtc, DateTimeKind.Utc); 
      result.SlaViolationLastAction = this.SlaViolationLastActionUtc == null ? this.SlaViolationLastActionUtc : DateTime.SpecifyKind((DateTime)this.SlaViolationLastActionUtc, DateTimeKind.Utc); 
      result.SlaViolationTimeClosed = this.SlaViolationTimeClosedUtc == null ? this.SlaViolationTimeClosedUtc : DateTime.SpecifyKind((DateTime)this.SlaViolationTimeClosedUtc, DateTimeKind.Utc); 
      result.DateClosed = this.DateClosedUtc == null ? this.DateClosedUtc : DateTime.SpecifyKind((DateTime)this.DateClosedUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
