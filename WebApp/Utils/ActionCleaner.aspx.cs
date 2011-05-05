using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Web.Services;
using System.Runtime.Serialization;

public partial class Utils_ActionCleaner : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (TSAuthentication.OrganizationID != 1078)
    {
      Response.StatusCode = 404;
      Response.End();
      return;
    }
  }


  [WebMethod(true)]
  public static TicketProxy GetTicketByNumber(int orgID, int number)
  {
    if (UserSession.LoginUser.OrganizationID != 1078) return null;
    Ticket ticket = Tickets.GetTicketByNumber(UserSession.LoginUser, orgID, number);
    if (ticket == null) return null;
    return ticket.GetProxy();
  }

  [WebMethod(true)]
  public static TicketProxy GetTicketByID(int ticketID)
  {
    if (UserSession.LoginUser.OrganizationID != 1078) return null;
    Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, ticketID);
    if (ticket == null) return null;
    return ticket.GetProxy();
  }

  [WebMethod(true)]
  public static ActionsViewItemProxy[] GetActions(int ticketID)
  {
    if (UserSession.LoginUser.OrganizationID != 1078) return null;
    ActionsView actions = new ActionsView(UserSession.LoginUser);
    actions.LoadByTicketID(ticketID);
    return actions.GetActionsViewItemProxies();
  }

  [WebMethod(true)]
  public static void CleanAction(int actionID)
  {
    if (UserSession.LoginUser.OrganizationID != 1078) return;
    TeamSupport.Data.Action action = Actions.GetAction(UserSession.LoginUser, actionID);
    action.Description = HtmlUtility.RemoveInvalidHtmlTags(action.Description);
    action.Collection.Save();
  }

  [WebMethod(true)]
  public static void CleanAllActions(int ticketID)
  {
    if (UserSession.LoginUser.OrganizationID != 1078) return;
    Actions actions = new Actions(UserSession.LoginUser);
    actions.LoadByTicketID(ticketID);
    foreach (TeamSupport.Data.Action action in actions)
    {
      action.Description = HtmlUtility.RemoveInvalidHtmlTags(action.Description);
    }
    actions.Save();
  }

  [WebMethod(true)]
  public static void CleanAllEmailActions(int ticketID)
  {
    if (UserSession.LoginUser.OrganizationID != 1078) return;
    Actions actions = new Actions(UserSession.LoginUser);
    actions.LoadByTicketID(ticketID);
    foreach (TeamSupport.Data.Action action in actions)
    {
      if (action.SystemActionTypeID == SystemActionType.Email)
        action.Description = HtmlUtility.RemoveInvalidHtmlTags(action.Description);
    }
    actions.Save();
  }

  [WebMethod(true)]
  public static AutocompleteItem[] GetOrganizations(string name)
  {
    List<AutocompleteItem> result = new List<AutocompleteItem>();
    Organizations organizations = new Organizations(UserSession.LoginUser);
    organizations.LoadByLikeOrganizationName(1, name, false, 20);
    foreach (Organization organization in organizations)
    {
      result.Add(new AutocompleteItem(organization.Name, organization.OrganizationID.ToString()));
    }

    return result.ToArray();
  }




  [DataContract]
  public class AutocompleteItem
  {
    public AutocompleteItem() { }

    public AutocompleteItem(string label, string id)
    {
      this.label = label;
      this.value = label;
      this.id = id;
    }

    public AutocompleteItem(string label, string value, string id)
    {
      this.label = label;
      this.value = value;
      this.id = id;
    }

    [DataMember]
    public string label { get; set; }
    [DataMember]
    public string value { get; set; }
    [DataMember]
    public string id { get; set; }
  }
}