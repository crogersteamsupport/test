using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class PhoneType
  {
  }

  public partial class PhoneTypes   
  {

    public void LoadByOrganizationID(int organizationID, string orderBy = "Position")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM PhoneTypes WHERE OrganizationID = @OrganizationID  ORDER BY " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    partial void BeforeDBDelete(int phoneTypeID)
    {



    }

    partial void AfterDBDelete(int phoneTypeID)
    {
      ValidatePositions(LoginUser.OrganizationID);
    
    }

      public PhoneType FindByName(string name)
    {
      foreach (PhoneType phoneType in this)
      {
        if (phoneType.Name == name)
        {
          return phoneType;
        }
      }
      return null;
    }    
    
  }
}
