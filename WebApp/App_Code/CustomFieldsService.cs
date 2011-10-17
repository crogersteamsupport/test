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
using Newtonsoft.Json;

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
    public FieldItem[] GetAllFields(ReferenceType refType, int? auxID, bool isReadOnly)
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
      fields.LoadByReportTableID(tableID, isReadOnly);

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

    [WebMethod]
    public CustomFieldProxy[] GetCustomFields(ReferenceType refType, int? auxID)
    {
      CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
      fields.LoadByReferenceType(TSAuthentication.OrganizationID, refType, auxID);
      return fields.GetCustomFieldProxies();
      
    }

    [WebMethod]
    public CustomFieldCategoryProxy[] GetCategories(ReferenceType refType, int? auxID)
    {
      CustomFieldCategories cats = new CustomFieldCategories(TSAuthentication.GetLoginUser());
      cats.LoadByRefType(refType, auxID);
      return cats.GetCustomFieldCategoryProxies();
    }

    public bool IsDuplicateCategory(int? categoryID, ReferenceType refType, int? auxID, string text)
    {
      text = text.Trim();
      CustomFieldCategories cats = new CustomFieldCategories(TSAuthentication.GetLoginUser());
      cats.LoadByRefType(refType, auxID);

      foreach (CustomFieldCategory item in cats)
      {
        if (categoryID != null && item.CustomFieldCategoryID == categoryID) continue;

        if (item.Category.Trim().ToLower() == text.ToLower())
        {
          return true;
        }
      }

      return false;
    }

    [WebMethod]
    public CustomFieldCategoryProxy NewCategory(ReferenceType refType, int? auxID, string text)
    {
      if (!TSAuthentication.IsSystemAdmin) return null;
      CustomFieldCategory cat = (new CustomFieldCategories(TSAuthentication.GetLoginUser()).AddNewCustomFieldCategory());
      cat.Category = text.Trim();
      cat.Position = CustomFieldCategories.GetMaxPosition(TSAuthentication.GetLoginUser(), refType, auxID) +1;
      cat.OrganizationID = TSAuthentication.OrganizationID;
      cat.AuxID = auxID;
      cat.RefType = refType;
      cat.Collection.Save();
      return cat.GetProxy();
    }

    [WebMethod]
    public CustomFieldCategoryProxy SaveCategory(int categoryID, string text)
    {
      if (!TSAuthentication.IsSystemAdmin) return null;
      CustomFieldCategory cat = CustomFieldCategories.GetCustomFieldCategory(TSAuthentication.GetLoginUser(), categoryID);
      if (cat.OrganizationID != TSAuthentication.OrganizationID) return null;
      cat.Category = text.Trim();
      cat.Collection.Save();
      return cat.GetProxy();
    }

    [WebMethod]
    public void DeleteCategory(int categoryID)
    {
      if (!TSAuthentication.IsSystemAdmin) return;
      CustomFieldCategory cat = CustomFieldCategories.GetCustomFieldCategory(TSAuthentication.GetLoginUser(), categoryID);
      if (cat.OrganizationID != TSAuthentication.OrganizationID) return;
      cat.Delete();
      cat.Collection.Save();
    }

    [WebMethod]
    public void DeleteCustomField(int fieldID)
    {
      if (!TSAuthentication.IsSystemAdmin) return;
      CustomField field = CustomFields.GetCustomField(TSAuthentication.GetLoginUser(), fieldID);
      field.Delete();
      field.Collection.Save();
    }
 
   [WebMethod]
   public CategoryOrder SaveOrder(string data)
   {
     List<CategoryOrder> orders = JsonConvert.DeserializeObject<List<CategoryOrder>>(data);

     if (!TSAuthentication.IsSystemAdmin) return null;
     CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
     fields.LoadByOrganization(TSAuthentication.OrganizationID);

     CustomFieldCategories cats = new CustomFieldCategories(TSAuthentication.GetLoginUser());
     cats.LoadByOrganization(TSAuthentication.OrganizationID);



     int x = 0;

     foreach (CategoryOrder order in orders)
     {
       if (order.CatID != null)
       {
         cats.FindByCustomFieldCategoryID((int)order.CatID).Position = x;
         x++;
       }
       
       int y = 0;
       foreach (int fieldID in order.FieldIDs)
       {
         CustomField field = fields.FindByCustomFieldID(fieldID);
         field.Position = y;
         y++;
         field.CustomFieldCategoryID = order.CatID;
       }
     }
     fields.Save();
     cats.Save();
     return null;
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

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class CategoryOrder
  {
    public CategoryOrder() {
    }
    [DataMember] public int? CatID {get; set;}
    [DataMember] public List<int> FieldIDs {get; set;}
  }

  

}