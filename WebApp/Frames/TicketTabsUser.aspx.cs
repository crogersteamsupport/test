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
using System.Text;

public partial class Frames_TicketTabsUser : BaseFramePage
{
  private User _user;


  protected void Page_Load(object sender, EventArgs e)
  {
    if (!IsPostBack)
    {
      try 
      {
        int userID = int.Parse(Request["UserID"]);
        _user = Users.GetUser(UserSession.LoginUser, userID);
        if (_user.OrganizationID != UserSession.LoginUser.OrganizationID) throw new Exception("Invalid User");


        CreateTabButtons();
        tsMain.SelectedIndex = Settings.UserDB.ReadInt("SelectedUserTicketTabIndex" + Request.Url, 0);
        ticketsFrame.Attributes["src"] = tsMain.SelectedTab.Value;
      }
      catch (Exception ex)
      {
        Response.Write("Invalid User.");
        Response.End();
      }
    }
  }

  private RadTab CreateTab(string text, string url)
  {
    RadTab tab = new RadTab();
    tab.Text = text;
    tab.Value = url;
    return tab;
  }

  private void CreateTabButtons()
  {
    tsMain.Tabs.Clear();

    StringBuilder builder = new StringBuilder();
    builder.Append("Tickets.aspx?UserID=" + _user.UserID);
    builder.Append("&TicketStatusID=" + TicketFilters.Values.Opened);
    tsMain.Tabs.Add(CreateTab("Open Tickets", builder.ToString()));

    builder = new StringBuilder();
    builder.Append("Tickets.aspx?UserID=" + _user.UserID);
    builder.Append("&TicketStatusID=" + TicketFilters.Values.Closed);
    tsMain.Tabs.Add(CreateTab("Closed Tickets", builder.ToString()));

    builder = new StringBuilder();
    builder.Append("Tickets.aspx?UserID=" + _user.UserID);
    tsMain.Tabs.Add(CreateTab("All Tickets", builder.ToString()));

    builder = new StringBuilder();
    builder.Append("TicketQueue.aspx?UserID=" + _user.UserID);
    tsMain.Tabs.Add(CreateTab("Ticket Queue", builder.ToString()));
  }

}
