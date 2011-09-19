using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class User 
  {
    public UsersViewItem GetUserView()
    {
      return UsersView.GetUsersViewItem(BaseCollection.LoginUser, UserID);
    }

    public ContactsViewItem GetContactView()
    {
      return ContactsView.GetContactsViewItem(BaseCollection.LoginUser, UserID);
    }

    public string DisplayName
    {
      get
      {
        return LastName + ", " + FirstName;
      }
    }
    public string FirstLastName
    {
      get
      {
        return FirstName + " " + LastName;
      }
    }

    public bool IsSameName(string name)
    {
      StringBuilder builder = new StringBuilder();
      name = name.Trim().ToLower();
      foreach (char c in name)
	    {
        if (char.IsLetterOrDigit(c)) builder.Append(c);
	    }

      return ((FirstName.Trim() + LastName.Trim()).ToLower() == name) || ((LastName.Trim() + FirstName.Trim()).ToLower() == name);
    }

    public void UpdatePing()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE Users SET LastPing = GETUTCDATE() WHERE UserID = @UserID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", UserID);
        Collection.ExecuteNonQuery(command, "Users");
      }
    }

  }
  
  public partial class Users
  {

    partial void BeforeRowDelete(int userID)
    {
      User user = (User)Users.GetUser(LoginUser, userID);
      string description;
      if (user.OrganizationID == LoginUser.OrganizationID)
        description = "Deleted user '" + user.FirstName + " " + user.LastName + "'";
      else
        description = "Deleted contact '" + user.FirstName + " " + user.LastName + "'";

      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Users, userID, description);
    }

    partial void BeforeRowEdit(User user)
    {
      string description;
      User oldUser = (User)Users.GetUser(LoginUser, user.UserID);
      string name = oldUser.FirstName + " " + oldUser.LastName;

      if (oldUser.CryptedPassword != user.CryptedPassword)
      {
        description = "Changed '" + name + "' password";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, user.UserID, description);
      }

      if (oldUser.Email != user.Email)
      {
        description = "Changed '" + name + "' email from '" + oldUser.Email + "' to '" + user.Email + '"';
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, user.UserID, description);
      }

      if (oldUser.FirstName != user.FirstName)
      {
        description = "Changed '" + name + "' first name to '" + user.FirstName + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, user.UserID, description);
      }

      if (oldUser.LastName != user.LastName)
      {
        description = "Changed '" + name + "' last name to '" + user.LastName + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, user.UserID, description);
      }

      if (oldUser.MiddleName != user.MiddleName)
      {
        description = "Changed '" + name + "' middle name to '" + user.MiddleName + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, user.UserID, description);
      }

      if (oldUser.InOffice != user.InOffice)
      {
        description = "Changed '" + name + "' in office status to '" + user.InOffice.ToString() + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, user.UserID, description);
      }

      if (oldUser.InOfficeComment != user.InOfficeComment)
      {
        description = "Changed '" + name + "' in office comment";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, user.UserID, description);
      }

      if (oldUser.IsActive != user.IsActive)
      {
        description = "Changed '" + name + "' active status to '" + user.IsActive.ToString() + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, user.UserID, description);
      }

      if (oldUser.IsFinanceAdmin != user.IsFinanceAdmin)
      {
        description = "Changed '" + name + "' financial administrator access rights to '" + user.IsFinanceAdmin.ToString() + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, user.UserID, description);
      }

      if (oldUser.IsSystemAdmin != user.IsSystemAdmin)
      {
        description = "Changed '" + name + "' system administrator access rights to '" + user.IsSystemAdmin.ToString() + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, user.UserID, description);
      }

      if (oldUser.IsPortalUser != user.IsPortalUser)
      {
        description = "Changed '" + name + "' user portal access rights to '" + user.IsPortalUser.ToString() + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, user.UserID, description);
      }

      if (oldUser.PrimaryGroupID != user.PrimaryGroupID)
      {
        string group1 = oldUser.PrimaryGroupID == null ? "Unassigned" : ((Group)Groups.GetGroup(LoginUser, (int) oldUser.PrimaryGroupID)).Name;
        string group2 = user.PrimaryGroupID == null ? "Unassigned" : ((Group)Groups.GetGroup(LoginUser, (int)user.PrimaryGroupID)).Name;

        description = "Changed '" + name + "' primary group from '" + group1 + "' to '" + group2 + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, user.UserID, description);
      }
    }

    partial void AfterRowInsert(User user)
    {
      string description;
      if (user.OrganizationID == LoginUser.OrganizationID)
        description = "Created user '" + user.FirstName + " " + user.LastName + "'";
      else
        description = "Created contact '" + user.FirstName + " " + user.LastName + "'";

      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, user.UserID, description);
    }

    public void LoadByImportID(string importID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Users WHERE (ImportID = @ImportID) AND (MarkDeleted = 0)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ImportID", importID.Trim());
        Fill(command);
      }
    }

    public void LoadByEmail(string email)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Users WHERE (Email = @Email) AND (MarkDeleted = 0)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@Email", email.Trim());
        Fill(command);
      }
    }

    public void LoadByEmail(string email, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT u.*
                                FROM Users u 
                                WHERE (u.OrganizationID = @OrganizationID)
                                AND (u.Email = @Email)
                                AND (u.MarkDeleted = 0)";


        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@Email", email.Trim());
        Fill(command);
      }
    }
    
    public void LoadByEmail(int parentID, string email)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT u.*
                                FROM Users u 
                                LEFT JOIN Organizations o
                                ON o.OrganizationID = u.OrganizationID
                                WHERE (o.ParentID = @ParentID)
                                AND (u.Email = @Email)
                                AND (u.MarkDeleted = 0)";


        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ParentID", parentID);
        command.Parameters.AddWithValue("@Email", email.Trim());
        Fill(command);
      }
    }

    public void LoadFinanceAdmins(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Users WHERE @OrganizationID = OrganizationID AND IsFinanceAdmin = 1 AND MarkDeleted = 0";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public bool IsEmailValid(string email, int userID, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT COUNT(*) FROM Users WHERE (Email = @Email) AND (UserID != @UserID) AND (OrganizationID = @OrganizationID) AND (MarkDeleted = 0)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@Email", email);
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        return (int)ExecuteScalar(command) < 1;
      }
    }

    public void LoadByGroupID(int groupID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Users u LEFT JOIN GroupUsers gu ON u.UserID = gu.UserID WHERE gu.GroupID = @GroupID AND u.IsActive = 1 AND u.MarkDeleted = 0";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@GroupID", groupID);
        Fill(command, "Users,GroupUsers");
      }
    }

    public void LoadByOrganizationID(int organizationID, bool loadOnlyActive)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT *, LastName + ', ' + FirstName AS DisplayName FROM Users WHERE OrganizationID = @OrganizationID AND (@ActiveOnly = 0 OR IsActive = 1) AND (MarkDeleted = 0) ORDER BY FirstName, LastName";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@ActiveOnly", loadOnlyActive);
        Fill(command);
      }
    }
    /// <summary>
    /// Get users by name
    /// </summary>
    /// <param name="text">Text containing the search terms to find the users name.</param>
    /// <param name="organizationID">The organization the user belongs to</param>
    /// <param name="loadOnlyActive">Load only active users</param>
    /// <param name="onlyAdmin">Load only admin users</param>
    /// <param name="onlyChat">Load only chat users</param>
    public void LoadByName(string text, int organizationID, bool onlyActive, bool onlyAdmin, bool onlyChat, int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Users WHERE (FirstName + ' ' + LastName LIKE '%'+@Text+'%' OR LastName + ', ' + FirstName LIKE '%'+@Text+'%') AND OrganizationID = @OrganizationID AND (@ActiveOnly = 0 OR IsActive = 1) AND (@IsSystemAdmin = 0 OR IsSystemAdmin = 1) AND (@IsChatUser = 0 OR IsChatUser = 1) AND (MarkDeleted = 0) AND (UserID <> @UserID) ORDER BY LastName, FirstName";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@IsChatUser", onlyChat);
        command.Parameters.AddWithValue("@IsSystemAdmin", onlyAdmin);
        command.Parameters.AddWithValue("@ActiveOnly", onlyActive);
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@Text", text);
        Fill(command);
      }
    }

    /// <summary>
    /// Get users by name
    /// </summary>
    /// <param name="text">Text containing the search terms to find the users name.</param>
    /// <param name="organizationID">The organization the user belongs to</param>
    /// <param name="loadOnlyActive">Load only active users</param>
    /// <param name="onlyAdmin">Load only admin users</param>
    /// <param name="onlyChat">Load only chat users</param>
    /// <param name="userID">Load all but this user</param>
    public void LoadByName(string text, int organizationID, bool onlyActive, bool onlyAdmin, bool onlyChat)
    {
      LoadByName(text, organizationID, onlyActive, onlyAdmin, onlyChat, -1);
    }

    public void LoadContactsAndUsers(int organizationID, bool loadOnlyActive)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT u.*, u.LastName + ', ' + u.FirstName AS DisplayName 
                                FROM Users u 
                                LEFT JOIN Organizations o
                                ON o.OrganizationID = u.OrganizationID
                                WHERE (o.OrganizationID = @OrganizationID OR o.ParentID = @OrganizationID)
                                AND (@ActiveOnly = 0 OR u.IsActive = 1) 
                                AND (@ActiveOnly = 0 OR o.IsActive = 1)
                                AND (u.MarkDeleted = 0) 
                                ORDER BY u.LastName, u.FirstName";
                                
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@ActiveOnly", loadOnlyActive);
        Fill(command);
      }
    }

    public void LoadContacts(int organizationID, bool loadOnlyActive)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT u.*, u.LastName + ', ' + u.FirstName AS DisplayName 
                                FROM Users u 
                                LEFT JOIN Organizations o
                                ON o.OrganizationID = u.OrganizationID
                                WHERE (o.ParentID = @OrganizationID)
                                AND (@ActiveOnly = 0 OR u.IsActive = 1) 
                                AND (@ActiveOnly = 0 OR o.IsActive = 1)
                                AND (u.MarkDeleted = 0) 
                                ORDER BY u.LastName, u.FirstName";

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@ActiveOnly", loadOnlyActive);
        Fill(command);
      }
    }

    public void LoadByOrganizationIDForGrid(int organizationID, bool loadOnlyActive)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT u.*,
                                (SELECT COUNT(*) FROM TicketGridView tgv WHERE (tgv.UserID = u.UserID) AND (tgv.IsClosed = 0)) AS TicketCount,
                                CASE
                                  WHEN DATEDIFF(ss, u.LastActivity, GETUTCDATE()) < 300 THEN 1 ELSE 0 END AS IsOnline
                                FROM Users u
                                WHERE (OrganizationID = @OrganizationID) 
                                AND (@ActiveOnly = 0 OR IsActive = 1) 
                                AND (u.MarkDeleted = 0)
                                ORDER BY LastName, FirstName";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@ActiveOnly", loadOnlyActive);
        Fill(command);
      }
    }

    public void LoadByOnline()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT 
                                u.*, 
                                o.Name AS OrganizationName,
                                CASE
                                  WHEN DATEDIFF(ss, u.LastActivity, GETUTCDATE()) > 300 THEN u.LastName + ', ' + u.FirstName + ' (Idle)'
                                  ELSE u.FirstName + ' ' + u.LastName
                                END
                                AS IdleName,
                                (SELECT TOP 1 lh.DateCreated FROM LoginHistory lh WHERE lh.UserID = u.UserID ORDER BY lh.DateCreated DESC) AS LoginDate,
                                (SELECT TOP 1 lh.Browser FROM LoginHistory lh WHERE lh.UserID = u.UserID ORDER BY lh.DateCreated DESC) AS Browser,
                                (SELECT TOP 1 lh.Version FROM LoginHistory lh WHERE lh.UserID = u.UserID ORDER BY lh.DateCreated DESC) AS Version
                                FROM Users u
                                LEFT JOIN Organizations o
                                ON o.OrganizationID = u.OrganizationID
                                WHERE (DATEDIFF(ss, u.LastPing, GETDATE()) < 45)
                                ORDER BY LastName, FirstName";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public void DeleteUserGroup(int userID, int groupID)
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

    public void AddUserGroup(int userID, int groupID)
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

    public void LoadByNotGroupID(int groupID, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Users u WHERE (u.OrganizationID = @OrganizationID) AND (u.IsActive = 1) AND (u.MarkDeleted = 0) AND (1 not in (SELECT 1 FROM GroupUsers gu WHERE gu.GroupID = @GroupID AND gu.UserID = u.UserID))";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@GroupID", groupID);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command, "Users,GroupUsers");
      }

    }

    public void LoadByTicketSubscription(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT u.* FROM Users u LEFT JOIN Subscriptions s ON u.UserID = s.UserID WHERE (s.RefID = @TicketID) AND (s.RefType = @RefType) AND (u.IsActive = 1) AND (u.MarkDeleted = 0)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        command.Parameters.AddWithValue("@RefType", (int)ReferenceType.Tickets);
        Fill(command, "Users,Tickets,Subscriptions");
      }
    }

    public void LoadByCustomerSubscription(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"SELECT u.* FROM Users u 
WHERE(u.IsActive = 1) 
AND (u.MarkDeleted = 0)
AND u.UserID IN
(
  SELECT s.UserID FROM Subscriptions s 
  WHERE s.RefType= @RefType
  AND s.RefID IN (SELECT ot.OrganizationID FROM OrganizationTickets ot WHERE (ot.TicketID = @TicketID))
)
";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        command.Parameters.AddWithValue("@RefType", (int)ReferenceType.Organizations);
        Fill(command, "Users,Tickets,Subscriptions");
      }
    }

    public void LoadBasicPortalUsers(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT u.* FROM Users u 
LEFT JOIN UserTickets ut ON ut.UserID = u.UserID
LEFT JOIN Organizations o ON o.OrganizationID = u.OrganizationID
WHERE ut.TicketID = @TicketID
AND (o.HasPortalAccess = 0 OR u.IsPortalUser = 0)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command, "Users,Tickets");
      }
    }

    public void LoadAdvancedPortalUsers(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT u.* FROM Users u 
LEFT JOIN UserTickets ut ON ut.UserID = u.UserID
LEFT JOIN Organizations o ON o.OrganizationID = u.OrganizationID
WHERE ut.TicketID = @TicketID
AND o.HasPortalAccess = 1
AND u.IsPortalUser = 1";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command, "Users,Tickets");
      }
    }

    public static string GetUserFullName(LoginUser loginUser, int userID)
    {
      User user = (User)Users.GetUser(loginUser, userID);
      return user == null ? "" : user.FirstName + " " + user.LastName;

    }

    public static void UpdateUserActivityTime(LoginUser loginUser, int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "uspUpdateUserActivityTime";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@UserID", userID);
        (new Users(loginUser)).ExecuteNonQuery(command, "Users");
      }
    }

    public static void UpdateUserPingTime(LoginUser loginUser, int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "uspUpdateUserPingTime";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@UserID", userID);
        (new Users(loginUser)).ExecuteNonQuery(command, "Users");
      }
    }

    public User FindByImportID(string importID)
    {
      importID = importID.Trim().ToLower();

      foreach (User user in this)
      {
        if ((user.ImportID != null && user.ImportID.Trim().ToLower() == importID) || 
            user.Email.Trim().ToLower() == importID ||
            user.IsSameName(importID))
        {
          return user;
        }
      }
      return null;
    }


    public User Find(string text)
    {
      if (string.IsNullOrEmpty(text)) return null;

      foreach (User user in this)
      {
        if (user.ImportID != null && user.ImportID.Trim().ToLower() == text.Trim().ToLower() ||
          string.Compare(user.FirstLastName, text, true) == 0 ||
          string.Compare(user.DisplayName, text, true) == 0 ||
          string.Compare(user.Email, text, true) == 0)
        {
          return user;
        }
      }
      return null;
    }

    public User FindByEmail(string email)
    {
      foreach (User user in this)
      {
        if (user.Email.Trim().ToLower() == email.Trim().ToLower())
        {
          return user;
        }
      }
      return null;
    }

    public User FindByName(string firstName, string lastName)
    {
      foreach (User user in this)
      {
        if (user.FirstName.Trim().ToLower() == firstName.Trim().ToLower() && user.LastName.Trim().ToLower() == lastName.Trim().ToLower())
        {
          return user;
        }
      }
      return null;
    }

    public static void MarkUserDeleted(LoginUser loginUser, int userID)
    {
      Users users = new Users(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM GroupUsers WHERE (UserID = @UserID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        users.ExecuteNonQuery(command, "GroupUsers");

        command.CommandText = "DELETE FROM Subscriptions WHERE (UserID = @UserID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@UserID", userID);
        users.ExecuteNonQuery(command, "Subscriptions");

        command.CommandText = "DELETE FROM TicketQueue WHERE (UserID = @UserID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@UserID", userID);
        users.ExecuteNonQuery(command, "TicketQueue");

        command.CommandText = "DELETE FROM UserSettings WHERE (UserID = @UserID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@UserID", userID);
        users.ExecuteNonQuery(command, "UserSettings");

        command.CommandText = "DELETE FROM UserSettings WHERE (UserID = @UserID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@UserID", userID);
        users.ExecuteNonQuery(command, "UserSettings");

        command.CommandText = "DELETE FROM Notes WHERE (RefID = @UserID) AND (RefType = 22)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@UserID", userID);
        users.ExecuteNonQuery(command, "Notes");

        command.CommandText = "DELETE FROM Addresses WHERE (RefID = @UserID) AND (RefType = 22)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@UserID", userID);
        users.ExecuteNonQuery(command, "Addresses");

        command.CommandText = "DELETE FROM PhoneNumbers WHERE (RefID = @UserID) AND (RefType = 22)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@UserID", userID);
        users.ExecuteNonQuery(command, "PhoneNumbers");

        command.CommandText = "UPDATE Tickets SET UserID = null WHERE (UserID = @UserID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@UserID", userID);
        users.ExecuteNonQuery(command, "Tickets");
      }

      User user = Users.GetUser(loginUser, userID);
      if (user != null)
      {
        user.MarkDeleted = true;
        user.Collection.Save();
      }
    
    }

    public static void MarkUsersChatUnavailable(LoginUser loginUser, int organizationID)
    {
      ChatUserSettings chatUserSettings = new ChatUserSettings(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE ChatUserSettings SET IsAvailable = 0 WHERE UserID IN (SELECT UserID FROM Users WHERE OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        chatUserSettings.ExecuteNonQuery(command, "ChatUserSettings");
      }
    }

    public static void DeleteUser(LoginUser loginUser, int userID)
    {
      Users users = new Users(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM GroupUsers WHERE (UserID = @UserID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        users.ExecuteNonQuery(command, "GroupUsers");
      }


      users.LoadByUserID(userID);
      if (!users.IsEmpty) users[0].Delete();
      users.Save();
    
    }

    public static Organization GetTSOrganization(LoginUser loginUser, int userID)
    {
      User user = (User)Users.GetUser(loginUser, userID);
      if (user == null) return null;
      Organization organization = (Organization)Organizations.GetOrganization(loginUser, user.OrganizationID);
      if (organization == null) return null;
      if (organization.ParentID == null || organization.ParentID == 1) return organization;

      Organization parent = (Organization)Organizations.GetOrganization(loginUser, (int)organization.ParentID);
      if (parent == null) return null;
      return parent;
    }
    
  }
}
