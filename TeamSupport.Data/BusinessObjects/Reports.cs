using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;


namespace TeamSupport.Data
{
  public partial class Report
  {
    // begin old stuff
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
            if (customField == null) continue;
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

    public string GetSqlWithOrderByClause(bool isSchemaOnly, ReportConditions extraConditions)
    {
      string orderByClause = null;
      ReportData reportData = new ReportData(Collection.LoginUser);
      reportData.LoadReportData(ReportID, Collection.LoginUser.UserID);
      if (!reportData.IsEmpty)
      {
        orderByClause = reportData[0].OrderByClause;
      }

      if (!string.IsNullOrEmpty(Query) || ReportSubcategoryID == null)
      {
        if (CustomRefType == ReferenceType.None)
        {
          string result = Query;
          if (!string.IsNullOrEmpty(orderByClause))
          {
            result = AddOrderByClause(result.Replace("_", " "), orderByClause);
          }
          return result;
        }

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

        if (!string.IsNullOrEmpty(orderByClause))
        {
          from = AddOrderByClause(from.Replace("_", " "), orderByClause);
        }

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

        string completeQuery = builder.ToString();

        if (!string.IsNullOrEmpty(orderByClause))
        {
          completeQuery = AddOrderByClause(completeQuery.Replace("_", " "), orderByClause);
        }

        return completeQuery;
      }
    }

    private string AddOrderByClause(string query, string orderByClause)
    {
      string result = query;
      int indexOfOrderBy = query.ToLower().LastIndexOf("order by");
      if (indexOfOrderBy > 0)
      {
        result = query.Substring(0, indexOfOrderBy);
      }
      return result + " ORDER BY " + orderByClause;
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

    public bool IsFavorite {
        get
        {
            return UserSettings.ReadString(Collection.LoginUser, "FavoriteReport", "").Split(',').Contains(this.ReportID.ToString());
        }
        set {
            string currentSetting = UserSettings.ReadString(Collection.LoginUser, "FavoriteReport", "");
            string newSetting = "";

            if (value && !IsFavorite)
            {
                newSetting = currentSetting + "," + this.ReportID.ToString();
            }
            else if (!value && IsFavorite)
            {
                newSetting = string.Join(",", currentSetting.Split(',').Where(val => val != this.ReportID.ToString()).ToArray());
            }

            if (newSetting != currentSetting)
            {
                newSetting = newSetting.Trim(',');
                UserSettings.WriteString(Collection.LoginUser, "FavoriteReport", newSetting);
            }
        }
    }
    // end old stuff


    public int CloneReport(string newName)
    {
      Reports reports = new Reports(this.Collection.LoginUser);
      Report report = reports.AddNewReport();

      report.ReportType = this.ReportType;
      report.LastSqlExecuted = this.LastSqlExecuted;
      report.ExternalURL = this.ExternalURL;
      report.QueryObject = this.QueryObject;
      report.ReportSubcategoryID = this.ReportSubcategoryID;
      report.CustomAuxID = this.CustomAuxID;
      report.Row["CustomRefType"] = this.Row["CustomRefType"];
      report.CustomFieldKeyName = this.CustomFieldKeyName;
      report.Query = this.Query;

      report.Description = this.Description;
      report.Name = newName;
      report.OrganizationID = this.Collection.LoginUser.OrganizationID;
      report.DateCreated = DateTime.UtcNow;
      report.DateModified = DateTime.UtcNow;
      report.ModifierID = this.Collection.LoginUser.UserID;
      report.CreatorID = this.Collection.LoginUser.UserID;
      report.ReportDefType = this.ReportDefType;
      report.ReportDef = this.ReportDef;
      reports.Save();

      return report.ReportID;
    }
  }

  public partial class Reports
  {
    // begin old stuff
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

    public void LoadStandard() {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT * FROM Reports WHERE OrganizationID IS NULL AND ExternalURL IS NULL ORDER BY Name";
            command.CommandType = CommandType.Text;
            Fill(command);
        }
    }

    public void LoadGraphical(int organizationID) {
        using (SqlCommand command = new SqlCommand()) {
            command.CommandText = "SELECT * FROM Reports WHERE ISNULL(OrganizationID,@OrganizationID) = @OrganizationID AND ExternalURL IS NOT NULL ORDER BY NAME";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            Fill(command);
        }
    }

