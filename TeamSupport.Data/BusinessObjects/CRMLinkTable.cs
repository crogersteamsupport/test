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
    public void LoadActive()
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
ORDER BY clt.LastProcessed ASC
";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }
  }
  
}
