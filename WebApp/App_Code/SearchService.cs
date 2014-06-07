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
using Newtonsoft.Json;
using System.Text.RegularExpressions;

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

      bool hasNonWikiFilters = false;
      bool hasNonNotesFilters = false;
      bool hasNonProductVersionsFilters = false;
      bool hasNonWaterCoolerFilters =  false;

      bool hasIndexesToSearch = GetHasIndexesToSearch(
                                  searchStandardFilter, 
                                  loginUser, 
                                  ref hasNonWikiFilters,
                                  ref hasNonNotesFilters,
                                  ref hasNonProductVersionsFilters,
                                  ref hasNonWaterCoolerFilters);

      if (hasIndexesToSearch)
      {
        string tempItemsTableFieldsDefinition = string.Empty;
        string tempItemsTableFields           = string.Empty;

        string ticketsQuery                   = string.Empty;
        string wikisQuery                     = string.Empty;
        string notesQuery                     = string.Empty;
        string productVersionsQuery           = string.Empty;
        string waterCoolerQuery               = string.Empty;

        string orderByClause                  = string.Empty;

        string fieldsList                     = string.Empty;

        string selectTicketsFields            = string.Empty;
        string selectWikisFields              = string.Empty;
        string selectNotesFields              = string.Empty;
        string selectProductVersionsFields    = string.Empty;
        string selectWaterCoolerFields        = string.Empty;

        orderByClause = GetOrderByClause(loginUser,
                                         ref tempItemsTableFieldsDefinition,
                                         ref tempItemsTableFields,
                                         ref selectTicketsFields,
                                         ref selectWikisFields,
                                         ref selectNotesFields,
                                         ref selectProductVersionsFields,
                                         ref selectWaterCoolerFields);

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

        if (searchStandardFilter.Wikis && !hasNonWikiFilters)
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

        if (searchStandardFilter.Notes && !hasNonNotesFilters)
        {
          List<SqlDataRecord> dtSearchNotesResultsList = NotesView.GetSearchResultsList(searchTerm, loginUser);

          if (dtSearchNotesResultsList.Count > 0)
          {
            bool includesPreviousQuery = false;
            if (ticketsQuery.Length > 0 || wikisQuery.Length > 0)
            {
              includesPreviousQuery = true;
            }

            notesQuery = GetNotesQuery(includesPreviousQuery, loginUser, selectNotesFields);

            SqlParameter dtSearchNotesResultsTable = new SqlParameter("@dtSearchNotesResultsTable", SqlDbType.Structured);
            dtSearchNotesResultsTable.TypeName = "dtSearch_results_tbltype";
            dtSearchNotesResultsTable.Value = dtSearchNotesResultsList;
            command.Parameters.Add(dtSearchNotesResultsTable);
          }
        }

        if (searchStandardFilter.ProductVersions && !hasNonProductVersionsFilters)
        {
          List<SqlDataRecord> dtSearchProductVersionsResultsList = ProductVersionsView.GetSearchResultsList(searchTerm, loginUser);

          if (dtSearchProductVersionsResultsList.Count > 0)
          {
            bool includesPreviousQuery = false;
            if (ticketsQuery.Length > 0 || wikisQuery.Length > 0 || notesQuery.Length > 0)
            {
              includesPreviousQuery = true;
            }

            productVersionsQuery = GetProductVersionsQuery(includesPreviousQuery, loginUser, selectProductVersionsFields);

            SqlParameter dtSearchProductVersionsResultsTable = new SqlParameter("@dtSearchProductVersionsResultsTable", SqlDbType.Structured);
            dtSearchProductVersionsResultsTable.TypeName = "dtSearch_results_tbltype";
            dtSearchProductVersionsResultsTable.Value = dtSearchProductVersionsResultsList;
            command.Parameters.Add(dtSearchProductVersionsResultsTable);
          }
        }

        if (searchStandardFilter.WaterCooler && !hasNonWaterCoolerFilters)
        {
          List<SqlDataRecord> dtSearchWaterCoolerResultsList = WaterCoolerView.GetSearchResultsList(searchTerm, loginUser);

          if (dtSearchWaterCoolerResultsList.Count > 0)
          {
            bool includesPreviousQuery = false;
            if (ticketsQuery.Length > 0 || wikisQuery.Length > 0 || notesQuery.Length > 0 || productVersionsQuery.Length > 0)
            {
              includesPreviousQuery = true;
            }

            waterCoolerQuery = GetWaterCoolerQuery(includesPreviousQuery, loginUser, selectWaterCoolerFields);

            SqlParameter dtSearchWaterCoolerResultsTable = new SqlParameter("@dtSearchWaterCoolerResultsTable", SqlDbType.Structured);
            dtSearchWaterCoolerResultsTable.TypeName = "dtSearch_results_tbltype";
            dtSearchWaterCoolerResultsTable.Value = dtSearchWaterCoolerResultsList;
            command.Parameters.Add(dtSearchWaterCoolerResultsTable);
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
        {5}
        {6}
        {7}
        
        SET @resultsCount = @@RowCount

        SELECT
          {8}
        FROM 
          @TempItems ti 
          LEFT JOIN dbo.TicketsView tv 
            ON ti.source = 17
            AND ti.recordID = tv.TicketID
          LEFT JOIN dbo.WikiArticlesView wv
            ON ti.source = 39
            AND ti.recordID = wv.ArticleID
          LEFT JOIN dbo.NotesView nv
            ON ti.source = 40
            AND ti.recordID = nv.NoteID
          LEFT JOIN dbo.ProductVersionsView pvv
            ON ti.source = 14
            AND ti.recordID = pvv.ProductVersionID
          LEFT JOIN dbo.NewWaterCoolerView wcv
            ON ti.source = 38
            AND ti.recordID = wcv.MessageID
        WHERE 
          ti.ID BETWEEN @FromIndex AND @toIndex
        ORDER BY 
          ti.ID
        ";

        command.CommandText = string.Format(
          query, 
          tempItemsTableFieldsDefinition, 
          tempItemsTableFields, 
          ticketsQuery, 
          wikisQuery, 
          notesQuery,
          productVersionsQuery,
          waterCoolerQuery,
          orderByClause, 
          fieldsList);
        command.CommandType = CommandType.Text;

        SqlParameter resultsCount = new SqlParameter("@resultsCount", SqlDbType.Int)
        {
          Direction = ParameterDirection.Output
        };
        command.Parameters.Add(resultsCount);

        command.Parameters.AddWithValue("@FromIndex", firstItemIndex + 1);
        command.Parameters.AddWithValue("@ToIndex",   firstItemIndex + pageSize);

        command.Parameters.AddWithValue("@userID", loginUser.UserID);

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
            case ReferenceType.Notes:
              resultItem.ID = (int)row["NoteID"];
              resultItem.CustomerID = (int)row["RefID"];
              resultItem.Creator = row["CreatorName"].ToString();
              resultItem.DateModified = row["DateModified"].ToString();
              switch ((ReferenceType)row["NoteRefType"])
              {
                case ReferenceType.Organizations:
                  resultItem.DisplayName = string.Format("{0}'s note: {1}",  row["OrganizationName"].ToString(), row["Title"].ToString());
                  resultItem.TypeID = 4;
                  break;
                case ReferenceType.Users:
                  resultItem.DisplayName = string.Format("{0}'s note: {1}", row["ContactName"].ToString(), row["Title"].ToString());
                  resultItem.TypeID = 7;
                  break;
              }

              break;
            case ReferenceType.ProductVersions:
              resultItem.ID = (int)row["ProductVersionID"];
              resultItem.ProductID = (int)row["ProductID"];
              resultItem.DisplayName = string.Format("{0}'s version {1}", row["ProductName"].ToString(), row["VersionNumber"].ToString());
              resultItem.Status = row["VersionStatus"].ToString();
              resultItem.DateModified = row["ProductVersionDateModified"].ToString();
              resultItem.TypeID = 5;

              break;
            case ReferenceType.WaterCooler:
              int messageParent = (int)row["MessageParent"];
              resultItem.ID = messageParent == -1 ? (int)row["MessageID"] : messageParent;
              resultItem.DisplayName = row["Message"].ToString();
              if (resultItem.DisplayName.Length > 100)
              {
                resultItem.DisplayName = resultItem.DisplayName.Substring(0, 100) + "...";
              }
              resultItem.Creator = row["UserName"].ToString();
              resultItem.DateModified = row["WaterCoolerLastModified"].ToString();
              resultItem.TypeID = 6;
              //resultItem.RefType = row["RefType"] is System.DBNull ? -1 : (int?)row["RefType"];
              //resultItem.AttachmentID = row["AttachmentID"] is System.DBNull ? -1 : (int?)row["AttachmentID"];
              resultItem.RefType = -1;
              resultItem.AttachmentID = -1;


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

    private bool GetHasIndexesToSearch(
      SearchStandardFilter searchStandardFilter, 
      LoginUser loginUser, 
      ref bool hasNonWikiFilters,
      ref bool hasNonNotesFilters,
      ref bool hasNonProductVersionsFilters,
      ref bool hasNonWaterCoolerFilters)
    {
      bool result = false;

      GetHasNonTableFilters(
        searchStandardFilter,
        loginUser,
        ref hasNonWikiFilters,
        ref hasNonNotesFilters,
        ref hasNonProductVersionsFilters,
        ref hasNonWaterCoolerFilters);

      if (
        searchStandardFilter.Tickets 
        || searchStandardFilter.KnowledgeBase
        || (searchStandardFilter.Wikis && !hasNonWikiFilters)
        || (searchStandardFilter.Notes && !hasNonNotesFilters)
        || (searchStandardFilter.ProductVersions && !hasNonProductVersionsFilters)
        || (searchStandardFilter.WaterCooler && !hasNonWaterCoolerFilters)
      )
      {
        result = true;
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

    private string GetNotesQuery(bool includePreviousQuery, LoginUser loginUser, string selectNotesFields)
    {
      StringBuilder resultBuilder = new StringBuilder();

      if (includePreviousQuery)
      {
        resultBuilder.Append(" UNION ");
      }

      resultBuilder.Append(@"
        SELECT
          nv.NoteID
          , 40
          , dnrt.relevance
          , nv.DateModified");
      resultBuilder.Append(selectNotesFields);

      resultBuilder.Append(@"
        FROM
          dbo.NotesView nv
          JOIN @dtSearchNotesResultsTable dnrt
            ON nv.NoteID = dnrt.RecordID
        WHERE
          1 = 1
      ");

      resultBuilder.Append(NotesView.GetSearchResultsWhereClause(loginUser));

      return resultBuilder.ToString();
    }

    private string GetProductVersionsQuery(bool includePreviousQuery, LoginUser loginUser, string selectProductVersionsFields)
    {
      StringBuilder resultBuilder = new StringBuilder();

      if (includePreviousQuery)
      {
        resultBuilder.Append(" UNION ");
      }

      resultBuilder.Append(@"
        SELECT
          pvv.ProductVersionID
          , 14
          , dpvrt.relevance
          , pvv.DateModified");
      resultBuilder.Append(selectProductVersionsFields);

      resultBuilder.Append(@"
        FROM
          dbo.ProductVersionsView pvv
          JOIN @dtSearchProductVersionsResultsTable dpvrt
            ON pvv.ProductVersionID = dpvrt.RecordID
        WHERE
          1 = 1
      ");

      resultBuilder.Append(ProductVersionsView.GetSearchResultsWhereClause(loginUser));

      return resultBuilder.ToString();
    }

    private string GetWaterCoolerQuery(bool includePreviousQuery, LoginUser loginUser, string selectWaterCoolerFields)
    {
      StringBuilder resultBuilder = new StringBuilder();

      if (includePreviousQuery)
      {
        resultBuilder.Append(" UNION ");
      }

      resultBuilder.Append(@"
        SELECT
          wcv.MessageID
          , 38
          , dpvrt.relevance
          , wcv.LastModified AS DateModified");
      resultBuilder.Append(selectWaterCoolerFields);

      resultBuilder.Append(@"
        FROM
          dbo.NewWaterCoolerView wcv
          JOIN @dtSearchWaterCoolerResultsTable dpvrt
            ON wcv.MessageID = dpvrt.RecordID
          LEFT JOIN dbo.NewWaterCoolerView pwcv
            ON wcv.MessageParent = pwcv.MessageID
          LEFT JOIN (SELECT COUNT(*) AS Count, x.MessageID FROM WatercoolerAttachments x WHERE x.RefType IN (3,4) GROUP BY x.MessageID) userOrGroupAttachments
            ON (wcv.MessageParent < 0 AND wcv.MessageID = userOrGroupAttachments.MessageID)
            OR (wcv.MessageParent > 0 AND wcv.MessageParent = userOrGroupAttachments.MessageID)
          LEFT JOIN (SELECT COUNT(*) AS Count, x.MessageID FROM WatercoolerAttachments x WHERE x.RefType = 3 AND x.AttachmentID = @userID GROUP BY x.MessageID) userAttachments
            ON (wcv.MessageParent < 0 AND wcv.MessageID = userAttachments.MessageID)
            OR (wcv.MessageParent > 0 AND wcv.MessageParent = userAttachments.MessageID)
          LEFT JOIN (SELECT COUNT(*) AS Count, x.MessageID FROM WatercoolerAttachments x WHERE x.RefType = 4 AND x.AttachmentID IN (SELECT gu.GroupID FROM GroupUsers gu WHERE gu.UserID = @UserID) GROUP BY x.MessageID) groupAttachments
            ON (wcv.MessageParent < 0 AND wcv.MessageID = groupAttachments.MessageID)
            OR (wcv.MessageParent > 0 AND wcv.MessageParent = groupAttachments.MessageID)
        WHERE
          wcv.IsDeleted = 0
          AND
          (
            (wcv.MessageParent < 0 AND wcv.UserID = @userID) OR (wcv.MessageParent > 0 AND pwcv.UserID = @userID)
            OR userOrGroupAttachments.Count IS NULL
            OR userAttachments.Count > 0    
            OR groupAttachments.Count > 0    
          )
      ");

      resultBuilder.Append(WaterCoolerView.GetSearchResultsWhereClause(loginUser));

      return resultBuilder.ToString();
    }

    private string GetOrderByClause(LoginUser loginUser, 
                                    ref string tempItemsTableFieldsDefinition, 
                                    ref string tempItemsTableFields,
                                    ref string selectTicketsFields,
                                    ref string selectWikisFields,
                                    ref string selectNotesFields,
                                    ref string selectProductVersionsFields,
                                    ref string selectWaterCoolerFields)
    {
      SearchSorters searchSorters = new SearchSorters(loginUser);
      searchSorters.LoadByUserID(loginUser.UserID);
      return searchSorters.ConvertToOrderByClause(ref tempItemsTableFieldsDefinition, 
                                                  ref tempItemsTableFields,
                                                  ref selectTicketsFields,
                                                  ref selectWikisFields,
                                                  ref selectNotesFields,
                                                  ref selectProductVersionsFields,
                                                  ref selectWaterCoolerFields);
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
        , nv.NoteID
        , nv.RefID
        , nv.OrganizationName
        , nv.Title
        , nv.CreatorName
        , nv.DateModified
        , nv.RefType AS NoteRefType
        , nv.ContactName
        , pvv.ProductVersionID
        , pvv.ProductID
        , pvv.ProductName
        , pvv.VersionNumber
        , pvv.VersionStatus
        , pvv.DateModified AS ProductVersionDateModified
        , wcv.MessageID
        , wcv.Message
        , wcv.UserName
        , wcv.LastModified AS WaterCoolerLastModified
        , wcv.RefType
        , wcv.AttachmentID
        , wcv.MessageParent
      ";
    }

    private void GetHasNonTableFilters(
      SearchStandardFilter searchStandardFilter,
      LoginUser loginUser,
      ref bool  hasNonWikiFilters,
      ref bool  hasNonNotesFilters,
      ref bool  hasNonProductVersionsFilters,
      ref bool  hasNonWaterCoolerFilters)
    {
      Organizations organizations = new Organizations(loginUser);
      organizations.LoadByOrganizationID(loginUser.OrganizationID);
      if (!organizations.IsEmpty && organizations[0].ProductType == ProductType.Express)
      {
        hasNonWikiFilters = true;
      }

      ReportTableFields ticketsViewFields = new ReportTableFields(loginUser);
      ticketsViewFields.LoadByReportTableID(10);

      SearchCustomFilters filters = new SearchCustomFilters(loginUser);
      filters.LoadByUserID(loginUser.UserID);

      WikiArticlesViewItem    wikiArticlesViewItem    = null;
      NotesViewItem           notesViewItem           = null;
      ProductVersionsViewItem productVersionsViewItem = null;
      WaterCoolerViewItem     waterCoolerViewItem     = null;

      bool isFirstIteration = true;

      foreach (SearchCustomFilter filter in filters)
      {
        ReportTableField field = ticketsViewFields.FindByReportTableFieldID(filter.FieldID);
        if (field == null)
        {
          hasNonWikiFilters             = true;
          hasNonNotesFilters            = true;
          hasNonProductVersionsFilters  = true;
          hasNonWaterCoolerFilters      = true;
        }
        else
        {
          string fieldName = field.FieldName;

        if (!hasNonWikiFilters && searchStandardFilter.Wikis)
        {
          if (isFirstIteration)
          {
            WikiArticlesView wikiArticlesView = new WikiArticlesView(loginUser);
            wikiArticlesViewItem = wikiArticlesView.AddNewWikiArticlesViewItem();
          }

        string wikiEquivalentFieldName = DataUtils.GetWikiEquivalentFieldName(fieldName);
          if (!DataUtils.GetIsColumnInBaseCollection(wikiArticlesViewItem.Collection, wikiEquivalentFieldName))
          {
            hasNonWikiFilters = true;
          }
        }

        if (!hasNonNotesFilters && searchStandardFilter.Notes)
        {
          if (isFirstIteration)
          {
            NotesView notesView = new NotesView(loginUser);
            notesViewItem = notesView.AddNewNotesViewItem();
          }

          string notesEquivalentFieldName = DataUtils.GetNotesEquivalentFieldName(fieldName);
          if (!DataUtils.GetIsColumnInBaseCollection(notesViewItem.Collection, notesEquivalentFieldName))
          {
            hasNonNotesFilters = true;
          }
        }

        if (!hasNonProductVersionsFilters && searchStandardFilter.ProductVersions)
        {
          if (isFirstIteration)
          {
            ProductVersionsView productVersionsView = new ProductVersionsView(loginUser);
            productVersionsViewItem = productVersionsView.AddNewProductVersionsViewItem();
          }

          string productVersionEquivalentFieldName = DataUtils.GetProductVersionsEquivalentFieldName(fieldName);
          if (!DataUtils.GetIsColumnInBaseCollection(productVersionsViewItem.Collection, productVersionEquivalentFieldName))
          {
            hasNonProductVersionsFilters = true;
          }
        }

        if (!hasNonWaterCoolerFilters && searchStandardFilter.WaterCooler)
        {
          if (isFirstIteration)
          {
            WaterCoolerView waterCoolerView = new WaterCoolerView(loginUser);
            waterCoolerViewItem = waterCoolerView.AddNewWaterCoolerViewItem();
          }

          string waterCoolerEquivalentFieldName = DataUtils.GetWaterCoolerEquivalentFieldName(fieldName);
          if (!DataUtils.GetIsColumnInBaseCollection(waterCoolerViewItem.Collection, waterCoolerEquivalentFieldName))
          {
            hasNonWaterCoolerFilters = true;
          }
        }
        }
      }
    }

    [WebMethod]
    public int AddStandardFilters(
      bool includeTickets, 
      bool includeKnowledgeBase,
      bool includeWikis,
      bool includeNotes,
      bool includeProductVersions,
      bool includeWaterCooler)
    {
      SearchStandardFilters filters = new SearchStandardFilters(TSAuthentication.GetLoginUser());
      SearchStandardFilter filter = filters.AddNewSearchStandardFilter();

      filter.UserID         = filters.LoginUser.UserID;
      filter.Tickets        = includeTickets;
      filter.KnowledgeBase  = includeKnowledgeBase;
      filter.Wikis          = includeWikis;
      filter.Notes            = includeNotes;
      filter.ProductVersions  = includeProductVersions;
      filter.WaterCooler      = includeWaterCooler;

      filters.Save();

      return filter.StandardFilterID;
    }

    [WebMethod]
    public void UpdateStandardFilters(
      int standardFilterID, 
      bool includeTickets, 
      bool includeKnowledgeBase, 
      bool includeWikis, 
      bool includeNotes, 
      bool includeProductVersions,
      bool includeWaterCooler)
    {
      SearchStandardFilters filters = new SearchStandardFilters(TSAuthentication.GetLoginUser());
      filters.LoadByStandardFilterID((int)standardFilterID);
      SearchStandardFilter filter = filters.FindByStandardFilterID((int)standardFilterID);

      filter.Tickets        = includeTickets;
      filter.KnowledgeBase  = includeKnowledgeBase;
      filter.Wikis          = includeWikis;
      filter.Notes            = includeNotes;
      filter.ProductVersions  = includeProductVersions;
      filter.WaterCooler      = includeWaterCooler;
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
        result.Notes            = (bool)standardFilters.Table.Rows[0]["Notes"];
        result.ProductVersions  = (bool)standardFilters.Table.Rows[0]["ProductVersions"];
        result.WaterCooler      = (bool)standardFilters.Table.Rows[0]["WaterCooler"];

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

      TicketTypes ticketTypes = new TicketTypes(fields.LoginUser);
      ticketTypes.LoadAllPositions(TSAuthentication.OrganizationID);

      foreach (CustomField custom in customs)
      {
        TicketType ticketType = ticketTypes.FindByTicketTypeID(custom.AuxID);
        if (ticketType == null)
        {
        fieldItems.Add(new AutoFieldItem(custom));
      }
        else
        {
          fieldItems.Add(new AutoFieldItem(custom, string.Format("{0} ({1})", custom.Name, ticketType.Name)));
        }
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

    [WebMethod]
    public string[] CustomerSearchTest(string searchTerm, int organizationID)
    {
      if (TSAuthentication.OrganizationID != 1078) return null;
      LoginUser loginUser = new LoginUser(TSAuthentication.GetLoginUser().ConnectionString, TSAuthentication.UserID, organizationID, null);
      SearchResults results = GetCustomerSearchResults(loginUser, searchTerm, true, true, 50);
      return GetSearchReport(results);
    }

    public string[] GetSearchReport(SearchResults results)
    {
      SearchReportJob report = results.NewSearchReportJob();
      report.SelectAll();
      report.Flags = ReportFlags.dtsReportStoreInResults | ReportFlags.dtsReportWholeFile | ReportFlags.dtsReportGetFromCache | ReportFlags.dtsReportIncludeAll;
      report.OutputFormat = OutputFormats.itUnformattedHTML;
      report.MaxContextBlocks = 3;
      report.MaxWordsToRead = 50000;
      report.OutputStringMaxSize = 50000;
      //report.WordsOfContext = 10;
      report.BeforeHit = "123456789";// "<strong style=\"color:red;\">";
      report.AfterHit = "987654321"; //"</strong>";
      report.OutputToString = true;
      report.Execute();
      report.ContextSeparator = "<br/><br/>";
      List<string> result = new List<string>();


      for (int i = 0; i < results.Count; i++)
      {
        result.Add(GetSearchItemReport(results, i));
      }
      return result.ToArray();
    }

    public string GetSearchItemReport(SearchResults results, int index)
    {
      results.GetNthDoc(index);

      using (FileConverter fc = new FileConverter())
      {

        // This sets up FileConverter with the input file, index location, and hits
        fc.SetInputItem(results, index);

        fc.OutputToString = true;
        fc.OutputStringMaxSize = 2000000;
        fc.OutputFormat = OutputFormats.itHTML;

        fc.Header = "<A NAME=hit0></A>"
          + "<TABLE border=0>"
          + "<TR><TD bgcolor=#003399 color=#ffffff>"
          + "<font face=verdana color=#ffffff size=2></TD></TR>"
          + "<TR><TD bgcolor=#eeeeee color=#000000>"
          + "<font face=verdana color=#003399 size=2>"
          + "<B>" + results.get_DocDetailItem("_shortName") + "   </B><BR>"
          + "<font face=verdana color=#000000 size=1>"
          + results.DocHitCount + " Hits."
          + "  <B>Location: </B>  <B>Date: </B>"
          + results.get_DocDetailItem("_date") + "<BR>"
          + "</FONT>"
          + "</TD></TR><TR><TD bgcolor=#003399 color=#ffffff>"
          + "<font face=verdana color=#ffffff size=2></TD></TR></TABLE>";


        fc.BeforeHit = "<span style=\"background-color: #FF0000\">";
        fc.AfterHit = "</span>";
        fc.Flags = (fc.Flags | ConvertFlags.dtsConvertGetFromCache);
        fc.Execute();
        return fc.OutputString;
      }
    }

    public SearchResults GetCustomerSearchResults(LoginUser loginUser, string searchTerm, bool searchCompanies, bool searchContacts, int max)
    { 
        Options options = new Options();
        options.TextFlags = TextFlags.dtsoTfRecognizeDates;
        using (SearchJob job = new SearchJob())
        {

          job.Request = searchTerm;
          job.FieldWeights = "Name:20,Email:10";
          job.MaxFilesToRetrieve = max;
          job.AutoStopLimit = 10000000;
          job.TimeoutSeconds = 30;

          job.Fuzziness = 0;
          job.Request = "*" + job.Request + "*";

          User user = Users.GetUser(loginUser, loginUser.UserID);
          if (user.TicketRights == TicketRightType.Customers)
          {
            StringBuilder conditions = new StringBuilder();
            Organizations orgs = new Organizations(loginUser);
            orgs.LoadByUserRights(user.UserID);

            for (int i = 0; i < orgs.Count; i++)
            {
              if (i > 0) conditions.Append(" OR ");
              conditions.Append("(OrganizationID::" + orgs[i].OrganizationID.ToString() + ") ");
            }

            job.BooleanConditions = conditions.ToString();
          }

          job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchDelayDocInfo;

          if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;

          string companiesIndexPath = DataUtils.GetCompaniesIndexPath(loginUser);

          if (searchCompanies)
          {
            job.IndexesToSearch.Add(companiesIndexPath);
          }

          string contactsIndexPath = DataUtils.GetContactsIndexPath(loginUser);
          if (searchContacts)
          {
            job.IndexesToSearch.Add(contactsIndexPath);
          }
          job.Execute();
          job.Results.Sort(SortFlags.dtsSortByRelevanceScore | SortFlags.dtsSortDescending, "");
          return job.Results;
        }
    }

    public SearchResults GetAssetsSearchResults(LoginUser loginUser, string searchTerm, bool searchAssigned, bool searchWarehouse, bool searchJunkyard, int max)
    {
      Options options = new Options();
      options.TextFlags = TextFlags.dtsoTfRecognizeDates;
      using (SearchJob job = new SearchJob())
      {

        job.Request = searchTerm;
        job.FieldWeights = "Name:20,SerialNumber:10";
        job.MaxFilesToRetrieve = max;
        job.AutoStopLimit = 10000000;
        job.TimeoutSeconds = 30;

        job.Fuzziness = 0;
        job.Request = "*" + job.Request + "*";

        if (!searchAssigned || !searchWarehouse || !searchJunkyard)
        {
          StringBuilder conditions = new StringBuilder();
          if (searchAssigned)
          {
            conditions.Append("(Location::1) ");
            if (searchWarehouse) conditions.Append("OR (Location::2) ");
            if (searchJunkyard) conditions.Append("OR (Location::3) ");
          }
          else if (searchWarehouse)
          {
            conditions.Append("(Location::2) ");
            if (searchJunkyard) conditions.Append("OR (Location::3) ");
          }
          else if (searchJunkyard)
          {
            conditions.Append("(Location::3) ");
          }
          job.BooleanConditions = conditions.ToString();
        }

        job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchDelayDocInfo;

        if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;

        job.IndexesToSearch.Add(DataUtils.GetAssetsIndexPath(loginUser));
        job.Execute();
        job.Results.Sort(SortFlags.dtsSortByRelevanceScore | SortFlags.dtsSortDescending, "");
        return job.Results;
      }
    }

    [WebMethod]
    public string[] SearchCompaniesAndContacts(string searchTerm, int from, int count, bool searchCompanies, bool searchContacts)
    {      
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      List<string> resultItems = new List<string>();
      if (string.IsNullOrWhiteSpace(searchTerm))
      {
        return GetAllCompaniesAndContacts(from, count, searchCompanies, searchContacts);
      }

      if (searchCompanies || searchContacts)
      {
          SearchResults results = GetCustomerSearchResults(loginUser, searchTerm, searchCompanies, searchContacts, 0);
          int topLimit = from + count;
          if (topLimit > results.Count)
          {
            topLimit = results.Count;
          }

          for (int i = from; i < topLimit; i++)
          {
            results.GetNthDoc(i);
            if (results.CurrentItem.UserFields["JSON"] != null)
              resultItems.Add(results.CurrentItem.UserFields["JSON"].ToString());
          }
        
      }

      return resultItems.ToArray();
    }

    private string[] GetAllCompaniesAndContacts(int from, int count, bool searchCompanies, bool searchContacts)
    {
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      List<string> results = new List<string>();
      SqlCommand command = new SqlCommand();

      string pageQuery = @"
WITH 
q AS ({0}),
r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY [NAME] ASC) AS 'RowNum' FROM q)
SELECT * INTO #X FROM r
WHERE RowNum BETWEEN @From AND @To

SELECT 
	o.Name AS Organization, 
	o.OrganizationID, 
	u.UserID, 
	o.Website, 
	u.IsPortalUser, 
	o.HasPortalAccess, 
	u.FirstName, 
	u.LastName, 
	u.Email, 
	u.Title,
	(SELECT COUNT(*) FROM TicketsView t LEFT JOIN OrganizationTickets ot ON ot.TicketID = t.TicketID WHERE ot.OrganizationID = o.OrganizationID AND t.IsClosed = 0) AS OrgOpenTickets,
	(SELECT COUNT(*) FROM TicketsView t LEFT JOIN UserTickets ut ON ut.TicketID = t.TicketID WHERE ut.UserID = u.UserID AND t.IsClosed = 0) AS ContactOpenTickets
FROM #X AS x
LEFT JOIN Organizations o ON o.OrganizationID = x.OrganizationID
LEFT JOIN Users u ON u.UserID = x.UserID";

      string companyQuery = @"
SELECT 
  LTRIM(o.Name) AS Name, 
  o.OrganizationID,
  NULL AS UserID
  FROM Organizations o WHERE o.ParentID = @OrganizationID
";

      string contactQuery = @"
SELECT 
  LTRIM(u.LastName + ' ' + u.FirstName) AS Name, 
  u.OrganizationID,
  u.UserID
  FROM Users u
  LEFT JOIN Organizations o ON u.OrganizationID = o.OrganizationID
  WHERE o.ParentID = @OrganizationID AND u.MarkDeleted=0
";
      User user = Users.GetUser(loginUser, loginUser.UserID);
      if (user.TicketRights == TicketRightType.Customers)
      {
        companyQuery = companyQuery + " AND o.OrganizationID IN (SELECT OrganizationID FROM UserRightsOrganizations WHERE UserID = " + user.UserID.ToString() + ")";
        contactQuery = contactQuery + " AND u.OrganizationID IN (SELECT OrganizationID FROM UserRightsOrganizations WHERE UserID = " + user.UserID.ToString() + ")";
      }

      if (searchContacts && searchCompanies)
      {
        command.CommandText = string.Format(pageQuery, companyQuery + " UNION ALL " + contactQuery);
      }
      else if (searchCompanies) {
        command.CommandText = string.Format(pageQuery, companyQuery);
      }
      else if (searchContacts) {
        command.CommandText = string.Format(pageQuery, contactQuery);
      }
      else
      {
        return results.ToArray();
      }


      command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
      command.Parameters.AddWithValue("@From", from+1);
      command.Parameters.AddWithValue("@To", from+count);

      DataTable table = SqlExecutor.ExecuteQuery(loginUser, command);

      foreach (DataRow row in table.Rows)
      {
        if (row["UserID"] == DBNull.Value)
        {
          CustomerSearchCompany company = new CustomerSearchCompany();
          company.name = (string)row["Organization"];
          company.organizationID = (int)row["OrganizationID"];
          company.isPortal = (bool)row["HasPortalAccess"];
          company.openTicketCount = (int)row["OrgOpenTickets"];
          company.website = GetDBString(row["Website"]);

          List<CustomerSearchPhone> phones = new List<CustomerSearchPhone>();
          PhoneNumbers phoneNumbers = new PhoneNumbers(loginUser);
          //phoneNumbers.LoadByID(company.organizationID, ReferenceType.Organizations);
          foreach (PhoneNumber number in phoneNumbers)
          {
            phones.Add(new CustomerSearchPhone(number));
          }
          company.phones = phones.ToArray();

          results.Add(JsonConvert.SerializeObject(company));
        }
        else
        {
          CustomerSearchContact contact = new CustomerSearchContact();
          contact.organizationID = (int)row["OrganizationID"];
          contact.isPortal = (bool)row["IsPortalUser"];
          contact.openTicketCount = (int)row["ContactOpenTickets"];

          contact.userID = (int)row["UserID"];
          contact.fName = GetDBString(row["FirstName"]);
          contact.lName = GetDBString(row["LastName"]);
          contact.email = GetDBString(row["Email"]);
          contact.title = GetDBString(row["Title"]);
          contact.organization = GetDBString(row["Organization"]);

          List<CustomerSearchPhone> phones = new List<CustomerSearchPhone>();
          PhoneNumbers phoneNumbers = new PhoneNumbers(loginUser);
          //phoneNumbers.LoadByID(contact.userID, ReferenceType.Contacts);
          foreach (PhoneNumber number in phoneNumbers)
          {
            phones.Add(new CustomerSearchPhone(number));
          }
          contact.phones = phones.ToArray();
          results.Add(JsonConvert.SerializeObject(contact));
        }
        
      }

      return results.ToArray();
    
    }

    public static string GetDBString(object o)
    {
      if (o == null || o == DBNull.Value) return "";
      return o.ToString();
    }

    [WebMethod]
    public string[] SearchAssets(string searchTerm, int from, int count, bool searchAssigned, bool searchWarehouse, bool searchJunkyard)
    {
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      List<string> resultItems = new List<string>();
      if (string.IsNullOrWhiteSpace(searchTerm))
      {
        //return GetAllAssets(from, count, searchAssigned, searchWarehouse, searchJunkyard);
        searchTerm = "xfirstword";
      }

      //if (searchCompanies || searchContacts)
      //{
        SearchResults results = GetAssetsSearchResults(loginUser, searchTerm, searchAssigned, searchWarehouse, searchJunkyard, 0);
        int topLimit = from + count;
        if (topLimit > results.Count)
        {
          topLimit = results.Count;
        }

        for (int i = from; i < topLimit; i++)
        {
          results.GetNthDoc(i);
          if (results.CurrentItem.UserFields["JSON"] != null)
            resultItems.Add(results.CurrentItem.UserFields["JSON"].ToString());
        }

      //}

      return resultItems.ToArray();
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
    public bool? Notes { get; set; }
    [DataMember]
    public bool? ProductVersions { get; set; }
    [DataMember]
    public bool? WaterCooler { get; set; }
    [DataMember]
    public AutoFieldItem[] Fields { get; set; }
    [DataMember]
    public SearchCustomFilterProxy[] CustomFilters { get; set; }
    [DataMember]
    public SearchSorterProxy[] Sorters { get; set; }
  }

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class CompaniesAndContactsSearchResults
  {
    [DataMember]
    public string SearchTerm { get; set; }
    [DataMember]
    public int From { get; set; }
    [DataMember]
    public int To { get; set; }
    [DataMember]
    public CompanyOrContact[] Items { get; set; }
  }

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class CompanyOrContact
  {
    [DataMember]
    public int Id { get; set; }
    [DataMember]
    public ReferenceType ReferenceType { get; set; }
  }
}