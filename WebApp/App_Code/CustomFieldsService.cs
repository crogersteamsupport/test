using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using System.Runtime.Serialization;

namespace TSWebServices
{
  [ScriptService]
  [WebService(Namespace = "http://teamsupport.com/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class CustomFieldsService : System.Web.Services.WebService
  {

    public CustomFieldsService()
    {
      //Uncomment the following line if using designed components 
      //InitializeComponent(); 
    }

    [WebMethod]
    public CustomValueProxy[] GetValues(ReferenceType refType, int id)
    {
      CustomValues values = new CustomValues(TSAuthentication.GetLoginUser());
      values.LoadByReferenceType(TSAuthentication.OrganizationID, refType, id);
      return values.GetCustomValueProxies();
    }

  }
}