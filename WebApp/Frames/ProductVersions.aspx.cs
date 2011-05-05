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

public partial class Frames_ProductVersions : BaseFramePage
{
  private int _productID;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    try
    {
      _productID = int.Parse(Request["ProductID"]);
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

  }
  protected void gridVersions_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
  {
    ProductVersions versions = new ProductVersions(UserSession.LoginUser);
    versions.LoadForGrid(_productID);
    gridVersions.DataSource = versions.Table;
  }
}
