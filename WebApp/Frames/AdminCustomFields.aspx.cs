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

public partial class Frames_AdminCustomFields : BaseFramePage
{
  public int SelectedTicketTypeID
  {
    get { return Settings.Session.ReadInt("SelectedCustomFieldsTicketTypeID", -1); }
    set { Settings.Session.WriteInt("SelectedCustomFieldsTicketTypeID", value); }
  }

  public int SelectedReferenceType
  {
    get { return Settings.Session.ReadInt("SelectedCustomFieldsReferenceType", -1); }
    set { Settings.Session.WriteInt("SelectedCustomFieldsReferenceType", value); }
  }


  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);

    LoadTicketTypes();
    LoadFieldTypes();
    if (SelectedTicketTypeID > -1) cmbTicketTypes.SelectedValue = SelectedTicketTypeID.ToString();
    if (SelectedReferenceType > -1) cmbFieldTypes.SelectedValue = SelectedReferenceType.ToString();
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
  
    SelectedReferenceType = int.Parse(cmbFieldTypes.SelectedValue);
    SelectedTicketTypeID = int.Parse(cmbTicketTypes.SelectedValue);
    
    pnlTicket.Visible = (ReferenceType)SelectedReferenceType == ReferenceType.Tickets;


    bool isAdmin = UserSession.CurrentUser.IsSystemAdmin;
    lnkAddStatus.Visible = isAdmin;
    gridProperties.Columns[0].Visible = isAdmin;
    gridProperties.Columns[1].Visible = isAdmin;
    gridProperties.Columns[2].Visible = isAdmin;
    gridProperties.Columns[3].Visible = isAdmin;
    if (lnkAddStatus.Visible)
      if (pnlTicket.Visible)
        lnkAddStatus.OnClientClick = "ShowDialog(top.GetCustomFieldDialog(null," + SelectedReferenceType.ToString() + "," + SelectedTicketTypeID.ToString() +")); return false;";
      else
        lnkAddStatus.OnClientClick = "ShowDialog(top.GetCustomFieldDialog(null," + SelectedReferenceType.ToString() + ",null)); return false;";
    

  }

  private void LoadFieldTypes()
  {
    cmbFieldTypes.Items.Clear();
    cmbFieldTypes.Items.Add(new RadComboBoxItem("Tickets", ((int)ReferenceType.Tickets).ToString()));
    cmbFieldTypes.Items.Add(new RadComboBoxItem("Users", ((int)ReferenceType.Users).ToString()));
    cmbFieldTypes.Items.Add(new RadComboBoxItem("Products", ((int)ReferenceType.Products).ToString()));
    cmbFieldTypes.Items.Add(new RadComboBoxItem("Product Versions", ((int)ReferenceType.ProductVersions).ToString()));
    cmbFieldTypes.Items.Add(new RadComboBoxItem("Customers", ((int)ReferenceType.Organizations).ToString()));
    cmbFieldTypes.Items.Add(new RadComboBoxItem("Customer Products", ((int)ReferenceType.OrganizationProducts).ToString()));
    cmbFieldTypes.Items.Add(new RadComboBoxItem("Customer Contacts", ((int)ReferenceType.Contacts).ToString()));
    cmbFieldTypes.SelectedIndex = 0;
  }

  private void LoadTicketTypes()
  {
    cmbTicketTypes.Items.Clear();
    TicketTypes types = new TicketTypes(UserSession.LoginUser);
    types.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, UserSession.CurrentUser.ProductType);
    foreach (TicketType type in types)
    {
      cmbTicketTypes.Items.Add(new RadComboBoxItem(type.Name, type.TicketTypeID.ToString()));
    }
    if (cmbTicketTypes.Items.Count > 0) cmbTicketTypes.SelectedIndex = 0;
  }

  protected void gridProperties_ItemCommand(object source, GridCommandEventArgs e)
  {
    int id = (int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["CustomFieldID"];
    if (e.CommandName == RadGrid.DeleteCommandName)
    {
      CustomFields fields = new CustomFields(UserSession.LoginUser);
      fields.DeleteFromDB(id);
    }
    else if (e.CommandName == "MoveUp")
    {
      CustomFields fields = new CustomFields(UserSession.LoginUser);
      fields.MovePositionUp(id);
    }
    else if (e.CommandName == "MoveDown")
    {
      CustomFields fields = new CustomFields(UserSession.LoginUser);
      fields.MovePositionDown(id);
    }

    gridProperties.Rebind();
  }

  protected void gridProperties_ItemDataBound(object sender, GridItemEventArgs e)
  {
    if (e.Item is GridDataItem)
    {
      GridDataItem item = e.Item as GridDataItem;
      Label label = item.FindControl("lblFieldType") as Label;
      label.Text = CustomFields.GetCustomFieldTypeName((CustomFieldType)(int.Parse(item["FieldType"].Text)));


      string key = item.GetDataKeyValue("CustomFieldID").ToString();

      ImageButton button = (ImageButton)item["ButtonEdit"].Controls[0];
      button.OnClientClick = "EditRow('" + key + "'); return false;";
    
    }
  }
  
  protected void gridProperties_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
  {
    ReferenceType refType = (ReferenceType)SelectedReferenceType;
    int auxID = SelectedTicketTypeID;
    CustomFields fields = new CustomFields(UserSession.LoginUser);
    if (refType != ReferenceType.Tickets)
      fields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, refType);
    else
      fields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, refType, auxID);

    gridProperties.DataSource = fields.Table;
  }
  protected void cmbFieldTypes_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
  {
    gridProperties.Rebind();

  }
  protected void cmbTicketTypes_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
  {
    gridProperties.Rebind();

  }
}

