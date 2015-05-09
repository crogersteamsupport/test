using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class Reminder
  {
  }
  
  public partial class Reminders
  {

    public void LoadByUser(int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Reminders WHERE (UserID = @UserID) AND (IsDismissed = 0) ORDER BY DueDate";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command);
      }
    }

    public void LoadByUserMonth(DateTime date, int userID, string Type, string ID)
    {
        string additional = "";
        string userStr = "(UserID = @UserID)";
        if (Type != "-1")
        {
            additional = "AND (Reftype = @type) AND (RefID = @id)";
            userStr = "1=1";
        }


        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = string.Format("SELECT * FROM Reminders WHERE {0} AND (IsDismissed = 0) AND (Month(Duedate) = @month) AND (Year(Duedate) = @year) {1} ORDER BY DueDate", userStr, additional);
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@UserID", userID);
            command.Parameters.AddWithValue("@month", date.Month);
            command.Parameters.AddWithValue("@year", date.Year);
            command.Parameters.AddWithValue("@type", Type);
            command.Parameters.AddWithValue("@id", ID);
            Fill(command);
        }
    }

    public void LoadByItem(ReferenceType refType, int refID, int? userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Reminders WHERE (UserID = @UserID OR @UserID < 0) AND (IsDismissed = 0) AND (RefType = @RefType) AND (RefID = @RefID) ORDER BY DueDate";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID ?? -1);
        command.Parameters.AddWithValue("@RefType", refType);
        command.Parameters.AddWithValue("@RefID", refID);
        Fill(command);
      }
    }

    public void LoadForEmail()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Reminders WHERE (HasEmailSent = 0) AND (IsDismissed = 0) AND (DueDate <= GETUTCDATE()) ORDER BY DueDate";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }
  }
  
}
