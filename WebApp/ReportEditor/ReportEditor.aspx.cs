using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;
using System.Xml;
using System.Text;
using System.IO;

public partial class ReportEditor_ReportEditor : System.Web.UI.Page
{
  int _reportID = -1;


  protected void Page_Load(object sender, EventArgs e)
  {

    Page.Culture = UserSession.LoginUser.OrganizationCulture.Name;

    if (Request["ReportID"] != null)
    {
      _reportID = int.Parse(Request["ReportID"]);
    }
   
    if (!IsPostBack)
    {
      mpMain.SelectedIndex = 0;

      if (_reportID > -1) 
        LoadReport();
      else
      {
        LoadCombos();
        LoadFields();
        filterControl.ReportSubcategoryID = int.Parse(cmbSubCat.SelectedValue);

      }
    }
  }
  
  protected void btnBack_Click(object sender, EventArgs e)
  {
    SetPage(false);
  }
  
  protected void btnNext_Click(object sender, EventArgs e)
  {
    if (mpMain.SelectedIndex == 2)
    {
      SaveReport();
      return;
    }

    SetPage(true);
    
  }

  private void SaveReport()
  { 
    Report report;
    if (_reportID < 0)
    {
      report = (new Reports(UserSession.LoginUser)).AddNewReport();
      report.OrganizationID = UserSession.LoginUser.OrganizationID;
    }
    else
    {
      report = (Report)Reports.GetReport(UserSession.LoginUser, _reportID);
      /// Ticket 9540. Not sure why did the call below was made. It seems to be redundant with the call made after the SaveFields method is invoked.
      //ReportData.DeleteReportData(UserSession.LoginUser, _reportID);
    }

    report.Name = textName.Text;
    report.QueryObject = DataUtils.ObjectToString(filterControl.ReportConditions);
    report.ReportSubcategoryID = int.Parse(cmbSubCat.SelectedValue);

    report.Collection.Save();

    SaveFields(report.ReportID);
    /// Ticket 9540. It seems that by design the ReportData was deleted after it was updated.
    /// This seemed to be a good idea, but after the user comments on this ticket and thinking twice about it, it seems better to persist it.
    //ReportData.DeleteReportData(UserSession.LoginUser, report.ReportID);

    Settings.UserDB.WriteInt("SelectedReportID", report.ReportID);
    Settings.Session.WriteInt("NewReportID", report.ReportID);

    ActionLogs.AddActionLog(UserSession.LoginUser, ActionLogType.Update, ReferenceType.Reports, report.ReportID, string.Format(
      "{0} updated report \"{1}\" ({2})",
      Users.GetUserFullName(UserSession.LoginUser, UserSession.LoginUser.UserID), report.Name, report.ReportID));

    CloseWindow(report.ReportID);
  }

  private void SaveFields(int reportID)
  {
    ReportFields fields = new ReportFields(UserSession.LoginUser);
    fields.ClearReportFields(reportID);

    foreach (ListItem item in cblFields.Items)
    {
      if (!item.Selected) continue;

      bool isCustomField = IsValueCustomField(item.Value);
      int id = GetValueFieldID(item.Value);

      ReportField field = fields.AddNewReportField();
      field.ReportID = reportID;
      field.LinkedFieldID = id;
      field.IsCustomField = isCustomField;
    }
    fields.Save();

  }

  private void CloseWindow(int reportID)
  {
    DynamicScript.ExecuteScript(Page, "CloseDialog", "Close(" + reportID.ToString() + ");");
  }
  
