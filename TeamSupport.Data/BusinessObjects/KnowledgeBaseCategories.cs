using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class KnowledgeBaseCategory
  {
    public static int? GetIDByName(LoginUser loginUser, string name, int? parentID)
    {
      KnowledgeBaseCategories categories = new KnowledgeBaseCategories(loginUser);
      categories.LoadByName(loginUser.OrganizationID, name);
      if (categories.IsEmpty) return null;
      else return categories[0].CategoryID;
    }
  }
  
  public partial class KnowledgeBaseCategories
  {
    public KnowledgeBaseCategory FindByName(string name)
    {
      return FindByName(name, null);
    }

    public KnowledgeBaseCategory FindByName(string name, int? parentID)
    {
      name = name.Trim().ToLower();
      foreach (KnowledgeBaseCategory knowledgeBaseCategory in this)
      {
        if (knowledgeBaseCategory.CategoryName.ToLower().Trim() == name && (parentID == null || parentID == knowledgeBaseCategory.ParentID))
        {
          return knowledgeBaseCategory;
        }
      }
      return null;
    }

    public void LoadAllCategories(int organizationID, string orderBy = "Position")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM KnowledgeBaseCategories WHERE OrganizationID = @OrganizationID ORDER BY " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadCategories(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM KnowledgeBaseCategories WHERE OrganizationID = @OrganizationID AND ParentID < 0 ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadSubcategories(int categoryID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM KnowledgeBaseCategories WHERE ParentID = @ParentID ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ParentID", categoryID);
        Fill(command);
      }
    }

    public void LoadByName(int organizationID, string name)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM KnowledgeBaseCategories WHERE OrganizationID = @OrganizationID AND CategoryName = @CategoryName";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@CategoryName", name);
        Fill(command);
      }
    }

    public static string GetKnowledgeBaseCategoryDisplayString(LoginUser loginUser, int categoryID)
    {
      KnowledgeBaseCategories knowledgeBaseCategories = new KnowledgeBaseCategories(loginUser);
      KnowledgeBaseCategories knowledgeBaseParentCategory = new KnowledgeBaseCategories(loginUser);

      knowledgeBaseCategories.LoadByCategoryID(categoryID);
      if (knowledgeBaseCategories.IsEmpty)
        return null;
      else if (knowledgeBaseCategories[0].ParentID > 0)
      {
        knowledgeBaseParentCategory.LoadByCategoryID(knowledgeBaseCategories[0].ParentID);
        return knowledgeBaseParentCategory[0].CategoryName + " -> " + knowledgeBaseCategories[0].CategoryName;
      }
      else
      {
        return knowledgeBaseCategories[0].CategoryName;
      }
    }
  }
  
}
