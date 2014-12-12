using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{

  public partial class Ticket
  {
    public TicketsViewItem GetTicketView()
    {
      return TicketsView.GetTicketsViewItem(BaseCollection.LoginUser, TicketID);
    }

    public void RemoveCommunityTicket()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM ForumTickets WHERE (TicketID = @TicketID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", TicketID);
        Collection.ExecuteNonQuery(command, "ForumTickets");
      }
    }

    public void AddCommunityTicket(int forumCategoryID)
    {
      RemoveCommunityTicket();
      ForumCategory cat = ForumCategories.GetForumCategory(Collection.LoginUser, forumCategoryID);
      if (cat == null) return;
      if (cat.OrganizationID != Collection.LoginUser.OrganizationID) return;

      ForumTicket ft = (new ForumTickets(Collection.LoginUser)).AddNewForumTicket();
      ft.TicketID = TicketID;
      ft.ForumCategory = forumCategoryID;
      ft.ViewCount = 0;
      ft.Collection.Save();
    }

    public static bool UserHasRights(User user, int? groupID, int? userID, int? ticketID, bool isKB)
    {
      if (isKB) return true;

      if (user.TicketRights == TicketRightType.Assigned && (userID == null || userID != user.UserID)) return false;
      
      if (user.TicketRights == TicketRightType.Groups && (userID != null || groupID != null))
      {
        if (userID == null || userID != user.UserID)
        {
          Groups groups = new Groups(user.Collection.LoginUser);
          if (groupID != null)
          {
            groups.LoadByUserID(user.UserID);
            foreach (Group group in groups)
            {
              if (group.GroupID == groupID) { return true; }
            }
          }
          return false;
        }
      }
      if (user.TicketRights == TicketRightType.Customers)
      {
        if (userID == null || userID != user.UserID)
        {
          SqlCommand command = new SqlCommand();
          command.CommandText = @"
SELECT COUNT(*) 
FROM OrganizationTickets ot
INNER JOIN UserRightsOrganizations uro ON ot.OrganizationID = uro.OrganizationID 
WHERE uro.UserID = @UserID
AND ot.TicketID = @TicketID
";
          command.Parameters.AddWithValue("UserID", user.UserID);
          command.Parameters.AddWithValue("TicketID", ticketID);
          return SqlExecutor.ExecuteInt(user.Collection.LoginUser, command) > 0;
        }
      }
      return true;
    }

    public static bool UserHasRights(User user, Ticket ticket)
    {
      return UserHasRights(user, ticket.GroupID, ticket.UserID, ticket.TicketID, ticket.IsKnowledgeBase);
    }

    public bool UserHasRights(User user)
    {
      return UserHasRights(user, this.GroupID, this.UserID, this.TicketID, this.IsKnowledgeBase);
    }

    public void FullReadFromXml(string data, bool isInsert, ref string description, ref int? contactID)
    {
      LoginUser user = Collection.LoginUser;
      FieldMap fieldMap = Collection.FieldMap;

      StringReader reader = new StringReader(data);
      DataSet dataSet = new DataSet();
      dataSet.ReadXml(reader);

      try
      {
        object name = (string)DataUtils.GetValueFromObject(user, fieldMap, dataSet, "Name", string.Empty, null, false, null);
        if (name != null) this.Name = Convert.ToString(name);
      }
      catch
      {
      }

      try
      {
        object descriptionObject = (string)DataUtils.GetValueFromObject(user, fieldMap, dataSet, "Description", string.Empty, null, false, null, true);
        if (descriptionObject != null) description = Convert.ToString(descriptionObject);
      }
      catch
      {
      }

      try
      {
        object isKnowledgeBase  = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "IsKnowledgeBase", string.Empty, null, false, null);
        if (isKnowledgeBase != null) this.IsKnowledgeBase = Convert.ToBoolean(isKnowledgeBase);
      }
      catch
      {
      }

      try
      {
        object isVisibleOnPortal = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "IsVisibleOnPortal", string.Empty, null, false, null);
        if (isVisibleOnPortal != null) this.IsVisibleOnPortal = Convert.ToBoolean(isVisibleOnPortal);
      }
      catch
      {
      }

      try
      {
        object ticketTypeID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "TicketTypeID", "TicketTypeName", TicketType.GetIDByName, false, null);
        if (ticketTypeID != null) this.TicketTypeID = Convert.ToInt32(ticketTypeID);
      }
      catch
      {
      }

      try
      {
        object ticketStatusID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "TicketStatusID", "Status", TicketStatus.GetIDByName, true, this.TicketTypeID);
        if (ticketStatusID != null) this.TicketStatusID = Convert.ToInt32(ticketStatusID);
      }
      catch
      {
      }

      try
      {
        object ticketSeverityID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "TicketSeverityID", "Severity", TicketSeverity.GetIDByName, false, null);
        if (ticketSeverityID != null) this.TicketSeverityID = Convert.ToInt32(ticketSeverityID);
      }
      catch
      {
      }

      try
      {
        object userID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "UserID", "UserEmail", User.GetIDByEmail, false, user.OrganizationID);
        if (userID != null) this.UserID = Convert.ToInt32(userID);
      }
      catch
      {
      }

      try
      {
        object groupID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "GroupID", "GroupName", Group.GetIDByName, false, null);
        if (groupID != null) this.GroupID = Convert.ToInt32(groupID);
      }
      catch
      {
      }

      try
      {
        object productID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "ProductID", "ProductName", Product.GetIDByName, false, null);
        if (productID != null) this.ProductID = Convert.ToInt32(productID);
      }
      catch
      {
      }

      try
      {
        object reportedVersionID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "ReportedVersionID", "ReportedVersion", ProductVersion.GetIDByName, true, this.ProductID);
        if (reportedVersionID != null) this.ReportedVersionID = Convert.ToInt32(reportedVersionID);
      }
      catch
      {
      }

      try
      {
        object solvedVersionID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "SolvedVersionID", "SolvedVersion", ProductVersion.GetIDByName, true, this.ProductID);
        if (solvedVersionID != null) this.SolvedVersionID = Convert.ToInt32(solvedVersionID);
      }
      catch
      {
      }

      try
      {
        object knowledgeBaseCategoryID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "KnowledgeBaseCategoryID", "KnowledgeBaseCategoryName", KnowledgeBaseCategory.GetIDByName, true, this.ProductID);
        if (knowledgeBaseCategoryID != null) this.KnowledgeBaseCategoryID = Convert.ToInt32(knowledgeBaseCategoryID);
      }
      catch
      {
      }

      try
      {
        object parentID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "ParentID", string.Empty, null, false, null);
        if (parentID != null) this.ParentID = Convert.ToInt32(parentID);
      }
      catch
      {
      }

      try
      {
        object dateCreated = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "DateCreated", string.Empty, null, false, null);
        if (dateCreated != null) this.DateCreated = Convert.ToDateTime(dateCreated);
      }
      catch
      {
      }

      try
      {
        object dateClosed = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "DateClosed", string.Empty, null, false, null);
        if (dateClosed != null) this.DateClosed = Convert.ToDateTime(dateClosed);
      }
      catch
      {
      }

      try
      {
        object closerID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "CloserID", "CloserEmail", User.GetIDByEmail, false, user.OrganizationID);
        if (closerID != null) this.CloserID = Convert.ToInt32(closerID);
      }
      catch
      {
      }

      try
      {
        object importID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "ImportID", string.Empty, null, false, null);
        if (importID != null) this.ImportID = Convert.ToString(importID);
      }
      catch
      {
      }

      try
      {
        object creatorID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "CreatorID", "CreatorEmail", User.GetIDByEmail, false, user.OrganizationID);
        if (creatorID != null) this.CreatorID = Convert.ToInt32(creatorID);
      }
      catch
      {
      }

      try
      {
        object contactIDObject = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "ContactID", "ContactEmail", User.GetIDByEmail, false, null, true);
        if (contactIDObject != null)
        {
          contactID = Convert.ToInt32(contactIDObject);
        }
        else
        {
          DataRow row = dataSet.Tables[0].Rows[0];
          DataColumn column = dataSet.Tables[0].Columns["ContactEmail"];
          if (row[column] != DBNull.Value && row[column].ToString().Trim() != string.Empty)
          {
            Users newUserCollection = new Users(user);
            User newUser = newUserCollection.AddNewUser();
            newUser.Email = row[column].ToString().Trim();
            newUser.LastName = newUser.Email;
            newUser.IsActive = true;
            newUser.ActivatedOn = DateTime.UtcNow;
            newUser.LastLogin = DateTime.UtcNow;
            newUser.LastActivity = DateTime.UtcNow.AddHours(-1);
            newUser.IsPasswordExpired = true;

            if (newUser.Email.Contains("@"))
            {
              Organizations matchDomainCompany = new Organizations(user);
              matchDomainCompany.LoadFirstDomainMatch(user.OrganizationID, newUser.Email.Substring(newUser.Email.LastIndexOf("@") + 1));
              if (!matchDomainCompany.IsEmpty)
              {
                newUser.OrganizationID = matchDomainCompany[0].OrganizationID;
              }
            }

            if (newUser.OrganizationID == 0)
            {
              newUser.OrganizationID = Organizations.GetUnknownCompanyID(user);
            }

            newUserCollection.Save();
            contactID = newUser.UserID;
          }
        }
      }
      catch
      {
      }
    }

    public void UpdateSalesForceData()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
          UPDATE
            Tickets
          SET
            SalesForceID = @SalesForceID,
            DateModifiedBySalesForceSync = @DateModifiedBySalesForceSync
          WHERE
            TicketID = @TicketID
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@SalesForceID", this.SalesForceID);
        command.Parameters.AddWithValue("@DateModifiedBySalesForceSync", this.DateModifiedBySalesForceSyncUtc);
        command.Parameters.AddWithValue("@TicketID", this.TicketID);
        this.BaseCollection.ExecuteNonQuery(command, "Tickets");
      }
    }

  }

  public partial class Tickets 
  {
    private string _actionLogInstantMessage = null;

    public string ActionLogInstantMessage
    {
      get
      {
        return _actionLogInstantMessage;
      }
      set
      {
        _actionLogInstantMessage = value;
      }
    }

    partial void BeforeRowInsert(Ticket ticket)
    {
      if (ticket.TicketNumber < 2)
      {
        ticket.TicketNumber = GetMaxTicketNumber(ticket.OrganizationID) + 1;
        if (ticket.TicketNumber < 2) ticket.TicketNumber = 1000;
        //if (ticket.TicketNumber < 1) ticket.TicketNumber = 1;

        // CHECK IF TICKET CLOSED
        TicketStatus ticketStatus = TicketStatuses.GetTicketStatus(LoginUser, ticket.TicketStatusID);
        if (ticketStatus.IsClosed)
        {
          ticket.CloserID = LoginUser.UserID;
          ticket.DateClosed = DateTime.UtcNow;
        }
      }
      ticket.Name = (ticket.Row["Name"] == DBNull.Value) ? string.Empty : ticket.Name;
    }

    partial void AfterRowInsert(Ticket ticket)
    {
      string description = "Created Ticket Number ";
      if (_actionLogInstantMessage != null) 
      {
        description = _actionLogInstantMessage;
      }
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, ticket.TicketID, description + ticket.TicketNumber.ToString());
      _actionLogInstantMessage = null;

      // CHECK IF TICKET CLOSED
      TicketStatus ticketStatus = TicketStatuses.GetTicketStatus(LoginUser, ticket.TicketStatusID);
      if (ticketStatus.IsClosed)
      {
          ticket.CloserID = LoginUser.UserID;
          ticket.DateClosed = DateTime.UtcNow;

          description = "Closed " + GetTicketLink(ticket);
          ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
      }

    }

    public static int GetMyOpenUnreadTicketCount(LoginUser loginUser, int userID)
    {
      int cnt = 0;
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
SELECT COUNT(*) FROM Tickets t
LEFT JOIN UserTicketStatusesView utsv ON t.TicketID=utsv.TicketID AND utsv.ViewerID = @ViewerID
LEFT JOIN TicketStatuses ts ON ts.TicketStatusID = t.TicketStatusID
WHERE t.UserID=@ViewerID
AND utsv.IsRead = 0
AND ts.IsClosed = 0";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ViewerID", userID);

        object o = SqlExecutor.ExecuteScalar(loginUser, command);
        if (o == DBNull.Value) return -1;
        cnt = (int)o;
      }
      return cnt;      
    }

    private bool _updateChildTickets = true;
    public void UpdateChildTickets(Ticket ticket)
    { 
      Tickets tickets = new Tickets(LoginUser);
      tickets.LoadChildren(ticket.TicketID);
      tickets._updateChildTickets = false;

      foreach (Ticket item in tickets)
	    {
		    if (item.TicketTypeID == ticket.TicketTypeID)
          item.TicketStatusID = ticket.TicketStatusID;
	    }
      tickets.Save();

    }
    
    partial void AfterRowEdit(Ticket ticket)
    { 
      if (_updateChildTickets) UpdateChildTickets(ticket);
    }

    partial void BeforeRowDelete(int ticketID)
    {
      Tickets.DeleteRelationships(LoginUser, ticketID);
      
      Ticket ticket = (Ticket) Tickets.GetTicket(LoginUser, ticketID);

      string description = "Deleted Ticket Number " + ticket.TicketNumber.ToString();
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Tickets, ticket.TicketID, description);


    }

    public static string GetTicketLink(Ticket ticket)
    {
    
//      return "<a class=\"actionLogLink\" href=\"Ticket.aspx?ticketid=" + ticket.TicketID + "\">Ticket Number " + ticket.TicketNumber.ToString() + "</a> ";

      return "<a class=\"actionLogLink\" href=\"javascript:openTicketWindow("+ticket.TicketID.ToString()+");\">Ticket Number " + ticket.TicketNumber.ToString() + "</a> ";
    }

    public static string GetTicketLink(LoginUser loginUser, int ticketID)
    {
      Ticket ticket = (Ticket)Tickets.GetTicket(loginUser, ticketID);
      return GetTicketLink(ticket);
    }

    partial void BeforeRowEdit(Ticket ticket)
    {
      TicketGridViewItem oldTicketView = (TicketGridViewItem)TicketGridView.GetTicketGridViewItem(LoginUser, ticket.TicketID);
      string description = "";
      string name1;
      string name2;

      
      if (oldTicketView.GroupID != ticket.GroupID)
      {
        if (oldTicketView.GroupID == null) { name1 = "Unassigned"; }  else { name1 = oldTicketView.GroupName; }
        if (ticket.GroupID == null) { name2 = "Unassigned"; } else { Group group = (Group)Groups.GetGroup(LoginUser, (int)ticket.GroupID); name2 = group.Name; }
        description = "Reassigned " + GetTicketLink(ticket) + " from group '" + name1 + "' to group '" + name2 + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
        if (oldTicketView.GroupID != null)
          ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Groups, (int)oldTicketView.GroupID, description);
      }

      if (oldTicketView.IsKnowledgeBase != ticket.IsKnowledgeBase)
      {
        if (ticket.IsKnowledgeBase)
          description = "Added " + GetTicketLink(ticket) + " to the knowledge base.";
        else
          description = "Removed " + GetTicketLink(ticket) + " from the knowledge base.";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
      }

      if (oldTicketView.IsVisibleOnPortal != ticket.IsVisibleOnPortal)
      {
        if (ticket.IsVisibleOnPortal)
          description = "Visible to customers set to true.";
        else
          description = "Visible to customers set to false.";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
      }

      ticket.Name = (ticket.Row["Name"] == DBNull.Value) ? string.Empty : ticket.Name;
      if (oldTicketView.Name != ticket.Name)
      {
        description = "Changed ticket name from '" + oldTicketView.Name + "' to '" + ticket.Name + "' for " + GetTicketLink(ticket);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
      }


      if (oldTicketView.ProductID != ticket.ProductID)
      {
        if (oldTicketView.ProductID == null) { name1 = "Unassigned"; } else { name1 = oldTicketView.ProductName; }
        if (ticket.ProductID == null) { name2 = "Unassigned"; } else { Product product = (Product)Products.GetProduct(LoginUser, (int)ticket.ProductID); name2 = product.Name; }
        description = "Changed  product from '" + name1 + "' to '" + name2 + "' for " + GetTicketLink(ticket);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
        if (oldTicketView.ProductID != null)
          ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Products, (int)oldTicketView.ProductID, description);
      }

      if (oldTicketView.ReportedVersionID != ticket.ReportedVersionID)
      {
        if (oldTicketView.ReportedVersionID == null) { name1 = "Unassigned"; } else { name1 = oldTicketView.ReportedVersion; }
        if (ticket.ReportedVersionID == null) { name2 = "Unassigned"; } else { ProductVersion productVersion = (ProductVersion)ProductVersions.GetProductVersion(LoginUser, (int)ticket.ReportedVersionID); name2 = productVersion.VersionNumber; }
        description = "Changed reported version from '" + name1 + "' to '" + name2 + "' for " + GetTicketLink(ticket);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
        if (oldTicketView.ReportedVersionID != null)
          ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.ProductVersions, (int)oldTicketView.ReportedVersionID, description);
      }

      if (oldTicketView.SolvedVersionID != ticket.SolvedVersionID)
      {
        if (oldTicketView.SolvedVersionID == null) { name1 = "Unassigned"; } else { name1 = oldTicketView.SolvedVersion; }
        if (ticket.SolvedVersionID == null) { name2 = "Unassigned"; } else { ProductVersion productVersion = (ProductVersion)ProductVersions.GetProductVersion(LoginUser, (int)ticket.SolvedVersionID); name2 = productVersion.VersionNumber; }
        description = "Changed resolved version from '" + name1 + "' to '" + name2 + "' for " + GetTicketLink(ticket);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
        if (oldTicketView.SolvedVersionID != null)
          ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.ProductVersions, (int)oldTicketView.SolvedVersionID, description);
      }

      if (oldTicketView.TicketSeverityID != ticket.TicketSeverityID)
      {
        name1 = oldTicketView.Severity; 
        TicketSeverity ticketSeverity = (TicketSeverity) TicketSeverities.GetTicketSeverity(LoginUser, (int)ticket.TicketSeverityID); 
        name2 = ticketSeverity.Name;
        description = "Changed severity from '" + name1 + "' to '" + name2 + "' for " + GetTicketLink(ticket);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
      }

      if (oldTicketView.TicketStatusID != ticket.TicketStatusID)
      {
        name1 = oldTicketView.Status;
        TicketStatus ticketStatus = (TicketStatus)TicketStatuses.GetTicketStatus(LoginUser, (int)ticket.TicketStatusID);
        name2 = ticketStatus.Name;
        description = "Changed status from '" + name1 + "' to '" + name2 + "' for " + GetTicketLink(ticket);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
      }

      if (oldTicketView.TicketTypeID != ticket.TicketTypeID)
      {
        name1 = oldTicketView.TicketTypeName;
        TicketType ticketType = (TicketType)TicketTypes.GetTicketType(LoginUser, (int)ticket.TicketTypeID);
        name2 = ticketType.Name;
        description = "Changed ticket type from '" + name1 + "' to '" + name2 + "' for " + GetTicketLink(ticket);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
      }

      if (oldTicketView.UserID != ticket.UserID)
      {
        if (oldTicketView.UserID == null) { name1 = "Unassigned"; } else { name1 = oldTicketView.UserName; }
        if (ticket.UserID == null) { name2 = "Unassigned"; } else { User u = (User)Users.GetUser(LoginUser, (int)ticket.UserID); name2 = u.FirstName + " " + u.LastName; }
        description = "Reassigned " + GetTicketLink(ticket) + " from user '" + name1 + "' to user '" + name2 + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
        if (oldTicketView.UserID != null)
          ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, (int)oldTicketView.UserID, description);
      }

      // CHECK IF TICKET CLOSED
      if (oldTicketView.TicketStatusID != ticket.TicketStatusID)
      {
        bool oldClosed = oldTicketView.IsClosed;
        TicketStatus ticketStatus = (TicketStatus)TicketStatuses.GetTicketStatus(LoginUser, (int)ticket.TicketStatusID);
        bool newClosed = ticketStatus.IsClosed;
        if (newClosed)
        {
          if (!oldClosed)
          {
            description = "Closed " + GetTicketLink(ticket);
            ticket.CloserID = LoginUser.UserID;
            ticket.DateClosed = DateTime.UtcNow;
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
          }
        }
        else
        {
          if (oldClosed)
          {
            description = "Reopened " + GetTicketLink(ticket);
            ticket.CloserID = null;
            ticket.DateClosed = null;
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
          }
        }
      }
    }

    public int GetMaxTicketNumber(int organizationID)
    {
      int max = -1;

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT MAX(TicketNumber) FROM Tickets WHERE (OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        object o = ExecuteScalar(command);
        if (o == DBNull.Value) return -1;
        max = (int)o;
      }
      return max;
    }

    public void LoadRelated(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT t.* FROM Tickets t WHERE t.TicketID IN (SELECT Ticket2ID FROM TicketRelationships WHERE Ticket1ID = @TicketID) OR t.TicketID IN (SELECT Ticket1ID FROM TicketRelationships WHERE Ticket2ID = @TicketID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadChildren(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT t.* FROM Tickets t WHERE t.ParentID = @TicketID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    /// <summary>
    /// Loads Tickets for a TeamSupport Customer.  All the tickets in the user's organization.
    /// </summary>
    /// <param name="organizationID"></param>

    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Tickets WHERE (OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command, "Tickets,Actions");
      }
    }

    public Ticket FindByImportID(string importID)
    {
      importID = importID.Trim().ToLower();
      foreach (Ticket ticket in this)
      {
        if (!string.IsNullOrWhiteSpace(ticket.ImportID) && ticket.ImportID.Trim().ToLower() == importID)
        {
          return ticket;
        }
      }
      return null;
    }    

    public void LoadByUserID(int? userID, int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketGridView WHERE (UserID = @UserID) AND (TicketTypeID = @TicketTypeID) AND (OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        if (userID == null) command.Parameters.AddWithValue("@UserID", DBNull.Value);
        else command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        Fill(command, "TicketGridView,Actions");
      }
    }

    public void LoadByGroupID(int? groupID, int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketGridView WHERE (GroupID = @GroupID) AND (TicketTypeID = @TicketTypeID) AND (OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        if (groupID == null) command.Parameters.AddWithValue("@GroupID", DBNull.Value);
        else command.Parameters.AddWithValue("@GroupID", groupID);

        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        Fill(command, "TicketGridView,Actions");
      }
    }

    public void LoadByIsClosed(bool isClosed, int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketGridView WHERE (IsClosed = @IsClosed) AND (TicketTypeID = @TicketTypeID) AND (OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@IsClosed", isClosed);
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        Fill(command, "TicketGridView,Actions");
      }
    }

    public void LoadByTicketType(int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Tickets WHERE (TicketTypeID = @TicketTypeID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
        Fill(command, "Tickets");
      }
    }

    public void LoadByCRMLinkItem(CRMLinkTableItem item) {
        using (SqlCommand command = new SqlCommand()) {
            command.CommandText = @"
              SELECT 
                *
              FROM
                Tickets t 
                JOIN OrganizationTickets ot
                  ON t.TicketID = ot.TicketID
                JOIN Organizations o 
                  ON o.OrganizationID = ot.OrganizationID
                LEFT JOIN TicketStatuses ts
                  ON t.TicketStatusID = ts.TicketStatusID
              WHERE 
                t.OrganizationID = @OrgID 
                AND 
                (
                  (
                    @LastTicketID > 0
                    AND t.TicketID > @LastTicketID
                  )
                  OR
                  (
                    @LastTicketID <= 0
                    AND ts.IsClosed = 0
                  )
                )
                AND ISNULL(o.CRMLinkID,'') <> ''
            ";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrgID", item.OrganizationID);
            command.Parameters.AddWithValue("@LastTicketID", item.LastTicketID);
            
            Fill(command, "Tickets");
        }
    }

    public void AddOrganization(int organizationID, int ticketID)
    {
      if (GetAssociatedOrganizationCount(LoginUser, organizationID, ticketID) > 0) return;
      
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "INSERT INTO OrganizationTickets (TicketID, OrganizationID, DateCreated, CreatorID, DateModified, ModifierID) VALUES (@TicketID, @OrganizationID, @DateCreated, @CreatorID, @DateModified, @ModifierID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@TicketID", ticketID);
        command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
        command.Parameters.AddWithValue("@CreatorID", LoginUser.UserID);
        command.Parameters.AddWithValue("@DateModified", DateTime.UtcNow);
        command.Parameters.AddWithValue("@ModifierID", LoginUser.UserID);
        ExecuteNonQuery(command, "OrganizationTickets");
      }


      Organization org = (Organization)Organizations.GetOrganization(LoginUser, organizationID);
      Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, ticketID);
      string description = "Added '" + org.Name + "' to the customer list for " + GetTicketLink(ticket);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, ticketID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Organizations, organizationID, description);
    }

    public static int GetAssetCount(LoginUser loginUser, int assetID, int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT COUNT(*) FROM AssetTickets WHERE (TicketID = @TicketID) AND (AssetID = @AssetID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@AssetID", assetID);
        command.Parameters.AddWithValue("@TicketID", ticketID);

        Tickets tickets = new Tickets(loginUser);
        return (int)tickets.ExecuteScalar(command);
      }
    }


    public void AddAsset(int assetID, int ticketID)
    {
      Asset asset = Assets.GetAsset(LoginUser, assetID);
      Ticket ticket = Tickets.GetTicket(LoginUser, ticketID);
      if (asset.OrganizationID != ticket.OrganizationID) return;
      if (GetAssetCount(LoginUser, assetID, ticketID) > 0) return;

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "INSERT INTO AssetTickets (TicketID, AssetID, DateCreated, CreatorID) VALUES (@TicketID, @AssetID, @DateCreated, @CreatorID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@AssetID", assetID);
        command.Parameters.AddWithValue("@TicketID", ticketID);
        command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
        command.Parameters.AddWithValue("@CreatorID", LoginUser.UserID);
        ExecuteNonQuery(command, "AssetTickets");
      }

      string description = "Added '" + asset.Name + "' to the asset list for " + GetTicketLink(ticket);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, ticketID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Assets, assetID, description);
    }


    public void RemoveAsset(int assetID, int ticketID)
    {
      Asset asset = Assets.GetAsset(LoginUser, assetID);
      Ticket ticket = Tickets.GetTicket(LoginUser, ticketID);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM AssetTickets WHERE (TicketID = @TicketID) AND (AssetID = @AssetID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@AssetID", assetID);
        command.Parameters.AddWithValue("@TicketID", ticketID);
        ExecuteNonQuery(command, "AssetTickets");
      }

      string description = "Removed '" + asset.Name + "' to the asset list for " + GetTicketLink(ticket);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Tickets, ticketID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Assets, assetID, description);
    }


    public static void DeleteRelationships(LoginUser loginUser, int ticketID)
    {
      Tickets tickets = new Tickets(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM TicketRelationships WHERE (Ticket1ID = @TicketID) OR (Ticket2ID = @TicketID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        tickets.ExecuteNonQuery(command, "TicketRelationships");
      }
    }

    public void RemoveOrganization(int organizationID, int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM OrganizationTickets WHERE (TicketID = @TicketID) AND (OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@TicketID", ticketID);
        ExecuteNonQuery(command, "OrganizationTickets");
      }
      Organization org = (Organization)Organizations.GetOrganization(LoginUser, organizationID);
      Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, ticketID);
      string description = "Removed '" + org.Name + "' from the customer list for " + GetTicketLink(ticket);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Tickets, ticketID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Organizations, organizationID, description);
    }

    public void AddContact(int userID, int ticketID)
    {
      try
      {

        using (SqlCommand command = new SqlCommand())
        {
          command.CommandText = "INSERT INTO UserTickets (TicketID, UserID, DateCreated, CreatorID) VALUES (@TicketID, @UserID, @DateCreated, @CreatorID)";
          command.CommandType = CommandType.Text;
          command.Parameters.AddWithValue("@UserID", userID);
          command.Parameters.AddWithValue("@TicketID", ticketID);
          command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
          command.Parameters.AddWithValue("@CreatorID", LoginUser.UserID);
          ExecuteNonQuery(command, "UserTickets");
        }
      }
      catch(Exception)
      {
      }


      UsersViewItem user = UsersView.GetUsersViewItem(LoginUser, userID);

      AddOrganization(user.OrganizationID, ticketID);
      Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, ticketID);
      string description = "Added '" + user.FirstName + " " + user.LastName + "' to the contact list for " + GetTicketLink(ticket);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, ticketID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, userID, description);
    }

    public void SetUserAsSentToSalesForce(int userID, int ticketID)
    {
      try
      {
        using (SqlCommand command = new SqlCommand())
        {
          command.CommandText = "UPDATE OrganizationTickets SET SentToSalesForce = 0 WHERE TicketID = @TicketID";
          command.CommandType = CommandType.Text;
          command.Parameters.AddWithValue("@TicketID", ticketID);
          ExecuteNonQuery(command, "UserTickets");
        }

        using (SqlCommand command = new SqlCommand())
        {
          command.CommandText = "UPDATE UserTickets SET SentToSalesForce = 0 WHERE TicketID = @TicketID";
          command.CommandType = CommandType.Text;
          command.Parameters.AddWithValue("@TicketID", ticketID);
          ExecuteNonQuery(command, "UserTickets");
        }

        using (SqlCommand command = new SqlCommand())
        {
          command.CommandText = "UPDATE UserTickets SET SentToSalesForce = 1 WHERE UserID = @UserID AND TicketID = @TicketID";
          command.CommandType = CommandType.Text;
          command.Parameters.AddWithValue("@UserID", userID);
          command.Parameters.AddWithValue("@TicketID", ticketID);
          ExecuteNonQuery(command, "UserTickets");
        }
      }
      catch (Exception)
      {
      }
    }

    public void SetOrganizationAsSentToSalesForce(int organizationID, int ticketID)
    {
      try
      {
        using (SqlCommand command = new SqlCommand())
        {
          command.CommandText = "UPDATE OrganizationTickets SET SentToSalesForce = 0 WHERE TicketID = @TicketID";
          command.CommandType = CommandType.Text;
          command.Parameters.AddWithValue("@TicketID", ticketID);
          ExecuteNonQuery(command, "UserTickets");
        }

        using (SqlCommand command = new SqlCommand())
        {
          command.CommandText = "UPDATE UserTickets SET SentToSalesForce = 0 WHERE TicketID = @TicketID";
          command.CommandType = CommandType.Text;
          command.Parameters.AddWithValue("@TicketID", ticketID);
          ExecuteNonQuery(command, "UserTickets");
        }

        using (SqlCommand command = new SqlCommand())
        {
          command.CommandText = "UPDATE OrganizationTickets SET SentToSalesForce = 1 WHERE OrganizationID = @organizationID AND TicketID = @TicketID";
          command.CommandType = CommandType.Text;
          command.Parameters.AddWithValue("@organizationID", organizationID);
          command.Parameters.AddWithValue("@TicketID", ticketID);
          ExecuteNonQuery(command, "UserTickets");
        }
      }
      catch (Exception)
      {
      }
    }

    public void LoadByContact(int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT t.* FROM Tickets t WHERE t.TicketID IN (SELECT TicketID FROM UserTickets WHERE UserID = @UserID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command);
      }
    }

    public void RemoveContact(int userID, int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM UserTickets WHERE (TicketID = @TicketID) AND (UserID = @UserID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@TicketID", ticketID);
        ExecuteNonQuery(command, "UserTickets");
      }
      UsersViewItem user = UsersView.GetUsersViewItem(LoginUser, userID);

      if (GetAssociatedContactCount(LoginUser, user.OrganizationID, ticketID) < 1) 
      {
        RemoveOrganization(user.OrganizationID, ticketID);
      }

      Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, ticketID);
      string description = "Removed '" + user.FirstName + " " + user.LastName + "' from the contact list for " + GetTicketLink(ticket);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Tickets, ticketID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Users, userID, description);
    }

    private void AddGridParameter(SqlCommand command, string name, int id)
    {
      if (id > -1)
      {
        command.CommandText = command.CommandText + " AND (" + name + " = @" + name + ")";
        command.Parameters.AddWithValue("@" + name, id);
      }
      else if (id == -2)
      {
        command.CommandText = command.CommandText + " AND (" + name + " is null)";
      }
      else if (id == -3)
      {
        command.CommandText = command.CommandText + " AND (IsClosed = 0)";
      }
      else if (id == -4)
      {
        command.CommandText = command.CommandText + " AND (IsClosed = 1)";
      }
    }

    public int LoadForGridCount(int organizationID, int ticketTypeID, int ticketStatusID, int ticketSeverityID,
      int userID, int groupID, int productID, int reportedVersionID, int resolvedVersionID,
      int customerID, bool? onlyPortal, bool? onlyKnowledgeBase,
      DateTime? dateCreatedBegin, DateTime? dateCreatedEnd, DateTime? dateModifiedBegin, DateTime? dateModifiedEnd,
      string search)
    {
      if (search.Trim() == "") search = "\"\"";
      using (SqlCommand command = new SqlCommand())
      {
        StringBuilder builder = new StringBuilder();
        builder.Append(@" SELECT COUNT(*)
                          FROM dbo.TicketGridView tgv LEFT JOIN Tickets t ON tgv.TicketID = t.TicketID
                          WHERE (tgv.OrganizationID = @OrganizationID)
                          AND ((tgv.TicketTypeID = @TicketTypeID) OR (@TicketTypeID = -1))
                          AND ((tgv.TicketSeverityID = @TicketSeverityID) OR (@TicketSeverityID = -1))
                          AND ((tgv.ProductID = @ProductID) OR (@ProductID = -1))
                          AND ((tgv.ReportedVersionID = @ReportedVersionID) OR (@ReportedVersionID = -1))
                          AND ((tgv.SolvedVersionID = @ResolvedVersionID) OR (@ResolvedVersionID = -1))
                          AND (((@UserID = -2) AND (tgv.UserID IS NULL)) OR (@UserID = tgv.UserID) OR (@UserID = -1))
                          AND (((@GroupID = -2) AND (tgv.GroupID IS NULL)) OR (@GroupID = tgv.GroupID) OR (@GroupID = -1))
                          AND ((tgv.TicketStatusID = @TicketStatusID) OR (@TicketStatusID = -1) OR ((@TicketStatusID = -3) AND (tgv.IsClosed = 0)) OR ((@TicketStatusID = -4) AND (tgv.IsClosed = 1)))
                          AND ((@CustomerID = -1) OR (EXISTS(SELECT * FROM OrganizationTickets ot WHERE (ot.OrganizationID = @CustomerID) AND (ot.TicketID = tgv.TicketID))))
                          AND ((@IsPortal is null) OR (tgv.IsVisibleOnPortal = @IsPortal))
                          AND ((@IsKnowledgeBase is null) OR (tgv.IsKnowledgeBase = @IsKnowledgeBase))
                          AND ((@DateCreatedBegin is null) OR (tgv.DateCreated >= @DateCreatedBegin))
                          AND ((@DateCreatedEnd is null) OR (tgv.DateCreated <= @DateCreatedEnd))
                          AND ((@DateModifiedBegin is null) OR (tgv.DateModified >= @DateModifiedBegin))
                          AND ((@DateModifiedEnd is null) OR (tgv.DateModified <= @DateModifiedEnd))
                          AND ((@Search = '""""') 
                                OR (CONTAINS((t.[Name]), @Search))
                                OR EXISTS (SELECT * FROM Actions a WHERE (a.TicketID = tgv.TicketID) AND CONTAINS((a.[Description], a.[Name]), @Search))
                                OR EXISTS (SELECT * FROM CustomValues cv 
                                        LEFT JOIN CustomFields cf ON cv.CustomFieldID = cf.CustomFieldID 
                                        WHERE (cf.RefType = 17)
                                        AND (cv.RefID = tgv.TicketID)
                                        AND CONTAINS((cv.[CustomValue]), @Search))
                                OR (tgv.TicketNumber LIKE '%'+@SearchClean+'%'))
                        ");
                      
        command.CommandText = builder.ToString();            
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
        command.Parameters.AddWithValue("@TicketStatusID", ticketStatusID);
        command.Parameters.AddWithValue("@TicketSeverityID", ticketSeverityID);
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@GroupID", groupID);
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@ReportedVersionID", reportedVersionID);
        command.Parameters.AddWithValue("@ResolvedVersionID", resolvedVersionID);
        command.Parameters.AddWithValue("@CustomerID", customerID);
        command.Parameters.AddWithValue("@IsPortal", onlyPortal == null ? (object)DBNull.Value : onlyPortal);
        command.Parameters.AddWithValue("@IsKnowledgeBase", onlyKnowledgeBase == null ? (object)DBNull.Value : onlyKnowledgeBase);
        command.Parameters.AddWithValue("@DateCreatedBegin", dateCreatedBegin == null ? (object)DBNull.Value : dateCreatedBegin);
        command.Parameters.AddWithValue("@DateCreatedEnd", dateCreatedEnd == null ? (object)DBNull.Value : dateCreatedEnd);
        command.Parameters.AddWithValue("@DateModifiedBegin", dateModifiedBegin == null ? (object)DBNull.Value : dateModifiedBegin);
        command.Parameters.AddWithValue("@DateModifiedEnd", dateModifiedEnd == null ? (object)DBNull.Value : dateModifiedEnd);
        command.Parameters.AddWithValue("@Search", search);
        command.Parameters.AddWithValue("@SearchClean", search.Replace("*", "").Replace("%", "").Replace("\"", ""));

        return (int)ExecuteScalar(command, "TicketGridView,Actions");
      }
    }


    public void LoadForGrid(int pageIndex, int pageSize, int organizationID, int ticketTypeID, int ticketStatusID, int ticketSeverityID,
      int userID, int groupID, int productID, int reportedVersionID, int resolvedVersionID,
      int customerID, bool? onlyPortal, bool? onlyKnowledgeBase,
      DateTime? dateCreatedBegin, DateTime? dateCreatedEnd, DateTime? dateModifiedBegin, DateTime? dateModifiedEnd,
      string search, string sortColumn, bool sortAsc)
    {
      if (search.Trim() == "") search = @"""""";
    
      using (SqlCommand command = new SqlCommand())
      {
        string sort = sortColumn;
        switch (sortColumn)
        {
          case "Severity": sort = "SeverityPosition"; break;
          case "Status": sort = "StatusPosition"; break;
          default: break;
        }
        StringBuilder builder = new StringBuilder();
        builder.Append("WITH TicketRows AS (SELECT ROW_NUMBER() OVER (ORDER BY tgv.");
        builder.Append(sort);
        if (sortAsc) builder.Append(" ASC");
        else builder.Append(" DESC");
        builder.Append(") AS RowNumber, tgv.*");
        builder.Append(@" 
                              	  
                                  
                                  FROM TicketGridView tgv LEFT JOIN Tickets t ON tgv.TicketID = t.TicketID
                                  WHERE (tgv.OrganizationID = @OrganizationID)
                                  AND ((tgv.TicketTypeID = @TicketTypeID) OR (@TicketTypeID = -1))
                                  AND ((tgv.TicketSeverityID = @TicketSeverityID) OR (@TicketSeverityID = -1))
                                  AND ((tgv.ProductID = @ProductID) OR (@ProductID = -1))
                                  AND ((tgv.ReportedVersionID = @ReportedVersionID) OR (@ReportedVersionID = -1))
                                  AND ((tgv.SolvedVersionID = @ResolvedVersionID) OR (@ResolvedVersionID = -1))
                                  AND (((@UserID = -2) AND (tgv.UserID IS NULL)) OR (@UserID = tgv.UserID) OR (@UserID = -1))
                                  AND (((@GroupID = -2) AND (tgv.GroupID IS NULL)) OR (@GroupID = tgv.GroupID) OR (@GroupID = -1))
                                  AND ((tgv.TicketStatusID = @TicketStatusID) OR (@TicketStatusID = -1) OR ((@TicketStatusID = -3) AND (tgv.IsClosed = 0)) OR ((@TicketStatusID = -4) AND (tgv.IsClosed = 1)))
                                  AND ((@CustomerID = -1) OR (EXISTS(SELECT * FROM OrganizationTickets ot WHERE (ot.OrganizationID = @CustomerID) AND (ot.TicketID = tgv.TicketID))))
                                  AND ((@IsPortal is null) OR (tgv.IsVisibleOnPortal = @IsPortal))
                                  AND ((@IsKnowledgeBase is null) OR (tgv.IsKnowledgeBase = @IsKnowledgeBase))
                                  AND ((@DateCreatedBegin is null) OR (tgv.DateCreated >= @DateCreatedBegin))
                                  AND ((@DateCreatedEnd is null) OR (tgv.DateCreated <= @DateCreatedEnd))
                                  AND ((@DateModifiedBegin is null) OR (tgv.DateModified >= @DateModifiedBegin))
                                  AND ((@DateModifiedEnd is null) OR (tgv.DateModified <= @DateModifiedEnd))
                                  AND ((@Search = '""""') OR (tgv.TicketNumber LIKE '%'+@SearchClean+'%') 
                                        OR (CONTAINS((t.[Name]), @Search))
                                        OR EXISTS (SELECT * FROM Actions a WHERE (a.TicketID = tgv.TicketID) AND CONTAINS((a.[Description], a.[Name]), @Search))
                                        --OR EXISTS (SELECT * FROM CustomValues cv 
                                          --      LEFT JOIN CustomFields cf ON cv.CustomFieldID = cf.CustomFieldID 
                                            --    WHERE (cf.RefType = 17)
                                              --  AND (cv.RefID = tgv.TicketID)
                                                --AND CONTAINS((cv.[CustomValue]), @Search))
                                        )
                                )
                              	  
                                  SELECT * FROM TicketRows 
                                  WHERE RowNumber BETWEEN @PageIndex*@PageSize+1 AND @PageIndex*@PageSize+@PageSize
                                  ORDER BY RowNumber ASC");
                      
        command.CommandText = builder.ToString();            
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@PageIndex", pageIndex);
        command.Parameters.AddWithValue("@PageSize", pageSize);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
        command.Parameters.AddWithValue("@TicketStatusID", ticketStatusID);
        command.Parameters.AddWithValue("@TicketSeverityID", ticketSeverityID);
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@GroupID", groupID);
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@ReportedVersionID", reportedVersionID);
        command.Parameters.AddWithValue("@ResolvedVersionID", resolvedVersionID);
        command.Parameters.AddWithValue("@CustomerID", customerID);
        command.Parameters.AddWithValue("@IsPortal", onlyPortal == null ? (object)DBNull.Value : onlyPortal);
        command.Parameters.AddWithValue("@IsKnowledgeBase", onlyKnowledgeBase == null ? (object)DBNull.Value : onlyKnowledgeBase);
        command.Parameters.AddWithValue("@DateCreatedBegin", dateCreatedBegin == null ? (object)DBNull.Value : dateCreatedBegin);
        command.Parameters.AddWithValue("@DateCreatedEnd", dateCreatedEnd == null ? (object)DBNull.Value : dateCreatedEnd);
        command.Parameters.AddWithValue("@DateModifiedBegin", dateModifiedBegin == null ? (object)DBNull.Value : dateModifiedBegin);
        command.Parameters.AddWithValue("@DateModifiedEnd", dateModifiedEnd == null ? (object)DBNull.Value : dateModifiedEnd);
        command.Parameters.AddWithValue("@Search", search);
        command.Parameters.AddWithValue("@SearchClean", search.Replace("*", "").Replace("%", "").Replace("\"", ""));


        Fill(command, "TicketGridView,Actions");
	    }
	  
     /* using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "uspSelectTicketPage";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@PageIndex", pageIndex);
        command.Parameters.AddWithValue("@PageSize", pageSize);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
        command.Parameters.AddWithValue("@TicketStatusID", ticketStatusID);
        command.Parameters.AddWithValue("@TicketSeverityID", ticketSeverityID);
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@GroupID", groupID);
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@ReportedVersionID", reportedVersionID);
        command.Parameters.AddWithValue("@ResolvedVersionID", resolvedVersionID);
        command.Parameters.AddWithValue("@CustomerID", customerID);
        command.Parameters.AddWithValue("@IsPortal", onlyPortal == null ? (object)DBNull.Value : onlyPortal);
        command.Parameters.AddWithValue("@IsKnowledgeBase", onlyKnowledgeBase == null ? (object)DBNull.Value : onlyKnowledgeBase);
        command.Parameters.AddWithValue("@DateCreatedBegin", dateCreatedBegin == null ? (object)DBNull.Value : dateCreatedBegin);
        command.Parameters.AddWithValue("@DateCreatedEnd", dateCreatedEnd == null ? (object)DBNull.Value : dateCreatedEnd);
        command.Parameters.AddWithValue("@DateModifiedBegin", dateModifiedBegin == null ? (object)DBNull.Value : dateModifiedBegin);
        command.Parameters.AddWithValue("@DateModifiedEnd", dateModifiedEnd == null ? (object)DBNull.Value : dateModifiedEnd);
        command.Parameters.AddWithValue("@Search", search);
        command.Parameters.AddWithValue("@SortColumn", "Name");
        command.Parameters.AddWithValue("@SortAsc", false);

        Fill(command, "TicketGridView,Actions");
      }*/
    }

    public int LoadForSearchCount(int organizationID, int ticketTypeID, int ticketStatusID, int ticketSeverityID,
      int userID, int groupID, int productID, int reportedVersionID, int resolvedVersionID,
      int customerID, bool? onlyPortal, bool? onlyKnowledgeBase,
      DateTime? dateCreatedBegin, DateTime? dateCreatedEnd, DateTime? dateModifiedBegin, DateTime? dateModifiedEnd,
      string search)
    {
      if (search.Trim() == "") search = "\"\"";
      using (SqlCommand command = new SqlCommand())
      {
        StringBuilder builder = new StringBuilder();
        builder.Append(@" SELECT COUNT(*)
                          FROM dbo.TicketGridView tgv LEFT JOIN Tickets t ON tgv.TicketID = t.TicketID
                          WHERE (tgv.OrganizationID = @OrganizationID)
                          AND ((tgv.TicketTypeID = @TicketTypeID) OR (@TicketTypeID = -1))
                          AND ((tgv.TicketSeverityID = @TicketSeverityID) OR (@TicketSeverityID = -1))
                          AND ((tgv.ProductID = @ProductID) OR (@ProductID = -1))
                          AND ((tgv.ReportedVersionID = @ReportedVersionID) OR (@ReportedVersionID = -1))
                          AND ((tgv.SolvedVersionID = @ResolvedVersionID) OR (@ResolvedVersionID = -1))
                          AND (((@UserID = -2) AND (tgv.UserID IS NULL)) OR (@UserID = tgv.UserID) OR (@UserID = -1))
                          AND (((@GroupID = -2) AND (tgv.GroupID IS NULL)) OR (@GroupID = tgv.GroupID) OR (@GroupID = -1))
                          AND ((tgv.TicketStatusID = @TicketStatusID) OR (@TicketStatusID = -1) OR ((@TicketStatusID = -3) AND (tgv.IsClosed = 0)) OR ((@TicketStatusID = -4) AND (tgv.IsClosed = 1)))
                          AND ((@CustomerID = -1) OR (EXISTS(SELECT * FROM OrganizationTickets ot WHERE (ot.OrganizationID = @CustomerID) AND (ot.TicketID = tgv.TicketID))))
                          AND ((@IsPortal is null) OR (tgv.IsVisibleOnPortal = @IsPortal))
                          AND ((@IsKnowledgeBase is null) OR (tgv.IsKnowledgeBase = @IsKnowledgeBase))
                          AND ((@DateCreatedBegin is null) OR (tgv.DateCreated >= @DateCreatedBegin))
                          AND ((@DateCreatedEnd is null) OR (tgv.DateCreated <= @DateCreatedEnd))
                          AND ((@DateModifiedBegin is null) OR (tgv.DateModified >= @DateModifiedBegin))
                          AND ((@DateModifiedEnd is null) OR (tgv.DateModified <= @DateModifiedEnd))
                          AND ((@Search = '""""') 
                                OR (CONTAINS((t.[Name]), @Search))
                                OR EXISTS (SELECT * FROM Actions a WHERE (a.TicketID = tgv.TicketID) AND CONTAINS((a.[Description], a.[Name]), @Search))
                                OR EXISTS (SELECT * FROM CustomValues cv 
                                        LEFT JOIN CustomFields cf ON cv.CustomFieldID = cf.CustomFieldID 
                                        WHERE (cf.RefType = 17)
                                        AND (cv.RefID = tgv.TicketID)
                                        AND CONTAINS((cv.[CustomValue]), @Search))
                                OR (tgv.TicketNumber LIKE '%'+@SearchClean+'%'))
                        ");

        command.CommandText = builder.ToString();
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
        command.Parameters.AddWithValue("@TicketStatusID", ticketStatusID);
        command.Parameters.AddWithValue("@TicketSeverityID", ticketSeverityID);
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@GroupID", groupID);
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@ReportedVersionID", reportedVersionID);
        command.Parameters.AddWithValue("@ResolvedVersionID", resolvedVersionID);
        command.Parameters.AddWithValue("@CustomerID", customerID);
        command.Parameters.AddWithValue("@IsPortal", onlyPortal == null ? (object)DBNull.Value : onlyPortal);
        command.Parameters.AddWithValue("@IsKnowledgeBase", onlyKnowledgeBase == null ? (object)DBNull.Value : onlyKnowledgeBase);
        command.Parameters.AddWithValue("@DateCreatedBegin", dateCreatedBegin == null ? (object)DBNull.Value : dateCreatedBegin);
        command.Parameters.AddWithValue("@DateCreatedEnd", dateCreatedEnd == null ? (object)DBNull.Value : dateCreatedEnd);
        command.Parameters.AddWithValue("@DateModifiedBegin", dateModifiedBegin == null ? (object)DBNull.Value : dateModifiedBegin);
        command.Parameters.AddWithValue("@DateModifiedEnd", dateModifiedEnd == null ? (object)DBNull.Value : dateModifiedEnd);
        command.Parameters.AddWithValue("@Search", search);
        command.Parameters.AddWithValue("@SearchClean", search.Replace("*", "").Replace("%", "").Replace("\"", ""));

        return (int)ExecuteScalar(command, "TicketGridView,Actions");
      }
    }

    public void LoadForSearch(int organizationID, string search)
    {
      if (search.Trim() == "") search = @"""""";

      using (SqlCommand command = new SqlCommand())
      {
          command.CommandText = @"
  SELECT tgv.* 
  FROM TicketGridView tgv 
    WHERE (tgv.OrganizationID = @OrganizationID)
    AND (
      (@Search = '""""') 
      OR (tgv.TicketNumber LIKE '%'+@SearchClean+'%') 
      --OR (tgv.Name LIKE @SearchClean+'%')
      OR EXISTS (SELECT * FROM Tickets t WHERE (t.TicketID = tgv.TicketID) AND CONTAINS((t.[Name]), @Search))
      OR EXISTS (SELECT * FROM Actions a WHERE (a.TicketID = tgv.TicketID) AND CONTAINS((a.[Description], a.[Name]), @Search))
      OR EXISTS (SELECT * FROM CustomValues cv LEFT JOIN CustomFields cf ON cv.CustomFieldID = cf.CustomFieldID WHERE (cf.RefType = 17) AND (cv.RefID = tgv.TicketID) AND CONTAINS((cv.[CustomValue]), @Search))
  )";
        /*
        command.CommandText = @"
select tgv.*
from ticketgridview tgv
where (tgv.OrganizationID = @OrganizationID)
and (
    (tgv.TicketNumber LIKE '%'+@SearchClean+'%') or (tgv.name like '%'+@search+'%')
    or tgv.ticketid in (select distinct(ticketid) from actions where contains(description,@search) or contains(name,@search))
    )
order by tgv.ticketnumber desc

";*/
        UseCache = true;
        CacheExpirationSeconds = 300;

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@Search", search);
        command.Parameters.AddWithValue("@SearchClean", search.Replace("*", "").Replace("%", "").Replace("\"", ""));
        Fill(command, "TicketSearch");
      }


    }

    public void LoadForTags(string tags)
    {
      string[] tagArray = tags.Split(',');

      StringBuilder builder = new StringBuilder(
@"SELECT tgv.[TicketID]
      ,tgv.[ProductName]
      ,tgv.[ReportedVersion]
      ,tgv.[SolvedVersion]
      ,tgv.[GroupName]
      ,tgv.[TicketTypeName]
      ,tgv.[UserName]
      ,tgv.[Status]
      ,tgv.[StatusPosition]
      ,tgv.[SeverityPosition]
      ,tgv.[IsClosed]
      ,tgv.[Severity]
      ,tgv.[TicketNumber]
      ,tgv.[IsVisibleOnPortal]
      ,tgv.[IsKnowledgeBase]
      ,tgv.[ReportedVersionID]
      ,tgv.[SolvedVersionID]
      ,tgv.[ProductID]
      ,tgv.[GroupID]
      ,tgv.[UserID]
      ,tgv.[TicketStatusID]
      ,tgv.[TicketTypeID]
      ,tgv.[TicketSeverityID]
      ,tgv.[OrganizationID]
      ,tgv.[Name]
      ,tgv.[ParentID]
      ,tgv.[ModifierID]
      ,tgv.[CreatorID]
      ,tgv.[DateModified]
      ,tgv.[DateCreated]
      ,tgv.[DateClosed]
      ,tgv.[CloserID]
      ,tgv.[DaysClosed]
      ,tgv.[DaysOpened]
      ,tgv.[CloserName]
      ,tgv.[CreatorName]
      ,tgv.[ModifierName]
      ,tgv.[SlaViolationTime]
      ,tgv.[SlaWarningTime]
      ,tgv.[SlaViolationHours]
      ,tgv.[SlaWarningHours]
FROM TicketGridView tgv 
WHERE tgv.OrganizationID = @OrganizationID"
);
      for (int i = 0; i < tagArray.Length; i++)
			{
        builder.Append(" AND EXISTS (SELECT * FROM TagLinksView WHERE TagLinksView.RefID=tgv.TicketID AND TagLinksView.RefType=17 AND TagLinksView.Value = @Value" + i.ToString() +")");
			}

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = builder.ToString();
        UseCache = false;
        CacheExpirationSeconds = 300;

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        for (int i = 0; i < tagArray.Length; i++)
        {
          command.Parameters.AddWithValue("@Value" + i.ToString(), tagArray[i]);
        }

        Fill(command, "TicketTags");
      }


    }

    public void LoadByDescription(int organizationID, string description)
    {
      LoadByDescription(organizationID, description, 0);
    }

    public void LoadKBByDescription(int organizationID, string description, int maxRecords)
    {
      if (description.Trim().Length < 1) return;
      string commandText;
      using (SqlCommand command = new SqlCommand())
      {
        StringBuilder builder = new StringBuilder();

        int number;
        string cleanSearch = description.Replace("*", "").Replace("%", "").Replace("\"", "");
        bool isNumber = int.TryParse(cleanSearch, out number);
        /*
        if (isNumber)
        {
          commandText = " SELECT TOP {0} CAST(TicketNumber AS VARCHAR(50)) + ':  ' + Name AS TicketDescription, TicketID, TicketNumber FROM Tickets" +
                                " WHERE OrganizationID = @OrganizationID" +
                                " AND (TicketNumber LIKE @SearchClean+'%')" +
                                " ORDER BY TicketNumber DESC";
        }
        else*/
        {
          commandText = @" 
SELECT TOP {0} CAST(TicketNumber AS VARCHAR(50)) + ':  ' + Name AS TicketDescription, TicketID, TicketNumber FROM Tickets t 
WHERE OrganizationID = @OrganizationID 
AND IsKnowledgebase = 1
AND (
  CONTAINS((t.[Name]), @Search)
  OR TicketNumber LIKE @SearchClean+'%'
  OR EXISTS (SELECT * FROM Actions a WHERE (a.TicketID = t.TicketID) AND CONTAINS((a.[Description], a.[Name]), @Search))
  OR EXISTS (SELECT * FROM CustomValues cv LEFT JOIN CustomFields cf ON cv.CustomFieldID = cf.CustomFieldID WHERE (cf.RefType = 17) AND (cv.RefID = t.TicketID) AND CONTAINS(cv.[CustomValue], @Search))                              
) 
ORDER BY TicketNumber DESC";
        }


        command.CommandText = String.Format(commandText, (maxRecords < 1 ? 15 : maxRecords).ToString());
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@Search", description);
        command.Parameters.AddWithValue("@SearchClean", cleanSearch);
        UseCache = true;
        CacheExpirationSeconds = 300;
        Fill(command, "QuickSearch");
      }

    }

    public void LoadByDescription(int organizationID, string description, int maxRecords)
    {
      if (description.Trim().Length < 1) return;
      string commandText;
      using (SqlCommand command = new SqlCommand())
      {
        StringBuilder builder = new StringBuilder();

        int number;
        string cleanSearch = description.Replace("*", "").Replace("%", "").Replace("\"", "");
        bool isNumber = int.TryParse(cleanSearch, out number);
        /*
        if (isNumber)
        {
          commandText = " SELECT TOP {0} CAST(TicketNumber AS VARCHAR(50)) + ':  ' + Name AS TicketDescription, TicketID, TicketNumber FROM Tickets " +
                                " WHERE OrganizationID = @OrganizationID" +
                                " AND (TicketNumber LIKE @SearchClean+'%')" +
                                " ORDER BY TicketNumber DESC";
        }
        else*/
        {
          commandText = @" 
SELECT TOP {0} CAST(TicketNumber AS VARCHAR(50)) + ':  ' + Name AS TicketDescription, TicketID, TicketNumber FROM Tickets t (NOLOCK)
WHERE OrganizationID = @OrganizationID 
AND (
  CONTAINS((t.[Name]), @Search)
  OR TicketNumber LIKE @SearchClean+'%'
  OR EXISTS (SELECT * FROM Actions a WHERE (a.TicketID = t.TicketID) AND CONTAINS((a.[Description], a.[Name]), @Search))
  OR EXISTS (SELECT * FROM CustomValues cv LEFT JOIN CustomFields cf ON cv.CustomFieldID = cf.CustomFieldID WHERE (cf.RefType = 17) AND (cv.RefID = t.TicketID) AND CONTAINS(cv.[CustomValue], @Search))                              
) 
ORDER BY TicketNumber DESC";
        }


        command.CommandText = String.Format(commandText, (maxRecords < 1 ? 15 : maxRecords).ToString());
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@Search", description);
        command.Parameters.AddWithValue("@SearchClean", cleanSearch);
        UseCache = true;
        CacheExpirationSeconds = 300;
        Fill(command, "QuickSearch");
      }

    }

    public static bool IsUserSubscribed(LoginUser loginUser, int userID, int ticketID)
    {
      return Subscriptions.IsUserSubscribed(loginUser, userID, ReferenceType.Tickets, ticketID);
    }

    public static int GetAssociatedOrganizationCount(LoginUser loginUser, int organizationID, int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT COUNT(*) FROM OrganizationTickets WHERE (TicketID = @TicketID) AND (OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@TicketID", ticketID);

        Tickets tickets = new Tickets(loginUser);
        return (int)tickets.ExecuteScalar(command);

      }
    }

    public static int GetAssociatedContactCount(LoginUser loginUser, int organizationID, int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = 
@"SELECT COUNT(*) FROM Users u
LEFT JOIN UserTickets ut ON ut.UserID = u.UserID
WHERE ut.TicketID = @TicketID 
AND u.OrganizationID = @OrganizationID
";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@TicketID", ticketID);

        Tickets tickets = new Tickets(loginUser);
        return (int)tickets.ExecuteScalar(command);

      }
    }

    public void RemoveSubscription(int userID, int ticketID)
    {
      Subscriptions.RemoveSubscription(LoginUser, userID, ReferenceType.Tickets, ticketID);
    }

    public void AddSubscription(int userID, int ticketID)
    {
      Subscriptions.AddSubscription(LoginUser, userID, ReferenceType.Tickets, ticketID);
    }

    public static Ticket GetTicketByNumber(LoginUser loginUser, int organizationID, int ticketNumber)
    {
      Tickets tickets = new Tickets(loginUser);
      tickets.LoadByTicketNumber(organizationID, ticketNumber);
      if (tickets.IsEmpty) return null;
      else return tickets[0];
    }

    public static Ticket GetTicketByNumber(LoginUser loginUser, int ticketNumber)
    {
      return GetTicketByNumber(loginUser, loginUser.OrganizationID, ticketNumber);
    }

    public static int GetTicketActionTime(LoginUser loginUser, int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT SUM(TimeSpent) FROM Actions WHERE TicketID = @TicketID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);

        Tickets tickets = new Tickets(loginUser);
        object o = tickets.ExecuteScalar(command, "Tickets,Actions");
        if (o == DBNull.Value)
          return 0;
        else
          return (int)o;
      }
    }

    public static int GetUserOpenTicketCount(LoginUser loginUser, int userID, int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT COUNT(*) FROM Tickets t LEFT JOIN TicketStatuses ts ON ts.TicketStatusID = t.TicketStatusID WHERE (t.TicketTypeID = @TicketTypeID) AND (t.UserID = @UserID) AND (ts.IsClosed = 0)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);

        Tickets tickets = new Tickets(loginUser);
        return (int)tickets.ExecuteScalar(command, "Tickets");
      }
    }

    public static int GetContactOpenTicketCount(LoginUser loginUser, int userID, int ticketTypeID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT COUNT(*) FROM Tickets t LEFT JOIN TicketStatuses ts ON ts.TicketStatusID = t.TicketStatusID WHERE (t.TicketTypeID = @TicketTypeID) AND (ts.IsClosed = 0) AND EXISTS(SELECT * FROM UserTickets ut WHERE (t.TicketID = ut.TicketID) AND (ut.UserID = @userID))";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@userID", userID);
            command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);

            Tickets tickets = new Tickets(loginUser);
            return (int)tickets.ExecuteScalar(command, "Tickets");
        }
    }

    public static int GetContactClosedTicketCount(LoginUser loginUser, int userID, int ticketTypeID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT COUNT(*) FROM Tickets t LEFT JOIN TicketStatuses ts ON ts.TicketStatusID = t.TicketStatusID WHERE (t.TicketTypeID = @TicketTypeID) AND (ts.IsClosed = 1) AND EXISTS(SELECT * FROM UserTickets ut WHERE (t.TicketID = ut.TicketID) AND (ut.UserID = @userID))";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@userID", userID);
            command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);

            Tickets tickets = new Tickets(loginUser);
            return (int)tickets.ExecuteScalar(command, "Tickets");
        }
    }

    public static int GetOrganizationOpenTicketCount(LoginUser loginUser, int organizationID, int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT COUNT(*) FROM Tickets t LEFT JOIN TicketStatuses ts ON ts.TicketStatusID = t.TicketStatusID WHERE (t.TicketTypeID = @TicketTypeID) AND (ts.IsClosed = 0) AND EXISTS(SELECT * FROM OrganizationTickets ot WHERE (t.TicketID = ot.TicketID) AND (ot.OrganizationID = @OrganizationID))";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);

        Tickets tickets = new Tickets(loginUser);
        return (int)tickets.ExecuteScalar(command, "Tickets");
      }
    }

    public static int GetOrganizationClosedTicketCount(LoginUser loginUser, int organizationID, int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT COUNT(*) FROM Tickets t LEFT JOIN TicketStatuses ts ON ts.TicketStatusID = t.TicketStatusID WHERE (t.TicketTypeID = @TicketTypeID) AND (ts.IsClosed = 1) AND EXISTS(SELECT * FROM OrganizationTickets ot WHERE (t.TicketID = ot.TicketID) AND (ot.OrganizationID = @OrganizationID))";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);

        Tickets tickets = new Tickets(loginUser);
        return (int)tickets.ExecuteScalar(command, "Tickets");
      }
    }

    public static int GetProductOpenTicketCount(LoginUser loginUser, int productID, int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT
          COUNT(*) 
        FROM 
          Tickets t 
          LEFT JOIN TicketStatuses ts 
            ON ts.TicketStatusID = t.TicketStatusID 
        WHERE 
          t.TicketTypeID = @TicketTypeID
          AND ts.IsClosed = 0
          AND t.ProductID = @ProductID
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);

        Tickets tickets = new Tickets(loginUser);
        return (int)tickets.ExecuteScalar(command, "Tickets");
      }
    }

    public static int GetProductClosedTicketCount(LoginUser loginUser, int productID, int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT
          COUNT(*)
        FROM
          Tickets t
          LEFT JOIN TicketStatuses ts
            ON ts.TicketStatusID = t.TicketStatusID
        WHERE 
          t.TicketTypeID = @TicketTypeID
          AND ts.IsClosed = 1
          AND t.ProductID = @ProductID
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);

        Tickets tickets = new Tickets(loginUser);
        return (int)tickets.ExecuteScalar(command, "Tickets");
      }
    }

    public static int GetProductVersionOpenTicketCount(LoginUser loginUser, int productVersionID, int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT
          COUNT(*) 
        FROM 
          Tickets t 
          LEFT JOIN TicketStatuses ts 
            ON ts.TicketStatusID = t.TicketStatusID 
        WHERE 
          t.TicketTypeID = @TicketTypeID
          AND ts.IsClosed = 0
          AND (t.ReportedVersionID = @ProductVersionID OR t.SolvedVersionID = @ProductVersionID)
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);

        Tickets tickets = new Tickets(loginUser);
        return (int)tickets.ExecuteScalar(command, "Tickets");
      }
    }

    public static int GetProductVersionClosedTicketCount(LoginUser loginUser, int productVersionID, int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT
          COUNT(*)
        FROM
          Tickets t
          LEFT JOIN TicketStatuses ts
            ON ts.TicketStatusID = t.TicketStatusID
        WHERE 
          t.TicketTypeID = @TicketTypeID
          AND ts.IsClosed = 1
          AND (t.ReportedVersionID = @ProductVersionID OR t.SolvedVersionID = @ProductVersionID)
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);

        Tickets tickets = new Tickets(loginUser);
        return (int)tickets.ExecuteScalar(command, "Tickets");
      }
    }

    public void LoadByGroupUnassigned(int userID, int top)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP " + top.ToString() + @" tgv.* FROM TicketGridView tgv 
                                WHERE (tgv.UserID is null)
                                AND (tgv.IsClosed = 0)
                                AND EXISTS(SELECT * FROM GroupUsers gu WHERE (GroupID = tgv.GroupID) AND (UserID = @UserID))
                                ORDER BY tgv.DateModified DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command, "TicketGridView");
      }
    }

    public void LoadByRecentKnowledgeBase(int organizationID, int top)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP " + top.ToString() + @" tgv.* FROM TicketGridView tgv 
                               WHERE (tgv.OrganizationID = @OrganizationID)
                               AND (tgv.IsKnowledgeBase = 1)
                               ORDER BY tgv.DateModified DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command, "TicketGridView");
      }
    }

    public void LoadByTicketNumber(int organizationID, int ticketNumber)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Tickets WHERE OrganizationID = @OrganizationID AND TicketNumber = @TicketNumber";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@TicketNumber", ticketNumber);
        Fill(command, "TicketGridView");
      }    
    }

    public void LoadByTicketIDs(int organizationID, int[] ticketIDs)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string ids = DataUtils.IntArrayToCommaString(ticketIDs);

        command.CommandText = "SELECT * FROM Tickets WHERE OrganizationID = @OrganizationID AND TicketID IN (" + ids + ")";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void ReplaceTicketType(int oldID, int newID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE Tickets SET TicketTypeID = @newID WHERE (TicketTypeID = @oldID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@oldID", oldID);
        command.Parameters.AddWithValue("@newID", newID);
        ExecuteNonQuery(command, "Tickets");
      }
    }

    public void ReplaceTicketStatus(int oldID, int newID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE Tickets SET TicketStatusID = @newID WHERE (TicketStatusID = @oldID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@oldID", oldID);
        command.Parameters.AddWithValue("@newID", newID);
        ExecuteNonQuery(command, "Tickets");
      }
    }

    public void ReplaceTicketSeverity(int oldID, int newID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE Tickets SET TicketSeverityID = @newID WHERE (TicketSeverityID = @oldID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@oldID", oldID);
        command.Parameters.AddWithValue("@newID", newID);
        ExecuteNonQuery(command, "Tickets");
      }
    }

    public void LoadAllUnnotifiedAndExpiredSla()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"
