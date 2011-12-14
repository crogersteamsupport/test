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
using TeamSupport.WebUtils;
using TeamSupport.Data;
using Telerik.Web.UI;
using System.Globalization;

public partial class Frames_UserInformation : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    int userID;
    User user;

    try
    {
      userID = int.Parse(Request["UserID"]);
      user = Users.GetUser(UserSession.LoginUser, userID);
      if (user.OrganizationID != TSAuthentication.OrganizationID) throw new Exception("Invalid user id.");
    }
    catch (Exception)
    {
      Response.Write("[No user to display.]");
      Response.End();
      return;
    }

    if (UserSession.CurrentUser.IsSystemAdmin)
    {
      pnlGroup.Attributes.Add("class", "");
      btnNewGroup.Visible = true;
    }
    else
    {
      pnlGroup.Attributes.Add("class", "adminDiv");
      btnNewGroup.Visible = false;
    }

    if (UserSession.CurrentUser.IsSystemAdmin || userID == UserSession.LoginUser.UserID)
    {
      btnNewPhone.Visible = true;
      btnNewAddress.Visible = true;
      btnEditProperties.Visible = true;
      pnlPhone.Attributes.Add("class", "");
      pnlAddress.Attributes.Add("class", "");
    }
    else
    {
      btnNewPhone.Visible = false;
      btnNewAddress.Visible = false;
      btnEditProperties.Visible = false;
      pnlPhone.Attributes.Add("class", "adminDiv");
      pnlAddress.Attributes.Add("class", "adminDiv");
    }

    if (btnNewAddress.Visible) btnNewAddress.OnClientClick = "ShowDialog(top.GetAddressDialog("+userID.ToString()+", 22)); return false;";
    if (btnNewPhone.Visible) btnNewPhone.OnClientClick = "ShowDialog(top.GetPhoneDialog(" + userID.ToString() + ", 22)); return false;";
    if (btnNewGroup.Visible) btnNewGroup.OnClientClick = "ShowDialog(top.GetSelectGroupDialog(" + userID.ToString() + ", 22)); return false;";
    if (btnEditProperties.Visible) btnEditProperties.OnClientClick = "ShowDialog(top.GetUserDialog(" + user.OrganizationID.ToString() + "," + userID.ToString() + ")); return false;";

    LoadDetails(userID);
  }

  private void LoadDetails(int userID)
  {
    LoadProperties(userID);
    LoadNumbers(userID);
    LoadAddresses(userID);
    LoadGroups(userID);
  }

  private void LoadProperties(int userID)
  {
    Users users = new Users(UserSession.LoginUser);

    users.LoadByUserID(userID);
    lblProperties.Visible = users.IsEmpty;

    if (!users.IsEmpty)
    {
      User user = users[0];

      DataTable table = new DataTable();
      table.Columns.Add("Name");
      table.Columns.Add("Value");

      table.Rows.Add(new string[] { "Email:", "<a href=\"mailto:" + user.Email + "\">" + user.Email + "</a>" });
      table.Rows.Add(new string[] { "Title:", user.Title });
      table.Rows.Add(new string[] { "Active:", user.IsActive.ToString() });
      table.Rows.Add(new string[] { "Email Ticket Notifications:", user.ReceiveTicketNotifications.ToString() });
      table.Rows.Add(new string[] { "Automatically subscribe to new tickets I post:", user.SubscribeToNewTickets.ToString() });
      table.Rows.Add(new string[] { "Automatically subscribe to tickets when I post an action:", user.SubscribeToNewActions.ToString() });
      table.Rows.Add(new string[] { "Do not subscribe to tickets when cc'd on email:", user.DoNotAutoSubscribe.ToString() });
      table.Rows.Add(new string[] { "Receive Assigned Group Notifications:", user.ReceiveAllGroupNotifications.ToString() });


      TimeZoneInfo timeZoneInfo = null;
      string timeZoneID = "Central Standard Time";
      if (string.IsNullOrEmpty(user.TimeZoneID))
      {
        Organization organization = (Organization)Organizations.GetOrganization(UserSession.LoginUser, user.OrganizationID);
        if (!string.IsNullOrEmpty(organization.TimeZoneID)) timeZoneID = organization.TimeZoneID;
      }
      else
	    {
        timeZoneID = user.TimeZoneID;
    	}

      CultureInfo culture = null;
      if (string.IsNullOrEmpty(user.CultureName))
      {
        culture = new CultureInfo(Organizations.GetOrganization(UserSession.LoginUser, user.OrganizationID).CultureName);
      }
      else
      {
        culture = new CultureInfo(user.CultureName);
      }

      try
      {
        timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(user.TimeZoneID);
      }
      catch (Exception)
      {
        timeZoneInfo = null;
      }

      table.Rows.Add(new string[] { "Time Zone:", timeZoneInfo == null ? "Central Standard Time" : timeZoneInfo.DisplayName });
      table.Rows.Add(new string[] { "Date Format:", culture.DisplayName });

      CustomFields fields = new CustomFields(UserSession.LoginUser);

      table.Rows.Add(new string[] { "System Administrator:", user.IsSystemAdmin.ToString() });
      table.Rows.Add(new string[] { "Financial Administrator:", user.IsFinanceAdmin.ToString() });
      if (UserSession.CurrentUser.ProductType == ProductType.Enterprise || UserSession.CurrentUser.ProductType == ProductType.HelpDesk)
        if (UserSession.CurrentUser.HasChatRights)
          table.Rows.Add(new string[] { "Chat User:", user.IsChatUser.ToString() });
      if (user.IsActive)
        table.Rows.Add(new string[] { "Activated On:", user.ActivatedOn.ToString("g", UserSession.LoginUser.CultureInfo) });
      else
      {
        if (user.DeactivatedOn != null)
        {
          DateTime dateTime = (DateTime)user.DeactivatedOn;
          table.Rows.Add(new string[] { "Deactivated On:", dateTime.ToString("g", UserSession.LoginUser.CultureInfo) });
        }
        else
        {
          table.Rows.Add(new string[] { "Deactivated On:", "" });

        }
      }

      table.Rows.Add(new string[] { "Last Logged In:", user.LastLogin.ToString("g", UserSession.LoginUser.CultureInfo) });

      fields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, ReferenceType.Users);

      foreach (CustomField field in fields)
      {
        CustomValue value = CustomValues.GetValue(UserSession.LoginUser, field.CustomFieldID, userID);
        table.Rows.Add(new string[] { field.Name + ":", value.Value });

      }

      rptProperties.DataSource = table;
      rptProperties.DataBind();

    }
  }

  private void LoadNumbers(int userID)
  {
    PhoneNumbers phoneNumbers = new PhoneNumbers(UserSession.LoginUser);
    phoneNumbers.LoadByID(userID, ReferenceType.Users);

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

  private void LoadGroups(int userID)
  {
    Groups groups = new Groups(UserSession.LoginUser);
    groups.LoadByUserID(userID);

    lblGroups.Visible = groups.IsEmpty;

    DataTable table = new DataTable();
    table.Columns.Add("GroupID");
    table.Columns.Add("UserID");
    table.Columns.Add("Name");

    foreach (Group group in groups)
    {
      table.Rows.Add(new string[] {group.GroupID.ToString(), userID.ToString(), group.Name  });
    }
    
    rptGroups.DataSource = table;
    rptGroups.DataBind();
  }

  private void LoadAddresses(int userID)
  {
    Addresses addresses = new Addresses(UserSession.LoginUser);
    addresses.LoadByID(userID, ReferenceType.Users);
    lblAddresses.Visible = addresses.IsEmpty;


    rptAddresses.DataSource = addresses;
    rptAddresses.DataBind();
  }
}
