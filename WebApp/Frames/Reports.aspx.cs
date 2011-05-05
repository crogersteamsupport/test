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
using System.Data.SqlClient;
using System.Web.Services;

public partial class Frames_Reports : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    Page.Culture = UserSession.LoginUser.CultureInfo.Name;
    
    int newReportID = Settings.Session.ReadInt("NewReportID");
    if (newReportID > -1)
    {
      Settings.Session.WriteInt("NewReportID", -1);
      Settings.UserDB.WriteInt("SelectedReportID", newReportID);
      gridReportList.MasterTableView.ClearSelectedItems();
      gridReportList.Rebind();
    }

    if (!IsPostBack)
    {
      paneGrid.Width = new Unit(Settings.UserDB.ReadInt("ReportsGridWidth", 200), UnitType.Pixel);
      SetToolbar();
    }
  }

  private void SetToolbar()
  {
    RadToolBarButton button = tbUser.FindItemByValue("ExportCSV") as RadToolBarButton;
    //button.NavigateUrl = GetCsvUrl();
    //button.Target = "_blank";
  
  }

  protected void gridReportList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
  {
    Reports reports = new Reports(UserSession.LoginUser);
    reports.LoadAll(UserSession.LoginUser.OrganizationID);
    gridReportList.DataSource = reports.Table;
  }

  protected void gridReportList_DataBound(object sender, EventArgs e)
  {
    if (!SetReportID(Settings.UserDB.ReadInt("SelectedReportID")) && gridReportList.Items.Count > 0)
    {
      gridReportList.Items[0].Selected = true;
      SetToolbar();
    }

    int id = GetReportID();
    Report report = (Report)Reports.GetReport(UserSession.LoginUser, id);
    string url = string.IsNullOrEmpty(report.ExternalURL) ? "ReportResults.aspx" : report.ExternalURL;
    if (report != null) frmReport.Attributes["src"] = url + "?ReportID=" + id.ToString();

    tbUser.Items[1].Enabled = UserSession.CurrentUser.IsSystemAdmin && report.OrganizationID != null && report.OrganizationID == UserSession.LoginUser.OrganizationID;
    tbUser.Items[2].Enabled = tbUser.Items[1].Enabled;
  }

  private int GetReportID()
  {
    if (gridReportList.SelectedItems.Count < 1) return -1;
    GridItem item = gridReportList.SelectedItems[0];
    return (int)item.OwnerTableView.DataKeyValues[item.ItemIndex]["ReportID"]; ;
  }

  private bool SetReportID(int id)
  {
    GridDataItem item = gridReportList.MasterTableView.FindItemByKeyValue("ReportID", id);
    if (item != null)
    {
      item.Selected = true;
      return true;
    }
    else
    {
      return false;
    }
  }
  
  [WebMethod(true)]
  public static string GetCsvUrl()
  {
    //'../dc/1078/reports/95'
    return string.Format("../dc/{0}/reports/{1}", UserSession.LoginUser.OrganizationID.ToString(), Settings.UserDB.ReadInt("SelectedReportID").ToString());
  }

  
}
