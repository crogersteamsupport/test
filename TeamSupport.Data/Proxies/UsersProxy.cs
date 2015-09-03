using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Globalization;
using Ganss.XSS;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(UserProxy))]
  public class UserProxy
  {
    public UserProxy() {}
    [DataMember] public int UserID { get; set; }
    [DataMember] public string Email { get; set; }
    [DataMember] public string FirstName { get; set; }
    [DataMember] public string MiddleName { get; set; }
    [DataMember] public string LastName { get; set; }
    [DataMember] public string Title { get; set; }
    [DataMember] public string CryptedPassword { get; set; }
    [DataMember] public bool IsActive { get; set; }
    [DataMember] public bool MarkDeleted { get; set; }
    [DataMember] public string TimeZoneID { get; set; }
    [DataMember] public string CultureName { get; set; }
    [DataMember] public DateTime LastLogin { get; set; }
    [DataMember] public DateTime LastActivity { get; set; }
    [DataMember] public DateTime? LastPing { get; set; }
    [DataMember] public int LastWaterCoolerID { get; set; }
    [DataMember] public bool IsSystemAdmin { get; set; }
    [DataMember] public bool IsFinanceAdmin { get; set; }
    [DataMember] public bool IsPasswordExpired { get; set; }
    [DataMember] public bool IsPortalUser { get; set; }
    [DataMember] public bool IsChatUser { get; set; }
    [DataMember] public int? PrimaryGroupID { get; set; }
    [DataMember] public bool InOffice { get; set; }
    [DataMember] public string InOfficeComment { get; set; }
    [DataMember] public bool ReceiveTicketNotifications { get; set; }
    [DataMember] public bool ReceiveAllGroupNotifications { get; set; }
    [DataMember] public bool ReceiveUnassignedGroupEmails { get; set; }
    [DataMember] public bool SubscribeToNewTickets { get; set; }
    [DataMember] public bool SubscribeToNewActions { get; set; }
    [DataMember] public DateTime ActivatedOn { get; set; }
    [DataMember] public DateTime? DeactivatedOn { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string LastVersion { get; set; }
    [DataMember] public Guid? SessionID { get; set; }
    [DataMember] public string ImportID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public string OrgsUserCanSeeOnPortal { get; set; }
    [DataMember] public bool DoNotAutoSubscribe { get; set; }
    [DataMember] public bool IsClassicView { get; set; }
    [DataMember] public string timeZoneDisplay { get; set; }
    [DataMember] public string CultureDisplay { get; set; }
    [DataMember] public bool ShowWelcomePage { get; set; }
    [DataMember] public string UserInformation { get; set; }
    [DataMember] public bool AppChatStatus { get; set; }
    [DataMember] public string AppChatID { get; set; }   
    [DataMember] public string MenuItems { get; set; }
    [DataMember] public string LinkedIn { get; set; }
    [DataMember] public TicketRightType TicketRights { get; set; }
    [DataMember] public bool OnlyEmailAfterHours { get; set; }
    [DataMember] public bool BlockInboundEmail { get; set; }
    [DataMember] public string SalesForceID { get; set; }
    [DataMember] public bool ChangeTicketVisibility { get; set; }
    [DataMember] public bool CanChangeCommunityVisibility { get; set; }
    [DataMember] public bool ChangeKbVisibility { get; set; }
    [DataMember] public string Avatar { get; set; }
    [DataMember] public bool EnforceSingleSession { get; set; }
    [DataMember] public bool NeedsIndexing { get; set; }
    [DataMember] public bool AllowAnyTicketCustomer { get; set; }
    [DataMember] public FontFamily FontFamily { get; set; }
    [DataMember] public FontSize FontSize { get; set; }
    [DataMember] public string FontFamilyDescription { get; set; }
    [DataMember] public string FontSizeDescription { get; set; }
    [DataMember] public bool CanCreateCompany { get; set; }
    [DataMember] public bool CanEditCompany { get; set; }
    [DataMember] public bool CanCreateContact { get; set; }
    [DataMember] public bool CanEditContact { get; set; }
    [DataMember] public bool CanCreateAsset { get; set; }
    [DataMember] public bool CanEditAsset { get; set; }
    [DataMember] public bool RestrictUserFromEditingAnyActions { get; set; }
    [DataMember] public bool AllowUserToEditAnyAction { get; set; }
    [DataMember] public bool UserCanPinAction { get; set; }
    [DataMember] public bool PortalLimitOrgTickets { get; set; }
    [DataMember] public bool FilterInactive { get; set; }
    [DataMember] public bool DisableExporting { get; set; }
    [DataMember] public bool CanCreateProducts { get; set; }
    [DataMember] public bool CanCreateVersions { get; set; }
    [DataMember] public bool CanEditProducts { get; set; }
    [DataMember] public bool CanEditVersions { get; set; }
    [DataMember] public ProductFamiliesRightType ProductFamiliesRights { get; set; }
    [DataMember] public bool? BlockEmailFromCreatingOnly { get; set; }
    [DataMember] public Guid CalGUID { get; set; }
	 [DataMember] public string verificationCode { get; set; }
	 [DataMember] public string verificationPhoneNumber { get; set; }
	 [DataMember] public DateTime? verificationCodeExpiration { get; set; }
          
  }
  
  public partial class User : BaseItem
  {
    public UserProxy GetProxy()
    {

      UserProxy result = new UserProxy();
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      result.verificationCode = this.verificationCode;
      result.verificationPhoneNumber = this.verificationPhoneNumber;
      result.CalGUID = this.CalGUID;
      result.BlockEmailFromCreatingOnly = this.BlockEmailFromCreatingOnly;
      result.ProductFamiliesRights = (ProductFamiliesRightType)this.ProductFamiliesRights;
      result.DisableExporting = this.DisableExporting;
      result.PortalLimitOrgTickets = this.PortalLimitOrgTickets;
      result.UserCanPinAction = this.UserCanPinAction;
      result.AllowUserToEditAnyAction = this.AllowUserToEditAnyAction;
      result.RestrictUserFromEditingAnyActions = this.RestrictUserFromEditingAnyActions;
      result.FontSize = this.FontSize;
      result.FontFamily = this.FontFamily;
      result.FontSizeDescription = this.FontSizeDescription;
      result.FontFamilyDescription = this.FontFamilyDescription;
      result.AllowAnyTicketCustomer = this.AllowAnyTicketCustomer;
      result.NeedsIndexing = this.NeedsIndexing;
      result.EnforceSingleSession = this.EnforceSingleSession;
      result.SalesForceID = this.SalesForceID;
      if (!this.LinkedIn.StartsWith("http"))
          result.LinkedIn = "http://" + this.LinkedIn;
      else
          result.LinkedIn = this.LinkedIn;
      result.TicketRights = this.TicketRights;
      result.MenuItems = this.MenuItems;
      result.OnlyEmailAfterHours = this.OnlyEmailAfterHours;
      result.BlockInboundEmail = this.BlockInboundEmail;
      result.ShowWelcomePage = this.ShowWelcomePage;
      result.IsClassicView = this.IsClassicView;
      result.DoNotAutoSubscribe = this.DoNotAutoSubscribe;
      result.OrgsUserCanSeeOnPortal = this.OrgsUserCanSeeOnPortal;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.ImportID = this.ImportID;
      result.SessionID = this.SessionID;
      result.LastVersion = this.LastVersion;
      result.OrganizationID = this.OrganizationID;
      result.SubscribeToNewTickets = this.SubscribeToNewTickets;
      result.SubscribeToNewActions = this.SubscribeToNewActions;
      result.ReceiveAllGroupNotifications = this.ReceiveAllGroupNotifications;
      result.ReceiveUnassignedGroupEmails = this.ReceiveUnassignedGroupEmails;
      result.ReceiveTicketNotifications = this.ReceiveTicketNotifications;
      result.InOfficeComment = this.InOfficeComment;
      result.InOffice = this.InOffice;
      result.PrimaryGroupID = this.PrimaryGroupID;
      result.IsChatUser = this.IsChatUser;
      result.IsPortalUser = this.IsPortalUser;
      result.IsPasswordExpired = this.IsPasswordExpired;
      result.IsFinanceAdmin = this.IsFinanceAdmin;
      result.IsSystemAdmin = this.IsSystemAdmin;
      result.LastWaterCoolerID = this.LastWaterCoolerID;
      result.CultureName = this.CultureName;
      result.TimeZoneID = this.TimeZoneID;
      result.MarkDeleted = this.MarkDeleted;
      result.IsActive = this.IsActive;
      result.CryptedPassword = this.CryptedPassword;
      result.Title = sanitizer.Sanitize(this.Title);
      result.LastName = sanitizer.Sanitize(this.LastName);
      result.MiddleName = sanitizer.Sanitize(this.MiddleName);
      result.FirstName = sanitizer.Sanitize(this.FirstName);
      result.Email = sanitizer.Sanitize(this.Email);
      result.UserID = this.UserID;
       
      result.LastLogin = DateTime.SpecifyKind(this.LastLoginUtc, DateTimeKind.Utc);
      result.LastActivity = DateTime.SpecifyKind(this.LastActivityUtc, DateTimeKind.Utc);
      result.ActivatedOn = DateTime.SpecifyKind(this.ActivatedOnUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      result.verificationCodeExpiration = this.verificationCodeExpirationUtc == null ? this.verificationCodeExpirationUtc : DateTime.SpecifyKind((DateTime)this.verificationCodeExpirationUtc, DateTimeKind.Utc); 
      result.DeactivatedOn = this.DeactivatedOnUtc == null ? this.DeactivatedOnUtc : DateTime.SpecifyKind((DateTime)this.DeactivatedOnUtc, DateTimeKind.Utc); 
      result.LastPing = this.LastPingUtc == null ? this.LastPingUtc : DateTime.SpecifyKind((DateTime)this.LastPingUtc, DateTimeKind.Utc);
      if (!string.IsNullOrEmpty(this.TimeZoneID))
      {
        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(this.TimeZoneID);
        if (timeZone != null) result.timeZoneDisplay = timeZone.DisplayName;
      }
      if (!string.IsNullOrEmpty(this.CultureName))
      {
        CultureInfo culture = new CultureInfo(this.CultureName);
        if (culture != null) result.CultureDisplay = culture.DisplayName;
      }
      result.UserInformation = this.UserInformation;
      result.AppChatID = this.AppChatID;
      result.AppChatStatus = this.AppChatStatus;
      result.ChangeTicketVisibility = this.ChangeTicketVisibility;
      result.CanChangeCommunityVisibility = this.CanChangeCommunityVisibility;
      result.FilterInactive = this.FilterInactive;
      result.ChangeKbVisibility = this.ChangeKBVisibility;
      result.CanCreateCompany = this.CanCreateCompany;
      result.CanCreateContact = this.CanCreateContact;
      result.CanEditCompany = this.CanEditCompany;
      result.CanEditContact = this.CanEditContact;
      result.CanCreateAsset = this.CanCreateAsset;
      result.CanEditAsset = this.CanEditAsset;
      result.CanCreateProducts = this.CanCreateProducts;
      result.CanEditProducts = this.CanEditProducts;
      result.CanCreateVersions = this.CanCreateVersions;
      result.CanEditVersions = this.CanEditVersions;
      Attachments att = new Attachments(BaseCollection.LoginUser);
      att.LoadByReference(ReferenceType.UserPhoto, this.UserID);

      if (att.Count > 0)
      {
          result.Avatar = String.Format("/dc/{0}/avatar/{1}", this.OrganizationID, att[0].AttachmentID);
      }
      else
          result.Avatar = "../images/blank_avatar.png";

         

      return result;
    }	
  }
}
