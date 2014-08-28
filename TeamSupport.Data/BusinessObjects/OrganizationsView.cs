using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class OrganizationsViewItem
  {
  }
  
  public partial class OrganizationsView
  {
    public void LoadByParentID(int parentID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM OrganizationsView WHERE (ParentID = @ParentID) ORDER BY Name ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ParentID", parentID);
        Fill(command);
      }
    }

    public void LoadOneByParentID(int parentID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP 1 * FROM OrganizationsView WHERE (ParentID = @ParentID) ORDER BY Name ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ParentID", parentID);
        Fill(command);
      }
    }

    public void LoadByParentID(int parentID, bool includeCustomFields, string orderBy = "Name", int? limitNumber = null)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string limit = string.Empty;
        if (limitNumber != null)
        {
          limit = "TOP " + limitNumber.ToString();
        }
        string sql = "SELECT " + limit + " * FROM OrganizationsView ov WHERE (ParentID = @ParentID) ORDER BY " + orderBy;
        if (includeCustomFields) sql = InjectCustomFields(sql, "ov.OrganizationID", ReferenceType.Organizations);
        command.CommandText = sql;
        command.CommandType = CommandType.Text;
        command.CommandTimeout = 300;
        command.Parameters.AddWithValue("@ParentID", parentID);
        Fill(command);
      }
    }

    public void LoadByParentID(int parentID, bool includeCustomFields, NameValueCollection filters, string orderBy = "Name", int? limitNumber = null)
    {
      this.LoadOneByParentID(parentID);
      if (this.Count > 0)
      {
        using (SqlCommand command = new SqlCommand())
        {
          string limit = string.Empty;
          if (limitNumber != null)
          {
            limit = "TOP " + limitNumber.ToString();
          }
          //string sql = "SELECT " + limit + " * FROM OrganizationsView WHERE (ParentID = @ParentID) ORDER BY " + orderBy;
          string sql = BuildLoadByParentIDSql(limit, parentID, orderBy, filters, command.Parameters);
          if (includeCustomFields) sql = InjectCustomFields(sql, "OrganizationID", ReferenceType.Organizations);
          command.CommandText = sql;
          command.CommandType = CommandType.Text;
          this.DeleteAll();
          Fill(command);
        }
      }
    }

    private string BuildLoadByParentIDSql(string limit, int parentID, string orderBy, NameValueCollection filters, SqlParameterCollection filterParameters)
    {
      StringBuilder whereClauses = new StringBuilder();
      StringBuilder result = new StringBuilder();

      result.Append("SELECT " + limit + " * ");
      result.Append("FROM OrganizationsView ");
      result.Append("WHERE ParentID = @ParentID " + BuildWhereClausesFromFilters(parentID, filters, ref filterParameters) + " ");
      result.Append("ORDER BY " + orderBy);

      filterParameters.AddWithValue("@ParentID", parentID);

      return result.ToString();
    }

    private string BuildWhereClausesFromFilters(int parentID, NameValueCollection filters, ref SqlParameterCollection filterParameters)
    {
      StringBuilder result = new StringBuilder();

      CustomFields customFields = new CustomFields(this.LoginUser);
      customFields.LoadByReferenceType(parentID, ReferenceType.Organizations);

      //For each filter in filters
      StringBuilder filterFieldName;
      StringBuilder filterOperator;
      List<string> filterValues;
      CustomField customField = null;
      for (int i = 0; i < filters.Count; i++)
      {
        if (filters.GetKey(i) != null)
        {
          filterFieldName = new StringBuilder();
          filterOperator = new StringBuilder();
          filterValues = new List<string>();

          filterFieldName = GetFilterFieldName(filters.GetKey(i), filters.GetValues(i), customFields, ref filterOperator, ref filterValues, ref customField);
          if (filterFieldName.Length > 0)
          {
            result.Append(" AND ");
            if (customField == null)
            {
              if (filterValues.Count > 1) result.Append("(");
              result.Append(filterFieldName + " " + filterOperator + " @" + filterFieldName);
              filterParameters.AddWithValue("@" + filterFieldName, filterValues[0]);

              if (filterValues.Count > 1)
              {
                for (int j = 1; j < filterValues.Count; j++)
                {
                  result.Append(" OR ");
                  result.Append(filterFieldName + " " + filterOperator + " @" + filterFieldName + j.ToString());
                  filterParameters.AddWithValue("@" + filterFieldName + j.ToString(), filterValues[j]);                
                }
                result.Append(")");
              }
            }
            else
            {
              result.Append("OrganizationID IN (SELECT RefID FROM CustomValues WHERE CustomFieldID = "); 
              result.Append(customField.CustomFieldID.ToString());
              result.Append(" AND ");
              if (filterValues.Count > 1) result.Append("(");
              result.Append("CustomValue " + filterOperator + " @" + filterFieldName);
              filterParameters.AddWithValue("@" + filterFieldName, filterValues[0]);

              if (filterValues.Count > 1)
              {
                for (int j = 1; j < filterValues.Count; j++)
                {
                  result.Append(" OR ");
                  result.Append("CustomValue " + filterOperator + " @" + filterFieldName + j.ToString());
                  filterParameters.AddWithValue("@" + filterFieldName + j.ToString(), filterValues[j]);                
                }
                result.Append(")");
              }
            }
          }
        }
      }

      return result.ToString();
    }

    private StringBuilder GetFilterFieldName(string rawFieldName, string[] rawValues, CustomFields customFields, ref StringBuilder filterOperator, ref List<string> filterValues, ref CustomField customField)
    {
      StringBuilder result = new StringBuilder();
      string rawOperator = "=";
      int i = rawFieldName.IndexOf('[');
      if (i > -1)
      {
        result.Append(rawFieldName.Substring(0, i));
        int j = rawFieldName.IndexOf(']');
        rawOperator = rawFieldName.Substring(i + 1, j - i - 1).ToLower();
      }
      else
      {
        result.Append(rawFieldName);
      }

      Type filterFieldDataType = GetFilterFieldDataType(result.ToString(), customFields, ref customField);

      if (filterFieldDataType != null)
      {
        filterOperator.Append(GetSqlOperator(filterFieldDataType, rawOperator, rawValues, ref filterValues));
        if (filterOperator.Length == 0)
        {
          result.Clear();
        }
      }
      else
      {
        result.Clear();
      }

      return result;
    }

    private Type GetFilterFieldDataType(string fieldName, CustomFields customFields, ref CustomField customField)
    {
      string field = FieldMap.GetPrivateField(fieldName);
      if (field != "")
      {
        BaseItem baseItem = new BaseItem(Table.Rows[0], this);
        object o = baseItem.Row[field];
        if (o == null)
        {
          return null;
        }
        else
        {
          customField = null;
          return baseItem.Row.Table.Columns[field].DataType;
        }
      }
      else
      {
        customField = customFields.FindByApiFieldName(fieldName);
        if (customField != null)
        {
          switch (customField.FieldType)
          {
            case CustomFieldType.Boolean:
              Boolean boolean = true;
              return boolean.GetType();
            case CustomFieldType.Date:
            case CustomFieldType.DateTime:
            case CustomFieldType.Time:
              DateTime dateTime = DateTime.Now;
              return dateTime.GetType();
            case CustomFieldType.Number:
              int integer = 0;
              return integer.GetType();
            case CustomFieldType.PickList:
            case CustomFieldType.Text:
              string text = string.Empty;
              return text.GetType();
            default:
              return null;
          }
        }
        else
        {
          return null;
        }
      }
    }

    private string GetSqlOperator(Type filterFieldDataType, string rawOperator, string[] rawValues, ref List<string> filterValues)
    {
      StringBuilder result = new StringBuilder();

      for (int i = 0; i < rawValues.Length; i++)
      {
        if (rawValues[i].ToLower() == "[null]") 
        {
          if (i == 0)
          {
            if (rawOperator.ToLower() == "not")
            {
              result.Append("IS NOT");
            }
            else
            {
              result.Append("IS");
            }
          }

          filterValues.Add("NULL");
        }
        else
        {
          if (filterFieldDataType == typeof(System.DateTime))
          {
            //12345678901234    
            //yyyymmddhhmmss
            if (rawValues[i].Length == 14)
            {
              StringBuilder filterValue = new StringBuilder();
              //sql default datetime format "yyyy-mm-dd hh:mm:ss"
              //yyyy
              filterValue.Append(rawValues[i].Substring(0, 4));
              filterValue.Append("-");
              //mm
              filterValue.Append(rawValues[i].Substring(4, 2));
              filterValue.Append("-");
              //dd
              filterValue.Append(rawValues[i].Substring(6, 2));
              filterValue.Append(" ");
              //hh
              filterValue.Append(rawValues[i].Substring(8, 2));
              filterValue.Append(":");
              //mm
              filterValue.Append(rawValues[i].Substring(10, 2));
              filterValue.Append(":");
              //ss
              filterValue.Append(rawValues[i].Substring(12, 2));

              filterValues.Add(filterValue.ToString());

              if (i == 0)
              {
                if (rawOperator == "lt")
                {
                  result.Append("<");
                }
                else
                {
                  result.Append(">");
                }
              }
            }
          }
          else if (filterFieldDataType == typeof(System.Boolean))
          {
            if (rawValues[i].ToLower().IndexOf("t") > -1 || rawValues[i].ToLower().IndexOf("1") > -1 || rawValues[i].ToLower().IndexOf("y") > -1)
            {
              filterValues.Add("1");
            }
            if (i == 0)
            {
              result.Append("=");
            }
          }
          else if (filterFieldDataType == typeof(System.Double))
          {
            double d = double.Parse(rawValues[i]);
            filterValues.Add(d.ToString());

            if (i == 0)
            {
              switch (rawOperator)
              {
                case "lt": result.Append("<"); break;
                case "lte": result.Append("<="); break;
                case "gt": result.Append(">"); break;
                case "gte": result.Append(">="); break;
                case "not": result.Append("<>"); break;
                default: result.Append("="); break;
              }
            }
          }
          else if (filterFieldDataType == typeof(System.Int32))
          {
            int j = int.Parse(rawValues[i]);
            filterValues.Add(j.ToString());

            if (i == 0)
            {
              switch (rawOperator)
              {
                case "lt": result.Append("<"); break;
                case "lte": result.Append("<="); break;
                case "gt": result.Append(">"); break;
                case "gte": result.Append(">="); break;
                case "not": result.Append("<>"); break;
                default: result.Append("="); break;
              }
            }
          }
          else
          {
            switch (rawOperator)
            {
              case "contains":
                if (i == 0) result.Append("LIKE");
                filterValues.Add("%" + rawValues[i] + "%");
                break;
              case "not":
                if (i == 0) result.Append("<>");
                filterValues.Add(rawValues[i]);
                break;
              default:
                if (i == 0) result.Append("=");
                filterValues.Add(rawValues[i]);
                break;
            }
          }
        }
      }
      return result.ToString();
    }

    public void LoadByTicketID(int ticketID, string orderBy = "Name")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT ov.* FROM OrganizationsView ov LEFT JOIN OrganizationTickets ot ON ot.OrganizationID = ov.OrganizationID WHERE ot.TicketID = @TicketID ORDER BY " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadByProductID(int productID, string orderBy = "Name")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = 
          @"
            SELECT 
              DISTINCT
              ov.* 
            FROM 
              OrganizationsView ov 
              JOIN OrganizationProducts op 
                ON op.OrganizationID = ov.OrganizationID 
            WHERE 
              op.ProductID = @ProductID 
            ORDER BY 
          " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        Fill(command);
      }
    }
    public void LoadByVersionID(int versionID, string orderBy = "Name")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT 
          ov.* 
        FROM
          OrganizationsView ov 
        WHERE 
	        ov.OrganizationID IN
	        (
		        SELECT
			        op.OrganizationID
		        FROM
			        OrganizationProducts op
		        WHERE
			        op.ProductVersionID = @VersionID 
          )
        ORDER BY " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@VersionID", versionID);
        Fill(command);
      }
    }

    public void LoadForIndexing(int organizationID, int max, bool isRebuilding)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string text = @"
        SELECT TOP {0} OrganizationID
        FROM OrganizationsView ov WITH(NOLOCK)
        WHERE ov.NeedsIndexing = 1
        AND ov.ParentID = @OrganizationID
        ORDER BY DateModified DESC";

        if (isRebuilding)
        {
          text = @"
        SELECT OrganizationID
        FROM OrganizationsView ov WITH(NOLOCK)
        WHERE ov.ParentID = @OrganizationID
        ORDER BY DateModified DESC";
        }

        command.CommandText = string.Format(text, max.ToString());
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }
  }
  public class CustomerSearchCompany
  {
    public CustomerSearchCompany() { }
    public CustomerSearchCompany(OrganizationsViewItem item)
    {
      organizationID = item.OrganizationID;
      name = item.Name;
      website = item.Website;
      isPortal = item.HasPortalAccess;
    }

    public int organizationID { get; set; }
    public string name { get; set; }
    public string website { get; set; }
    public bool isPortal { get; set; }
    public int openTicketCount { get; set; }
    public CustomerSearchPhone[] phones { get; set; }
  }  
}
