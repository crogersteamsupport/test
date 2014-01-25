using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Newtonsoft.Json;


namespace TeamSupport.Data
{
  public partial class Report
  {
    // begin old stuff
    public string GetSqlOld(bool isSchemaOnly)
    {
      return GetSqlOld(isSchemaOnly, null);
    }

    public string GetSqlOld(bool isSchemaOnly, ReportConditions extraConditions)
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

    public void GetCommand(SqlCommand command)
    {
      GetCommand(command, true, false);
    }

    public void GetCommand(SqlCommand command, bool inlcudeHiddenFields)
    {
      GetCommand(command, inlcudeHiddenFields, false);
    }

    public void GetCommand(SqlCommand command, bool inlcudeHiddenFields, bool isSchemaOnly) 
    {
      MigrateToNewReport();

      command.CommandType = CommandType.Text;
      switch (ReportDefType)
      {
        case ReportType.Table:
          GetTabularSql(command, inlcudeHiddenFields, isSchemaOnly);
          break;
        case ReportType.Chart:
          GetSummarySql(command);
          break;
        case ReportType.Custom:
          GetCustomSql(command, isSchemaOnly);
          break;
        case ReportType.Summary:
          GetSummarySql(command);
          break;
        default:
          break;
      }
      if (!isSchemaOnly)
      {
        LastSqlExecuted = command.CommandText;
        Collection.Save();

        ReportView reportView = (new ReportViews(Collection.LoginUser)).AddNewReportView();
        reportView.UserID = Collection.LoginUser.UserID;
        reportView.ReportID = ReportID;
        reportView.DateViewed = DateTime.UtcNow;
        reportView.SQLExecuted = command.CommandText;
        reportView.Collection.Save();
      }
      

      User user = Users.GetUser(Collection.LoginUser, Collection.LoginUser.UserID);
      if (command.CommandText.IndexOf("@OrganizationID") > -1)
      {
        command.Parameters.AddWithValue("OrganizationID", user.OrganizationID);
        command.Parameters.AddWithValue("Self", user.FirstLastName);
        command.Parameters.AddWithValue("SelfID", user.UserID);
        command.Parameters.AddWithValue("UserID", user.UserID);
      }
      else
      {
       // throw new Exception("Missing OrganizationID parameter in report query.");
      }

    }

    private void GetCustomSql(SqlCommand command, bool isSchemaOnly)
    {
      CustomReport customReport = JsonConvert.DeserializeObject<CustomReport>(ReportDef);
      command.CommandText = customReport.Query;
    }

    private void GetTabularSql(SqlCommand command, bool inlcudeHiddenFields, bool isSchemaOnly)
    {
      TabularReport tabularReport = JsonConvert.DeserializeObject<TabularReport>(ReportDef);

      StringBuilder builder = new StringBuilder();
      GetTabluarSelectClause(command, builder, tabularReport, inlcudeHiddenFields, isSchemaOnly);
      if (isSchemaOnly)
      {
        command.CommandText = builder.ToString();
      }
      else
      {
        GetWhereClause(command, builder, tabularReport.Filters);
        Report report = Reports.GetReport(Collection.LoginUser, ReportID, Collection.LoginUser.UserID);
        if (report != null && report.Row["Settings"] != DBNull.Value)
        {
          try
          {
            UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)report.Row["Settings"]);
            GetWhereClause(command, builder, userFilters.Filters);
          }
          catch (Exception ex)
          {
            ExceptionLogs.LogException(Collection.LoginUser, ex, "Tabular SQL - User filters");
          }
        }

