using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Text;

public partial class Resources_144_Pages_Tips_Customer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
      if (Request["CustomerID"] == null) EndResponse("Invalid Customer");

      int organizationID = int.Parse(Request["CustomerID"]);
      Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), organizationID);
      if (organization == null) EndResponse("Invalid Customer");

      if (organization.OrganizationID != TSAuthentication.OrganizationID && organization.ParentID != TSAuthentication.OrganizationID) EndResponse("Invalid Customer");
      
      tipCompany.InnerText = organization.Name;
      tipCompany.Attributes.Add("onclick", "top.Ts.MainPage.openCustomer(" + organizationID.ToString() + "); return false;");

      StringBuilder props = new StringBuilder();
      if (!string.IsNullOrEmpty(organization.Website))
      {
        props.Append(string.Format("<dt>Website</dt><dd><a target=\"_blank\" href=\"{0}\">{0}</a></dd>", organization.Website));
      }

      PhoneNumbersView numbers = new PhoneNumbersView(organization.Collection.LoginUser);
      numbers.LoadByID(organization.OrganizationID, ReferenceType.Organizations);

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