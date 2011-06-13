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
      if (TSAuthentication.OrganizationID != 1078 && organization.ParentID != TSAuthentication.OrganizationID && organization.OrganizationID != TSAuthentication.OrganizationID) return null;
       return organization.GetProxy();
    }

    [WebMethod]
    public CRMLinkTableItemProxy GetCrmLink(int organizationID)
    {
      CRMLinkTable table = new CRMLinkTable(TSAuthentication.GetLoginUser());
      table.LoadByOrganizationID(organizationID);
      if (table.IsEmpty) return null;
      if (TSAuthentication.OrganizationID != 1078) return null;
      return table[0].GetProxy();
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
      if (TSAuthentication.OrganizationID != 1078) return result.ToArray();
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



  }
}