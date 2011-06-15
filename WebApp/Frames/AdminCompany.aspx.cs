using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;
using System.Web.Services;
using System.Text;
using System.Globalization;

public partial class Frames_AdminCompany : BaseFramePage
{
  private int _organizationID;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    _organizationID = UserSession.LoginUser.OrganizationID;


    Refresh();

    btnEditProperties.Visible = UserSession.CurrentUser.IsSystemAdmin;
    btnChat.Visible = UserSession.CurrentUser.IsSystemAdmin;
    btnNewPhone.Visible = UserSession.CurrentUser.IsSystemAdmin;
    btnNewAddress.Visible = UserSession.CurrentUser.IsSystemAdmin;
    btnCrm.Visible = UserSession.CurrentUser.IsSystemAdmin;

    if (UserSession.CurrentUser.IsSystemAdmin)
    {
      pnlProperties.Attributes.Add("class", "");
      pnlPhone.Attributes.Add("class", "");
      pnlAddress.Attributes.Add("class", "");
      pnlCrm.Attributes.Add("class", "");
    }
    else
    {
      pnlProperties.Attributes.Add("class", "adminDiv");
      pnlPhone.Attributes.Add("class", "adminDiv");
      pnlAddress.Attributes.Add("class", "adminDiv");
      pnlCrm.Attributes.Add("class", "adminDiv");
    }

    if (UserSession.CurrentUser.ProductType == ProductType.Express)
    {
      divCrm.Visible = false;
    }

    divChat.Visible = UserSession.CurrentUser.HasChatRights;
    
