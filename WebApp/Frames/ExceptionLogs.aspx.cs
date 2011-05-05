using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;

public partial class Frames_ExceptionLogs : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }
  protected void gridExceptions_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
  {
    ExceptionLogView view = new ExceptionLogView(UserSession.LoginUser);
    view.LoadAll();
    gridExceptions.DataSource = view.Table;
  }
}
