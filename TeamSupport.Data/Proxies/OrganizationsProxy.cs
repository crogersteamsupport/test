using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class Organization : BaseItem
  {
    public OrganizationProxy GetProxy()
    {
      OrganizationProxy result = new OrganizationProxy();
      result.APIRequestMinuteLimit = this.APIRequestMinuteLimit;
      result.UseWatson = this.UseWatson;
      result.RequireTwoFactor = this.RequireTwoFactor;
      result.NoAttachmentsInOutboundExcludeProductLine = this.NoAttachmentsInOutboundExcludeProductLine;
      result.AlertContactNoEmail = this.AlertContactNoEmail;
      result.RequireGroupAssignmentOnTickets = this.RequireGroupAssignmentOnTickets;
      result.AutoAssociateCustomerToTicketBasedOnAssetAssignment = this.AutoAssociateCustomerToTicketBasedOnAssetAssignment;
      result.AutoAssignCustomerWithAssetOnTickets = this.AutoAssignCustomerWithAssetOnTickets;
	  result.DaysBeforePasswordExpire = this.DaysBeforePasswordExpire;
	  result.NoAttachmentsInOutboundEmail = this.NoAttachmentsInOutboundEmail;
      result.ImportFileID = this.ImportFileID;
      result.TwoStepVerificationEnabled = this.TwoStepVerificationEnabled;
      result.IsCustomerInsightsActive = this.IsCustomerInsightsActive;
      result.UseProductFamilies = this.UseProductFamilies;
      result.CustDistIndexTrend = this.CustDistIndexTrend;
      result.HideDismissNonAdmins = this.HideDismissNonAdmins;
      result.AddEmailViaTS = this.AddEmailViaTS;
      result.AgentRating = this.AgentRating;
      result.ForceUseOfReplyTo = this.ForceUseOfReplyTo;
      result.ReplyToAlternateEmailAddresses = this.ReplyToAlternateEmailAddresses;
      result.UpdateTicketChildrenGroupWithParent = this.UpdateTicketChildrenGroupWithParent;
      result.ShowGroupMembersFirstInTicketAssignmentList = this.ShowGroupMembersFirstInTicketAssignmentList;
      result.SlaInitRespAnyAction = this.SlaInitRespAnyAction;
      result.FontSize = this.FontSize;
      result.FontFamily = this.FontFamily;
      result.CustDisIndex = this.CustDisIndex;
      result.AvgTimeToClose = this.AvgTimeToClose;
      result.AvgTimeOpen = this.AvgTimeOpen;
      result.CreatedLast30 = this.CreatedLast30;
      result.TicketsOpen = this.TicketsOpen;
      result.TotalTicketsCreated = this.TotalTicketsCreated;
      result.NeedCustForTicketMatch = this.NeedCustForTicketMatch;
      result.NeedsIndexing = this.NeedsIndexing;
      result.IsIndexLocked = this.IsIndexLocked;
      result.IsRebuildingIndex = this.IsRebuildingIndex;
      result.UnknownCompanyID = this.UnknownCompanyID;
      result.ForceBCCEmailsPrivate = this.ForceBCCEmailsPrivate;
      result.AllowUnsecureAttachmentViewing = this.AllowUnsecureAttachmentViewing;
      result.ProductVersionRequired = this.ProductVersionRequired;
      result.ProductRequired = this.ProductRequired;
      result.SupportHoursMonth = this.SupportHoursMonth;
      result.SetNewActionsVisibleToCustomers = this.SetNewActionsVisibleToCustomers;
      result.UseForums = this.UseForums;
      result.IsPublicArticles = this.IsPublicArticles;
      result.ChangeStatusIfClosed = this.ChangeStatusIfClosed;
      result.AddAdditionalContacts = this.AddAdditionalContacts;
      result.EvalProcess = this.EvalProcess;
      result.PotentialSeats = this.PotentialSeats;
      result.PrimaryInterest = this.PrimaryInterest;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.MatchEmailSubject = this.MatchEmailSubject;
      result.MarkSpam = this.MarkSpam;
      result.TimedActionsRequired = this.TimedActionsRequired;
      result.CultureName = this.CultureName;
      result.UseEuropeDate = this.UseEuropeDate;
      result.BusinessDays = this.BusinessDays;
      result.InternalSlaLevelID = this.InternalSlaLevelID;
      result.SlaLevelID = this.SlaLevelID;
      result.DefaultWikiArticleID = this.DefaultWikiArticleID;
      result.ShowWiki = this.ShowWiki;
      result.AdminOnlyReports = this.AdminOnlyReports;
      result.AdminOnlyCustomers = this.AdminOnlyCustomers;
      result.CompanyDomains = this.CompanyDomains;
      result.OrganizationReplyToAddress = this.OrganizationReplyToAddress;
      result.EmailDelimiter = this.EmailDelimiter;
      result.RequireKnownUserForNewEmail = this.RequireKnownUserForNewEmail;
      result.RequireNewKeyword = this.RequireNewKeyword;
      result.APIRequestLimit = this.APIRequestLimit;
      result.CRMLinkID = this.CRMLinkID;
      result.PortalGuid = this.PortalGuid;
      result.ChatID = this.ChatID;
      result.SystemEmailID = this.SystemEmailID;
      result.WebServiceID = this.WebServiceID;
      result.ParentID = this.ParentID;
      result.ProductType = this.ProductType;
      result.DefaultSupportUserID = this.DefaultSupportUserID;
      result.DefaultSupportGroupID = this.DefaultSupportGroupID;
      result.DefaultPortalGroupID = this.DefaultPortalGroupID;
      result.PrimaryUserID = this.PrimaryUserID;
      result.IsBasicPortal = this.IsBasicPortal;
      result.IsAdvancedPortal = this.IsAdvancedPortal;
      result.HasPortalAccess = this.HasPortalAccess;
      result.InActiveReason = this.InActiveReason;
      result.TimeZoneID = this.TimeZoneID;
      result.IsInventoryEnabled = this.IsInventoryEnabled;
      result.IsApiEnabled = this.IsApiEnabled;
      result.IsApiActive = this.IsApiActive;
      result.IsActive = this.IsActive;
      result.ImportID = this.ImportID;
      result.ExtraStorageUnits = this.ExtraStorageUnits;
      result.ChatSeats = this.ChatSeats;
      result.PortalSeats = this.PortalSeats;
      result.UserSeats = this.UserSeats;
      result.IsCustomerFree = this.IsCustomerFree;
      result.PromoCode = this.PromoCode;
      result.WhereHeard = this.WhereHeard;
      result.Website = (this.Website);
      result.Description = (this.Description);
      result.Name = (this.Name);
      result.OrganizationID = this.OrganizationID;
      result.DisableSupportLogin = this.DisableSupportLogin;


      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
      result.BusinessDayStart = DateTime.SpecifyKind(this.BusinessDayStartUtc, DateTimeKind.Utc);
      result.BusinessDayEnd = DateTime.SpecifyKind(this.BusinessDayEndUtc, DateTimeKind.Utc);
      result.LastIndexRebuilt = DateTime.SpecifyKind(this.LastIndexRebuiltUtc, DateTimeKind.Utc);

      result.SAExpirationDate = this.SAExpirationDateUtc == null ? this.SAExpirationDateUtc : DateTime.SpecifyKind((DateTime)this.SAExpirationDateUtc, DateTimeKind.Utc);


      return result;
    }
  }
}