  private void SetPage(bool isForward)
  {
    if (isForward)
    {
      switch (mpMain.SelectedIndex)
      {
        case 0:
          if (textName.Text.Trim() == "")
          {
            RadAjaxManager1.Alert("Please choose a name for this report.");
            return;
          }
          break;
        case 1:
          if (cblFields.SelectedIndex < 0)
          {
            RadAjaxManager1.Alert("Please select some fields for this report.");
            return;
          }
          break;
        default:
          break;
      }
    }


    btnBack.Enabled = true;
    btnNext.Text = "Next >>";

    if (isForward)
    {
      if (mpMain.SelectedIndex > 1) return;
      mpMain.SelectedIndex = mpMain.SelectedIndex + 1;
      if (mpMain.SelectedIndex == 2) btnNext.Text = "Finish";
    }
    else
    {
      if (mpMain.SelectedIndex < 1) return;
      mpMain.SelectedIndex = mpMain.SelectedIndex - 1;
      if (mpMain.SelectedIndex == 0) btnBack.Enabled = false;
    }
  }
  
  protected void btnCancel_Click(object sender, EventArgs e)
  {
    CloseWindow(-1);
  }

  private void LoadCombos()
  {
    LoadCategories();
    LoadSubCat(int.Parse(cmbCategory.SelectedValue));
  }
  
  private void LoadReport()
  {
    Report report = (Report)Reports.GetReport(UserSession.LoginUser, _reportID);
    textName.Text = report.Name;
    LoadCategories();
    if (report != null && report.ReportSubcategoryID != null)
    {
      ReportSubcategory sub = (ReportSubcategory)ReportSubcategories.GetReportSubcategory(UserSession.LoginUser, (int)report.ReportSubcategoryID);
      cmbCategory.SelectedValue = sub.ReportCategoryTableID.ToString();
    }
    else
    {
      cmbCategory.SelectedIndex = 0;
    }

    LoadSubCat(int.Parse(cmbCategory.SelectedValue));

    if (report != null && report.ReportSubcategoryID != null)
    {
      ReportSubcategory sub = (ReportSubcategory)ReportSubcategories.GetReportSubcategory(UserSession.LoginUser, (int)report.ReportSubcategoryID);
      cmbSubCat.SelectedValue = sub.ReportSubcategoryID.ToString();
    }
    else
    {
      cmbSubCat.SelectedIndex = 0;
    }

    LoadFields();
    
    SetSelectedFields();

    filterControl.ReportSubcategoryID = int.Parse(cmbSubCat.SelectedValue);
    filterControl.ReportConditions = (ReportConditions)DataUtils.StringToObject(report.QueryObject);

  }

  private void LoadCategories()
  {
    ReportTables tables = new ReportTables(UserSession.LoginUser);
    tables.LoadCategories();

    cmbCategory.Items.Clear();
    foreach (ReportTable table in tables)
    {
      cmbCategory.Items.Add(new RadComboBoxItem(table.Alias, table.ReportTableID.ToString()));
    } 
    cmbCategory.SelectedIndex = 0;
  }
  
  private void LoadSubCat(int reportTableID)
  {
    cmbSubCat.Items.Clear();
    ReportSubcategories sub = new ReportSubcategories(UserSession.LoginUser);
    sub.LoadByReportTableID(reportTableID);
    foreach (ReportSubcategory item in sub)
    {
      if (item.ReportTableID != null)
        cmbSubCat.Items.Add(new RadComboBoxItem(item.Row["Alias"].ToString(), item.ReportSubcategoryID.ToString()));
      else
        cmbSubCat.Items.Add(new RadComboBoxItem("[None]", item.ReportSubcategoryID.ToString()));
    }
    cmbSubCat.SelectedIndex = 0;
  }

  protected void cmbCategory_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
  {
    LoadSubCat(int.Parse(cmbCategory.SelectedValue));
    cmbSubCat.SelectedIndex = 0;
    filterControl.ReportSubcategoryID = int.Parse(cmbSubCat.SelectedValue);
    LoadFields();

  }

  protected void cmbSubCat_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
  {
    filterControl.ReportSubcategoryID = int.Parse(cmbSubCat.SelectedValue);
    LoadFields();
  }

  private void LoadFields()
  {
    cblFields.Items.Clear();
    int primaryTableID = int.Parse(cmbCategory.SelectedValue);
    int secondaryTableID = -1;
    
    TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
    ticketTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);
    
