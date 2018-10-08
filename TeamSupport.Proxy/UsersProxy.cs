using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Globalization;

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
    [DataMember] public bool DisablePublic { get; set; }
    [DataMember] public bool CanCreateProducts { get; set; }
    [DataMember] public bool CanCreateVersions { get; set; }
    [DataMember] public bool CanEditProducts { get; set; }
    [DataMember] public bool CanEditVersions { get; set; }
    [DataMember] public ProductFamiliesRightType ProductFamiliesRights { get; set; }
    [DataMember] public bool? BlockEmailFromCreatingOnly { get; set; }
    [DataMember] public Guid CalGUID { get; set; }
    [DataMember] public bool PortalViewOnly { get; set; }
	[DataMember] public string verificationCode { get; set; }
	[DataMember] public string verificationPhoneNumber { get; set; }
	[DataMember] public DateTime? verificationCodeExpiration { get; set; }
	[DataMember] public DateTime? PasswordCreatedUtc { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
    [DataMember] public bool PortalLimitOrgChildrenTickets { get; set; }
    [DataMember] public bool CanBulkMerge { get; set; }

  }

}
