using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class TicketFrame : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    try 
	  {
      if (!IsPostBack)
      {
        string url;
        if (Request["TicketNumber"] != null)
        {
          int ticketNumber = int.Parse(Request["TicketNumber"]);
          url = "Frames/Ticket.aspx?TicketNumber=" + ticketNumber.ToString();
        }
        else
        {
          int ticketID = int.Parse(Request["TicketID"]);
          url = "Frames/Ticket.aspx?TicketID=" + ticketID.ToString();
        }
        frmContent.Attributes["src"] = url;
        //Response.Redirect(url);
      }
	  }
	  catch (Exception)
	  {
		  Response.Write("Invalid ticket id.");
      Response.End();
	  }
  }
}