    if (btnNewAddress.Visible) btnNewAddress.OnClientClick = "ShowDialog(top.GetAddressDialog(" + _organizationID.ToString() + ", 9)); return false;";
    if (btnNewPhone.Visible) btnNewPhone.OnClientClick = "ShowDialog(top.GetPhoneDialog(" + _organizationID.ToString() + ", 9)); return false;";
    if (btnEditProperties.Visible) btnEditProperties.OnClientClick = "ShowDialog(top.GetMyCompanyDialog(" + _organizationID.ToString() + ")); return false;";
    if (btnCrm.Visible) btnCrm.OnClientClick = "ShowDialog(top.GetCRMPropertiesDialog(" + _organizationID.ToString() + ")); return false;";
    if (btnChat.Visible) btnChat.OnClientClick = "ShowDialog(top.GetChatPropertiesDialog(" + _organizationID.ToString() + ")); return false;";

  }


  public void Refresh()
  {
    LoadProperties(_organizationID);
    LoadCrm(_organizationID);
    LoadChat(_organizationID);
    LoadNumbers(_organizationID);
    LoadAddresses(_organizationID);
  }

  private void LoadProperties(int organizationID)
  {
    lblProperties.Visible = true;

    Organizations organizations = new Organizations(UserSession.LoginUser);
    organizations.LoadByOrganizationID(organizationID);

    if (organizations.IsEmpty) return;
    Organization organization = organizations[0];


    Users users = new Users(UserSession.LoginUser);
    string primaryUser = "[Unassigned]";
    if (organization.PrimaryUserID != null)
    {
      users.LoadByUserID((int)organization.PrimaryUserID);
      primaryUser = users.IsEmpty ? "" : users[0].FirstLastName;
    }

    
    string defaultGroup = "[Unassigned]";
    if (organization.DefaultPortalGroupID != null)
    {
      Group group = (Group)Groups.GetGroup(UserSession.LoginUser, (int)organization.DefaultPortalGroupID);
      if (group != null)
      {
        defaultGroup = group.Name;
      }
    }

    lblProperties.Visible = organizations.IsEmpty;

    DataTable table = new DataTable();
    table.Columns.Add("Name");
    table.Columns.Add("Value");

    table.Rows.Add(new string[] { "Name:", organization.Name });
    table.Rows.Add(new string[] { "Description:", organization.Description });
    //table.Rows.Add(new string[] { "Portal Access:", organization.HasPortalAccess.ToString() });
    /*
    if (UserSession.CurrentUser.HasPortalRights)
    {
      string portalLink = "http://portal.teamsupport.com?OrganizationID=" + organization.OrganizationID.ToString();
      portalLink = @"<a href=""" + portalLink + @""" target=""PortalLink"" onclick=""window.open('" + portalLink + @"', 'PortalLink')"">" + portalLink + "</a>";
      table.Rows.Add(new string[] { "Portal Link:", portalLink });
      table.Rows.Add(new string[] { "Default Portal Group:", defaultGroup });
    }
     */
    table.Rows.Add(new string[] { "Organization ID:", organization.OrganizationID.ToString() });
    table.Rows.Add(new string[] { "Primary Contact:", primaryUser });
    
    
    //"Central Standard Time"

    TimeZoneInfo timeZoneInfo = null;
    try 
	  {	        
  		timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(organization.TimeZoneID);
	  }
	  catch (Exception)
	  {
      timeZoneInfo = null;
	  }

    table.Rows.Add(new string[] { "Time Zone:", timeZoneInfo == null ? "Central Standard Time" : timeZoneInfo.DisplayName });

    table.Rows.Add(new string[] { "Date Format:", (new CultureInfo(organization.CultureName)).DisplayName });
    table.Rows.Add(new string[] { "Business Days:", organization.BusinessDaysText == "" ? "[None Assigned]" : organization.BusinessDaysText });
    table.Rows.Add(new string[] { "Business Day Start:", organization.BusinessDayStart == null ? "[None Assigned]" : ((DateTime)organization.BusinessDayStart).ToString("t", UserSession.LoginUser.CultureInfo) });
    table.Rows.Add(new string[] { "Business Day End:", organization.BusinessDayEnd == null ? "[None Assigned]" : ((DateTime)organization.BusinessDayEnd).ToString("t", UserSession.LoginUser.CultureInfo) });


    
    table.Rows.Add(new string[] { "Only show products for the customers of a ticket:", Settings.OrganizationDB.ReadBool("ShowOnlyCustomerProducts", false).ToString() });
    table.Rows.Add(new string[] { "Require customer for new ticket:", Settings.OrganizationDB.ReadBool("RequireNewTicketCustomer", false).ToString() });
    table.Rows.Add(new string[] { "Require time spent on timed actions:", organization.TimedActionsRequired.ToString() });
    table.Rows.Add(new string[] { "Disable ticket status update emails:", Settings.OrganizationDB.ReadBool("DisableStatusNotification", false).ToString() });
    table.Rows.Add(new string[] { "Chat ID:", organization.ChatID.ToString() });
    
/*    string email = organization.SystemEmailID + "@teamsupport.com";
    table.Rows.Add(new string[] { "System Email:", "<a href=\"mailto:" + email + "\">" + email + "</a>" });
    table.Rows.Add(new string[] { "Organization Reply To Address:", organization.OrganizationReplyToAddress });
    table.Rows.Add(new string[] { "Require [New] keyword for emails:", organization.RequireNewKeyword.ToString() });
    table.Rows.Add(new string[] { "Require a known email address for emails:", organization.RequireKnownUserForNewEmail.ToString() });
    */
    table.Rows.Add(new string[] { "Default Wiki Article:", organization.DefaultWikiArticleID == null ? "[None Assigned]" : WikiArticles.GetWikiArticle(UserSession.LoginUser, (int)organization.DefaultWikiArticleID).ArticleName });
    table.Rows.Add(new string[] { "Only Admin Can Modify Customers:", organization.AdminOnlyCustomers.ToString() });
    table.Rows.Add(new string[] { "Only Admin Can View Reports:", organization.AdminOnlyReports.ToString() });

    if (organization.InternalSlaLevelID != null)
    {
      SlaLevel level = SlaLevels.GetSlaLevel(UserSession.LoginUser, (int)organization.InternalSlaLevelID);
      table.Rows.Add(new string[] { "Internal SLA:", level.Name });
    }
    else
	  {
      table.Rows.Add(new string[] { "Internal SLA:", "[None Assigned]" });
	  }


    
    //table.Rows.Add(new string[] { "Web Services ID:", organization.WebServiceID.ToString() });
    if (UserSession.CurrentUser.IsSystemAdmin)
    {

      if (organization.ProductType != ProductType.Express)
      {
        if (organization.IsApiActive != null && organization.IsApiActive == true)
        {
          //table.Rows.Add(new string[] { "API is Activated.", "" });
          if (organization.IsApiEnabled)
            table.Rows.Add(new string[] { "API is Enabled.", "" });
          else
            table.Rows.Add(new string[] { "API is Disabled.", "" });

          table.Rows.Add(new string[] { "API Authentication Token:", "<span class=\"apiToken\" style=\"display:none;\"></span><a class=\"showApiToken\" href=\"#\" onclick=\"GetApiToken();\">Show Me</a>" });

        }
        else
        {
          table.Rows.Add(new string[] { "API is Not Activated.", "Please contact us at 800.596.2820 x806 or send an email to <a href=\"mailto:support@teamsupport.com\">sales@teamsupport.com</a>" });
        }
      }
      
    }
    rptProperties.DataSource = table;
    rptProperties.DataBind();
  }

  private void LoadCrm(int organizationID)
  {
    //lblCrm.Visible = true;
    //lblCrm.Text = "Coming Soon.";
    //return;

    lblCrm.Visible = false;

    CRMLinkTable crmLinkTable = new CRMLinkTable(UserSession.LoginUser);
    crmLinkTable.LoadByOrganizationID(organizationID);

    if (crmLinkTable.IsEmpty)
    {
      lblCrm.Visible = true;
      btnCRMResync.Visible = false;
      return;
    
    }
    

    DataTable table = new DataTable();
    table.Columns.Add("Name");
    table.Columns.Add("Value");

    CRMLinkTableItem item = crmLinkTable[0];
    if (item.CRMType.ToLower().Trim() == "highrise")
    {
      string link = @"{0} <a href=""http://{0}.highrisehq.com"" target=""_blank"">Test</a>";
      table.Rows.Add(new string[] { "CRM Type:", "Highrise" });
      table.Rows.Add(new string[] { "Highrise company name:", string.Format(link, item.Username) });
      table.Rows.Add(new string[] { "Tag which identifies customer:", item.TypeFieldMatch });
      table.Rows.Add(new string[] { "CRM synchronization active:", item.Active.ToString() });
    }
    else if (item.CRMType.ToLower().Trim() == "batchbook")
    {
      table.Rows.Add(new string[] { "CRM Type:", "Batchbook" });
      table.Rows.Add(new string[] { "Batchbook user name:", item.Username });
      table.Rows.Add(new string[] { "Tag which identifies customer:", item.TypeFieldMatch });
      table.Rows.Add(new string[] { "CRM synchronization active:", item.Active.ToString() });
    }
    else if (item.CRMType.ToLower().Trim() == "salesforce")
    {
      table.Rows.Add(new string[] { "CRM Type:", "Salesforce.com" });
      table.Rows.Add(new string[] { "Salesforce user name:", item.Username });
      table.Rows.Add(new string[] { "Account type to link to TeamSupport:", item.TypeFieldMatch });
      table.Rows.Add(new string[] { "CRM synchronization active:", item.Active.ToString() });
    }
    else
    {
      lblCrm.Visible = true;
      return;
    }

    rptCrm.DataSource = table;
    rptCrm.DataBind();
  }

  private void LoadChat(int organzationID)
  {
    Organization organization = Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);

    ChatSetting setting = ChatSettings.GetSetting(UserSession.LoginUser, organzationID);
    StringBuilder builder = new StringBuilder();
    //builder.Append("<table cellpadding=\"0\" cellspacing=\"5\" border=\"0\">");
    //builder.Append("<tr>");
    //builder.Append("<td>Available Image:</td>");
    builder.Append("<div><strong>Customer Chat Link:</strong></div>");
    builder.Append("<div>");
    string script = string.Format("window.open('https://app.teamsupport.com/Chat/ChatInit.aspx?uid={0}', 'TSChat', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=450,height=500'); return false;", organization.ChatID.ToString());
    
    string link = string.Format("<a href=\"#\" onclick=\"{0}\"><img src=\"https://app.teamsupport.com/dc/{1}/chat/image\" border=\"0\" /></a>", script, organization.OrganizationID);
    textChatCode.Text = link;
    builder.Append(string.Format("<a href=\"#\" onclick=\"{0}\">Test</a>", script));
    builder.Append("</div>");
    divChatProperties.InnerHtml = builder.ToString();
  }

  private void LoadNumbers(int organizationID)
  {
    PhoneNumbers phoneNumbers = new PhoneNumbers(UserSession.LoginUser);
    phoneNumbers.LoadByID(organizationID, ReferenceType.Organizations);

    lblPhone.Visible = phoneNumbers.IsEmpty;

    DataTable table = new DataTable();
    table.Columns.Add("PhoneID");
    table.Columns.Add("Type");
    table.Columns.Add("Number");
    table.Columns.Add("Ext");

    foreach (PhoneNumber phoneNumber in phoneNumbers)
    {
      table.Rows.Add(new string[] { phoneNumber.PhoneID.ToString(), phoneNumber.PhoneTypeName, phoneNumber.Number, phoneNumber.Extension == "" ? "" : " Ext: " + phoneNumber.Extension });
    }

    rptPhone.DataSource = table;
    rptPhone.DataBind();
  }

  [WebMethod(true)]
  public static string GetApiToken()
  {
    if (!UserSession.CurrentUser.IsSystemAdmin) return "";
    Organization organization = Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
    return organization.WebServiceID.ToString();
  }

  private void LoadAddresses(int organizationID)
  {
    Addresses addresses = new Addresses(UserSession.LoginUser);
    addresses.LoadByID(organizationID, ReferenceType.Organizations);

    lblAddresses.Visible = addresses.IsEmpty;

    rptAddresses.DataSource = addresses;
    rptAddresses.DataBind();
  }

  [WebMethod(true)]
  public static void ForceCRMResync()
  {
    CRMLinkTable crmLinkTable = new CRMLinkTable(UserSession.LoginUser);
    crmLinkTable.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
    if (!crmLinkTable.IsEmpty)
    {
      crmLinkTable[0].LastProcessed = new DateTime(1900, 1, 1);
      crmLinkTable[0].LastLink = new DateTime(1980, 1, 1);
      crmLinkTable.Save();
    }
  
  }

  [WebMethod(true)]
  public static void MarkChatOffline()
  {
    Users.MarkUsersChatUnavailable(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);  
  }


}
