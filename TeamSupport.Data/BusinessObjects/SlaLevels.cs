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
    public static int? GetIDByName(LoginUser loginUser, string name, int? parentID)
    {
      SlaLevels slaLevels = new SlaLevels(loginUser);
      slaLevels.LoadByName(loginUser.OrganizationID, name);
      if (slaLevels.IsEmpty) return null;
      else return slaLevels[0].SlaLevelID;
    }
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

    public void LoadByName(int organizationID, string name)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM SlaLevels WHERE OrganizationID = @OrganizationID AND Name = @Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@Name", name);
        Fill(command);
      }
    }
  }
  
}
