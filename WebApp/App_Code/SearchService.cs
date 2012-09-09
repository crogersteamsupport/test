using Microsoft.SqlServer.Server;
using dtSearch.Engine;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Script.Services;
using System.Web.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;

namespace TSWebServices
{
  [ScriptService]
  [WebService(Namespace = "http://teamsupport.com/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class SearchService : System.Web.Services.WebService
  {
    public SearchService()
    {

    }

    // The fields available to filter are the TicketsView fields.
    // Nevertheless some of these fields also exists in the WikiArticlesView (e.g. ParentID)
    // If a field that does not exists in or have no equivalent in the WikiArticlesView is selected as a filter,
    // the wiki index will not be included in the search and only tickets results will be displayed.
    [WebMethod]
    public SearchResultsProxy GetSearchResults(string searchTerm, int firstItemIndex, int pageSize)
    {
      SearchResultsProxy result = new SearchResultsProxy();
      List<SearchResultsItemProxy> resultItems = new List<SearchResultsItemProxy>();

      LoginUser loginUser = TSAuthentication.GetLoginUser();

      SearchStandardFilter searchStandardFilter = GetSearchStandardFilter(loginUser);

      bool hasTicketsOnlyFilters = false;

      bool hasIndexesToSearch = GetHasIndexesToSearch(searchStandardFilter, loginUser, ref hasTicketsOnlyFilters);

      if (hasIndexesToSearch)
      {
        string tempItemsTableFieldsDefinition = string.Empty;
        string tempItemsTableFields           = string.Empty;
        string ticketsQuery                   = string.Empty;
        string wikisQuery                     = string.Empty;
        string orderByClause                  = string.Empty;
        string fieldsList                     = string.Empty;
        string selectTicketsFields            = string.Empty;
        string selectWikisFields              = string.Empty;

        orderByClause = GetOrderByClause(loginUser,
                                         ref tempItemsTableFieldsDefinition,
                                         ref tempItemsTableFields,
                                         ref selectTicketsFields,
                                         ref selectWikisFields);

        SqlCommand command = new SqlCommand();

        if (searchStandardFilter.Tickets || searchStandardFilter.KnowledgeBase)
        {
          List<SqlDataRecord> dtSearchTicketsResultsList = TicketsView.GetSearchResultsList(searchTerm, loginUser);

          if (dtSearchTicketsResultsList.Count > 0)
          {
            ticketsQuery = GetTicketsQuery(searchTerm, loginUser, selectTicketsFields);

            SqlParameter dtSearchTicketsResultsTable = new SqlParameter("@dtSearchTicketsResultsTable", SqlDbType.Structured);
            dtSearchTicketsResultsTable.TypeName = "dtSearch_results_tbltype";
            dtSearchTicketsResultsTable.Value = dtSearchTicketsResultsList;
            command.Parameters.Add(dtSearchTicketsResultsTable);
          }
        }

        if (searchStandardFilter.Wikis && !hasTicketsOnlyFilters)
        {
          List<SqlDataRecord> dtSearchWikisResultsList = WikiArticlesView.GetSearchResultsList(searchTerm, loginUser);

          if (dtSearchWikisResultsList.Count > 0)
          {
            bool includesTickets = false;
            if (ticketsQuery.Length > 0)
            {
              includesTickets = true;
            }

            wikisQuery = GetWikisQuery(includesTickets, loginUser, selectWikisFields);

            SqlParameter dtSearchWikisResultsTable = new SqlParameter("@dtSearchWikisResultsTable", SqlDbType.Structured);
            dtSearchWikisResultsTable.TypeName = "dtSearch_results_tbltype";
            dtSearchWikisResultsTable.Value = dtSearchWikisResultsList;
            command.Parameters.Add(dtSearchWikisResultsTable);
          }
        }

        fieldsList    = GetFieldsList();

        string query = @"
        DECLARE @TempItems 
        TABLE
        ( 
          ID            int IDENTITY, 
          recordID      int, 
          source        int, 
          relevance     int, 
          DateModified  datetime 
          {0}
        )

        INSERT INTO @TempItems 
        (
          recordID, 
          source, 
          relevance, 
          DateModified 
          {1}
        ) 
        {2}
        {3}
        {4}
        
        SET @resultsCount = @@RowCount

        SELECT
          {5}
        FROM 
          @TempItems ti 
          LEFT JOIN dbo.TicketsView tv 
            ON ti.source = 17
            AND ti.recordID = tv.TicketID
          LEFT JOIN dbo.WikiArticlesView wv
            ON ti.source = 39
            AND ti.recordID = wv.ArticleID
        WHERE 
          ti.ID BETWEEN @FromIndex AND @toIndex
        ORDER BY 
          ti.ID
        ";

        command.CommandText = string.Format(query, tempItemsTableFieldsDefinition, tempItemsTableFields, ticketsQuery, wikisQuery, orderByClause, fieldsList);
        command.CommandType = CommandType.Text;

        SqlParameter resultsCount = new SqlParameter("@resultsCount", SqlDbType.Int)
        {
          Direction = ParameterDirection.Output
        };
        command.Parameters.Add(resultsCount);

        command.Parameters.AddWithValue("@FromIndex", firstItemIndex + 1);
        command.Parameters.AddWithValue("@ToIndex",   firstItemIndex + pageSize);

        DataTable table = new DataTable();
        using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
        {
          command.Connection = connection;
          connection.Open();
          using (SqlDataAdapter adapter = new SqlDataAdapter(command))
          {
            try
            {
              adapter.Fill(table);
              if (resultsCount.Value != DBNull.Value)
              {
                result.Count = (int)resultsCount.Value;
              }
            }
            catch (Exception e)
            {

            }
          }
        }

        foreach (DataRow row in table.Rows)
        {
          SearchResultsItemProxy resultItem = new SearchResultsItemProxy();

          resultItem.ScorePercent = (int)row["relevance"];

          switch ((ReferenceType)row["source"])
          {
            case ReferenceType.Tickets:
              resultItem.ID = (int)row["TicketID"];
              resultItem.DisplayName = string.Format("{0}: {1}", row["TicketNumber"].ToString(), row["Name"].ToString());
              resultItem.Number = (int)row["TicketNumber"];
              resultItem.Status = row["Status"].ToString();
              resultItem.Severity = row["Severity"].ToString();

              if ((bool)row["IsKnowledgeBase"])
              {
                resultItem.TypeID = 2;
              }
              else
              {
                resultItem.TypeID = 1;
              }

              break;
            case ReferenceType.Wikis:
              resultItem.ID = (int)row["ArticleID"];
              resultItem.DisplayName = row["ArticleName"].ToString();
              resultItem.Creator = row["Creator"].ToString();
              resultItem.Modifier = row["Modifier"].ToString();
              resultItem.TypeID = 3;

              break;
          }

          resultItems.Add(resultItem);
        }
      }

      result.Items = resultItems.ToArray();

      return result;
    }

    private SearchStandardFilter GetSearchStandardFilter(LoginUser loginUser)
    {
      SearchStandardFilter result = null;

      SearchStandardFilters searchStandardFilters = new SearchStandardFilters(loginUser);
      searchStandardFilters.LoadByUserID(loginUser.UserID);

      if (searchStandardFilters.Count == 0)
      {
        // Calling AddNewSearchStandardFilter after LoadByUserID generates a 'UserID doesn't allow null' exception.
        searchStandardFilters = new SearchStandardFilters(loginUser);
        result = searchStandardFilters.AddNewSearchStandardFilter();
        result.Tickets = true;
        result.KnowledgeBase = true;
        result.Wikis = true;
      }
      else
      {
        result = searchStandardFilters[0];
      }

      return result;
    }

    private bool GetHasIndexesToSearch(SearchStandardFilter searchStandardFilter, LoginUser loginUser, ref bool hasTicketsOnlyFilters)
    {
      bool result = true;

      if (!searchStandardFilter.Tickets && !searchStandardFilter.KnowledgeBase && !searchStandardFilter.Wikis)
      {
        result = false;
      }
      else
      {
        if (searchStandardFilter.Wikis)
        {
          hasTicketsOnlyFilters = GetHasTicketsOnlyFilters(loginUser);
          if (!searchStandardFilter.Tickets && !searchStandardFilter.KnowledgeBase && hasTicketsOnlyFilters)
          {
            result = false;
          }
        }
      }

      return result;
    }

    private string GetTicketsQuery(string searchTerm, LoginUser loginUser, string selectTicketsFields)
    {
      string result = @"
        SELECT
          tv.TicketID
          , 17
          , dtrt.relevance
          , tv.DateModified
          {0}
        FROM
          dbo.TicketsView tv
          JOIN @dtSearchTicketsResultsTable dtrt
            ON tv.TicketID = dtrt.recordID
	        {1}
        WHERE
          1 = 1
          {2}
      ";

      string joinClause = GetJoinClause(loginUser);

      string whereClause = TicketsView.GetSearchResultsWhereClause(loginUser);

      return string.Format(result, selectTicketsFields, joinClause, whereClause);
    }

    private string GetJoinClause(LoginUser loginUser)
    {
      SearchCustomFilters searchCustomFilters = new SearchCustomFilters(loginUser);
      searchCustomFilters.LoadByUserID(loginUser.UserID);
      return searchCustomFilters.GetJoinClause();
    }

    private string GetWikisQuery(bool includeTickets, LoginUser loginUser, string selectWikisFields)
    {
      StringBuilder resultBuilder = new StringBuilder();

      if (includeTickets)
      {
        resultBuilder.Append(" UNION ");
      }

      resultBuilder.Append(@"
        SELECT
          wav.ArticleID
          , 39
          , dwrt.relevance
          , wav.ModifiedDate AS DateModified");
      resultBuilder.Append(selectWikisFields);

      resultBuilder.Append(@"
        FROM
          dbo.WikiArticlesView wav
          JOIN @dtSearchWikisResultsTable dwrt
            ON wav.ArticleID = dwrt.RecordID
        WHERE
          1 = 1
      ");

      resultBuilder.Append(WikiArticlesView.GetSearchResultsWhereClause(loginUser));

      return resultBuilder.ToString();
    }

    private string GetOrderByClause(LoginUser loginUser, 
                                    ref string tempItemsTableFieldsDefinition, 
                                    ref string tempItemsTableFields,
                                    ref string selectTicketsFields,
                                    ref string selectWikisFields)
    {
      SearchSorters searchSorters = new SearchSorters(loginUser);
      searchSorters.LoadByUserID(loginUser.UserID);
      return searchSorters.ConvertToOrderByClause(ref tempItemsTableFieldsDefinition, 
                                                  ref tempItemsTableFields,
                                                  ref selectTicketsFields,
                                                  ref selectWikisFields);
    }

    private string GetFieldsList()
    {
      return @"
        ti.ID
        , ti.source
        , ti.relevance
        , tv.TicketID
        , tv.TicketNumber
        , tv.Name
        , tv.Status
        , tv.Severity
        , tv.IsKnowledgeBase
        , wv.ArticleID
        , wv.ArticleName
        , wv.Creator
        , wv.Modifier
      ";
    }

    private bool GetHasTicketsOnlyFilters(LoginUser loginUser)
    {
      bool result = false;

      WikiArticlesView wikiArticleViewFields = new WikiArticlesView(loginUser);
      wikiArticleViewFields.LoadColumnNames();

      ReportTableFields ticketsViewFields = new ReportTableFields(loginUser);
      ticketsViewFields.LoadByReportTableID(10);

      SearchCustomFilters filters = new SearchCustomFilters(loginUser);
      filters.LoadByUserID(loginUser.UserID);
      foreach (SearchCustomFilter filter in filters)
      {
        string fieldName = ticketsViewFields.FindByReportTableFieldID(filter.FieldID).FieldName;
        string wikiEquivalentFieldName = DataUtils.GetWikiEquivalentFieldName(fieldName);
        if (!DataUtils.GetIsColumnInBaseCollection(wikiArticleViewFields, wikiEquivalentFieldName))
        {
          result = true;
          break;
        }
      }

      return result;
    }

    [WebMethod]
    public int AddStandardFilters(bool includeTickets, bool includeKnowledgeBase, bool includeWikis)
    {
      SearchStandardFilters filters = new SearchStandardFilters(TSAuthentication.GetLoginUser());
      SearchStandardFilter filter = filters.AddNewSearchStandardFilter();

      filter.UserID         = filters.LoginUser.UserID;
      filter.Tickets        = includeTickets;
      filter.KnowledgeBase  = includeKnowledgeBase;
      filter.Wikis          = includeWikis;

      filters.Save();

      return filter.StandardFilterID;
    }

    [WebMethod]
    public void UpdateStandardFilters(int standardFilterID, bool includeTickets, bool includeKnowledgeBase, bool includeWikis)
    {
      SearchStandardFilters filters = new SearchStandardFilters(TSAuthentication.GetLoginUser());
      filters.LoadByStandardFilterID((int)standardFilterID);
      SearchStandardFilter filter = filters.FindByStandardFilterID((int)standardFilterID);

      filter.Tickets        = includeTickets;
      filter.KnowledgeBase  = includeKnowledgeBase;
      filter.Wikis          = includeWikis;

      filters.Save();
    }

    [WebMethod]
    public int AddCustomFilter(int fieldID, string measure, string value, bool isCustom)
    {
      SearchCustomFilters filters = new SearchCustomFilters(TSAuthentication.GetLoginUser());
      SearchCustomFilter filter = filters.AddNewSearchCustomFilter();

      filter.TableID    = isCustom ? -1 : 10;
      filter.FieldID    = fieldID;
      filter.Measure    = measure;
      filter.TestValue  = value;
      filter.UserID     = filters.LoginUser.UserID;
      filter.MatchAll   = true;

      filters.Save();

      return (int)filters.Table.Rows[0]["CustomFilterID"];
    }

    [WebMethod]
    public void UpdateCustomFilter(int customFilterID, int fieldID, string measure, string value, bool isCustom)
    {
      SearchCustomFilters filters = new SearchCustomFilters(TSAuthentication.GetLoginUser());
      filters.LoadByCustomFilterID(customFilterID);
      SearchCustomFilter filter = filters.FindByCustomFilterID(customFilterID);

      filter.TableID    = isCustom ? -1 : 10;
      filter.FieldID    = fieldID;
      filter.Measure    = measure;
      filter.TestValue  = value;

      filters.Save();
    }

    [WebMethod]
    public void DeleteCustomFilter(int filterID)
    {
      SearchCustomFilters filters = new SearchCustomFilters(TSAuthentication.GetLoginUser());
      filters.DeleteFromDB(filterID);      
    }

    [WebMethod]
    public int AddSorter(int fieldID, bool descending, bool isCustom)
    {
      SearchSorters sorters = new SearchSorters(TSAuthentication.GetLoginUser());
      SearchSorter sorter = sorters.AddNewSearchSorter();

      sorter.TableID    = isCustom ? -1 : 10;
      sorter.FieldID    = fieldID;
      sorter.Descending = descending;
      sorter.UserID     = sorters.LoginUser.UserID;

      sorters.Save();

      return (int)sorters.Table.Rows[0]["SorterID"];
    }

    [WebMethod]
    public void UpdateSorter(int sorterID, int fieldID, bool descending, bool isCustom)
    {
      SearchSorters sorters = new SearchSorters(TSAuthentication.GetLoginUser());
      sorters.LoadBySorterID(sorterID);
      SearchSorter sorter = sorters.FindBySorterID(sorterID);

      sorter.TableID    = isCustom ? -1 : 10;
      sorter.FieldID    = fieldID;
      sorter.Descending = descending;

      sorters.Save();
    }

    [WebMethod]
    public void DeleteSorter(int sorterID)
    {
      SearchSorters sorters = new SearchSorters(TSAuthentication.GetLoginUser());
      sorters.DeleteFromDB(sorterID);
    }

    [WebMethod(true)]
    public AdvancedSearchOptions GetAdvancedSearchOptions()
    {
      AdvancedSearchOptions result = new AdvancedSearchOptions();

      LoginUser loginUser = TSAuthentication.GetLoginUser();

      // GetStandardFilters
      SearchStandardFilters standardFilters = new SearchStandardFilters(loginUser);
      standardFilters.LoadByUserID(loginUser.UserID);
      if (standardFilters.Table.Rows.Count > 0)
      {
        result.StandardFilterID = (int)standardFilters.Table.Rows[0]["StandardFilterID"];
        result.Tickets          = (bool)standardFilters.Table.Rows[0]["Tickets"];
        result.KnowledgeBase    = (bool)standardFilters.Table.Rows[0]["KnowledgeBase"];
        result.Wikis            = (bool)standardFilters.Table.Rows[0]["Wikis"];
      }

      // GetFields
      List<AutoFieldItem> fieldItems = new List<AutoFieldItem>();
      ReportTableFields fields = new ReportTableFields(loginUser);
      fields.LoadByReportTableID(10);

      CustomFields customs = new CustomFields(fields.LoginUser);
      customs.LoadByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Tickets);

      foreach (ReportTableField field in fields)
      {
        fieldItems.Add(new AutoFieldItem(field));
      }

      foreach (CustomField custom in customs)
      {
        fieldItems.Add(new AutoFieldItem(custom));
      }

      result.Fields = fieldItems.ToArray();

      // GetCustomFilters
      SearchCustomFilters customFilters = new SearchCustomFilters(loginUser);
      customFilters.LoadByUserID(loginUser.UserID);
      result.CustomFilters = customFilters.GetSearchCustomFilterProxies();

      // GetCustomSorters
      SearchSorters sorters = new SearchSorters(loginUser);
      sorters.LoadByUserID(loginUser.UserID);
      result.Sorters = sorters.GetSearchSorterProxies();

      return result;
    }

  }

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class AdvancedSearchOptions
  {
    [DataMember]
    public int? StandardFilterID { get; set; }
    [DataMember]
    public bool? Tickets { get; set; }
    [DataMember]
    public bool? KnowledgeBase { get; set; }
    [DataMember]
    public bool? Wikis { get; set; }
    [DataMember]
    public AutoFieldItem[] Fields { get; set; }
    [DataMember]
    public SearchCustomFilterProxy[] CustomFilters { get; set; }
    [DataMember]
    public SearchSorterProxy[] Sorters { get; set; }
  }
}