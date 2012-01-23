using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using System.Runtime.Serialization;
using dtSearch.Engine;
using System.Net;
using System.IO;

namespace TSWebServices
{
  [ScriptService]
  [WebService(Namespace = "http://teamsupport.com/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class OrganizationService : System.Web.Services.WebService
  {

    public OrganizationService()
    {

      //Uncomment the following line if using designed components 
      //InitializeComponent(); 
    }

    [WebMethod]
    public int GetIDByName(string name)
    {
      Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
      organizations.LoadByLikeOrganizationName(TSAuthentication.OrganizationID, name, true, 1);
      if (organizations.IsEmpty) return -1;
      return organizations[0].OrganizationID;
    }

    //[0] = orgid
    //[1] = userid
    [WebMethod]
    public int[] GetIDByPhone(string phone)
    {
      int[] result = new int[2] { -1, -1 };
      phone = phone.Replace(")", "").Replace("(", "").Replace("-", "").Replace(".", "").Replace(" ", "").ToLower();

      PhoneNumbers phoneNumbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
      phoneNumbers.LoadByOrganizationID(TSAuthentication.OrganizationID);

      foreach (PhoneNumber phoneNumber in phoneNumbers)
      {
        string number = phoneNumber.Number.Replace(")", "").Replace("(", "").Replace("-", "").Replace(".", "").Replace(" ", "").ToLower();
        if (number == phone)
        {
          if (phoneNumber.RefType == ReferenceType.Organizations)
          {
            result[0] = phoneNumber.RefID;
          }
          else
          {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), phoneNumber.RefID);
            if (user != null)
            {
              result[0] = user.OrganizationID;
              result[1] = user.UserID;
              break;
            }

          }
        }
      }
      return result;
    }

    // need to move to users service
    [WebMethod]
    public UserProxy GetUser(int userID)
    {
      User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
      if (user == null) return null;

      if (user.OrganizationID != TSAuthentication.OrganizationID)
      {
        Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), user.OrganizationID);
        if (organization.ParentID != TSAuthentication.OrganizationID) return null;
      }

