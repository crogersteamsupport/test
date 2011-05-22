using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CustomField 
  {
    public object GetValue(int id)
    {
      object result = null;

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "uspGetCustomValue";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@RefID", id);
        command.Parameters.AddWithValue("@CustomFieldID", CustomFieldID);

        object o = (BaseCollection as CustomFields).ExecuteScalar(command, "CustomValues");
        if (o != null && o != DBNull.Value) 
        {
          result = o;
        }
        else if (FieldType == CustomFieldType.PickList)
        {
          string[] items = ListValues.Split('|');
          if (items.Length > 0) result = items[0];
        }
      }

      return result;
    }

    public object GetTypedValue(int id)
    {
      try
      {
        object result = GetValue(id);
        switch (FieldType)
        {
          case CustomFieldType.Text: return result;
          case CustomFieldType.DateTime: return DateTime.Parse(result.ToString(), BaseCollection.LoginUser.CultureInfo);
          case CustomFieldType.Boolean: return bool.Parse(result.ToString());
          case CustomFieldType.Number: return double.Parse(result.ToString());
          case CustomFieldType.PickList: return result;
          default: return null;
        }
      }
      catch 
      {
        return null;
      }
    }

    public void SetValue(int id, object o)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "uspSetCustomValue";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@CustomFieldID", CustomFieldID);
        command.Parameters.AddWithValue("@RefID", id);
        command.Parameters.AddWithValue("@CustomValue", o.ToString());
        command.Parameters.AddWithValue("@ModifierID", BaseCollection.LoginUser.UserID);
        BaseCollection.ExecuteNonQuery(command, "CustomValues");
      }
    }
  }

  public partial class CustomFields 
  {

    public CustomField FindByName(string name)
    {
      foreach (CustomField customField in this)
      {
        if (customField.Name.ToLower() == name.ToLower())
        {
          return customField;
        }
      }
      return null;
    }

    public CustomField FindByApiFieldName(string name)
    {
      foreach (CustomField customField in this)
      {
        if (customField.ApiFieldName.ToLower() == name.ToLower())
        {
          return customField;
        }
      }
      return null;
    }

    partial void BeforeDBDelete(int customFieldID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM CustomValues WHERE (CustomFieldID = @CustomFieldID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@CustomFieldID", customFieldID);
        ExecuteNonQuery(command, "CustomValues");
      }
    }

    public static string GetCustomFieldTypeName(CustomFieldType type)
    {
      string result;
      switch (type)
      {
        case CustomFieldType.Text: result = "Text"; break;
        case CustomFieldType.DateTime: result = "Date and Time"; break;
        case CustomFieldType.Boolean: result = "True or False"; break;
        case CustomFieldType.Number: result = "Number"; break;
        case CustomFieldType.PickList: result = "Pick List"; break;
        default: result = ""; break;
      }
      return result;
    }

    public void LoadByOrganization(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM CustomFields WHERE (OrganizationID = @OrganizationID) ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command, "CustomFields");
      }
    
    }

    public void LoadByReferenceType(int organizationID, ReferenceType refType, int? auxID)
    {
      if (auxID == null) auxID = -1;
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM CustomFields WHERE (OrganizationID = @OrganizationID) AND (RefType = @RefType) AND (AuxID = @AuxID OR @AuxID < 0) ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@RefType", (int)refType);
        command.Parameters.AddWithValue("@AuxID", auxID);
        Fill(command, "CustomFields");
      }
    }

    public void LoadByReferenceType(int organizationID, ReferenceType refType)
    {
      LoadByReferenceType(organizationID, refType, -1);

    }

    public void LoadByTicketTypeID(int organizationID, int ticketTypeID)
    {
      LoadByReferenceType(organizationID, ReferenceType.Tickets, ticketTypeID);
    }

    public void LoadByPosition(int organizationID, ReferenceType refType, int auxID, int position)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM CustomFields WHERE (OrganizationID = @OrganizationID) AND (RefType = @RefType) AND (AuxID = @AuxID OR AuxID < 0) AND (Position = @Position)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@RefType", refType);
        command.Parameters.AddWithValue("@AuxID", auxID);
        command.Parameters.AddWithValue("@Position", position);
        Fill(command);
      }

    }

    public void ValidatePositions(int organizationID, ReferenceType refType, int auxID)
    {
      CustomFields fields = new CustomFields(LoginUser);
      fields.LoadByReferenceType(organizationID, refType, auxID);
      int i = 0;
      foreach (CustomField field in fields)
      {
        field.Position = i;
        i++;
      }
      fields.Save();
    }

    public void MovePositionUp(int customFieldID)
    {
      CustomFields fields1 = new CustomFields(LoginUser);
      fields1.LoadByCustomFieldID(customFieldID);
      ValidatePositions(fields1[0].OrganizationID, fields1[0].RefType, fields1[0].AuxID);
      if (fields1.IsEmpty || fields1[0].Position < 1) return;

      CustomFields fields2 = new CustomFields(LoginUser);
      fields2.LoadByPosition(fields1[0].OrganizationID, fields1[0].RefType, fields1[0].AuxID, fields1[0].Position - 1);
      if (!fields2.IsEmpty)
      {
        fields2[0].Position = fields2[0].Position + 1;
        fields2.Save();
      }

      fields1[0].Position = fields1[0].Position - 1;
      fields1.Save();
    }

    public void MovePositionDown(int customFieldID)
    {
      CustomFields fields1 = new CustomFields(LoginUser);
      fields1.LoadByCustomFieldID(customFieldID);
      ValidatePositions(fields1[0].OrganizationID, fields1[0].RefType, fields1[0].AuxID);
      if (fields1.IsEmpty || fields1[0].Position >= GetMaxPosition(fields1[0].OrganizationID, fields1[0].RefType, fields1[0].AuxID)) return;

      CustomFields fields2 = new CustomFields(LoginUser);
      fields2.LoadByPosition(fields1[0].OrganizationID, fields1[0].RefType, fields1[0].AuxID, fields1[0].Position + 1);
      if (!fields2.IsEmpty)
      {
        fields2[0].Position = fields2[0].Position - 1;
        fields2.Save();
      }

      fields1[0].Position = fields1[0].Position + 1;
      fields1.Save();
    }

    public virtual int GetMaxPosition(int organizationID, ReferenceType refType, int auxID)
    {
      int position = -1;

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT MAX(Position) FROM CustomFields WHERE (RefType = @RefType) AND (AuxID = @AuxID OR AuxID < 0) AND (OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@RefType", refType);
        command.Parameters.AddWithValue("@AuxID", auxID);
        
        object o = ExecuteScalar(command);
        if (o == DBNull.Value) return -1;
        position = (int)o;
      }
      return position;
    }

    
  }
}
