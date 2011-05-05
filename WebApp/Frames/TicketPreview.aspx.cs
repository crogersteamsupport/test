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
using System.Web.Services;

public partial class Frames_TicketPreview : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    //divTags.Visible = UserSession.LoginUser.UserID == 34;
    try
    {
      int ticketID = int.Parse(Request["TicketID"]);
      Ticket ticket = (Ticket)Tickets.GetTicket(UserSession.LoginUser, ticketID);
      if (ticket.OrganizationID != UserSession.LoginUser.OrganizationID) throw new Exception("Unauthorized ticket ID.");
      LoadHeader(ticketID);
      (ucActions as UserControls_Actions).TicketID = ticketID;
      fieldTicketID.Value = ticketID.ToString();
    }
    catch (Exception ex)
    {
      Response.Write("No ticket to display.");
      Response.End();
    }

    if (UserSession.CurrentUser.ProductType == ProductType.Express || UserSession.CurrentUser.ProductType == ProductType.BugTracking)
    {
      spanCustomers.Visible = false;
    }

    if (UserSession.CurrentUser.ProductType == ProductType.Express || UserSession.CurrentUser.ProductType == ProductType.HelpDesk)
    {
      spanProduct.Visible = false;
      spanReported.Visible = false;
      spanResolved.Visible = false;
    }
  }

  private void LoadHeader(int ticketID)
  {
    lblTicketCaption.Text = "[No Ticket Selected]";
    TicketGridView view = new TicketGridView(UserSession.LoginUser);
    view.LoadByTicketID(ticketID);
    if (view.IsEmpty) return;

    TicketGridViewItem item = view[0];
    lblTicketCaption.Text = item.TicketNumber + ": " + item.Name;
    lblTicketType.Text = item.TicketTypeName;
    lblStatus.Text = item.Status;
    lblSeverity.Text = item.Severity;
    lblUser.Text = item.UserName ?? "[Not Assigned]";
    lblGroup.Text = item.GroupName ?? "[Not Assigned]";
    lblProduct.Text = item.ProductName ?? "[None]";
    lblReproted.Text = item.ReportedVersion ?? "[None]";
    lblSolved.Text = item.SolvedVersion ?? "[None]";

    Organizations organizations = new Organizations(UserSession.LoginUser);
    organizations.LoadByTicketID(ticketID);

    StringBuilder builder = new StringBuilder();
    for (int i = 0; i < organizations.Count; i++)
    {
      builder.Append(organizations[i].Name);
      if (i < organizations.Count - 1) builder.Append(", ");
    }

    lblCustomers.Text = builder.ToString() == "" ? "[None]" : builder.ToString();
  }

  [WebMethod(true)]
  public static TagProxy[] GetTags(int ticketID)
  {
    Tags tags = new Tags(UserSession.LoginUser);
    tags.LoadByReference(ReferenceType.Tickets, ticketID);
    return tags.GetTagProxies();
  }

  [WebMethod(true)]
  public static void DeleteTicketTag(int tagID, int ticketID)
  {
    TagLink link = TagLinks.GetTagLink(UserSession.LoginUser, ReferenceType.Tickets, ticketID, tagID);
    Tag tag = Tags.GetTag(UserSession.LoginUser, tagID);
    int count = tag.GetLinkCount();
    link.Delete();
    link.Collection.Save();
    if (count < 2)
    {
      tag.Delete();
      tag.Collection.Save();
    }
  }

}