        command.CommandText = builder.ToString();
      }
    }

    private void GetTabluarSelectClause(SqlCommand command, StringBuilder builder, TabularReport tabularReport, bool includeHiddenFields, bool isSchemaOnly)
    {
      ReportSubcategory sub = ReportSubcategories.GetReportSubcategory(Collection.LoginUser, tabularReport.Subcategory);

      ReportTables tables = new ReportTables(Collection.LoginUser);
      tables.LoadAll();

      ReportTableFields tableFields = new ReportTableFields(Collection.LoginUser);
      tableFields.LoadAll();
      TimeSpan offset = Collection.LoginUser.TimeZoneInfo.BaseUtcOffset;

      foreach (ReportSelectedField field in tabularReport.Fields)
      {

        if (field.IsCustom)
        {
          CustomField customField = (CustomField)CustomFields.GetCustomField(Collection.LoginUser, field.FieldID);
          if (customField == null) continue;
          string fieldName = DataUtils.GetReportPrimaryKeyFieldName(customField.RefType);
          if (fieldName != "")
          {
            fieldName = DataUtils.GetCustomFieldColumn(Collection.LoginUser, customField, fieldName, true, false);
            if (customField.FieldType == CustomFieldType.DateTime)
            {
              fieldName = string.Format("SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}')",
              fieldName,
              offset < TimeSpan.Zero ? "-" : "+",
              Math.Abs(offset.Hours),
              Math.Abs(offset.Minutes));
            }

            if (builder.Length < 1)
            {
              builder.Append(string.Format("SELECT {0} AS [{1}]", fieldName, customField.Name));
            }
            else
            {
              builder.Append(string.Format(", {0} AS [{1}]", fieldName, customField.Name));
            }

          }

        }
        else
        {
          ReportTableField tableField = tableFields.FindByReportTableFieldID(field.FieldID);
          ReportTable table = tables.FindByReportTableID(tableField.ReportTableID);
          string fieldName = table.TableName + "." + tableField.FieldName;
          if (tableField.DataType.Trim().ToLower() == "text")
            fieldName = "dbo.StripHTML(" + fieldName + ")";
          if (tableField.DataType.Trim().ToLower() == "datetime")
          {
            fieldName = string.Format("SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}')",
              fieldName,
              offset < TimeSpan.Zero ? "-" : "+",
              Math.Abs(offset.Hours),
              Math.Abs(offset.Minutes));

          }
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

      if (includeHiddenFields)
      {
        ReportTable hiddenTable = tables.FindByReportTableID(sub.ReportCategoryTableID);
        if (!string.IsNullOrWhiteSpace(hiddenTable.LookupKeyFieldName))
        {
          builder.Append(string.Format(", {1}.{0} AS [hidden{0}]", hiddenTable.LookupKeyFieldName, hiddenTable.TableName));
          if (sub.ReportTableID != null && !string.IsNullOrWhiteSpace(hiddenTable.LookupKeyFieldName))
          {
            hiddenTable = tables.FindByReportTableID((int)sub.ReportTableID);
            builder.Append(string.Format(", {1}.{0} AS [hidden{0}]", hiddenTable.LookupKeyFieldName, hiddenTable.TableName));
          }
        }

      }
      builder.Append(" " + sub.BaseQuery);

      ReportTable mainTable = tables.FindByReportTableID(sub.ReportCategoryTableID);
      builder.Append(" WHERE (" + mainTable.TableName + "." + mainTable.OrganizationIDFieldName + " = @OrganizationID)");
      if (isSchemaOnly) builder.Append(" AND (0=1)");
    }

    private void GetWhereClause(SqlCommand command, StringBuilder builder, ReportFilter[] filters)
    {
      WriteFilters(command, builder, filters, null);
    }

    private void WriteFilters(SqlCommand command, StringBuilder builder, ReportFilter[] filters, ReportFilter parentFilter)
    {
      foreach (ReportFilter filter in filters)
      {
        if (filter.Conditions.Length < 1) continue;

        builder.Append(string.Format(" {0} (", parentFilter == null ? "AND" : parentFilter.Conjunction.ToUpper()));
        WriteFilter(command, builder, filter);
        WriteFilters(command, builder, filter.Filters, filter);
        builder.Append(")");
      }
    }

    private void WriteFilter(SqlCommand command, StringBuilder builder, ReportFilter filter)
    {
      for (int i = 0; i < filter.Conditions.Length; i++)
      {
        ReportFilterCondition condition = filter.Conditions[i];
        if (i > 0) builder.Append(string.Format(" {0} ", filter.Conjunction.ToUpper()));
        builder.Append("(");
        WriteFilterCondition(command, builder, condition);
        builder.Append(")");
      }
    }

    private void WriteFilterCondition(SqlCommand command, StringBuilder builder, ReportFilterCondition condition)
    {
      string fieldName = "";
      string dataType;
      if (condition.IsCustom)
      {
        CustomField customField = TeamSupport.Data.CustomFields.GetCustomField(Collection.LoginUser, condition.FieldID);
        if (customField == null) return;
        fieldName = DataUtils.GetCustomFieldColumn(Collection.LoginUser, customField, DataUtils.GetReportPrimaryKeyFieldName(customField.RefType), true, false);
        switch (customField.FieldType)
        {
          case CustomFieldType.DateTime:
            dataType = "datetime";
            break;
          case CustomFieldType.Boolean:
            dataType = "bit";
            break;
          case CustomFieldType.Number:
            dataType = "float";
            break;
          default:
            dataType = "text";
            break;
        }
        
      }
      else
      {
        ReportTableField field = ReportTableFields.GetReportTableField(Collection.LoginUser, condition.FieldID);
        ReportTable reportTable = ReportTables.GetReportTable(Collection.LoginUser, field.ReportTableID);
        fieldName = reportTable.TableName + ".[" + field.FieldName + "]";
        dataType = field.DataType;
      }

      string paramName = string.Format("Param{0:D5}", command.Parameters.Count + 1);
      switch (dataType)
      {
        case "float":
          switch (condition.Comparator.ToUpper())
          {
            case "IS":
              builder.Append(string.Format("{0} = @{1}", fieldName, paramName));
              command.Parameters.Add(paramName, SqlDbType.Float).Value = float.Parse(condition.Value1);
              break;
            case "IS NOT":
              builder.Append(string.Format("{0} <> @{1}", fieldName, paramName));
              command.Parameters.Add(paramName, SqlDbType.Float).Value = float.Parse(condition.Value1);
              break;
            case "LESS THAN": builder.Append(string.Format("{0} < @{2}", fieldName, paramName));
              command.Parameters.AddWithValue(paramName, float.Parse(condition.Value1));
              break;
            case "GREATER THAN": builder.Append(string.Format("{0} > @{2}", fieldName, paramName));
              command.Parameters.AddWithValue(paramName, float.Parse(condition.Value1));
              break;
            case "IS EMPTY":
              builder.Append(string.Format("{0} IS NULL", fieldName));
              break;
            case "IS NOT EMPTY":
              builder.Append(string.Format("{0} IS NOT NULL", fieldName));
              break;
            default:
              break;
          }
          break;
        case "int":
          switch (condition.Comparator.ToUpper())
          {
            case "IS":
              builder.Append(string.Format("{0} = @{1}", fieldName, paramName));
              command.Parameters.Add(paramName, SqlDbType.Int).Value = int.Parse(condition.Value1);
              break;
            case "IS NOT":
              builder.Append(string.Format("{0} <> @{1}", fieldName, paramName));
              command.Parameters.Add(paramName, SqlDbType.Int).Value = int.Parse(condition.Value1);
              break;
            case "LESS THAN": builder.Append(string.Format("{0} < @{2}", fieldName, paramName));
              command.Parameters.AddWithValue(paramName, int.Parse(condition.Value1));
              break;
            case "GREATER THAN": builder.Append(string.Format("{0} > @{2}", fieldName, paramName));
              command.Parameters.AddWithValue(paramName, int.Parse(condition.Value1));
              break;
            case "IS EMPTY":
              builder.Append(string.Format("{0} IS NULL", fieldName));
              break;
            case "IS NOT EMPTY":
              builder.Append(string.Format("{0} IS NOT NULL", fieldName));
              break;
            default:
              break;
          }
          break;
        case "bit":
          switch (condition.Comparator.ToUpper())
          {
            case "IS TRUE": builder.Append(string.Format("{0} = 1", fieldName)); break;
            case "IS FALSE": builder.Append(string.Format("{0} = 0", fieldName)); break;
            case "IS EMPTY": builder.Append(string.Format("{0} IS NULL", fieldName)); break;
            case "IS NOT EMPTY": builder.Append(string.Format("{0} IS NOT NULL", fieldName)); break;
            default:
              break;
          }
          break;
        case "datetime":
          TimeSpan offset = Collection.LoginUser.TimeZoneInfo.BaseUtcOffset;
          string datetimeSql = string.Format("SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}')",
            fieldName,
            offset < TimeSpan.Zero ? "-" : "+",
            Math.Abs(offset.Hours),
            Math.Abs(offset.Minutes));
          string timeSql = string.Format("CAST({0} AS TIME)", datetimeSql);
          string dateSql = string.Format("CAST({0} AS DATE)", datetimeSql);
          switch (condition.Comparator.ToUpper())
          {

            case "IS":
              builder.Append(string.Format("{0} = @{1}", dateSql, paramName));
              command.Parameters.Add(paramName, SqlDbType.Date).Value = DateTime.Parse(condition.Value1);
              break;
            case "IS NOT":
              builder.Append(string.Format("{0} <> @{1}", dateSql, paramName));
              command.Parameters.Add(paramName, SqlDbType.Date).Value = DateTime.Parse(condition.Value1);
              break;
            case "IS BEFORE":
              builder.Append(string.Format("{0} < @{1}", dateSql, paramName));
              command.Parameters.Add(paramName, SqlDbType.Date).Value = DateTime.Parse(condition.Value1);
              break;
            case "IS AFTER":
              builder.Append(string.Format("{0} > @{1}", dateSql, paramName));
              command.Parameters.Add(paramName, SqlDbType.Date).Value = DateTime.Parse(condition.Value1);
              break;
            case "IS EMPY":
              builder.Append(string.Format("{0} IS NULL", fieldName));
              break;
            case "IS NOT EMPTY":
              builder.Append(string.Format("{0} IS NOT NULL", fieldName));
              break;
            case "CURRENT YEAR":
              builder.Append(string.Format("DATEPART(year, {0}) = {1}", datetimeSql, DateTime.Now.Year));
              break;
            case "PREVIOUS YEAR":
              builder.Append(string.Format("DATEPART(year, {0}) = {1}", datetimeSql, DateTime.Now.Year - 1));
              break;
            case "NEXT YEAR":
              builder.Append(string.Format("DATEPART(year, {0}) = {1}", datetimeSql, DateTime.Now.Year + 1));
              break;
            case "SPECIFIC YEAR": 
              builder.Append(string.Format("DATEPART(year, {0}) = @{1}", datetimeSql, paramName));
              command.Parameters.AddWithValue(paramName, int.Parse(condition.Value1));
              break;
            case "PREVIOUS # YEARS":
              builder.Append(string.Format("(DATEPART(year, {0}) < {1:D} AND DATEPART(year, {0}) > {2:D})", dateSql, DateTime.Now.Year, DateTime.Now.Year - int.Parse(condition.Value1)));
              break;
            case "LAST # YEARS":
              builder.Append(string.Format("(DATEPART(year, {0}) < {1:D} AND DATEPART(year, {0}) > {2:D})", dateSql, DateTime.Now.Year+1, DateTime.Now.Year - int.Parse(condition.Value1)));
              break;
            case "NEXT # YEARS":
              builder.Append(string.Format("(DATEPART(year, {0}) < {1:D} AND DATEPART(year, {0}) > {2:D})", dateSql, DateTime.Now.Year + int.Parse(condition.Value1), DateTime.Now.Year));
              break;

            case "CURRENT QUARTER":
              builder.Append(string.Format("(DATEPART(quarter, {0}) = {1:D} AND DATEPART(year, {0}) = {2:D})", dateSql, GetQuarter(DateTime.Now), DateTime.Now.Year));
              break;
            case "PREVIOUS QUARTER":
              int prevQuarter = GetQuarter(DateTime.Now) - 1;
              int prevYear = DateTime.Now.Year;
              if (prevQuarter < 1) 
              {
                prevQuarter = 4;
                prevYear--;
              }
              builder.Append(string.Format("(DATEPART(quarter, {0}) = {1:D} AND DATEPART(year, {0}) = {2:D})", dateSql, prevQuarter, prevYear));
              break;
            case "NEXT QUARTER": 
              int nextQuarter = GetQuarter(DateTime.Now) + 1;
              int nextYear = DateTime.Now.Year;
              if (nextQuarter > 4)
              {
                nextQuarter = 1;
                nextYear++;
              }
              builder.Append(string.Format("(DATEPART(quarter, {0}) = {1:D} AND DATEPART(year, {0}) = {2:D})", dateSql, nextQuarter, nextYear));
              break;
            case "SPECIFIC QUARTER":
              builder.Append(string.Format("DATEPART(quarter, {0}) = {1:D}", dateSql, int.Parse(condition.Value1)));
              break;
            case "PREVIOUS # QUARTERS": break;
            case "LAST # QUARTERS": break;
            case "NEXT # QUARTERS": break;

            case "CURRENT MONTH": 
              builder.Append(string.Format("(DATEPART(month, {0}) = {1:D} AND DATEPART(year, {0}) = {2:D})", dateSql, DateTime.Now.Month, DateTime.Now.Year));
              break;
            case "PREVIOUS MONTH": 
              int prevMonth = DateTime.Now.Month - 1;
              int prevMYear = DateTime.Now.Year;
              if (prevMonth < 1) 
              {
                prevMonth = 12;
                prevMYear--;
              }
              builder.Append(string.Format("(DATEPART(month, {0}) = {1:D} AND DATEPART(year, {0}) = {2:D})", dateSql, prevMonth, prevMYear));
              break;

            case "NEXT MONTH": 
              int nextMonth = DateTime.Now.Month + 1;
              int nextMYear = DateTime.Now.Year;
              if (nextMonth > 12)
              {
                nextMonth = 1;
                nextMYear++;
              }
              builder.Append(string.Format("(DATEPART(month, {0}) = {1:D} AND DATEPART(year, {0}) = {2:D})", dateSql, nextMonth, nextMYear));
              break;
            case "SPECIFIC MONTH": 
              builder.Append(string.Format("DATEPART(month, {0}) = {1:D}", dateSql, int.Parse(condition.Value1)));
              break;
            case "PREVIOUS # MONTHS": break;
            case "LAST # MONTHS": break;
            case "NEXT # MONTHS": break;

            case "CURRENT WEEK":
              builder.Append(string.Format("(DATEPART(week, {0}) = DATEPART(week, GETUTCDATE()) AND DATEPART(year, {0}) = {1:D})", dateSql, DateTime.Now.Year));
              break;
            case "PREVIOUS WEEK": 
              builder.Append(string.Format("(DATEPART(week, {0}) = DATEPART(week, GETUTCDATE())-1 AND DATEPART(year, {0}) = {1:D})", dateSql, DateTime.Now.Year));
              break;
            case "NEXT WEEK": 
              builder.Append(string.Format("(DATEPART(week, {0}) = DATEPART(week, GETUTCDATE())+1 AND DATEPART(year, {0}) = {1:D})", dateSql, DateTime.Now.Year));
              break;
            case "PREVIOUS # WEEKS": break;
            case "LAST # WEEKS": break;
            case "NEXT # WEEKS": break;

            case "TODAY":
              builder.Append(string.Format("{0} = @{1}", dateSql, paramName));
              command.Parameters.Add(paramName, SqlDbType.Date).Value = DataUtils.DateToLocal(Collection.LoginUser, DateTime.UtcNow).Date;
              break;
            case "YESTERDAY": 
              builder.Append(string.Format("{0} = @{1}", dateSql, paramName));
              command.Parameters.Add(paramName, SqlDbType.Date).Value = DataUtils.DateToLocal(Collection.LoginUser, DateTime.UtcNow).AddDays(-1).Date;
              break;
            case "TOMORROW": 
              builder.Append(string.Format("{0} = @{1}", dateSql, paramName));
              command.Parameters.Add(paramName, SqlDbType.Date).Value = DataUtils.DateToLocal(Collection.LoginUser, DateTime.UtcNow).AddDays(-1).Date;
              break;
            case "SPECIFIC DAY":
              builder.Append(string.Format("DATEPART(weekday, {0}) = {1:D}", dateSql, int.Parse(condition.Value1)));
              break;
            case "PREVIOUS # DAYS":
              string paramName2 = paramName + "-2";
              builder.Append(string.Format("({0} >= @{1} AND {0} < @{2})", dateSql, paramName, paramName2));
              command.Parameters.Add(paramName, SqlDbType.Date).Value = DataUtils.DateToLocal(Collection.LoginUser, DateTime.UtcNow).AddDays(-1*int.Parse(condition.Value1)).Date;
              command.Parameters.Add(paramName2, SqlDbType.Date).Value = DataUtils.DateToLocal(Collection.LoginUser, DateTime.UtcNow).Date;
              break;
            case "LAST # DAYS": 
              string paramName2a = paramName + "-2";
              builder.Append(string.Format("({0} >= @{1} AND {0} <= @{2})", dateSql, paramName, paramName2a));
              command.Parameters.Add(paramName, SqlDbType.Date).Value = DataUtils.DateToLocal(Collection.LoginUser, DateTime.UtcNow).AddDays(-1*int.Parse(condition.Value1)).Date;
              command.Parameters.Add(paramName2a, SqlDbType.Date).Value = DataUtils.DateToLocal(Collection.LoginUser, DateTime.UtcNow).Date;
              break;
            case "NEXT # DAYS": 
              string paramName2b = paramName + "-2";
              builder.Append(string.Format("({0} >= @{1} AND {0} <= @{2})", dateSql, paramName2b, paramName));
              command.Parameters.Add(paramName, SqlDbType.Date).Value = DataUtils.DateToLocal(Collection.LoginUser, DateTime.UtcNow).AddDays(int.Parse(condition.Value1)).Date;
              command.Parameters.Add(paramName2b, SqlDbType.Date).Value = DataUtils.DateToLocal(Collection.LoginUser, DateTime.UtcNow).Date;
              break;

            case "CURRENT HOUR": break;
            case "PREVIOUS HOUR": break;
            case "NEXT HOUR": break;
            case "SPECIFIC HOUR": break;
            case "PREVIOUS # HOURS": break;
            case "LAST # HOURS": break;
            case "NEXT # HOURS": break;
            case "": break;
            default:
              break;
          }
          break;
        default:
          if (condition.Value1 != null) command.Parameters.AddWithValue(paramName, condition.Value1);
          switch (condition.Comparator.ToUpper())
          {
            case "IS": builder.Append(string.Format("{0} = @{1}", fieldName, paramName)); break;
            case "IS NOT": builder.Append(string.Format("{0} <> @{1}", fieldName, paramName)); break;
            case "CONTAINS": builder.Append(string.Format("{0} LIKE '%' + @{1} + '%'", fieldName, paramName)); break;
            case "DOES NOT CONTAIN": builder.Append(string.Format("{0} NOT LIKE '%' + @{1} + '%'", fieldName, paramName)); break;
            case "STARTS WITH": builder.Append(string.Format("{0} LIKE @{1} + '%'", fieldName, paramName)); break;
            case "ENDS WITH": builder.Append(string.Format("{0} LIKE '%' + @{1}", fieldName, paramName)); break;
            case "IS EMPTY": builder.Append(string.Format("{0} IS NULL", fieldName)); break;
            case "IS NOT EMPTY": builder.Append(string.Format("{0} IS NOT NULL", fieldName)); break;
            default:
              break;
          }
          break;
      }
    }

    public static int GetQuarter(DateTime date)
    {
      if (date.Month >= 4 && date.Month <= 6)
        return 1;
      else if (date.Month >= 7 && date.Month <= 9)
        return 2;
      else if (date.Month >= 10 && date.Month <= 12)
        return 3;
      else
        return 4;

    }

    public void GetSummarySql(SqlCommand command)
    {
       
    }


    public ReportColumn[] GetSqlColumns()
    {
      DataTable table = new DataTable();

      using (SqlCommand command = new SqlCommand())
      {
        GetCommand(command, false, true);

        using (SqlConnection connection = new SqlConnection(Collection.LoginUser.ConnectionString))
        {
          connection.Open();
          command.Connection = connection;
          using (SqlDataAdapter adapter = new SqlDataAdapter(command))
          {
            adapter.FillSchema(table, SchemaType.Source);
            adapter.Fill(table);
          }
          connection.Close();
        }
      }

      List<ReportColumn> result = new List<ReportColumn>();
      foreach (DataColumn column in table.Columns)
      {
        ReportColumn col = new ReportColumn();
        col.Name = column.ColumnName;
        col.IsEmail = false;
        col.IsLink = false;
        col.IsOpenable = false;
        col.IsCustomField = false;
        col.FieldID = -1;
        col.OpenField = "";
        if (column.DataType == typeof(System.DateTime)) col.DataType = "datetime";
        else if (column.DataType == typeof(System.Boolean)) col.DataType = "bit";
        else if (column.DataType == typeof(System.Int32)) col.DataType = "int";
        else if (column.DataType == typeof(System.Double)) col.DataType = "float";
        
        result.Add(col);

      }

      return result.ToArray();
    }

    public ReportColumn[] GetTabularColumns()
    {
      List<ReportColumn> result = new List<ReportColumn>();
      TabularReport tabularReport = JsonConvert.DeserializeObject<TabularReport>(ReportDef);
      ReportTables tables = new ReportTables(Collection.LoginUser);
      tables.LoadAll();

      ReportTableFields tableFields = new ReportTableFields(Collection.LoginUser);
      tableFields.LoadAll();

      foreach (ReportSelectedField field in tabularReport.Fields)
      {

        if (field.IsCustom)
        {
          CustomField customField = (CustomField)CustomFields.GetCustomField(Collection.LoginUser, field.FieldID);
          if (customField == null) continue;
          ReportColumn col = new ReportColumn();
          col.Name = customField.Name;
          col.IsEmail = false;
          col.IsLink = false;
          col.IsOpenable = false;
          col.OpenField = "";
          col.IsCustomField = true;
          col.FieldID = customField.CustomFieldID;

          switch (customField.FieldType)
          {
            case CustomFieldType.DateTime: col.DataType = "datetime"; break;
            case CustomFieldType.Boolean: col.DataType = "bit"; break;
            case CustomFieldType.Number: col.DataType = "float"; break;
            default: col.DataType = "varchar"; break;
          }
          result.Add(col);

        }
        else
        {
          ReportTableField tableField = tableFields.FindByReportTableFieldID(field.FieldID);
          ReportColumn col = new ReportColumn();
          col.Name = tableField.Alias;
          col.IsEmail = tableField.IsEmail;
          col.IsLink = tableField.IsLink;
          col.IsOpenable = tableField.IsOpenable;
          col.OpenField = tables.FindByReportTableID(tableField.ReportTableID).LookupKeyFieldName;
          col.DataType = tableField.DataType;
          col.IsCustomField = false;
          col.FieldID = tableField.ReportTableFieldID;
          result.Add(col);

        }
      }

      return result.ToArray();
    }

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

    public void MigrateToNewReport()
    {
      if (ReportDef == null)
      {
        if (!string.IsNullOrWhiteSpace(Query))
        {
          ReportDefType = Data.ReportType.Custom;
          CustomReport customReport = new CustomReport();
          customReport.Query = Query;
          string q = Query.ToLower();

          var start = q.IndexOf(" order by ");
          if (start > -1)
          {
            customReport.Query = Query.Substring(0, start);
          }

          customReport.UsePaging = Query.ToLower().IndexOf(" top ") < 0 && Query.ToLower().IndexOf(" group by ") < 0 && Query.ToLower().IndexOf(" distinct ") < 0;
          ReportDef = JsonConvert.SerializeObject(customReport);
        }
        else if (!string.IsNullOrWhiteSpace(ExternalURL))
        {
          ReportDef = ExternalURL;
          if (ExternalURL.IndexOf("../") == 0) ReportDef = "../../" + ReportDef;
          ReportDefType = Data.ReportType.External;
        }
        else if (ReportSubcategoryID != null && QueryObject != null)
        {
          TabularReport tabularReport = new TabularReport();
          tabularReport.Subcategory = (int)ReportSubcategoryID;

          ReportFields fields = new ReportFields(Collection.LoginUser);
          fields.LoadByReportID(ReportID);
          List<ReportSelectedField> selectedFields = new List<ReportSelectedField>();
          foreach (ReportField field in fields)
          {
            ReportSelectedField selectedField = new ReportSelectedField();
            selectedField.FieldID = field.LinkedFieldID;
            selectedField.IsCustom = field.IsCustomField;
            selectedFields.Add(selectedField);
          }

          tabularReport.Fields = selectedFields.ToArray();

          ReportConditions conditions = (ReportConditions)DataUtils.StringToObject(QueryObject);
          conditions.LoginUser = Collection.LoginUser;

          List<ReportFilter> filters = new List<ReportFilter>();
          ReportFilter filter = new ReportFilter();
          filter.Filters = new ReportFilter[0];
          filters.Add(filter);

          filter.Conjunction = conditions.MatchAll ? "AND" : "OR";
          List<ReportFilterCondition> filterConditions = new List<ReportFilterCondition>();
          foreach (ReportCondition condition in conditions.Items)
          {
            ReportFilterCondition filterCondition = new ReportFilterCondition();
            filterCondition.FieldID = condition.FieldID;
            filterCondition.IsCustom = condition.IsCustomField;
            filterCondition.Value1 = condition.Value1 == null ? "" : condition.Value1.ToString();

            string dataType;

            if (condition.IsCustomField)
            {
              CustomField customField = TeamSupport.Data.CustomFields.GetCustomField(Collection.LoginUser, condition.FieldID);
              if (customField == null) continue;
              switch (customField.FieldType)
              {
                case CustomFieldType.DateTime:
                  dataType = "datetime";
                  break;
                case CustomFieldType.Boolean:
                  dataType = "bit";
                  break;
                case CustomFieldType.Number:
                  dataType = "float";
                  break;
                default:
                  dataType = "text";
                  break;
              }
              
            }
            else
            {
              ReportTableField field = ReportTableFields.GetReportTableField(Collection.LoginUser, condition.FieldID);
              dataType = field.DataType;
            }

            switch (dataType)
            {
              case "datetime":
                switch (condition.ConditionOperator)
                {
                  case ConditionOperator.IsEqualTo:
                    filterCondition.Comparator = "Is";
                    break;
                  case ConditionOperator.IsNotEqualTo:
                    filterCondition.Comparator = "Is Not";
                    break;
                  case ConditionOperator.IsGreaterThan:
                    filterCondition.Comparator = "Is After";
                    break;
                  case ConditionOperator.IsLessThan:
                    filterCondition.Comparator = "Is Before";
                    break;
                  case ConditionOperator.IsInBetween:
                    break;
                  case ConditionOperator.IsNotInBetween:
                    break;
                  default:
                    break;
                }
                break;
              case "bit":
                switch (condition.ConditionOperator)
                {
                  case ConditionOperator.IsEqualTo:
                    filterCondition.Comparator = ((bool)condition.Value1) ? "Is True" : "Is False";
                    break;
                  case ConditionOperator.IsNotEqualTo:
                    filterCondition.Comparator = ((bool)condition.Value1) ? "Is False" : "Is True";
                    break;
                  default:
                    break;
                }
                break;
              case "int":
              case "float":
                switch (condition.ConditionOperator)
                {
                  case ConditionOperator.IsEqualTo:
                    filterCondition.Comparator = "Is";
                    break;
                  case ConditionOperator.IsNotEqualTo:
                    filterCondition.Comparator = "Is Not";
                    break;
                  case ConditionOperator.IsGreaterThan:
                    filterCondition.Comparator = "Greater Than";
                    break;
                  case ConditionOperator.IsLessThan:
                    filterCondition.Comparator = "Less Than";
                    break;
                  case ConditionOperator.IsInBetween:
                    break;
                  case ConditionOperator.IsNotInBetween:
                    break;
                  default:
                    break;
                }
                break;
              default:
                switch (condition.ConditionOperator)
                {
                  case ConditionOperator.IsEqualTo:
                    filterCondition.Comparator = "Is";
                    break;
                  case ConditionOperator.IsNotEqualTo:
                    filterCondition.Comparator = "Is Not";
                    break;
                  case ConditionOperator.StartsWith:
                    filterCondition.Comparator = "Starts With";
                    break;
                  case ConditionOperator.EndsWith:
                    filterCondition.Comparator = "Ends With";
                    break;
                  case ConditionOperator.Contains:
                    filterCondition.Comparator = "Contains";
                    break;
                  default:
                    break;
                }
                break;
            }

            filterConditions.Add(filterCondition);
          }
          filter.Conditions = filterConditions.ToArray();
          tabularReport.Filters = filters.ToArray();

          ReportDefType = Data.ReportType.Table;
          ReportDef = JsonConvert.SerializeObject(tabularReport);
        }
      }

     Collection.Save();
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

    public static GridResult GetReportData(LoginUser loginUser, int reportID, int from, int to, string sortField, bool isDesc)
    {
      Report report = Reports.GetReport(loginUser, reportID);

      if (report.ReportDefType == ReportType.Custom)
      {
        CustomReport customReport = JsonConvert.DeserializeObject<CustomReport>(report.ReportDef);
        if (customReport.UsePaging)
        {
          try
          {
            return GetReportDataPage(loginUser, report, from, to, sortField, isDesc);
          }
          catch (Exception ex)
          {
            customReport.UsePaging = false;
            report.ReportDef = JsonConvert.SerializeObject(customReport);
            report.Collection.Save();

            try
            {
              return GetReportDataAll(loginUser, report, sortField, isDesc);
            }
            catch (Exception ex2)
            {
              ExceptionLogs.LogException(loginUser, ex2, "Custom Report");
              throw;
            }
          }
        }
        else
        {
          return GetReportDataAll(loginUser, report, sortField, isDesc);
        }

      }
      else
      {
        return GetReportDataPage(loginUser, report, from, to, sortField, isDesc);
      }

    }

    private static GridResult GetReportDataPage(LoginUser loginUser, Report report, int from, int to, string sortField, bool isDesc)
    {
      from++;
      to++;

      SqlCommand command = new SqlCommand();
      string query = @"
WITH 
q AS ({0}),
r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY [{1}] {2}) AS 'RowNum' FROM q)
SELECT  *, (SELECT MAX(RowNum) FROM r) AS 'TotalRows' FROM r
WHERE RowNum BETWEEN @From AND @To";

      if (string.IsNullOrWhiteSpace(sortField))
      {
        sortField = GetReportColumnNames(loginUser, report.ReportID)[0];
        isDesc = false;
      }
      command.Parameters.AddWithValue("@From", from);
      command.Parameters.AddWithValue("@To", to);
      report.GetCommand(command);
      command.CommandText = string.Format(query, command.CommandText, sortField, isDesc ? "DESC" : "ASC");

      DataTable table = new DataTable();
      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        connection.Open();
        command.Connection = connection;
        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
        {
          try
          {
            adapter.Fill(table);
          }
          catch (Exception ex)
          {
            ExceptionLogs.LogException(loginUser, ex, "Report Data");
            throw;
          }
        }
        connection.Close();
      }

      GridResult result = new GridResult();
      result.From = --from;
      result.To = --to;
      result.Data = JsonConvert.SerializeObject(table);
      return result;
    }

    private static GridResult GetReportDataAll(LoginUser loginUser, Report report, string sortField, bool isDesc)
    {
      SqlCommand command = new SqlCommand();

      report.GetCommand(command);

      if (command.CommandText.ToLower().IndexOf(" order by ") < 0 && !string.IsNullOrWhiteSpace(sortField))
      {
        command.CommandText = command.CommandText + " ORDER BY " + sortField + (isDesc ? "DESC" : "ASC");
      }

      DataTable table = new DataTable();
      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        connection.Open();
        command.Connection = connection;
        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
        {
          try
          {
            adapter.Fill(table);
          }
          catch (Exception ex)
          {
            ExceptionLogs.LogException(loginUser, ex, "Report Data");
            throw;
          }
        }
        connection.Close();
      }

      GridResult result = new GridResult();
      result.From = 0;
      result.To = table.Rows.Count - 1;
      result.Data = JsonConvert.SerializeObject(table);
      return result;

    }

    public static string[] GetReportColumnNames(LoginUser loginUser, int reportID)
    {
      List<string> result = new List<string>();
      ReportColumn[] columns = GetReportColumns(loginUser, reportID);
      foreach (ReportColumn column in columns)
      {
        result.Add(column.Name);
      }
      return result.ToArray();
    }

    public static ReportColumn[] GetReportColumns(LoginUser loginUser, int reportID)
    {
      Report report = Reports.GetReport(loginUser, reportID);
      if (report.ReportDefType == ReportType.Custom) return report.GetSqlColumns();
      return report.GetTabularColumns();
    }

    public void LoadByFolder(int organizationID, int folderID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT r.* FROM Reports r LEFT JOIN ReportOrganizationSettings ros ON ros.ReportID = r.ReportID WHERE r.OrganizationID = @OrganizationID AND ros.FolderID = @FolderID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@FolderID", folderID);
        Fill(command);
      }
    }

    public void LoadAll(int organizationID, int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
SELECT r.*, u1.FirstName + ' ' + u1.LastName AS Creator, u2.FirstName + ' ' + u2.LastName AS Modifier,
(SELECT TOP 1 rv.DateViewed FROM ReportViews rv WHERE rv.ReportID = r.ReportID AND rv.UserID = @UserID ORDER BY rv.DateViewed DESC) AS LastViewed,
ISNULL((SELECT rus.Settings FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), '') AS Settings,
ISNULL((SELECT rus.IsFavorite FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsFavorite,
ISNULL((SELECT rus.IsHidden FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsHidden,
(SELECT ros.FolderID FROM ReportOrganizationSettings ros WHERE ros.ReportID = r.ReportID AND ros.OrganizationID = @OrganizationID) AS FolderID
FROM Reports r
LEFT JOIN Users u1 ON u1.UserID = r.CreatorID
LEFT JOIN Users u2 ON u2.UserID = r.ModifierID
WHERE (r.OrganizationID = @OrganizationID) OR (r.OrganizationID IS NULL) 
AND r.ReportID NOT IN (SELECT ros.ReportID FROM ReportOrganizationSettings ros WHERE ros.OrganizationID = @OrganizationID AND ros.IsHidden=1) 
ORDER BY r.Name
";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command);
      }
    }

    public static Report GetReport(LoginUser loginUser, int reportID, int userID)
    {
      Reports reports = new Reports(loginUser);
      reports.LoadByReportID(reportID);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
SELECT r.*, u1.FirstName + ' ' + u1.LastName AS Creator, u2.FirstName + ' ' + u2.LastName AS Modifier,
(SELECT TOP 1 rv.DateViewed FROM ReportViews rv WHERE rv.ReportID = r.ReportID AND rv.UserID = @UserID ORDER BY rv.DateViewed DESC) AS LastViewed,
ISNULL((SELECT rus.Settings FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), '') AS Settings,
ISNULL((SELECT rus.IsFavorite FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsFavorite,
ISNULL((SELECT rus.IsHidden FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsHidden,
(SELECT ros.FolderID FROM ReportOrganizationSettings ros WHERE ros.ReportID = r.ReportID AND ros.OrganizationID = @OrganizationID) AS FolderID
FROM Reports r
LEFT JOIN Users u1 ON u1.UserID = r.CreatorID
LEFT JOIN Users u2 ON u2.UserID = r.ModifierID
WHERE (r.ReportID = @ReportID)";

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ReportID", reportID);
        command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
        command.Parameters.AddWithValue("@UserID", userID);
        reports.Fill(command);
      }

      if (reports.IsEmpty)
        return null;
      else
        return reports[0];
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

    public static void AssignFolder(LoginUser loginUser, int folderID, int organizationID, int reportID)
    {
      SqlCommand command = new SqlCommand();

      command.CommandText = @"
UPDATE ReportOrganizationSettings SET FolderID=@FolderID WHERE OrganizationID = @OrganizationID AND ReportID = @ReportID
IF @@ROWCOUNT=0
    INSERT INTO ReportOrganizationSettings (OrganizationID, ReportID, FolderID) VALUES (@OrganizationID, @ReportID, @FolderID)
";
      command.Parameters.AddWithValue("OrganizationID", organizationID);
      command.Parameters.AddWithValue("FolderID", folderID);
      command.Parameters.AddWithValue("ReportID", reportID);
      SqlExecutor.ExecuteNonQuery(loginUser, command);
    }

    public static void UnassignFolder(LoginUser loginUser, int folderID)
    {
      SqlCommand command = new SqlCommand();

      command.CommandText = @"UPDATE ReportOrganizationSettings SET FolderID=NULL WHERE FolderID=@FolderID";
      command.Parameters.AddWithValue("FolderID", folderID);
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

    public static void SetUserSettings(LoginUser loginUser, int userID, int reportID, string value)
    {
      SqlCommand command = new SqlCommand();

      command.CommandText = @"
UPDATE ReportUserSettings SET Settings=@value WHERE UserID = @UserID AND ReportID = @ReportID
IF @@ROWCOUNT=0
    INSERT INTO ReportUserSettings (UserID, ReportID, Settings) VALUES (@UserID, @ReportID, @value)
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

  public class CustomReport
  {
    public CustomReport() { }
    public bool UsePaging { get; set; }
    public string Query { get; set; }
  }

  public class TabularReport
  {
    public TabularReport() { }
    public int Subcategory { get; set; }
    public ReportSelectedField[] Fields { get; set; }
    public ReportFilter[] Filters { get; set; }
  }

  public class UserTabularSettings
  {
    public UserTabularSettings() { }
    public ReportFilter[] Filters { get; set; }
    public bool IsSortDesc { get; set; }
    public string sortField { get; set; }
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
    public int FieldID { get; set; }
    public bool IsCustom { get; set; }
    public string Comparator { get; set; }
    public string Value1 { get; set; }
  }

  [DataContract]
  public class ReportColumn
  {
    public ReportColumn() { }
    [DataMember] public string Name { get; set; }
    [DataMember] public string DataType { get; set; }
    [DataMember] public string OpenField { get; set; }
    [DataMember] public bool IsOpenable { get; set; }
    [DataMember] public bool IsEmail { get; set; }
    [DataMember] public bool IsLink { get; set; }
    [DataMember] public bool IsCustomField { get; set; }
    [DataMember] public int FieldID { get; set; }

  }

  [DataContract]
      public class ReportItem
      {
 
        public ReportItem(Report report, bool indcludeDef)
        {
          this.ReportID = report.ReportID;
          this.OrganizationID = report.OrganizationID;
          this.Name = report.Name;
          this.Description = report.Description;
          this.IsFavorite = (report.Row.Table.Columns.IndexOf("IsFavorite") < 0 || report.Row["IsFavorite"] == DBNull.Value ? false : (bool)report.Row["IsFavorite"]);
          this.IsHidden = (report.Row.Table.Columns.IndexOf("IsHidden") < 0 || report.Row["IsHidden"] == DBNull.Value ? false : (bool)report.Row["IsHidden"]);
          this.UserSettings = (report.Row.Table.Columns.IndexOf("Settings") < 0 || report.Row["Settings"] == DBNull.Value ? "" : (string)report.Row["Settings"]);
          this.LastModified = report.DateModifiedUtc;
          this.LastViewed = (report.Row.Table.Columns.IndexOf("LastViewed") < 0 || report.Row["LastViewed"] == DBNull.Value ? null : (DateTime?)report.Row["LastViewed"]);
          this.Creator = (report.Row.Table.Columns.IndexOf("Creator") < 0 || report.Row["Creator"] == DBNull.Value ? "" : (string)report.Row["Creator"]);
          this.Modifier = (report.Row.Table.Columns.IndexOf("Modifier") < 0 || report.Row["Modifier"] == DBNull.Value ? "" : (string)report.Row["Modifier"]);
          this.FolderID = (report.Row.Table.Columns.IndexOf("FolderID") < 0 || report.Row["FolderID"] == DBNull.Value ? null : (int?)report.Row["FolderID"]);
          if ((int)report.ReportDefType < 0)
          {
            if (!string.IsNullOrWhiteSpace(report.ExternalURL))
            {
              this.ReportType = ReportType.External;
            }
            else if (!string.IsNullOrWhiteSpace(report.Query))
            {
              this.ReportType = ReportType.Custom;
            }
            else if ((int)report.ReportType == 3)
            {
              this.ReportType = ReportType.Chart;
            }
            else
            {
              this.ReportType = ReportType.Table;
            }
          }
          else
          {
            this.ReportType = report.ReportDefType;
          }

          if (indcludeDef) this.ReportDef = report.ReportDef;
          
          this.CreatorID = report.CreatorID;
        }

        [DataMember] public int ReportID { get; set; }
        [DataMember] public int? OrganizationID { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] public ReportType ReportType { get; set; }
        [DataMember] public string ReportDef { get; set; }
        [DataMember] public string UserSettings { get; set; }
        [DataMember] public bool IsFavorite { get; set; }
        [DataMember] public bool IsHidden { get; set; }
        [DataMember] public int CreatorID { get; set; }
        [DataMember] public string Creator { get; set; }
        [DataMember] public string Modifier { get; set; }
        [DataMember] public DateTime? LastViewed { get; set; }
        [DataMember] public DateTime LastModified { get; set; }
        [DataMember] public int? FolderID { get; set; }
      }
 
        [DataContract]
      public class GridResult
      {
        public GridResult() { }
        [DataMember] public int From { get; set; }
        [DataMember] public int To { get; set; }
        [DataMember] public string Data { get; set; }
      }



}
