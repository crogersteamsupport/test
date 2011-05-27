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
    /// I need to start putting this mark up for the queries so we can see what they do in intellsense
    /// </summary>
    public void LoadActive()
    {
      //This Query loads all the active CRMLinkTable items
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM CRMLinkTable WHERE Active = 1";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }
  }
  
}
