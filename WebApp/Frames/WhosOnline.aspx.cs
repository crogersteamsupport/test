using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.WebUtils;
using TeamSupport.Data;

public partial class Frames_WhosOnline : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }

  protected void gridUsers_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
  {
    Users users = new Users(UserSession.LoginUser);
    users.LoadByOnline();
    gridUsers.DataSource = users.Table;
  }
}
