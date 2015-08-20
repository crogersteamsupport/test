using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace TeamSupport.Data
{
  public partial class AssetsViewItem
  {
    public string DisplayName
    {
      get
      {
        if (String.IsNullOrEmpty(this.Name))
        {
          if (String.IsNullOrEmpty(this.SerialNumber))
          {
            return this.AssetID.ToString();
          }
          else
          {
            return this.SerialNumber;
          }
        }
        else
        {
          return this.Name;
        }
      }
    }
  }
  
  public partial class AssetsView
  {
    public void LoadByRefID(int refID, ReferenceType refType)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
          SELECT
            a.* 
          FROM
            AssetsView a
          WHERE
            a.AssetID IN
            (
              SELECT
                a.AssetID
              FROM
                  Assets a
                  JOIN AssetHistory h
                    ON a.AssetID = h.AssetID
                  JOIN AssetAssignments aa
                    ON h.HistoryID = aa.HistoryID
              WHERE 
                h.ShippedTo = @RefID
                AND h.RefType = @RefType
            )
          ORDER BY
            a.AssetID DESC
          ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefID", refID);
        command.Parameters.AddWithValue("@RefType", refType);
        Fill(command);
      }
    }

    public void LoadAssignedToContactsByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
          SELECT
            a.* 
          FROM
            AssetsView a
            JOIN AssetHistory h
              ON a.AssetID = h.AssetID
            JOIN AssetAssignments aa
              ON h.HistoryID = aa.HistoryID
            JOIN Users u
              ON h.ShippedTo = u.UserID
              AND h.RefType = 32
            JOIN Organizations o
              ON u.OrganizationID = o.OrganizationID
	            AND o.Name <> '_Unknown Company'	
          WHERE 
            o.OrganizationID = @OrganizationID
          ORDER BY 
            aa.AssetAssignmentsID DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByLikeAssetDisplayName(int organizationID, string name, int maxRows)
    {
      using (SqlCommand command = new SqlCommand())
      {
        StringBuilder text = new StringBuilder(@"
        SELECT 
          TOP (@MaxRows) * 
        FROM 
          Assets 
        WHERE 
          OrganizationID = @OrganizationID
          AND Location = 2
          AND (Name LIKE '%'+@Name+'%' OR SerialNumber LIKE '%'+@Name+'%')
        ");
        command.CommandText = text.ToString();
        command.CommandType = CommandType.Text;

        command.Parameters.AddWithValue("@Name", name.Trim());
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@MaxRows", maxRows);
        Fill(command);
      }
    }

    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM AssetsView WHERE OrganizationID = @OrganizationID ORDER BY DateCreated DESC";
        command.CommandText = InjectCustomFields(command.CommandText, "AssetID", ReferenceType.Assets);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadOneByOrganizationID(int organizationId)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP 1 * FROM AssetsView WHERE (organizationId = @OrganizationId)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationId", organizationId);

        Fill(command);
      }
    }

    public void LoadByOrganizationID(int organizationId, NameValueCollection filters, string orderBy = "DateCreated DESC", int? limitNumber = null)
    {
      //Get the column names, this row will be deleted before getting the actual data
      this.LoadOneByOrganizationID(organizationId);

      using (SqlCommand command = new SqlCommand())
      {
        string limit = string.Empty;

        if (limitNumber != null)
        {
          limit = "TOP " + limitNumber.ToString();
        }

        string sql = BuildLoadByParentOrganizationIdSql(limit, organizationId, orderBy, filters, command.Parameters);
        sql = InjectCustomFields(sql, "AssetID", ReferenceType.Assets);
        command.CommandType = CommandType.Text;
        command.CommandText = sql;
        command.Parameters.AddWithValue("@OrganizationId", organizationId);
        this.DeleteAll();

        Fill(command);
      }
    }

    /// <summary>
    /// Build the sql statement including its filters to avoid using the .NET filtering. This improves performance greatly.
    /// </summary>
    /// <param name="limit">Return the specified number of rows. E.g. "TOP 1"</param>
    /// <param name="parentID">Organization Id.</param>
    /// <param name="orderBy">Fields in sql format to sort the results by. E.g. "LastName, FirstName"</param>
    /// <param name="filters">Filters to be applied. Specified in the URL request.</param>
    /// <param name="filterParameters">SqlParamenterCollection for the input parameters of the sql query.</param>
    /// <returns>A string with the full sql statement.</returns>
    public string BuildLoadByParentOrganizationIdSql(string limit, int organizationParentId, string orderBy, NameValueCollection filters, SqlParameterCollection filterParameters)
    {
      StringBuilder result = new StringBuilder();

      result.Append("SELECT " + limit + " * ");
      result.Append("FROM AssetsView ");
      result.Append("WHERE OrganizationID = @OrganizationId " + BuildWhereClausesFromFilters(organizationParentId, filters, ref filterParameters) + " ");
      result.Append("ORDER BY " + orderBy);

      return result.ToString();
    }

    private string BuildWhereClausesFromFilters(int organizationParentId, NameValueCollection filters, ref SqlParameterCollection filterParameters)
    {
      StringBuilder result = new StringBuilder();

      CustomFields customFields = new CustomFields(this.LoginUser);
      customFields.LoadByReferenceType(organizationParentId, ReferenceType.Contacts);

      StringBuilder filterFieldName;
      StringBuilder filterOperator;
      List<string> filterValues;
      CustomField customField = null;

      foreach (string key in filters)
      {
        var value = filters[key];

        if (!string.IsNullOrEmpty(key))
        {
          filterFieldName = new StringBuilder();
          filterOperator = new StringBuilder();
          filterValues = new List<string>();

          filterFieldName = GetFilterFieldName(key, filters.GetValues(key), customFields, ref filterOperator, ref filterValues, ref customField);

          if (filterFieldName.Length > 0)
          {
            result.Append(" AND ");

            if (customField == null)
            {
              if (filterValues.Count > 1)
                result.Append("(");

              if (filterValues[0] == null)
              {
                string notEmptyOperator = filterOperator.ToString().ToLower() == "is not" ? "<>" : "=";
                result.Append("(");
                result.Append(filterFieldName + " " + filterOperator + " NULL");
                result.Append(" OR ");
                result.Append(filterFieldName + " " + notEmptyOperator + " ''");
                result.Append(")");
              }
              else
              {
                result.Append(filterFieldName + " " + filterOperator + " @" + filterFieldName);
                filterParameters.AddWithValue("@" + filterFieldName, filterValues[0]);
              }


              if (filterValues.Count > 1)
              {
                for (int j = 1; j < filterValues.Count; j++)
                {
                  result.Append(" OR ");

                  if (filterValues[j] == null)
                  {
                    string notEmptyOperator = filterOperator.ToString().ToLower() == "is not" ? "<>" : "=";
                    result.Append("(");
                    result.Append(filterFieldName + " " + filterOperator + " NULL");
                    result.Append(" OR ");
                    result.Append(filterFieldName + " " + notEmptyOperator + " ''");
                    result.Append(")");
                  }
                  else
                  {
                    result.Append(filterFieldName + " " + filterOperator + " @" + filterFieldName + j.ToString());
                    filterParameters.AddWithValue("@" + filterFieldName + j.ToString(), filterValues[j]);
                  }
                }

                result.Append(")");
              }
            }
            else
            {
              result.Append("AssetID IN (SELECT RefID FROM CustomValues WHERE CustomFieldID = ");
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

					result.Append(")");
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

      if (rawFieldName.Contains('['))
      {
        int index = rawFieldName.IndexOf('[');
        result.Append(rawFieldName.Substring(0, index));
        rawOperator = Regex.Match(rawFieldName, @"\[([^]]*)\]").Groups[1].Value;
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
      Type fieldDataType = null;
      string field = FieldMap.GetPrivateField(fieldName);

      if (!string.IsNullOrEmpty(field))
      {
        BaseItem baseItem = new BaseItem(Table.Rows[0], this);
        object fieldObject = baseItem.Row[field];

        if (fieldObject != null)
        {
          customField = null;
          fieldDataType = baseItem.Row.Table.Columns[field].DataType;
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
              fieldDataType = boolean.GetType();
              break;
            case CustomFieldType.Date:
            case CustomFieldType.DateTime:
            case CustomFieldType.Time:
              DateTime dateTime = DateTime.Now;
              fieldDataType = dateTime.GetType();
              break;
            case CustomFieldType.Number:
              int integer = 0;
              fieldDataType = integer.GetType();
              break;
            case CustomFieldType.PickList:
            case CustomFieldType.Text:
              string text = string.Empty;
              fieldDataType = text.GetType();
              break;
            default:
              fieldDataType = null;
              break;
          }
        }
      }

      return fieldDataType;
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

          filterValues.Add(null);
        }
        else
        {
          if (filterFieldDataType == typeof(System.DateTime))
          {
            //format needs to be: yyyymmddhhmmss
            if (rawValues[i].Length == "yyyymmddhhmmss".Length)
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

    public virtual void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SET NOCOUNT OFF; 
        SELECT 
          av.* 
        FROM 
          AssetsView av
          JOIN AssetTickets at 
            ON av.AssetID = at.AssetID
        WHERE 
          at.TicketID = @TicketID";
        command.CommandText = InjectCustomFields(command.CommandText, "av.AssetID", ReferenceType.Assets);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketID", ticketID);
        Fill(command);
      }
    }

    public virtual void LoadByTicketNumber(int ticketNumber, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SET NOCOUNT OFF; 
        SELECT 
          av.* 
        FROM 
          AssetsView av
          JOIN AssetTickets at 
            ON av.AssetID = at.AssetID
          JOIN Tickets t
            ON at.TicketID = t.TicketID
        WHERE 
          t.TicketNumber = @TicketNumber
          AND t.OrganizationID = @OrganizationID";
        command.CommandText = InjectCustomFields(command.CommandText, "av.AssetID", ReferenceType.Assets);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketNumber", ticketNumber);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByProductID(int productID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
          SELECT
            a.* 
          FROM
            AssetsView a
          WHERE
            a.ProductID = @ProductID
          ORDER BY
            a.AssetID DESC
          ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        Fill(command);
      }
    }

    public void LoadByProductIDLimit(int productID, int start)
    {
        int end = start + 10;
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
        WITH OrderedAsset AS
        (
	        SELECT 
		        AssetID, 
		        ROW_NUMBER() OVER (ORDER BY AssetID DESC) AS rownum
	        FROM 
		        AssetsView 
	        WHERE 
		        ProductID = @ProductID 
        ) 
        SELECT 
          a.*
        FROM
          AssetsView a
          JOIN OrderedAsset oa
            ON a.AssetID = oa.AssetID
        WHERE 
	        oa.rownum BETWEEN @start and @end
        ORDER BY
          a.AssetID DESC";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ProductID", productID);
            command.Parameters.AddWithValue("@start", start);
            command.Parameters.AddWithValue("@end", end);
            Fill(command);
        }
    }

    public void LoadByProductVersionID(int productVersionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
          SELECT
            a.* 
          FROM
            AssetsView a
          WHERE
            a.ProductVersionID = @ProductVersionID
          ORDER BY
            a.AssetID DESC
          ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
        Fill(command);
      }
    }

    public void LoadByProductVersionIDLimit(int productVersionID, int start)
    {
        int end = start + 10;
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
        WITH OrderedAsset AS
        (
	        SELECT 
		        AssetID, 
		        ROW_NUMBER() OVER (ORDER BY AssetID DESC) AS rownum
	        FROM 
		        AssetsView 
	        WHERE 
		        ProductVersionID = @ProductVersionID 
        ) 
        SELECT 
          a.*
        FROM
          AssetsView a
          JOIN OrderedAsset oa
            ON a.AssetID = oa.AssetID
        WHERE 
	        oa.rownum BETWEEN @start and @end
        ORDER BY
          a.AssetID DESC";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
            command.Parameters.AddWithValue("@start", start);
            command.Parameters.AddWithValue("@end", end);
            Fill(command);
        }
    }
  }
  
  public class InventorySearchAsset
  {
    public InventorySearchAsset() { }
    public InventorySearchAsset(AssetsViewItem item)
    {
      assetID               = item.AssetID;
      organizationID        = item.OrganizationID;
      productName           = item.ProductName;
      productVersionNumber  = item.ProductVersionNumber;
      serialNumber          = item.SerialNumber;
      name                  = item.Name;
      location              = item.Location;  
      notes                 = item.Notes;
      warrantyExpiration    = item.WarrantyExpiration;
      dateCreated           = item.DateCreated;
      dateModified          = item.DateModified;
      creatorName           = item.CreatorName;
      modifierName          = item.ModifierName;

    }

    public int assetID { get; set; }
    public int organizationID { get; set; }
    public string productName { get; set; }
    public string productVersionNumber { get; set; }
    public string serialNumber { get; set; }
    public string name { get; set; }
    public string location { get; set; }
    public string notes { get; set; }
    public DateTime? warrantyExpiration { get; set; }
    public DateTime? dateCreated { get; set; }
    public DateTime? dateModified { get; set; }
    public string creatorName { get; set; }
    public string modifierName { get; set; }
  }
}
