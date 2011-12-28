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
using System.Drawing;

public partial class Dialogs_PortalOptions : BaseDialogPage
{
  private int _organizatinID = -1;

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);

  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    if (!UserSession.CurrentUser.IsSystemAdmin)
    {
      Response.Write("");
      Response.End();
      return;
    }

    if (Request["OrganizationID"] != null) _organizatinID = int.Parse(Request["OrganizationID"]);

    if (_organizatinID != UserSession.LoginUser.OrganizationID)
    {
      Response.Write("Invalid Request");
      Response.End();
      return;
    }

    if (!IsPostBack)
    {
      LoadGroups(_organizatinID);
      LoadThemes();
      LoadData(_organizatinID);
    }
  }

  private void LoadThemes()
  {
    cmbTheme.Items.Clear();

    foreach (KeyValuePair<string, string> item in Enums.PortalThemeNames)
    {
      cmbTheme.Items.Add(new RadComboBoxItem(item.Value, item.Key));
    }

    cmbTheme.SelectedIndex = 0;
  }

  private void LoadGroups(int organizationID)
  {
    cmbGroups.Items.Clear();

    Groups groups = new Groups(UserSession.LoginUser);
    groups.LoadByOrganizationID(organizationID);
    cmbGroups.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
    foreach (Group group in groups)
    {
      cmbGroups.Items.Add(new RadComboBoxItem(group.Name, group.GroupID.ToString()));
    }
    cmbGroups.SelectedIndex = 0;

  }

  private void LoadData(int organizationID)
  {
    Organization organization = (Organization)Organizations.GetOrganization(UserSession.LoginUser, organizationID);
    PortalOption portalOption = (PortalOption)PortalOptions.GetPortalOption(UserSession.LoginUser, organizationID);
    if (portalOption == null || organization == null) return;
    textFooter.Text = portalOption.PortalHTMLFooter;
    textLanding.Text = portalOption.LandingPageHtml;
    textHeader.Text = portalOption.PortalHTMLHeader;
    cbUseCompanyInBasic.Checked = portalOption.UseCompanyInBasic == null ? false : (bool)portalOption.UseCompanyInBasic;
    cbCompanyRequiredInBasic.Checked = portalOption.CompanyRequiredInBasic == null ? false : (bool)portalOption.CompanyRequiredInBasic;
    cbHideGroupAssignedTo.Checked = portalOption.HideGroupAssignedTo == null ? false : (bool)portalOption.HideGroupAssignedTo;
    cbHideUserAssignedTo.Checked = portalOption.HideUserAssignedTo == null ? false : (bool)portalOption.HideUserAssignedTo;
    cbRecaptcha.Checked = portalOption.UseRecaptcha == null ? true : (bool)portalOption.UseRecaptcha;
    cbScreenr.Checked = portalOption.EnableScreenr;
    cbLanding.Checked = portalOption.DisplayLandingPage;
    cbDeflection.Checked = portalOption.DeflectionEnabled == null ? false : (bool)portalOption.DeflectionEnabled;
    textPortalName.Text = portalOption.PortalName;
    cbProduct.Checked = portalOption.DisplayProducts == null ? false : (bool) portalOption.DisplayProducts;
    cbVersion.Checked = portalOption.DisplayProductVersion;
    cbArticles.Checked = organization.IsPublicArticles;
    cbKb.Checked = portalOption.KBAccess == null ? false : (bool)portalOption.KBAccess;
    cbGroup.Checked = portalOption.DisplayGroups == null ? false : (bool)portalOption.DisplayGroups;
    textWidth.Value = portalOption.BasicPortalColumnWidth;
    textBasicPortalDirections.Text = portalOption.BasicPortalDirections;
    textAdvWidth.Value = portalOption.AdvPortalWidth;
    textExternalLink.Text = Settings.OrganizationDB.ReadString("ExternalPortalLink", "");
    cmbGroups.SelectedValue = organization.DefaultPortalGroupID.ToString();
    cmbTheme.SelectedValue = portalOption.Theme;
  }

  public override bool Save()
  {
    PortalOption portalOption = (PortalOption)PortalOptions.GetPortalOption(UserSession.LoginUser, _organizatinID);
    if (portalOption == null)
    {
      PortalOptions portalOptions = new PortalOptions(UserSession.LoginUser);
      portalOption = portalOptions.AddNewPortalOption();
      portalOption.OrganizationID = _organizatinID;
    }

    portalOption.PortalName = PortalOptions.ValidatePortalNameChars(textPortalName.Text);
    PortalOptions options = new PortalOptions(UserSession.LoginUser);
    options.LoadByPortalName(portalOption.PortalName);
    if (options.Count > 0)
    {
      if (options.Count > 1 || options[0].OrganizationID != UserSession.LoginUser.OrganizationID)
      {
        RadAjaxManager.GetCurrent(this.Page).Alert("That portal name is already taken.  Please choose a different portal name.");
        return false;
      }
    }



    portalOption.PortalHTMLHeader = textHeader.Text;
    portalOption.PortalHTMLFooter = textFooter.Text;
    portalOption.LandingPageHtml = textLanding.Text;
    portalOption.UseCompanyInBasic = cbUseCompanyInBasic.Checked;
    portalOption.CompanyRequiredInBasic = cbCompanyRequiredInBasic.Checked;
    portalOption.HideUserAssignedTo = cbHideUserAssignedTo.Checked;
    portalOption.HideGroupAssignedTo = cbHideGroupAssignedTo.Checked;
    portalOption.UseRecaptcha = cbRecaptcha.Checked;
    portalOption.EnableScreenr = cbScreenr.Checked;
    portalOption.DisplayLandingPage = cbLanding.Checked;
    portalOption.DeflectionEnabled = cbDeflection.Checked;
    portalOption.KBAccess = cbKb.Checked;
    portalOption.DisplayGroups = cbGroup.Checked;
    portalOption.DisplayProducts = cbProduct.Checked;
    portalOption.DisplayProductVersion = cbVersion.Checked;
    portalOption.BasicPortalColumnWidth = textWidth.Value == null ? null : (int?)textWidth.Value;
    portalOption.BasicPortalDirections = textBasicPortalDirections.Text == null ? "" : textBasicPortalDirections.Text;
    portalOption.AdvPortalWidth = textAdvWidth.Value == null ? null : (int?)textAdvWidth.Value;
    portalOption.Theme = cmbTheme.SelectedValue;

    if (textExternalLink.Text.IndexOf("http") < 0 && textExternalLink.Text.Trim() != "")
      textExternalLink.Text = "http://" + textExternalLink.Text;
    Settings.OrganizationDB.WriteString("ExternalPortalLink", textExternalLink.Text.Trim());

    portalOption.Collection.Save();

    Organization organization = (Organization)Organizations.GetOrganization(UserSession.LoginUser, _organizatinID);
    int groupID = int.Parse(cmbGroups.SelectedValue);
    organization.IsPublicArticles = cbArticles.Checked;

    if (groupID < 0) 
      organization.DefaultPortalGroupID = null;
    else
      organization.DefaultPortalGroupID = groupID;
    organization.Collection.Save();



    return true;
  }


    
  
}
