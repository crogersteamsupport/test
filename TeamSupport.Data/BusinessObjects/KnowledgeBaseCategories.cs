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

  }
  
}
