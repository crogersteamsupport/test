using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TaskEmailPost
  {
  }
  
  public partial class TaskEmailPosts
  {
        public static void UnlockThread(LoginUser loginUser, int thread)
        {
            TaskEmailPosts taskEmailPosts = new TaskEmailPosts(loginUser);

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE TaskEmailPosts SET LockProcessID = NULL WHERE LockProcessID = @id";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("id", thread);
                taskEmailPosts.ExecuteNonQuery(command);
            }
        }

        public static void UnlockAll(LoginUser loginUser)
        {
            TaskEmailPosts taskEmailPosts = new TaskEmailPosts(loginUser);

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE TaskEmailPosts SET LockProcessID = NULL";
                command.CommandType = CommandType.Text;
                taskEmailPosts.ExecuteNonQuery(command);
            }
        }

        public static TaskEmailPost GetNextWaiting(LoginUser loginUser, string processID)
        {
            TaskEmailPosts emails = new TaskEmailPosts(loginUser);

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                    UPDATE
                        TaskEmailPosts 
                    SET
                        LockProcessID = @ProcessID 
                    OUTPUT
                        Inserted.*
                    WHERE
                        TaskEmailPostID IN 
                        (
                            SELECT 
                                TOP 1
                                TaskEmailPostID 
                            FROM
                                TaskEmailPosts
                            WHERE
                                LockProcessID IS NULL 
                                AND DATEADD(SECOND, HoldTime, DateCreated) < GETUTCDATE() 
                            ORDER BY 
                                DateCreated
                        )
                ";

                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ProcessID", processID);
                emails.Fill(command);
            }

            if (emails.IsEmpty)
                return null;
            else
                return emails[0];
        }

        public void LoadByReminderID(int reminderID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM TaskEmailPosts WHERE ReminderID = @ReminderID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ReminderID", reminderID);
                Fill(command);
            }
        }
    }

}
