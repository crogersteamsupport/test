using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CRMLinkTableItem
  {
    #region Properties

    public string SecurityToken1
    {
      get
      {
        string result = null;

        if (Row["SecurityToken"] != DBNull.Value)
        {
          string rawValue = (string)Row["SecurityToken"];
          int commaIndex = rawValue.IndexOf(", ");
          if (commaIndex >= 0)
          {
            result = rawValue.Substring(commaIndex + 2);
          }
          else
          {
            result = rawValue;
          }
        }

        return result;
      }

      set { Row["SecurityToken"] = CheckValue("SecurityToken", value); }
    }

    #endregion

  }
  
  public partial class CRMLinkTable
  {
    /// <summary>
    /// Loads the record with the matching OrganizationID
    /// </summary>
    /// <param name="organizationID"></param>
    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM CRMLinkTable WHERE OrganizationID = @OrganizationID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    /// <summary>
    /// Loads all the active CRM Link Table Items and sorts them by LastProcessed date
    /// </summary>
    public void LoadActive(int processInterval)
    {
      //This Query loads all the active CRMLinkTable items
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"SELECT clt.* 
FROM CRMLinkTable clt 
INNER JOIN Organizations o 
ON clt.OrganizationID = o.OrganizationID 
WHERE clt.Active = 1 AND o.IsActive=1
	AND clt.LastProcessed < DATEADD(MINUTE, @ProcessInterval, GETUTCDATE())
ORDER BY clt.LastProcessed ASC
";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProcessInterval", processInterval * -1);
        Fill(command);
      }
    }

    public static List<string> GetOrganizationJiraProjectKeys(int organizationId, LoginUser login)
    {
      List<string> jiraProjectKeys = new List<string>();

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"SELECT JiraProjectKey
FROM Products
WHERE organizationId = @organizationId AND JiraProjectKey IS NOT NULL AND JiraProjectKey != ''
UNION
SELECT JiraProjectKey
FROM ProductVersions
WHERE productId IN (SELECT productId FROM Products WHERE organizationId = @organizationId AND JiraProjectKey IS NOT NULL AND JiraProjectKey != '') AND JiraProjectKey IS NOT NULL AND JiraProjectKey != ''
UNION
SELECT DefaultProject
FROM CrmLinkTable
WHERE organizationId = @organizationId AND crmType = 'Jira' AND DefaultProject IS NOT NULL AND DefaultProject != ''
";

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@organizationId", organizationId);

        using (SqlConnection connection = new SqlConnection(login.ConnectionString))
        {
          connection.Open();
          SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
          command.Connection = connection;
          command.Transaction = transaction;
          command.CommandTimeout = 300;
          SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

          while (reader.Read())
          {
            string jiraProjectKey = reader["JiraProjectKey"].ToString();
            jiraProjectKeys.Add(jiraProjectKey);
          }
        }
      }

      return jiraProjectKeys.ToList();
    }
  }
  
}
