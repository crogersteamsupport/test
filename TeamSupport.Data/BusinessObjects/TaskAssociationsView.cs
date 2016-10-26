using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TaskAssociationsViewItem
  {
  }
  
  public partial class TaskAssociationsView
  {
    public void LoadByReminderIDOnly(int ReminderID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
                SELECT 
                    * 
                FROM 
                    TaskAssociationsView
                WHERE 
                    ReminderID = @ReminderID
                ORDER BY RefType";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ReminderID", ReminderID);
            Fill(command);
        }
    }
  }
}