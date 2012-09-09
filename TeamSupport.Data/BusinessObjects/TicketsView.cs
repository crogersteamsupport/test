using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using dtSearch.Engine;

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
    [DataMember] public int? ResolvedID { get; set; }
    [DataMember] public int? ReportedID { get; set; }
    [DataMember] public bool? IsClosed { get; set; }
    [DataMember] public int? TicketStatusID { get; set; }
    [DataMember] public int? TicketSeverityID { get; set; }
    [DataMember] public int? UserID { get; set; }
    [DataMember] public int? GroupID { get; set; }
    [DataMember] public int? CustomerID { get; set; }
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
  }
  
  public partial class TicketsView
  {

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


    /// <summary>
    /// Loads tickets that are associated with a customer's organizationid
    /// </summary>
    /// <param name="organizationID"></param>
    public void LoadByCustomerID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT tv.* FROM TicketsView tv LEFT JOIN OrganizationTickets ot ON ot.TicketID = tv.TicketID WHERE ot.OrganizationID = @OrganizationID ORDER BY TicketNumber";
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

    public void LoadAllSlaViolations()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TicketID, SlaViolationTime, SlaWarningTime  FROM TicketsView WHERE SlaViolationTime <= 0 OR SlaWarningTime <=0";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public void LoadForIndexing(int organizationID, int max)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string text = @"
        SELECT TOP {0} TicketID
        FROM TicketsView tv WITH(NOLOCK)
        WHERE tv.NeedsIndexing = 1
        AND tv.OrganizationID= @OrganizationID
        ORDER BY DateModified DESC";

        command.CommandText = string.Format(text, max.ToString());
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByFilter(int pageIndex, int pageSize, TicketLoadFilter filter)
    {/*
      using (SqlCommand command = new SqlCommand())
      {
        string sort = filter.SortColumn.Trim();
        switch (sort)
        {
          case "Severity": sort = "SeverityPosition"; break;
          case "Status": sort = "StatusPosition"; break;
          default: break;
        }
        StringBuilder builder = new StringBuilder();
        builder.Append("WITH TicketRows AS (SELECT ROW_NUMBER() OVER (ORDER BY tv.");
        builder.Append(sort);
        if (filter.SortAsc) builder.Append(" ASC"); else builder.Append(" DESC");
        builder.Append(
              @") AS RowNumber
              ,tv.[TicketID]
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
              ,0 AS [SlaViolationTime]
              ,0 AS [SlaWarningTime]
              ,CAST(0 AS dec(24,6)) AS [SlaViolationHours]
              ,CAST(0 AS dec(24,6)) AS [SlaWarningHours]"
              );

        GetFilterWhereClause(filter, command, builder);
        builder.Append(") SELECT * FROM TicketRows  WHERE RowNumber BETWEEN @PageIndex*@PageSize+1 AND @PageIndex*@PageSize+@PageSize ORDER BY RowNumber ASC");
        command.CommandText = builder.ToString().Replace(Environment.NewLine, " ");
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@PageIndex", pageIndex);
        command.Parameters.AddWithValue("@PageSize", pageSize);
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        Fill(command);
      }
      */
      LoadByRange(pageIndex * pageSize, pageIndex * pageSize + pageSize, filter);
    }

    public void LoadByRange(int from, int to, TicketLoadFilter filter)
    {
      Fill(GetLoadRangeCommand(LoginUser, from, to, filter));
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
            ,tv.[SlaViolationTime]");
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
      switch (sort)
      {
        case "Severity": sort = string.Format("[SeverityPosition] {0}", (filter.SortAsc ? "ASC" : "DESC")); break;
        case "Status": sort = string.Format("[StatusPosition] {0}, [Status] {0}, [TicketTypeName] {0}", (filter.SortAsc ? "ASC" : "DESC")); break;
        default: 
          sort = string.Format("[{0}] {1}", sort, (filter.SortAsc ? "ASC" : "DESC"));
          break;
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
        ,tv.IsRead
        ,tv.IsFlagged";
      StringBuilder where = new StringBuilder();
      GetFilterWhereClause(loginUser, filter, command, where);

      string query = @"
        DECLARE @TempItems TABLE( ID int IDENTITY, TicketID int )

        INSERT INTO @TempItems (TicketID)
        SELECT tv.TicketID {0}
        ORDER BY {1}

        SELECT {2}
        FROM @TempItems ti INNER JOIN UserTicketsView tv ON tv.TicketID = ti.TicketID
        WHERE ID BETWEEN @FromIndex AND @toIndex
        AND tv.ViewerID = @ViewerID

        ORDER BY ti.ID
        ";

      command.CommandText = string.Format(query, where.ToString(), sort, fields);
      command.CommandType = CommandType.Text;
      command.Parameters.AddWithValue("@FromIndex", from+1);
      command.Parameters.AddWithValue("@ToIndex", to+1);
      command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
      return command;
    }


    public int GetFilterCount(TicketLoadFilter filter)
    {
      using (SqlCommand command = new SqlCommand())
      {
        StringBuilder builder = new StringBuilder();
        builder.Append("SELECT COUNT(*) ");
        GetFilterWhereClause(LoginUser, filter, command, builder);
        command.CommandText = builder.ToString().Replace(Environment.NewLine, " ");
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        object o = ExecuteScalar(command);
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
      AddTicketParameter("ReportedVersionID", filter.ReportedID, true, builder, command);
      AddTicketParameter("SolvedVersionID", filter.ResolvedID, true, builder, command);
      AddTicketParameter("IsVisibleOnPortal", filter.IsVisibleOnPortal, false, builder, command);
      AddTicketParameter("IsKnowledgeBase", filter.IsVisibleOnPortal, false, builder, command);
      AddTicketParameter("DateCreated", "DateCreatedBegin", filter.DateCreatedBegin, ">=", builder, command);
      AddTicketParameter("DateCreated", "DateCreatedEnd", filter.DateCreatedEnd, "<=", builder, command);
      AddTicketParameter("DateModified", "DateModifiedBegin", filter.DateModifiedBegin, ">=", builder, command);
      AddTicketParameter("DateModified", "DateModifiedEnd", filter.DateModifiedEnd, "<=", builder, command);
      AddTicketParameter("ViewerID", loginUser.UserID, false, builder, command);

      if (filter.ForumCategoryID != null && filter.ForumCategoryID == -1)
      {
        builder.Append(" AND (tv.ForumCategory IS NOT NULL)");
      }

      if (filter.UserID != null && filter.GroupID != null && filter.GroupID == -1)
      {
        builder.Append(" AND (tv.GroupID IN (SELECT gu.GroupID FROM GroupUsers gu WHERE gu.UserID = @UserID))");
        command.Parameters.AddWithValue("UserID", filter.UserID);
      }
      else if (filter.UserID != null && filter.GroupID != null && filter.GroupID == -2)
      {
        builder.Append(" AND (tv.UserID IS NULL AND tv.GroupID IN (SELECT gu.GroupID FROM GroupUsers gu WHERE gu.UserID = @UserID))");
        command.Parameters.AddWithValue("UserID", filter.UserID);
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
        //command.Parameters.AddWithValue("@TicketIDs", ids);
        /*
        command.Parameters.AddWithValue("@Search", DataUtils.BuildSearchString(search, filter.MatchAllTerms));
        command.Parameters.AddWithValue("@SearchClean", search.Replace("*", "").Replace("%", "").Replace("\"", ""));
        builder.Append(
                        @" AND ( 
                            (tv.TicketNumber LIKE '%'+@SearchClean+'%') OR
                            (tv.Name LIKE @SearchClean+'%') OR
                            EXISTS (SELECT * FROM Actions a WHERE (a.TicketID = tv.TicketID) AND CONTAINS((a.[Description], a.[Name]), @Search))
                          )");
        //--OR EXISTS (SELECT * FROM Tickets t WHERE (t.TicketID = tv.TicketID) AND CONTAINS((t.[Name]), @Search))
        //--OR EXISTS (SELECT * FROM CustomValues cv LEFT JOIN CustomFields cf ON cv.CustomFieldID = cf.CustomFieldID WHERE (cf.RefType = 17) AND (cv.RefID = tv.TicketID) AND CONTAINS((cv.[CustomValue]), @Search))
         */
      }

      if (filter.Tags != null && filter.Tags.Length > 0)
      {
        for (int i = 0; i < filter.Tags.Length; i++)
        {
          builder.Append(" AND EXISTS (SELECT * FROM TagLinks WHERE TagLinks.RefID=tv.TicketID AND TagLinks.RefType=17 AND TagLinks.TagID = @TagID" + i.ToString() + ")");
          command.Parameters.AddWithValue("@TagID" + i.ToString(), filter.Tags[i]);
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
            if (filter.IsKnowledgeBase != null) conditions.Append("(IsKnowledgeBase::" + filter.IsKnowledgeBase.ToString() + ")");
          }

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

    public static SearchResults GetSearchTicketResults(string searchTerm, LoginUser loginUser, TicketLoadFilter filter)
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
          if (filter.IsKnowledgeBase != null) conditions.Append("(IsKnowledgeBase::" + filter.IsKnowledgeBase.ToString() + ")");
        }
        job.BooleanConditions = conditions.ToString();


        job.MaxFilesToRetrieve = 1000;
        job.AutoStopLimit = 1000000;
        job.TimeoutSeconds = 30;
        job.SearchFlags =
          SearchFlags.dtsSearchSelectMostRecent |
          SearchFlags.dtsSearchStemming |
          SearchFlags.dtsSearchDelayDocInfo;

        int num = 0;
        if (!int.TryParse(searchTerm, out num))
        {
          job.Fuzziness = 1;
          job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchFuzzy;
        }

        if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;
        job.IndexesToSearch.Add(DataUtils.GetTicketIndexPath(loginUser));
        job.Execute();

        return job.Results;
      }
    
    }

    public static SearchResults GetSearchTicketResultsv2(string searchTerm, LoginUser loginUser)
    {
      Options options = new Options();
      options.TextFlags = TextFlags.dtsoTfRecognizeDates;
      using (SearchJob job = new SearchJob())
      {

        searchTerm = searchTerm.Trim();
        job.Request = searchTerm;
        job.FieldWeights = "Name: 1000";


        StringBuilder conditions = new StringBuilder();
        //if (filter != null)
        //{
        //  if (filter.IsKnowledgeBase != null) conditions.Append("(IsKnowledgeBase::" + filter.IsKnowledgeBase.ToString() + ")");
        //}
        job.BooleanConditions = conditions.ToString();


        job.MaxFilesToRetrieve = 1000;
        job.AutoStopLimit = 1000000;
        job.TimeoutSeconds = 30;
        job.SearchFlags =
          //SearchFlags.dtsSearchSelectMostRecent |
          SearchFlags.dtsSearchDelayDocInfo;

        int num = 0;
        if (!int.TryParse(searchTerm, out num))
        {
          job.Fuzziness = 1;
          job.SearchFlags = job.SearchFlags | 
            SearchFlags.dtsSearchFuzzy | 
            SearchFlags.dtsSearchStemming |
            SearchFlags.dtsSearchPositionalScoring |
            SearchFlags.dtsSearchAutoTermWeight;
        }

        if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;
        job.IndexesToSearch.Add(DataUtils.GetTicketIndexPath(loginUser));
        job.Execute();

        return job.Results;
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
      SearchResults results = GetSearchTicketResults(searchTerm, loginUser, filter);

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

      SearchResults results = GetSearchTicketResultsv2(searchTerm, loginUser);

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
            builder.Append(" AND (tv."+name+" IS NULL)");
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
            command.CommandText = @"SELECT TicketNumber, 'https://app.teamsupport.com?ticketid=' + CONVERT(VARCHAR,tv.TicketID) AS 'TicketURL',
                                        Name, TicketTypeName, TicketSource, Status, Severity, UserName AS 'AssignedTo', Customers, Contacts,
                                        ProductName, ReportedVersion, SolvedVersion, GroupName, DateModified, DateCreated, DaysOpened, IsClosed,
                                        CloserName, SlaViolationTime, StatusPosition, SeverityPosition, IsVisibleOnPortal, IsKnowledgeBase, DateClosed,
                                        DaysClosed, CreatorName, ModifierName, HoursSpent, Tags, SlaWarningTime, SlaViolationHours, SlaWarningHours, 
                                        MinsSinceCreated, DaysSinceCreated,
                                        (datediff(mi,'1900-01-01',(select top 1 timeinoldstatus from statushistory where ticketid = tv.ticketid and isnull(timeinoldstatus,-99)<>-99 order by statuschangetime))) as MinutesToFirstResponse
                                    FROM TicketsView tv
                                    WHERE OrganizationID = @OrganizationID and DateModified > @LastMod
                                    ORDER BY TicketNumber";
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
  }


  
}
