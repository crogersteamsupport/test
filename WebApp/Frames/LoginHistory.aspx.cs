using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;

public partial class Frames_LoginHistory : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }
  protected void gridUsers_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
  {
    LoginHistory history = new LoginHistory(UserSession.LoginUser);
    history.LoadForGrid();
    gridUsers.DataSource = history.Table;
  }
}
