using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class Group
  {
  }

  public partial class Groups
  {

 
    public void LoadByUserID(int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Groups g LEFT JOIN GroupUsers gu ON g.GroupID = gu.GroupID WHERE gu.UserID = @UserID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command, "Groups,GroupUsers");
      }
    }

    public Group FindByImportID(string importID)
    {
      importID = importID.Trim().ToLower();
      foreach (Group group in this)
      {
        if ((group.ImportID != null && group.ImportID.Trim().ToLower() == importID) || group.Name.Trim().ToLower() == importID)
        {
          return group;
        }
      }
      return null;
    }

    public Group FindByName(string name)
    {
      foreach (Group group in this)
      {
        if (group.Name.Trim().ToLower() == name.Trim().ToLower())
        {
          return group;
        }
      }
      return null;
    }
    
    public void LoadByNotUserID(int userID, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Groups g WHERE (g.OrganizationID = @OrganizationID) AND (1 not in (SELECT 1 FROM GroupUsers gu WHERE gu.UserID = @UserID AND gu.GroupID = g.GroupID))";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command, "Groups,GroupUsers");
      }
    }

    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Groups g WHERE (g.OrganizationID = @OrganizationID) ORDER BY g.Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByOrganizationIDForGrid(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT g.*, (SELECT COUNT(*) FROM TicketGridView tgv WHERE (tgv.GroupID = g.GroupID) AND (tgv.IsClosed = 0)) AS TicketCount
                                FROM Groups g
                                WHERE (g.OrganizationID = @OrganizationID) 
                                ORDER BY Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    partial void BeforeDBDelete(int groupID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE Tickets SET GroupID = NULL WHERE (GroupID = @GroupID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@GroupID", groupID);
        ExecuteNonQuery(command, "Tickets");
      }

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE Organizations SET DefaultPortalGroupID = NULL WHERE (DefaultPortalGroupID = @GroupID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@GroupID", groupID);
        ExecuteNonQuery(command, "Organizations");
      } 
      
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM GroupUsers WHERE (GroupID = @GroupID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@GroupID", groupID);
        ExecuteNonQuery(command, "GroupUsers");
      }

    }

    public void AddGroupUser(int userID, int groupID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "INSERT INTO GroupUsers (GroupID, UserID, DateCreated, DateModified, CreatorID, ModifierID) VALUES (@GroupID, @UserID, @DateCreated, @DateModified, @CreatorID, @ModifierID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@GroupID", groupID);
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
        command.Parameters.AddWithValue("@DateModified", DateTime.UtcNow);
        command.Parameters.AddWithValue("@CreatorID", LoginUser.UserID);
        command.Parameters.AddWithValue("@ModifierID", LoginUser.UserID);
        ExecuteNonQuery(command, "GroupUsers");
      }

      User user = (User)Users.GetUser(LoginUser, userID);
      Group group = (Group)Groups.GetGroup(LoginUser, groupID);
      string description = "Added '" + user.FirstName + " " + user.LastName + "' to group '" + group.Name + "'";
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Groups, groupID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, userID, description);


    }

    public void DeleteGroupUser(int groupID, int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM GroupUsers WHERE (UserID = @UserID) AND (GroupID = @GroupID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@GroupID", groupID);
        ExecuteNonQuery(command, "GroupUsers");
      }
      User user = (User)Users.GetUser(LoginUser, userID);
      Group group = (Group)Groups.GetGroup(LoginUser, groupID);
      string description = "Removed '" + user.FirstName + " " + user.LastName + "' from group '" + group.Name + "'";
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Groups, groupID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, userID, description);
    }

  }
}
