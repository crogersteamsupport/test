using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using dtSearch.Engine;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(TicketLoadFilter))]
  [KnownType(typeof(int[]))]
  public class TicketLoadFilter
  {
    public TicketLoadFilter()
    {
      Tags = new int[0];
      SearchText = "";
      SortColumn = "TicketNumber";
      SortAsc = false;
      MatchAllTerms = true;
    }
    [DataMember] public int? TicketTypeID { get; set; }
    [DataMember] public int? ProductID { get; set; }
    [DataMember] public int? ProductVersionID { get; set; }
    [DataMember] public int? SolvedVersionID { get; set; }
    [DataMember] public int? ReportedVersionID { get; set; }
    [DataMember] public bool? IsClosed { get; set; }
    [DataMember] public int? TicketStatusID { get; set; }
    [DataMember] public int? TicketSeverityID { get; set; }
    [DataMember] public int? UserID { get; set; }
    [DataMember] public int? GroupID { get; set; }
    [DataMember] public int? CustomerID { get; set; }
    [DataMember] public int? ContactID { get; set; }
    [DataMember] public int? AssetID { get; set; }
    [DataMember] public int? ForumCategoryID { get; set; }
    [DataMember] public bool? IsVisibleOnPortal { get; set; }
    [DataMember] public bool? IsKnowledgeBase { get; set; }
    [DataMember] public DateTime? DateCreatedBegin { get; set; }
    [DataMember] public DateTime? DateCreatedEnd { get; set; }
    [DataMember] public DateTime? DateModifiedBegin { get; set; }
    [DataMember] public DateTime? DateModifiedEnd { get; set; }
    [DataMember] public string SearchText { get; set; }
    [DataMember] public int[]  Tags { get; set; }
    [DataMember] public string SortColumn { get; set; }
    [DataMember] public bool SortAsc { get; set; }
    [DataMember] public bool MatchAllTerms { get; set; }
    [DataMember] public bool? IsSubscribed { get; set; }
    [DataMember] public bool? IsFlagged { get; set; }
    [DataMember] public bool? IsEnqueued { get; set; }
    [DataMember] public int? KnowledgeBaseCategoryID { get; set; }
    [DataMember] public int? ViewerID { get; set; }
    
  }



  public partial class TicketsViewItem
  {
    public string TicketUrl { get { return SystemSettings.ReadString(Collection.LoginUser, "AppDomain", "https://app.teamsupport.com") + "?TicketID=" + TicketID.ToString(); } }

    public string PortalUrl
    {
      get 
      {
        string portalLink = OrganizationSettings.ReadString(BaseCollection.LoginUser, OrganizationID, "ExternalPortalLink", "");
        if (portalLink == "") portalLink = "http://portal.teamsupport.com/protected/ticketdetail.aspx";
        portalLink = portalLink + "?OrganizationID=" + OrganizationID.ToString() + "&TicketNumber=" + TicketNumber.ToString();
        return portalLink;
      }
    }


    public bool IsRead
    {
      get
      {
        if (Row.Table.Columns.Contains("IsRead"))
        {
          return (bool)Row["IsRead"];
        }
        return false;
      }
    }

    public bool IsFlagged
    {
      get
      {
        if (Row.Table.Columns.Contains("IsFlagged"))
        {
          return (bool)Row["IsFlagged"];
        }
        return false;
      }
    }

    public bool IsSubscribed
    {
      get
      {
        if (Row.Table.Columns.Contains("IsSubscribed"))
        {
          return (bool)Row["IsSubscribed"];
        }
        return false;
      }
    }

    public bool IsEnqueued
    {
      get
      {
        if (Row.Table.Columns.Contains("IsEnqueued"))
        {
          return (bool)Row["IsEnqueued"];
        }
        return false;
      }
    }

    public int? ViewerID
    {
      get
      {
        if (Row.Table.Columns.Contains("ViewerID") && Row["ViewerID"] != DBNull.Value)
        {
          return (int)Row["ViewerID"];
        }
        return null;
      }
    }

    public bool GetIsCustomer(int organizationID)
    {

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT COUNT(*) FROM OrganizationTickets WHERE (OrganizationID = @OrganizationID) AND (TicketID = @TicketID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", TicketID);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        object o = Collection.ExecuteScalar(command);
        if (o == null || o == DBNull.Value) return false;
        return (int)o > 0;
      }
    }

    public bool UserHasRights(User user)
    {
      return Ticket.UserHasRights(user, this.GroupID, this.UserID, this.TicketID, this.IsKnowledgeBase);
    }



  }
  
  public partial class TicketsView
  {

    public int GetUserTicketCount(int userID, int closed)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT COUNT(*) FROM TicketsView tv LEFT JOIN UserTickets ut ON ut.TicketID = tv.TicketID WHERE ut.UserID = @UserID and tv.IsClosed = @closed";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@UserID", userID);
            command.Parameters.AddWithValue("@closed", closed);
            object o = ExecuteScalar(command);
            if (o == null || o == DBNull.Value) return 0;
            return (int)o;
        }
    }

    public int GetOrganizationTicketCount(int organizationID, int closed)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT COUNT(*) FROM TicketsView tv LEFT JOIN OrganizationTickets ot ON ot.TicketID = tv.TicketID WHERE ot.OrganizationID = @OrganizationID and tv.IsClosed = @closed";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@closed", closed);
        object o = ExecuteScalar(command);
        if (o == null || o == DBNull.Value) return 0;
        return (int)o;
      }
    }

    public int GetProductTicketCount(int productID, int closed)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT 
          COUNT(*) 
        FROM 
          TicketsView tv 
        WHERE 
          tv.ProductID = @ProductID
          AND tv.IsClosed = @closed
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@closed", closed);
        object o = ExecuteScalar(command);
        if (o == null || o == DBNull.Value) return 0;
        return (int)o;
      }
    }

    public int GetProductFamilyTicketCount(int productFamilyID, int closed)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
        SELECT 
          COUNT(*) 
        FROM 
          TicketsView tv 
          JOIN Products p
            ON tv.ProductID = p.ProductID            
        WHERE 
          p.ProductFamilyID = @ProductFamilyID
          AND tv.IsClosed = @closed
        ";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ProductFamilyID", productFamilyID);
            command.Parameters.AddWithValue("@closed", closed);
            object o = ExecuteScalar(command);
            if (o == null || o == DBNull.Value) return 0;
            return (int)o;
        }
    }


    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketsView WHERE (OrganizationID = @OrganizationID) ORDER BY TicketNumber";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    // This is to support handling the ticket number along with the ticketID
    public void LoadByTicketIDAndOrganizationID(int organizationID, int ticketID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
            SELECT 
                * 
            FROM 
                TicketsView 
            WHERE
                OrganizationID = @OrganizationID
                AND TicketID = @TicketID
            ORDER BY 
                TicketNumber";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@TicketID", ticketID);
            Fill(command);
        }
    }

    public void LoadByOrganizationIDOrderByNumberDESC(int organizationID, int? limitNumber = null)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string limit = string.Empty;
        if (limitNumber != null)
        {
          limit = "TOP " + limitNumber.ToString();
        }
        command.CommandText = "SELECT " + limit + " * FROM TicketsView WHERE (OrganizationID = @OrganizationID) ORDER BY TicketNumber DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadLatest5Tickets(int organizationID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT top 5 tv.* FROM TicketsView tv LEFT JOIN OrganizationTickets ot ON ot.TicketID = tv.TicketID WHERE ot.OrganizationID = @OrganizationID ORDER BY TicketNumber desc";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            Fill(command);
        }
    }

    public void LoadLatest5ProductTickets(int productID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT 
          top 5 tv.* 
        FROM 
          TicketsView tv 
        WHERE 
          tv.ProductID = @ProductID 
        ORDER BY 
          TicketNumber desc";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        Fill(command);
      }
    }

    public void LoadLatest5ProductVersionTickets(int productVersionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT 
          top 5 tv.* 
        FROM 
          TicketsView tv 
        WHERE 
          tv.ReportedVersionID = @ProductVersionID
          OR tv.SolvedVersionID = @ProductVersionID
        ORDER BY 
          TicketNumber desc";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
        Fill(command);
      }
    }

    public void LoadLatest5ProductFamilyTickets(int productFamilyID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
        SELECT 
          top 5 tv.* 
        FROM 
          TicketsView tv 
          JOIN Products p
            ON tv.ProductID = p.ProductID           
        WHERE 
          p.ProductFamilyID = @ProductFamilyID 
        ORDER BY 
          TicketNumber desc";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ProductFamilyID", productFamilyID);
            Fill(command);
        }
    }

    public void LoadRelated(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT tv.* FROM TicketsView tv WHERE tv.TicketID IN (SELECT Ticket2ID FROM TicketRelationships WHERE Ticket1ID = @TicketID) OR tv.TicketID IN (SELECT Ticket1ID FROM TicketRelationships WHERE Ticket2ID = @TicketID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadRelatedByTicketNumber(int ticketNumber, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT 
          tv.*
        FROM
          TicketsView tv
        WHERE
          tv.OrganizationID = @OrganizationID
          AND tv.TicketID IN 
          (
            SELECT
              tr2.Ticket2ID
            FROM
              TicketRelationships tr2
              JOIN Tickets t2
                ON tr2.Ticket1ID = t2.TicketID
            WHERE 
              t2.TicketNumber = @TicketNumber
              AND t2.OrganizationID = @OrganizationID
          ) 
          OR tv.TicketID IN 
          (
            SELECT
              tr1.Ticket1ID
            FROM
              TicketRelationships tr1
              JOIN Tickets t1
                ON tr1.Ticket2ID = t1.TicketID
            WHERE
              t1.TicketNumber = @TicketNumber
              AND t1.OrganizationID = @OrganizationID
          )";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@TicketNumber", ticketNumber);
        Fill(command);
      }
    }

    public void LoadChildren(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT tv.* FROM TicketsView tv WHERE tv.ParentID = @TicketID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public static TicketsViewItem GetTicketsViewItemByNumber(LoginUser loginUser, int ticketNumber)
    {
      TicketsView ticketsView = new TicketsView(loginUser);
      ticketsView.LoadByTicketNumber(ticketNumber, loginUser.OrganizationID);
      if (ticketsView.IsEmpty)
        return null;
      else
        return ticketsView[0];
    }

    public void LoadUserTicketsByTicketId(int ticketId, int organizationId)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM UserTicketsView WHERE OrganizationID = @organizationId AND TicketID = @ticketId";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@organizationId", organizationId);
        command.Parameters.AddWithValue("@ticketId", ticketId);
        Fill(command);
      }
    }

    public void LoadByTicketNumber(int ticketNumber, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP 1 * FROM UserTicketsView WHERE OrganizationID = @OrganizationID AND TicketNumber= @TicketNumber AND ViewerID = @ViewerID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@ViewerID", LoginUser.UserID);
        command.Parameters.AddWithValue("@TicketNumber", ticketNumber);
        Fill(command);
      }
    }

    public void LoadByTicketNumberFromTicketsView(int ticketNumber, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP 1 * FROM TicketsView WHERE OrganizationID = @OrganizationID AND TicketNumber= @TicketNumber";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@TicketNumber", ticketNumber);
        Fill(command);
      }
    }

    /// <summary>
    /// Loads tickets that are associated with a customer's organizationid
    /// </summary>
    /// <param name="organizationID"></param>
    public void LoadByCustomerID(int organizationID, string orderBy = "TicketNumber")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT tv.* FROM TicketsView tv LEFT JOIN OrganizationTickets ot ON ot.TicketID = tv.TicketID WHERE ot.OrganizationID = @OrganizationID ORDER BY " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }
    /// <summary>
    /// Loads tickets that are associated with a customer's organizationid by ticket type
    /// </summary>
    /// <param name="organizationID"></param>
    /// <param name="ticketTypeID"></param>
    public void LoadByCustomerTicketTypeID(int organizationID, int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT tv.* FROM TicketsView tv  WHERE (tv.TicketTypeID = @TicketTypeID)  AND EXISTS (SELECT * FROM OrganizationTickets WHERE OrganizationID = @OrganizationID AND TicketID = tv.TicketID) ORDER BY tv.TicketNumber";
        command.CommandText = InjectCustomFields(command.CommandText, "TicketID", ReferenceType.Tickets, ticketTypeID);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByContactID(int userID, string orderBy)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT tv.* FROM TicketsView tv LEFT JOIN UserTickets ut ON ut.TicketID = tv.TicketID WHERE ut.UserID = @UserID ORDER BY " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command);
      }
    }

    public void Load5MostRecentByContactID(int userID, string orderBy)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT tv.* FROM TicketsView tv LEFT JOIN UserTickets ut ON ut.TicketID = tv.TicketID WHERE ut.UserID = @UserID and tv.IsClosed = 'False'  ORDER BY " + orderBy;
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@UserID", userID);
            Fill(command);
        }
    }

    public void LoadByContactID(int userID)
    {
      LoadByContactID(userID, "TicketNumber");
    }

    public void LoadByTicketTypeID(int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketsView WHERE (TicketTypeID = @TicketTypeID) ORDER BY TicketNumber";
        command.CommandText = InjectCustomFields(command.CommandText, "TicketID", ReferenceType.Tickets, ticketTypeID);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
        Fill(command);
      }
    }

    public void LoadOneByOrganizationId(int organizationId)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP 1 * FROM TicketsView WHERE OrganizationID = @OrganizationID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationId);
        Fill(command);
      }
    }

    public void LoadByTicketTypeID(int ticketTypeID, int organizationId, NameValueCollection filters)
    {
      //Get the column names, this row will be deleted before getting the actual data
      this.LoadOneByOrganizationId(organizationId);

      using (SqlCommand command = new SqlCommand())
      {
        SqlParameterCollection filterParameters = command.Parameters;
        command.CommandText = "SELECT * FROM TicketsView WHERE (TicketTypeID = @TicketTypeID) " + BuildWhereClausesFromFilters(organizationId, filters, ref filterParameters) + " ORDER BY TicketNumber";
        command.CommandText = InjectCustomFields(command.CommandText, "TicketID", ReferenceType.Tickets, ticketTypeID);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
        this.DeleteAll();
        Fill(command);
      }
    }

    private string BuildWhereClausesFromFilters(int organizationId, NameValueCollection filters, ref SqlParameterCollection filterParameters)
    {
      StringBuilder result = new StringBuilder();
      CustomFields customFields = new CustomFields(this.LoginUser);
      customFields.LoadByReferenceType(organizationId, ReferenceType.Contacts);

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

    public void LoadAllSlaViolations()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TicketID, SlaViolationTime, SlaWarningTime  FROM TicketsView WHERE SlaViolationTime <= 0 OR SlaWarningTime <=0";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public void LoadForIndexing(int organizationID, int max, bool isRebuilding)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string text = @"
        SELECT TOP {0} TicketID
        FROM TicketsView tv WITH(NOLOCK)
        WHERE tv.NeedsIndexing = 1
        AND tv.OrganizationID= @OrganizationID
        ORDER BY DateModified DESC";

        if (isRebuilding)
        {
        text = @"
        SELECT TicketID
        FROM TicketsView tv WITH(NOLOCK)
        WHERE tv.OrganizationID= @OrganizationID
        ORDER BY DateModified DESC";
        }

        command.CommandText = string.Format(text, max.ToString());
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByFilter(int pageIndex, int pageSize, TicketLoadFilter filter)
    {
      LoadByRange(pageIndex * pageSize, pageIndex * pageSize + pageSize, filter);
    }

    public void LoadByRange(int from, int to, TicketLoadFilter filter)
    {
      SqlCommand command = GetLoadRangeCommand(LoginUser, from, to, filter);
      Fill(command);
    }


    public static SqlCommand GetLoadExportCommand(LoginUser loginUser, TicketLoadFilter filter)
    {
      SqlCommand command = new SqlCommand();
      string sort = filter.SortColumn.Trim();
      StringBuilder builder = new StringBuilder();

      builder.Append(
            @"SELECT tv.[TicketNumber]
            ,tv.[Name]
            ,tv.[TicketTypeName]
            ,tv.[Status]
            ,tv.[Severity]
            ,tv.[UserName]
            ,tv.[Customers]
            ,tv.[Contacts]
            ,tv.[ProductName]
            ,tv.[ReportedVersion]
            ,tv.[SolvedVersion]
            ,tv.[GroupName]
            ,tv.[DateModified]
            ,tv.[DateCreated]
            ,tv.[DaysOpened]
            ,tv.[IsClosed]
            ,tv.[CloserName]
            ,tv.[SlaViolationTime]
            ,tv.[DueDate]");
      GetFilterWhereClause(loginUser, filter, command, builder);
      builder.Append(" ORDER BY tv.[" + sort);
      if (filter.SortAsc) builder.Append("] ASC"); else builder.Append("] DESC");
      command.CommandText = builder.ToString().Replace(Environment.NewLine, " ");
      command.CommandType = CommandType.Text;
      command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
      return command;
    
    }

    public static SqlCommand GetLoadRangeCommand(LoginUser loginUser, int from, int to, TicketLoadFilter filter)
    {
      SqlCommand command = new SqlCommand();

      string sort = filter.SortColumn.Trim();
      string sortFields;
      switch (sort)
      {
        case "Severity": 
          sortFields = "[SeverityPosition]";
          sort = string.Format("[SeverityPosition] {0}", (filter.SortAsc ? "ASC" : "DESC"));
          break;
        case "Status": 
          sortFields = "[StatusPosition], [Status], [TicketTypeName]";
          sort = string.Format("[StatusPosition] {0}, [Status] {0}, [TicketTypeName] {0}", (filter.SortAsc ? "ASC" : "DESC"));
          break;
        default: 
          sortFields = string.Format("[{0}]", sort);
          sort = string.Format("[{0}] {1}", sort, (filter.SortAsc ? "ASC" : "DESC"));
          break;
      }

      if (filter.IsEnqueued == true)
      {
        sort = "[QueuePosition] ASC";
        sortFields = "[QueuePosition]";
      }

      string fields =
        @"
          tv.[TicketID]
        ,tv.[ProductName]
        ,tv.[ReportedVersion]
        ,tv.[SolvedVersion]
        ,tv.[GroupName]
        ,tv.[TicketTypeName]
        ,tv.[UserName]
        ,tv.[Status]
        ,tv.[StatusPosition]
        ,tv.[SeverityPosition]
        ,tv.[IsClosed]
        ,tv.[Severity]
        ,tv.[TicketNumber]
        ,tv.[IsVisibleOnPortal]
        ,tv.[IsKnowledgeBase]
        ,tv.[ReportedVersionID]
        ,tv.[SolvedVersionID]
        ,tv.[ProductID]
        ,tv.[GroupID]
        ,tv.[UserID]
        ,tv.[TicketStatusID]
        ,tv.[TicketTypeID]
        ,tv.[TicketSeverityID]
        ,tv.[OrganizationID]
        ,tv.[Name]
        ,tv.[ParentID]
        ,tv.[ModifierID]
        ,tv.[CreatorID]
        ,tv.[DateModified]
        ,tv.[DateCreated]
        ,tv.[DateClosed]
        ,tv.[CloserID]
        ,tv.[DaysClosed]
        ,tv.[DaysOpened]
        ,tv.[CloserName]
        ,tv.[CreatorName]
        ,tv.[ModifierName]
        ,tv.[HoursSpent]
        ,tv.[Tags]
        ,tv.[Customers]
        ,tv.[Contacts]
        ,tv.[SlaViolationTime]
        ,tv.[SlaWarningTime]
        ,tv.[SlaViolationDate]
        ,tv.[SlaWarningDate]
        ,tv.[TicketSource]
        ,tv.[ForumCategory]
        ,tv.[CategoryName]
        ,CAST(0 AS dec(24,6)) AS [SlaViolationHours]
        ,CAST(0 AS dec(24,6)) AS [SlaWarningHours]
        ,tv.ViewerID
        ,tv.IsSubscribed
        ,tv.IsEnqueued
        ,tv.QueuePosition
        ,tv.IsRead
        ,tv.IsFlagged
        ,tv.KnowledgeBaseCategoryID
        ,tv.KnowledgeBaseCategoryName
        ,tv.SalesForceID
        ,tv.DateModifiedBySalesForceSync
        ,tv.DueDate
        ,tv.ProductFamilyID";
      StringBuilder where = new StringBuilder();
      GetFilterWhereClause(loginUser, filter, command, where);

      string query = @"

        WITH
        BaseQuery AS(
          SELECT tv.TicketID, {1} {0}
        ),

        RowQuery AS (
          SELECT BaseQuery.*, ROW_NUMBER() OVER (ORDER BY {2}) AS 'RowNum' FROM BaseQuery
        ),

        PageQuery AS (
          SELECT  * FROM RowQuery WHERE RowNum BETWEEN  @FromIndex AND @ToIndex
        )

        SELECT PageQuery.RowNum, {3}
        FROM PageQuery
        INNER JOIN UserTicketsView tv ON tv.TicketID = PageQuery.TicketID 
        WHERE tv.ViewerID = @ViewerID
        ORDER BY PageQuery.RowNum ASC
        ";

      command.CommandText = string.Format(query, where.ToString(), sortFields, sort, fields);
      command.CommandType = CommandType.Text;
      command.Parameters.AddWithValue("@FromIndex", from+1);
      command.Parameters.AddWithValue("@ToIndex", to+1);
      command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
      return command;
    }


    public int GetFilterCount(TicketLoadFilter filter)
    {
      return GetFilterCount(LoginUser, filter);
    }

    public static int GetFilterCount(LoginUser loginUser, TicketLoadFilter filter)
    {
      using (SqlCommand command = new SqlCommand())
      {
        StringBuilder builder = new StringBuilder();
        builder.Append("SELECT COUNT(*) ");
        GetFilterWhereClause(loginUser, filter, command, builder);
        command.CommandText = builder.ToString().Replace(Environment.NewLine, " ");
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
        object o = SqlExecutor.ExecuteScalar(loginUser, command);
        if (o == null || o == DBNull.Value) return 0;
        return (int)o;
      }
    }


    private static void GetFilterWhereClause(LoginUser loginUser, TicketLoadFilter filter, SqlCommand command, StringBuilder builder)
    {
      builder.Append(" FROM UserTicketsView tv WHERE (tv.OrganizationID = @OrganizationID)");
      AddTicketParameter("TicketTypeID", filter.TicketTypeID, false, builder, command);
      if (filter.TicketStatusID != null) AddTicketParameter("TicketStatusID", filter.TicketStatusID, false, builder, command);
      else AddTicketParameter("IsClosed", filter.IsClosed, false, builder, command);
      AddTicketParameter("TicketSeverityID", filter.TicketSeverityID, false, builder, command);
      AddTicketParameter("ProductID", filter.ProductID, true, builder, command);
      AddTicketParameter("ReportedVersionID", filter.ReportedVersionID, true, builder, command);
      AddTicketParameter("SolvedVersionID", filter.SolvedVersionID, true, builder, command);
      AddTicketParameter("IsVisibleOnPortal", filter.IsVisibleOnPortal, false, builder, command);
      AddTicketParameter("IsKnowledgeBase", filter.IsKnowledgeBase, false, builder, command);
      AddTicketParameter("KnowledgeBaseCategoryID", filter.KnowledgeBaseCategoryID, false, builder, command);
      AddTicketParameter("IsSubscribed", filter.IsSubscribed, false, builder, command);
      AddTicketParameter("IsFlagged", filter.IsFlagged, false, builder, command);
      AddTicketParameter("IsEnqueued", filter.IsEnqueued, false, builder, command);
      AddTicketParameter("DateCreated", "DateCreatedBegin", filter.DateCreatedBegin, ">=", builder, command);
      AddTicketParameter("DateCreated", "DateCreatedEnd", filter.DateCreatedEnd, "<=", builder, command);
      AddTicketParameter("DateModified", "DateModifiedBegin", filter.DateModifiedBegin, ">=", builder, command);
      AddTicketParameter("DateModified", "DateModifiedEnd", filter.DateModifiedEnd, "<=", builder, command);
      AddTicketParameter("ViewerID", filter.ViewerID != null ? filter.ViewerID : loginUser.UserID, false, builder, command);

      if (filter.ForumCategoryID != null && filter.ForumCategoryID == -1)
      {
        builder.Append(" AND (tv.ForumCategory IS NOT NULL)");
      }

      if (filter.UserID != null && filter.GroupID != null && filter.GroupID == -1)
      {
        //User's all groups all tickets
        builder.Append(" AND (tv.GroupID IN (SELECT gu.GroupID FROM GroupUsers gu WHERE gu.UserID = @UserID))");
        command.Parameters.AddWithValue("UserID", filter.UserID);
      }
      else if (filter.UserID != null && filter.GroupID != null && filter.GroupID == -2)
      {
        //Users's all groups, unassigned tickets
        builder.Append(" AND ((tv.UserID IS NULL OR tv.UserID < 0) AND tv.GroupID IN (SELECT gu.GroupID FROM GroupUsers gu WHERE gu.UserID = @UserID))");
        command.Parameters.AddWithValue("UserID", filter.UserID);
      }
      else if (filter.GroupID != null && filter.UserID != null && filter.UserID == -2)
      {
        //Group's unassigned tickets
        builder.Append(" AND ((tv.UserID IS NULL OR tv.UserID < 0) AND tv.GroupID = @GroupID)");
        command.Parameters.AddWithValue("GroupID", filter.GroupID);
      }
      else if (filter.GroupID == null && filter.UserID != null && filter.UserID == -2)
      {
        //All unassigned tickets
        builder.Append(" AND (tv.UserID IS NULL OR tv.UserID < 0) ");
      }
      else
      {
        AddTicketParameter("UserID", filter.UserID, true, builder, command);
        AddTicketParameter("GroupID", filter.GroupID, true, builder, command);
      }

      if (filter.CustomerID != null)
      {
        builder.Append(" AND (EXISTS(SELECT * FROM OrganizationTickets ot WHERE (ot.OrganizationID = @CustomerID) AND (ot.TicketID = tv.TicketID)))");
        command.Parameters.AddWithValue("CustomerID", filter.CustomerID);
      }

      if (filter.ContactID != null)
      {
        builder.Append(" AND (EXISTS(SELECT * FROM UserTickets ut WHERE (ut.UserID = @ContactID) AND (ut.TicketID = tv.TicketID)))");
        command.Parameters.AddWithValue("ContactID", filter.ContactID);
      }

      if (filter.AssetID != null)
      {
        builder.Append(" AND (EXISTS(SELECT * FROM AssetTickets asst WHERE (asst.AssetID = @AssetID) AND (asst.TicketID = tv.TicketID)))");
        command.Parameters.AddWithValue("AssetID", filter.AssetID);
      }

      if (filter.ProductVersionID != null)
      {
        builder.Append(" AND (tv.ReportedVersionID = @ProductVersionID OR tv.SolvedVersionID = @ProductVersionID)");
        command.Parameters.AddWithValue("ProductVersionID", filter.ProductVersionID);
      }

      if (!String.IsNullOrEmpty(filter.SearchText.Trim()))
      {
        int[] list = GetTicketIDs(filter.SearchText, loginUser);
        if (list.Length > 0)
        {
          string ids = string.Join(",", Array.ConvertAll<int, string>(list, Convert.ToString));
          builder.Append(string.Format(" AND (tv.TicketID IN ({0})) ", ids));
        }
        else
        {
          builder.Append(" AND (tv.TicketID IN (-1)) ");
        }
      }

      if (filter.Tags != null && filter.Tags.Length > 0)
      {
        for (int i = 0; i < filter.Tags.Length; i++)
        {
          builder.Append(" AND EXISTS (SELECT * FROM TagLinks WHERE TagLinks.RefID=tv.TicketID AND TagLinks.RefType=17 AND TagLinks.TagID = @TagID" + i.ToString() + ")");
          command.Parameters.AddWithValue("@TagID" + i.ToString(), filter.Tags[i]);
        }
      }

      string rightsClause = "";

      User user = Users.GetUser(loginUser, loginUser.UserID);
      switch (user.TicketRights)
      {
        case TicketRightType.All:
          break;
        case TicketRightType.Assigned:
          builder.Append(" AND (tv.UserID=" + loginUser.UserID.ToString() + " OR tv.IsKnowledgeBase=1) ");
          break;
        case TicketRightType.Groups:
          rightsClause = @" AND ({0}
              (tv.UserID = {1}) OR
              (tv.IsKnowledgeBase = 1) OR
              (tv.UserID IS NULL AND tv.GroupID IS NULL)) ";
          Groups groups = new Groups(loginUser);
          groups.LoadByUserID(loginUser.UserID);
          List<int> groupList = new List<int>();
          foreach (Group group in groups)
	        {
            groupList.Add(group.GroupID);
	        }
          string groupString = groupList.Count < 1 ? "" : string.Format("(tv.GroupID IN ({0})) OR ", DataUtils.IntArrayToCommaString(groupList.ToArray()));
          builder.Append(string.Format(rightsClause, groupString, loginUser.UserID.ToString()));
          break;
        case TicketRightType.Customers:
          rightsClause = @" AND (TicketID in (
            SELECT ot.TicketID FROM OrganizationTickets ot
            INNER JOIN UserRightsOrganizations uro ON ot.OrganizationID = uro.OrganizationID 
            WHERE uro.UserID={0}) OR
            tv.UserID = {0} OR
            tv.IsKnowledgeBase = 1)";
          builder.Append(string.Format(rightsClause, loginUser.UserID.ToString()));
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
                    TicketID IN 
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
                            urpf.UserID = {0}
                    ) 
                    OR tv.UserID = {0} 
                  )";
                  builder.Append(string.Format(rightsClause, loginUser.UserID.ToString()));
                  break;
              default:
                  break;
          }
      }
    }

    public static SearchResults GetQuickSearchTicketResults(string searchTerm, LoginUser loginUser, TicketLoadFilter filter)
    { 
        Options options = new Options();
        options.TextFlags = TextFlags.dtsoTfRecognizeDates;

        using (SearchJob job = new SearchJob())
        {
          searchTerm = searchTerm.Trim();
          job.Request = searchTerm;
          job.FieldWeights = "TicketNumber: 5000, Name: 1000";

          StringBuilder conditions = new StringBuilder();
          if (filter != null)
          {
            if (filter.IsKnowledgeBase != null) conditions.Append(" (IsKnowledgeBase::" + filter.IsKnowledgeBase.ToString() + ") ");
          }
          AppendTicketRightsConditions(loginUser, conditions);

          job.BooleanConditions = conditions.ToString();
          job.MaxFilesToRetrieve = 25;
          job.AutoStopLimit = 100000;
          job.TimeoutSeconds = 10;
          job.SearchFlags =
            SearchFlags.dtsSearchStemming |
            SearchFlags.dtsSearchDelayDocInfo;

          int num = 0;
          if (!int.TryParse(searchTerm, out num))
          {
            job.Fuzziness = 1;
            job.Request = job.Request + "*";
            job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchSelectMostRecent;
            //job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchFuzzy | SearchFlags.dtsSearchSelectMostRecent;
          }
          

          if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;
          job.IndexesToSearch.Add(DataUtils.GetTicketIndexPath(loginUser));
          job.Execute();
          
          return job.Results;
        }
    }

    public static SearchResults GetSearchTicketResults(string searchTerm, LoginUser loginUser)
    {
      Options options = new Options();
      options.TextFlags = TextFlags.dtsoTfRecognizeDates;
      using (SearchJob job = new SearchJob())
      {

        job.Request = searchTerm;
        job.FieldWeights = "Name: 1000";


        StringBuilder conditions = new StringBuilder();
        AppendTicketRightsConditions(loginUser, conditions);

        job.BooleanConditions = conditions.ToString();


        //job.MaxFilesToRetrieve = 1000;
        //job.AutoStopLimit = 1000000;
        job.TimeoutSeconds = 30;
        job.SearchFlags =
          //SearchFlags.dtsSearchSelectMostRecent |
          SearchFlags.dtsSearchDelayDocInfo;

        int num = 0;
        if (!int.TryParse(searchTerm, out num))
        {
          //job.Fuzziness = 1;
          job.SearchFlags = job.SearchFlags | 
            //SearchFlags.dtsSearchFuzzy | 
            //SearchFlags.dtsSearchStemming |
            SearchFlags.dtsSearchPositionalScoring |
            SearchFlags.dtsSearchAutoTermWeight;
        }

        if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;
        job.IndexesToSearch.Add(DataUtils.GetTicketIndexPath(loginUser));
        job.Execute();

        return job.Results;
      }

    }

    private static void AppendTicketRightsConditions(LoginUser loginUser, StringBuilder conditions)
    {

      User user = Users.GetUser(loginUser, loginUser.UserID);
      if (user.TicketRights == TicketRightType.Assigned)
      {
        if (conditions.Length > 0) conditions.Append(" AND");
        conditions.Append(" ((IsKnowledgeBase::True) OR (UserID::" + user.UserID.ToString() + ")) ");
      }
      else if (user.TicketRights == TicketRightType.Groups)
      {
        if (conditions.Length > 0) conditions.Append(" AND");
        conditions.Append(" (");
        Groups groups = new Groups(loginUser);
        groups.LoadByUserID(user.UserID);
        conditions.Append(" (IsKnowledgeBase::True) OR (UserID::" + user.UserID.ToString() + ") ");
        foreach (Group group in groups)
        {
          conditions.Append("OR (GroupID::" + group.GroupID.ToString() + ") ");
        }
        conditions.Append(") ");
      }
      else if (user.TicketRights == TicketRightType.Customers)
      {
        if (conditions.Length > 0) conditions.Append(" AND");
        conditions.Append(" (");
        conditions.Append(" (IsKnowledgeBase::True) OR (UserID::" + user.UserID.ToString() + ") ");
        Organizations orgs = new Organizations(loginUser);
        orgs.LoadByUserRights(user.UserID);

        foreach (Organization org in orgs)
        {
          conditions.Append("OR (Customers::" + Regex.Replace(org.Name, @"[()]", "", RegexOptions.IgnoreCase) + ") ");
        }

        conditions.Append(") ");
      }

      if ((ProductFamiliesRightType)user.ProductFamiliesRights != ProductFamiliesRightType.AllFamilies)
      {
          Organization organization = Organizations.GetOrganization(loginUser, loginUser.OrganizationID);
          if (organization.UseProductFamilies)
          {
              ProductFamilies userRightsProductFamilies = new ProductFamilies(loginUser);
              userRightsProductFamilies.LoadByUserRights(loginUser.UserID);
              if (userRightsProductFamilies.Count > 0)
              {
                  if (conditions.Length > 0) conditions.Append(" AND");
                  conditions.Append(" (");
                  conditions.Append(" (UserID::" + user.UserID.ToString() + ") ");
                  foreach (ProductFamily productFamily in userRightsProductFamilies)
                  {
                      conditions.Append("OR (ProductFamilyID::" + productFamily.ProductFamilyID.ToString() + ") ");
                  }
                  conditions.Append(") ");
              }
          }
      }
        
    }

    public static int[] GetTicketIDs(string searchTerm, LoginUser loginUser)
    {
      return GetTicketIDs(searchTerm, loginUser, null);
    }

    /// <summary>
    /// Searches tickets based on search term and fitlers
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <param name="loginUser"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static int[] GetTicketIDs(string searchTerm, LoginUser loginUser, TicketLoadFilter filter)
    {
      SearchResults results = GetQuickSearchTicketResults(searchTerm, loginUser, filter);

      List<int> items = new List<int>();
      for (int i = 0; i < results.Count; i++)
      {
        results.GetNthDoc(i);
        items.Add(int.Parse(results.CurrentItem.Filename));
      }
      return items.ToArray();
    }

    public static List<SqlDataRecord> GetSearchResultsList(string searchTerm, LoginUser loginUser)
    {
      SqlMetaData recordIDColumn  = new SqlMetaData("recordID", SqlDbType.Int);
      SqlMetaData relevanceColumn = new SqlMetaData("relevance", SqlDbType.Int);

      SqlMetaData[] columns = new SqlMetaData[] { recordIDColumn, relevanceColumn };

      List<SqlDataRecord> result = new List<SqlDataRecord>();

      SearchResults results = GetSearchTicketResults(searchTerm, loginUser);

      List<int> items = new List<int>();
      for (int i = 0; i < results.Count; i++)
      {
        results.GetNthDoc(i);

        SqlDataRecord record = new SqlDataRecord(columns);
        record.SetInt32(0, int.Parse(results.CurrentItem.Filename));
        record.SetInt32(1, results.CurrentItem.ScorePercent);

        result.Add(record);
      }

      return result;
    } 

    private static void AddTicketParameter(string name, object value, bool isUnassignableInt, StringBuilder builder, SqlCommand command)
    {
        if (value != null)
        {
          if (isUnassignableInt && (int)value < 0)
          {
            builder.Append(" AND (tv." + name + " IS NULL OR tv." + name + " < 0)");
          }
          else          
          {
            builder.Append(string.Format(" AND (tv.{0} = @{0})", name));
            command.Parameters.AddWithValue(name, value);
          }
        }
    
    }

    private static void AddTicketParameter(string columnName, string paramName, DateTime? value, string op, StringBuilder builder, SqlCommand command)
    {
        if (value != null)
        {
          builder.Append(string.Format(" AND (tv.{0} {2} @{1})", columnName, paramName, op));
          command.Parameters.AddWithValue(paramName, value);
        }
    
    }

    public void LoadForZoho(int organizationID, DateTime? lastModified)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
            WITH TicketIDs 
            AS 
            (
              SELECT
                t.TicketID 
              FROM 
                Tickets t 
              WHERE
                OrganizationID = @OrganizationID 
                AND DateModified > @LastMod
            )              
            SELECT
              TicketNumber,
              'https://app.teamsupport.com?ticketid=' + CONVERT(VARCHAR,tv.TicketID) AS 'TicketURL',
              Name,
              TicketTypeName,
              TicketSource,
              Status,
              Severity,
              UserName AS 'AssignedTo',
              Customers,
              Contacts,
              ProductName,
              ReportedVersion,
              SolvedVersion,
              GroupName,
              DateModified,
              DateCreated,
              DaysOpened,
              IsClosed,
              CloserName,
              SlaViolationTime,
              StatusPosition,
              SeverityPosition,
              IsVisibleOnPortal,
              IsKnowledgeBase,
              DateClosed,
              DaysClosed,
              CreatorName,
              ModifierName,
              HoursSpent,
              Tags,
              SlaWarningTime,
              SlaViolationHours,
              SlaWarningHours,
              MinsSinceCreated,
              DaysSinceCreated,
              DATEDIFF(
                mi,
                '1900-01-01',
                (
                  SELECT
                    TOP 1 
                    timeinoldstatus 
                  FROM
                    statushistory
                  WHERE
                    ticketid = tv.ticketid
                    AND ISNULL(timeinoldstatus,-99) <> -99 
                  ORDER BY
                    statuschangetime
                )
              ) as MinutesToFirstResponse
            FROM
              TicketsView tv
              INNER JOIN TicketIDs
                ON TicketIDs.TicketID = tv.TicketID
            ORDER BY
              TicketNumber";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@LastMod", lastModified.HasValue ? lastModified.Value.AddMinutes(-15) : new DateTime(1900,1,1));
            Fill(command);
        }
    }

    public void LoadForTags(string tags)
    {
      string[] tagArray = tags.Split(',');

      StringBuilder builder = new StringBuilder(
@"SELECT tgv.[TicketID]
      ,tgv.[ProductName]
      ,tgv.[ReportedVersion]
      ,tgv.[SolvedVersion]
      ,tgv.[GroupName]
      ,tgv.[TicketTypeName]
      ,tgv.[UserName]
      ,tgv.[Status]
      ,tgv.[StatusPosition]
      ,tgv.[SeverityPosition]
      ,tgv.[IsClosed]
      ,tgv.[Severity]
      ,tgv.[TicketNumber]
      ,tgv.[IsVisibleOnPortal]
      ,tgv.[IsKnowledgeBase]
      ,tgv.[ReportedVersionID]
      ,tgv.[SolvedVersionID]
      ,tgv.[ProductID]
      ,tgv.[GroupID]
      ,tgv.[UserID]
      ,tgv.[TicketStatusID]
      ,tgv.[TicketTypeID]
      ,tgv.[TicketSeverityID]
      ,tgv.[OrganizationID]
      ,tgv.[Name]
      ,tgv.[ParentID]
      ,tgv.[ModifierID]
      ,tgv.[CreatorID]
      ,tgv.[DateModified]
      ,tgv.[DateCreated]
      ,tgv.[DateClosed]
      ,tgv.[CloserID]
      ,tgv.[DaysClosed]
      ,tgv.[DaysOpened]
      ,tgv.[CloserName]
      ,tgv.[CreatorName]
      ,tgv.[ModifierName]
      ,tgv.[SlaViolationTime]
      ,tgv.[SlaWarningTime]
      ,tgv.[SlaViolationHours]
      ,tgv.[SlaWarningHours]
FROM TicketGridView tgv 
WHERE tgv.OrganizationID = @OrganizationID"
);
      for (int i = 0; i < tagArray.Length; i++)
      {
        builder.Append(" AND EXISTS (SELECT * FROM TagLinksView WHERE TagLinksView.RefID=tgv.TicketID AND TagLinksView.RefType=17 AND TagLinksView.Value = @Value" + i.ToString() + ")");
      }

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = builder.ToString();
        UseCache = false;
        CacheExpirationSeconds = 300;

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        for (int i = 0; i < tagArray.Length; i++)
        {
          command.Parameters.AddWithValue("@Value" + i.ToString(), tagArray[i]);
        }

        Fill(command, "TicketTags");
      }


    }

    public int LoadForGridCount(int organizationID, int ticketTypeID, int ticketStatusID, int ticketSeverityID,
      int userID, int groupID, int productID, int reportedVersionID, int resolvedVersionID,
      int customerID, bool? onlyPortal, bool? onlyKnowledgeBase,
      DateTime? dateCreatedBegin, DateTime? dateCreatedEnd, DateTime? dateModifiedBegin, DateTime? dateModifiedEnd,
      string search)
    {
      if (search.Trim() == "") search = "\"\"";
      using (SqlCommand command = new SqlCommand())
      {
        StringBuilder builder = new StringBuilder();
        builder.Append(@" SELECT COUNT(*)
                          FROM dbo.TicketGridView tgv LEFT JOIN Tickets t ON tgv.TicketID = t.TicketID
                          WHERE (tgv.OrganizationID = @OrganizationID)
                          AND ((tgv.TicketTypeID = @TicketTypeID) OR (@TicketTypeID = -1))
                          AND ((tgv.TicketSeverityID = @TicketSeverityID) OR (@TicketSeverityID = -1))
                          AND ((tgv.ProductID = @ProductID) OR (@ProductID = -1))
                          AND ((tgv.ReportedVersionID = @ReportedVersionID) OR (@ReportedVersionID = -1))
                          AND ((tgv.SolvedVersionID = @ResolvedVersionID) OR (@ResolvedVersionID = -1))
                          AND (((@UserID = -2) AND (tgv.UserID IS NULL)) OR (@UserID = tgv.UserID) OR (@UserID = -1))
                          AND (((@GroupID = -2) AND (tgv.GroupID IS NULL)) OR (@GroupID = tgv.GroupID) OR (@GroupID = -1))
                          AND ((tgv.TicketStatusID = @TicketStatusID) OR (@TicketStatusID = -1) OR ((@TicketStatusID = -3) AND (tgv.IsClosed = 0)) OR ((@TicketStatusID = -4) AND (tgv.IsClosed = 1)))
                          AND ((@CustomerID = -1) OR (EXISTS(SELECT * FROM OrganizationTickets ot WHERE (ot.OrganizationID = @CustomerID) AND (ot.TicketID = tgv.TicketID))))
                          AND ((@IsPortal is null) OR (tgv.IsVisibleOnPortal = @IsPortal))
                          AND ((@IsKnowledgeBase is null) OR (tgv.IsKnowledgeBase = @IsKnowledgeBase))
                          AND ((@DateCreatedBegin is null) OR (tgv.DateCreated >= @DateCreatedBegin))
                          AND ((@DateCreatedEnd is null) OR (tgv.DateCreated <= @DateCreatedEnd))
                          AND ((@DateModifiedBegin is null) OR (tgv.DateModified >= @DateModifiedBegin))
                          AND ((@DateModifiedEnd is null) OR (tgv.DateModified <= @DateModifiedEnd))
                          AND ((@Search = '""""') 
                                OR (CONTAINS((t.[Name]), @Search))
                                OR EXISTS (SELECT * FROM Actions a WHERE (a.TicketID = tgv.TicketID) AND CONTAINS((a.[Description], a.[Name]), @Search))
                                OR EXISTS (SELECT * FROM CustomValues cv 
                                        LEFT JOIN CustomFields cf ON cv.CustomFieldID = cf.CustomFieldID 
                                        WHERE (cf.RefType = 17)
                                        AND (cv.RefID = tgv.TicketID)
                                        AND CONTAINS((cv.[CustomValue]), @Search))
                                OR (tgv.TicketNumber LIKE '%'+@SearchClean+'%'))
                        ");

        command.CommandText = builder.ToString();
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
        command.Parameters.AddWithValue("@TicketStatusID", ticketStatusID);
        command.Parameters.AddWithValue("@TicketSeverityID", ticketSeverityID);
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@GroupID", groupID);
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@ReportedVersionID", reportedVersionID);
        command.Parameters.AddWithValue("@ResolvedVersionID", resolvedVersionID);
        command.Parameters.AddWithValue("@CustomerID", customerID);
        command.Parameters.AddWithValue("@IsPortal", onlyPortal == null ? (object)DBNull.Value : onlyPortal);
        command.Parameters.AddWithValue("@IsKnowledgeBase", onlyKnowledgeBase == null ? (object)DBNull.Value : onlyKnowledgeBase);
        command.Parameters.AddWithValue("@DateCreatedBegin", dateCreatedBegin == null ? (object)DBNull.Value : dateCreatedBegin);
        command.Parameters.AddWithValue("@DateCreatedEnd", dateCreatedEnd == null ? (object)DBNull.Value : dateCreatedEnd);
        command.Parameters.AddWithValue("@DateModifiedBegin", dateModifiedBegin == null ? (object)DBNull.Value : dateModifiedBegin);
        command.Parameters.AddWithValue("@DateModifiedEnd", dateModifiedEnd == null ? (object)DBNull.Value : dateModifiedEnd);
        command.Parameters.AddWithValue("@Search", search);
        command.Parameters.AddWithValue("@SearchClean", search.Replace("*", "").Replace("%", "").Replace("\"", ""));

        return (int)ExecuteScalar(command, "TicketGridView,Actions");
      }
    }


    public void LoadForGrid(int pageIndex, int pageSize, int organizationID, int ticketTypeID, int ticketStatusID, int ticketSeverityID,
      int userID, int groupID, int productID, int reportedVersionID, int resolvedVersionID,
      int customerID, bool? onlyPortal, bool? onlyKnowledgeBase,
      DateTime? dateCreatedBegin, DateTime? dateCreatedEnd, DateTime? dateModifiedBegin, DateTime? dateModifiedEnd,
      string search, string sortColumn, bool sortAsc)
    {
      if (search.Trim() == "") search = @"""""";

      using (SqlCommand command = new SqlCommand())
      {
        string sort = sortColumn;
        switch (sortColumn)
        {
          case "Severity": sort = "SeverityPosition"; break;
          case "Status": sort = "StatusPosition"; break;
          default: break;
        }
        StringBuilder builder = new StringBuilder();
        builder.Append("WITH TicketRows AS (SELECT ROW_NUMBER() OVER (ORDER BY tgv.");
        builder.Append(sort);
        if (sortAsc) builder.Append(" ASC");
        else builder.Append(" DESC");
        builder.Append(") AS RowNumber, tgv.*");
        builder.Append(@" 
                              	  
                                  
                                  FROM TicketGridView tgv LEFT JOIN Tickets t ON tgv.TicketID = t.TicketID
                                  WHERE (tgv.OrganizationID = @OrganizationID)
                                  AND ((tgv.TicketTypeID = @TicketTypeID) OR (@TicketTypeID = -1))
                                  AND ((tgv.TicketSeverityID = @TicketSeverityID) OR (@TicketSeverityID = -1))
                                  AND ((tgv.ProductID = @ProductID) OR (@ProductID = -1))
                                  AND ((tgv.ReportedVersionID = @ReportedVersionID) OR (@ReportedVersionID = -1))
                                  AND ((tgv.SolvedVersionID = @ResolvedVersionID) OR (@ResolvedVersionID = -1))
                                  AND (((@UserID = -2) AND (tgv.UserID IS NULL)) OR (@UserID = tgv.UserID) OR (@UserID = -1))
                                  AND (((@GroupID = -2) AND (tgv.GroupID IS NULL)) OR (@GroupID = tgv.GroupID) OR (@GroupID = -1))
                                  AND ((tgv.TicketStatusID = @TicketStatusID) OR (@TicketStatusID = -1) OR ((@TicketStatusID = -3) AND (tgv.IsClosed = 0)) OR ((@TicketStatusID = -4) AND (tgv.IsClosed = 1)))
                                  AND ((@CustomerID = -1) OR (EXISTS(SELECT * FROM OrganizationTickets ot WHERE (ot.OrganizationID = @CustomerID) AND (ot.TicketID = tgv.TicketID))))
                                  AND ((@IsPortal is null) OR (tgv.IsVisibleOnPortal = @IsPortal))
                                  AND ((@IsKnowledgeBase is null) OR (tgv.IsKnowledgeBase = @IsKnowledgeBase))
                                  AND ((@DateCreatedBegin is null) OR (tgv.DateCreated >= @DateCreatedBegin))
                                  AND ((@DateCreatedEnd is null) OR (tgv.DateCreated <= @DateCreatedEnd))
                                  AND ((@DateModifiedBegin is null) OR (tgv.DateModified >= @DateModifiedBegin))
                                  AND ((@DateModifiedEnd is null) OR (tgv.DateModified <= @DateModifiedEnd))
                             
                                )
                              	  
                                  SELECT * FROM TicketRows 
                                  WHERE RowNumber BETWEEN @PageIndex*@PageSize+1 AND @PageIndex*@PageSize+@PageSize
                                  ORDER BY RowNumber ASC");

        command.CommandText = builder.ToString();
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@PageIndex", pageIndex);
        command.Parameters.AddWithValue("@PageSize", pageSize);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
        command.Parameters.AddWithValue("@TicketStatusID", ticketStatusID);
        command.Parameters.AddWithValue("@TicketSeverityID", ticketSeverityID);
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@GroupID", groupID);
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@ReportedVersionID", reportedVersionID);
        command.Parameters.AddWithValue("@ResolvedVersionID", resolvedVersionID);
        command.Parameters.AddWithValue("@CustomerID", customerID);
        command.Parameters.AddWithValue("@IsPortal", onlyPortal == null ? (object)DBNull.Value : onlyPortal);
        command.Parameters.AddWithValue("@IsKnowledgeBase", onlyKnowledgeBase == null ? (object)DBNull.Value : onlyKnowledgeBase);
        command.Parameters.AddWithValue("@DateCreatedBegin", dateCreatedBegin == null ? (object)DBNull.Value : dateCreatedBegin);
        command.Parameters.AddWithValue("@DateCreatedEnd", dateCreatedEnd == null ? (object)DBNull.Value : dateCreatedEnd);
        command.Parameters.AddWithValue("@DateModifiedBegin", dateModifiedBegin == null ? (object)DBNull.Value : dateModifiedBegin);
        command.Parameters.AddWithValue("@DateModifiedEnd", dateModifiedEnd == null ? (object)DBNull.Value : dateModifiedEnd);
        command.Parameters.AddWithValue("@Search", search);
        command.Parameters.AddWithValue("@SearchClean", search.Replace("*", "").Replace("%", "").Replace("\"", ""));


        Fill(command, "TicketGridView,Actions");
      }

      /* using (SqlCommand command = new SqlCommand())
       {
         command.CommandText = "uspSelectTicketPage";
         command.CommandType = CommandType.StoredProcedure;
         command.Parameters.AddWithValue("@PageIndex", pageIndex);
         command.Parameters.AddWithValue("@PageSize", pageSize);
         command.Parameters.AddWithValue("@OrganizationID", organizationID);
         command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
         command.Parameters.AddWithValue("@TicketStatusID", ticketStatusID);
         command.Parameters.AddWithValue("@TicketSeverityID", ticketSeverityID);
         command.Parameters.AddWithValue("@UserID", userID);
         command.Parameters.AddWithValue("@GroupID", groupID);
         command.Parameters.AddWithValue("@ProductID", productID);
         command.Parameters.AddWithValue("@ReportedVersionID", reportedVersionID);
         command.Parameters.AddWithValue("@ResolvedVersionID", resolvedVersionID);
         command.Parameters.AddWithValue("@CustomerID", customerID);
         command.Parameters.AddWithValue("@IsPortal", onlyPortal == null ? (object)DBNull.Value : onlyPortal);
         command.Parameters.AddWithValue("@IsKnowledgeBase", onlyKnowledgeBase == null ? (object)DBNull.Value : onlyKnowledgeBase);
         command.Parameters.AddWithValue("@DateCreatedBegin", dateCreatedBegin == null ? (object)DBNull.Value : dateCreatedBegin);
         command.Parameters.AddWithValue("@DateCreatedEnd", dateCreatedEnd == null ? (object)DBNull.Value : dateCreatedEnd);
         command.Parameters.AddWithValue("@DateModifiedBegin", dateModifiedBegin == null ? (object)DBNull.Value : dateModifiedBegin);
         command.Parameters.AddWithValue("@DateModifiedEnd", dateModifiedEnd == null ? (object)DBNull.Value : dateModifiedEnd);
         command.Parameters.AddWithValue("@Search", search);
         command.Parameters.AddWithValue("@SortColumn", "Name");
         command.Parameters.AddWithValue("@SortAsc", false);

         Fill(command, "TicketGridView,Actions");
       }*/
    }

    public static string GetSearchResultsWhereClause(LoginUser loginUser)
    {
      StringBuilder resultBuilder = new StringBuilder();

      SearchStandardFilters searchStandardFilters = new SearchStandardFilters(loginUser);
      searchStandardFilters.LoadByUserID(loginUser.UserID);
      resultBuilder.Append(searchStandardFilters.ConvertToWhereClause());

      SearchCustomFilters searchCustomFilters = new SearchCustomFilters(loginUser);
      searchCustomFilters.LoadByUserID(loginUser.UserID);
      resultBuilder.Append(searchCustomFilters.ConvertToWhereClause());

      return resultBuilder.ToString();
    }

    public void LoadModifiedByCRMLinkItem(CRMLinkTableItem item)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
        @"
          SELECT 
            t.* 
          FROM 
            TicketsView t
            LEFT JOIN TicketStatuses ts
              ON t.TicketStatusID = ts.TicketStatusID
          WHERE 
            t.OrganizationID = @OrgID 
            AND 
            (
              (
                @DateModified > '1753-01-01'
                AND t.DateModified > @DateModified
              )
              OR
              (
                @DateModified <= '1753-01-01'
                AND ts.IsClosed = 0
              )
            )
            AND 
            (
              t.DateModifiedBySalesForceSync IS NULL
              OR t.DateModified > DATEADD(s, 2, t.DateModifiedBySalesForceSync)
            )
          ORDER BY 
            t.DateCreated DESC
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrgID", item.OrganizationID);
        command.Parameters.AddWithValue("@DateModified", item.LastLink == null ? new DateTime(1753, 1, 1) : item.LastLinkUtc.Value.AddHours(-1));

        using (SqlConnection connection = new SqlConnection(this.LoginUser.ConnectionString))
        {
          connection.Open();
          SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);

          command.Connection = connection;
          command.Transaction = transaction;
          SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
          this.Table.Load(reader);
        }
      }
    }

    public TicketsViewItem FindBySalesForceID(string salesForceID)
    {
      foreach (TicketsViewItem ticketsViewItem in this)
      {
        if (ticketsViewItem.SalesForceID != null && ticketsViewItem.SalesForceID.Trim().ToLower() == salesForceID.Trim().ToLower())
        {
          return ticketsViewItem;
        }
      }
      return null;
    }

    //Changes to this method needs to be applied to TicketLinkToJira.LoadToPushToJira also.
    public void LoadToPushToJira(CRMLinkTableItem item)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
        @"
          SELECT 
            t.* 
          FROM 
            TicketsView t
            JOIN TicketLinkToJira j
              ON t.TicketID = j.TicketID
          WHERE 
            j.SyncWithJira = 1
            AND t.OrganizationID = @OrgID 
            AND 
            (
              j.DateModifiedByJiraSync IS NULL
              OR 
              (
                t.DateModified > DATEADD(s, 2, j.DateModifiedByJiraSync)
                AND t.DateModified > @DateModified
              )
            )
          ORDER BY 
            t.DateCreated DESC
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrgID", item.OrganizationID);
        command.Parameters.AddWithValue("@DateModified", item.LastLink == null ? new DateTime(1753, 1, 1) : item.LastLinkUtc.Value.AddHours(-1));

        Fill(command);
      }
    }

    public void LoadForGridPointSalesOrders(CRMLinkTableItem item, string query)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = query;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrgID", item.OrganizationID);
        command.Parameters.AddWithValue("@DateModified", item.LastLink == null ? new DateTime(1753, 1, 1) : item.LastLinkUtc.Value.AddHours(-1));

        Fill(command, "TicketsView");
      }
    }

    /// <summary>
    /// Loads tickets that are associated with a asset's assetid
    /// </summary>
    /// <param name="assetID"></param>
    public void LoadByAssetID(int assetID, string orderBy = "AssetID")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT tv.* FROM TicketsView tv JOIN AssetTickets at ON at.TicketID = tv.TicketID WHERE at.AssetID = @AssetID ORDER BY " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@AssetID", assetID);
        Fill(command);
      }
    }

    public static TicketsViewItem GetTicketsViewItemByIdOrNumber(LoginUser loginUser, int ticketIDOrTicketNumber)
    {
      TicketsView ticketsView = new TicketsView(loginUser);
      ticketsView.LoadByTicketIDAndOrganizationID(loginUser.OrganizationID, ticketIDOrTicketNumber);
      if (ticketsView.IsEmpty)
      {
        ticketsView = new TicketsView(loginUser);
        ticketsView.LoadByTicketNumberFromTicketsView(ticketIDOrTicketNumber, loginUser.OrganizationID);
        if (ticketsView.IsEmpty)
          return null;
        else
          return ticketsView[0];
      }
      else
        return ticketsView[0];
    }

    public void LoadNewCustomerResponded(LoginUser loginUser, int lastStatusHistoryID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
SELECT * FROM TicketsView WHERE TicketID IN
(
  SELECT DISTINCT(sh.TicketID)
  FROM StatusHistory sh
  INNER JOIN TicketStatuses ts ON ts.TicketStatusID = sh.NewStatus
  WHERE ts.OrganizationID = 1078
  AND ts.IsEmailResponse = 1
  AND sh.StatusHistoryID > @StatusHistoryID
)
";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@StatusHistoryID", lastStatusHistoryID);
        Fill(command);
      }
    }

    public void LoadNewUrgentTickets(LoginUser loginUser, int lastStatusHistoryID)
    {
    }
  } 
}
