using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ForumCategory
  {
  }
  
  public partial class ForumCategories
  {

    public void LoadCategories(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ForumCategories WHERE OrganizationID = @OrganizationID AND ParentID < 0 ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadSubcategories(int categoryID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ForumCategories WHERE ParentID = @ParentID ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ParentID", categoryID);
        Fill(command);
      }
    }
  }
  
}