SELECT ticketnumber, t.*, sn.*
FROM Tickets t
LEFT JOIN SlaNotifications sn ON t.TicketID = sn.TicketID
WHERE  
(
( 
    t.SlaViolationTimeClosed IS NOT NULL AND
    ABS(DATEDIFF(day, t.SlaViolationTimeClosed, GETUTCDATE())) < 1 AND
    DATEDIFF(minute,  t.SlaViolationTimeClosed, ISNULL(sn.TimeClosedViolationDate, '1/1/1980')) < 10 
  )
  OR
  (
    t.SlaWarningTimeClosed IS NOT NULL AND
    ABS(DATEDIFF(day, t.SlaWarningTimeClosed, GETUTCDATE())) < 1 AND
    DATEDIFF(minute,  t.SlaWarningTimeClosed, ISNULL(sn.TimeClosedWarningDate, '1/1/1980')) < 10 
  )
  OR
  (
    t.SlaViolationLastAction IS NOT NULL AND
    ABS(DATEDIFF(day, t.SlaViolationLastAction, GETUTCDATE())) < 1 AND
    DATEDIFF(minute,  t.SlaViolationLastAction, ISNULL(sn.LastActionViolationDate, '1/1/1980')) < 10 
  )
  OR
  (
    t.SlaWarningLastAction IS NOT NULL AND
    ABS(DATEDIFF(day, t.SlaWarningLastAction, GETUTCDATE())) < 1 AND
    DATEDIFF(minute,  t.SlaWarningLastAction, ISNULL(sn.LastActionWarningDate, '1/1/1980')) < 10 
  )
  OR
  (
    t.SlaViolationInitialResponse IS NOT NULL AND
    ABS(DATEDIFF(day, t.SlaViolationInitialResponse, GETUTCDATE())) < 1 AND
    DATEDIFF(minute,  t.SlaViolationInitialResponse, ISNULL(sn.InitialResponseViolationDate, '1/1/1980')) < 10 
  )
  OR
  (
    t.SlaWarningInitialResponse IS NOT NULL AND
    ABS(DATEDIFF(day, t.SlaWarningInitialResponse, GETUTCDATE())) < 1 AND
    DATEDIFF(minute,  t.SlaWarningInitialResponse, ISNULL(sn.InitialResponseWarningDate, '1/1/1980')) < 10 
  )

)";




        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }


    public void AddTags(Tag tag, int ticketID)
    {
        Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, ticketID);
        string description = "Added '" + tag.Value + "' to the tag list for " + GetTicketLink(ticket);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, ticketID, description);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tags, tag.TagID, description);

    }

    public void RemoveTags(Tag tag, int ticketID)
    {
        Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, ticketID);
        string description = "Removed '" + tag.Value + "' from the tag list for " + GetTicketLink(ticket);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Tickets, ticketID, description);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Tags, tag.TagID, description);

    }

    public void LoadBySalesForceID(string salesForceID, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Tickets WHERE SalesForceID = @SalesForceID AND OrganizationID = @OrganizationID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@SalesForceID", salesForceID);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void GetUsersTickets(int ticketID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT * FROM userstickets where ticketid = @ticketID";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ticketID", ticketID);
            Fill(command);
        }
    }

    public void MergeUpdateContact(int oldticketID, int newticketID)
    {

        try
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
            BEGIN transaction

            DELETE FROM UserTickets
            WHERE TicketID = @oldticketID AND UserID IN(SELECT UserID FROM UserTickets WHERE TicketID=@newticketID);

            UPDATE UserTickets
            SET TicketID = @newticketID
            WHERE TicketID = @oldticketID;

            /*If no error run*/
            COMMIT transaction
            /*If error run*/
            ROLLBACK transaction
            ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@newticketID", newticketID);
                command.Parameters.AddWithValue("@oldticketID", oldticketID);
                ExecuteNonQuery(command, "UserTickets");
            }
        }
        catch (Exception e){

        }

        Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
        string description = "Merged '" + ticket.TicketNumber + "' Customers";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, newticketID, description);
    }

    public void MergeUpdateOrganizations(int oldticketID, int newticketID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
            BEGIN transaction

            DELETE FROM OrganizationTickets
            WHERE TicketID = @oldticketID AND OrganizationID IN(SELECT OrganizationID FROM OrganizationTickets WHERE TicketID=@newticketID);

            UPDATE OrganizationTickets
            SET TicketID = @newticketID
            WHERE TicketID = @oldticketID;

            /*If no error run*/
            COMMIT transaction
            /*If error run*/
            ROLLBACK transaction
            ";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@newticketID", newticketID);
            command.Parameters.AddWithValue("@oldticketID", oldticketID);
            ExecuteNonQuery(command, "OrganizationTickets");
        }

        Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
        string description = "Merged '" + ticket.TicketNumber + "' Organizations";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Organizations, newticketID, description);
    }

    public void MergeUpdateTags(int oldticketID, int newticketID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "UPDATE Taglinks SET RefID=@newticketID WHERE (RefID = @oldticketID) AND RefType = @refType";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@newticketID", newticketID);
            command.Parameters.AddWithValue("@oldticketID", oldticketID);
            command.Parameters.AddWithValue("@refType", ReferenceType.Tickets);
            ExecuteNonQuery(command, "Taglinks");
        }

        Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
        string description = "Merged '" + ticket.TicketNumber + "' Tags";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, newticketID, description);
    }

    public void MergeUpdateCustomFields(int oldticketID, int newticketID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "UPDATE CustomValues SET RefID=@newticketID WHERE (RefID = @oldticketID)";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@newticketID", newticketID);
            command.Parameters.AddWithValue("@oldticketID", oldticketID);
            ExecuteNonQuery(command, "CustomValues");
        }

        Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
        string description = "Merged '" + ticket.TicketNumber + "' Tags";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, newticketID, description);
    }

    public void MergeUpdateSubscribers(int oldticketID, int newticketID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "UPDATE Subscriptions SET RefID=@newticketID WHERE (RefID = @oldticketID)";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@newticketID", newticketID);
            command.Parameters.AddWithValue("@oldticketID", oldticketID);
            ExecuteNonQuery(command, "Subscriptions");
        }

        Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
        string description = "Merged '" + ticket.TicketNumber + "' Subscribers";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, newticketID, description);
    }

    public void MergeUpdateQueuers(int oldticketID, int newticketID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "UPDATE TicketQueue SET TicketID=@newticketID WHERE (TicketID = @oldticketID)";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@newticketID", newticketID);
            command.Parameters.AddWithValue("@oldticketID", oldticketID);
            ExecuteNonQuery(command, "TicketQueue");
        }

        Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
        string description = "Merged '" + ticket.TicketNumber + "' Queuers";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, newticketID, description);
    }

    public void MergeUpdateReminders(int oldticketID, int newticketID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "UPDATE Reminders SET RefID=@newticketID WHERE (RefID = @oldticketID and RefType = @reftype)";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@newticketID", newticketID);
            command.Parameters.AddWithValue("@oldticketID", oldticketID);
            command.Parameters.AddWithValue("@reftype", ReferenceType.Tickets);
            ExecuteNonQuery(command, "Reminders");
        }

        Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
        string description = "Merged '" + ticket.TicketNumber + "' Reminders";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, newticketID, description);
    }

    public void MergeUpdateAssets(int oldticketID, int newticketID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "UPDATE AssetTickets SET TicketID=@newticketID WHERE (TicketID = @oldticketID)";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@newticketID", newticketID);
            command.Parameters.AddWithValue("@oldticketID", oldticketID);
            ExecuteNonQuery(command, "AssetTickets");
        }

        Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
        string description = "Merged '" + ticket.TicketNumber + "' Assets";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, newticketID, description);
    }

    public void MergeUpdateActions(int oldticketID, int newticketID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "UPDATE Actions SET TicketID=@newticketID WHERE (TicketID = @oldticketID)";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@newticketID", newticketID);
            command.Parameters.AddWithValue("@oldticketID", oldticketID);
            ExecuteNonQuery(command, "Actions");
        }

        Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
        string description = "Merged '" + ticket.TicketNumber + "' Actions";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, newticketID, description);
    }

    public void MergeUpdateRelationships(int oldticketID, int newticketID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "UPDATE TicketRelationships SET Ticket1ID=@newticketID WHERE (Ticket1ID = @oldticketID)";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@newticketID", newticketID);
            command.Parameters.AddWithValue("@oldticketID", oldticketID);
            ExecuteNonQuery(command, "TicketRelationships");
        }
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "UPDATE TicketRelationships SET Ticket2ID=@newticketID WHERE (Ticket2ID = @oldticketID)";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@newticketID", newticketID);
            command.Parameters.AddWithValue("@oldticketID", oldticketID);
            ExecuteNonQuery(command, "TicketRelationships");
        }
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "UPDATE Tickets SET ParentID=@newticketID WHERE (ParentID = @oldticketID)";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@newticketID", newticketID);
            command.Parameters.AddWithValue("@oldticketID", oldticketID);
            ExecuteNonQuery(command, "Tickets");
        }
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "UPDATE Tickets SET ParentID=null WHERE (ParentID = ticketID)";
            command.CommandType = CommandType.Text;
            ExecuteNonQuery(command, "Tickets");
        }

        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "DELETE FROM TicketRelationships WHERE (Ticket1ID = @newticketID AND Ticket2ID = @newticketID)";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@newticketID", newticketID);
            ExecuteNonQuery(command, "TicketRelationships");
        }
        Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
        string description = "Merged '" + ticket.TicketNumber + "' Relationships";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, newticketID, description);
    }

    public void MergeAttachments(int oldticketID, int newticketID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "UPDATE attachments SET RefID=@newticketID WHERE (RefID = @oldticketID) AND RefType = @refType";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@newticketID", newticketID);
            command.Parameters.AddWithValue("@oldticketID", oldticketID);
            command.Parameters.AddWithValue("@refType", ReferenceType.Actions);
            ExecuteNonQuery(command, "attachments");
        }

        Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, oldticketID);
        string description = "Merged '" + ticket.TicketNumber + "' Action Attachments";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, newticketID, description);
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, newticketID, description);           
    }

    public void LoadFirstJiraSynced(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP 1 t.* FROM Tickets t JOIN TicketLinkToJira j ON t.TicketID = j.TicketID WHERE t.OrganizationID = @OrganizationID ORDER BY t.DateCreated";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public int GetProductVersionTicketCount(int productVersionID, int closed)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
        SELECT 
          COUNT(*) 
        FROM 
          Tickets t
          JOIN TicketStatuses ts
            ON t.TicketStatusID = ts.TicketStatusID
        WHERE 
          ts.IsClosed = @closed
          AND (t.ReportedVersionID = @ProductVersionID OR t.SolvedVersionID = @ProductVersionID)
        ";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
            command.Parameters.AddWithValue("@closed", closed);
            object o = ExecuteScalar(command);
            if (o == null || o == DBNull.Value) return 0;
            return (int)o;
        }
    }

    public int GetProductTicketCount(int productID, int closed)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT 
          COUNT(*) 
        FROM 
          Tickets t
          JOIN TicketStatuses ts
            ON t.TicketStatusID = ts.TicketStatusID
        WHERE 
          ts.IsClosed = @closed
          AND t.ProductID = @ProductID
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@closed", closed);
        object o = ExecuteScalar(command);
        if (o == null || o == DBNull.Value) return 0;
        return (int)o;
      }
    }
  }
}

