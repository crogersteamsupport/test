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
using System.Linq;
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
		result = !string.IsNullOrEmpty(GetJiraInstanceNameForTicket(ticketId));
		return result;
	}

	[WebMethod]
	public string GetJiraInstanceNameForTicket(int ticketId)
	{
		string jiraInstanceName = string.Empty;
		LoginUser loginUser = TSAuthentication.GetLoginUser();
		CRMLinkTable organizationLinks = new CRMLinkTable(loginUser);
		organizationLinks.LoadByOrganizationID(TSAuthentication.OrganizationID);

		List<CRMLinkTableItem> organizationJiraLinks = organizationLinks.Where(p => p.CRMType.ToLower() == "jira").ToList();

		if (organizationJiraLinks != null)
		{
			TicketLinkToJira ticketLink = new TicketLinkToJira(loginUser);
			ticketLink.LoadByTicketID(ticketId);

			if (ticketLink != null
					&& ticketLink.Any()
					&& organizationJiraLinks.Where(p => p.CRMLinkID == ticketLink[0].CrmLinkID && p.Active).Any())
			{
				CRMLinkTableItem crmJiraInstance = CRMLinkTable.GetCRMLinkTableItem(loginUser, (int)ticketLink[0].CrmLinkID);

				if (crmJiraInstance != null && !string.IsNullOrEmpty(crmJiraInstance.InstanceName))
				{
					jiraInstanceName = crmJiraInstance.InstanceName;
				}
			}
			else
			{
				TicketsViewItem ticket = TicketsView.GetTicketsViewItem(loginUser, ticketId);
				CRMLinkTableItem ticketJiraInstance = organizationJiraLinks.Where(p => p.Active).FirstOrDefault();

				if (ticket.ProductID != null)
				{
					JiraInstanceProducts jiraInstanceProduct = new JiraInstanceProducts(loginUser);
					jiraInstanceProduct.LoadByProductAndOrganization((int)ticket.ProductID, ticket.OrganizationID, "Jira");

					if (jiraInstanceProduct != null && jiraInstanceProduct.Count > 0)
					{
						ticketJiraInstance = organizationJiraLinks.Where(p => p.CRMLinkID == jiraInstanceProduct[0].CrmLinkId && p.Active).SingleOrDefault();
					}
				}

				//if ticket does not have Product then use the default instance if it's active
				if (ticketJiraInstance == null && ticket.ProductID == null)
				{
					ticketJiraInstance = organizationJiraLinks.Where(p => p.InstanceName.Trim().ToLower() == "default" && p.Active).SingleOrDefault();
				}

				if (ticketJiraInstance != null && ticketJiraInstance.CRMLinkID != 0)
				{
					if (string.IsNullOrEmpty(ticketJiraInstance.RestrictedToTicketTypes))
					{
						jiraInstanceName = ticketJiraInstance.InstanceName;
					}
					else
					{
						foreach (string allowedTicketType in ticketJiraInstance.RestrictedToTicketTypes.Split(','))
						{
							if (ticket.TicketTypeID.ToString() == allowedTicketType)
							{
								jiraInstanceName = ticketJiraInstance.InstanceName;
								break;
							}
						}
					}
				}
			}
		}

		return jiraInstanceName;
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

	 [WebMethod]
	 public void RollbackImport(int importFileID, ReferenceType refType)
	 {
		StringBuilder query = new StringBuilder();
		query.Append(@"
			DECLARE @OrganizationID int
	
			SELECT
				@OrganizationID = OrganizationID
			FROM
				Imports
			WHERE
				ImportID = @ImportFileID
		");

		switch (refType)
		{
			case ReferenceType.Actions:
				query.Append(GetRollbackActionsQuery());
				break;
			case ReferenceType.Assets:
				query.Append(GetRollbackAssetTicketsQuery());
				query.Append(GetRollbackCustomValuesQuery());
				query.Append(GetRollbackAssetsQuery());
				break;
			case ReferenceType.Organizations:
				query.Append(GetRollbackCustomValuesQuery());
				query.Append(GetRollbackAddressesAndPhoneNumbersQuery());
				query.Append(GetRollbackCompaniesQuery());
				break;
			case ReferenceType.CompanyAddresses:
				query.Append(GetRollbackAddressesAndPhoneNumbersQuery());
				query.Append(GetRollbackContactsQuery());
				query.Append(GetRollbackCompaniesQuery());
				break;
			case ReferenceType.CompanyPhoneNumbers:
				query.Append(GetRollbackAddressesAndPhoneNumbersQuery());
				query.Append(GetRollbackContactsQuery());
				query.Append(GetRollbackCompaniesQuery());
				break;
			case ReferenceType.Contacts:
				query.Append(GetRollbackCustomValuesQuery());
				query.Append(GetRollbackAddressesAndPhoneNumbersQuery());
				query.Append(GetRollbackContactsQuery());
				query.Append(GetRollbackCompaniesQuery());
				break;
			case ReferenceType.ContactAddresses:
				query.Append(GetRollbackContactsQuery());
				query.Append(GetRollbackCompaniesQuery());
				break;
			case ReferenceType.ContactPhoneNumbers:
				query.Append(GetRollbackContactsQuery());
				query.Append(GetRollbackCompaniesQuery());
				break;
			case ReferenceType.Tickets:
				query.Append(GetRollbackCustomValuesQuery());
				query.Append(GetRollbackProductsQuery());
				query.Append(GetRollbackProductVersionsQuery());
				query.Append(GetRollbackTicketRelationshipsQuery());
				query.Append(GetRollbackAssetTicketsQuery());
				query.Append(GetRollbackContactTicketsQuery());
				query.Append(GetRollbackOrganizationTicketsQuery());
				query.Append(GetRollbackActionsQuery());
				query.Append(GetRollbackTicketsQuery());
				break;
			case ReferenceType.OrganizationTickets:
				query.Append(GetRollbackOrganizationTicketsQuery());
				break;
			case ReferenceType.ContactTickets:
				query.Append(GetRollbackContactTicketsQuery());
				break;
			case ReferenceType.AssetTickets:
				query.Append(GetRollbackAssetTicketsQuery());
				break;
			case ReferenceType.TicketRelationships:
				query.Append(GetRollbackTicketRelationshipsQuery());
				break;
			//case ReferenceType.CustomFieldPickList:
			//  ImportCustomFieldPickList(import);
			//  break;
			case ReferenceType.Products:
				query.Append(GetRollbackCustomValuesQuery());
				query.Append(GetRollbackProductsQuery());
				break;
			case ReferenceType.ProductVersions:
				query.Append(GetRollbackCustomValuesQuery());
				query.Append(GetRollbackProductVersionsQuery());
				break;
			case ReferenceType.Users:
				query.Append(GetRollbackCustomValuesQuery());
				query.Append(GetRollbackAddressesAndPhoneNumbersQuery());
				query.Append(GetRollbackUsersQuery());
				break;
			case ReferenceType.OrganizationProducts:
				query.Append(GetRollbackCustomValuesQuery());
				query.Append(GetRollbackOrganizationProductsQuery());
				break;
			case ReferenceType.Notes:
				query.Append(GetRollbackOrganizationNotesQuery());
				break;
		}

		query.Append(@"
			UPDATE 
				Imports
			SET 
				IsRolledBack = 1
			WHERE 
				ImportID = @ImportFileID		
		");

		using (SqlConnection connection = new SqlConnection(TSAuthentication.GetLoginUser().ConnectionString))
      {
			try
			{
				connection.Open();
				SqlCommand command = connection.CreateCommand();
				command.Connection = connection;
				command.CommandType = CommandType.Text;
				command.CommandText = query.ToString();
				command.Parameters.AddWithValue("@ImportFileID", importFileID);
				command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), e, "AdminService.RollbackImport");
			}
		}
	 }

	 private string GetRollbackActionsQuery()
	 {
		 return @"
			DELETE
				a
			FROM 
				Actions a
				JOIN Tickets t
					ON a.TicketID = t.TicketID
			WHERE
				t.OrganizationID = @OrganizationID
				AND a.ImportFileID = @ImportFileID
		";
	 }

	 private string GetRollbackAssetsQuery()
	 {
		 return @"
			DELETE
				aa
			FROM
				AssetAssignments aa
				JOIN AssetHistory ah
					ON aa.HistoryID = ah.HistoryID
			WHERE
				ah.OrganizationID = @OrganizationID
				AND aa.ImportFileID = @ImportFileID

			DELETE 
				AssetHistory 
			WHERE 
				OrganizationID = @OrganizationID
				AND ImportFileID = @ImportFileID

			DELETE 
				Assets 
			WHERE 
				OrganizationID = @OrganizationID
				AND ImportFileID = @ImportFileID
		";
	 }

	 private string GetRollbackAssetTicketsQuery()
	 {
		 return @"
			DELETE
				at
			FROM
				AssetTickets at
				JOIN Tickets t
					ON at.TicketID = t.TicketID
			WHERE
				t.OrganizationID = @OrganizationID
				AND at.ImportFileID = @ImportFileID
		 ";
	 }

	 private string GetRollbackCustomValuesQuery()
	 {
		 return @"
			DELETE
				cv
			FROM
				CustomValues cv
				JOIN CustomFields cf
					ON cv.CustomFieldID = cf.CustomFieldID
			WHERE
				cf.OrganizationID = @OrganizationID
				AND cv.ImportFileID = @ImportFileID
		 ";
	 }

	 private string GetRollbackAddressesAndPhoneNumbersQuery()
	 {
		 return @"
			DELETE
				a
			FROM
				Addresses a
				LEFT JOIN Users u
					ON a.RefType = 22
					AND a.RefID = u.UserID
				LEFT JOIN Organizations uo
					ON a.RefType = 22
					AND u.OrganizationID = uo.OrganizationID
				LEFT JOIN Organizations o
					ON a.RefType = 9
					AND a.RefID = o.OrganizationID
			WHERE
				(
					(a.RefType = 9 AND o.ParentID = @OrganizationID)
					OR (a.RefType = 22 AND (uo.ParentID = @OrganizationID OR uo.OrganizationID = @OrganizationID))
				)
				AND a.ImportFileID = @ImportFileID

			DELETE
				pn
			FROM
				PhoneNumbers pn
				LEFT JOIN Users c
					ON pn.RefType = 32
					AND pn.RefID = c.UserID
				LEFT JOIN Organizations co
					ON pn.RefType = 32
					AND c.OrganizationID = co.OrganizationID
				LEFT JOIN Organizations o
					ON pn.RefType = 9
					AND pn.RefID = o.OrganizationID
				LEFT JOIN Users u
					ON pn.RefType = 22
					AND pn.RefID = u.UserID
			WHERE
				(
					(pn.RefType = 9 AND o.ParentID = @OrganizationID)
					OR (pn.RefType = 32 AND co.ParentID = @OrganizationID)
					OR (pn.RefType = 22 AND u.OrganizationID = @OrganizationID)
				)
				AND pn.ImportFileID = @ImportFileID
		 ";
	 }

	 private string GetRollbackCompaniesQuery()
	 {
		 return @"
			DELETE 
				Organizations 
			WHERE 
				ParentID = @OrganizationID
				AND ImportFileID = @ImportFileID
		 ";
	 }

	 private string GetRollbackContactsQuery()
	 {
		 return @"
			DELETE
				c
			FROM 
				Users c
				JOIN Organizations o
					ON c.OrganizationID = o.OrganizationID
			WHERE
				o.ParentID = @OrganizationID
				AND c.ImportFileID = @ImportFileID
		 ";
	 }

	 private string GetRollbackProductsQuery()
	 {
		 return @"
			DELETE 
				Products 
			WHERE 
				OrganizationID = @OrganizationID
				AND ImportFileID = @ImportFileID
		 ";
	 }

	 private string GetRollbackProductVersionsQuery()
	 {
		 return @"
			DELETE
				pv
			FROM 
				ProductVersions pv
				JOIN Products p
					ON pv.ProductID = p.ProductID
			WHERE
				p.OrganizationID = @OrganizationID
				AND pv.ImportFileID = @ImportFileID
		 ";
	 }

	 private string GetRollbackTicketRelationshipsQuery()
	 {
		 return @"
			DELETE 
				TicketRelationships 
			WHERE 
				OrganizationID = @OrganizationID
				AND ImportFileID = @ImportFileID
		 ";
	 }

	 private string GetRollbackContactTicketsQuery()
	 {
		 return @"
			DELETE
				ut
			FROM
				UserTickets ut
				JOIN Tickets t
					ON ut.TicketID = t.TicketID
			WHERE
				t.OrganizationID = @OrganizationID
				AND ut.ImportFileID = @ImportFileID
		 ";
	 }

	 private string GetRollbackOrganizationTicketsQuery()
	 {
		 return @"
			DELETE
				ot
			FROM 
				OrganizationTickets ot
				JOIN Organizations o
					ON ot.OrganizationID = o.OrganizationID
			WHERE
				o.ParentID = @OrganizationID
				AND ot.ImportFileID = @ImportFileID
		 ";
	 }

	 private string GetRollbackTicketsQuery()
	 {
		 return @"
			DELETE 
				Tickets
			WHERE 
				OrganizationID = @OrganizationID
				AND ImportFileID = @ImportFileID
		 ";
	 }

	 private string GetRollbackUsersQuery()
	 {
		 return @"
			DELETE
				u
			FROM
				Users u
			WHERE
				u.OrganizationID = @OrganizationID
				AND u.ImportFileID = @ImportFileID
		 ";
	 }

	 private string GetRollbackOrganizationProductsQuery()
	 {
		 return @"
			DELETE
				op
			FROM
				OrganizationProducts op
				JOIN Organizations o
					ON op.OrganizationID = o.OrganizationID
			WHERE
				o.ParentID = @OrganizationID
				AND op.ImportFileID = @ImportFileID
		 ";
	 }

	 private string GetRollbackOrganizationNotesQuery()
	 {
		 return @"
			DELETE
				n
			FROM
				Notes n
				JOIN Organizations o
					ON n.RefType = 9
					AND n.RefID = o.OrganizationID
			WHERE
				o.ParentID = @OrganizationID
				AND n.ImportFileID = @ImportFileID
		 ";
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