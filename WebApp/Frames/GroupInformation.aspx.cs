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
using System.Data;

public partial class Frames_GroupInformation : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    int groupID = int.Parse(Request["GroupID"]);

    btnAddUser.Visible = UserSession.CurrentUser.IsSystemAdmin;
    if (btnAddUser.Visible) btnAddUser.OnClientClick = "ShowDialog(top.GetSelectUserDialog(" + groupID.ToString() + ", 6)); return false;";

    if (UserSession.CurrentUser.IsSystemAdmin)
    {
      pnlUsers.Attributes.Add("class", "");
    }
    else
    {
      pnlUsers.Attributes.Add("class", "adminDiv");
    }

    LoadUsers(groupID);

  }

  private void LoadUsers(int groupID)
  {
    DataTable table = new DataTable();
    table.Columns.Add("UserID", Type.GetType("System.Int32"));
    table.Columns.Add("GroupID", Type.GetType("System.Int32"));
    table.Columns.Add("Name", Type.GetType("System.String"));

    Users users = new Users(UserSession.LoginUser);
    users.LoadByGroupID(groupID);

    foreach (User user in users)
    {
      table.Rows.Add(new object[] { user.UserID, groupID, user.LastName + ", " + user.FirstName });
    }
    rptUsers.DataSource = table;
    rptUsers.DataBind();

  }
}
