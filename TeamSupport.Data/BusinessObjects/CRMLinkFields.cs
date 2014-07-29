using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CRMLinkField
  {
  }
  
  public partial class CRMLinkFields
  {

    public void LoadByCrmLinkID(int crmLinkID)
    { 
      
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = " SELECT * FROM CrmLinkFields WHERE (CrmLinkID = @CrmLinkID) ORDER BY CrmObjectName, CrmFieldName, TsFieldName";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@CrmLinkID", crmLinkID);
        Fill(command);
      }

    }

      public void LoadByObjectType(string objType, int CRMLinkID) {

          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = "SELECT * FROM CRMLinkFields WHERE CRMLinkID = @CRMLinkID AND CRMObjectName = @objectType";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@objectType", objType);
              command.Parameters.AddWithValue("@CRMLinkID", CRMLinkID);
              Fill(command, "CRMLinkFields");
          }
      }

      public void LoadByObjectTypeAndCustomFieldAuxID(string objType, int CRMLinkID, int customFieldAuxID)
      {

        using (SqlCommand command = new SqlCommand())
        {
          command.CommandText = @"
          SELECT 
            crm.* 
          FROM 
            CRMLinkFields crm
            LEFT JOIN CustomFields cf
              ON crm.CustomFieldID = cf.CustomFieldID
          WHERE 
            crm.CRMLinkID = @CRMLinkID 
            AND crm.CRMObjectName = @objectType
            AND (crm.CustomFieldID IS NULL OR cf.AuxID = @CustomFieldAuxID)";
          command.CommandType = CommandType.Text;
          command.Parameters.AddWithValue("@objectType", objType);
          command.Parameters.AddWithValue("@CRMLinkID", CRMLinkID);
          command.Parameters.AddWithValue("@CustomFieldAuxID", customFieldAuxID);
          Fill(command, "CRMLinkFields");
        }
      }

      public CRMLinkField FindByCRMFieldName(string name)
      {
          foreach (CRMLinkField field in this) { 
              if (field.CRMFieldName.Trim().ToLower() == name.Trim().ToLower()){
                  return field;
              }
          }
          return null;
      }
  }
  
}
