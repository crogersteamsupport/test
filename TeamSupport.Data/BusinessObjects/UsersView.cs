using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class UsersViewItem
  {
  }
  
  public partial class UsersView
  {

    public void LoadByOrganizationID(int organizationID, bool loadOnlyActive)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM UsersView WHERE OrganizationID = @OrganizationID AND (@ActiveOnly = 0 OR IsActive = 1) AND (MarkDeleted = 0) ORDER BY LastName, FirstName";
        command.CommandText = InjectCustomFields(command.CommandText, "UserID", ReferenceType.Users);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@ActiveOnly", loadOnlyActive);
        Fill(command);
      }
    }

    public void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT u.* FROM UsersView u LEFT JOIN UserTickets ut ON ut.UserID = u.UserID WHERE ut.TicketID = @TicketID ORDER BY u.LastName, u.FirstName";
        command.CommandText = InjectCustomFields(command.CommandText, "u.UserID", ReferenceType.Users);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command, "Organizations,OrganizationTickets");
      }
    }

    public void LoadByLikeName(int organizationID, string name)
    {
      LoadByLikeName(organizationID, name, int.MaxValue);
    }

    public void LoadByTerm(int parentID, string term, int max)
    {
      if (term.Trim().Length < 2) return;
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"SELECT TOP (@MaxRows) u.* FROM UsersView u
WHERE ((RTRIM(u.FirstName) +' '+ RTRIM(u.LastName) LIKE '%'+@term+'%') 
OR (RTRIM(RTRIM(u.LastName)) +' '+ RTRIM(u.FirstName) LIKE '%'+@term+'%') 
OR (RTRIM(u.LastName) +', '+ RTRIM(u.FirstName) LIKE '%'+@term+'%') 
OR (RTRIM(u.LastName) +','+ RTRIM(u.FirstName) LIKE '%'+@term+'%') 
OR (RTRIM(u.LastName) LIKE '%'+@term+'%') 
OR (RTRIM(u.FirstName) LIKE '%'+@term+'%') 
--OR (u.email LIKE '%'+@term+'%')
)
AND (u.OrganizationID = @OrganizationID)  
AND (u.MarkDeleted = 0)
ORDER BY u.LastName, u.FirstName";
        command.CommandType = CommandType.Text;

        command.Parameters.AddWithValue("@term", term);
        command.Parameters.AddWithValue("@MaxRows", max);
        command.Parameters.AddWithValue("@OrganizationID", parentID);
        Fill(command);
      }
    }

    public void LoadByLikeName(int organizationID, string name, int max)
    {
      if (name.Trim().Length < 2) return;
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"SELECT TOP (@MaxRows) u.* FROM UsersView u
LEFT JOIN Organizations o1 ON o1.OrganizationID = u.OrganizationID
LEFT JOIN Organizations o2 ON o2.OrganizationID = o1.ParentID
WHERE ((RTRIM(u.FirstName) +' '+ RTRIM(u.LastName) LIKE '%'+@Name+'%') 
OR (RTRIM(RTRIM(u.LastName)) +' '+ RTRIM(u.FirstName) LIKE '%'+@Name+'%') 
OR (RTRIM(u.LastName) +', '+ RTRIM(u.FirstName) LIKE '%'+@Name+'%') 
OR (RTRIM(u.LastName) +','+ RTRIM(u.FirstName) LIKE '%'+@Name+'%') 
OR (RTRIM(u.LastName) LIKE '%'+@Name+'%') 
OR (RTRIM(u.FirstName) LIKE '%'+@Name+'%') 
--OR (u.email LIKE '%'+@Name+'%')
)
AND (o2.OrganizationID = @OrganizationID)  
AND (u.MarkDeleted = 0)
ORDER BY u.LastName, u.FirstName";
        command.CommandType = CommandType.Text;

        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@MaxRows", max);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadBySubscription(int refID, ReferenceType refType)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = 
@"SELECT u.* FROM UsersView u 
LEFT JOIN Subscriptions s ON u.UserID = s.UserID 
WHERE (s.RefID = @TicketID) 
AND (s.RefType = @RefType) 
AND (u.IsActive = 1) 
AND (u.MarkDeleted = 0)
ORDER BY u.FirstName, u.LastName";

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", refID);
        command.Parameters.AddWithValue("@RefType", (int)refType);
        Fill(command, "Users,Tickets,Subscriptions");
      }
    }

    public void LoadByTicketQueue(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"
SELECT u.* FROM 
UsersView u 
LEFT JOIN TicketQueue q ON u.UserID = q.UserID 
WHERE (q.TicketID = @TicketID) 
AND (u.IsActive = 1) 
AND (u.MarkDeleted = 0)
ORDER BY u.FirstName, u.LastName";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }
  }

  
}
