using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class Report
  {
    public string GetSql(bool isSchemaOnly)
    {
      return GetSql(isSchemaOnly, null);
    }

    public string GetSql(bool isSchemaOnly, ReportConditions extraConditions)
    {
      if (!string.IsNullOrEmpty(Query) || ReportSubcategoryID == null)
      {
        if (CustomRefType == ReferenceType.None) return Query;

        int fromStart = Query.IndexOf("FROM");
        string select = Query.Substring(0, fromStart) +
          DataUtils.GetCustomFieldColumns(
            Collection.LoginUser,
            CustomRefType,
            CustomAuxID,
            Collection.LoginUser.OrganizationID,
            CustomFieldKeyName,
            1000,
            false);

        string from = select + " " + Query.Substring(fromStart);
        return from;
      }
      else
      {
        ReportSubcategory sub = (ReportSubcategory)ReportSubcategories.GetReportSubcategory(Collection.LoginUser, (int)ReportSubcategoryID);

        ReportFields fields = new ReportFields(Collection.LoginUser);
        fields.LoadByReportID(ReportID);

        ReportTables tables = new ReportTables(Collection.LoginUser);
        tables.LoadAll();

        ReportTableFields tableFields = new ReportTableFields(Collection.LoginUser);
        tableFields.LoadAll();

        StringBuilder builder = new StringBuilder();
        foreach (ReportField field in fields)
        {

          if (field.IsCustomField)
          {
            CustomField customField = (CustomField)CustomFields.GetCustomField(Collection.LoginUser, field.LinkedFieldID);
            string fieldName = DataUtils.GetReportPrimaryKeyFieldName(customField.RefType);
            if (fieldName != "")
            {
              fieldName = DataUtils.GetCustomFieldColumn(Collection.LoginUser, customField, fieldName, true);
              if (builder.Length < 1)
              {
                builder.Append("SELECT " + fieldName);
              }
              else
              {
                builder.Append(", " + fieldName);
              }

            }

          }
          else
          {
            ReportTableField tableField = tableFields.FindByReportTableFieldID(field.LinkedFieldID);
            ReportTable table = tables.FindByReportTableID(tableField.ReportTableID);
            string fieldName = table.TableName + "." + tableField.FieldName;
            if (tableField.DataType.Trim().ToLower() == "text")
              fieldName = "dbo.StripHTML(" + fieldName + ")";

            if (builder.Length < 1)
            {
              builder.Append("SELECT " + fieldName + " AS [" + tableField.Alias + "]");
            }
            else
            {
              builder.Append(", " + fieldName + " AS [" + tableField.Alias + "]");
            }

          }
        }

        builder.Append(" " + sub.BaseQuery);

        ReportTable mainTable = tables.FindByReportTableID(sub.ReportCategoryTableID);
        builder.Append(" WHERE (" + mainTable.TableName + "." + mainTable.OrganizationIDFieldName + " = @OrganizationID)");
        if (isSchemaOnly) builder.Append(" AND (0=1)");
        if (!string.IsNullOrEmpty(QueryObject))
        {
          ReportConditions conditions = (ReportConditions)DataUtils.StringToObject(QueryObject);
          conditions.LoginUser = Collection.LoginUser;
          string where = conditions.GetSQL();
          if (where != "") builder.Append(" AND " + where);
        }

        if (extraConditions != null)
        {
          string where = extraConditions.GetSQL();
          if (where != "") builder.Append(" AND " + where);
        }

        return builder.ToString();
      }
    }

    public static void CreateParameters(LoginUser loginUser, SqlCommand command, int userID)
    {
      User user = Users.GetUser(loginUser, userID);
      if (command.CommandText.IndexOf("@OrganizationID") > -1)
      {
        command.Parameters.AddWithValue("OrganizationID", user.OrganizationID);
        command.Parameters.AddWithValue("Self", user.FirstLastName);
        command.Parameters.AddWithValue("SelfID", user.UserID);
        command.Parameters.AddWithValue("UserID", user.UserID);
      }
      else
      {
        throw new Exception("Missing OrganizationID parameter in report query.");
      }
    }
  }

  public partial class Reports
  {

    public void LoadAll(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Reports WHERE (OrganizationID = @OrganizationID) OR (OrganizationID IS NULL) ORDER BY Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }
  }
}
