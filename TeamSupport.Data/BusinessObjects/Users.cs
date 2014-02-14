using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;

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
        StringBuilder builder = new StringBuilder();
        if (!string.IsNullOrEmpty(this.FirstName))
        {
          builder.Append(this.FirstName);
        }

        if (!string.IsNullOrEmpty(this.LastName))
        {
          if (builder.Length > 0) builder.Append(" ");
          builder.Append(this.LastName);
        }

        return builder.Length > 0 ? builder.ToString() : "Unknown";
      }
    }
    public string FirstLastName
    {
      get
      {
        return DisplayName;
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

      name = builder.ToString();

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

    public void EmailCountToMuroc(bool isNew)
    { 
      UsersViewItem view = GetUserView();
      MailMessage message = new MailMessage();
      message.From = new MailAddress("support@teamsupport.com");
      message.To.Add("eharrington@teamsupport.com");
      message.To.Add("jhathaway@teamsupport.com");
      message.Subject = isNew ? "TeamSupport User Added" : "TeamSupport User Removed";
      message.Subject += " - " + view.Organization;
      int count = Organizations.GetUserCount(Collection.LoginUser, OrganizationID);
      message.IsBodyHtml = true;
      string body = @"
<div>{5}</div>
<table>
  <tr>
    <td>Organization:</td>
    <td>{0} ({1:D})</td>
  </tr>
  <tr>
    <td>User:</td>
    <td>{2} ({3:D})</td>
  </tr>
  <tr>
    <td>Total Active Count:</td>
    <td>{4:D}</td>
  </tr>
</table>";
      message.Body = string.Format(body, view.Organization, OrganizationID, FirstLastName, UserID, count, message.Subject);
      Emails.AddEmail(Collection.LoginUser, 1078, null, "User Count Changed", message);
    
    }

    public static int? GetIDByName(LoginUser loginUser, string name, int? parentID)
    {
      Users users = new Users(loginUser);
      users.LoadByName(name, loginUser.OrganizationID, true, false, false);
      if (users.IsEmpty) return null;
      else return users[0].UserID;
    }

    public static int? GetIDByEmail(LoginUser loginUser, string email, int? parentID)
    {
      Users users = new Users(loginUser);
      if (parentID != null)
      {
        users.LoadByEmail(email, (int)parentID);
      }
      else
      {
        users.LoadByEmail(loginUser.OrganizationID, email);
      }
      if (users.IsEmpty) return null;
      else return users[0].UserID;
    }

    public void FullReadFromXml(string data, bool isInsert, ref PhoneNumber phoneNumber, ref Address address)
    {
      //None of its fields are foreign keys. So we call the ReadFromXml method and then we add the phone and address fields.
      this.ReadFromXml(data, isInsert);

      LoginUser user = Collection.LoginUser;
      FieldMap fieldMap = Collection.FieldMap;

      StringReader reader = new StringReader(data);
      DataSet dataSet = new DataSet();
      dataSet.ReadXml(reader);

      try
      {
        object phoneTypeID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "PhoneTypeID", "PhoneType", PhoneType.GetIDByName, false, null, true);
        if (phoneTypeID != null) phoneNumber.PhoneTypeID = Convert.ToInt32(phoneTypeID);
      }
      catch
      {
      }

      try
      {
        object phoneNumberObject = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "PhoneNumber", string.Empty, null, false, null, true);
        if (phoneNumberObject != null) phoneNumber.Number = Convert.ToString(phoneNumberObject);
      }
      catch
      {
      }

      try
      {
        object phoneExtension = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "phoneExtension", string.Empty, null, false, null, true);
        if (phoneExtension != null) phoneNumber.Extension = Convert.ToString(phoneExtension);
      }
      catch
      {
      }

      try
      {
        object addressDescription = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "AddressDescription", string.Empty, null, false, null, true);
        if (addressDescription != null) address.Description = Convert.ToString(addressDescription);
      }
      catch
      {
      }

      try
      {
        object addressLine1 = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "AddressLine1", string.Empty, null, false, null, true);
        if (addressLine1 != null) address.Addr1 = Convert.ToString(addressLine1);
      }
      catch
      {
      }

      try
      {
        object addressLine2 = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "AddressLine2", string.Empty, null, false, null, true);
        if (addressLine2 != null) address.Addr2 = Convert.ToString(addressLine2);
      }
      catch
      {
      }

      try
      {
        object addressLine3 = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "AddressLine3", string.Empty, null, false, null, true);
        if (addressLine3 != null) address.Addr3 = Convert.ToString(addressLine3);
      }
      catch
      {
      }

      try
      {
        object addressCity = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "AddressCity", string.Empty, null, false, null, true);
        if (addressCity != null) address.City = Convert.ToString(addressCity);
      }
      catch
      {
      }

      try
      {
        object addressState = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "AddressState", string.Empty, null, false, null, true);
        if (addressState != null) address.State = Convert.ToString(addressState);
      }
      catch
      {
      }

      try
      {
        object addressZip = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "AddressZip", string.Empty, null, false, null, true);
        if (addressZip != null) address.Zip = Convert.ToString(addressZip);
      }
      catch
      {
      }

      try
      {
        object addressCountry = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "AddressCountry", string.Empty, null, false, null, true);
        if (addressCountry != null) address.Country = Convert.ToString(addressCountry);
      }
      catch
      {
      }
    }

    public string FontSizeDescription
    {
      get { return Enums.GetDescription((FontSize)Row["FontSize"]); }
    }

    public string FontFamilyDescription
    {
      get { return Enums.GetDescription((FontFamily)Row["FontFamily"]); }
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

    public void LoadByChatID(string chatID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT * FROM Users WHERE (AppChatID = @chatid)";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@chatid", chatID);
            Fill(command);
        }
    }

    public void LoadChatOnlineUsers(int organizationID, int userID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT *  FROM Users WHERE (AppChatstatus = 1) AND (organizationid = @orgid) and (AppChatID != '') and (userid != @userid)";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@orgid", organizationID);
            command.Parameters.AddWithValue("@userid", userID);
            Fill(command);
        }
    }

    public int GetChatConnections()
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT COUNT(*) FROM Users (AppChatstatus = 1) and (AppChatID != '')";
            command.CommandType = CommandType.Text;
            return (int)ExecuteScalar(command);
        }
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
    
    public void LoadByEmailIncludingDeleted(string email, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT u.*
                                FROM Users u 
                                WHERE (u.OrganizationID = @OrganizationID)
                                AND (u.Email = @Email)";


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
    public void LoadByOrganizationIDLastName(int organizationID, bool loadOnlyActive)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT *, LastName + ', ' + FirstName AS DisplayName FROM Users WHERE OrganizationID = @OrganizationID AND (@ActiveOnly = 0 OR IsActive = 1) AND (MarkDeleted = 0) ORDER BY LastName, FirstName";
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

    public void UpdateDeletedOrg(int organizationID, int unknownID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "UPDATE Users SET OrganizationID = @unknownID WHERE OrganizationID = @OrgID";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrgID", organizationID);
            command.Parameters.AddWithValue("@unknownID", unknownID);
            ExecuteNonQuery(command, "Users");

            command.CommandText = "UPDATE OrganizationTickets SET OrganizationID = @unknownID WHERE OrganizationID = @OrgID";
            command.CommandType = CommandType.Text;
            ExecuteNonQuery(command, "OrganizationTickets");


            command.CommandText =  @"DELETE FROM RecentlyViewedItems WHERE (refID = @orgID) AND (refType = 1)";
            command.CommandType = CommandType.Text;
            ExecuteNonQuery(command, "RecentlyViewedItems");
        }
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

    public void AddUserCustomer(int userID, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "INSERT INTO UserRightsOrganizations (UserID, OrganizationID) VALUES (@UserID, @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@UserID", userID);
        ExecuteNonQuery(command);
      }

      User user = (User)Users.GetUser(LoginUser, userID);
      Organization organization = Organizations.GetOrganization(LoginUser, organizationID);
      string description = string.Format("Added customer '{0}' to user '{1}'.", organization.Name, user.FirstLastName);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Groups, organizationID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, userID, description);
    }

    public void RemoveUserCustomer(int userID, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM UserRightsOrganizations WHERE UserID=@UserID AND OrganizationID=@OrganizationID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@UserID", userID);
        ExecuteNonQuery(command);
      }

      User user = (User)Users.GetUser(LoginUser, userID);
      Organization organization = Organizations.GetOrganization(LoginUser, organizationID);
      string description = string.Format("Removed customer '{0}' from user '{1}'.", organization.Name, user.FirstLastName);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Groups, organizationID, description);
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
AND u.MarkDeleted = 0
AND o.IsActive = 1
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
AND u.MarkDeleted = 0
AND o.IsActive = 1
AND u.IsPortalUser = 1";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command, "Users,Tickets");
      }
    }

    public void LoadBySalesForceID(string salesForceID, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Users WHERE (SalesForceID = @salesForceID) AND (OrganizationID = @organizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@salesForceID", salesForceID.Trim());
        command.Parameters.AddWithValue("@organizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadBySalesForceIDInParentOrganization(string salesForceID, int parentOrganizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = 
        @"
          SELECT 
            u.* 
          FROM 
            Users u
            JOIN Organizations o
              ON u.OrganizationID = o.OrganizationID
          WHERE
            u.SalesForceID = @salesForceID
            AND o.Parent = @parentOrganizationID
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@salesForceID", salesForceID.Trim());
        command.Parameters.AddWithValue("@organizationID", parentOrganizationID);
        Fill(command);
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
            (user.ImportID != null && user.ImportID.Trim().ToLower() == "[contact]" + importID) || 
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

    public static void CreateDefaultUsers(LoginUser loginUser)
    {
      Users users = new Users(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "";
        command.CommandType = CommandType.Text;


        Organization organization = Organizations.GetOrganization(loginUser, 1078);
        if (organization == null)
        {
          command.CommandText = @"
SET IDENTITY_INSERT [Organizations] ON
INSERT INTO [Organizations]
           (OrganizationID,
           [Name]
           ,[Description]
           ,[Website]
           ,[WhereHeard]
           ,[PromoCode]
           ,[IsCustomerFree]
           ,[UserSeats]
           ,[PortalSeats]
           ,[ChatSeats]
           ,[ExtraStorageUnits]
           ,[ImportID]
           ,[IsActive]
           ,[IsApiActive]
           ,[IsApiEnabled]
           ,[IsInventoryEnabled]
           ,[TimeZoneID]
           ,[InActiveReason]
           ,[HasPortalAccess]
           ,[IsAdvancedPortal]
           ,[IsBasicPortal]
           ,[PrimaryUserID]
           ,[DefaultPortalGroupID]
           ,[DefaultSupportGroupID]
           ,[DefaultSupportUserID]
           ,[ProductType]
           ,[ParentID]
           ,[WebServiceID]
           ,[SystemEmailID]
           ,[ChatID]
           ,[PortalGuid]
           ,[CRMLinkID]
           ,[SAExpirationDate]
           ,[APIRequestLimit]
           ,[DateCreated]
           ,[DateModified]
           ,[RequireNewKeyword]
           ,[RequireKnownUserForNewEmail]
           ,[EmailDelimiter]
           ,[OrganizationReplyToAddress]
           ,[CompanyDomains]
           ,[AdminOnlyCustomers]
           ,[AdminOnlyReports]
           ,[ShowWiki]
           ,[DefaultWikiArticleID]
           ,[SlaLevelID]
           ,[InternalSlaLevelID]
           ,[BusinessDays]
           ,[BusinessDayStart]
           ,[BusinessDayEnd]
           ,[UseEuropeDate]
           ,[CultureName]
           ,[TimedActionsRequired]
           ,[MatchEmailSubject]
           ,[CreatorID]
           ,[ModifierID]
           ,[PrimaryInterest]
           ,[PotentialSeats]
           ,[EvalProcess]
           ,[AddAdditionalContacts]
           ,[ChangeStatusIfClosed]
           ,[IsPublicArticles])
     VALUES
     (
1078,'Muroc Systems, Inc.','','',NULL,NULL,1,10,1000,10,0,NULL,1,1,1,1,'Central Standard Time','',1,1,1,43,16,NULL,NULL,2,1,'8B7E4E7E-E28E-438E-825B-F6F2A2A4DA65','58AECA7C-9B9B-4C7E-AD2D-EE79E2E69E3D','22BD89B8-5162-4509-8B0D-F209A0AA6EE9','57EE1F58-5C8B-4B47-B629-BE7C702A2022',NULL,NULL,5000,'2008-10-10 17:30:47.177','2011-10-28 01:21:03.057',0,0,NULL,'Support@TeamSupport.com','',0,0,1,26,NULL,8,62,'2010-02-22 15:00:00.000','2010-02-22 23:00:00.000',0,'en-US',0,0,43,43,NULL,NULL,NULL,1,1,1
     )
SET IDENTITY_INSERT [Organizations] OFF
";
          users.ExecuteNonQuery(command, "Organizations");
        }

        organization = Organizations.GetOrganization(loginUser, 1);
        if (organization == null)
        {
          command.CommandText = @"
SET IDENTITY_INSERT [Organizations] ON

INSERT INTO [Organizations]
           (OrganizationID,
           [Name]
           ,[Description]
           ,[Website]
           ,[WhereHeard]
           ,[PromoCode]
           ,[IsCustomerFree]
           ,[UserSeats]
           ,[PortalSeats]
           ,[ChatSeats]
           ,[ExtraStorageUnits]
           ,[ImportID]
           ,[IsActive]
           ,[IsApiActive]
           ,[IsApiEnabled]
           ,[IsInventoryEnabled]
           ,[TimeZoneID]
           ,[InActiveReason]
           ,[HasPortalAccess]
           ,[IsAdvancedPortal]
           ,[IsBasicPortal]
           ,[PrimaryUserID]
           ,[DefaultPortalGroupID]
           ,[DefaultSupportGroupID]
           ,[DefaultSupportUserID]
           ,[ProductType]
           ,[ParentID]
           ,[WebServiceID]
           ,[SystemEmailID]
           ,[ChatID]
           ,[PortalGuid]
           ,[CRMLinkID]
           ,[SAExpirationDate]
           ,[APIRequestLimit]
           ,[DateCreated]
           ,[DateModified]
           ,[RequireNewKeyword]
           ,[RequireKnownUserForNewEmail]
           ,[EmailDelimiter]
           ,[OrganizationReplyToAddress]
           ,[CompanyDomains]
           ,[AdminOnlyCustomers]
           ,[AdminOnlyReports]
           ,[ShowWiki]
           ,[DefaultWikiArticleID]
           ,[SlaLevelID]
           ,[InternalSlaLevelID]
           ,[BusinessDays]
           ,[BusinessDayStart]
           ,[BusinessDayEnd]
           ,[UseEuropeDate]
           ,[CultureName]
           ,[TimedActionsRequired]
           ,[MatchEmailSubject]
           ,[CreatorID]
           ,[ModifierID]
           ,[PrimaryInterest]
           ,[PotentialSeats]
           ,[EvalProcess]
           ,[AddAdditionalContacts]
           ,[ChangeStatusIfClosed]
           ,[IsPublicArticles])
     VALUES
     (
     1,'TeamSupport.com','',NULL,NULL,NULL,1,9999999,99999999,9999999,999999,NULL,1,1,0,0,'Central Standard Time',NULL,1,0,0,NULL,NULL,NULL,NULL,2,NULL,'863e9558-5f29-48bb-8cb9-c93eb77947cf','81e72c55-9b3b-4db0-8d88-b3a991d9672c','d9565c77-c78c-4176-9580-674a29f60de6','e266e1ce-d96e-4a1a-a46d-c18778723ad9',NULL,NULL,5000,'2008-06-20 05:00:00.000','2008-11-05 01:09:56.213',0,0,NULL,NULL,NULL,0,0,0,NULL,NULL,NULL,62,'2010-02-22 15:00:00.000','2010-02-22 23:00:00.000',0,'en-US',0,0,2,43,NULL,NULL,NULL,1,1,0
     )

SET IDENTITY_INSERT [Organizations] OFF
";
          users.ExecuteNonQuery(command, "Organizations");
        }

        User user = Users.GetUser(loginUser, 54);
        if (user == null)
        {
          command.CommandText = @"
SET IDENTITY_INSERT Users On

INSERT INTO [Users]
           (UserID,
           [Email]
           ,[FirstName]
           ,[MiddleName]
           ,[LastName]
           ,[Title]
           ,[CryptedPassword]
           ,[IsActive]
           ,[MarkDeleted]
           ,[TimeZoneID]
           ,[CultureName]
           ,[LastLogin]
           ,[LastActivity]
           ,[LastPing]
           ,[LastWaterCoolerID]
           ,[IsSystemAdmin]
           ,[IsFinanceAdmin]
           ,[IsPasswordExpired]
           ,[IsPortalUser]
           ,[IsChatUser]
           ,[PrimaryGroupID]
           ,[InOffice]
           ,[InOfficeComment]
           ,[ReceiveTicketNotifications]
           ,[ReceiveAllGroupNotifications]
           ,[SubscribeToNewTickets]
           ,[ActivatedOn]
           ,[DeactivatedOn]
           ,[OrganizationID]
           ,[LastVersion]
           ,[SessionID]
           ,[ImportID]
           ,[DateCreated]
           ,[DateModified]
           ,[CreatorID]
           ,[ModifierID]
           ,[OrgsUserCanSeeOnPortal]
           ,[DoNotAutoSubscribe])
     VALUES
           (
54,'kjones@murocsystems.com','Kevin','','Jones','','C6E3C4E8F7BFAFF9A1FC35C07BD5574E',1,0,'Central Standard Time','en-US','2011-11-15 05:20:46.050','2011-11-15 05:41:54.820','2011-11-15 06:04:28.203',17961,1,1,0,1,1,NULL,1,'',1,0,0,'2008-06-27 22:05:10.993',NULL,1,'1.2.9','C4735AE0-5C96-4969-B7C9-485E48167FFB',NULL,'2008-07-01 11:31:39.737','2011-11-15 05:41:54.823',30,-1,NULL,0           
           )

SET IDENTITY_INSERT Users Off
";
          users.ExecuteNonQuery(command, "Users");
        }

        user = Users.GetUser(loginUser, 34);
        if (user == null)
        {
          command.CommandText = @"
SET IDENTITY_INSERT Users On

INSERT INTO [Users]
           (UserID,
           [Email]
           ,[FirstName]
           ,[MiddleName]
           ,[LastName]
           ,[Title]
           ,[CryptedPassword]
           ,[IsActive]
           ,[MarkDeleted]
           ,[TimeZoneID]
           ,[CultureName]
           ,[LastLogin]
           ,[LastActivity]
           ,[LastPing]
           ,[LastWaterCoolerID]
           ,[IsSystemAdmin]
           ,[IsFinanceAdmin]
           ,[IsPasswordExpired]
           ,[IsPortalUser]
           ,[IsChatUser]
           ,[PrimaryGroupID]
           ,[InOffice]
           ,[InOfficeComment]
           ,[ReceiveTicketNotifications]
           ,[ReceiveAllGroupNotifications]
           ,[SubscribeToNewTickets]
           ,[ActivatedOn]
           ,[DeactivatedOn]
           ,[OrganizationID]
           ,[LastVersion]
           ,[SessionID]
           ,[ImportID]
           ,[DateCreated]
           ,[DateModified]
           ,[CreatorID]
           ,[ModifierID]
           ,[OrgsUserCanSeeOnPortal]
           ,[DoNotAutoSubscribe])
     VALUES
           (
34,'kjones@murocsystems.com','Kevin','','Jones','','C6E3C4E8F7BFAFF9A1FC35C07BD5574E',1,0,'Central Standard Time','en-US','2011-11-15 05:20:46.050','2011-11-15 05:41:54.820','2011-11-15 06:04:28.203',17961,1,1,0,1,1,NULL,1,'',1,0,0,'2008-06-27 22:05:10.993',NULL,1078,'1.2.9','C4735AE0-5C96-4969-B7C9-485E48167FFB',NULL,'2008-07-01 11:31:39.737','2011-11-15 05:41:54.823',30,-1,NULL,0           
           )

SET IDENTITY_INSERT Users Off

";
          users.ExecuteNonQuery(command, "Users");
        }
      }

    
    
    }

  }
}
