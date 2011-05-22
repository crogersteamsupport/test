using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CustomValue 
  {
    //cf.Name AS FieldName, cf.ApiFieldName, cf.FieldType, cf.ListValues
    public string FieldName
    {
      get
      {
        if (Row.Table.Columns.Contains("FieldName") && Row["FieldName"] != DBNull.Value)
        {
          return (string)Row["FieldName"];
        }
        else return "";
      }
    }
    public string ApiFieldName
    {
      get
      {
        if (Row.Table.Columns.Contains("ApiFieldName") && Row["ApiFieldName"] != DBNull.Value)
        {
          return (string)Row["ApiFieldName"];
        }
        else return "";
      }
    }
    public string ListValues
    {
      get
      {
        if (Row.Table.Columns.Contains("ListValues") && Row["ListValues"] != DBNull.Value)
        {
          return (string)Row["ListValues"];
        }
        else return "";
      }
    }
    public CustomFieldType FieldType
    {
      get
      {
        if (Row.Table.Columns.Contains("FieldType") && Row["FieldType"] != DBNull.Value)
        {
          return (CustomFieldType)Row["FieldType"];
        }
        else return CustomFieldType.Text;
      }
    }
    public string Name
    {
      get
      {
        if (Row.Table.Columns.Contains("Name") && Row["Name"] != DBNull.Value)
        {
          return (string)Row["Name"];
        }
        else return "";
      }
    }
    public string Description
    {
      get
      {
        if (Row.Table.Columns.Contains("Description") && Row["Description"] != DBNull.Value)
        {
          return (string)Row["Description"];
        }
        else return "";
      }
    }
    public ReferenceType RefType
    {
      get
      {
        if (Row.Table.Columns.Contains("RefType") && Row["RefType"] != DBNull.Value)
        {
          return (ReferenceType)Row["RefType"];
        }
        else return ReferenceType.None;
      }
    }
    public int? AuxID
    {
      get
      {
        if (Row.Table.Columns.Contains("AuxID") && Row["AuxID"] != DBNull.Value)
        {
          return (int?)Row["AuxID"];
        }
        else return null;
      }
    }
    public int Position
    {
      get
      {
        if (Row.Table.Columns.Contains("Position") && Row["Position"] != DBNull.Value)
        {
          return (int)Row["Position"];
        }
        else return -1;
      }
    }
    public bool IsVisibleOnPortal
    {
      get
      {
        if (Row.Table.Columns.Contains("IsVisibleOnPortal") && Row["IsVisibleOnPortal"] != DBNull.Value)
        {
          return (bool)Row["IsVisibleOnPortal"];
        }
        else return false;
      }
    }
    public bool IsFirstIndexSelect
    {
      get
      {
        if (Row.Table.Columns.Contains("IsFirstIndexSelect") && Row["IsFirstIndexSelect"] != DBNull.Value)
        {
          return (bool)Row["IsFirstIndexSelect"];
        }
        else return false;
      }
    }
    public bool IsRequired
    {
      get
      {
        if (Row.Table.Columns.Contains("IsRequired") && Row["IsRequired"] != DBNull.Value)
        {
          return (bool)Row["IsRequired"];
        }
        else return false;
      }
    }
    public int OrganizationID
    {
      get
      {
        if (Row.Table.Columns.Contains("OrganizationID") && Row["OrganizationID"] != DBNull.Value)
        {
          return (int)Row["OrganizationID"];
        }
        else return -1;
      }
    }
  }

  public partial class CustomValues 
  {
    public void LoadByFieldID(int customFieldID, int refID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM CustomValues WHERE (RefID = @RefID) AND (CustomFieldID = @CustomFieldID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefID", refID);
        command.Parameters.AddWithValue("@CustomFieldID", customFieldID);
        Fill(command);
      }
    }

    public void LoadByReferenceType(int organizationID, ReferenceType refType, int refID)
    {
      LoadByReferenceType(organizationID, refType, null, refID); 
    }

    public void LoadByReferenceType(int organizationID, ReferenceType refType, int? auxID, int refID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
SELECT 
cv.CustomValueID, 
cv.RefID, 
cv.CustomValue, 
cv.DateCreated, 
cv.DateModified, 
cv.CreatorID, 
cv.ModifierID, 
cf.Name, 
cf.ApiFieldName, 
cf.FieldType, 
cf.ListValues, 
cf.Description, 
cf.RefType, 
cf.AuxID, 
cf.Position, 
cf.IsVisibleOnPortal, 
cf.IsFirstIndexSelect,
cf.IsRequired,
cf.OrganizationID, 
cf.CustomFieldID
FROM CustomFields cf LEFT JOIN CustomValues cv on cv.CustomFieldID = cf.CustomFieldID AND cv.RefID=@RefID
WHERE cf.OrganizationID = @OrganizationID
AND cf.RefType=@RefType
AND (cf.AuxID = @AuxID OR @AuxID < 0)
ORDER BY cf.Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@RefID", refID);
        command.Parameters.AddWithValue("@RefType", (int)refType);
        command.Parameters.AddWithValue("@AuxID", auxID ?? -1);
        Fill(command, "CustomFields, CustomValues");
      }
    }

    partial void BeforeRowEdit(CustomValue newValue)
    {
      CustomValue oldValue = CustomValues.GetCustomValue(LoginUser, newValue.CustomValueID);
      if (oldValue.Value == newValue.Value) return;
      CustomField customField = CustomFields.GetCustomField(LoginUser, newValue.CustomFieldID);
      string format = "Changed {0} from '{1}' to '{2}'.";
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, customField.RefType, newValue.RefID, string.Format(format, customField.Name, oldValue.Value, newValue.Value));
    }

    public static CustomValue GetValue(LoginUser loginUser, int customFieldID, int refID)
    {
      CustomValues values = new CustomValues(loginUser);
      values.LoadByFieldID(customFieldID, refID);

      if (values.IsEmpty)
      {
        CustomField field = CustomFields.GetCustomField(loginUser, customFieldID);
        values = new CustomValues(loginUser);
        CustomValue value = values.AddNewCustomValue();
        value.CustomFieldID = customFieldID;
        value.Value = ""; 
        if (field.FieldType == CustomFieldType.PickList)
        {      
          string[] items = field.ListValues.Split('|');
          if (items.Length > 0) value.Value = items[0];
        }

        
        value.RefID = refID;
        return value;
      }
      else
      {
        return values[0];
      }
        
    }
  }
}
