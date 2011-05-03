using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class EscNotTrigger
  {
  }
  
  public partial class EscNotTriggers
  {

    public void LoadActive(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM EscNotTriggers WHERE OrganizationID = @OrganizationID AND Active=1";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command, "EscNotTriggers");
      }
    }
  }
  
}
