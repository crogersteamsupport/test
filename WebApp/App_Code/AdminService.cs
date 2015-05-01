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
      if (cat.OrganizationID != TSAuthentication.OrganizationID) return null;
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
      cat.OrganizationID = TSAuthentication.OrganizationID;
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
      if (cat.OrganizationID != TSAuthentication.OrganizationID) return false;

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
    
    /// <summary>
    /// Knowledge Base Categories
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public KnowledgeBaseCategoryInfo[] GetKnowledgeBaseCategories()
    {
      List<KnowledgeBaseCategoryInfo> result = new List<KnowledgeBaseCategoryInfo>();
      KnowledgeBaseCategories cats = new KnowledgeBaseCategories(TSAuthentication.GetLoginUser());
      cats.LoadCategories(TSAuthentication.OrganizationID);

      foreach (KnowledgeBaseCategory cat in cats)
      {
        KnowledgeBaseCategoryInfo info = new KnowledgeBaseCategoryInfo();
        info.Category = cat.GetProxy();

        KnowledgeBaseCategories subs = new KnowledgeBaseCategories(cats.LoginUser);
        subs.LoadSubcategories(cat.CategoryID);
        info.Subcategories = subs.GetKnowledgeBaseCategoryProxies();

        result.Add(info);
      }

      return result.ToArray();
    }

    [WebMethod]
    public KnowledgeBaseCategoryProxy UpdateKnowledgeBaseCategory(int categoryID, string name, string description, bool visibleOnPortal)
    {
      if (!TSAuthentication.IsSystemAdmin) return null;
      KnowledgeBaseCategory cat = KnowledgeBaseCategories.GetKnowledgeBaseCategory(TSAuthentication.GetLoginUser(), categoryID);
      if (cat.OrganizationID != TSAuthentication.OrganizationID) return null;
      cat.CategoryName = name;
      cat.CategoryDesc = description;
      cat.VisibleOnPortal = visibleOnPortal;
      cat.Collection.Save();
      return cat.GetProxy();
    }

    [WebMethod]
    public KnowledgeBaseCategoryProxy AddKnowledgeBaseCategory(int? parentID)
    {
      if (!TSAuthentication.IsSystemAdmin) return null;


      KnowledgeBaseCategory cat = (new KnowledgeBaseCategories(TSAuthentication.GetLoginUser())).AddNewKnowledgeBaseCategory();
      cat.OrganizationID = TSAuthentication.OrganizationID;
      cat.CategoryName = parentID == null ? "Untitled Category" : "Untitled Subcategory";
      cat.ParentID = parentID ?? -1;
      cat.Position = GetKnowledgeBaseCategoryMaxPosition(parentID) + 1;
      cat.VisibleOnPortal = true;
      cat.Collection.Save();
      return cat.GetProxy();
    }

    private int GetKnowledgeBaseCategoryMaxPosition(int? parentID)
    { 
      parentID = parentID ?? -1;

      KnowledgeBaseCategories cats = new KnowledgeBaseCategories(TSAuthentication.GetLoginUser());
      if (parentID < 0) cats.LoadCategories(TSAuthentication.OrganizationID);
      else cats.LoadSubcategories((int)parentID);

      int max = -1;

      foreach (KnowledgeBaseCategory cat in cats)
	    {
        if (cat.Position != null && cat.Position > max) max = (int)cat.Position;
	    }

      return max;
    }

    [WebMethod]
    public bool DeleteKnowledgeBaseCategory(int categoryID)
    {
      if (!TSAuthentication.IsSystemAdmin) return false;
      KnowledgeBaseCategory cat = KnowledgeBaseCategories.GetKnowledgeBaseCategory(TSAuthentication.GetLoginUser(), categoryID);
      if (cat.OrganizationID != TSAuthentication.OrganizationID) return false;

      if (cat.ParentID < 0)
      {
        KnowledgeBaseCategories cats = new KnowledgeBaseCategories(TSAuthentication.GetLoginUser());
        cats.LoadSubcategories(cat.CategoryID);

        foreach (KnowledgeBaseCategory item in cats)
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
    public void UpdateKnowledgeBaseCategoryOrder(string data)
   {
     List<KnowledgeBaseCategoryOrder> orders = JsonConvert.DeserializeObject<List<KnowledgeBaseCategoryOrder>>(data);

     if (!TSAuthentication.IsSystemAdmin) return;

     LoginUser loginUser = TSAuthentication.GetLoginUser();
     int catPos = 0;
     foreach (KnowledgeBaseCategoryOrder order in orders)
     {
       KnowledgeBaseCategory cat = KnowledgeBaseCategories.GetKnowledgeBaseCategory(loginUser, (int)order.ParentID);
       cat.Position = catPos;
       cat.Collection.Save();

       int subPos = 0;
       foreach (int id in order.CategoryIDs)
       {
         KnowledgeBaseCategory sub = KnowledgeBaseCategories.GetKnowledgeBaseCategory(loginUser, id);
         sub.Position = subPos;
         sub.ParentID = (int)order.ParentID;
         sub.Collection.Save();
         subPos++;
       }
       catPos++;
     }
   }
    
    /// <summary>
    /// Checks if the ticket type is allowed to be linked to Jira in the Integration Admin settings. Link should be active in account too.
    /// </summary>
    /// <returns>True or False</returns>
    [WebMethod]
    public bool GetIsJiraLinkActiveForTicket(int ticketId)
    {
      bool result = false;
   
      CRMLinkTable organizationLinks = new CRMLinkTable(TSAuthentication.GetLoginUser());
      organizationLinks.LoadByOrganizationID(TSAuthentication.OrganizationID);

      foreach (CRMLinkTableItem link in organizationLinks)
      {
        if (link.CRMType == "Jira" && link.Active)
        {
          if (string.IsNullOrEmpty(link.RestrictedToTicketTypes))
          {
            result = true;
          }
          else
          {
            TicketsView ticket = new TicketsView(TSAuthentication.GetLoginUser());
            ticket.LoadByTicketID(ticketId);

            foreach(string allowedTicketType in link.RestrictedToTicketTypes.Split(','))
            {
              result = ticket[0].TicketTypeID.ToString() == allowedTicketType;
              
              if (result)
              {
                break;
              }
            }

            //If restricted check if it was linked already
            if (!result)
            {
              TicketLinkToJira ticketLinkToJira = new TicketLinkToJira(TSAuthentication.GetLoginUser());
              ticketLinkToJira.LoadByTicketID(ticketId);
              result = ticketLinkToJira != null && ticketLinkToJira.Count > 0;
            }
          }
        }
      }

      return result;
    }

    /// <summary>
    /// Checks if the Jira Integration is active.
    /// </summary>
    /// <returns>True or False</returns>
    [WebMethod]
    public bool GetIsJiraLinkActiveForOrganization()
    {
      bool result = false;

      CRMLinkTable organizationLinks = new CRMLinkTable(TSAuthentication.GetLoginUser());
      organizationLinks.LoadByOrganizationID(TSAuthentication.OrganizationID);

      foreach (CRMLinkTableItem link in organizationLinks)
      {
        if (link.CRMType == "Jira" && link.Active)
        {
          result = true;
        }
      }

      return result;
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

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class KnowledgeBaseCategoryInfo
  {
    public KnowledgeBaseCategoryInfo() { }
    [DataMember]
    public KnowledgeBaseCategoryProxy Category { get; set; }
    [DataMember]
    public KnowledgeBaseCategoryProxy[] Subcategories { get; set; }
  }

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class KnowledgeBaseCategoryOrder
  {
    public KnowledgeBaseCategoryOrder() { }
    [DataMember]
    public int? ParentID { get; set; }
    [DataMember]
    public List<int> CategoryIDs { get; set; }
  }
}