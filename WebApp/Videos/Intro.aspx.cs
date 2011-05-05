using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;

public partial class Videos_Intro : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    litVideo.Text = Settings.SystemDB.ReadString("IntroVideoLink");
  }
}
