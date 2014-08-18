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
using System.Globalization;

public partial class Dialogs_Organization : BaseDialogPage
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


    if (!IsPostBack)
    {
      LoadDateFormats();
      LoadTimeZones();
      LoadFontFamilies();
      LoadFontSizes();
      LoadWikiArticles();
      LoadSlas();
      LoadUsers(_organizationID);
      LoadOrganization(_organizationID);
    }
  }

  private void LoadDateFormats()
  {
    cmbDateFormat.Items.Clear();
    
    foreach (CultureInfo info in CultureInfo.GetCultures(CultureTypes.AllCultures))
    {
      if (!info.IsNeutralCulture) cmbDateFormat.Items.Add(new RadComboBoxItem(info.DisplayName, info.Name));
    }
    cmbDateFormat.Sort = RadComboBoxSort.Ascending;
    cmbDateFormat.SortItems();
    cmbDateFormat.SelectedValue = "en-US";
  
  }

  private void LoadTimeZones()
  {
    cmbTimeZones.Items.Clear();

    System.Collections.ObjectModel.ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
    foreach (TimeZoneInfo info in timeZones)
    {
      cmbTimeZones.Items.Add(new RadComboBoxItem(info.DisplayName, info.Id));
    }
  }

  private void LoadFontFamilies()
  {
    cmbFontFamily.Items.Clear();

    Array fontFamilyValues = Enum.GetValues(typeof(FontFamily));
    int x = 0;
    foreach (FontFamily value in fontFamilyValues)
    {
      x = (int)value;
      cmbFontFamily.Items.Add(new RadComboBoxItem(Enums.GetDescription(value), x.ToString()));
    }
  }

  private void LoadFontSizes()
  {
    cmbFontSize.Items.Clear();

    Array fontSizeValues = Enum.GetValues(typeof(TeamSupport.Data.FontSize));
    int x = 0;
    foreach (TeamSupport.Data.FontSize value in fontSizeValues)
    {
      x = (int)value;
      cmbFontSize.Items.Add(new RadComboBoxItem(Enums.GetDescription(value), x.ToString()));
    }
  }

  private void LoadWikiArticles()
  {
    WikiArticlesView view = new WikiArticlesView(UserSession.LoginUser);
    view.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
    cmbWikiArticle.Items.Add(new RadComboBoxItem("Unassigned", ""));
    foreach (WikiArticlesViewItem item in view)
    {
      cmbWikiArticle.Items.Add(new RadComboBoxItem(item.ArticleName, item.ArticleID.ToString()));
    }
  
  }

  private void LoadSlas()
  {
    SlaLevels levels = new SlaLevels(UserSession.LoginUser);
    levels.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);

    cmbSla.Items.Clear();
    cmbSla.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
    foreach (SlaLevel level in levels)
    {
      cmbSla.Items.Add(new RadComboBoxItem(level.Name, level.SlaLevelID.ToString()));
    }
    cmbSla.SelectedIndex = 0;
  }

  private void LoadUsers(int organizationID)
  {
    cmbUsers.Items.Clear();

    Users users = new Users(UserSession.LoginUser);
    users.LoadByOrganizationID(organizationID, true);
    cmbUsers.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
    foreach (User user in users)
    {
      cmbUsers.Items.Add(new RadComboBoxItem(user.DisplayName, user.UserID.ToString()));
    }
    cmbUsers.SelectedIndex = 0;


    users = new Users(UserSession.LoginUser);
    users.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, true);
  }

  private void LoadOrganization(int organizationID)
  {
    Organization organization = Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);

    if (organization.OrganizationID != UserSession.LoginUser.OrganizationID)
    {
      Response.Write("Invalid Request");
      Response.End();
      return;
    }

    textWebSite.Text = organization.Website;
    textDomains.Text = organization.CompanyDomains;
    textDescription.Text = organization.Description;
    cbDisableStatusNotifications.Checked = Settings.OrganizationDB.ReadBool("DisableStatusNotification", false);
    cbNewActionsVisible.Checked = organization.SetNewActionsVisibleToCustomers;
    cbUnsecureAttachments.Checked = organization.AllowUnsecureAttachmentViewing;
    cbSlaInitRespAnyAction.Checked = organization.SlaInitRespAnyAction;
    cbShowGroupMembersFirstInTicketAssignmentList.Checked = organization.ShowGroupMembersFirstInTicketAssignmentList;
    cbUpdateTicketChildrenGroupWithParent.Checked = organization.UpdateTicketChildrenGroupWithParent;
    cbHideDismissNonAdmins.Checked = organization.HideDismissNonAdmins;

   // cbCommunity.Checked = organization.UseForums;
    cbRequireCustomer.Checked = Settings.OrganizationDB.ReadBool("RequireNewTicketCustomer", false);
    //cbAdminCustomers.Checked = organization.AdminOnlyCustomers;
    cbTimeRequired.Checked = organization.TimedActionsRequired;
    cbAdminReports.Checked = organization.AdminOnlyReports;
    cmbUsers.SelectedValue = organization.PrimaryUserID.ToString();
    cmbWikiArticle.SelectedValue = organization.DefaultWikiArticleID == null ? "" : organization.DefaultWikiArticleID.ToString();

    if ((organization.ProductType == ProductType.Enterprise || organization.ProductType == ProductType.BugTracking))
    {
        cbRequireProduct.Checked = organization.ProductRequired;
        cbRequireProductVersion.Checked = organization.ProductVersionRequired;
    }
    else
    {
        cbRequireProduct.Visible = false;
        cbRequireProductVersion.Visible = false;
    }

    if (string.IsNullOrEmpty(organization.TimeZoneID))
      cmbTimeZones.SelectedValue = "Central Standard Time";
    else
      cmbTimeZones.SelectedValue = organization.TimeZoneID;

    if (!string.IsNullOrEmpty(organization.CultureName)) cmbDateFormat.SelectedValue = organization.CultureName;

    cmbFontFamily.SelectedValue = Convert.ToInt32(organization.FontFamily).ToString();
    cmbFontSize.SelectedValue = Convert.ToInt32(organization.FontSize).ToString();

    cbLinkTicketCustomersWithProducts.Checked = Settings.OrganizationDB.ReadBool("ShowOnlyCustomerProducts", false);

    timeBDEnd.SelectedDate = organization.BusinessDayEnd;
    timeBDStart.SelectedDate = organization.BusinessDayStart;

    cbBDSunday.Checked = organization.IsDayInBusinessHours(DayOfWeek.Sunday);
    cbBDMonday.Checked = organization.IsDayInBusinessHours(DayOfWeek.Monday);
    cbBDTuesday.Checked = organization.IsDayInBusinessHours(DayOfWeek.Tuesday);
    cbBDWednesday.Checked = organization.IsDayInBusinessHours(DayOfWeek.Wednesday);
    cbBDThursday.Checked = organization.IsDayInBusinessHours(DayOfWeek.Thursday);
    cbBDFriday.Checked = organization.IsDayInBusinessHours(DayOfWeek.Friday);
    cbBDSaturday.Checked = organization.IsDayInBusinessHours(DayOfWeek.Saturday);

    if (organization.InternalSlaLevelID != null) cmbSla.SelectedValue = organization.InternalSlaLevelID.ToString();

  }

  public override bool Save()
  {
    Organization organization = (Organization)Organizations.GetOrganization(UserSession.LoginUser, _organizationID);

    int? id = int.Parse(cmbUsers.SelectedValue);
    organization.PrimaryUserID = id < 0 ? null : id;

    organization.TimeZoneID = cmbTimeZones.SelectedValue;
    UserSession.LoginUser.TimeZoneInfo = null;
    try
    {
      TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(organization.TimeZoneID);
    }
    catch (Exception)
    {
    }
    Settings.OrganizationDB.WriteBool("ShowOnlyCustomerProducts", cbLinkTicketCustomersWithProducts.Checked);
    organization.CultureName = cmbDateFormat.SelectedValue;
    organization.UseEuropeDate = (cmbDateFormat.SelectedValue == "en-GB");

    organization.FontFamily = (FontFamily)Convert.ToInt32(cmbFontFamily.SelectedValue);
    organization.FontSize = (TeamSupport.Data.FontSize)Convert.ToInt32(cmbFontSize.SelectedValue);

    if (cmbSla.SelectedIndex == 0)
      organization.InternalSlaLevelID = null;
    else
      organization.InternalSlaLevelID = int.Parse(cmbSla.SelectedValue);
    Settings.OrganizationDB.WriteBool("DisableStatusNotification", cbDisableStatusNotifications.Checked);
    organization.SetNewActionsVisibleToCustomers = cbNewActionsVisible.Checked;
    organization.AllowUnsecureAttachmentViewing = cbUnsecureAttachments.Checked;
    organization.SlaInitRespAnyAction = cbSlaInitRespAnyAction.Checked;
    organization.ShowGroupMembersFirstInTicketAssignmentList = cbShowGroupMembersFirstInTicketAssignmentList.Checked;
    organization.HideDismissNonAdmins = cbHideDismissNonAdmins.Checked;
    organization.UpdateTicketChildrenGroupWithParent = cbUpdateTicketChildrenGroupWithParent.Checked;
    //organization.UseForums = cbCommunity.Checked;
    Settings.OrganizationDB.WriteBool("RequireNewTicketCustomer", cbRequireCustomer.Checked);
    //organization.AdminOnlyCustomers = cbAdminCustomers.Checked;
    organization.TimedActionsRequired = cbTimeRequired.Checked;
    organization.AdminOnlyReports = cbAdminReports.Checked;
    organization.Website = textWebSite.Text;
    organization.CompanyDomains = textDomains.Text;
    organization.Description = textDescription.Text;
    if (timeBDStart.SelectedDate != null)
      organization.BusinessDayStart = (DateTime) DataUtils.DateToUtc(UserSession.LoginUser, timeBDStart.SelectedDate);
    if (timeBDEnd.SelectedDate != null)
      organization.BusinessDayEnd = (DateTime) DataUtils.DateToUtc(UserSession.LoginUser, timeBDEnd.SelectedDate);
    organization.ClearBusinessDays();
    if (cbBDSunday.Checked) organization.AddBusinessDay(DayOfWeek.Sunday);
    if (cbBDMonday.Checked) organization.AddBusinessDay(DayOfWeek.Monday);
    if (cbBDTuesday.Checked) organization.AddBusinessDay(DayOfWeek.Tuesday);
    if (cbBDWednesday.Checked) organization.AddBusinessDay(DayOfWeek.Wednesday);
    if (cbBDThursday.Checked) organization.AddBusinessDay(DayOfWeek.Thursday);
    if (cbBDFriday.Checked) organization.AddBusinessDay(DayOfWeek.Friday);
    if (cbBDSaturday.Checked) organization.AddBusinessDay(DayOfWeek.Saturday);

    organization.ProductRequired = cbRequireProduct.Checked;
    organization.ProductVersionRequired = cbRequireProductVersion.Checked;

    try
    {
      organization.DefaultWikiArticleID = int.Parse(cmbWikiArticle.SelectedValue);
    }
    catch (Exception)
    {
      organization.DefaultWikiArticleID = null;
    }

    organization.Collection.Save();

    UserSession.RefreshCurrentUserInfo();
    UserSession.RefreshLoginUser();
    return true;
  }


  protected void btnResetEmail_Click(object sender, EventArgs e)
  {
    Organization organization = Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
    organization.SystemEmailID = Guid.NewGuid();
    organization.Collection.Save();
  }
}
