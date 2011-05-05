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

public partial class Frames_KnowledgeBase : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    string url = "Tickets.aspx?IsKnowledgeBase=1&IsToolBarVisible=0";
    if (Request["ProductID"] != null) url = url + "&ProductID=" + Request["ProductID"];
    ticketsFrame.Attributes["src"] = url;
  }
}
