using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class WikiArticle
  {
  }
  
  public partial class WikiArticles
  {
    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM WikiArticles WHERE OrganizationID = @OrganizationID ORDER BY ArticleName";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        Fill(command);
      }
    }

    public void LoadByOrganizationIDAndUserID()
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT * FROM WikiArticles WHERE ([OrganizationID] = @OrganizationID) and ((IsNull(Private,0)=0) or (CreatedBy=@UserID)) and IsNull(IsDeleted,0)=0 ORDER BY ArticleName";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
            command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
            Fill(command);
        }
    }

    public void LoadParentsByOrganizationID()
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT * FROM WikiArticles WHERE ([OrganizationID] = @OrganizationID) and ((IsNull(Private,0)=0) or (CreatedBy=@UserID)) and IsNull(IsDeleted,0)=0 and ParentID is null ORDER BY ArticleName";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
            command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
            Fill(command);
        }
    }

    public void LoadSubArticlesByParentID(int parentID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            //command.CommandText = "SELECT * FROM WikiArticles WHERE ParentID = @ParentID ORDER BY ArticleName";
            command.CommandText = "SELECT * FROM WikiArticles WHERE ParentID = @ParentID and ([OrganizationID] = @OrganizationID) and ((IsNull(Private,0)=0) or (CreatedBy=@UserID)) and IsNull(IsDeleted,0)=0 ORDER BY ArticleName";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ParentID", parentID);
            command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
            command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
            Fill(command);
        }
    }

    public void LoadRevisionsByArticleID(int articleID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT * FROM WikiHistory WHERE ArticleID = @ArticleID ORDER BY ArticleName";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ArticleID", articleID);
            Fill(command);
        }
    }

    public void LoadBySearchTerm(string term)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT * FROM WikiArticles WHERE ([OrganizationID] = @OrganizationID) and ((IsNull(Private,0)=0) or (CreatedBy=@UserID)) and IsNull(IsDeleted,0)=0 AND ArticleName LIKE '%'+@Value+'%' ORDER BY ArticleName";
            //command.CommandText = "SELECT * FROM Tags WHERE OrganizationID = @OrganizationID AND Value LIKE @Value+'%' ORDER BY Value";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
            command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
            command.Parameters.AddWithValue("@Value", term);
            Fill(command);
        }

    }
  }
  
}