    public void LoadCustom(int organizationID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT * FROM Reports WHERE OrganizationID = @OrganizationID AND ExternalURL IS NULL ORDER BY Name";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            Fill(command);
        }
    }

    public void LoadFavorites() {
        if (UserSettings.ReadString(LoginUser, "FavoriteReport", "") != "")
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT * FROM Reports WHERE ReportID IN (" + UserSettings.ReadString(LoginUser, "FavoriteReport", "").Trim(',') +
                                        ") AND ISNULL(OrganizationID,@OrganizationID) = @OrganizationID ORDER BY Name";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
                    Fill(command);
            }
        }
    }
    // end old stuff

    public void LoadAll(int organizationID, int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
SELECT r.*, 
ISNULL((SELECT rus.IsFavorite FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsFavorite,
ISNULL((SELECT rus.IsHidden FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsHidden
FROM Reports r
WHERE (r.OrganizationID = @OrganizationID) OR (r.OrganizationID IS NULL) 
AND r.ReportID NOT IN (SELECT ros.ReportID FROM ReportOrganizationSettings ros WHERE ros.OrganizationID = @OrganizationID AND ros.IsHidden=1) 
ORDER BY IsFavorite DESC, r.Name
";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command);
      }
    }

    public static void HideStockReport(LoginUser loginUser, int organizationID, int reportID)
    {
      SqlCommand command = new SqlCommand();

      command.CommandText = @"
UPDATE ReportOrganizationSettings SET IsHidden=1 WHERE OrganizationID = @OrganizationID AND ReportID = @ReportID
IF @@ROWCOUNT=0
    INSERT INTO ReportOrganizationSettings (OrganizationID, ReportID, IsHidden) VALUES (@OrganizationID, @ReportID, 1)
";
      command.Parameters.AddWithValue("OrganizationID", organizationID);
      command.Parameters.AddWithValue("ReportID", reportID);
      SqlExecutor.ExecuteNonQuery(loginUser, command); 
    }

    public static void SetIsFavorite(LoginUser loginUser, int userID, int reportID, bool value)
    {
      SqlCommand command = new SqlCommand();

      command.CommandText = @"
UPDATE ReportUserSettings SET IsFavorite=@value WHERE UserID = @UserID AND ReportID = @ReportID
IF @@ROWCOUNT=0
    INSERT INTO ReportUserSettings (UserID, ReportID, IsFavorite) VALUES (@UserID, @ReportID, @value)
";
      command.Parameters.AddWithValue("UserID", userID);
      command.Parameters.AddWithValue("ReportID", reportID);
      command.Parameters.AddWithValue("value", value);

      SqlExecutor.ExecuteNonQuery(loginUser, command);
    }

    public static void SetIsHiddenFromUser(LoginUser loginUser, int userID, int reportID, bool value)
    {
      SqlCommand command = new SqlCommand();

      command.CommandText = @"
UPDATE ReportUserSettings SET IsHidden=@value WHERE UserID = @UserID AND ReportID = @ReportID
IF @@ROWCOUNT=0
    INSERT INTO ReportUserSettings (UserID, ReportID, IsHidden) VALUES (@UserID, @ReportID, @value)
";
      command.Parameters.AddWithValue("UserID", userID);
      command.Parameters.AddWithValue("ReportID", reportID);
      command.Parameters.AddWithValue("value", value);

      SqlExecutor.ExecuteNonQuery(loginUser, command);
    }

    public Report FindByName(string name)
    {
      name = name.Trim().ToLower();
      foreach (Report report in this)
      {
        if (report.Name.ToLower().Trim() == name)
        {
          return report;
        }
      }
      return null;
    }
  }


  public class TabularReport
  {
    public TabularReport() { }
    public int Category { get; set; }
    public int Subcategory { get; set; }
    public ReportSelectedField[] Fields { get; set; }
    public ReportFilter[] Filters { get; set; }
  }

  public class ReportSelectedField 
  {
    public ReportSelectedField() { }
    public int FieldID { get; set; }
    public bool IsCustom { get; set; }
  }

  public class ReportFilter
  {
    public ReportFilter() { }
    public string Conjunction { get; set; }
    public ReportFilterCondition[] Conditions { get; set; }
    public ReportFilter[] Filters { get; set; }
  }

  public class ReportFilterCondition
  {
    public ReportFilterCondition() { }
    public string FieldID { get; set; }
    public bool IsCustom { get; set; }
    public string Comparator { get; set; }
    public string Value1 { get; set; }
  }

 




}
