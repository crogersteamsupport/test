using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

public partial class SignUpThanks : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }

  protected void btnContinue_Click(object sender, EventArgs e)
  {
    RadAjaxManager.GetCurrent(Page).Redirect("Default.aspx");
  }

}
