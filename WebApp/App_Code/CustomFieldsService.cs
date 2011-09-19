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


    [WebMethod]
    public FieldItem[] GetAllFields(ReferenceType refType, int? auxID)
    {
      List<FieldItem> items = new List<FieldItem>();

      int tableID;
      switch (refType)
      {
        case ReferenceType.Organizations: tableID = 6; break;
        case ReferenceType.Tickets: tableID = 10; break;
        case ReferenceType.Users: tableID = 11; break;
        case ReferenceType.Contacts: tableID = 12; break;
        default: return null;
      }


      ReportTableFields fields = new ReportTableFields(TSAuthentication.GetLoginUser());
      fields.LoadByReportTableID(tableID);

      CustomFields customs = new CustomFields(fields.LoginUser);
      customs.LoadByReferenceType(TSAuthentication.OrganizationID, refType, auxID);

      foreach (ReportTableField field in fields)
      {
        items.Add(new FieldItem(field.ReportTableFieldID, false, field.FieldName));
      }

      foreach (CustomField custom in customs)
      {
        items.Add(new FieldItem(custom.CustomFieldID, true, custom.Name));
      }

      return items.ToArray();
    }

  }

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class FieldItem
  {
    public FieldItem(int id, bool isCustom, string name)
    {
      ID = id;
      IsCustom = isCustom;
      Name = name;
    }

    [DataMember] public int ID { get; set; }
    [DataMember] public bool IsCustom { get; set; }
    [DataMember] public string Name { get; set; }
  }
}