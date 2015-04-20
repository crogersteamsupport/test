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

      StringBuilder props = new StringBuilder();
      if (!string.IsNullOrEmpty(asset.SerialNumber)) { props.Append(string.Format("<dt>{0}</dt><dd>{1}</dd>", "Serial Number", asset.SerialNumber)); }
      if (!string.IsNullOrEmpty(asset.Location)) { props.Append(string.Format("<dt>{0}</dt><dd>{1}</dd>", "Location", asset.Location)); }
      switch (asset.Location.Trim())
      {
        case "1": props.Append("<dt>Location</dt><dd>Assigned</dd>"); break;
        case "2": props.Append("<dt>Location</dt><dd>Warehouse</dd>"); break;
        case "3": props.Append("<dt>Location</dt><dd>Junkyard</dd>"); break;
        default: props.Append("<dt>Location</dt><dd>Unknown</dd>"); break;
      }
      if (!string.IsNullOrEmpty(asset.Status)) { props.Append(string.Format("<dt>{0}</dt><dd>{1}</dd>", "Status", asset.Status)); }
      if (asset.WarrantyExpiration != null) { props.Append(string.Format("<dt>{0:d}</dt><dd>{1}</dd>", "Warranty Expiration", (DateTime)asset.WarrantyExpiration)); }

      if (asset.ProductID != null)
      {
        Product product = Products.GetProduct(TSAuthentication.GetLoginUser(), (int) asset.ProductID);
        if (product != null) props.Append(string.Format("<dt>{0}</dt><dd>{1}</dd>", "Product", product.Name));
      }

      if (asset.AssignedTo != null)
      {
        User user = Users.GetUser(TSAuthentication.GetLoginUser(), (int)asset.AssignedTo);
        if (user != null) props.Append(string.Format("<dt>{0}</dt><dd>{1}</dd>", "Assigned To", user.FirstLastName));
      }
     

      if (!string.IsNullOrEmpty(asset.Notes)) { props.Append(string.Format("<dt>{0}</dt><dd>{1}</dd>", "Notes", asset.Notes)); }

      tipProps.InnerHtml = props.ToString();
    }

    private void EndResponse(string message)
    {
      Response.Write(message);
      Response.End();
    }
}