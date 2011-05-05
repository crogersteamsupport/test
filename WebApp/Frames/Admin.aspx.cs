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
using System.Text;

public partial class Frames_Admin : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!IsPostBack)
    {
      tsMain.SelectedIndex = Settings.UserDB.ReadInt("SelectedAdminTabIndex", 0);
      if (tsMain.SelectedTab == null) tsMain.SelectedIndex = 0;
      frmAdmin.Attributes["src"] = tsMain.SelectedTab.Value;
      tsMain.FindTabByText("Ticket Automation").Visible = UserSession.LoginUser.OrganizationID == 1078 || UserSession.LoginUser.OrganizationID == 13679 || UserSession.LoginUser.OrganizationID == 1088 || UserSession.LoginUser.OrganizationID == 2706 || UserSession.LoginUser.OrganizationID == 294204 || UserSession.LoginUser.OrganizationID == 362372;
    }

    //tsMain.Tabs[1].Visible = UserSession.CurrentUser.IsFinanceAdmin;
  }
}
