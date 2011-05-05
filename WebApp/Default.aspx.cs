using System;
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
using System.IO;
using System.IO.Compression;
using System.Drawing;
using System.Collections.Generic;
using System.Web.Services;
using System.Text;
using System.Net.Mail;

public partial class _Default : System.Web.UI.Page
{
  protected override void InitializeCulture()
  {
    //base.InitializeCulture();
    this.Culture = UserSession.LoginUser.CultureInfo.Name;
  }

  protected override void OnLoad(EventArgs e)
  {
    //Response.Redirect("SiteDown.aspx");
    //Response.End();
    base.OnLoad(e);
    Session.Clear();
    Session.Abandon();
    Response.Cache.SetAllowResponseInBrowserHistory(false);
    Response.Cache.SetCacheability(HttpCacheability.NoCache);
    Response.Cache.SetNoStore();
    Response.Expires = 0;

    User user = Users.GetUser(UserSession.LoginUser, UserSession.LoginUser.UserID);
    user.LastActivity = DateTime.UtcNow;
    if (!TSAuthentication.IsBackdoor) user.SessionID = Guid.NewGuid();
    user.Collection.Save();
    fieldSID.Value = TSAuthentication.SessionID;

    ChatUserSetting setting = ChatUserSettings.GetSetting(UserSession.LoginUser, UserSession.LoginUser.UserID);
    setting.CurrentChatID = -1;
    setting.Collection.Save();

  }

  [WebMethod(true)]
  public static RadComboBoxItemData[] GetQuickTicket(RadComboBoxContext context)
  {
    IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;
    List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
    try
    {
      Tickets tickets = new Tickets(UserSession.LoginUser);
      string search = context["FilterString"].ToString();
      search = DataUtils.BuildSearchString(search, false);
      tickets.LoadByDescription(UserSession.LoginUser.OrganizationID, search);

      foreach (Ticket ticket in tickets)
      {
        RadComboBoxItemData itemData = new RadComboBoxItemData();
        itemData.Text = ticket.Row[0].ToString();
        itemData.Value = ticket.TicketID.ToString();
        list.Add(itemData);
      }
    }
    catch (Exception)
    {
    }
    if (list.Count < 1)
    {
      RadComboBoxItemData noData = new RadComboBoxItemData();
      noData.Text = "[No tickets to display.]";
      noData.Value = "-1";
      list.Add(noData);
    }

    return list.ToArray();
  }

  [WebMethod(true)]
  public static RadComboBoxItemData[] GetUsers(RadComboBoxContext context)
  {
    IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;
    List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
    try
    {
      Users users = new Users(UserSession.LoginUser);
      string[] s = context["FilterString"].ToString().Split(',');
      string filter = s[0];
      string search = s[1];
      switch (filter)
      {
        case "OtherChatUsers": users.LoadByName(search, UserSession.LoginUser.OrganizationID, true, false, true, UserSession.LoginUser.UserID); break;
        case "AllChatUsers": users.LoadByName(search, UserSession.LoginUser.OrganizationID, true, false, true); break;
        case "OtherUsers": users.LoadByName(search, UserSession.LoginUser.OrganizationID, true, false, false, UserSession.LoginUser.UserID); break;
        case "AdminUsers": users.LoadByName(search, UserSession.LoginUser.OrganizationID, true, true, false); break;
        default:
          users.LoadByName(search, UserSession.LoginUser.OrganizationID, true, false, false);
          break;
      }


      foreach (User user in users)
      {
        RadComboBoxItemData itemData = new RadComboBoxItemData();
        itemData.Text = user.FirstLastName;
        itemData.Value = user.UserID.ToString();
        list.Add(itemData);
      }
    }
    catch (Exception ex)
    {
      RadComboBoxItemData noData = new RadComboBoxItemData();
      noData.Text = ex.Message;
      noData.Value = "-1";
      list.Add(noData);
      return list.ToArray();
    }
    if (list.Count < 1)
    {
      RadComboBoxItemData noData = new RadComboBoxItemData();
      noData.Text = "[No users to display.]";
      noData.Value = "-1";
      list.Add(noData);
    }

    return list.ToArray();
  }

}

