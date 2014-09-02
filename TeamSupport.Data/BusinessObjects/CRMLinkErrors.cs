using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CRMLinkError
  {
  }
  
  public partial class CRMLinkErrors
  {
    public void LoadByOperation(int organizationID, string CRMType, string orientation, string objectType)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
        @"
          SELECT 
            * 
          FROM 
            CRMLinkErrors
          WHERE 
            OrganizationID = @OrganizationID
            AND CRMType = @CRMType
            AND Orientation = @Orientation
            AND ObjectType = @ObjectType
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@CRMType", CRMType);
        command.Parameters.AddWithValue("@Orientation", orientation);
        command.Parameters.AddWithValue("@ObjectType", objectType);

        Fill(command, "CRMLinkErrors");
      }
    }

    public CRMLinkError FindByObjectIDAndFieldName(string objectID, string fieldName)
    {
      foreach (CRMLinkError item in this)
      {
        if (item.ObjectID == objectID && item.ObjectFieldName == fieldName)
        {
          return item;
        }
      }
      return null;
    }
  }
  
}
