using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public class Logs
  {
    private LoginUser _loginUser;
    private string _connectionString;
    private string _application;
    private string _category;
    private int _failures;

    public static string GetConnectionString(LoginUser loginUser)
    {
      SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(loginUser.ConnectionString);
      builder.InitialCatalog = "TeamSupportAudit";
      return builder.ConnectionString;
    }

    public Logs(LoginUser loginUser, string application, string category)
    {
      _loginUser = loginUser;
      _application = application;
      _category = category;
      _connectionString = GetConnectionString(loginUser);
      _failures = 0;
    }

    public void Log(string text)
    {
      Log(text, null, null, null);
    }

    public void Log(string text, int? userID)
    {
      Log(text, null);
    }

    public void Log(string text, ReferenceType? refType, int? refID)
    {
      Log(text, null, refType, refID);
    }

    public void Log(string text, int? userID, ReferenceType? refType, int? refID)
    {
      if (_failures > 3) return;
      try
      {
        SqlCommand command = new SqlCommand();
        command.CommandText =
  @"INSERT INTO [Logs]
             ([Application]
             ,[Category]
             ,[RefType]
             ,[RefID]
             ,[LogText]
             ,[CreatorID])
       VALUES
             (@Application
             ,@Category
             ,@RefType
             ,@RefID
             ,@LogText
             ,@CreatorID)";

        command.Parameters.AddWithValue("Application", _application);
        command.Parameters.AddWithValue("Category", _category);
        command.Parameters.AddWithValue("RefType", (int?)refType ?? -1);
        command.Parameters.AddWithValue("RefID", refID ?? -1);
        command.Parameters.AddWithValue("LogText", text);
        command.Parameters.AddWithValue("CreatorID", userID ?? -1);
        command.CommandType = CommandType.Text;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
          command.Connection = connection;
          connection.Open();
          try
          {
            command.ExecuteNonQuery();
          }
          finally
          {
            connection.Close();
          }
        }
        _failures = 0;
      }
      catch (Exception ex)
      {
        _failures++;
        ExceptionLogs.LogException(_loginUser, ex, "Logs", _connectionString);
      }
    }


    public static void Log(LoginUser loginUser, string application, string category, string text, int? userID, ReferenceType? refType, int? refID)
    {
      Logs logs = new Logs(loginUser, application, category);
      logs.Log(text, userID, refType, refID);
    }
  }
}
