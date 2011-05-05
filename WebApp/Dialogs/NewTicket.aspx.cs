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
using System.IO;

public partial class Dialogs_NewTicket : BaseDialogPage
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

  private string _tabText;
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
    _tabText = Settings.Session.ReadString("MainTabText", "");

    if (!IsPostBack)
    {
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

    _manager.AjaxSettings.Clear();

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

    _manager.AjaxSettings.Clear();
    _manager.AjaxSettings.AddAjaxSetting(pnlProperties, pnlProperties);
    _manager.AjaxSettings.AddAjaxSetting(editorDescription, editorDescription);

    if (!IsPostBack)
    {
      LoadCombos();
    }
    
  }

  public override bool Save()
  {
    if (uploadMain.UploadedFiles.Count > 0)
    {
      int used = Organizations.GetStorageUsed(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
      int allowed = Organizations.GetTotalStorageAllowed(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);

      if (used > allowed)
      {
        if (UserSession.CurrentUser.IsFinanceAdmin)
        {
          _manager.Alert("You have exceeded your allocated storage capacity.  If you would like to add additional storage, please contact our sales team at 800.596.2820 x806, or send an email to sales@teamsupport.com");
        }
        else
        {
          Users users = new Users(UserSession.LoginUser);
          users.LoadFinanceAdmins(UserSession.LoginUser.OrganizationID);
          if (users.IsEmpty)
          {
            _manager.Alert("Please ask your billing administrator to purchase additional storage to add additional attachments.");
          }
          else
          {
            _manager.Alert("Please ask your billing administrator (" + users[0].FirstLastName + ") to purchase additional storage to add additional attachments.");
          }
        }


        return false;
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
      _manager.Alert("Please enter a name for this ticket.");
      return false;
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

    if (cmbCustomer.SelectedValue.Length > 0)
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

    SaveAttachments(action.ActionID);

    (new CustomFieldControls(ReferenceType.Tickets, GetSelectedTicketTypeID(), ticket.TicketID, 2, tblCustomControls, false)).SaveCustomFields();

    return true;
  }

  private void SaveAttachments(int actionID)
  {
    Attachments attachments = new Attachments(UserSession.LoginUser);

    foreach (UploadedFile file in uploadMain.UploadedFiles)
    {
      string directory = TSUtils.GetAttachmentPath("Actions", actionID);

      Attachment attachment = attachments.AddNewAttachment();
      attachment.RefType = ReferenceType.Actions;
      attachment.RefID = actionID;
      attachment.OrganizationID = UserSession.LoginUser.OrganizationID;
      attachment.FileName = file.GetName();
      attachment.Path = Path.Combine(directory, attachment.FileName);
      attachment.FileType = file.ContentType;
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
    cmbTicketType.SelectedValue = LastTicketTypeID.ToString();

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

    Organizations organizations = new Organizations(UserSession.LoginUser);
    organizations.LoadByParentID(UserSession.LoginUser.OrganizationID, true);
    /*cmbCustomer.Items.Clear();
    cmbCustomer.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
    foreach (Organization organization in organizations)
    {
      cmbCustomer.Items.Add(new RadComboBoxItem(organization.Name, organization.OrganizationID.ToString()));
    }*/

    if (_tabText == "Customers")
    {
      Organization customer = Organizations.GetOrganization(UserSession.LoginUser, Settings.UserDB.ReadInt("SelectedOrganizationID", -1));
      if (customer != null)
      {
        cmbCustomer.Items.Clear();
        cmbCustomer.Items.Add(new RadComboBoxItem(customer.Name, 'o'+ customer.OrganizationID.ToString()));
        cmbCustomer.SelectedIndex = 0;
      
      }
    }
    //cmbCustomer.SelectedValue = GetStickyID(ComboBoxIDTypes.Customer).ToString();



    LoadProducts();
    if (_tabText == "Products")
      cmbProduct.SelectedValue = Settings.UserDB.ReadString("SelectedProductID", "-1");
    else
      cmbProduct.SelectedValue = GetStickyID(ComboBoxIDTypes.Product).ToString();  

    LoadVersions();

    if (_tabText != "Products")
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

  private void LoadProducts()
  {
    Products products = new Products(UserSession.LoginUser);
    if (Settings.OrganizationDB.ReadBool("ShowOnlyCustomerProducts", false))
    {
      try
      {
        products.LoadByCustomerID(GetSelectedCustomerID());
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

      _manager.Alert("Your ticket status information is invalid.  Please contact your administrator.");

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

  protected void cmbTicketType_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
  {
    tblCustomControls.Controls.Clear();
    LastTicketTypeID = GetSelectedTicketTypeID();
    (new CustomFieldControls(ReferenceType.Tickets, GetSelectedTicketTypeID(), -1, 2, tblCustomControls, false)).LoadCustomControls(200);
    LoadStatuses();
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
}
