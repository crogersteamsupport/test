using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using TeamSupport.WebUtils;
using TeamSupport.Data;
using Telerik.Web.UI;
using System.Data;

public partial class Frames_VersionInformation : BaseFramePage
{
  private int _versionID;

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);
    try
    {
      _versionID = int.Parse(Request["VersionID"]);
    }
    catch (Exception)
    {
      Response.Write("");
      Response.End();
      return;
    }
  }
  protected void Page_Load(object sender, EventArgs e)
  {
    LoadVersion(_versionID);
  }

  private void LoadVersion(int versionID)
  {
    ProductVersion version = (ProductVersion)ProductVersions.GetProductVersion(UserSession.LoginUser, versionID);
    lblProperties.Visible = version == null;
    if (version == null) return;

    ProductVersionStatus status = (ProductVersionStatus)ProductVersionStatuses.GetProductVersionStatus(UserSession.LoginUser, version.ProductVersionStatusID);

    //lblVersionNumber.Text = versions[0].VersionNumber;
    //lblReleased.Text = versions[0].IsReleased ? "Yes" : "No";
    //lblReleaseDate.Text = versions[0].ReleaseDate != null ? ((DateTime)versions[0].ReleaseDate).ToString("d") : "No release date set.";
    //lblDescription.Text = versions[0].Description;
    //if (!statuses.IsEmpty) lblStatus.Text = statuses[0].Name;


    DataTable table = new DataTable();
    table.Columns.Add("Name");
    table.Columns.Add("Value");

    table.Rows.Add(new string[] { "Version Number:", version.VersionNumber });
    table.Rows.Add(new string[] { "Version Status:", status.Name });
    table.Rows.Add(new string[] { "Version Released:", version.IsReleased ? "Yes" : "No" });
    if (version.IsReleased)
      table.Rows.Add(new string[] { "Release Date:", version.ReleaseDate != null ? ((DateTime)version.ReleaseDate).ToString("d", UserSession.LoginUser.CultureInfo) : "No release date set." });
    else
      table.Rows.Add(new string[] { "Expected Release Date:", version.ReleaseDate != null ? ((DateTime)version.ReleaseDate).ToString("d", UserSession.LoginUser.CultureInfo) : "No release date set." });


    CustomFields fields = new CustomFields(UserSession.LoginUser);
    fields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, ReferenceType.ProductVersions);

    foreach (CustomField field in fields)
    {
      CustomValue value = CustomValues.GetValue(UserSession.LoginUser, field.CustomFieldID, versionID);
      table.Rows.Add(new string[] { field.Name + ":", value.Value });
    }

    table.Rows.Add(new string[] { "Description:", "<div style=\"padding: 10px 20px; \">" + version.Description + "</div>" });

    rptProperties.DataSource = table;
    rptProperties.DataBind();

  }
}
