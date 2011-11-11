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
  }
  
}
