using System;
using System.Collections.Generic;
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
  }

  public partial class Tickets 
  {
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
    }

    partial void AfterRowInsert(Ticket ticket)
    {
      string description = "Created Ticket Number " + ticket.TicketNumber.ToString();
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, ticket.TicketID, description);
    }

    public void UpdateChildTickets(Ticket ticket)
    { 
      Tickets tickets = new Tickets(LoginUser);
      tickets.LoadChildren(ticket.TicketID);

      foreach (Ticket item in tickets)
	    {
		    if (item.TicketTypeID == ticket.TicketTypeID)
          item.TicketStatusID = ticket.TicketStatusID;
	    }
      tickets.Save();

    }
    
    partial void AfterRowEdit(Ticket ticket)
    {
      UpdateChildTickets(ticket);
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
          description = "Added " + GetTicketLink(ticket) + " to the user portal.";
        else
          description = "Removed " + GetTicketLink(ticket) + " from the user portal.";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
      }

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
          description = "Closed " + GetTicketLink(ticket);
          ticket.CloserID = LoginUser.UserID;
          ticket.DateClosed = DateTime.UtcNow;
        }
        else
        {
          description = "Reopened " + GetTicketLink(ticket);
          ticket.CloserID = null;
          ticket.DateClosed = null;
        }

        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
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
      foreach (Ticket ticket in this)
      {
        if (ticket.ImportID == importID)
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

    public void LoadForIndexing()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"SELECT t.*,
(
t.Name + ' ' +

ISNULL(
(
  SELECT CAST(cv.CustomValue + ' ' AS VARCHAR(MAX)) FROM CustomValues cv LEFT JOIN CustomFields cf ON cf.CustomFieldID = cv.CustomFieldID 
  WHERE cf.RefType=17 AND cv.RefID=t.TicketID    
  FOR XML PATH('')
), '') + ' ' +
(
  SELECT CAST(a.Description + ' ' + a.Name + ' ' AS VARCHAR(MAX))
  FROM Actions a
  WHERE a.TicketID = t.TicketID
  FOR XML PATH('')
)

) AS Text
FROM Tickets t
WHERE t.NeedsIndexing = 1
ORDER BY DateModified 
";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }


  }
}
