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
using System.ServiceModel;
using System.Web.Services;
using System.IO;

public partial class Frames_Ticket : BaseFramePage
{
 
  public enum TicketRelation { Related, Child, Parent }

  [Serializable]
  public class RelatedTicket
  {
    public RelatedTicket(TicketsViewItem ticket, TicketRelation relation)
    {
      TicketID = ticket.TicketID;
      TicketNumber = ticket.TicketNumber;
      Name = ticket.Name;
      Relation = relation;
      IsClosed = ticket.IsClosed;
    }

    public int TicketID { get; set; }
    public int TicketNumber { get; set; }
    public string Name { get; set; }
    public TicketRelation Relation { get; set; }
    public bool IsClosed { get; set; }
  }
  
  private TicketGridViewItem _ticket;

  private CustomFieldControls _fieldControls;

  public int LastTicketTypeID
  {
    get
    {
      return Settings.Session.ReadInt("Ticket" + _ticket.TicketID.ToString() + "LastTicketType", -1);
    }
    set
    {
      Settings.Session.WriteInt("Ticket" + _ticket.TicketID.ToString() + "LastTicketType", value);
    }


  }

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);
    CachePage = true;

    try
    {
      if (Request["TicketNumber"] != null)
      {
        int ticketNumber = int.Parse(Request["TicketNumber"]);
        TicketGridView tickets = new TicketGridView(UserSession.LoginUser);
        tickets.LoadByTicketNumber(UserSession.LoginUser.OrganizationID, ticketNumber);
        _ticket = tickets[0];
      }
      else
      {
        _ticket = TicketGridView.GetTicketGridViewItem(UserSession.LoginUser, int.Parse(Request["TicketID"]));
      }

      Page.Title = "Team Support Ticket: " + _ticket.TicketNumber.ToString();

      if (_ticket.OrganizationID != UserSession.LoginUser.OrganizationID)
      {
        return;
      }

      fieldTicketID.Value = _ticket.TicketID.ToString();
      if (!IsPostBack)
      {
        LastTicketTypeID = _ticket.TicketTypeID;
      }
      spanUrl.InnerText = "https://app.teamsupport.com?TicketID=" + _ticket.TicketID.ToString(); 
      _fieldControls = new CustomFieldControls(ReferenceType.Tickets, LastTicketTypeID, _ticket.TicketID, 2, tblCustomControls, "PropertyChanged");
      _fieldControls.LoadCustomControls(200);
    }
    catch (Exception ex)
    {
      throw ex;
      return;
      ExceptionLogs.LogException(UserSession.LoginUser, ex, "Ticket.aspx");
      Response.Write("The ticket you have requested is not available.");
      Response.End();
      return;
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {

    //divTags.Visible = UserSession.LoginUser.OrganizationID == 1078 || UserSession.LoginUser.OrganizationID == 1088;

    if (_ticket == null || _ticket.OrganizationID != UserSession.LoginUser.OrganizationID)
    {
      return;
    }

    if (!IsPostBack)
    {
      spanPortal.Visible = UserSession.CurrentUser.HasPortalRights;
      cbPortal.Visible = UserSession.CurrentUser.HasPortalRights;

      if (UserSession.CurrentUser.ProductType == ProductType.Express || UserSession.CurrentUser.ProductType == ProductType.BugTracking)
      {
        divCustomers.Visible = false;
        divCustomers.Visible = false;
        ((RadToolBarButton)tbMain.FindItemByValue("AddOrganization")).Visible = false;
      }

      if (UserSession.CurrentUser.ProductType == ProductType.Express || UserSession.CurrentUser.ProductType == ProductType.HelpDesk)
      {
        cmbProduct.Visible = false;
        spanProduct.Visible = false;
        cmbReported.Visible = false;
        spanReported.Visible = false;
        cmbResolved.Visible = false;
        spanResolved.Visible = false;
      }

      _fieldControls.RefID = _ticket.TicketID;
      _fieldControls.LoadValues();
      if (Tickets.IsUserSubscribed(UserSession.LoginUser, UserSession.LoginUser.UserID, _ticket.TicketID)) tbMain.FindItemByValue("SubscribeMe").Text = "Unsubscribe Me";

      lblTicketNumber.Text = _ticket.TicketNumber.ToString();
      lblTicketName.Text = _ticket.Name;
      LoadUsers();
      LoadGroups();
      LoadTicketTypes();
      LoadSeverities();
      cmbTicketType.SelectedValue = _ticket.TicketTypeID.ToString();
      cmbUser.SelectedValue = _ticket.UserID.ToString();
      cmbGroup.SelectedValue = _ticket.GroupID.ToString();
      cmbSeverity.SelectedValue = _ticket.TicketSeverityID.ToString();
      cbKnowledgeBase.Checked = _ticket.IsKnowledgeBase;
      cbPortal.Checked = _ticket.IsVisibleOnPortal;

      cmbStatus.Items.Add(GetRadComboBoxItem(_ticket.Status, _ticket.TicketStatusID));
      cmbProduct.Items.Add(GetRadComboBoxItem(_ticket.ProductName, _ticket.ProductID));
      cmbReported.Items.Add(GetRadComboBoxItem(_ticket.ReportedVersion, _ticket.ReportedVersionID));
      cmbResolved.Items.Add(GetRadComboBoxItem(_ticket.SolvedVersion, _ticket.SolvedVersionID));

      divProperties.InnerHtml = GetPropertyHtml(_ticket.TicketID);
      divCustomerList.InnerHtml = GetCustomerText(_ticket.TicketID);
      divActions.InnerHtml = GetActionsHtml(_ticket.TicketID);
    }




    if (UserSession.CurrentUser.IsSystemAdmin)
    {
      ((RadToolBarButton)tbMain.FindItemByValue("Delete")).Visible = true;
    }

  }

  private RadComboBoxItem GetRadComboBoxItem(string name, int? id)
  {
    if (id == null)
    {
      return new RadComboBoxItem("Unassigned", "-1");
    }
    return new RadComboBoxItem(name, id.ToString());
  }

  protected override void OnPreRender(EventArgs e)
  {
    ScriptManager.RegisterStartupScript(this, this.GetType(), "CustomControlsScript", _fieldControls.Script, true);
    base.OnPreRender(e);
  }

  private void LoadTicketTypes()
  {
    TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
    ticketTypes.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, UserSession.CurrentUser.ProductType);
    cmbTicketType.DataTextField = "Name";
    cmbTicketType.DataValueField = "TicketTypeID";
    cmbTicketType.DataSource = ticketTypes.Table;
    cmbTicketType.DataBind();
    cmbTicketType.SelectedIndex = 0;
  }

  private void LoadSeverities()
  {
    TicketSeverities ticketSeverities = new TicketSeverities(UserSession.LoginUser);
    ticketSeverities.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
    cmbSeverity.DataTextField = "Name";
    cmbSeverity.DataValueField = "TicketSeverityID";
    cmbSeverity.DataSource = ticketSeverities.Table;
    cmbSeverity.DataBind();
    cmbSeverity.SelectedIndex = 0;

  }

  private void LoadUsers()
  {

    Users users = new Users(UserSession.LoginUser);
    users.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, true);
    cmbUser.Items.Clear();
    cmbUser.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
    foreach (User user in users)
    {
      cmbUser.Items.Add(new RadComboBoxItem(user.LastName + ", " + user.FirstName, user.UserID.ToString()));
    }
    cmbUser.SelectedIndex = 0;

  }

  private void LoadGroups()
  {
    Groups groups = new Groups(UserSession.LoginUser);
    groups.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
    cmbGroup.Items.Clear();
    cmbGroup.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
    foreach (Group group in groups)
    {
      cmbGroup.Items.Add(new RadComboBoxItem(group.Name, group.GroupID.ToString()));
    }
    cmbGroup.SelectedIndex = 0;

  }

  protected void cmbTicketType_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
  {
    _fieldControls.ClearCustomControls();

    int ticketTypeID = int.Parse(cmbTicketType.SelectedValue);
    TransferCustomFields(LastTicketTypeID, ticketTypeID);
    LastTicketTypeID = ticketTypeID;
    _fieldControls.AuxID = ticketTypeID;
    _fieldControls.LoadCustomControls(200);
    _fieldControls.LoadValues();
  }

  protected void btnCancel_OnClick(object o, EventArgs e)
  {

    cmbTicketType.SelectedValue = _ticket.TicketTypeID.ToString();

    _fieldControls.ClearCustomControls();

    LastTicketTypeID = _ticket.TicketTypeID;
    _fieldControls.AuxID = _ticket.TicketTypeID;
    _fieldControls.LoadCustomControls(200);
    _fieldControls.LoadValues();
  }

  [WebMethod(true)]
  public static TicketProxy GetTicket(int ticketID)
  {
    Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, ticketID);
    return ticket.GetProxy();
  }

  [WebMethod(true)]
  public static void SaveTicket(TicketProxy proxy)
  {
    Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, proxy.TicketID);

    if (ticket == null || ticket.OrganizationID != UserSession.LoginUser.OrganizationID) return;

    ticket.TicketTypeID = proxy.TicketTypeID;
    ticket.TicketStatusID = proxy.TicketStatusID;
    ticket.TicketSeverityID = proxy.TicketSeverityID;
    ticket.UserID = proxy.UserID < 0 ? null : proxy.UserID;
    ticket.GroupID = proxy.GroupID < 0 ? null : proxy.GroupID;
    ticket.ProductID = proxy.ProductID < 0 ? null : proxy.ProductID;
    ticket.ReportedVersionID = proxy.ReportedVersionID < 0 ? null : proxy.ReportedVersionID;
    ticket.SolvedVersionID = proxy.SolvedVersionID < 0 ? null : proxy.SolvedVersionID;
    ticket.IsKnowledgeBase = proxy.IsKnowledgeBase;
    ticket.IsVisibleOnPortal = proxy.IsVisibleOnPortal;

    ticket.Collection.Save();

  }

  [WebMethod(true)]
  public static void EmailTicket(int ticketID, string addresses)
  {
    EmailPosts posts = new EmailPosts(UserSession.LoginUser);
    EmailPost post = posts.AddNewEmailPost();
    post.EmailPostType = EmailPostType.TicketSendEmail;
    post.HoldTime = 0;

    post.Param1 = UserSession.LoginUser.UserID.ToString();
    post.Param2 = ticketID.ToString();
    post.Param3 = addresses;
    posts.Save();
  }

  [WebMethod(true)]
  public static string GetCustomerText(int ticketID)
  {
    Organizations organizations = new Organizations(UserSession.LoginUser);
    organizations.LoadByNotContactTicketID(ticketID);
    UsersView users = new UsersView(UserSession.LoginUser);
    users.LoadByTicketID(ticketID);

    if (users.IsEmpty && organizations.IsEmpty)
    {
      return "There are no customers or contacts associated with this ticket.";
    }

    StringBuilder builder = new StringBuilder();

    foreach (Organization organization in organizations)
    {

      if (builder.Length > 0) builder.Append(", ");
      builder.Append(String.Format("&nbsp&nbsp<a class=\"customerLink ts-link\" title=\"Go to {0}\" href=\"#\" onclick=\"top.Ts.MainPage.openCustomer({1}); return false;\">{0}</a>", organization.Name, organization.OrganizationID.ToString()));
      builder.Append(String.Format("&nbsp<sup>(<a class=\"removeLink\" title=\"Remove association with {0}\" href=\"javascript:DeleteCustomer({1});\">x</a>)</sup>", organization.Name, organization.OrganizationID.ToString()));
    }

    foreach (UsersViewItem user in users)
    {
      if (builder.Length > 0) builder.Append(", ");
      builder.Append(String.Format("&nbsp&nbsp<a class=\"customerLink ts-link\" title=\"Go to {0}\" href=\"#\" onclick=\"top.Ts.MainPage.openContact({1}, {2}); return false;\">{0}</a>", user.Organization + " [" + user.LastName + ", " + user.FirstName + "]", user.UserID.ToString(), user.OrganizationID));
      builder.Append(String.Format("&nbsp<sup>(<a class=\"removeLink\" title=\"Remove association with {0}\" href=\"javascript:DeleteContact({1});\">x</a>)</sup>", user.Organization + " [" + user.LastName + ", " + user.FirstName + "]", user.UserID.ToString()));
    }

    return builder.ToString();

  }



  [WebMethod(true)]
  public static void DeleteTicket(int ticketID)
  {
    Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, ticketID);
    if (ticket.OrganizationID == UserSession.LoginUser.OrganizationID)
    {
      ticket.Delete();
      ticket.Collection.Save();
    }
  }

  [WebMethod(true)]
  public static RadComboBoxItemData[] GetStatuses(int ticketTypeID, int? ticketStatusID)
  {

    List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();

    if (ticketStatusID != null)
    {
      TicketNextStatuses ticketNextStatuses = new TicketNextStatuses(UserSession.LoginUser);
      ticketNextStatuses.LoadNextStatuses((int)ticketStatusID);

      if (!ticketNextStatuses.IsEmpty)
      {
        TicketStatus status = TicketStatuses.GetTicketStatus(UserSession.LoginUser, (int)ticketStatusID);

        if (status != null)
        {
          RadComboBoxItemData item = new RadComboBoxItemData();
          item.Text = status.Name;
          item.Value = status.TicketStatusID.ToString();
          list.Add(item);
        }

        foreach (TicketNextStatus ticketNextStatus in ticketNextStatuses)
        {
          RadComboBoxItemData itemData = new RadComboBoxItemData();
          itemData.Text = ticketNextStatus.Name;
          itemData.Value = ticketNextStatus.NextStatusID.ToString();
          list.Add(itemData);
        }
        return list.ToArray();
      }
    }

    TicketStatuses ticketStatuses = new TicketStatuses(UserSession.LoginUser);
    ticketStatuses.LoadByTicketTypeID(ticketTypeID);

    foreach (TicketStatus ticketStatus in ticketStatuses)
    {
      RadComboBoxItemData itemData = new RadComboBoxItemData();
      itemData.Text = ticketStatus.Name;
      itemData.Value = ticketStatus.TicketStatusID.ToString();
      list.Add(itemData);
    }


    return list.ToArray();
  }

  [WebMethod(true)]
  public static RadComboBoxItemData[] GetVersions(int? productID, int ticketID)
  {
    List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();

    RadComboBoxItemData unassignedData = new RadComboBoxItemData();
    unassignedData.Text = "Unassigned";
    unassignedData.Value = "-1";
    list.Add(unassignedData);

    if (productID == null)
    {
      return list.ToArray();
    }

    ProductVersions productVersions = new ProductVersions(UserSession.LoginUser);

    if (Settings.OrganizationDB.ReadBool("ShowOnlyCustomerProducts", false))
    {
      productVersions.LoadByProductAndTicket((int)productID, ticketID);
      if (productVersions.Count < 2) productVersions.LoadByProductID((int)productID);
    }
    else
    {
      productVersions.LoadByProductID((int)productID);
    }

    foreach (ProductVersion version in productVersions)
    {
      RadComboBoxItemData itemData = new RadComboBoxItemData();
      itemData.Text = version.VersionNumber;
      itemData.Value = version.ProductVersionID.ToString();
      list.Add(itemData);
    }
    return list.ToArray();
  }

  [WebMethod(true)]
  public static RadComboBoxItemData[] GetProducts(int ticketID)
  {
    Products products = new Products(UserSession.LoginUser);
    if (Settings.OrganizationDB.ReadBool("ShowOnlyCustomerProducts", false))
    {
      products.LoadByTicketID(ticketID);
      if (products.Count < 2) products.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
    }
    else
    {
      products.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
    }

    List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();

    RadComboBoxItemData unassignedData = new RadComboBoxItemData();
    unassignedData.Text = "Unassigned";
    unassignedData.Value = "-1";
    list.Add(unassignedData);
    foreach (Product product in products)
    {
      RadComboBoxItemData itemData = new RadComboBoxItemData();
      itemData.Text = product.Name;
      itemData.Value = product.ProductID.ToString();
      list.Add(itemData);
    }
    return list.ToArray();
  }

  [WebMethod(true)]
  public static string ToggleKnowledge(int actionID)
  {
    TeamSupport.Data.Action action = Actions.GetAction(UserSession.LoginUser, actionID);
    action.IsKnowledgeBase = !action.IsKnowledgeBase;
    action.Collection.Save();
    return GetActionsHtml(action.TicketID);
  }

  [WebMethod(true)]
  public static string TogglePortal(int actionID)
  {
    TeamSupport.Data.Action action = Actions.GetAction(UserSession.LoginUser, actionID);
    action.IsVisibleOnPortal = !action.IsVisibleOnPortal;
    action.Collection.Save();
    return GetActionsHtml(action.TicketID);
  }

  [WebMethod(true)]
  public static string[] GetSLATips(int ticketID)
  {
    string tip =
  @"<table cellspacing=""0"" cellpadding=""5"" border=""0"">
      <tr><td>Initial Response: </td><td>{0}</td></tr>
      <tr><td>Last Action: </td><td>{1}</td></tr>
      <tr><td>Time to Close: </td><td>{2}</td></tr></table>";

    TicketSlaViewItem sla = TicketSlaView.GetTicketSlaViewItem(UserSession.LoginUser, ticketID);
    List<string> result = new List<string>();
    if (sla != null)
    {

      result.Add(string.Format(tip,
        sla.ViolationInitialResponse == null ? "[None]" : DataUtils.MinutesToDisplayTime((int)sla.ViolationInitialResponse, "0"),
        sla.ViolationLastAction == null ? "[None]" : DataUtils.MinutesToDisplayTime((int)sla.ViolationLastAction, "0"),
        sla.ViolationTimeClosed == null ? "[None]" : DataUtils.MinutesToDisplayTime((int)sla.ViolationTimeClosed, "0")
        ));
      result.Add(string.Format(tip,
        sla.WarningInitialResponse == null ? "[None]" : DataUtils.MinutesToDisplayTime((int)sla.WarningInitialResponse, "0"),
        sla.WarningLastAction == null ? "[None]" : DataUtils.MinutesToDisplayTime((int)sla.WarningLastAction, "0"),
        sla.WarningTimeClosed == null ? "[None]" : DataUtils.MinutesToDisplayTime((int)sla.WarningTimeClosed, "0")
        ));
    }
    return result.ToArray();
  }


  [WebMethod(true)]
  public static string GetPropertyHtml(int ticketID)
  {
    TicketGridViewItem ticket = TicketGridView.GetTicketGridViewItem(UserSession.LoginUser, ticketID);
    StringBuilder builder = new StringBuilder();

    builder.Append(@"<table width=""775px"" cellpadding="""" cellspacing=""5"" border=""0""><tr>");
    builder.Append("<td class=\"labelColTD\">SLA Violation Time:</td><td class=\"inputColTD\"><span class=\"");
    if (ticket.SlaViolationTime != null) builder.Append("spanSla ");
    builder.Append(ticket.SlaViolationTime < 1 ? "spanViolation\"" : "\"");
    if (ticket.SlaViolationTime != null) builder.Append(" id=\"spanSlaViolation\"");
    builder.Append(">");
    builder.Append(ticket.SlaViolationTime == null ? "[None]" : DataUtils.MinutesToDisplayTime((int)ticket.SlaViolationTime, "0"));
    builder.Append("</span>");
    if (ticket.SlaViolationTime != null)
      builder.Append("<span class=\"infoImg\" onmouseover=\"ShowSLATip(true);\"><img id=\"imgViolationInfo\" src=\"../images/icons/information.png\" alt=\"info\"/></span>");
    builder.Append("</td>");

    builder.Append("<td class=\"labelColTD\">SLA Warning Time:</td><td class=\"inputColTD\"><span class=\"");
    if (ticket.SlaWarningTime != null) builder.Append("spanSla ");
    builder.Append(ticket.SlaWarningTime < 1 ? "spanWarning\"" : "\"");
    if (ticket.SlaWarningTime != null) builder.Append(" id=\"spanSlaWarning\"");
    builder.Append(">");
    builder.Append(ticket.SlaWarningTime == null ? "[None]" : (ticket.SlaViolationTime < 1 ? "Violated" : DataUtils.MinutesToDisplayTime((int)ticket.SlaWarningTime, "0")));
    builder.Append("</span>");
    if (ticket.SlaViolationTime != null)
      builder.Append("<span class=\"infoImg\" onmouseover=\"ShowSLATip(false);\"><img id=\"imgWarningInfo\" src=\"../images/icons/information.png\" alt=\"info\"/></span>");
    builder.Append("</td>");

    builder.Append("<tr>");
    builder.Append("<td class=\"labelColTD\">Opened By:</td><td class=\"inputColTD\"> <a href=\"javascript:top.Ts.MainPage.openUser(" + ticket.CreatorID + ");\" target=\"TSMain\">");
    builder.Append(ticket.CreatorName);
    builder.Append("</a>" + DataUtils.GetMailLink(UserSession.LoginUser, ticket.CreatorID, ticket.TicketID) + "</td>");
    builder.Append("<td class=\"labelColTD\">Opened On:</td><td class=\"inputColTD\"><div style=\"width:100%;\">");
    builder.Append(ticket.DateCreated.ToString("g", UserSession.LoginUser.CultureInfo));
    builder.Append("</div></td>");
    builder.Append("</tr>");
    builder.Append("<tr>");
    builder.Append("<td class=\"labelColTD\">Last Modified By:</td><td class=\"inputColTD\"> <a href=\"javascript:top.Ts.MainPage.openUser(" + ticket.ModifierID + ");\" target=\"TSMain\">");
    builder.Append(ticket.ModifierName);
    builder.Append("</a>" + DataUtils.GetMailLink(UserSession.LoginUser, ticket.ModifierID, ticket.TicketID) + "</td>");
    builder.Append("<td class=\"labelColTD\">Last Modified On:</td><td class=\"inputColTD\"><div style=\"width:100%;\">");
    builder.Append(ticket.DateModified.ToString("g", UserSession.LoginUser.CultureInfo));
    builder.Append("</div></td>");
    builder.Append("</tr>");

    builder.Append("<tr>");
    if (ticket.IsClosed)
    {
      builder.Append("<td class=\"labelColTD\">Days Closed:</td><td class=\"inputColTD\"><div style=\"width:100%;\">");
      builder.Append(ticket.DaysClosed.ToString());
      builder.Append("</div></td>");
    }
    else
    {
      builder.Append("<td class=\"labelColTD\">Days Opened:</td><td class=\"inputColTD\"><div style=\"width:100%;\">");
      builder.Append((ticket.DaysOpened < 1 ? 0 : ticket.DaysOpened).ToString());
      builder.Append("</div></td>");
    }

    builder.Append("<td class=\"labelColTD\">Total Time Spent:</td><td class=\"inputColTD\"><div style=\"width:100%;\">");
    builder.Append(DataUtils.MinutesToDisplayTime(Tickets.GetTicketActionTime(UserSession.LoginUser, ticket.TicketID)));
    builder.Append("</div></td>");

    builder.Append("</tr>");

    if (ticket.IsClosed)
    {
      builder.Append("<tr>");
      if (ticket.CloserID != null)
      {
        builder.Append("<td class=\"labelColTD\">Closed By:</td><td class=\"inputColTD\"> <a href=\"javascript:top.Ts.MainPage.openUser(" + ticket.CloserID + ");\" target=\"TSMain\">");
        builder.Append(ticket.CloserName);
        builder.Append("</a>" + DataUtils.GetMailLink(UserSession.LoginUser, (int)ticket.CloserID, ticket.TicketID) + "</td>");
      }

      builder.Append("<td class=\"labelColTD\">Closed On:</td><td class=\"inputColTD\"><div style=\"width:100%;\">");
      if (ticket.DateClosed != null) builder.Append(((DateTime)ticket.DateClosed).ToString("g", UserSession.LoginUser.CultureInfo));
      builder.Append("</div></td>");
      builder.Append("</tr>");

    }



    builder.Append("</table>");
    return builder.ToString();
  }

  [WebMethod(true)]
  public static string GetTicketEmailLink(int ticketID, int userID)
  {
    return DataUtils.GetMailLinkHRef(UserSession.LoginUser, userID, ticketID);
  }


  [WebMethod(true)]
  public static string DeleteAction(int actionID)
  {
    TeamSupport.Data.Action action = Actions.GetAction(UserSession.LoginUser, actionID);
    Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, action.TicketID);
    if (ticket.OrganizationID == UserSession.LoginUser.OrganizationID)
    {
      action.Delete();
      action.Collection.Save();
    }
    return GetActionsHtml(ticket.TicketID);
  }

  [WebMethod(true)]
  public static string DeleteAttachment(int attachmentID)
  {

    TeamSupport.Data.Attachment attachment = Attachments.GetAttachment(UserSession.LoginUser, attachmentID);
    if (attachment.RefType == ReferenceType.Actions)
    {
      TeamSupport.Data.Action action = Actions.GetAction(UserSession.LoginUser, attachment.RefID);
      Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, action.TicketID);
      if (ticket.OrganizationID == UserSession.LoginUser.OrganizationID)
      {
        Attachments.DeleteAttachmentAndFile(UserSession.LoginUser, attachmentID);
        return GetActionsHtml(ticket.TicketID);
      }
    }
    return "";
  }

  [WebMethod(true)]
  public static void UpdateTicketName(int ticketID, string name)
  {
    Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, ticketID);
    if (ticket.OrganizationID == UserSession.LoginUser.OrganizationID)
    {
      name = name.Trim();
      if (name != "")
      {
        ticket.Name = name;
        ticket.Collection.Save();
      }
    }
  }

  private static string GetActionHtml(TeamSupport.Data.Action action, ActionType actionType)
  {
    bool canEdit = action.CreatorID == UserSession.CurrentUser.UserID || UserSession.CurrentUser.IsSystemAdmin || action.SystemActionTypeID == SystemActionType.Description;
    bool canDelete = action.SystemActionTypeID != SystemActionType.Description && canEdit;

    Attachments attachments = new Attachments(UserSession.LoginUser);
    attachments.LoadByActionID(action.ActionID);
    User user = Users.GetUser(UserSession.LoginUser, action.CreatorID);


    StringBuilder builder = new StringBuilder();
    builder.Append(@"<br />
      <div class=""groupBarDiv groupBarBlue"" style=""width: 98%; margin: 0 auto;"">
        <div class=""groupBarHeaderDiv"">
          <span class=""groupBarHeaderSpan""></span>
          <div class=""groupBarImages"">");

    if (canEdit)
    {
      if (action.IsKnowledgeBase)
        builder.Append(@"<img alt=""""  title=""This action is visible in the Knowledge Base"" src=""../images/icons/knowledge.png"" style=""cursor:pointer;"" class=""groupBarImage"" onclick=""ToggleKnowledge({1});"" />");
      else
        builder.Append(@"<img alt=""""  title=""This action is not visible in the Knowledge Base"" src=""../images/icons/knowledged.png"" style=""cursor:pointer;"" class=""groupBarImage"" onclick=""ToggleKnowledge({1});"" />");
      if (action.IsVisibleOnPortal)
        builder.Append(@"<img alt="""" title=""This action is visible to customers"" src=""../images/icons/portal.png"" style=""cursor:pointer;"" class=""groupBarImage"" onclick=""TogglePortal({1});"" />");
      else
        builder.Append(@"<img alt="""" title=""This action is not visible to customers"" src=""../images/icons/portald.png"" style=""cursor:pointer;"" class=""groupBarImage""  onclick=""TogglePortal({1});""/>");

    }
    else
    {
      if (action.IsKnowledgeBase)
        builder.Append(@"<img alt="""" title=""This action is visible in the Knowledge Base"" src=""../images/icons/knowledge.png"" class=""groupBarImage"" />");
      else
        builder.Append(@"<img alt="""" title=""This action is not visible in the Knowledge Base"" src=""../images/icons/knowledged.png"" class=""groupBarImage"" />");
      if (action.IsVisibleOnPortal)
        builder.Append(@"<img alt="""" title=""This action is visible to customers"" src=""../images/icons/portal.png"" class=""groupBarImage"" />");
      else
        builder.Append(@"<img alt="""" title=""This action is not visible to customers"" src=""../images/icons/portald.png"" class=""groupBarImage"" />");

    }




    builder.Append(@"</div><div class=""groupBarCaptionSpan"">{0}</div>");


    if (canEdit || canDelete)
    {
      builder.Append(@"<div class=""groupBarButtonSpanWrapper"">");
      if (canEdit)
      {
        builder.Append(@"<span class=""groupBarButtonsSpan"">
              <a class=""groupBarButtonLink"" href=""#"" onclick=""ShowAction({1});"">
                <span class=""groupBarButtonSpan"">
                  <img alt="""" src=""../images/icons/edit.png"" class=""groupBarButtonImage"" />
                  <span class=""groupBarButtonTextSpan"">Edit</span>
                </span>
              </a>
            </span>");

      }

      if (canDelete)
      {
        builder.Append(@"<span class=""groupBarButtonsSpan"">
              <a class=""groupBarButtonLink"" href=""#"" onclick=""DeleteAction({1});"">
                <span class=""groupBarButtonSpan"">
                  <img alt="""" src=""../images/icons/trash.png"" class=""groupBarButtonImage"" />
                  <span class=""groupBarButtonTextSpan"">Delete</span>
                </span>
              </a>
            </span>");

      }
      builder.Append(@"</div>");


    }

    builder.Append(@"
        </div>
      </div>
      <br />
      <div class=""actionBodyDiv"">
        <html>
        <body>
          <div>{2}</div>
      </div>
      </body></html>");

    if (!attachments.IsEmpty)
    {
      builder.Append(@"
      <br />
      <div class=""actionAttachmentsDiv"">
        <strong>Attachments:</strong> &nbsp");


      string attachmentString;

      if (canEdit) attachmentString = " &nbsp <a href=\"../dc/{0}/attachments/{1}/\" target=\"_blank\">{2}</a> &nbsp<sup>(<a class=\"removeLink\" title=\"Delete attachment {2}\" href=\"javascript:DeleteAttachment({1});\">x</a>)</sup>";
      else attachmentString = " &nbsp <a href=\"../dc/{0}/attachments/{1}/{2}\"  target=\"_blank\">{2}</a>";
      for (int i = 0; i < attachments.Count; i++)
      {
        builder.Append(DataUtils.FixStringFormat(String.Format(attachmentString, UserSession.LoginUser.OrganizationID.ToString(), attachments[i].AttachmentID.ToString(), attachments[i].FileName)));
        if (i != attachments.Count - 1) builder.Append(",");
      }


      builder.Append("</div>");

    }
    builder.Append(@"
      <br />
      {3}
      <div class=""actionFooterDiv"">
        -&nbsp<span>{4}</span>
        &nbsp&nbsp&nbsp<span>{5}</span>
      </div>");

    /*
     * 0:title
     * 1:actionid
     * 2:desc
     * 3:time
     * 4:user
     * 5:date
     */

    string title = action.Row["ActionTypeName"] == DBNull.Value ? "[No Action Type]" : (string)action.Row["ActionTypeName"] + ": " + action.Name;
    if (action.SystemActionTypeID == SystemActionType.Description) title = "Description";
    else if (action.SystemActionTypeID == SystemActionType.Resolution) title = "Resolution";
    else if (action.SystemActionTypeID == SystemActionType.Chat) title = "Chat";
    else if (action.SystemActionTypeID == SystemActionType.Email) title = "Email: " + action.Name;

    string time = "";
    if (actionType != null && action.DateStarted != null && action.TimeSpent != null)
    {
      time = "<div class=\"actionTimeDiv\"><strong>Date Started:</strong> " + action.DateStarted.ToString() + "&nbsp&nbsp&nbsp <strong>TimeSpent:</strong> " + DataUtils.MinutesToDisplayTime((int)action.TimeSpent) + "</div><br />";
    }

    string desc = TagActionDescription(HtmlCleaner.CleanHtml(action.Description)); 
    //string desc = TagActionDescription(HtmlUtility.RemoveInvalidHtmlTags(action.Description)); 
    //string desc = TagActionDescription(DataUtils.SanitizeHtml(action.Description,true)); 
    string userName = user == null ? "" : "<a href=\"javascript:top.Ts.MainPage.openUser(" + action.CreatorID.ToString() + ");\" target=\"TSMain\">" + user.FirstLastName + "</a>" + DataUtils.GetMailLink(UserSession.LoginUser, action.CreatorID, action.TicketID);
    string date = action.DateCreated.ToString("g", UserSession.LoginUser.CultureInfo);
    string result = "";
    try
    {
      result = String.Format(builder.ToString(), title, action.ActionID.ToString(), desc, time, userName, date);
    }
    catch (Exception ex)
    {
      ex.Data["String"] = builder.ToString();
      ex.Data["Title"] = title;
      ex.Data["Desc"] = desc;
      ex.Data["Time"] = time;
      ex.Data["UserName"] = userName;
      ex.Data["Date"] = date;
      ExceptionLogs.LogException(UserSession.LoginUser, ex, "");
      
    }

    return result;


  }

  public static string TagActionDescription(string description)
  {
    Tags tags = new Tags(UserSession.LoginUser);
    tags.LoadByOrganization(UserSession.LoginUser.OrganizationID);

    foreach (Tag tag in tags)
    {
      description = DataUtils.CreateLinks(description, tag.Value, "#", "ts-link tag-link tagid-" + tag.TagID.ToString(), "_blank");
    }
    return description;
  }

  [WebMethod(true)]
  public static string GetActionsHtml(int ticketID)
  {
    Actions actions = new Actions(UserSession.LoginUser);
    actions.LoadByTicketID(ticketID);
    if (actions.Count < 1) return "[No actions to display]";
    ActionTypes actionTypes = new ActionTypes(UserSession.LoginUser);
    actionTypes.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);


    StringBuilder builder = new StringBuilder();
    foreach (TeamSupport.Data.Action action in actions)
    {
      ActionType actionType = action.ActionTypeID != null ? actionTypes.FindByActionTypeID((int)action.ActionTypeID) : null;
      builder.Append(GetActionHtml(action, actionType));
    }

    return builder.ToString();
  }

  [WebMethod(true)]
  public static bool ToggleSubscription(int ticketID)
  {
    bool result = Tickets.IsUserSubscribed(UserSession.LoginUser, UserSession.LoginUser.UserID, ticketID);
    Tickets tickets = new Tickets(UserSession.LoginUser);
    if (result) tickets.RemoveSubscription(UserSession.LoginUser.UserID, ticketID);
    else tickets.AddSubscription(UserSession.LoginUser.UserID, ticketID);
    return !result;
  }

  [WebMethod(true)]
  public static string SubscribeUser(int ticketID, int userID)
  {
    Tickets tickets = new Tickets(UserSession.LoginUser);
    tickets.LoadByTicketID(ticketID);
    Ticket ticket = tickets[0];
    if (ticket.OrganizationID != UserSession.LoginUser.OrganizationID) return null;
    User user = Users.GetUser(UserSession.LoginUser, userID);
    if (user.OrganizationID != UserSession.LoginUser.OrganizationID) return null;

    tickets.AddSubscription(userID, ticketID);
    return user.FirstLastName;
  }

  [WebMethod(true)]
  public static int TakeOwnership(int ticketID)
  {
    Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, ticketID);
    ticket.UserID = UserSession.LoginUser.UserID;
    ticket.Collection.Save();
    return UserSession.LoginUser.UserID;
  }

  [WebMethod(true)]
  public static void Enqueue(int ticketID)
  {
    TicketQueue.Enqueue(UserSession.LoginUser, ticketID, UserSession.LoginUser.UserID);
  }

  [WebMethod(true)]
  public static RelatedTicket[] GetRelatedTickets(int ticketID)
  {
    List<RelatedTicket> result = new List<RelatedTicket>();

    Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, ticketID);

    if (ticket.ParentID != null)
    { 
      TicketsViewItem parent = TicketsView.GetTicketsViewItem(UserSession.LoginUser, (int)ticket.ParentID);
      result.Add(new RelatedTicket(parent, TicketRelation.Parent));
    }

    TicketsView children = new TicketsView(UserSession.LoginUser);
    children.LoadChildren(ticketID);
    foreach (TicketsViewItem item in children)
    {
      result.Add(new RelatedTicket(item, TicketRelation.Child));
    }

    TicketsView related = new TicketsView(UserSession.LoginUser);
    related.LoadRelated(ticketID);
    foreach (TicketsViewItem item in related)
    {
      result.Add(new RelatedTicket(item, TicketRelation.Related));
    }

    return result.ToArray();
  }

  [WebMethod(true)]
  public static TagProxy[] GetTags(int ticketID)
  {
    Tags tags = new Tags(UserSession.LoginUser);
    tags.LoadByReference(ReferenceType.Tickets, ticketID);
    return tags.GetTagProxies();
  }

  [WebMethod(true)]
  public static void DeleteTicketTag(int tagID, int ticketID)
  {
    TagLink link = TagLinks.GetTagLink(UserSession.LoginUser, ReferenceType.Tickets, ticketID, tagID);
    Tag tag = Tags.GetTag(UserSession.LoginUser, tagID);
    int count = tag.GetLinkCount();
    link.Delete();
    link.Collection.Save();
    if (count < 2)
    {
      tag.Delete();
      tag.Collection.Save();
    }
  }


  private void TransferCustomFields(int oldTicketTypeID, int newTicketTypeID)
  {
    CustomFields oldFields = new CustomFields(UserSession.LoginUser);
    oldFields.LoadByTicketTypeID(UserSession.LoginUser.OrganizationID, oldTicketTypeID);

    CustomFields newFields = new CustomFields(UserSession.LoginUser);
    newFields.LoadByTicketTypeID(UserSession.LoginUser.OrganizationID, newTicketTypeID);


    foreach (CustomField oldField in oldFields)
    {
      CustomField newField = newFields.FindByName(oldField.Name);
      if (newField != null)
      { 
        CustomValue newValue = CustomValues.GetValue(UserSession.LoginUser, newField.CustomFieldID, _ticket.TicketID);
        CustomValue oldValue = CustomValues.GetValue(UserSession.LoginUser, oldField.CustomFieldID, _ticket.TicketID);

        if (newValue != null && oldValue != null)
        {
          newValue.Value = oldValue.Value;
          newValue.Collection.Save();
        }
      }

    }
  
  }
  

}
