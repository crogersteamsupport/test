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
    [DataMember] public bool SubscribeToNewTickets { get; set; }
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
    [DataMember] public bool SubscribeToNewActions { get; set; }
    [DataMember] public bool ApprovedTerms { get; set; }
    [DataMember] public bool ShowWelcomePage { get; set; }
    [DataMember] public string UserInformation { get; set; }
    [DataMember] public bool PortalAutoReg { get; set; }
    [DataMember] public string AppChatID { get; set; }
    [DataMember] public bool AppChatStatus { get; set; }
          
  }
  
  public partial class User : BaseItem
  {
    public UserProxy GetProxy()
    {
      UserProxy result = new UserProxy();
      result.AppChatStatus = this.AppChatStatus;
      result.AppChatID = this.AppChatID;
      result.PortalAutoReg = this.PortalAutoReg;
      result.UserInformation = this.UserInformation;
      result.ShowWelcomePage = this.ShowWelcomePage;
      result.ApprovedTerms = this.ApprovedTerms;
      result.SubscribeToNewActions = this.SubscribeToNewActions;
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
      result.ReceiveAllGroupNotifications = this.ReceiveAllGroupNotifications;
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
      result.Title = this.Title;
      result.LastName = this.LastName;
      result.MiddleName = this.MiddleName;
      result.FirstName = this.FirstName;
      result.Email = this.Email;
      result.UserID = this.UserID;
       
      result.LastLogin = DateTime.SpecifyKind(this.LastLoginUtc, DateTimeKind.Utc);
      result.LastActivity = DateTime.SpecifyKind(this.LastActivityUtc, DateTimeKind.Utc);
      result.ActivatedOn = DateTime.SpecifyKind(this.ActivatedOnUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      result.DeactivatedOn = this.DeactivatedOnUtc == null ? this.DeactivatedOnUtc : DateTime.SpecifyKind((DateTime)this.DeactivatedOnUtc, DateTimeKind.Utc); 
      result.LastPing = this.LastPingUtc == null ? this.LastPingUtc : DateTime.SpecifyKind((DateTime)this.LastPingUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
