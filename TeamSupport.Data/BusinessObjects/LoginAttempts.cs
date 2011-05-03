using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class LoginAttempt
  {
  }
  
  public partial class LoginAttempts
  {

    public static int GetAttemptCount(LoginUser loginUser, int userID, int minutes)
    {
      using (SqlCommand command = new SqlCommand())
      {
        DateTime dateCreated = DateTime.UtcNow.AddMinutes(-minutes);
        command.CommandText = "SELECT COUNT(*) FROM LoginAttempts WHERE DateCreated > @DateCreated AND UserID = @UserID AND Successful = 0";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@DateCreated", dateCreated);

        Organizations organizations = new Organizations(loginUser);
        object o = organizations.ExecuteScalar(command);
        if (o == DBNull.Value)
        {
          return 0;
        }
        else
        {
          return (int)o;
        }
      }
    }


    public static void AddAttempt(LoginUser loginUser, int userID, bool success)
    {
      LoginAttempts loginAttempts = new LoginAttempts(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "INSERT INTO LoginAttempts (UserID, DateCreated, Successful) VALUES (@UserID, @DateCreated, @Success)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@Success", success);
        command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
        loginAttempts.ExecuteNonQuery(command, "LoginAttempts");
      }
    }


  }
  
}
