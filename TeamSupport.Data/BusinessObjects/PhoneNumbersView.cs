using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class PhoneNumbersViewItem
  {
  }
  
  public partial class PhoneNumbersView
  {
    public void LoadByID(int refID, ReferenceType referenceType)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = " SELECT * FROM PhoneNumbersView WHERE (RefID = @RefID) AND (RefType = @RefType)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefID", refID);
        command.Parameters.AddWithValue("@RefType", referenceType);
        Fill(command, "PhoneNumbers,PhoneTypes");
      }
    }

  }
  
}
