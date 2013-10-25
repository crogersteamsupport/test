using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Security;


namespace TeamSupport.Data
{
  public class SqlExecutor
  {
    public static DataTable ExecuteQuery(LoginUser loginUser, SqlCommand command)
    {
      DataTable result = new DataTable();
      BaseCollection.FixCommandParameters(command);
      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
        command.Connection = connection;
        command.Transaction = transaction;
        try
        {
          transaction.Commit();
          using (SqlDataAdapter adapter = new SqlDataAdapter(command))
          {
            adapter.Fill(result);
          }
        }
        catch (Exception e)
        {
          transaction.Rollback();
          e.Data["CommandText"] = command.CommandText;
          ExceptionLogs.LogException(loginUser, e, "SqlExecutor.Fill");
          throw;
        }
        connection.Close();
      }
      return result;
    }

    public static object ExecuteScalar(LoginUser loginUser, string commandText)
    {
      return ExecuteScalar(loginUser, new SqlCommand(commandText));
    }

    public static object ExecuteScalar(LoginUser loginUser, SqlCommand command)
    {
      BaseCollection.FixCommandParameters(command);
      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        object o;
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
        command.Connection = connection;
        command.Transaction = transaction;
        try
        {
          o = command.ExecuteScalar();
          transaction.Commit();
        }
        catch (Exception e)
        {
          transaction.Rollback();
          e.Data["CommandText"] = command.CommandText;
          ExceptionLogs.LogException(loginUser, e, "SqlExecutor.ExecuteScalar");
          throw;
        }

        connection.Close();
        if (o == null || o == DBNull.Value) return null;
        return o;
      }
    }

    public static int ExecuteInt(LoginUser loginUser, string commandText)
    {
      return ExecuteInt(loginUser, new SqlCommand(commandText));
    }

    public static int ExecuteInt(LoginUser loginUser, SqlCommand command)
    {
      int? result = (int?)ExecuteScalar(loginUser, command);
      return result == null ? -1 : (int)result;
    }

    public static void ExecuteNonQuery(LoginUser loginUser, string commandText)
    {
      ExecuteNonQuery(loginUser, new SqlCommand(commandText));
    }
    
    public static void ExecuteNonQuery(LoginUser loginUser, SqlCommand command)
    {
      BaseCollection.FixCommandParameters(command);
      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        connection.Open();
        command.Connection = connection;
        command.ExecuteNonQuery();
        connection.Close();
      }
    }
  }

}
