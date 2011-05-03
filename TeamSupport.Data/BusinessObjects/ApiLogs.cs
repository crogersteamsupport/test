using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ApiLog
  {
  }
  
  public partial class ApiLogs
  {
    public static int GetDailyRequestCount(LoginUser loginUser, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT COUNT(*) FROM ApiLogs WHERE DateCreated > DATEADD(d, -1, GETUTCDATE()) AND OrganizationID=@OrganizationID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);

        ApiLogs apiLogs = new ApiLogs(loginUser);
        return (int)apiLogs.ExecuteScalar(command);
      }
    }

    
  }
  
}
