using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ActionLog
  {
    public string CreatorName
    {
      get
      {
        if (Row.Table.Columns.Contains("CreatorName") && Row["CreatorName"] != DBNull.Value)
        {
          return (string)Row["CreatorName"];
        }
        else return "";
      }
    }
  }

  public partial class ActionLogs : BaseCollection, IEnumerable<ActionLog>
  {

    public static void AddActionLog(LoginUser loginUser, ActionLogType type, ReferenceType refType, int refID, string description)
    {
      ActionLogs actionLogs = new ActionLogs(loginUser);
      ActionLog actionLog = actionLogs.AddNewActionLog();
      actionLog.Description = description;
      actionLog.OrganizationID = loginUser.OrganizationID < 0 ? null : (int?)loginUser.OrganizationID;
      actionLog.ActionLogType = type;
      actionLog.RefID = refID;
      actionLog.RefType = refType;
      actionLogs.Save();
    }

    public static void AddInternalActionLog(LoginUser loginUser, string description)
    {
      ActionLogs actionLogs = new ActionLogs(loginUser);
      ActionLog actionLog = actionLogs.AddNewActionLog();
      actionLog.Description = description;
      actionLog.OrganizationID = null;
      actionLog.ActionLogType = ActionLogType.Insert;
      actionLog.RefID = -1;
      actionLog.RefType = ReferenceType.SystemSettings;
      actionLogs.Save();
    }

    public void LoadAll()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                ORDER BY DateCreated DESC
                                ";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public void LoadByGroupID(int groupID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.RefType = 6) AND (al.RefID = @GroupID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Tickets t ON t.TicketID = al.RefID
                                WHERE (al.RefType = 17) AND (t.GroupID = @GroupID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Actions a ON a.ActionID = al.RefID
                                LEFT JOIN Tickets t ON t.TicketID = a.TicketID
                                WHERE (al.RefType = 0) AND (t.GroupID = @GroupID)

                                ORDER BY DateCreated DESC
                                ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@GroupID", groupID);
        Fill(command);
      }
    }

    public void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.RefType = 17) AND (al.RefID = @TicketID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Actions a ON a.ActionID = al.RefID
                                WHERE (al.RefType = 0) AND (a.TicketID = @TicketID)

                                ORDER BY DateCreated DESC
                                ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadByTicketID(int ticketID, DateTime minUtcDate)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.RefType = 17) AND (al.RefID = @TicketID) AND (al.DateCreated >= @MinDate)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Actions a ON a.ActionID = al.RefID
                                WHERE (al.RefType = 0) AND (a.TicketID = @TicketID) AND (al.DateCreated >= @MinDate)

                                ORDER BY DateCreated DESC
                                ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        command.Parameters.AddWithValue("@MinDate", minUtcDate);
        Fill(command);
      }
    }

    public void LoadByProductID(int productID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.RefType = 13) AND (al.RefID = @ProductID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Tickets t ON t.TicketID = al.RefID
                                WHERE (al.RefType = 17) AND (t.ProductID = @ProductID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Actions a ON a.ActionID = al.RefID
                                LEFT JOIN Tickets t ON t.TicketID = a.TicketID
                                WHERE (al.RefType = 0) AND (t.ProductID = @ProductID)

                                ORDER BY DateCreated DESC
                                ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        Fill(command);
      }
    }

    public void LoadByProductVersionID(int productVersionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.RefType = 14) AND (al.RefID = @ProductVersionID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Tickets t ON t.TicketID = al.RefID
                                WHERE (al.RefType = 17) AND ((t.ReportedVersionID = @ProductVersionID) OR (t.SolvedVersionID = @ProductVersionID))
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Actions a ON a.ActionID = al.RefID
                                LEFT JOIN Tickets t ON t.TicketID = a.TicketID
                                WHERE (al.RefType = 0) AND ((t.ReportedVersionID = @ProductVersionID) OR (t.SolvedVersionID = @ProductVersionID))

                                ORDER BY DateCreated DESC
                                ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
        Fill(command);
      }
    }

    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.RefType = 9) AND (al.RefID = @OrganizationID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Tickets t ON t.TicketID = al.RefID
                                LEFT JOIN OrganizationTickets ot ON ot.TicketID = t.TicketID
                                WHERE (al.RefType = 17) AND (ot.OrganizationID = @OrganizationID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Actions a ON a.ActionID = al.RefID
                                LEFT JOIN Tickets t ON t.TicketID = a.TicketID
                                LEFT JOIN OrganizationTickets ot ON ot.TicketID = t.TicketID
                                WHERE (al.RefType = 0) AND (ot.OrganizationID = @OrganizationID)
                                
                                UNION 

                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.OrganizationID = @OrganizationID)

                                ORDER BY DateCreated DESC
                                ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }


    public void LoadByOrganizationIDLimit(int organizationID, int start)
    {
        int end = start + 9;
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"select *  from (SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName, ROW_NUMBER() OVER (ORDER BY al.DateCreated Desc) AS rownum
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.RefType = 9) AND (al.RefID = @OrganizationID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName, ROW_NUMBER() OVER (ORDER BY al.DateCreated Desc) AS rownum
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Tickets t ON t.TicketID = al.RefID
                                LEFT JOIN OrganizationTickets ot ON ot.TicketID = t.TicketID
                                WHERE (al.RefType = 17) AND (ot.OrganizationID = @OrganizationID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName, ROW_NUMBER() OVER (ORDER BY al.DateCreated Desc) AS rownum
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Actions a ON a.ActionID = al.RefID
                                LEFT JOIN Tickets t ON t.TicketID = a.TicketID
                                LEFT JOIN OrganizationTickets ot ON ot.TicketID = t.TicketID
                                WHERE (al.RefType = 0) AND (ot.OrganizationID = @OrganizationID)
                                
                                UNION 

                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName, ROW_NUMBER() OVER (ORDER BY al.DateCreated Desc) AS rownum
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.OrganizationID = @OrganizationID)) as temp
                                where rownum between @start and @end
								order by rownum asc

                                ";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@start", start);
            command.Parameters.AddWithValue("@end", end);
            Fill(command);
        }
    }

    public void LoadByUserID(int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.RefType = 22) AND (al.RefID = @UserID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.CreatorID = @UserID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Tickets t ON t.TicketID = al.RefID
                                WHERE (al.RefType = 17) AND (t.UserID = @UserID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Actions a ON a.ActionID = al.RefID
                                LEFT JOIN Tickets t ON t.TicketID = a.TicketID
                                WHERE (al.RefType = 0) AND (t.UserID = @UserID)
                                
                                ORDER BY DateCreated DESC
                                ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command);
      }
    }


    public void LoadByUserIDLimit(int userID, int start)
    {
        int end = start + 19;
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"select *  from  (SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName, ROW_NUMBER() OVER (ORDER BY al.DateCreated Desc) AS rownum
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.RefType = 22) AND (al.RefID = @UserID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName, ROW_NUMBER() OVER (ORDER BY al.DateCreated Desc) AS rownum
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.CreatorID = @UserID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName, ROW_NUMBER() OVER (ORDER BY al.DateCreated Desc) AS rownum
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Tickets t ON t.TicketID = al.RefID
                                WHERE (al.RefType = 17) AND (t.UserID = @UserID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName, ROW_NUMBER() OVER (ORDER BY al.DateCreated Desc) AS rownum
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Actions a ON a.ActionID = al.RefID
                                LEFT JOIN Tickets t ON t.TicketID = a.TicketID
                                WHERE (al.RefType = 0) AND (t.UserID = @UserID)
                                ) as temp
                                where rownum between @start and @end
								order by rownum asc
                                ";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@UserID", userID);
            command.Parameters.AddWithValue("@start", start);
            command.Parameters.AddWithValue("@end", end);
            Fill(command);
        }
    }

  }
}
