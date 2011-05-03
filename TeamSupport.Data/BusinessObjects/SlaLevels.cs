using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class SlaLevel
  {
  }
  
  public partial class SlaLevels
  {
    partial void BeforeRowDelete(int slaLevelID)
    {
      SlaLevel level = GetSlaLevel(LoginUser, slaLevelID);
      if (level == null) return;
      Organizations organizations = new Organizations(LoginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE Organizations SET SlaLevelID = NULL WHERE SlaLevelID = @SlaLevelID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@SlaLevelID", slaLevelID);
        organizations.ExecuteNonQuery(command, "Organizations");
      }
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE Organizations SET InternalSlaLevelID = NULL WHERE InternalSlaLevelID = @SlaLevelID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@SlaLevelID", slaLevelID);
        organizations.ExecuteNonQuery(command, "Organizations");
      }
    }

    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM SlaLevels WHERE OrganizationID = @OrganizationID ORDER BY Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

  }
  
}
