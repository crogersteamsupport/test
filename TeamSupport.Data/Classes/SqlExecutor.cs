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
        command.Connection = connection;
        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
        {
          adapter.FillSchema(result, SchemaType.Source);
          adapter.Fill(result);
        }
        connection.Close();
      }
      return result;
    }

    public static object ExecuteScalar(LoginUser loginUser, SqlCommand command)
    {
      BaseCollection.FixCommandParameters(command);
      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        object o;
        connection.Open();
        command.Connection = connection;
        o = command.ExecuteScalar();
        connection.Close();
        return o;
      }
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
