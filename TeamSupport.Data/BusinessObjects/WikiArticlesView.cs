using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class WikiArticlesViewItem
  {
  }
  
  public partial class WikiArticlesView
  {
    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM WikiArticlesView WHERE OrganizationID = @OrganizationID AND Private <> 1 ORDER BY ArticleName";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public int LoadForGridCount(int organizationID, int userID,  string search)
    {
      if (search.Trim() == "") search = "\"\"";
      using (SqlCommand command = new SqlCommand())
      {
        StringBuilder builder = new StringBuilder();
        builder.Append(@" SELECT COUNT(*)
                          FROM WikiArticlesView wav LEFT JOIN WikiArticles wa ON wav.ArticleID = wa.ArticleID
                          WHERE (wav.OrganizationID = @OrganizationID)
                                  AND (ISNULL(wa.IsDeleted, 0) <> 1)
                                  AND (wa.Private = 0 OR wa.CreatedBy = @UserID)
                                  AND (
                                   (@Search = '""""') OR 
                                   (CONTAINS((wa.[Body]), @Search)) OR 
                                   (CONTAINS((wa.[ArticleName]), @Search))
                                  )
                        ");

        command.CommandText = builder.ToString();
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@Search", search);

        return (int)ExecuteScalar(command, "WikiArticleView");
      }
    }

    public void LoadForGrid(int pageIndex, int pageSize, int organizationID, int userID, string search, string sortColumn, bool sortAsc)
    {
      if (search.Trim() == "") search = @"""""";

      using (SqlCommand command = new SqlCommand())
      {
        string sort = sortColumn;

        StringBuilder builder = new StringBuilder();
        builder.Append("WITH WikiRows AS (SELECT ROW_NUMBER() OVER (ORDER BY wav.");
        builder.Append(sort);
        if (sortAsc) builder.Append(" ASC");
        else builder.Append(" DESC");
        builder.Append(") AS RowNumber, wav.*");
        builder.Append(@" 
                              	  
                                  
                                  FROM WikiArticlesView wav LEFT JOIN WikiArticles wa ON wav.ArticleID = wa.ArticleID
                                  WHERE (wav.OrganizationID = @OrganizationID)
                                  AND (wa.Private = 0 OR wa.CreatedBy = @UserID)
                                  AND (ISNULL(wa.IsDeleted, 0) <> 1)
                                  AND (
                                   (@Search = '""""') OR 
                                   (CONTAINS((wa.[Body]), @Search)) OR 
                                   (CONTAINS((wa.[ArticleName]), @Search))
                                  ))
                              	  
                                  SELECT * FROM WikiRows 
                                  WHERE RowNumber BETWEEN @PageIndex*@PageSize+1 AND @PageIndex*@PageSize+@PageSize
                                  ORDER BY RowNumber ASC");

        command.CommandText = builder.ToString();
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@PageIndex", pageIndex);
        command.Parameters.AddWithValue("@PageSize", pageSize);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@Search", search);

        Fill(command);
      }

    }

    public void LoadForIndexing(int organizationID, int max)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string text = @"
        SELECT TOP {0} ArticleID
        FROM WikiArticlesView w WITH(NOLOCK)
        WHERE w.NeedsIndexing = 1
        AND w.OrganizationID= @OrganizationID
        ORDER BY ModifiedDate DESC";

        command.CommandText = string.Format(text, max.ToString());
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadColumnNames()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT *
        FROM WikiArticlesView w WITH(NOLOCK)
        WHERE 1 = 2";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }
  }
  
}
