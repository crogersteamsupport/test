using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CustomFieldCategory
  {
  }
  
  public partial class CustomFieldCategories
  {

    partial void BeforeRowDelete(int customFieldCategoryID)
    {
      CustomFieldCategories cats = new CustomFieldCategories(LoginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE CustomFields SET CustomFieldCategoryID = NULL WHERE CustomFieldCategoryID = @CustomFieldCategoryID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@CustomFieldCategoryID", customFieldCategoryID);
        cats.ExecuteNonQuery(command, "CustomFields");
      }
    }

    public void LoadByRefType(ReferenceType refType)
    {
      LoadByRefType(refType, null);
    }

    public void LoadByRefType(ReferenceType refType, int? auxID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"SELECT * FROM CustomFieldCategories 
WHERE (RefType = @RefType) 
AND (AuxID = @AuxID OR @AuxID < 0)
AND (OrganizationID = @OrganizationID)
ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefType", refType);
        command.Parameters.AddWithValue("@AuxID", auxID == null ? -1 : (int)auxID);
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        Fill(command);
      }

    }

    public void LoadByOrganization(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT * FROM CustomFieldCategories WHERE (OrganizationID = @OrganizationID) ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        Fill(command);
      }

    }

    public static int GetMaxPosition(LoginUser loginUser, ReferenceType refType, int? auxID)
    {
      CustomFieldCategories cats = new CustomFieldCategories(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"SELECT MAX(Position) FROM CustomFieldCategories 
WHERE (RefType = @RefType) 
AND (AuxID = @AuxID OR @AuxID < 0)
AND (OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefType", refType);
        command.Parameters.AddWithValue("@AuxID", auxID == null ? -1 : (int)auxID);
        command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
        object o = cats.ExecuteScalar(command);
        if (o == null || o == DBNull.Value) return -1;
        return (int)o;
      }
    }

  }
  
}