      return user.GetProxy();
    }

    [WebMethod]
    public OrganizationProxy GetOrganization(int organizationID)
    {
      Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), organizationID);
      if (TSAuthentication.OrganizationID != 1088 && TSAuthentication.OrganizationID != 1078 && organization.ParentID != TSAuthentication.OrganizationID && organization.OrganizationID != TSAuthentication.OrganizationID) return null;
       return organization.GetProxy();
    }

    [WebMethod]
    public PortalOptionProxy GetPortalOption(int organizationID)
    {
      Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), organizationID);
      if (organization.OrganizationID != TSAuthentication.OrganizationID || !TSAuthentication.IsSystemAdmin) return null;
      PortalOption result = PortalOptions.GetPortalOption(organization.Collection.LoginUser, organizationID);

      if (result == null)
      {
        result = (new PortalOptions(organization.Collection.LoginUser).AddNewPortalOption());
        result.OrganizationID = organizationID;
        result.Collection.Save();
      }


      return result.GetProxy();
    }

    [WebMethod]
    public string SetPortalOption(PortalOptionProxy proxy, string externalLink, bool isPublicArticles, int? groupID)
    {
      Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), proxy.OrganizationID);
      if (organization.OrganizationID != TSAuthentication.OrganizationID || !TSAuthentication.IsSystemAdmin) return null;
      


      PortalOption option = PortalOptions.GetPortalOption(organization.Collection.LoginUser, proxy.OrganizationID);
      
      option.PortalName = PortalOptions.ValidatePortalNameChars(proxy.PortalName);
      PortalOptions options = new PortalOptions(organization.Collection.LoginUser);
      options.LoadByPortalName(option.PortalName);
      if (options.Count > 0)
      {
        if (options.Count > 1 || options[0].OrganizationID != TSAuthentication.OrganizationID)
        {
          return "That portal name is already taken.  Please choose a different portal name.";
        }
      }
      
      option.PublicLandingPageBody = proxy.PublicLandingPageBody;
      option.PublicLandingPageHeader = proxy.PublicLandingPageHeader;
      option.EnableScreenr = proxy.EnableScreenr;
      option.DisplayLandingPage = proxy.DisplayLandingPage;
      option.LandingPageHtml = proxy.LandingPageHtml;
      option.DisplayProductVersion = proxy.DisplayProductVersion;
      option.DisplayAdvKB = proxy.DisplayAdvKB;
      option.DisplayAdvProducts = proxy.DisplayAdvProducts;
      option.DisplayPortalPhone = proxy.DisplayPortalPhone;
      option.DisplayFooter = proxy.DisplayFooter;
      option.DisplayForum = proxy.DisplayForum;
      option.DeflectionEnabled = proxy.DeflectionEnabled;
      option.BasicPortalDirections = proxy.BasicPortalDirections;
      option.AdvPortalWidth = proxy.AdvPortalWidth;
      option.Theme = proxy.Theme;
      option.HideCloseButton = proxy.HideCloseButton;
      option.HonorSupportExpiration = proxy.HonorSupportExpiration;
      option.DisplayProducts = proxy.DisplayProducts;
      option.KBAccess = proxy.KBAccess;
      //option.PortalName = proxy.PortalName;
      option.DisplayGroups = proxy.DisplayGroups;
      option.BasicPortalColumnWidth = proxy.BasicPortalColumnWidth;
      option.HideGroupAssignedTo = proxy.HideGroupAssignedTo;
      option.HideUserAssignedTo = proxy.HideUserAssignedTo;
      option.CompanyRequiredInBasic = proxy.CompanyRequiredInBasic;
      option.UseCompanyInBasic = proxy.UseCompanyInBasic;
      option.PageBackgroundColor = proxy.PageBackgroundColor;
      //option.FontColor = proxy.FontColor;
      //option.FontFamily = proxy.FontFamily;
      option.UseRecaptcha = proxy.UseRecaptcha;
      option.PortalHTMLFooter = proxy.PortalHTMLFooter;
      option.PortalHTMLHeader = proxy.PortalHTMLHeader;
      //option.OrganizationID = proxy.OrganizationID;

      option.Collection.Save();

      organization.IsPublicArticles = isPublicArticles;
      organization.DefaultPortalGroupID = groupID;
      organization.Collection.Save();

      externalLink = externalLink.Trim();
      if (externalLink.IndexOf("http") < 0 && externalLink != "") externalLink = "http://" + externalLink;
      Settings.OrganizationDB.WriteString("ExternalPortalLink", externalLink);

      return null;
    }

    [WebMethod]
    public CRMLinkTableItemProxy[] AdminGetCrmLinks(int organizationID)
    {
      if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return null;

      CRMLinkTable table = new CRMLinkTable(TSAuthentication.GetLoginUser());
      table.LoadByOrganizationID(organizationID);
      return table.GetCRMLinkTableItemProxies();
    }


    [WebMethod]
    public void ResetCrmLastLink(int crmLinkID)
    {
      if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return;
      CRMLinkTableItem item = CRMLinkTable.GetCRMLinkTableItem(TSAuthentication.GetLoginUser(), crmLinkID);
      item.LastLink = null;
      item.Collection.Save();
    }

    [WebMethod]
    public CRMLinkTableItemProxy[] GetCrmLinks()
    {
      if (!TSAuthentication.IsSystemAdmin) return null;
      CRMLinkTable table = new CRMLinkTable(TSAuthentication.GetLoginUser());
      table.LoadByOrganizationID(TSAuthentication.OrganizationID);
      return table.GetCRMLinkTableItemProxies();
    }

    [WebMethod]
    public CRMLinkTableItemProxy SaveCrmLink(int crmLinkID, bool isActive, string crmType, string password, string token, string tag, string userName, bool email, bool portal)
    {
      if (!TSAuthentication.IsSystemAdmin) return null;
      CRMLinkTableItem item;

      if (crmLinkID < 0)
      {
        item = (new CRMLinkTable(TSAuthentication.GetLoginUser())).AddNewCRMLinkTableItem();
        item.LastProcessed = DateTime.UtcNow;
        item.LastTicketID = -1;
        item.OrganizationID = TSAuthentication.OrganizationID;
        item.SendBackTicketData = true;
      }
      else
      {
        item = CRMLinkTable.GetCRMLinkTableItem(TSAuthentication.GetLoginUser(), crmLinkID);

        if (item.TypeFieldMatch != tag && (crmType == "Batchbook" || crmType == "Highrise" || crmType == "Salesforce" || crmType == "ZohoCrm"))
        {
              item.LastLink = null;
          }
        if (item.OrganizationID != TSAuthentication.OrganizationID) return null;
      }

      item.Active = isActive;
      item.AllowPortalAccess = portal;
      item.CRMType = crmType;
      item.SendWelcomeEmail = email;
      item.Password = password;
      item.SecurityToken = token;
      item.TypeFieldMatch = tag;
      item.Username = userName;

      item.Collection.Save();
      return item.GetProxy();

    }


    [WebMethod]
    public CRMLinkFieldProxy[] GetCrmLinkFields(int crmLinkID)
    {
      CRMLinkFields fields = new CRMLinkFields(TSAuthentication.GetLoginUser());
      fields.LoadByCrmLinkID(crmLinkID);
      return fields.GetCRMLinkFieldProxies();
    }

    [WebMethod]
    public CRMLinkFieldProxy[] SaveCrmLinkField(int crmLinkID, int tsFieldID, bool isCustom, string crmName, ReferenceType refType)
    {
      CRMLinkTableItem link = CRMLinkTable.GetCRMLinkTableItem(TSAuthentication.GetLoginUser(), crmLinkID);
      if (link.OrganizationID != TSAuthentication.OrganizationID) return null;
      CRMLinkField field = (new CRMLinkFields(link.Collection.LoginUser)).AddNewCRMLinkField();

      if (isCustom)
      {
        CustomField custom = CustomFields.GetCustomField(link.Collection.LoginUser, tsFieldID);
        field.TSFieldName = custom.Name;
      }
      else
      {
        ReportTableField rtf = ReportTableFields.GetReportTableField(link.Collection.LoginUser, tsFieldID);
        field.TSFieldName = rtf.FieldName;
      }

      switch (refType)
      {
        case ReferenceType.Organizations:
          field.CRMObjectName = "Account";
          break;
        case ReferenceType.Contacts:
          field.CRMObjectName = "Contact";
          break;
        default:
          break;
      }

      field.CRMLinkID = crmLinkID;
      field.CRMFieldName = crmName;
      field.CustomFieldID = isCustom ? (int?)tsFieldID : null;
      field.Collection.Save();
      return GetCrmLinkFields(crmLinkID);
    }

    [WebMethod]
    public void DeleteCrmLinkField(int crmLinkFieldID)
    {
      CRMLinkField field = CRMLinkFields.GetCRMLinkField(TSAuthentication.GetLoginUser(), crmLinkFieldID);
      CRMLinkTableItem link = CRMLinkTable.GetCRMLinkTableItem(TSAuthentication.GetLoginUser(), field.CRMLinkID);
      if (link.OrganizationID != TSAuthentication.OrganizationID) return;
      field.Delete();
      field.Collection.Save();
    }

    // need to move to users service
    [WebMethod]
    public bool IsUserContact(int userID)
    {
      User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
      if (user.OrganizationID != TSAuthentication.OrganizationID) return true;
      return false;
    }

    [WebMethod]
    public OrganizationProductProxy GetOrganizationProduct(int organizationProductID)
    {
      OrganizationProduct op = OrganizationProducts.GetOrganizationProduct(TSAuthentication.GetLoginUser(), organizationProductID);
      return op.GetProxy();
    }

    [WebMethod]
    public AutocompleteItem[] AdminGetOrganizations(string name)
    {
      List<AutocompleteItem> result = new List<AutocompleteItem>();
      if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return result.ToArray();
      Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
      organizations.LoadByLikeOrganizationName(1, name, false, 20);
      foreach (Organization organization in organizations)
      {
        result.Add(new AutocompleteItem(organization.Name + " (" + organization.OrganizationID.ToString() + ")", organization.OrganizationID.ToString()));
      }

      return result.ToArray();
    }

    [WebMethod]
    public string CleanUpOrphanTicketStatuses()
    {
      if (TSAuthentication.UserID != 34) return "No Access";
      int orgID = 305383;
      Tickets tickets = new Tickets(TSAuthentication.GetLoginUser());
      tickets.LoadByOrganizationID(orgID);

      TicketStatuses statuses = new TicketStatuses(tickets.LoginUser);
      statuses.LoadByOrganizationID(orgID);
      int count = 0;
      int total = 0;
      foreach (Ticket ticket in tickets)
      {
        TicketStatus status = statuses.FindByTicketStatusID(ticket.TicketStatusID);
        if (status != null && status.TicketTypeID != ticket.TicketTypeID)
        {
          total++;
          TicketStatus newStatus = statuses.FindByName(status.Name, ticket.TicketTypeID);
          if (newStatus == null)
          {
            foreach (TicketStatus item in statuses)
	          {
              if (item.TicketTypeID == ticket.TicketTypeID && item.IsClosed == status.IsClosed)
              {
                newStatus = item;
                break;
              }
	          }
          }
          if (newStatus != null)
          {
            count++;
            ticket.TicketStatusID = newStatus.TicketStatusID;
          }
        }
      }
      tickets.Save();
      return "Clean up was successful! (" + count.ToString() + " of " + total.ToString() + ")";
    }

    [WebMethod]
    public string CopyOrganizationSettings(int sourceID, int destID)
    {
      string result = "No Access";
      if (TSAuthentication.UserID != 34) return result;

/*      Organization source = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), sourceID);
      Organization dest = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), sourceID);

      dest.ChangeStatusIfClosed = source.ChangeStatusIfClosed;
      dest.AddAdditionalContacts = source.AddAdditionalContacts;
      //dest.ModifierID = source.ModifierID;
      //dest.CreatorID = source.CreatorID;
      dest.MatchEmailSubject = source.MatchEmailSubject;
      dest.TimedActionsRequired = source.TimedActionsRequired;
      dest.CultureName = source.CultureName;
      dest.UseEuropeDate = source.UseEuropeDate;
      dest.BusinessDays = source.BusinessDays;
      dest.InternalSlaLevelID = source.InternalSlaLevelID;
      dest.SlaLevelID = source.SlaLevelID;
      dest.DefaultWikiArticleID = source.DefaultWikiArticleID;
      dest.ShowWiki = source.ShowWiki;
      dest.AdminOnlyReports = source.AdminOnlyReports;
      dest.AdminOnlyCustomers = source.AdminOnlyCustomers;
      dest.CompanyDomains = source.CompanyDomains;
      dest.OrganizationReplyToAddress = source.OrganizationReplyToAddress;
      dest.EmailDelimiter = source.EmailDelimiter;
      dest.RequireKnownUserForNewEmail = source.RequireKnownUserForNewEmail;
      dest.RequireNewKeyword = source.RequireNewKeyword;
      dest.APIRequestLimit = source.APIRequestLimit;
      dest.CRMLinkID = source.CRMLinkID;
      dest.PortalGuid = source.PortalGuid;
      dest.ChatID = source.ChatID;
      dest.SystemEmailID = source.SystemEmailID;
      dest.WebServiceID = source.WebServiceID;
      dest.ParentID = source.ParentID;
      dest.ProductType = source.ProductType;
      dest.DefaultSupportUserID = source.DefaultSupportUserID;
      dest.DefaultSupportGroupID = source.DefaultSupportGroupID;
      dest.DefaultPortalGroupID = source.DefaultPortalGroupID;
      dest.PrimaryUserID = source.PrimaryUserID;
      dest.IsBasicPortal = source.IsBasicPortal;
      dest.IsAdvancedPortal = source.IsAdvancedPortal;
      dest.HasPortalAccess = source.HasPortalAccess;
      dest.InActiveReason = source.InActiveReason;
      dest.TimeZoneID = source.TimeZoneID;
      dest.IsInventoryEnabled = source.IsInventoryEnabled;
      dest.IsApiEnabled = source.IsApiEnabled;
      dest.IsApiActive = source.IsApiActive;
      dest.IsActive = source.IsActive;
      dest.ImportID = source.ImportID;
      dest.ExtraStorageUnits = source.ExtraStorageUnits;
      dest.ChatSeats = source.ChatSeats;
      dest.PortalSeats = source.PortalSeats;
      dest.UserSeats = source.UserSeats;
      dest.IsCustomerFree = source.IsCustomerFree;
      dest.PromoCode = source.PromoCode;
      dest.EvalProcess = source.EvalProcess;
      dest.PotentialSeats = source.PotentialSeats;
      dest.PrimaryInterest = source.PrimaryInterest;
      dest.WhereHeard = source.WhereHeard;
      dest.Website = source.Website;
      dest.Description = source.Description;
      dest.Name = source.Name;
      dest.OrganizationID = source.OrganizationID;


      CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
      fields.LoadByOrganization(dest.OrganizationID);
      fields.DeleteAll();
      fields.Save();

      */
      return result;
    }

    [WebMethod]
    public ApiInfo GenerateNewApiToken()
    {
      if (!TSAuthentication.IsSystemAdmin) return null;
      Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID);
      organization.WebServiceID = Guid.NewGuid();
      organization.Collection.Save();
      return GetApiInfo();
    }

    [WebMethod]
    public ApiInfo SetApiEnabled(bool value)
    {
      if (!TSAuthentication.IsSystemAdmin) return null;
      Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID);
      organization.IsApiEnabled = value;
      organization.Collection.Save();
      return GetApiInfo();
    }

    [WebMethod]
    public ApiInfo GetApiInfo()
    {
      if (!TSAuthentication.IsSystemAdmin) return null;
      Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID);
      ApiInfo info = new ApiInfo();
      info.RequestCount = ApiLogs.GetDailyRequestCount(organization.Collection.LoginUser, TSAuthentication.OrganizationID);
      info.RequestMax = organization.APIRequestLimit;
      info.Token = organization.WebServiceID.ToString();
      info.IsEnabled = organization.IsApiEnabled;
      info.IsActive = organization.IsApiActive;
      return info;
    }

    [WebMethod]
    public AutocompleteItem[] GetUserOrOrganization(string searchTerm)
    {
      Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
      organizations.LoadByLikeOrganizationName(TSAuthentication.OrganizationID, searchTerm, true);

      UsersView users = new UsersView(organizations.LoginUser);
      users.LoadByLikeName(TSAuthentication.OrganizationID, searchTerm);

      List<AutocompleteItem> list = new List<AutocompleteItem>();
      foreach (Organization organization in organizations)
      {
        list.Add(new AutocompleteItem(organization.Name, organization.OrganizationID.ToString(), "o"));
      }

      foreach (UsersViewItem user in users)
      {
        list.Add(new AutocompleteItem(String.Format("{0}, {1} [{2}]", user.LastName, user.FirstName, user.Organization), user.UserID.ToString(), "u"));
      }

      return list.ToArray();
    }

    [WebMethod]
    public AutocompleteItem[] SearchOrganization(string searchTerm)
    {
      Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
      organizations.LoadByLikeOrganizationName(TSAuthentication.OrganizationID, searchTerm, true);

      List<AutocompleteItem> list = new List<AutocompleteItem>();
      foreach (Organization organization in organizations)
      {
        list.Add(new AutocompleteItem(organization.Name, organization.OrganizationID.ToString(), organization));
      }

      return list.ToArray();
    }

    [WebMethod]
    public string ReadServiceSettings()
    {
      if (TSAuthentication.UserID != 34) return "";
      string text = "DELETE FROM Reports WHERE ReportID = ";
      SqlCommand command = new SqlCommand(text);
      SqlExecutor.ExecuteNonQuery(TSAuthentication.GetLoginUser(), command);


      return "";
    }



    public string DataTableToHTMLTable(DataTable inTable)
    {
      StringBuilder dString = new StringBuilder();
      dString.Append("<table>");
      dString.Append(GetHeader(inTable));
      dString.Append(GetBody(inTable));
      dString.Append("</table>");
      return dString.ToString();
    }

    private string GetHeader(DataTable dTable)
    {
      StringBuilder dString = new StringBuilder();

      dString.Append("<thead><tr>");
      foreach (DataColumn dColumn in dTable.Columns)
      {
        dString.AppendFormat("<th>{0}</th>", dColumn.ColumnName);
      }
      dString.Append("</tr></thead>");

      return dString.ToString();
    }

    private string GetBody(DataTable dTable)
    {
      StringBuilder dString = new StringBuilder();

      dString.Append("<tbody>");

      foreach (DataRow dRow in dTable.Rows)
      {
        dString.Append("<tr>");
        for (int dCount = 0; dCount <= dTable.Columns.Count - 1; dCount++)
        {
          dString.AppendFormat("<td>{0}</td>", dRow[dCount]);
        }
        dString.Append("</tr>");
      }
      dString.Append("</tbody>");

      return dString.ToString();
    }

  }


  [DataContract]
  public class ApiInfo
  {
    [DataMember]
    public int RequestCount { get; set; }
    [DataMember]
    public int RequestMax { get; set; }
    [DataMember]
    public string Token { get; set; }
    [DataMember]
    public bool IsEnabled { get; set; }
    [DataMember]
    public bool IsActive { get; set; }

  }
}