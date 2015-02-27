using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class Import
  {
  }
  
  public partial class Imports
  {

    public void LoadWaiting()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Imports WHERE (IsDone = 0) AND (IsRunning = 0) ORDER BY DateCreated";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }
  }
  
}
