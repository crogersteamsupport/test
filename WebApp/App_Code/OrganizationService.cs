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
using Newtonsoft.Json;

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
      name = name.Replace('+', ' ').Replace('_', ' ');
      Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
      organizations.LoadByLikeOrganizationName(TSAuthentication.OrganizationID, name, true, 1);
      if (organizations.IsEmpty) return -1;
      return organizations[0].OrganizationID;
    }


    [WebMethod]
    public int GetIDByExactName(string name)
    {
        name = name.Replace('+', ' ').Replace('_', ' ');
        Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
        organizations.LoadByOrganizationNameActive(name, TSAuthentication.OrganizationID);
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
    public bool IsProductRequired()
    {   
        Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID);

        return organization.ProductRequired;
    }

    [WebMethod]
    public bool IsProductVersionRequired()
    {
        Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID);

        return organization.ProductVersionRequired;
    }

    [WebMethod]
    public PortalOptionProxy GetPortalOption(int organizationID)
    {
      Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), organizationID);
      //if (organization.OrganizationID != TSAuthentication.OrganizationID || !TSAuthentication.IsSystemAdmin) return null;
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

      FacebookOptions fb = new FacebookOptions(TSAuthentication.GetLoginUser());
      fb.LoadByOrganizationID(proxy.OrganizationID);
      if (fb.IsEmpty)
      {
          FacebookOption fbo = (new FacebookOptions(TSAuthentication.GetLoginUser()).AddNewFacebookOption());
          fbo.OrganizationID = proxy.OrganizationID;
          fbo.DisplayArticles = proxy.DisplayFbArticles;
          fbo.DisplayKB = proxy.DisplayFbKB;
          fbo.Collection.Save();
      }
      else
      {
          fb[0].DisplayArticles = proxy.DisplayFbArticles;
          fb[0].DisplayKB = proxy.DisplayFbKB;
          fb[0].Collection.Save();
      }

      option.EnableSaExpiration = proxy.EnableSaExpiration;
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
      option.DisablePublicMyTickets = proxy.DisablePublicMyTickets;
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
      option.DisplayAdvArticles = proxy.DisplayAdvArticles;
      //option.OrganizationID = proxy.OrganizationID;
      option.TwoColumnFields = proxy.TwoColumnFields;
      option.DisplayForum = proxy.DisplayForum;
      option.DisplayTandC = proxy.DisplayTandC;
      option.TermsAndConditions = proxy.TermsAndConditions;
      option.AutoRegister = proxy.AutoRegister;
      option.RequestAccess = proxy.RequestAccess;
      option.RequestGroup = proxy.RequestGroup;
      option.RequestType = proxy.RequestType;

      option.Collection.Save();

      organization.IsPublicArticles = isPublicArticles;
      organization.DefaultPortalGroupID = groupID;
      organization.UseForums = proxy.DisplayForum == null ? false : (bool)proxy.DisplayForum;
      organization.Collection.Save();

      externalLink = externalLink.Trim();
      if (externalLink.IndexOf("http") < 0 && externalLink != "") externalLink = "http://" + externalLink;
      Settings.OrganizationDB.WriteString("ExternalPortalLink", externalLink);

      return null;
    }

    [WebMethod]
    public bool? UpdateUseCommunity(bool value) 
    {
      if (!TSAuthentication.IsSystemAdmin) return null;
      Organization org = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID);
      org.UseForums = value;
      org.Collection.Save();
      return value;
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
    public void SetShowWelcomePage(int organizationID)
    {
      if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return;
      Users users = new Users(TSAuthentication.GetLoginUser());
      users.LoadByOrganizationID(organizationID, true);
      foreach (User user in users)
      {
        if (user.IsSystemAdmin) user.ShowWelcomePage = true;
      }

      users.Save();
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
    public SlaLevelProxy[] GetSlaLevels()
    {
      if (!TSAuthentication.IsSystemAdmin) return null;
      SlaLevels table = new SlaLevels(TSAuthentication.GetLoginUser());
      table.LoadByOrganizationID(TSAuthentication.OrganizationID);
      return table.GetSlaLevelProxies();
    }

    [WebMethod]
    public CRMLinkTableItemProxy SaveCrmLink(
      int     crmLinkID, 
      bool    isActive, 
      string  crmType, 
      string  password, 
      string  token, 
      string  tag, 
      string  userName, 
      bool    email, 
      bool    portal, 
      int?    defaultSlaLevelID,
      bool    pullCasesAsTickets,
      bool    pushTicketsAsCases,
      bool    pushTicketsAsAccountComments,
      bool    pullCustomerProducts,
      int?    actionTypeIDToPush,
      string  hostName,
      string  defaultProject,
      bool?   updateStatus,
      bool    matchAccountsByName
    )
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
      item.SecurityToken1 = token;
      item.TypeFieldMatch = tag;
      item.Username = userName;
      item.DefaultSlaLevelID = defaultSlaLevelID;
      item.PullCasesAsTickets = pullCasesAsTickets;
      item.PushTicketsAsCases = pushTicketsAsCases;
      item.SendBackTicketData = pushTicketsAsAccountComments;
      item.PullCustomerProducts = pullCustomerProducts;
      item.ActionTypeIDToPush = actionTypeIDToPush;
      item.HostName = hostName;
      item.DefaultProject = defaultProject;
      item.UpdateStatus = updateStatus;
      item.MatchAccountsByName = matchAccountsByName;

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
        case ReferenceType.Tickets:
          field.CRMObjectName = "Ticket";
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
      return AdminGetCustomers(1, name);
    }

    [WebMethod]
    public void AdminSetAllPortalUsers(int organizationID, bool value)
    {
      if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return;
      Organizations.SetAllPortalUsers(TSAuthentication.GetLoginUser(), organizationID, value);
    }


    [WebMethod]
    public void AdminQueryOrganizations(int parentID, string query)
    {
      List<TypeAheadItem> result = new List<TypeAheadItem>();
      if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return;

      int orgID = -1;
      bool flag = true;
      if (int.TryParse(query, out orgID))
      {
        Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
        if (organization != null)
        {
          result.Add(new TypeAheadItem(organization.Name + " (" + organization.OrganizationID.ToString() + ")", organization.OrganizationID.ToString()));
          flag = false;
        }
      
      }

      if (flag) 
      {
        Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
        organizations.LoadByOrganizationName(query, parentID);

        if (organizations.Count > 0)
        {
          foreach (Organization organization in organizations)
          {
            result.Add(new TypeAheadItem(organization.Name + " (" + organization.OrganizationID.ToString() + ")", organization.OrganizationID.ToString()));
          }
        }
        else
        {
          organizations.LoadByLikeOrganizationName(parentID, query, false, 20);
          foreach (Organization organization in organizations)
          {
            result.Add(new TypeAheadItem(organization.Name + " (" + organization.OrganizationID.ToString() + ")", organization.OrganizationID.ToString()));
          }
        }
      }

      HttpContext.Current.Response.ContentType = "application/json";
      HttpContext.Current.Response.Write(JsonConvert.SerializeObject(result));
      HttpContext.Current.Response.End();
    }

    [WebMethod]
    public AutocompleteItem[] AdminGetCustomers(int parentID, string name)
    {
      List<AutocompleteItem> result = new List<AutocompleteItem>();
      if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return result.ToArray();
      Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
      organizations.LoadByLikeOrganizationName(parentID, name, false, 20);
      foreach (Organization organization in organizations)
      {
        result.Add(new AutocompleteItem(organization.Name + " (" + organization.OrganizationID.ToString() + ")", organization.OrganizationID.ToString()));
      }

      return result.ToArray();
    }


    [WebMethod]
    public int AdminGetUserCount(int orgID)
    {
      Organization org = GetAdminOrgTarget(orgID);
      return Organizations.GetUserCount(TSAuthentication.GetLoginUser(), org.OrganizationID);
    }

    [WebMethod]
    public int AdminGetStorageUsed(int orgID)
    {
      Organization org = GetAdminOrgTarget(orgID);
      return Organizations.GetStorageUsed(TSAuthentication.GetLoginUser(), org.OrganizationID);
    }

    [WebMethod]
    public void AdminRenameCompany(int orgID, string name)
    {
      Organization org = GetAdminOrgTarget(orgID);
      org.Name = name;
      org.Collection.Save();
    }

    [WebMethod]
    public void AdminUpdateSeats(int orgID, int count)
    {
      Organization org = GetAdminOrgTarget(orgID);
      org.UserSeats = count;
      org.Collection.Save();
    }

    [WebMethod]
    public void AdminUpdateProductType(int orgID, int productType)
    {
      Organization org = GetAdminOrgTarget(orgID);
      org.ProductType = (ProductType)productType;
      org.IsInventoryEnabled = org.ProductType == ProductType.Enterprise;
      org.Collection.Save();
    }

    [WebMethod]
    public void AdminSetInventory(int orgID, bool value)
    {
      Organization org = GetAdminOrgTarget(orgID);
      org.IsInventoryEnabled = value;
      org.Collection.Save();
    }

    [WebMethod]
    public void AdminSetApiCount(int orgID, int count)
    {
      Organization org = GetAdminOrgTarget(orgID);
      org.APIRequestLimit = count;
      org.Collection.Save();
    }

    [WebMethod]
    public void AdminEnable(int orgID, bool value)
    {
      Organization org = GetAdminOrgTarget(orgID);
      org.IsActive = value;
      org.IsAdvancedPortal = value;
      org.IsBasicPortal = value;
      org.IsApiActive = value;
      org.IsApiEnabled = value;
      org.IsInventoryEnabled = value;
      org.Collection.Save();
    }

    public Organization GetAdminOrgTarget(int orgID)
    {
      if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return null;
      return Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
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
    public AutocompleteItem[] GetUserOrOrganizationForTicket(string searchTerm)
    {
      User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
      return GetUserOrOrganizationFiltered(searchTerm, !user.AllowAnyTicketCustomer);
    }

    [WebMethod]
    public AutocompleteItem[] GetUserOrOrganization(string searchTerm)
    {
      return GetUserOrOrganizationFiltered(searchTerm, true);
    }

    public AutocompleteItem[] GetUserOrOrganizationFiltered(string searchTerm, bool filterByUserRights)
    {
      Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
      organizations.LoadByLikeOrganizationName(TSAuthentication.OrganizationID, searchTerm, true, 50, filterByUserRights);

      UsersView users = new UsersView(organizations.LoginUser);
      users.LoadByLikeName(TSAuthentication.OrganizationID, searchTerm, 50, true, true);

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
        list.Add(new AutocompleteItem(organization.Name, organization.OrganizationID.ToString()));
      }

      return list.ToArray();
    }

    [WebMethod]
    public AutocompleteItem[] WCSearchOrganization(string searchTerm)
    {
        Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
        organizations.LoadByLikeOrganizationName(TSAuthentication.OrganizationID, searchTerm, true);

        List<AutocompleteItem> list = new List<AutocompleteItem>();
        foreach (Organization organization in organizations)
        {
            list.Add(new AutocompleteItem(organization.Name, organization.OrganizationID.ToString(), organization.OrganizationID.ToString()));
        }

        return list.ToArray();
    }


    [WebMethod]
    public string ReadServiceSettings()
    {
      if (TSAuthentication.UserID != 34) return "";
      string text = "UPDATE Tickets SET NeedsIndexing = 1";
      SqlCommand command = new SqlCommand(text);
      //SqlExecutor.ExecuteNonQuery(TSAuthentication.GetLoginUser(), command);
      
      text = "SELECT count(*) FROM tickets where needsindexing > 0 ";
      command = new SqlCommand(text);
      return DataTableToHTMLTable(SqlExecutor.ExecuteQuery(TSAuthentication.GetLoginUser(), command));
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