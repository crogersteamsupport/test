using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
 public partial class TicketsViewItem : BaseItem
  {
    public TicketsViewItemProxy GetProxy()
    {
      TicketsViewItemProxy result = new TicketsViewItemProxy();
      result.ProductFamilyID = this.ProductFamilyID;
      result.SalesForceID = this.SalesForceID;
      result.KnowledgeBaseCategoryName = this.KnowledgeBaseCategoryName;
      result.KnowledgeBaseCategoryID = this.KnowledgeBaseCategoryID;
      result.CategoryName = (this.CategoryName);
      result.ForumCategory = this.ForumCategory;
      result.TicketSource = this.TicketSource;
      result.Customers = this.Customers;
      result.Contacts = this.Contacts;
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
      result.UserName = this.UserName;
      result.TicketTypeName = (this.TicketTypeName);
      result.GroupName = (this.GroupName);
      result.SolvedVersion = this.SolvedVersion;
      result.ReportedVersion = this.ReportedVersion;
      result.ProductName = (this.ProductName);
      result.TicketID = this.TicketID;

      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);

      //result.SlaWarningInitialResponse = this.SlaWarningInitialResponseUtc == null ? this.SlaWarningInitialResponseUtc : DateTime.SpecifyKind((DateTime)this.SlaWarningInitialResponseUtc, DateTimeKind.Utc); 
      result.DateModifiedBySalesForceSync = this.DateModifiedBySalesForceSyncUtc == null ? this.DateModifiedBySalesForceSyncUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedBySalesForceSyncUtc, DateTimeKind.Utc); 

      result.SlaWarningDate = this.SlaWarningDateUtc == null ? this.SlaWarningDateUtc : DateTime.SpecifyKind((DateTime)this.SlaWarningDateUtc, DateTimeKind.Utc);
      result.SlaViolationDate = this.SlaViolationDateUtc == null ? this.SlaViolationDateUtc : DateTime.SpecifyKind((DateTime)this.SlaViolationDateUtc, DateTimeKind.Utc);
      result.DateClosed = this.DateClosedUtc == null ? this.DateClosedUtc : DateTime.SpecifyKind((DateTime)this.DateClosedUtc, DateTimeKind.Utc);

      result.IsRead = this.IsRead;// || (Collection.LoginUser.UserID != 34 && Collection.LoginUser.UserID != 43 && Collection.LoginUser.OrganizationID != 300197);
      result.IsFlagged = this.IsFlagged;
      result.IsSubscribed = this.IsSubscribed;
      result.IsEnqueued = this.IsEnqueued;
      result.ViewerID = this.ViewerID;
      result.DueDate = this.DueDateUtc == null ? this.DueDateUtc : DateTime.SpecifyKind((DateTime)this.DueDateUtc, DateTimeKind.Utc);
      return result;
    }	
  }
}
