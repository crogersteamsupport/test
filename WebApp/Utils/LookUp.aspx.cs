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

public partial class Utils_LookUp : System.Web.UI.Page
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

    [WebMethod(true)]
    public static AutocompleteItem[] GetOrganizations(string name)
    {
      List<AutocompleteItem> result = new List<AutocompleteItem>();
      Organizations organizations = new Organizations(UserSession.LoginUser);
      organizations.LoadByLikeOrganizationName(1, name, false, 20);
      foreach (Organization organization in organizations)
      {
        result.Add(new AutocompleteItem(organization.Name, organization.OrganizationID.ToString()));
      } 

      return result.ToArray();
    }

    [DataContract]
    public class AutocompleteItem
    {
      public AutocompleteItem() { }

      public AutocompleteItem(string label, string id)
      {
        this.label = label;
        this.value = label;
        this.id = id;
      }

      public AutocompleteItem(string label, string value, string id)
      {
        this.label = label;
        this.value = value;
        this.id = id;
      }

      [DataMember]
      public string label { get; set; }
      [DataMember]
      public string value { get; set; }
      [DataMember]
      public string id { get; set; }
    }
}