using System;
using System.Collections;
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
using System.IO;
using System.Web.Services;
using System.Runtime.Serialization;

public partial class Frames_NewTicket : BaseFramePage
{
  enum ComboBoxIDTypes
  {
    Customer,
    Status,
    Severity,
    User,
    Group,
    Product,
    Version
  }

  private int _chatID = -1;
  private string _menuID = "";
  public int LastTicketTypeID 
  {
    get
    {
      if (Session["LastNewTicketTicketTypeID"] != null)
        return (int)Session["LastNewTicketTicketTypeID"];
      else 
        return -1;
    }
    set { Session["LastNewTicketTicketTypeID"] = value; }
  }

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);


    try
    {
      _chatID = Request["ChatID"] != null ? int.Parse(Request["ChatID"]) : -1;
    }
    catch (Exception)
    {
      _chatID = -1;
    }
    

    if (!IsPostBack)
    {
      LastTicketTypeID = -1;
      _menuID = Settings.UserDB.ReadString("main-menu-selected", "");
      try
      {
        if (_menuID.Length > 13)
        {
          //0123456789012
          //mniTicketTypeXXXX
          LastTicketTypeID = int.Parse(_menuID.Substring(13, _menuID.Length - 13));
        }
      }
      catch (Exception)
      {
      }

      if (Request["TicketTypeID"] != null)
      {
        try
        {
          LastTicketTypeID = int.Parse(Request["TicketTypeID"]);
        }
        catch
        {
        }
      }

      if (LastTicketTypeID < 0)
      {
        TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
        ticketTypes.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, UserSession.CurrentUser.ProductType);
        if (!ticketTypes.IsEmpty) LastTicketTypeID = ticketTypes[0].TicketTypeID;
      }
    }

    if (LastTicketTypeID > 0)
    (new CustomFieldControls(ReferenceType.Tickets, LastTicketTypeID, -1, 2, tblCustomControls, false)).LoadCustomControls(200);


  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    if (Settings.OrganizationDB.ReadBool("RequireNewTicketCustomer", false)) cmbCustomer.CssClass = "validateXCombo validateCustomer";

    if (!UserSession.CurrentUser.HasPortalRights)
    {
      spanPortal.Visible = false;
      cbPortal.Visible = false;
    }

    if (UserSession.CurrentUser.ProductType == ProductType.Express || UserSession.CurrentUser.ProductType == ProductType.BugTracking)
    {
      cmbCustomer.Visible = false;
      spanCustomer.Visible = false;
    }

    if (UserSession.CurrentUser.ProductType == ProductType.Express || UserSession.CurrentUser.ProductType == ProductType.HelpDesk)
    {
      cmbProduct.Visible = false;
      spanProduct.Visible = false;
      cmbVersion.Visible = false;
      spanVersion.Visible = false;
    }

    if (!IsPostBack)
    {
      LoadCombos();

      
      if (TicketTemplates.GetTicketTypeCount(UserSession.LoginUser) > 0)
      {
        TicketTemplate template = TicketTemplates.GetByTicketType(UserSession.LoginUser, GetSelectedTicketTypeID());
        editorDescription.Content = template != null ? template.TemplateText : "";
      }

    }
    

  }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender(e);

    Response.Cache.SetAllowResponseInBrowserHistory(false);
    Response.Cache.SetCacheability(HttpCacheability.NoCache);
    Response.Cache.SetNoStore();
    Response.Expires = 0;
  }

  public int Save()
  {
    try
    {
      if (uploadMain.UploadedFiles.Count > 0)
      {
        int used = Organizations.GetStorageUsed(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
        int allowed = Organizations.GetTotalStorageAllowed(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);

        if (used > allowed)
        {
          if (UserSession.CurrentUser.IsFinanceAdmin)
          {
            RadAjaxManager1.Alert("You have exceeded your allocated storage capacity.  If you would like to add additional storage, please contact our sales team at 800.596.2820 x806, or send an email to sales@teamsupport.com");
          }
          else
          {
            Users users = new Users(UserSession.LoginUser);
            users.LoadFinanceAdmins(UserSession.LoginUser.OrganizationID);
            if (users.IsEmpty)
            {
              RadAjaxManager1.Alert("Please ask your billing administrator to purchase additional storage to add additional attachments.");
            }
            else
            {
              RadAjaxManager1.Alert("Please ask your billing administrator (" + users[0].FirstLastName + ") to purchase additional storage to add additional attachments.");
            }
          }
          return -1;
        }
      }

      try
      {
        SetStickyID(ComboBoxIDTypes.Customer, GetComboID(cmbCustomer));
        SetStickyID(ComboBoxIDTypes.Version, GetComboID(cmbVersion));
        SetStickyID(ComboBoxIDTypes.Product, GetComboID(cmbProduct));

        SetStickyID(ComboBoxIDTypes.Group, GetComboID(cmbGroup));
        SetStickyID(ComboBoxIDTypes.Severity, GetComboID(cmbSeverity));
        SetStickyID(ComboBoxIDTypes.Status, GetComboID(cmbStatus));
        SetStickyID(ComboBoxIDTypes.User, GetComboID(cmbUser));
      }
      catch (Exception)
      {
      }

      if (textName.Text.Trim() == "")
      {
        RadAjaxManager1.Alert("Please enter a name for this ticket.");
        return -1;
      }


      Tickets tickets = new Tickets(UserSession.LoginUser);
      Ticket ticket = tickets.AddNewTicket();
      ticket.OrganizationID = UserSession.LoginUser.OrganizationID;
      ticket.Name = textName.Text;
      ticket.TicketTypeID = int.Parse(cmbTicketType.SelectedValue);
      ticket.TicketStatusID = int.Parse(cmbStatus.SelectedValue);
      ticket.TicketSeverityID = int.Parse(cmbSeverity.SelectedValue);
      ticket.IsKnowledgeBase = cbKnowledgeBase.Checked;
      ticket.IsVisibleOnPortal = cbPortal.Checked;
      if (cmbUser.SelectedIndex > 0) ticket.UserID = int.Parse(cmbUser.SelectedValue);
      if (cmbGroup.SelectedIndex > 0) ticket.GroupID = int.Parse(cmbGroup.SelectedValue);
      if (cmbProduct.SelectedIndex > 0) ticket.ProductID = int.Parse(cmbProduct.SelectedValue);
      if (cmbVersion.SelectedIndex > 0) ticket.ReportedVersionID = int.Parse(cmbVersion.SelectedValue);
      tickets.Save();

      Actions actions = new Actions(UserSession.LoginUser);
      TeamSupport.Data.Action action = actions.AddNewAction();
      action.ActionTypeID = null;
      action.Name = "Description";
      action.SystemActionTypeID = SystemActionType.Description;
      action.Description = editorDescription.Content;
      action.IsVisibleOnPortal = cbPortal.Checked;
      action.IsKnowledgeBase = cbKnowledgeBase.Checked;
      action.TicketID = ticket.TicketID;
      actions.Save();

      if (_chatID > -1)
      {
        Chat chat = Chats.GetChat(UserSession.LoginUser, _chatID);
        if (chat != null)
        {
          actions = new Actions(UserSession.LoginUser);
          TeamSupport.Data.Action chatAction = actions.AddNewAction();
          chatAction.ActionTypeID = null;
          chatAction.Name = "Chat";
          chatAction.SystemActionTypeID = SystemActionType.Chat;
          chatAction.Description = chat.GetHtml(true, UserSession.LoginUser.OrganizationCulture);
          chatAction.IsVisibleOnPortal = cbPortal.Checked;
          chatAction.IsKnowledgeBase = cbKnowledgeBase.Checked;
          chatAction.TicketID = ticket.TicketID;
          actions.Save();
          chat.ActionID = chatAction.ActionID;
          chat.Collection.Save();
        }

      }


      try
      {
        if (cmbCustomer.SelectedValue.Length > 0 && cmbCustomer.SelectedValue != "-1")
        {
          int id = int.Parse(cmbCustomer.SelectedValue.Remove(0, 1));
          if (cmbCustomer.SelectedValue[0] == 'o')
          {
            tickets.AddOrganization(id, ticket.TicketID);
          }
          else
          {
            tickets.AddContact(id, ticket.TicketID);
          }
        }

      }
      catch (Exception)
      {

      }

      SaveAttachments(action.ActionID);

      (new CustomFieldControls(ReferenceType.Tickets, GetSelectedTicketTypeID(), ticket.TicketID, 2, tblCustomControls, false)).SaveCustomFields();


      User user = Users.GetUser(UserSession.LoginUser, UserSession.LoginUser.UserID);
      if (user.SubscribeToNewTickets)
        Subscriptions.AddSubscription(UserSession.LoginUser, UserSession.LoginUser.UserID, ReferenceType.Tickets, ticket.TicketID);

      return ticket.TicketID;
    }
    catch (Exception ex)
    {
      RadAjaxManager1.Alert("There was an error saving your ticket.  Please try again later.");
      ExceptionLogs.LogException(UserSession.LoginUser, ex, "New Ticket");
      return -1;
    }
  }

  private void SaveAttachments(int actionID)
  {
    Attachments attachments = new Attachments(UserSession.LoginUser);

    foreach (UploadedFile file in uploadMain.UploadedFiles)
    {
      string directory = TSUtils.GetAttachmentPath("Actions", actionID);

      TeamSupport.Data.Attachment attachment = attachments.AddNewAttachment();
      attachment.RefType = ReferenceType.Actions;
      attachment.RefID = actionID;
      attachment.OrganizationID = UserSession.LoginUser.OrganizationID;
      attachment.FileName = file.GetName();
      attachment.Path = Path.Combine(directory, attachment.FileName);
      attachment.FileType = string.IsNullOrEmpty(file.ContentType) ? "application/octet-stream" : file.ContentType;
      attachment.FileSize = file.ContentLength;

      Directory.CreateDirectory(directory);
      file.SaveAs(attachment.Path, true);
      attachments.Save();
    }

  }

  private int GetComboID(RadComboBox combo)
  {
    if (combo.SelectedIndex < 0)
      return -1;
    else
      return int.Parse(combo.SelectedValue);
  }

  private int GetSelectedTicketTypeID()
  {
    return GetComboID(cmbTicketType);
  }

  private string GetSettingKey(ComboBoxIDTypes type)
  {
    return "NewTicketDialogSetting-" + GetSelectedTicketTypeID().ToString() + "-" + ((int)type).ToString();
  }

  private void SetStickyID(ComboBoxIDTypes type, int id)
  {
    if (GetSelectedTicketTypeID() > -1)
    {

      Settings.UserDB.WriteInt(GetSettingKey(type), id);
    }
  }

  private int GetStickyID(ComboBoxIDTypes type)
  {/*
    int result;
    string idParam = null;
    switch (type)
    {
      case ComboBoxIDTypes.Customer: idParam = Request["CustomerID"]; break;
      case ComboBoxIDTypes.Status: idParam = Request["TicketStatusID"]; break;
      case ComboBoxIDTypes.Severity: idParam = Request["TicketSeverityID"]; break;
      case ComboBoxIDTypes.User: idParam = Request["UserID"]; break;
      case ComboBoxIDTypes.Group: idParam = Request["GroupID"]; break;
      case ComboBoxIDTypes.Product: idParam = Request["ProductID"]; break;
      case ComboBoxIDTypes.Version: idParam = Request["VersionID"]; break;
      default: break;
    }

    if (!string.IsNullOrEmpty(idParam))
    {
      try
      {
        result = int.Parse(idParam);
        return result;
      }
      catch { }
    }
    */

    if (GetSelectedTicketTypeID() < 0)
      return -1;
    else
      return Settings.UserDB.ReadInt(GetSettingKey(type), -1);
  }

  private void LoadCombos()
  {
    TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
    ticketTypes.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, UserSession.CurrentUser.ProductType);
    cmbTicketType.DataTextField = "Name";
    cmbTicketType.DataValueField = "TicketTypeID";
    cmbTicketType.DataSource = ticketTypes.Table;
    cmbTicketType.DataBind();

    if (cmbTicketType.Items.Count > 0) cmbTicketType.SelectedIndex = 0;
    var ticketTypeItem = cmbTicketType.FindItemByValue(LastTicketTypeID.ToString());
    if (ticketTypeItem != null) cmbTicketType.SelectedValue = LastTicketTypeID.ToString();



    LoadStatuses();

    TicketSeverities ticketSeverities = new TicketSeverities(UserSession.LoginUser);
    ticketSeverities.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
    cmbSeverity.DataTextField = "Name";
    cmbSeverity.DataValueField = "TicketSeverityID";
    cmbSeverity.DataSource = ticketSeverities.Table;
    cmbSeverity.DataBind();
    //cmbSeverity.SelectedIndex =  GetStickyID(ComboBoxIDTypes.Severity).ToString();
    cmbSeverity.SelectedIndex = 0;

    Users users = new Users(UserSession.LoginUser);
    users.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, true);
    cmbUser.Items.Clear();
    cmbUser.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
    foreach (User user in users)
    {
      cmbUser.Items.Add(new RadComboBoxItem(user.LastName + ", " + user.FirstName, user.UserID.ToString()));
    }
    cmbUser.SelectedValue = UserSession.LoginUser.UserID.ToString();

    /*if (GetStickyID(ComboBoxIDTypes.User) < 0)
      cmbUser.SelectedValue = UserSession.LoginUser.UserID.ToString();
    else
      cmbUser.SelectedValue = GetStickyID(ComboBoxIDTypes.User).ToString();*/



    Groups groups = new Groups(UserSession.LoginUser);
    groups.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
    cmbGroup.Items.Clear();
    cmbGroup.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
    foreach (Group group in groups)
    {
      cmbGroup.Items.Add(new RadComboBoxItem(group.Name, group.GroupID.ToString()));
    }
    cmbGroup.SelectedIndex = 0;
    //cmbGroup.SelectedValue = GetStickyID(ComboBoxIDTypes.Group).ToString();

    //Organizations organizations = new Organizations(UserSession.LoginUser);
    //organizations.LoadByParentID(UserSession.LoginUser.OrganizationID, true);
    /*cmbCustomer.Items.Clear();
    cmbCustomer.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
    foreach (Organization organization in organizations)
    {
      cmbCustomer.Items.Add(new RadComboBoxItem(organization.Name, organization.OrganizationID.ToString()));
    }*/

    if (_menuID == "mniCustomers")
    {
      UsersViewItem user = UsersView.GetUsersViewItem(UserSession.LoginUser, Settings.UserDB.ReadInt("SelectedContactID", -1));
      if (user != null)
      { 
          cmbCustomer.Items.Clear();
          cmbCustomer.Items.Add(new RadComboBoxItem(String.Format("{0}, {1} [{2}]", user.LastName, user.FirstName, user.Organization), 'u' + user.UserID.ToString()));
          cmbCustomer.SelectedIndex = 0;
      }
      else
      {
        Organization customer = Organizations.GetOrganization(UserSession.LoginUser, Settings.UserDB.ReadInt("SelectedOrganizationID", -1));
        if (customer != null)
        {
          cmbCustomer.Items.Clear();
          cmbCustomer.Items.Add(new RadComboBoxItem(customer.Name, 'o' + customer.OrganizationID.ToString()));
          cmbCustomer.SelectedIndex = 0;

        }
      }

    }

    if (_chatID > -1)
    {
      Chat chat = Chats.GetChat(UserSession.LoginUser, _chatID);
      if (chat != null)
      {
        User contact = Users.GetUser(UserSession.LoginUser, chat.GetInitiatorLinkedUserID());
        if (contact != null)
        {
          cmbCustomer.Items.Clear();
          cmbCustomer.Items.Add(new RadComboBoxItem(contact.FirstLastName, 'u' + contact.UserID.ToString()));
          cmbCustomer.SelectedIndex = 0;

        }
      }
      

    
    }
    //cmbCustomer.SelectedValue = GetStickyID(ComboBoxIDTypes.Customer).ToString();



    LoadProducts();
    if (_menuID == "mniProducts")
      cmbProduct.SelectedValue = Settings.UserDB.ReadString("SelectedProductID", "-1");
    else
      cmbProduct.SelectedValue = GetStickyID(ComboBoxIDTypes.Product).ToString();  

    LoadVersions();

    if (_menuID != "mniProducts")
    {
      if (GetStickyID(ComboBoxIDTypes.Version) > -1)
        cmbVersion.SelectedValue = GetStickyID(ComboBoxIDTypes.Version).ToString();
    }
    else
    {
      int id = Settings.UserDB.ReadInt("SelectedVersionID", -1);
      if (id > -1) cmbVersion.SelectedValue = id.ToString();
    }


    if (cmbVersion.SelectedIndex < 1 && cmbVersion.Items.Count > 1) cmbVersion.SelectedIndex = 1;

  }

  private void LoadProducts()
  {
    Products products = new Products(UserSession.LoginUser);
    if (Settings.OrganizationDB.ReadBool("ShowOnlyCustomerProducts", false))
    {
      try
      {
        int customerID = GetSelectedCustomerID();
        if (customerID > -1) products.LoadByCustomerID(customerID);
        if (products.IsEmpty) products.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
      }
      catch (Exception)
      {
        products.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
      }
    }
    else
    {
      products.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
    }

    cmbProduct.Items.Clear();
    cmbProduct.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
    foreach (Product product in products)
    {
      cmbProduct.Items.Add(new RadComboBoxItem(product.Name, product.ProductID.ToString()));
    }
  }

  private void LoadVersions()
  {
    cmbVersion.Items.Clear();
    cmbVersion.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
    cmbVersion.SelectedIndex = 0;

    if (cmbProduct.SelectedIndex < 0 || int.Parse(cmbProduct.SelectedValue) < 0) return;

    ProductVersions productVersions = new ProductVersions(UserSession.LoginUser);
    if (Settings.OrganizationDB.ReadBool("ShowOnlyCustomerProducts", false))
    {
      try
      {
        productVersions.LoadByProductAndCustomer(int.Parse(cmbProduct.SelectedValue), GetSelectedCustomerID());
        if (productVersions.IsEmpty) productVersions.LoadByProductID(int.Parse(cmbProduct.SelectedValue));

      }
      catch (Exception)
      {
        productVersions.LoadByProductID(int.Parse(cmbProduct.SelectedValue));
      }
    }
    else
    {
      productVersions.LoadByProductID(int.Parse(cmbProduct.SelectedValue));
    }

    
    
    foreach (ProductVersion version in productVersions)
    {
      cmbVersion.Items.Add(new RadComboBoxItem(version.VersionNumber, version.ProductVersionID.ToString()));
    }
  }

  private void LoadStatuses()
  {
    cmbStatus.Items.Clear();
    if (cmbTicketType.SelectedIndex < 0) return;
    TicketStatuses ticketStatuses = new TicketStatuses(UserSession.LoginUser);
    ticketStatuses.LoadByPosition(int.Parse(cmbTicketType.SelectedValue), 0);
    if (ticketStatuses.IsEmpty)
    {
      ticketStatuses = new TicketStatuses(UserSession.LoginUser);
      ticketStatuses.LoadByTicketTypeID(int.Parse(cmbTicketType.SelectedValue));

      foreach (TicketStatus status in ticketStatuses)
      {
        cmbStatus.Items.Add(new RadComboBoxItem(status.Name, status.TicketStatusID.ToString()));
      }

      cmbStatus.SelectedValue = GetStickyID(ComboBoxIDTypes.Status).ToString();

      RadAjaxManager1.Alert("Your ticket status information is invalid.  Please contact your administrator.");

      return;
    }

    cmbStatus.Items.Add(new RadComboBoxItem(ticketStatuses[0].Name, ticketStatuses[0].TicketStatusID.ToString()));

    TicketNextStatuses ticketNextStatuses = new TicketNextStatuses(UserSession.LoginUser);
    ticketNextStatuses.LoadNextStatuses(ticketStatuses[0].TicketStatusID);
    foreach (TicketNextStatus ticketNextStatus in ticketNextStatuses)
    {
      cmbStatus.Items.Add(new RadComboBoxItem(ticketNextStatus.Name, ticketNextStatus.NextStatusID.ToString()));
    }

    //cmbStatus.SelectedValue = GetStickyID(ComboBoxIDTypes.Status).ToString();
    cmbStatus.SelectedIndex = 0;


  }

  private int GetSelectedCustomerID()
  {
    try
    {
      string id = cmbCustomer.SelectedValue;
      bool isUser = id.ToLower()[0] == 'u';
      int i = int.Parse(id.Remove(0, 1));
      if (isUser)
      {
        i = Users.GetUser(UserSession.LoginUser, i).OrganizationID;
      }
      return i;
    }
    catch (Exception)
    {
      return -1;
    }

  }


  protected void cmbTicketType_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
  {
    tblCustomControls.Controls.Clear();
    LastTicketTypeID = GetSelectedTicketTypeID();
    (new CustomFieldControls(ReferenceType.Tickets, GetSelectedTicketTypeID(), -1, 2, tblCustomControls, false)).LoadCustomControls(200);
    LoadStatuses();

    if (TicketTemplates.GetTicketTypeCount(UserSession.LoginUser) > 0)
    {
      TicketTemplate template = TicketTemplates.GetByTicketType(UserSession.LoginUser, GetSelectedTicketTypeID());
      editorDescription.Content = template != null ? template.TemplateText : "";
    }

  }

  protected void cmbProduct_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
  {
    LoadVersions();
  }

  protected void cmbCustomer_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
  {
    string productID = cmbProduct.SelectedValue;
    string versionID = cmbVersion.SelectedValue;

    LoadProducts();
    cmbProduct.SelectedValue = productID;

    LoadVersions();
    cmbVersion.SelectedValue = versionID;
  }

  protected void btnSave_Click(object sender, EventArgs e)
  {
    int ticketID = Save();
    if (ticketID > -1) DynamicScript.ExecuteScript(Page, "NewTicketSaveScript", "SaveAndClose(" + ticketID.ToString() + ");");
    
  }

  [WebMethod(true)]
  public static RadComboBoxItemData[] GetUserOrOrganization(RadComboBoxContext context)
  {
    IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;

    Organizations organizations = new Organizations(UserSession.LoginUser);
    organizations.LoadByLikeOrganizationName(UserSession.LoginUser.OrganizationID, context["FilterString"].ToString(), !UserSession.CurrentUser.IsSystemAdmin, 250);

    UsersView users = new UsersView(UserSession.LoginUser);
    users.LoadByLikeName(UserSession.LoginUser.OrganizationID, context["FilterString"].ToString(), 250);

    List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();

    RadComboBoxItemData unassigned = new RadComboBoxItemData();
    unassigned.Text = "Unassigned";
    unassigned.Value = "-1";
    list.Add(unassigned);

    foreach (Organization organization in organizations)
    {
      RadComboBoxItemData itemData = new RadComboBoxItemData();
      itemData.Text = organization.Name;
      itemData.Value = 'o' + organization.OrganizationID.ToString();
      list.Add(itemData);
    }

    foreach (UsersViewItem user in users)
    {
      RadComboBoxItemData itemData = new RadComboBoxItemData();
      itemData.Text = String.Format("{0}, {1} [{2}]", user.LastName, user.FirstName, user.Organization);
      itemData.Value = 'u' + user.UserID.ToString();
      list.Add(itemData);
    }

    return list.ToArray();
  }

  [WebMethod(true)]
  public static string GetPickListTemplateText(string value)
  {
    int count = TicketTemplates.GetTriggerTextCount(UserSession.LoginUser);
    if (count < 1) return "void";

    TicketTemplate template = TicketTemplates.GetByTriggerText(UserSession.LoginUser, value);
    if (template == null) return "";
    if (template.OrganizationID != TSAuthentication.OrganizationID) return "";
    return template.TemplateText;
  }

  protected void btnSaveView_Click(object sender, EventArgs e)
  {
    int ticketID = Save();
    if (ticketID > -1) DynamicScript.ExecuteScript(Page, "NewTicketSaveScript", "SaveAndView(" + ticketID.ToString() + ");");
  }

  [WebMethod(true)]
  public static TicketAndActions GetTicketActions(int ticketID)
  {
    TicketAndActions result = new TicketAndActions();
    result.Ticket = TicketsView.GetTicketsViewItem(UserSession.LoginUser, ticketID).GetProxy();
    if (result.Ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
    ActionsView actions = new ActionsView(UserSession.LoginUser);
    actions.LoadKBByTicketID(ticketID);
    result.Actions = actions.GetActionsViewItemProxies();
    return result;
  }

  [DataContract]
  public class TicketAndActions
  {
    [DataMember]
    public TicketsViewItemProxy Ticket { get; set; }
    [DataMember]
    public ActionsViewItemProxy[] Actions { get; set; }

  }
}
