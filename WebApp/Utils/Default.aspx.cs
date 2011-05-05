using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Web.Services;
using System.Runtime.Serialization;

public partial class Utils_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
      if (TSAuthentication.OrganizationID != 1078)
      {
        Response.StatusCode = 404;
        Response.End();
        return;
      }

    }
}