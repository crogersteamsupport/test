using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class Users
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("Email", "Email", false, false, false);
      _fieldMap.AddMap("FirstName", "FirstName", false, false, false);
      _fieldMap.AddMap("MiddleName", "MiddleName", false, false, false);
      _fieldMap.AddMap("LastName", "LastName", false, false, false);
      _fieldMap.AddMap("Title", "Title", false, false, false);
      _fieldMap.AddMap("CryptedPassword", "CryptedPassword", false, false, false);
      _fieldMap.AddMap("IsActive", "IsActive", false, false, false);
      _fieldMap.AddMap("MarkDeleted", "MarkDeleted", false, false, false);
      _fieldMap.AddMap("TimeZoneID", "TimeZoneID", false, false, false);
      _fieldMap.AddMap("CultureName", "CultureName", false, false, false);
      _fieldMap.AddMap("LastLogin", "LastLogin", false, false, false);
      _fieldMap.AddMap("LastActivity", "LastActivity", false, false, false);
      _fieldMap.AddMap("LastPing", "LastPing", false, false, false);
      _fieldMap.AddMap("LastWaterCoolerID", "LastWaterCoolerID", false, false, false);
      _fieldMap.AddMap("IsSystemAdmin", "IsSystemAdmin", false, false, false);
      _fieldMap.AddMap("IsFinanceAdmin", "IsFinanceAdmin", false, false, false);
      _fieldMap.AddMap("IsPasswordExpired", "IsPasswordExpired", false, false, false);
      _fieldMap.AddMap("IsPortalUser", "IsPortalUser", false, false, false);
      _fieldMap.AddMap("IsChatUser", "IsChatUser", false, false, false);
      _fieldMap.AddMap("PrimaryGroupID", "PrimaryGroupID", false, false, false);
      _fieldMap.AddMap("InOffice", "InOffice", false, false, false);
      _fieldMap.AddMap("InOfficeComment", "InOfficeComment", false, false, false);
      _fieldMap.AddMap("ReceiveTicketNotifications", "ReceiveTicketNotifications", false, false, false);
      _fieldMap.AddMap("ReceiveAllGroupNotifications", "ReceiveAllGroupNotifications", false, false, false);
      _fieldMap.AddMap("SubscribeToNewTickets", "SubscribeToNewTickets", false, false, false);
      _fieldMap.AddMap("ActivatedOn", "ActivatedOn", false, false, false);
      _fieldMap.AddMap("DeactivatedOn", "DeactivatedOn", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("LastVersion", "LastVersion", false, false, false);
      _fieldMap.AddMap("SessionID", "SessionID", false, false, false);
      _fieldMap.AddMap("ImportID", "ImportID", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("OrgsUserCanSeeOnPortal", "OrgsUserCanSeeOnPortal", false, false, false);
      _fieldMap.AddMap("DoNotAutoSubscribe", "DoNotAutoSubscribe", false, false, false);
      _fieldMap.AddMap("IsClassicView", "IsClassicView", false, false, false);
      _fieldMap.AddMap("SubscribeToNewActions", "SubscribeToNewActions", false, false, false);
      _fieldMap.AddMap("ApprovedTerms", "ApprovedTerms", false, false, false);
      _fieldMap.AddMap("ShowWelcomePage", "ShowWelcomePage", false, false, false);
      _fieldMap.AddMap("UserInformation", "UserInformation", false, false, false);
      _fieldMap.AddMap("PortalAutoReg", "PortalAutoReg", false, false, false);
      _fieldMap.AddMap("AppChatID", "AppChatID", false, false, false);
      _fieldMap.AddMap("AppChatStatus", "AppChatStatus", false, false, false);
      _fieldMap.AddMap("MenuItems", "MenuItems", false, false, false);
      _fieldMap.AddMap("TicketRights", "TicketRights", false, false, false);
      _fieldMap.AddMap("Signature", "Signature", false, false, false);
      _fieldMap.AddMap("LinkedIn", "LinkedIn", false, false, false);
      _fieldMap.AddMap("OnlyEmailAfterHours", "OnlyEmailAfterHours", false, false, false);
      _fieldMap.AddMap("BlockInboundEmail", "BlockInboundEmail", false, false, false);
      _fieldMap.AddMap("SalesForceID", "SalesForceID", false, false, false);
      _fieldMap.AddMap("ChangeTicketVisibility", "ChangeTicketVisibility", false, false, false);
      _fieldMap.AddMap("ChangeKBVisibility", "ChangeKBVisibility", false, false, false);
      _fieldMap.AddMap("EnforceSingleSession", "EnforceSingleSession", false, false, false);
      _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, false);
      _fieldMap.AddMap("AllowAnyTicketCustomer", "AllowAnyTicketCustomer", false, false, false);
      _fieldMap.AddMap("FontFamily", "FontFamily", false, false, false);
      _fieldMap.AddMap("FontSize", "FontSize", false, false, false);
            
    }
  }
  
}