    if (cmbSubCat.SelectedIndex > 0)
    {
      ReportSubcategory sub = (ReportSubcategory)ReportSubcategories.GetReportSubcategory(UserSession.LoginUser, int.Parse(cmbSubCat.SelectedValue));
      if (sub.ReportTableID != null) secondaryTableID = (int)sub.ReportTableID;
    }

    ReportTableFields fields = new ReportTableFields(UserSession.LoginUser);
    fields.LoadByReportTableID(primaryTableID);
    foreach (ReportTableField field in fields)
    {
      cblFields.Items.Add(new ListItem(field.Row["TableAlias"].ToString() + " :: " + field.Alias, "R" + field.ReportTableFieldID.ToString()));
    }

    ReportTable table = (ReportTable)ReportTables.GetReportTable(UserSession.LoginUser, primaryTableID);
    if (table.CustomFieldRefType != ReferenceType.None)
    {
      CustomFields customFields = new CustomFields(UserSession.LoginUser);
      customFields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, (ReferenceType)table.CustomFieldRefType);

      foreach (CustomField customField in customFields)
      {
        if (customField.RefType == ReferenceType.Tickets || customField.RefType == ReferenceType.Actions)
        {
          TicketType ticketType = ticketTypes.FindByTicketTypeID(customField.AuxID);
          if (ticketType != null)
            cblFields.Items.Add(new ListItem(table.Alias + " :: " + customField.Name + " (" + ticketType.Name + ")", "C" + customField.CustomFieldID.ToString()));
        }
        else
          cblFields.Items.Add(new ListItem(table.Alias + " :: " + customField.Name, "C" + customField.CustomFieldID.ToString()));
      }
    }

    if (secondaryTableID > -1)
    {
      fields = new ReportTableFields(UserSession.LoginUser);
      fields.LoadByReportTableID(secondaryTableID);
      foreach (ReportTableField field in fields)
      {
        cblFields.Items.Add(new ListItem(field.Row["TableAlias"].ToString() + " :: " + field.Alias, "R" + field.ReportTableFieldID.ToString()));
      }

      table = (ReportTable)ReportTables.GetReportTable(UserSession.LoginUser, secondaryTableID);
      if (table.CustomFieldRefType !=  ReferenceType.None)
      {
        CustomFields customFields = new CustomFields(UserSession.LoginUser);
        customFields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, (ReferenceType)table.CustomFieldRefType);

        foreach (CustomField customField in customFields)
        {
          if (customField.RefType == ReferenceType.Tickets || customField.RefType == ReferenceType.Actions)
          {
            TicketType ticketType = ticketTypes.FindByTicketTypeID(customField.AuxID);
            if (ticketType != null)
              cblFields.Items.Add(new ListItem(table.Alias + " :: " + customField.Name + " (" + ticketType.Name + ")", "C" + customField.CustomFieldID.ToString()));
          }
          else
            cblFields.Items.Add(new ListItem(table.Alias + " :: " + customField.Name, "C" + customField.CustomFieldID.ToString()));
        }
      }
    }
  }

  private void SetSelectedFields()
  {
    if (_reportID < 0) return;
    ReportFields fields = new ReportFields(UserSession.LoginUser);
    fields.LoadByReportID(_reportID);

    foreach (ReportField field in fields)
    {
      foreach (ListItem item in cblFields.Items)
      {
        bool isCustomField = IsValueCustomField(item.Value);
        int id = GetValueFieldID(item.Value);
        if (field.IsCustomField == isCustomField && field.LinkedFieldID == id)
        {
          item.Selected = true;
        }


      }
    }
  }

  private bool IsValueCustomField(string value)
  {
    if (value.Length < 1) return false;
    return value[0] == 'C';
  }

  private int GetValueFieldID(string value)
  {
    if (value.Length < 1) return -1;
    string s = value.Substring(1, value.Length - 1);
    try
    {
      return int.Parse(s);
    }
    catch (Exception)
    {

      return -1;
    }

  }

}
