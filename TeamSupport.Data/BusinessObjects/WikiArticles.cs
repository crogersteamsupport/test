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
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByOrganizationIDAndUserID(int organizationID, int userID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT * FROM WikiArticles WHERE ([OrganizationID] = @OrganizationID) and ((IsNull(Private,0)=0) or (CreatedBy=@UserID)) and IsNull(IsDeleted,0)=0 ORDER BY ArticleName";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@UserID", userID);
            Fill(command);
        }
    }

    public void LoadParentsByOrganizationID(int organizationID, int userID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT * FROM WikiArticles WHERE ([OrganizationID] = @OrganizationID) and ((IsNull(Private,0)=0) or (CreatedBy=@UserID)) and IsNull(IsDeleted,0)=0 and ParentID is null ORDER BY ArticleName";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@UserID", userID);
            Fill(command);
        }
    }

    public void LoadSubArticlesByParentID(int parentID, int organizationID, int userID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            //command.CommandText = "SELECT * FROM WikiArticles WHERE ParentID = @ParentID ORDER BY ArticleName";
            command.CommandText = "SELECT * FROM WikiArticles WHERE ParentID = @ParentID and ([OrganizationID] = @OrganizationID) and ((IsNull(Private,0)=0) or (CreatedBy=@UserID)) and IsNull(IsDeleted,0)=0 ORDER BY ArticleName";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ParentID", parentID);
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@UserID", userID);
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
  }
  
}
