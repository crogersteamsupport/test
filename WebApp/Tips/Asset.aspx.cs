using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Text;

public partial class Tips_Asset : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
      if (Request["AssetID"] == null) EndResponse("Invalid Asset");

      int assetID = int.Parse(Request["AssetID"]);
      Asset asset = Assets.GetAsset(TSAuthentication.GetLoginUser(), assetID);
      if (asset == null) EndResponse("Invalid Asset");

      if (asset.OrganizationID != TSAuthentication.OrganizationID) EndResponse("Invalid Asset");
      
      tipAsset.InnerText = asset.Name;
      tipAsset.Attributes.Add("onclick", "top.Ts.MainPage.openAsset(" + assetID.ToString() + "); return false;");

      /*
      StringBuilder props = new StringBuilder();
      if (!string.IsNullOrEmpty(asset.Name))
      {
        props.Append(string.Format("<dt>Website</dt><dd><a target=\"_blank\" href=\"{0}\">{0}</a></dd>", organization.Website));
      }

      if (organization.SAExpirationDate != null)
      {
        string css = organization.SAExpirationDate <= DateTime.UtcNow ? "tip-customer-expired" : "";
        props.Append(string.Format("<dt>Service Expiration</dt><dd class=\"{0}\">{1:D}</dd>", css, (DateTime)organization.SAExpirationDate));
      }

      PhoneNumbersView numbers = new PhoneNumbersView(organization.Collection.LoginUser);
      numbers.LoadByID(organization.OrganizationID, ReferenceType.Organizations);

      foreach (PhoneNumbersViewItem number in numbers)
      {
        props.Append(string.Format("<dt>{0}</dt><dd>{1} {2}</dd>", number.PhoneType, number.PhoneNumber, number.Extension));
      }

      tipProps.InnerHtml = props.ToString();
       */
    }

    private void EndResponse(string message)
    {
      Response.Write(message);
      Response.End();
    }
}