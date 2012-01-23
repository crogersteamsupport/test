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
using dtSearch.Engine;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace TSWebServices
{
  [ScriptService]
  [WebService(Namespace = "http://teamsupport.com/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class AdminService : System.Web.Services.WebService
  {
    
    public AdminService()
    {

      //Uncomment the following line if using designed components 
      //InitializeComponent(); 
    }

    [WebMethod]
    public ForumCategoryInfo[] GetForumCategories()
    {
      List<ForumCategoryInfo> result = new List<ForumCategoryInfo>();
      ForumCategories cats = new ForumCategories(TSAuthentication.GetLoginUser());
      cats.LoadCategories(TSAuthentication.OrganizationID);

      foreach (ForumCategory cat in cats)
      {
        ForumCategoryInfo info = new ForumCategoryInfo();
        info.Category = cat.GetProxy();

        ForumCategories subs = new ForumCategories(cats.LoginUser);
        subs.LoadSubcategories(cat.CategoryID);
        info.Subcategories = subs.GetForumCategoryProxies();

        result.Add(info);
      }

      return result.ToArray();
    }

    [WebMethod]
    public ForumCategoryProxy UpdateForumCategory(int categoryID, string name, string description, int? ticketTypeID, int? groupID, int? productID)
    {
      if (!TSAuthentication.IsSystemAdmin) return null;
      ForumCategory cat = ForumCategories.GetForumCategory(TSAuthentication.GetLoginUser(), categoryID);
      if (int.Parse(cat.OrganizationID) != TSAuthentication.OrganizationID) return null;
      cat.CategoryName = name;
      cat.CategoryDesc = description;
      cat.TicketType = ticketTypeID;
      cat.GroupID = groupID;
      cat.ProductID = productID;
      cat.Collection.Save();
      return cat.GetProxy();
    }

    [WebMethod]
    public ForumCategoryProxy AddForumCategory(int? parentID)
    {
      if (!TSAuthentication.IsSystemAdmin) return null;

     
      ForumCategory cat = (new ForumCategories(TSAuthentication.GetLoginUser())).AddNewForumCategory();
      cat.OrganizationID = TSAuthentication.OrganizationID.ToString();
      cat.CategoryName = parentID == null ? "Untitled Category" : "Untitled Subcategory";
      cat.ParentID = parentID ?? -1;
      cat.Position = GetForumCategoryMaxPosition(parentID) + 1;
      cat.Collection.Save();
      return cat.GetProxy();
    }

    private int GetForumCategoryMaxPosition(int? parentID)
    { 
      parentID = parentID ?? -1;
      
      ForumCategories cats = new ForumCategories(TSAuthentication.GetLoginUser());
      if (parentID < 0) cats.LoadCategories(TSAuthentication.OrganizationID);
      else cats.LoadSubcategories((int)parentID);

      int max = -1;

      foreach (ForumCategory cat in cats)
	    {
        if (cat.Position != null && cat.Position > max) max = (int)cat.Position;
	    }

      return max;
    }

    [WebMethod]
    public bool DeleteForumCategory(int categoryID)
    {
      if (!TSAuthentication.IsSystemAdmin) return false;
      ForumCategory cat = ForumCategories.GetForumCategory(TSAuthentication.GetLoginUser(), categoryID);
      if (int.Parse(cat.OrganizationID) != TSAuthentication.OrganizationID) return false;

      if (cat.ParentID < 0)
      {
        ForumCategories cats = new ForumCategories(TSAuthentication.GetLoginUser());
        cats.LoadSubcategories(cat.CategoryID);

        foreach (ForumCategory item in cats)
        {
          item.Delete();
        }
        cats.Save();
      }

      cat.Delete();
      cat.Collection.Save();
      return true;
    }

 
   [WebMethod]
    public void UpdateForumCategoryOrder(string data)
   {
     List<ForumCategoryOrder> orders = JsonConvert.DeserializeObject<List<ForumCategoryOrder>>(data);

     if (!TSAuthentication.IsSystemAdmin) return;

     LoginUser loginUser = TSAuthentication.GetLoginUser();
     int catPos = 0;
     foreach (ForumCategoryOrder order in orders)
     {
       ForumCategory cat = ForumCategories.GetForumCategory(loginUser, (int)order.ParentID);
       cat.Position = catPos;
       cat.Collection.Save();

       int subPos = 0;
       foreach (int id in order.CategoryIDs)
       {
         ForumCategory sub = ForumCategories.GetForumCategory(loginUser, id);
         sub.Position = subPos;
         sub.ParentID = (int)order.ParentID;
         sub.Collection.Save();
         subPos++;
       }
       catPos++;
     }
   }
    
  }

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class ForumCategoryInfo
  {
    public ForumCategoryInfo() {}
    [DataMember] public ForumCategoryProxy Category { get; set; }
    [DataMember] public ForumCategoryProxy[] Subcategories { get; set; }
  }

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class ForumCategoryOrder
  {
    public ForumCategoryOrder() {}
    [DataMember] public int? ParentID {get; set;}
    [DataMember] public List<int> CategoryIDs {get; set;}
  }
}