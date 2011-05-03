using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ActionLogsViewItem
  {
  }
  
  public partial class ActionLogsView
  {
    public void LoadByReference(int refID, ReferenceType referenceType)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = " SELECT * FROM ActionLogsView WHERE (RefID = @RefID) AND (RefType = @RefType) AND (OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefID", refID);
        command.Parameters.AddWithValue("@RefType", referenceType);
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        Fill(command);
      }
    }

  }
  
}
