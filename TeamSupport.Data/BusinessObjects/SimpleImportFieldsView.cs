using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class SimpleImportFieldsViewItem
  {
  }
  
  public partial class SimpleImportFieldsView
  {
    public void LoadByRefType(int refType)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
        @"
          SELECT 
            * 
          FROM 
            SimpleImportFieldsView
          WHERE
            RefType = @RefType
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefType", (int)refType);

        Fill(command);
      }
    }
  }
  
}
