using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class SystemSetting 
  {
  }

  public partial class SystemSettings 
  {

    public static string ReadString(LoginUser loginUser, string key, string defaultValue)
    {
      string result = defaultValue;
      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT SettingValue FROM SystemSettings WHERE (SettingKey=@SettingKey)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@SettingKey", key);

        SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);
        if (reader.Read())
        {
          result = (string)reader[0];
        }

        connection.Close();
      }

      return result;
    }
    public static void WriteString(LoginUser loginUser, string key, string value)
    {


      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        connection.Open();

        using (SqlCommand command = new SqlCommand())
        {
          command.Connection = connection;
          command.CommandText = "uspWriteSystemSetting";
          command.CommandType = CommandType.StoredProcedure;
          command.Parameters.AddWithValue("@SettingKey", key);
          command.Parameters.AddWithValue("@SettingValue", value);
          command.Parameters.AddWithValue("@ModifierID", loginUser.UserID);
          command.ExecuteNonQuery();
        }
        connection.Close();
      }
    }

  }
}
