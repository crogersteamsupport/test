using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class DeletedIndexItem
  {
  }
  
  public partial class DeletedIndexItems
  {

    public void LoadByReferenceType(ReferenceType refType)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM DeletedIndexItems WHERE RefType = @RefType";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefType", (int)refType);
        Fill(command);
      }
    }
  }
  
}
