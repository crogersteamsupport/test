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

public partial class Dialogs_CRMProperties : BaseDialogPage
{
  private int _organizationID = -1;

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);
    if (Request["OrganizationID"] != null) _organizationID = int.Parse(Request["OrganizationID"]);
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    if (_organizationID != UserSession.LoginUser.OrganizationID)
    {
      Response.Write("Invalid paramater");
      Response.End();
      return;
    }

    if (!IsPostBack)
    {
      cmbType.Items.Add(new RadComboBoxItem("Highrise", "Highrise"));
      cmbType.Items.Add(new RadComboBoxItem("Salesforce.com", "Salesforce"));
      SetLabels(cmbType.SelectedValue);
      LoadData();
    }
  }

  private void LoadData()
  {
    CRMLinkTableItem item = GetCrmItem();
    if (item == null) return;

    cmbType.SelectedValue = item.CRMType;
    textUserName.Text = item.Username;
    textPassword.Text = item.Password;
    textPasswordConfirm.Text = item.Password;
    textSecurityToken.Text = item.SecurityToken;
    textSecurityTokenConfirm.Text = item.SecurityToken;
    textTypeFieldMatch.Text = item.TypeFieldMatch;
    cbActive.Checked = item.Active == null ? false : (bool)item.Active;
    SetLabels(item.CRMType);
  }

  private CRMLinkTableItem GetCrmItem()
  {
    CRMLinkTable table = new CRMLinkTable(UserSession.LoginUser);
    table.LoadByOrganizationID(_organizationID);
    if (table.IsEmpty) return null;
    return table[0];

  }

  private void SetLabels(string crmType)
  { 
    crmType = crmType.ToLower().Trim();
    if (crmType == "highrise")
    {
      lblUserName.Text = "Highrise Company Name<br/>(Example: 'mycompany' .highrisehq.com)";
      lblSecurityToken.Text = "Authentication Token";
      lblSecurityTokenConfirm.Text = "Confirm Authentication Token";
      lblTypeFieldMatch.Text = "Tag which identifies customer";
      divPassword.Visible = false;
      textSecurityTokenConfirm.Visible = true;
      textSecurityToken.TextMode = InputMode.SingleLine;
    }
    else if (crmType == "salesforce")
    {
      lblTypeFieldMatch.Text = "Account Type to link to TeamSupport";
      lblUserName.Text = "Salesforce Username";
      lblPassword.Text = "Salesforce Password";
      lblConfirm.Text = "Confirm Salesforce Password";
      lblSecurityToken.Text = "Salesforce Security Token";
      lblSecurityTokenConfirm.Text = "Confirm Salesforce Security Token";
      textSecurityToken.TextMode = InputMode.Password;
      textSecurityTokenConfirm.Visible = true;
      divPassword.Visible = true;
    }
  
  }

  public override bool Save()
  {
    if (divPassword.Visible && textPassword.Text != textPasswordConfirm.Text)
    {
      _manager.Alert("Passwords do not match.");
      return false;
    }

    if (textSecurityToken.Text != textSecurityTokenConfirm.Text)
    {
      _manager.Alert("Security Tokens do not match.");
      return false;
    }

    if (textSecurityToken.Text.Trim() == "")
    {
      _manager.Alert("Please enter a security token.");
      return false;
    }

    if (divPassword.Visible && textPassword.Text.Trim() == "")
    {
      _manager.Alert("Please enter a password.");
      return false;
    }
    CRMLinkTableItem item = GetCrmItem();
    if (item == null)
    {
      CRMLinkTable table = new CRMLinkTable(UserSession.LoginUser);
      item = table.AddNewCRMLinkTableItem();
      item.OrganizationID = UserSession.LoginUser.OrganizationID;
    }
    item.Active = cbActive.Checked;
    item.CRMType = cmbType.SelectedValue;
    item.Password = textPassword.Text;
    item.SecurityToken = textSecurityToken.Text;
    item.TypeFieldMatch = textTypeFieldMatch.Text;
    item.Username = cmbType.SelectedIndex == 0 ? textUserName.Text.Replace(" ", "") : textUserName.Text;
    item.Collection.Save();


    return true;
  }




  protected void cmbType_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
  {
    SetLabels(cmbType.SelectedValue);
  }
}
