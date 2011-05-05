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
using Telerik.Web.UI;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Text;

public partial class Frames_Wikis : BaseFramePage
{
  string _searchText = "";
  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    RadAjaxManager manager = RadAjaxManager.GetCurrent(Page);
    manager.AjaxSettings.AddAjaxSetting(gridWikis, gridWikis);

    if (Request["SearchText"] != null)
    {
      _searchText = Request["SearchText"];
    }
    if (!IsPostBack)
    {
      gridWikis.Rebind();
    }
  }


  protected void gridWikis_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
  {
    WikiArticlesView view = new WikiArticlesView(UserSession.LoginUser);
    view.LoadForGrid(0, 10000, UserSession.LoginUser.OrganizationID, UserSession.LoginUser.UserID, _searchText, "ModifiedDate", false);
    gridWikis.DataSource = view;

  }
  protected void gridWikis_ItemDataBound(object sender, GridItemEventArgs e)
  {
    if (e.Item is GridDataItem)
    {
      GridDataItem item = (GridDataItem)e.Item;

      string key = item.GetDataKeyValue("ArticleID").ToString();
      ImageButton button = (ImageButton)item["ButtonOpen"].Controls[0];
      button.OnClientClick = "OpenWiki('" + key + "'); return false;";
    }

  }
}
