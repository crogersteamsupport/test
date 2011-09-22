using System;
using System.Collections.Generic;
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

public partial class UserControls_Actions : System.Web.UI.UserControl
{
  private int _ticketID;

  public int TicketID
  {
    get { return _ticketID; }
    set { _ticketID = value; }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    LoadActions(_ticketID);
  }

  private void LoadActions(int ticketID)
  {
    actionDetailsDiv.Visible = false;
    
    Actions actions = new Actions(UserSession.LoginUser);
    actions.LoadByTicketID(ticketID);
    if (actions.Count < 1) return;

    actionDetailsDiv.Visible = true;

    DataTable table = new DataTable();
    table.Columns.Add("ActionID");
    table.Columns.Add("TicketID");
    table.Columns.Add("Name");
    table.Columns.Add("Description");
    table.Columns.Add("UserName");
    table.Columns.Add("Date");
    table.Columns.Add("Attachments");
    table.Columns.Add("Time");
    table.Columns.Add("Properties");
    table.Columns.Add("CanDelete");
    table.Columns.Add("CanEdit");


    ActionTypes actionTypes = new ActionTypes(UserSession.LoginUser);
    actionTypes.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);

    foreach (TeamSupport.Data.Action action in actions)
    {
      actionDetailsDiv.Visible = true;
      if (action.SystemActionTypeID == SystemActionType.Description)
      {
        table.Rows.Add(new object[] {
          action.ActionID, action.TicketID,
          "Description", 
          GetDescriptionHTML(action), 
          GetUserNameLink(action), 
          action.DateCreated.ToString("g", UserSession.LoginUser.CultureInfo), 
          GetAttachmentList(action),
          "",
          GetPropertyText(action),
          "hiddenButton",
          GetButtonHidden(action)});
      }
      else if (action.SystemActionTypeID == SystemActionType.Resolution)
      {
        table.Rows.Add(new object[] {
        action.ActionID, action.TicketID,
        "Resolution", 
        GetDescriptionHTML(action), 
          GetUserNameLink(action), 
        action.DateCreated.ToString("g", UserSession.LoginUser.CultureInfo), 
        GetAttachmentList(action),
        "",
        GetPropertyText(action),
        GetButtonHidden(action),
        GetButtonHidden(action)
        });
      }
      else if (action.SystemActionTypeID == SystemActionType.Chat)
      {
        table.Rows.Add(new object[] {
        action.ActionID, action.TicketID,
        "Chat", 
        GetDescriptionHTML(action), 
          GetUserNameLink(action), 
        action.DateCreated.ToString("g", UserSession.LoginUser.CultureInfo), 
        GetAttachmentList(action),
        "",
        GetPropertyText(action),
        GetButtonHidden(action),
        GetButtonHidden(action)
        });
      }
      else if (action.SystemActionTypeID == SystemActionType.Custom)
      {
        string time = "";
        if (action.ActionTypeID != null)
        { 
          ActionType actionType = (ActionType)actionTypes.FindByActionTypeID((int)action.ActionTypeID);
          if (actionType != null && action.DateStarted != null && action.TimeSpent != null)
          {
            time = "<div class=\"actionTimeDiv\"><strong>Date Started:</strong> " + action.DateStarted.ToString() + "&nbsp&nbsp&nbsp <strong>TimeSpent:</strong> " + DataUtils.MinutesToDisplayTime((int)action.TimeSpent) + "</div><br />";
          }
        }
        string actionName = action.Row["ActionTypeName"] == DBNull.Value ? "[No Action Type]" : (string)action.Row["ActionTypeName"];
        table.Rows.Add(new object[] {
          action.ActionID, action.TicketID,
          actionName + ": "+action.Name, 
          GetDescriptionHTML(action), 
          GetUserNameLink(action), 
          action.DateCreated.ToString("g", UserSession.LoginUser.CultureInfo), 
          GetAttachmentList(action),
          time,
          GetPropertyText(action),
          GetButtonHidden(action),
          GetButtonHidden(action)});
      }
      else if (action.SystemActionTypeID == SystemActionType.Email)
      {
        table.Rows.Add(new object[] {
          action.ActionID, action.TicketID,
          "Email: "+action.Name, 
          DataUtils.StripHtml(GetDescriptionHTML(action)), 
          GetUserNameLink(action), 
          action.DateCreated.ToString("g", UserSession.LoginUser.CultureInfo), 
          GetAttachmentList(action),
          "",
          GetPropertyText(action),
          GetButtonHidden(action),
          GetButtonHidden(action)});

      }
    }

    rptActions.DataSource = table;
    rptActions.DataBind();
  }

  private string GetButtonHidden(TeamSupport.Data.Action action)
  {
    if (action.CreatorID == UserSession.CurrentUser.UserID || UserSession.CurrentUser.IsSystemAdmin)
      return "";
    else
      return "hiddenButton";
  }

  private string GetUserNameLink(TeamSupport.Data.Action action)
  {
    User user = Users.GetUser(UserSession.LoginUser, action.CreatorID);
    if (user == null) return "";
    return "<a href=\"../Default.aspx?UserID=" + action.CreatorID.ToString() + "\" target=\"TSMain\">" + user.FirstLastName +
      "</a>" + DataUtils.GetMailLink(UserSession.LoginUser, action.CreatorID, action.TicketID);
  }

  private string GetPropertyText(TeamSupport.Data.Action action)
  {
    return "<div class=\"actionPropertiesDiv\"><strong>Is Knowledge Base:</strong> " + action.IsKnowledgeBase.ToString() + "&nbsp&nbsp&nbsp <strong>Visible on Portal:</strong> " + action.IsVisibleOnPortal.ToString() + "</div><br />";
  }

  private string GetDescriptionHTML(TeamSupport.Data.Action action)
  {
    return DataUtils.StripHTMLTag(new string[] { "Script" }, action.Description);    
  }


  private string GetAttachmentList(TeamSupport.Data.Action action)
  {
    Attachments attachments = new Attachments(UserSession.LoginUser);
    attachments.LoadByActionID(action.ActionID);

    List<string> list = new List<string>();

    StringBuilder builder = new StringBuilder();
    for (int i = 0; i < attachments.Count; i++)
    {
      builder.Append(" &nbsp <a href=\"javascript:top.openAttachment('" + attachments[i].AttachmentID.ToString() + "');\">" + attachments[i].FileName + "</a>");
      if (i != attachments.Count - 1) builder.Append(",");
    }

    return attachments.Count < 1 ? "[None]" : builder.ToString();
  }





}
