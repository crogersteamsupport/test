using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Globalization;


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

    public void GetCommand(SqlCommand command, bool inlcudeHiddenFields = true, bool isSchemaOnly = false, bool useUserFilter = true) 
    {
      MigrateToNewReport();

      command.CommandType = CommandType.Text;
      command.CommandTimeout = 180;
      switch (ReportDefType)
      {
        case ReportType.Table:
          GetTabularSql(Collection.LoginUser, command, JsonConvert.DeserializeObject<TabularReport>(ReportDef), inlcudeHiddenFields, isSchemaOnly, ReportID, useUserFilter);
          break;
        case ReportType.Chart:
          GetSummarySql(Collection.LoginUser, command, JsonConvert.DeserializeObject<SummaryReport>(ReportDef), isSchemaOnly, null, false, true);
          break;
        case ReportType.Custom:
          GetCustomSql(command, isSchemaOnly, useUserFilter);
          break;
        case ReportType.Summary:
          GetSummarySql(Collection.LoginUser, command, JsonConvert.DeserializeObject<SummaryReport>(ReportDef), isSchemaOnly, ReportID, useUserFilter, false);
          break;
        case ReportType.TicketView:
          GetTabularSql(Collection.LoginUser, command, JsonConvert.DeserializeObject<TabularReport>(ReportDef), inlcudeHiddenFields, isSchemaOnly, ReportID, useUserFilter);
          break;
        default:
          break;
      }

      AddCommandParameters(command, Collection.LoginUser);
    }

    private static void AddCommandParameters(SqlCommand command, LoginUser loginUser)
    {
      User user = loginUser.GetUser();

      if (command.CommandText.IndexOf("@OrganizationID") > -1)
      {
        command.Parameters.AddWithValue("OrganizationID", user.OrganizationID);
        command.Parameters.AddWithValue("Self", user.FirstLastName);
        command.Parameters.AddWithValue("SelfID", user.UserID);
        command.Parameters.AddWithValue("UserID", user.UserID);
  
        TimeSpan offset = loginUser.Offset;
        command.Parameters.AddWithValue("Offset", string.Format("{0}{1:D2}:{2:D2}", offset < TimeSpan.Zero ? "-" : "+", Math.Abs(offset.Hours), Math.Abs(offset.Minutes)));
      }
      else
      {
        // throw new Exception("Missing OrganizationID parameter in report query.");
      }
    }

    private void GetCustomSql(SqlCommand command, bool isSchemaOnly, bool useUserFilter)
    {
      if (isSchemaOnly)
      {
        command.CommandText = string.Format("WITH q AS ({0}) SELECT * FROM q WHERE (0=1)", Query);
        return;
      }

      Report report = Reports.GetReport(Collection.LoginUser, ReportID, Collection.LoginUser.UserID);
      if (report != null && report.Row["Settings"] != DBNull.Value)
      {
        try
        {
          UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)report.Row["Settings"]);
          StringBuilder builder = new StringBuilder();
          if (userFilters != null && userFilters.Filters != null && userFilters.Filters.Length > 0)
          {
            GetWhereClause(Collection.LoginUser, command, builder, userFilters.Filters);
            builder.Remove(0, 4);
            command.CommandText = string.Format("c AS ({0}), q AS (SELECT * FROM c WHERE {1})", Query, builder.ToString());
          }
          else
          {
            command.CommandText = string.Format("c AS ({0}), q AS (SELECT * FROM c)", Query);
          }
        }
        catch (Exception ex)
        {
          ExceptionLogs.LogException(Collection.LoginUser, ex, "Tabular SQL - User filters");
          throw;
        }
      }
      else
      {
        command.CommandText = string.Format("c AS ({0}), q AS (SELECT * FROM c)", Query);

      }


    }

    private static void GetTabularSql(LoginUser loginUser, SqlCommand command, TabularReport tabularReport, bool inlcudeHiddenFields, bool isSchemaOnly, int? reportID, bool useUserFilter)
    {
      StringBuilder builder = new StringBuilder();
      GetTabluarSelectClause(loginUser, command, builder, tabularReport, inlcudeHiddenFields, isSchemaOnly);
      if (isSchemaOnly)
      {
        command.CommandText = builder.ToString();
      }
      else
      {

        string primaryTableKeyFieldName = null;
        if (tabularReport.Subcategory == 70)
        {
            primaryTableKeyFieldName = "UserTicketsView.TicketID";
        }

        GetWhereClause(loginUser, command, builder, tabularReport.Filters, primaryTableKeyFieldName);
        if (useUserFilter && reportID != null)
        {
          Report report = Reports.GetReport(loginUser, (int)reportID, loginUser.UserID);
          if (report != null && report.Row["Settings"] != DBNull.Value)
          {
            try
            {
              UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)report.Row["Settings"]);

              if (userFilters != null)
              {
                GetWhereClause(loginUser, command, builder, userFilters.Filters);
              }
            }
            catch (Exception ex)
            {
              ExceptionLogs.LogException(loginUser, ex, "Tabular SQL - User filters");
            }
          }
        }

        command.CommandText = builder.ToString();
      }
    }

    private static void GetTabluarSelectClause(LoginUser loginUser, SqlCommand command, StringBuilder builder, TabularReport tabularReport, bool includeHiddenFields, bool isSchemaOnly)
    {
      ReportSubcategory sub = ReportSubcategories.GetReportSubcategory(loginUser, tabularReport.Subcategory);

      ReportTables tables = new ReportTables(loginUser);
      tables.LoadAll();

      ReportTableFields tableFields = new ReportTableFields(loginUser);
      tableFields.LoadAll();
      TimeSpan offset = loginUser.Offset;
      TicketTypes ticketTypes = new TicketTypes(loginUser);
      ticketTypes.LoadByOrganizationID(loginUser.OrganizationID);
        
      foreach (ReportSelectedField field in tabularReport.Fields)
      {

        if (field.IsCustom)
        {
          CustomField customField = (CustomField)CustomFields.GetCustomField(loginUser, field.FieldID);
          if (customField == null) continue;
          string fieldName = DataUtils.GetReportPrimaryKeyFieldName(customField.RefType);
          if (fieldName != "")
          {
            //handle the ticket views custom fields
            if (tabularReport.Subcategory == 70)
            {
                fieldName = "UserTicketsView.TicketID";
            }


            fieldName = DataUtils.GetCustomFieldColumn(loginUser, customField, fieldName, true, false);

            if (customField.FieldType == CustomFieldType.DateTime)
            {
              fieldName = string.Format("CAST(SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}') AS DATETIME)",
              fieldName,
              offset < TimeSpan.Zero ? "-" : "+",
              Math.Abs(offset.Hours),
              Math.Abs(offset.Minutes));
            }

            builder.Append(builder.Length < 1 ? "SELECT " : ", ");

            if (customField.AuxID > 0 && customField.RefType == ReferenceType.Tickets)
            {
              TicketType ticketType = ticketTypes.FindByTicketTypeID(customField.AuxID);
              if (ticketType != null && ticketType.OrganizationID == customField.OrganizationID)
              {
                builder.Append(string.Format("{0} AS [{1} ({2})]", fieldName, customField.Name, ticketType.Name)); 
              }
              else
              {
                builder.Append(string.Format("{0} AS [{1}]", fieldName, customField.Name));
              }
            }
            else
            {
              builder.Append(string.Format("{0} AS [{1}]", fieldName, customField.Name));
            }
          }

        }
        else
        {
          ReportTableField tableField = tableFields.FindByReportTableFieldID(field.FieldID);

          ReportTable table = tables.FindByReportTableID(tableField.ReportTableID);
          string fieldName = table.TableName + "." + tableField.FieldName;
			 if (tableField.DataType.Trim().ToLower() == "text" || tableField.DataType.Trim().ToLower() == "varchar")
            fieldName = "dbo.StripHTML(" + fieldName + ")";
          if (tableField.DataType.Trim().ToLower() == "datetime")
          {
            fieldName = string.Format("CAST(SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}') AS DATETIME)",
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
        }

        if (sub.ReportTableID != null)
        {
            hiddenTable = tables.FindByReportTableID((int)sub.ReportTableID);
            if (!string.IsNullOrWhiteSpace(hiddenTable.LookupKeyFieldName))
                builder.Append(string.Format(", {1}.{0} AS [hidden{0}]", hiddenTable.LookupKeyFieldName, hiddenTable.TableName));
        }

        if (tabularReport.Subcategory == 70)
        {
            string dueDateField = hiddenTable.TableName + ".DueDate";
                dueDateField = string.Format("CAST(SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}') AS DATETIME)",
                dueDateField,
                offset < TimeSpan.Zero ? "-" : "+",
                Math.Abs(offset.Hours),
                Math.Abs(offset.Minutes));
            builder.Append(string.Format(", {0} AS [hiddenDueDate]", dueDateField));

            string dateModifiedField = hiddenTable.TableName + ".DateModified";
            dateModifiedField = string.Format("CAST(SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}') AS DATETIME)",
            dateModifiedField,
            offset < TimeSpan.Zero ? "-" : "+",
            Math.Abs(offset.Hours),
            Math.Abs(offset.Minutes));
            builder.Append(string.Format(", {0} AS [hiddenDateModified]", dateModifiedField));

            builder.Append(string.Format(", {1}.{0} AS [hidden{0}]", "SlaWarningTime", hiddenTable.TableName));
            builder.Append(string.Format(", {1}.{0} AS [hidden{0}]", "SlaViolationTime", hiddenTable.TableName));
            builder.Append(string.Format(", {1}.{0} AS [hidden{0}]", "IsRead", hiddenTable.TableName));
            builder.Append(string.Format(", {1}.{0} AS [hidden{0}]", "IsClosed", hiddenTable.TableName));
            builder.Append(string.Format(", {1}.{0} AS [hidden{0}]", "TicketTypeID", hiddenTable.TableName));
            builder.Append(string.Format(", {1}.{0} AS [hidden{0}]", "UserID", hiddenTable.TableName));
            builder.Append(string.Format(", {1}.{0} AS [hidden{0}]", "SeverityPosition", hiddenTable.TableName));
            builder.Append(string.Format(", {1}.{0} AS [hidden{0}]", "StatusPosition", hiddenTable.TableName));
        }

      }
      builder.Append(" " + sub.BaseQuery);

      ReportTable mainTable = tables.FindByReportTableID(sub.ReportCategoryTableID);
      builder.Append(" WHERE (" + mainTable.TableName + "." + mainTable.OrganizationIDFieldName + " = @OrganizationID)");
      if (tabularReport.Subcategory == 70)
      {
          builder.Append(" AND (" + mainTable.TableName + ".ViewerID = @UserID)");
      }

      UseTicketRights(loginUser, (int)tabularReport.Subcategory, tables, command, builder);

      if (isSchemaOnly) builder.Append(" AND (0=1)");
    }

    private static void UseTicketRights(LoginUser loginUser, int subCAtID,  ReportTables tables, SqlCommand command, StringBuilder builder)
    {
        ReportSubcategory subCat = ReportSubcategories.GetReportSubcategory(loginUser, subCAtID);

        if (subCat != null)
        {
            ReportTable catTable = tables.FindByReportTableID((int)subCat.ReportCategoryTableID);

            if(catTable.UseTicketRights)
            {
                GetUserRightsClause(loginUser, command, builder, catTable.TableName);
                return;
            }
            else if (catTable.ReportTableID == 6)
            {
                GetCustomerUserRightsClause(loginUser, command, builder, catTable.TableName);
            }

            if (subCat.ReportTableID != null)
            {
                ReportTable reportTable = tables.FindByReportTableID((int)subCat.ReportTableID);

                if (reportTable.UseTicketRights)
                {
                    GetUserRightsClause(loginUser, command, builder, reportTable.TableName);
                    return;
                }
                else if (reportTable.ReportTableID == 6)
                {
                    GetCustomerUserRightsClause(loginUser, command, builder, reportTable.TableName);
                }
            }
        }
    }

    private static void GetUserRightsClause(LoginUser loginUser, SqlCommand command, StringBuilder builder, string mainTableName)
    {
        string rightsClause = "";

        User user = Users.GetUser(loginUser, loginUser.UserID);
        switch (user.TicketRights)
        {
            case TicketRightType.All:
                break;
            case TicketRightType.Assigned:
                builder.Append(string.Format("AND ({0}.TicketID in ( SELECT t.TicketID FROM tickets t WHERE (t.UserID = @UserID OR t.IsKnowledgeBase=1)))", mainTableName));
                break;
            case TicketRightType.Groups:
                rightsClause = @"AND ({0}.TicketID in ( 
		                                SELECT t.TicketID 
		                                FROM tickets t          
		                                WHERE ({1} 
                                        (t.UserID = @UserID) 
		                                OR (t.IsKnowledgeBase = 1) 
		                                OR (t.UserID IS NULL AND t.GroupID IS NULL))
	                                ))";

                Groups groups = new Groups(loginUser);
                groups.LoadByUserID(loginUser.UserID);

                string groupString = groups.Count < 1 ? "" : string.Format("(t.GroupID IN ({0})) OR ", DataUtils.IntArrayToCommaString(groups.Select(gr => gr.GroupID).ToArray()));

                builder.Append(string.Format(rightsClause, mainTableName, groupString));
                break;
            case TicketRightType.Customers:
                rightsClause = @" AND ({0}.TicketID in (
                                    SELECT ot.TicketID FROM OrganizationTickets ot
                                    INNER JOIN UserRightsOrganizations uro ON ot.OrganizationID = uro.OrganizationID 
                                    WHERE uro.UserID = @UserID) OR
                                    {0}.UserID = @UserID OR
                                    {0}.IsKnowledgeBase = 1)";
                builder.Append(string.Format(rightsClause, mainTableName));
                break;
            default:
                break;
        }

        Organizations organization = new Organizations(loginUser);
        organization.LoadByOrganizationID(loginUser.OrganizationID);
        if (organization.Count > 0 && organization[0].UseProductFamilies)
        {
            switch ((ProductFamiliesRightType)user.ProductFamiliesRights)
            {
                case ProductFamiliesRightType.AllFamilies:
                    break;
                case ProductFamiliesRightType.SomeFamilies:
                    rightsClause = @" AND (
                    {0}.TicketID IN 
                    (
                        SELECT 
                            t.TicketID 
                        FROM 
                            Tickets t
                            JOIN Products p
                                ON t.ProductID = p.ProductID
                            JOIN UserRightsProductFamilies urpf
                                ON p.ProductFamilyID = urpf.ProductFamilyID 
                        WHERE 
                            urpf.UserID = @UserID
                    ) 
                    OR {0}.UserID = @UserID 
                )";
                    builder.Append(string.Format(rightsClause, mainTableName));
                    break;
                default:
                    break;
            }
        }
    }

    private static void GetCustomerUserRightsClause(LoginUser loginUser, SqlCommand command, StringBuilder builder, string mainTableName)
    {
        string rightsClause = "";

        User user = Users.GetUser(loginUser, loginUser.UserID);

        if (user.TicketRights == TicketRightType.Customers)
        {
            rightsClause = @"AND (OrganizationsView.OrganizationID in (
                                SELECT uro.OrganizationID 
                                FROM UserRightsOrganizations uro                                       
                                WHERE uro.UserID = @UserID))";
            builder.Append(string.Format(rightsClause, mainTableName));
        }
    }

    private static void GetWhereClause(LoginUser loginUser, SqlCommand command, StringBuilder builder, ReportFilter[] filters, string primaryTableKeyName = null)
    {
        if (filters != null) WriteFilters(loginUser, command, builder, filters, null, primaryTableKeyName);
    }

    private static void WriteFilters(LoginUser loginUser, SqlCommand command, StringBuilder builder, ReportFilter[] filters, ReportFilter parentFilter, string primaryTableKeyName = null)
    {
      foreach (ReportFilter filter in filters)
      {
        if (filter.Conditions.Length < 1) continue;

        builder.Append(string.Format(" {0} (", parentFilter == null ? "AND" : parentFilter.Conjunction.ToUpper()));
        WriteFilter(loginUser, command, builder, filter, primaryTableKeyName);
        WriteFilters(loginUser, command, builder, filter.Filters, filter, primaryTableKeyName);
        builder.Append(")");
      }
    }

    private static void WriteFilter(LoginUser loginUser, SqlCommand command, StringBuilder builder, ReportFilter filter, string primaryTableKeyName = null)
    {
      for (int i = 0; i < filter.Conditions.Length; i++)
      {
        ReportFilterCondition condition = filter.Conditions[i];
        if (i > 0) builder.Append(string.Format(" {0} ", filter.Conjunction.ToUpper()));
        builder.Append("(");
        WriteFilterCondition(loginUser, command, builder, condition, primaryTableKeyName);
        builder.Append(")");
      }
    }

    private static void WriteFilterCondition(LoginUser loginUser, SqlCommand command, StringBuilder builder, ReportFilterCondition condition, string primaryTableKeyName = null)
    {
      string fieldName = "";
      string dataType;
      bool isUnicode = false;
      if (condition.FieldID < 0)
      {
        dataType = condition.DataType;
        fieldName = string.Format("[{0}]", condition.FieldName);
      }
      else
      {

        if (condition.IsCustom)
        {
          CustomField customField = TeamSupport.Data.CustomFields.GetCustomField(loginUser, condition.FieldID);
          if (customField == null) return;
          
          //assign the primary key to a variable and update it with the passed in value if present
          string PrimaryTableKeyName = DataUtils.GetReportPrimaryKeyFieldName(customField.RefType);
          if (primaryTableKeyName != null) PrimaryTableKeyName = primaryTableKeyName;

          fieldName = DataUtils.GetCustomFieldColumn(loginUser, customField, PrimaryTableKeyName, true, false);
          switch (customField.FieldType)
          {
            case CustomFieldType.DateTime:
              dataType = "datetime";
              break;
						case CustomFieldType.Date:
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
          ReportTableField field = ReportTableFields.GetReportTableField(loginUser, condition.FieldID);
          ReportTable reportTable = ReportTables.GetReportTable(loginUser, field.ReportTableID);
          fieldName = reportTable.TableName + ".[" + field.FieldName + "]";
          dataType = field.DataType;
        }
      }

      string paramName= string.Format("Param{0:D5}", command.Parameters.Count + 1);

      if (condition.Value1 == "The Report Viewer")
      {
        condition.Value1 = loginUser.GetUserFullName();
      }

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
            case "LESS THAN": builder.Append(string.Format("{0} < @{1}", fieldName, paramName));
              command.Parameters.AddWithValue(paramName, float.Parse(condition.Value1));
              break;
            case "GREATER THAN": builder.Append(string.Format("{0} > @{1}", fieldName, paramName));
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
            case "LESS THAN": builder.Append(string.Format("{0} < @{1}", fieldName, paramName));
              command.Parameters.AddWithValue(paramName, int.Parse(condition.Value1));
              break;
            case "GREATER THAN": builder.Append(string.Format("{0} > @{1}", fieldName, paramName));
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
          TimeSpan offset = loginUser.Offset;
          string datetimeSql = string.Format("CAST(SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}') AS DATETIME)",
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
            case "IS EMPTY":
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
              builder.Append(string.Format("(DATEPART(quarter, {0}) = {1:D} AND DATEPART(year, {0}) = {2:D})", dateSql, DataUtils.GetQuarter(DateTime.Now), DateTime.Now.Year));
              break;
            case "PREVIOUS QUARTER":
              int prevQuarter = DataUtils.GetQuarter(DateTime.Now) -1;
              int prevYear = DateTime.Now.Year;
              if (prevQuarter < 1) 
              {
                prevQuarter = 4;
                prevYear--;
              }
              builder.Append(string.Format("(DATEPART(quarter, {0}) = {1:D} AND DATEPART(year, {0}) = {2:D})", dateSql, prevQuarter, prevYear));
              break;
            case "NEXT QUARTER": 
              int nextQuarter = DataUtils.GetQuarter(DateTime.Now) + 1;
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
              command.Parameters.Add(paramName, SqlDbType.Date).Value = DataUtils.DateToLocal(loginUser, DateTime.UtcNow).Date;
              break;
            case "YESTERDAY": 
              builder.Append(string.Format("{0} = @{1}", dateSql, paramName));
              command.Parameters.Add(paramName, SqlDbType.Date).Value = DataUtils.DateToLocal(loginUser, DateTime.UtcNow).AddDays(-1).Date;
              break;
            case "TOMORROW": 
              builder.Append(string.Format("{0} = @{1}", dateSql, paramName));
              command.Parameters.Add(paramName, SqlDbType.Date).Value = DataUtils.DateToLocal(loginUser, DateTime.UtcNow).AddDays(1).Date;
              break;
            case "SPECIFIC DAY":
              builder.Append(string.Format("DATEPART(weekday, {0}) = {1:D}", dateSql, int.Parse(condition.Value1)));
              break;
            case "PREVIOUS # DAYS":
              string paramName2 = paramName + "x2";
              builder.Append(string.Format("({0} >= @{1} AND {0} < @{2})", dateSql, paramName, paramName2));
              command.Parameters.Add(paramName, SqlDbType.Date).Value = DataUtils.DateToLocal(loginUser, DateTime.UtcNow).AddDays(-1*int.Parse(condition.Value1)).Date;
              command.Parameters.Add(paramName2, SqlDbType.Date).Value = DataUtils.DateToLocal(loginUser, DateTime.UtcNow).Date;
              break;
            case "LAST # DAYS": 
              string paramName2a = paramName + "x2";
              builder.Append(string.Format("({0} >= @{1} AND {0} <= @{2})", dateSql, paramName, paramName2a));
              command.Parameters.Add(paramName, SqlDbType.Date).Value = DataUtils.DateToLocal(loginUser, DateTime.UtcNow).AddDays(-1*int.Parse(condition.Value1)).Date;
              command.Parameters.Add(paramName2a, SqlDbType.Date).Value = DataUtils.DateToLocal(loginUser, DateTime.UtcNow).Date;
              break;
            case "NEXT # DAYS": 
              string paramName2b = paramName + "x2";
              builder.Append(string.Format("({0} >= @{1} AND {0} <= @{2})", dateSql, paramName2b, paramName));
              command.Parameters.Add(paramName, SqlDbType.Date).Value = DataUtils.DateToLocal(loginUser, DateTime.UtcNow).AddDays(int.Parse(condition.Value1)).Date;
              command.Parameters.Add(paramName2b, SqlDbType.Date).Value = DataUtils.DateToLocal(loginUser, DateTime.UtcNow).Date;
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
          if (condition.Value1 != null)
          {
            command.Parameters.Add(paramName, SqlDbType.VarChar).Value = condition.Value1;
          }
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
        return (date.Month - 1) / 3 + 1;
    }

    private static void GetSummarySql(LoginUser loginUser, SqlCommand command, SummaryReport summaryReport, bool isSchemaOnly, int? reportID, bool useUserFilter, bool useDefaultOrderBy)
    {
      StringBuilder builder = new StringBuilder();
      ReportSubcategory sub = ReportSubcategories.GetReportSubcategory(loginUser, summaryReport.Subcategory);
      ReportTables tables = new ReportTables(loginUser);
      tables.LoadAll();
      List<DescriptiveClauseItem> descFields = GetSummaryDescFields(loginUser, summaryReport);
      List<CalculatedClauseItem> calcFields = GetSummaryCalcFields(loginUser, summaryReport);

      builder.Append("WITH x AS (");
      bool flag = true;
      foreach (DescriptiveClauseItem descField in descFields)
      {
        if (flag)
          builder.Append(string.Format(" SELECT {0} AS [{1}]", descField.Field, descField.Alias));
        else
          builder.Append(string.Format(", {0} AS [{1}]", descField.Field, descField.Alias));
        flag = false;
      }

      foreach (CalculatedClauseItem calcField in calcFields)
      {
        builder.Append(string.Format(", {0} AS [{1}]", calcField.Field, calcField.Alias));
      }

      // from + where clause
      builder.Append(" " + sub.BaseQuery);
      ReportTable mainTable = tables.FindByReportTableID(sub.ReportCategoryTableID);
      builder.Append(" WHERE (" + mainTable.TableName + "." + mainTable.OrganizationIDFieldName + " = @OrganizationID)");
        //add user rights where needed
      UseTicketRights(loginUser, (int)summaryReport.Subcategory, tables, command, builder);
      if (isSchemaOnly) builder.Append(" AND (0=1)");

      // filters
      if (!isSchemaOnly)
      {
        GetWhereClause(loginUser, command, builder, summaryReport.Filters);
        if (useUserFilter == true && reportID != null)
        {
          Report report = Reports.GetReport(loginUser, (int)reportID, loginUser.UserID);
          if (report != null && report.Row["Settings"] != DBNull.Value)
          {
            try
            {
              UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)report.Row["Settings"]);
              if (userFilters != null)
              {
                GetWhereClause(loginUser, command, builder, userFilters.Filters);
              }
            }
            catch (Exception ex)
            {
              ExceptionLogs.LogException(loginUser, ex, "Summary SQL - User filters");
            }
          }
        }
      }
      flag = true;

      builder.Append(")"); // end with

      flag = true;
      foreach (DescriptiveClauseItem descField in descFields)
      {
        if (flag)
          builder.Append(string.Format(" SELECT [{0}]", descField.Alias));
        else
          builder.Append(string.Format(", [{0}]", descField.Alias));
        flag = false;
      }

      foreach (CalculatedClauseItem calcField in calcFields)
      {
        builder.Append(string.Format(", {0} AS [{1}]", calcField.AggField, calcField.Alias));
      }

      builder.Append(" FROM x ");

      // group by
      flag = true;
      foreach (DescriptiveClauseItem descField in descFields)
      {
        if (flag)
          builder.Append(string.Format(" GROUP BY [{0}]", descField.Alias));
        else
          builder.Append(string.Format(", [{0}]", descField.Alias));

        flag = false;
      }

      // having
      flag = true;
      foreach (CalculatedClauseItem calcField in calcFields)
      {
        if (calcField.Comparator == null) continue;
        if (flag)
          builder.Append(string.Format(" HAVING {0}", calcField.Comparator));
        else
          builder.Append(string.Format(" AND {0}", calcField.Comparator));
        flag = false;
      }

      if (useDefaultOrderBy)
      {
        // order by
       /* flag = true;
        foreach (DescriptiveClauseItem descField in descFields)
        {
          if (flag)
            builder.Append(string.Format(" ORDER BY [{0}]", descField.Alias));
          else
            builder.Append(string.Format(", [{0}]", descField.Alias));

          flag = false;
        }*/
        
        // order by
        for (int i = descFields.Count - 1; i > -1; i--)
        {
          if (i == descFields.Count - 1)
            builder.Append(string.Format(" ORDER BY [{0}]", descFields[i].Alias));
          else
            builder.Append(string.Format(", [{0}]", descFields[i].Alias));
        }
      }
      command.CommandText = builder.ToString();
    }

    public static void GetSummaryCommand(LoginUser loginUser, SqlCommand command, SummaryReport summaryReport, bool isSchemaOnly, bool useUserFilter, bool useDefaultOrderBy)
    {
      command.CommandType = CommandType.Text;
      GetSummarySql(loginUser, command, summaryReport, isSchemaOnly, null, useUserFilter, useDefaultOrderBy);
      AddCommandParameters(command, loginUser);
    }

    private static List<DescriptiveClauseItem> GetSummaryDescFields(LoginUser loginUser, SummaryReport summaryReport)
    {
      List<DescriptiveClauseItem> result = new List<DescriptiveClauseItem>();
      ReportSubcategory sub = ReportSubcategories.GetReportSubcategory(loginUser, summaryReport.Subcategory);

      ReportTables tables = new ReportTables(loginUser);
      tables.LoadAll();

      ReportTableFields tableFields = new ReportTableFields(loginUser);
      tableFields.LoadAll();
      TimeSpan offset = loginUser.Offset;
      TicketTypes ticketTypes = new TicketTypes(loginUser);
      ticketTypes.LoadByOrganizationID(loginUser.OrganizationID);

      foreach (ReportSummaryDescriptiveField field in summaryReport.Fields.Descriptive)
      {
        if (field.Field.IsCustom)
        {
          CustomField customField = (CustomField)CustomFields.GetCustomField(loginUser, field.Field.FieldID);
          if (customField == null) continue;
          string fieldName = DataUtils.GetReportPrimaryKeyFieldName(customField.RefType);
          if (fieldName != "")
          {
            fieldName = DataUtils.GetCustomFieldColumn(loginUser, customField, fieldName, true, false);

            if (customField.FieldType == CustomFieldType.DateTime)
            {
              fieldName = string.Format("CAST(SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}') AS DATETIME)",
              fieldName,
              offset < TimeSpan.Zero ? "-" : "+",
              Math.Abs(offset.Hours),
              Math.Abs(offset.Minutes));

              fieldName = GetDateGroupField(fieldName, field.Value1);
            }
            string alias = customField.Name;

            if (customField.AuxID > 0 && customField.RefType == ReferenceType.Tickets)
            {
              TicketType ticketType = ticketTypes.FindByTicketTypeID(customField.AuxID);
              if (ticketType != null && ticketType.OrganizationID == customField.OrganizationID)
              {
                alias =  string.Format("{1} ({2})", fieldName, customField.Name, ticketType.Name); 
              }
            }
            result.Add(new DescriptiveClauseItem(fieldName, alias));
          }
        }
        else
        {
          ReportTableField tableField = tableFields.FindByReportTableFieldID(field.Field.FieldID);
          ReportTable table = tables.FindByReportTableID(tableField.ReportTableID);
          string fieldName = table.TableName + "." + tableField.FieldName;
          if (tableField.DataType.Trim().ToLower() == "datetime")
          {
            fieldName = string.Format("CAST(SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}') AS DATETIME)",
              fieldName,
              offset < TimeSpan.Zero ? "-" : "+",
              Math.Abs(offset.Hours),
              Math.Abs(offset.Minutes));
            fieldName = GetDateGroupField(fieldName, field.Value1);
          }

          result.Add(new DescriptiveClauseItem(fieldName, tableField.Alias));
        }
      }
      return result;
    }

    private static List<CalculatedClauseItem> GetSummaryCalcFields(LoginUser loginUser, SummaryReport summaryReport)
    {
      List<CalculatedClauseItem> result = new List<CalculatedClauseItem>();
      ReportSubcategory sub = ReportSubcategories.GetReportSubcategory(loginUser, summaryReport.Subcategory);

      ReportTables tables = new ReportTables(loginUser);
      tables.LoadAll();

      ReportTableFields tableFields = new ReportTableFields(loginUser);
      tableFields.LoadAll(false);
      TimeSpan offset = loginUser.Offset;

      foreach (ReportSummaryCalculatedField field in summaryReport.Fields.Calculated)
      {
        StringBuilder builder = new StringBuilder();
        if (field.Field.IsCustom)
        {
          CustomField customField = (CustomField)CustomFields.GetCustomField(loginUser, field.Field.FieldID);
          if (customField == null) continue;
          string fieldName = DataUtils.GetReportPrimaryKeyFieldName(customField.RefType);
          if (fieldName != "")
          {
            fieldName = DataUtils.GetCustomFieldColumn(loginUser, customField, fieldName, true, false);


            if (customField.FieldType == CustomFieldType.DateTime)
            {
              fieldName = string.Format("CAST(SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}') AS DATETIME)",
              fieldName,
              offset < TimeSpan.Zero ? "-" : "+",
              Math.Abs(offset.Hours),
              Math.Abs(offset.Minutes));
            }

            result.Add(GetCalcItem(fieldName, customField.Name, field));
          }

        }
        else
        {
          ReportTableField tableField = tableFields.FindByReportTableFieldID(field.Field.FieldID);
          ReportTable table = tables.FindByReportTableID(tableField.ReportTableID);
          string fieldName = table.TableName + "." + tableField.FieldName;
          if (tableField.DataType.Trim().ToLower() == "datetime")
          {
            fieldName = string.Format("CAST(SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}') AS DATETIME)",
              fieldName,
              offset < TimeSpan.Zero ? "-" : "+",
              Math.Abs(offset.Hours),
              Math.Abs(offset.Minutes));
          }
            result.Add(GetCalcItem(fieldName, tableField.Alias, field));
        }
      }
      return result;
    }

    private static CalculatedClauseItem GetCalcItem(string field, string alias, ReportSummaryCalculatedField calc)
    {
      CalculatedClauseItem result = new CalculatedClauseItem();

      result.Field = field;
      switch (calc.Aggregate.ToLower())
      {
        case "sum":
          result.Alias = alias + " Sum";
          result.AggField = string.Format("SUM([{0}])", result.Alias);
          break;
        case "max":
          result.Alias = alias + " Max";
          result.AggField = string.Format("MAX([{0}])", result.Alias);
          break;
        case "min":
          result.Alias = alias + " Min";
          result.AggField = string.Format("MIN([{0}])", result.Alias);
          break;
        case "avg":
          result.Alias = alias + " Average";
          result.AggField = string.Format("AVG([{0}])", result.Alias);
          break;
        case "stdev": 
          result.Alias = alias + " Std Dev";
          result.AggField = string.Format("STDEV([{0}])", result.Alias);
          break;
        case "var": 
          result.Alias = alias + " Variance";
          result.AggField = string.Format("VAR([{0}])", result.Alias);
          break;
        case "count": 
          result.Alias = alias + " Count";
          result.AggField = string.Format("COUNT([{0}])", result.Alias);
          break;
        case "countdistinct": 
          result.Alias = alias + " Distinct Count";
          result.AggField = string.Format("COUNT(DISTINCT([{0}]))", result.Alias); 
          break;
        default:
          break;
      }

      if (calc.Comparator.ToLower() == "none")
      {
        result.Comparator = null;
        return result;
      }

      // verify values are numbers for sql injection
      float.Parse(calc.Value1);
      

      switch (calc.Comparator.ToLower())
      {
        case "lt": result.Comparator = string.Format("({0} < {1})", result.AggField, calc.Value1); break;
        case "gt": result.Comparator = string.Format("({0} > {1})", result.AggField, calc.Value1); break;
        case "bet": result.Comparator = string.Format("({0} BETWEEN {1} AND {2})", result.AggField, calc.Value1, calc.Value2); float.Parse(calc.Value2); break;
        case "eq": result.Comparator = string.Format("({0} = {1})", result.AggField, calc.Value1); break;
        default:
          break;
      }


      return result;
    }

    private static string GetDateGroupField(string fieldName, string option)
    {
      switch (option)
      {
        case "year": return string.Format("DATEPART(YEAR, {0})", fieldName);
        case "qtryear": return string.Format("CAST(DATEPART(YEAR, {0}) AS VARCHAR) + '-' + CAST(DATEPART(QUARTER, {0}) AS VARCHAR)", fieldName);
        case "monthyear": return string.Format("CONVERT(CHAR(7), {0}, 121)", fieldName);
        case "weekyear": return string.Format("CAST(DATEPART(YEAR, {0}) AS VARCHAR) + '-' + CAST(DATEPART(WEEK, {0}) AS VARCHAR)", fieldName);
        case "date": return string.Format("CAST({0} AS DATE)", fieldName);
        case "qtr": return string.Format("DATEPART(QUARTER, {0})", fieldName);
        case "month": return string.Format("DATEPART(MONTH, {0}) ", fieldName);
        case "week": return string.Format("DATEPART(WEEK, {0}) ", fieldName);
        case "dayweek": return string.Format("CAST(DATEPART(WEEKDAY, {0}) AS VARCHAR)", fieldName);
        case "daymonth": return string.Format("DATEPART(DAY, {0})", fieldName);
        case "hourday": return string.Format("DATEPART(HOUR, {0})", fieldName);
        default:
          break;
      }

      return "";
    }

    public ReportColumn[] GetSqlColumns()
    {
      DataTable table = new DataTable();

      using (SqlCommand command = new SqlCommand())
      {
        GetCommand(command, true, true, false);
        BaseCollection.FixCommandParameters(command);

        using (SqlConnection connection = new SqlConnection(Collection.LoginUser.ConnectionString))
        {
          connection.Open();
          command.Connection = connection;
          using (SqlDataAdapter adapter = new SqlDataAdapter(command))
          {
            try
            {
              adapter.FillSchema(table, SchemaType.Source);
              adapter.Fill(table);
            }
            catch (Exception ex)
            {
              ExceptionLogs.LogException(Collection.LoginUser, ex, "GetSqlColumns", command.CommandText);
              Report report = Reports.GetReport(Collection.LoginUser, ReportID);
              report.LastSqlExecuted = DataUtils.GetCommandTextSql(command);
              report.Collection.Save();
              throw;
            }
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
        if (column.DataType == typeof(System.DateTime) || column.DataType == typeof(System.DateTimeOffset)) col.DataType = "datetime";
        else if (column.DataType == typeof(System.Boolean)) col.DataType = "bit";
        else if (column.DataType == typeof(System.Int32)) col.DataType = "int";
        else if (column.DataType == typeof(System.Double)) col.DataType = "float";
        else col.DataType = "text";
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
          col.IsEmail = false;
          col.IsLink = false;
          col.IsOpenable = false;
          col.OpenField = "";
          col.IsCustomField = true;
          col.FieldID = customField.CustomFieldID;
          col.Name = customField.Name;

          TicketTypes ticketTypes = new TicketTypes(Collection.LoginUser);
          ticketTypes.LoadByOrganizationID(Collection.LoginUser.OrganizationID);

          if (customField.AuxID > 0 && customField.RefType == ReferenceType.Tickets)
          {
            TicketType ticketType = ticketTypes.FindByTicketTypeID(customField.AuxID);
            if (ticketType != null && ticketType.OrganizationID == customField.OrganizationID)
            {
              col.Name = string.Format("{0} ({1})", customField.Name, ticketType.Name); 
            }
          }


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
      report.IsPrivate = this.IsPrivate;
      reports.Save();

      return report.ReportID;
    }

    public void MigrateToNewReport()
    {
      if (ReportDef == null || ReportDefType < 0)
      {
        if (!string.IsNullOrWhiteSpace(Query))
        {
          ReportDefType = Data.ReportType.Custom;
          /*
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
            */
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

    public void UpdateFavoritesToNew()
    {
      SqlCommand command = new SqlCommand();
      DataTable userFavorites = new DataTable();
      BaseCollection.FixCommandParameters(command);

      command.CommandText = "select UserID, SettingValue from UserSettings where SettingKey = 'FavoriteReport'";
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();
        command.Connection = connection;
        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
        {
          adapter.Fill(userFavorites);
        }
        connection.Close();
      }

      foreach (DataRow row in userFavorites.Rows)
      {
        try
        {

          int userID = (int)row[0];
          string favstring = row[1].ToString();
          string[] favs = favstring.Split(',');

          foreach (string fav in favs)
          {
            try
            {
              int reportID = int.Parse(fav);
              SetIsFavorite(LoginUser, userID, reportID, true);

            }
            catch (Exception)
            {

            }
          }
        }
        catch (Exception)
        {

        }

      }
    
    
    }
    // end old stuff

    public static GridResult GetReportData(LoginUser loginUser, int reportID, int from, int to, string sortField, bool isDesc, bool useUserFilter)
    {
      Report report = Reports.GetReport(loginUser, reportID, loginUser.UserID);
      GridResult result;
      try
      {
        if (report.ReportDefType == ReportType.Summary || report.ReportDefType == ReportType.Chart)
        {
          result = GetReportDataAll(loginUser, report, sortField, isDesc, useUserFilter);
        }
        else
        {
          result = GetReportDataPage(loginUser, report, from, to, sortField, isDesc, useUserFilter);
        }
      }
      catch (Exception)
      {
        // try without the sort
        try
        {
          if (report.ReportDefType == ReportType.Summary || report.ReportDefType == ReportType.Chart)
          {
            result = GetReportDataAll(loginUser, report, null, isDesc, useUserFilter);
          }
          else
          {
            result = GetReportDataPage(loginUser, report, from, to, null, isDesc, useUserFilter);
          }
        }
        catch (Exception)
        {
          // try without the user filters
          if (report.ReportDefType == ReportType.Summary || report.ReportDefType == ReportType.Chart)
          {
            result = GetReportDataAll(loginUser, report, null, isDesc, false);
          }
          else
          {
            result = GetReportDataPage(loginUser, report, from, to, null, isDesc, false);
          }

          UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)report.Row["Settings"]);
          userFilters.Filters = null;
          report.Row["Settings"] = JsonConvert.SerializeObject(userFilters);
          report.Collection.Save();
        }
      }
      return result;

    }

    public static DataTable GetReportTable(LoginUser loginUser, int reportID, int from, int to, string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
    {
      Report report = Reports.GetReport(loginUser, reportID);
      DataTable result = null;
      try
      {
        if (report.ReportDefType == ReportType.Summary || report.ReportDefType == ReportType.Chart)
        {
          result = GetReportTableAll(loginUser, report, sortField, isDesc, useUserFilter, includeHiddenFields);
        }
        else
        {
          result = GetReportTablePage(loginUser, report, from, to, sortField, isDesc, useUserFilter, includeHiddenFields);
        }
      }
      catch (Exception)
      {
        // try without the sort
        try
        {
          if (report.ReportDefType == ReportType.Summary || report.ReportDefType == ReportType.Chart)
          {
            result = GetReportTableAll(loginUser, report, null, isDesc, useUserFilter, includeHiddenFields);
          }
          else
          {
            result = GetReportTablePage(loginUser, report, from, to, null, isDesc, useUserFilter, includeHiddenFields);
          }
        }
        catch (Exception)
        {
          // try without the user filters
          if (report.ReportDefType == ReportType.Summary || report.ReportDefType == ReportType.Chart)
          {
            result = GetReportTableAll(loginUser, report, null, isDesc, false, includeHiddenFields);
          }
          else
          {
            result = GetReportTablePage(loginUser, report, from, to, null, isDesc, false, includeHiddenFields);
          }

          UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)report.Row["Settings"]);
          userFilters.Filters = null;
          report.Row["Settings"] = JsonConvert.SerializeObject(userFilters);
          report.Collection.Save();
        }
      }
      return result;
    }

    private static GridResult GetReportDataPage(LoginUser loginUser, Report report, int from, int to, string sortField, bool isDesc, bool useUserFilter)
    {
      DataTable table = GetReportTablePage(loginUser, report, from, to, sortField, isDesc, useUserFilter, true);
      GridResult result = new GridResult();
      result.From = from;
      result.To = to;
      result.Total = table.Rows.Count > 0 ? (int)table.Rows[0]["TotalRows"] : 0;
      result.Data = table;
      return result;
    }

    private static DataTable GetReportTablePage(LoginUser loginUser, Report report, int from, int to, string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
    {
      from++;
      to++;

      SqlCommand command = new SqlCommand();
      string totalRows = includeHiddenFields ? ", (SELECT COUNT(RowNum) FROM r) AS 'TotalRows'" : "";
        string query = @"
WITH 
q AS ({0}),
r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY [{1}] {2}) AS 'RowNum' FROM q)
SELECT  *{3} FROM r
WHERE RowNum BETWEEN @From AND @To";

      if (report.ReportDefType == ReportType.Custom)
      {
        query = @"
WITH 
{0}
,r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY [{1}] {2}) AS 'RowNum' FROM q)
SELECT  *{3} FROM r
WHERE RowNum BETWEEN @From AND @To";
      }

      if (string.IsNullOrWhiteSpace(sortField))
      {
          sortField = GetReportColumnNames(loginUser, report.ReportID)[0];
          isDesc = false;
      }

      if (includeHiddenFields && report.ReportSubcategoryID == 70 )
      {
          switch (sortField)
          {
              case "Severity":
                  sortField = "hiddenSeverityPosition";
                  break;
              case "Status":
                  sortField = "hiddenStatusPosition";
                  break;
          }
      }


      command.Parameters.AddWithValue("@From", from);
      command.Parameters.AddWithValue("@To", to);
      report.GetCommand(command, includeHiddenFields, false, useUserFilter);
      command.CommandText = string.Format(query, command.CommandText, sortField, isDesc ? "DESC" : "ASC", totalRows);

      report.LastSqlExecuted = DataUtils.GetCommandTextSql(command);
      report.Collection.Save();
      FixCommandParameters(command);

      DataTable table = new DataTable();
      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);

        command.Connection = connection;
        command.Transaction = transaction;
        try
        {
          using (SqlDataAdapter adapter = new SqlDataAdapter(command))
          {
            adapter.Fill(table);
          }
          transaction.Commit();
          table = DataUtils.DecodeDataTable(table);
        }
        catch (Exception ex)
        {
          transaction.Rollback();
          ExceptionLogs.LogException(loginUser, ex, "Report Data", DataUtils.GetCommandTextSql(command));
          throw;
        }
        connection.Close();
      }

      if (!includeHiddenFields) table.Columns.Remove("RowNum");

      return table;
    }

    private static GridResult GetReportDataAll(LoginUser loginUser, Report report, string sortField, bool isDesc, bool useUserFilter)
    {
      DataTable table = GetReportTableAll(loginUser, report, sortField, isDesc, useUserFilter, true);
  
      GridResult result = new GridResult();
      result.From = 0;
      result.To = table.Rows.Count - 1;
      result.Total = table.Rows.Count;
      result.Data = table;
      return result;
    }

    private static DataTable GetReportTableAll(LoginUser loginUser, Report report, string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
    {
      SqlCommand command = new SqlCommand();

      report.GetCommand(command, includeHiddenFields, false, useUserFilter);
      if (command.CommandText.ToLower().IndexOf(" order by ") < 0)
      {
        if (string.IsNullOrWhiteSpace(sortField))
        {
          sortField = GetReportColumnNames(loginUser, report.ReportID)[0];
          isDesc = false;
        }
        command.CommandText = command.CommandText + " ORDER BY [" + sortField + (isDesc ? "] DESC" : "] ASC");
      }

      report.LastSqlExecuted = DataUtils.GetCommandTextSql(command);
      report.Collection.Save();
      FixCommandParameters(command);

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
      return table;
    }

    public static DataTable GetSummaryData(LoginUser loginUser, SummaryReport summaryReport, bool useDefaultOrderBy, Report report = null)
    {
      SqlCommand command = new SqlCommand();
      Report.GetSummaryCommand(loginUser, command, summaryReport, false, false, useDefaultOrderBy);
      FixCommandParameters(command);
      if (report != null)
      {
        report.LastSqlExecuted = DataUtils.GetCommandTextSql(command);
        report.Collection.Save();
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
            ExceptionLogs.LogException(loginUser, ex, "GetSummaryData");
            throw;
          }
        }
        connection.Close();
      }
      return table;
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
      if (report.ReportDefType == ReportType.Table || report.ReportDefType == ReportType.TicketView) return report.GetTabularColumns();
      return report.GetSqlColumns();
    }

    public void LoadByFolder(int organizationID, int folderID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
SELECT r.* FROM Reports r 
LEFT JOIN ReportOrganizationSettings ros ON ros.ReportID = r.ReportID 
WHERE ((r.OrganizationID = @OrganizationID) OR (r.OrganizationID IS NULL))
AND ros.FolderID = @FolderID";
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
SELECT r.*, u1.FirstName + ' ' + u1.LastName AS Creator, u2.FirstName + ' ' + u2.LastName AS Editor,
(SELECT TOP 1 rv.DateViewed FROM ReportViews rv WHERE rv.ReportID = r.ReportID AND rv.UserID = @UserID ORDER BY rv.DateViewed DESC) AS LastViewed,
ISNULL((SELECT rus.Settings FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), '') AS Settings,
ISNULL((SELECT rus.IsFavorite FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsFavorite,
ISNULL((SELECT rus.IsHidden FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsHidden,
(SELECT ros.FolderID FROM ReportOrganizationSettings ros WHERE ros.ReportID = r.ReportID AND ros.OrganizationID = @OrganizationID) AS Folder
FROM Reports r
LEFT JOIN Users u1 ON u1.UserID = r.CreatorID
LEFT JOIN Users u2 ON u2.UserID = r.EditorID
WHERE ((r.OrganizationID = @OrganizationID) OR (r.OrganizationID IS NULL))
AND r.ReportID NOT IN (SELECT ros.ReportID FROM ReportOrganizationSettings ros WHERE ros.OrganizationID = @OrganizationID AND ros.IsHidden=1) 
ORDER BY r.Name
";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command);
      }
    }

    public void LoadAllTicketViews(int organizationID, int userID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
SELECT r.*, u1.FirstName + ' ' + u1.LastName AS Creator, u2.FirstName + ' ' + u2.LastName AS Editor,
(SELECT TOP 1 rv.DateViewed FROM ReportViews rv WHERE rv.ReportID = r.ReportID AND rv.UserID = @UserID ORDER BY rv.DateViewed DESC) AS LastViewed,
ISNULL((SELECT rus.Settings FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), '') AS Settings,
ISNULL((SELECT rus.IsFavorite FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsFavorite,
ISNULL((SELECT rus.IsHidden FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsHidden,
(SELECT ros.FolderID FROM ReportOrganizationSettings ros WHERE ros.ReportID = r.ReportID AND ros.OrganizationID = @OrganizationID) AS Folder
FROM Reports r
LEFT JOIN Users u1 ON u1.UserID = r.CreatorID
LEFT JOIN Users u2 ON u2.UserID = r.EditorID
WHERE ((r.OrganizationID = @OrganizationID) OR (r.OrganizationID IS NULL)) AND (r.CreatorID = @UserID OR r.IsPrivate = 0) AND ReportDefType = 5
AND r.ReportID NOT IN (SELECT ros.ReportID FROM ReportOrganizationSettings ros WHERE ros.OrganizationID = @OrganizationID AND ros.IsHidden=1) 
ORDER BY r.Name
";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@UserID", userID);
            Fill(command);
        }
    }

    public void LoadAllPrivateTicketViews(int organizationID, int userID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
SELECT r.*, u1.FirstName + ' ' + u1.LastName AS Creator, u2.FirstName + ' ' + u2.LastName AS Editor,
(SELECT TOP 1 rv.DateViewed FROM ReportViews rv WHERE rv.ReportID = r.ReportID AND rv.UserID = @UserID ORDER BY rv.DateViewed DESC) AS LastViewed,
ISNULL((SELECT rus.Settings FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), '') AS Settings,
ISNULL((SELECT rus.IsFavorite FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsFavorite,
ISNULL((SELECT rus.IsHidden FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsHidden,
(SELECT ros.FolderID FROM ReportOrganizationSettings ros WHERE ros.ReportID = r.ReportID AND ros.OrganizationID = @OrganizationID) AS Folder
FROM Reports r
LEFT JOIN Users u1 ON u1.UserID = r.CreatorID
LEFT JOIN Users u2 ON u2.UserID = r.EditorID
WHERE ((r.OrganizationID = @OrganizationID) OR (r.OrganizationID IS NULL)) AND (r.CreatorID = @UserID AND r.IsPrivate = 1) AND ReportDefType = 5
AND r.ReportID NOT IN (SELECT ros.ReportID FROM ReportOrganizationSettings ros WHERE ros.OrganizationID = @OrganizationID AND ros.IsHidden=1) 
ORDER BY r.Name
";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@UserID", userID);
            Fill(command);
        }
    }

    public void LoadAllPublicTicketViews(int organizationID, int userID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
SELECT r.*, u1.FirstName + ' ' + u1.LastName AS Creator, u2.FirstName + ' ' + u2.LastName AS Editor,
(SELECT TOP 1 rv.DateViewed FROM ReportViews rv WHERE rv.ReportID = r.ReportID AND rv.UserID = @UserID ORDER BY rv.DateViewed DESC) AS LastViewed,
ISNULL((SELECT rus.Settings FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), '') AS Settings,
ISNULL((SELECT rus.IsFavorite FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsFavorite,
ISNULL((SELECT rus.IsHidden FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsHidden,
(SELECT ros.FolderID FROM ReportOrganizationSettings ros WHERE ros.ReportID = r.ReportID AND ros.OrganizationID = @OrganizationID) AS Folder
FROM Reports r
LEFT JOIN Users u1 ON u1.UserID = r.CreatorID
LEFT JOIN Users u2 ON u2.UserID = r.EditorID
WHERE ((r.OrganizationID = @OrganizationID) OR (r.OrganizationID IS NULL)) AND (r.IsPrivate = 0) AND ReportDefType = 5
AND r.ReportID NOT IN (SELECT ros.ReportID FROM ReportOrganizationSettings ros WHERE ros.OrganizationID = @OrganizationID AND ros.IsHidden=1) 
ORDER BY r.Name
";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@UserID", userID);
            Fill(command);
        }
    }

    public void LoadCustomReports() 
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT * FROM Reports WHERE Query IS NOT NULL ORDER BY Name";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public void Search(int organizationID, string term, int top = 25)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
SELECT TOP {0:D} r.*
FROM Reports r
WHERE ((r.OrganizationID = @OrganizationID) OR (r.OrganizationID IS NULL))
AND r.ReportID NOT IN (SELECT ros.ReportID FROM ReportOrganizationSettings ros WHERE ros.OrganizationID = @OrganizationID AND ros.IsHidden=1) 
AND r.Name LIKE '%' + @Term + '%'
ORDER BY r.Name
";
        command.CommandText = string.Format(command.CommandText, top);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@Term", term);
        Fill(command);
      }
    }

    public void LoadList(int organizationID, int userID, string[] ids)
    {
      if (ids.Length < 1) return;
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
SELECT r.*, u1.FirstName + ' ' + u1.LastName AS Creator, u2.FirstName + ' ' + u2.LastName AS Editor,
(SELECT TOP 1 rv.DateViewed FROM ReportViews rv WHERE rv.ReportID = r.ReportID AND rv.UserID = @UserID ORDER BY rv.DateViewed DESC) AS LastViewed,
ISNULL((SELECT rus.Settings FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), '') AS Settings,
ISNULL((SELECT rus.IsFavorite FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsFavorite,
ISNULL((SELECT rus.IsHidden FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsHidden,
(SELECT ros.FolderID FROM ReportOrganizationSettings ros WHERE ros.ReportID = r.ReportID AND ros.OrganizationID = @OrganizationID) AS Folder
FROM Reports r
LEFT JOIN Users u1 ON u1.UserID = r.CreatorID
LEFT JOIN Users u2 ON u2.UserID = r.EditorID
WHERE ((r.OrganizationID = @OrganizationID) OR (r.OrganizationID IS NULL))
AND r.ReportID NOT IN (SELECT ros.ReportID FROM ReportOrganizationSettings ros WHERE ros.OrganizationID = @OrganizationID AND ros.IsHidden=1) 
AND r.ReportID IN ({0})
ORDER BY r.Name
";
        command.CommandText = string.Format(command.CommandText, string.Join(",", ids));
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
SELECT r.*, u1.FirstName + ' ' + u1.LastName AS Creator, u2.FirstName + ' ' + u2.LastName AS Editor,
(SELECT TOP 1 rv.DateViewed FROM ReportViews rv WHERE rv.ReportID = r.ReportID AND rv.UserID = @UserID ORDER BY rv.DateViewed DESC) AS LastViewed,
ISNULL((SELECT rus.Settings FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), '') AS Settings,
ISNULL((SELECT rus.IsFavorite FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsFavorite,
ISNULL((SELECT rus.IsHidden FROM ReportUserSettings rus WHERE rus.ReportID = r.ReportID AND rus.UserID = @UserID), 0) AS IsHidden,
(SELECT ros.FolderID FROM ReportOrganizationSettings ros WHERE ros.ReportID = r.ReportID AND ros.OrganizationID = @OrganizationID) AS Folder
FROM Reports r
LEFT JOIN Users u1 ON u1.UserID = r.CreatorID
LEFT JOIN Users u2 ON u2.UserID = r.EditorID
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

    public static void AssignFolder(LoginUser loginUser, int? folderID, int organizationID, int reportID)
    {
      SqlCommand command = new SqlCommand();

      if (folderID == null)
      {
        command.CommandText = @"UPDATE ReportOrganizationSettings SET FolderID=NULL WHERE OrganizationID = @OrganizationID AND ReportID = @ReportID";
      }
      else
      {
        command.CommandText = @"
UPDATE ReportOrganizationSettings SET FolderID=@FolderID WHERE OrganizationID = @OrganizationID AND ReportID = @ReportID
IF @@ROWCOUNT=0
    INSERT INTO ReportOrganizationSettings (OrganizationID, ReportID, FolderID) VALUES (@OrganizationID, @ReportID, @FolderID)
";
        command.Parameters.AddWithValue("FolderID", folderID);
      }
      command.Parameters.AddWithValue("OrganizationID", organizationID);
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

    public static void UpdateReportView(LoginUser loginUser, int reportID)
    {
      ReportView reportView = (new ReportViews(loginUser)).AddNewReportView();
      reportView.UserID = loginUser.UserID;
      reportView.ReportID = reportID;
      reportView.DateViewed = DateTime.UtcNow;
      reportView.Collection.Save();
    }

    public static string BuildChartData(LoginUser loginUser, DataTable table, SummaryReport summaryReport)
    {
      DataResult[] result = new DataResult[table.Columns.Count];

      for (int i = 0; i < table.Columns.Count; i++)
      {
        result[i] = new DataResult();
        result[i].name = table.Columns[i].ColumnName;
        result[i].data = new object[table.Rows.Count];

        for (int j = 0; j < table.Rows.Count; j++)
        {
          object data = table.Rows[j][i];
          result[i].data[j] = data == null || data == DBNull.Value ? null : data;
        }

        if (i < summaryReport.Fields.Descriptive.Length)
        {
          result[i].fieldType = summaryReport.Fields.Descriptive[i].Field.FieldType;
          result[i].format = summaryReport.Fields.Descriptive[i].Value1;
          if (result[i].fieldType == "datetime") FixChartDateNames(loginUser, result[i].data, summaryReport.Fields.Descriptive[i].Value1);
        }


      }

      return JsonConvert.SerializeObject(result);
    }

    public static void FixChartDateNames(LoginUser loginUser, object[] list, string dateType)
    {
      try
      {
        DateTime baseDate = new DateTime(1970, 1, 1);
        for (int i = 0; i < list.Length; i++)
        {
          if (string.IsNullOrWhiteSpace((string)list[i])) continue;
          string item = ((string)list[i]).Trim().ToLower();


          if (dateType == "qtryear" || dateType == "monthyear" || dateType == "weekyear")
          {
            string[] items = item.Split('-');
            if (items.Length == 2)
            {
              string year = items[0];
              string value = items[1];

              switch (dateType)
              {
                case "qtryear": list[i] = string.Format("{1} - Q{0}", value, year); break;
                case "monthyear": list[i] =
                  string.Format("{1} {0}", loginUser.CultureInfo.DateTimeFormat.GetAbbreviatedMonthName(int.Parse(value)), year);
                  break;
                case "weekyear": list[i] = string.Format("{1}-{0}", value, year); break;
                default:
                  break;
              }
            }


          }
          else if (dateType == "qtr")
          {
            list[i] = "Q" + item;
          }
          else if (dateType == "month")
          {
            list[i] = loginUser.CultureInfo.DateTimeFormat.GetMonthName(int.Parse(item));
          }
          else if (dateType == "dayweek")
          {
            list[i] = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((DayOfWeek)(int.Parse(item) - 1));
          }
          else if (dateType == "date")
          {

            DateTime d = DateTime.SpecifyKind(DateTime.Parse(item), DateTimeKind.Utc);
            list[i] = new TimeSpan(d.Ticks - baseDate.Ticks).TotalMilliseconds.ToString();
          }
          else
          {
            break;
          }
        }
      }
      catch (Exception)
      {


      }

    }

  }

  public class CustomReport
  {
    public CustomReport() { }
    public bool UsePaging { get; set; }
    public string Query { get; set; }
  }


  public class SummaryReport
  {
    public SummaryReport() { }
    public int Subcategory { get; set; }
    public ReportSummaryFields Fields { get; set; }
    public ReportFilter[] Filters { get; set; }
  }

  public class ReportSummaryFields
  {
    public ReportSummaryFields() { }
    public ReportSummaryDescriptiveField[] Descriptive { get; set; }
    public ReportSummaryCalculatedField[] Calculated { get; set; }
  }

  public class ReportSummaryDescriptiveField
  {
    public ReportSummaryDescriptiveField() { }
    public SummarySelectedField Field { get; set; }
    public string Value1 { get; set; }
  }

  public class ReportSummaryCalculatedField
  {
    public ReportSummaryCalculatedField() { }
    public SummarySelectedField Field { get; set; }
    public string Aggregate { get; set; }
    public string Comparator { get; set; }
    public string Value1 { get; set; }
    public string Value2 { get; set; }
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

  public class SummarySelectedField
  {
    public SummarySelectedField() { }
    public int FieldID { get; set; }
    public bool IsCustom { get; set; }
    public string FieldType { get; set; }
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
    public string FieldName { get; set; }
    public string DataType { get; set; }
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
          this.DateEdited = report.DateEditedUtc;
          this.LastViewed = (report.Row.Table.Columns.IndexOf("LastViewed") < 0 || report.Row["LastViewed"] == DBNull.Value ? null : (DateTime?)report.Row["LastViewed"]);
          this.Creator = (report.Row.Table.Columns.IndexOf("Creator") < 0 || report.Row["Creator"] == DBNull.Value ? "" : (string)report.Row["Creator"]);
          this.Editor = (report.Row.Table.Columns.IndexOf("Editor") < 0 || report.Row["Editor"] == DBNull.Value ? "" : (string)report.Row["Editor"]);
          this.EditorID = report.EditorID;
          this.FolderID = (report.Row.Table.Columns.IndexOf("Folder") < 0 || report.Row["Folder"] == DBNull.Value ? null : (int?)report.Row["Folder"]);
          this.IsPrivate = (report.Row.Table.Columns.IndexOf("IsPrivate") < 0 || report.Row["IsPrivate"] == DBNull.Value ? false : (bool)report.Row["IsPrivate"]);
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
        [DataMember] public string Editor { get; set; }
        [DataMember] public int EditorID { get; set; }
        [DataMember] public DateTime? LastViewed { get; set; }
        [DataMember] public DateTime DateEdited { get; set; }
        [DataMember] public int? FolderID { get; set; }
        [DataMember] public bool IsPrivate { get; set; }
      }
 
      [DataContract]
      public class GridResult
      {
        public GridResult() { }
        [DataMember] public int From { get; set; }
        [DataMember] public int To { get; set; }
        [DataMember] public int Total { get; set; }
        [DataMember] public object Data { get; set; }
      }

  public class CalculatedClauseItem
  {
    public CalculatedClauseItem() { }
    public CalculatedClauseItem(string field, string comparator, string alias) {
      this.Field = field;
      this.Comparator = comparator;
      this.Alias = alias;
    }
    public string Field { get; set; }
    public string AggField { get; set; }
    public string Comparator { get; set; }
    public string Alias { get; set; }
  }

  public class DescriptiveClauseItem
  {
    public DescriptiveClauseItem(string field, string alias) {
      this.Field = field;
      this.Alias = alias;
    }
    public string Field { get; set; }
    public string Alias { get; set; }
  }

    [DataContract]
  public class DataResult
  {
    public DataResult() { }
        
    [DataMember] public string name { get; set; }
    [DataMember] public string fieldType { get; set; }
    [DataMember] public string format { get; set; }
    [DataMember] public object[] data { get; set; }
  }


}


