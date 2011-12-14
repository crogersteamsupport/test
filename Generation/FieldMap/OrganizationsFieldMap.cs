using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class Organizations
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("Name", "Name", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("Website", "Website", false, false, false);
      _fieldMap.AddMap("WhereHeard", "WhereHeard", false, false, false);
      _fieldMap.AddMap("PromoCode", "PromoCode", false, false, false);
      _fieldMap.AddMap("IsCustomerFree", "IsCustomerFree", false, false, false);
      _fieldMap.AddMap("UserSeats", "UserSeats", false, false, false);
      _fieldMap.AddMap("PortalSeats", "PortalSeats", false, false, false);
      _fieldMap.AddMap("ChatSeats", "ChatSeats", false, false, false);
      _fieldMap.AddMap("ExtraStorageUnits", "ExtraStorageUnits", false, false, false);
      _fieldMap.AddMap("ImportID", "ImportID", false, false, false);
      _fieldMap.AddMap("IsActive", "IsActive", false, false, false);
      _fieldMap.AddMap("IsApiActive", "IsApiActive", false, false, false);
      _fieldMap.AddMap("IsApiEnabled", "IsApiEnabled", false, false, false);
      _fieldMap.AddMap("IsInventoryEnabled", "IsInventoryEnabled", false, false, false);
      _fieldMap.AddMap("TimeZoneID", "TimeZoneID", false, false, false);
      _fieldMap.AddMap("InActiveReason", "InActiveReason", false, false, false);
      _fieldMap.AddMap("HasPortalAccess", "HasPortalAccess", false, false, false);
      _fieldMap.AddMap("IsAdvancedPortal", "IsAdvancedPortal", false, false, false);
      _fieldMap.AddMap("IsBasicPortal", "IsBasicPortal", false, false, false);
      _fieldMap.AddMap("PrimaryUserID", "PrimaryUserID", false, false, false);
      _fieldMap.AddMap("DefaultPortalGroupID", "DefaultPortalGroupID", false, false, false);
      _fieldMap.AddMap("DefaultSupportGroupID", "DefaultSupportGroupID", false, false, false);
      _fieldMap.AddMap("DefaultSupportUserID", "DefaultSupportUserID", false, false, false);
      _fieldMap.AddMap("ProductType", "ProductType", false, false, false);
      _fieldMap.AddMap("ParentID", "ParentID", false, false, false);
      _fieldMap.AddMap("WebServiceID", "WebServiceID", false, false, false);
      _fieldMap.AddMap("SystemEmailID", "SystemEmailID", false, false, false);
      _fieldMap.AddMap("ChatID", "ChatID", false, false, false);
      _fieldMap.AddMap("PortalGuid", "PortalGuid", false, false, false);
      _fieldMap.AddMap("CRMLinkID", "CRMLinkID", false, false, false);
      _fieldMap.AddMap("SAExpirationDate", "SAExpirationDate", false, false, false);
      _fieldMap.AddMap("APIRequestLimit", "APIRequestLimit", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("RequireNewKeyword", "RequireNewKeyword", false, false, false);
      _fieldMap.AddMap("RequireKnownUserForNewEmail", "RequireKnownUserForNewEmail", false, false, false);
      _fieldMap.AddMap("EmailDelimiter", "EmailDelimiter", false, false, false);
      _fieldMap.AddMap("OrganizationReplyToAddress", "OrganizationReplyToAddress", false, false, false);
      _fieldMap.AddMap("CompanyDomains", "CompanyDomains", false, false, false);
      _fieldMap.AddMap("AdminOnlyCustomers", "AdminOnlyCustomers", false, false, false);
      _fieldMap.AddMap("AdminOnlyReports", "AdminOnlyReports", false, false, false);
      _fieldMap.AddMap("ShowWiki", "ShowWiki", false, false, false);
      _fieldMap.AddMap("DefaultWikiArticleID", "DefaultWikiArticleID", false, false, false);
      _fieldMap.AddMap("SlaLevelID", "SlaLevelID", false, false, false);
      _fieldMap.AddMap("InternalSlaLevelID", "InternalSlaLevelID", false, false, false);
      _fieldMap.AddMap("BusinessDays", "BusinessDays", false, false, false);
      _fieldMap.AddMap("BusinessDayStart", "BusinessDayStart", false, false, false);
      _fieldMap.AddMap("BusinessDayEnd", "BusinessDayEnd", false, false, false);
      _fieldMap.AddMap("UseEuropeDate", "UseEuropeDate", false, false, false);
      _fieldMap.AddMap("CultureName", "CultureName", false, false, false);
      _fieldMap.AddMap("TimedActionsRequired", "TimedActionsRequired", false, false, false);
      _fieldMap.AddMap("MatchEmailSubject", "MatchEmailSubject", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("PrimaryInterest", "PrimaryInterest", false, false, false);
      _fieldMap.AddMap("PotentialSeats", "PotentialSeats", false, false, false);
      _fieldMap.AddMap("EvalProcess", "EvalProcess", false, false, false);
      _fieldMap.AddMap("AddAdditionalContacts", "AddAdditionalContacts", false, false, false);
      _fieldMap.AddMap("ChangeStatusIfClosed", "ChangeStatusIfClosed", false, false, false);
      _fieldMap.AddMap("IsPublicArticles", "IsPublicArticles", false, false, false);
      _fieldMap.AddMap("UseForums", "UseForums", false, false, false);
            
    }
  }
  
}
