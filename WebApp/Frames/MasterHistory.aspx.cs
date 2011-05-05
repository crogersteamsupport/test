using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;
using System.Text;

public partial class Frames_MasterHistory : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    RadAjaxManager1.AjaxSettings.AddAjaxSetting(gridActionLogs, gridActionLogs);

  }
  protected void gridActionLogs_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
  {
    ActionLogs actionLogs = new ActionLogs(UserSession.LoginUser);
    actionLogs.LoadAll();

    gridActionLogs.VirtualItemCount = actionLogs.Count;
    gridActionLogs.DataSource = actionLogs.Table;
  }
}
