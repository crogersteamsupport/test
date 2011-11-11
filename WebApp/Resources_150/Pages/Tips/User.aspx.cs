using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Text;

public partial class Resources_150_Pages_Tips_User : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
      if (Request["UserID"] == null) EndResponse("Invalid User");
      int? ticketID = null;
      if (Request["TicketID"] != null)
      {
        int id;
        if (int.TryParse(Request["TicketID"], out id))
        {
          ticketID = id;
        }
      }

      int userID = int.Parse(Request["UserID"]);
      User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
      if (user == null) EndResponse("Invalid User");
      Organization organization = Organizations.GetOrganization(user.Collection.LoginUser, user.OrganizationID);
      if (user.OrganizationID != TSAuthentication.OrganizationID && organization.ParentID != TSAuthentication.OrganizationID) EndResponse("Invalid User");
      tipName.InnerText = user.FirstLastName;

      if (user.OrganizationID == TSAuthentication.OrganizationID)
      {
        tipName.Attributes.Add("onclick", "top.Ts.MainPage.openUser("+user.UserID.ToString()+"); return false;");
        tipCompany.Visible = false;
      }
      else
      {
        tipName.Attributes.Add("onclick", "top.Ts.MainPage.openContact(" + user.UserID.ToString() + "," + user.OrganizationID.ToString() +"); return false;");
        tipCompany.Visible = true;
      }

      tipCompany.InnerText = organization.Name;
      tipCompany.Attributes.Add("onclick", "top.Ts.MainPage.openCustomer(" + user.OrganizationID.ToString() + "); return false;");
      if (!string.IsNullOrEmpty(user.Title)) tipTitle.InnerHtml = user.Title + ", ";

      StringBuilder props = new StringBuilder();
      if (!string.IsNullOrEmpty(user.Email))
      {
        if (ticketID != null)
        {
          props.Append(string.Format("<dt>Email</dt><dd><a href=\"{0}\">{1}</a></dd>", DataUtils.GetMailLinkHRef(user.Collection.LoginUser, userID, (int)ticketID), user.Email));
        }
        else
	      {
          props.Append(string.Format("<dt>Email</dt><dd><a href=\"mailto:{0}\">{0}</a></dd>", user.Email));
	      }
      }

      PhoneNumbersView numbers = new PhoneNumbersView(user.Collection.LoginUser);
      numbers.LoadByID(user.UserID, ReferenceType.Users);

      foreach (PhoneNumbersViewItem number in numbers)
      {
        props.Append(string.Format("<dt>{0}</dt><dd>{1} {2}</dd>", number.PhoneType, number.PhoneNumber, number.Extension));
      }

      tipProps.InnerHtml = props.ToString();
    }

    private void EndResponse(string message)
    {
      Response.Write(message);
      Response.End();
    }
}