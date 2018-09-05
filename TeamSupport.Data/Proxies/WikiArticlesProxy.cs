using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class WikiArticle : BaseItem
  {
    public WikiArticleProxy GetProxy()
    {
      WikiArticleProxy result = new WikiArticleProxy();
      result.NeedsIndexing = this.NeedsIndexing;
      result.ModifiedBy = this.ModifiedBy;
      result.CreatedBy = this.CreatedBy;
      result.IsDeleted = this.IsDeleted;
      result.Private = this.Private;
      result.PortalEdit = this.PortalEdit;
      result.PortalView = this.PortalView;
      result.PublicEdit = this.PublicEdit;
      result.PublicView = this.PublicView;
      result.Version = this.Version;
      result.Body = (this.Body);
      result.ArticleName = (this.ArticleName);
      result.OrganizationID = this.OrganizationID;
      result.ParentID = this.ParentID;
      result.ArticleID = this.ArticleID;
       
       
      result.ModifiedDate = this.ModifiedDateUtc == null ? this.ModifiedDateUtc : DateTime.SpecifyKind((DateTime)this.ModifiedDateUtc, DateTimeKind.Utc); 
      result.CreatedDate = this.CreatedDateUtc == null ? this.CreatedDateUtc : DateTime.SpecifyKind((DateTime)this.CreatedDateUtc, DateTimeKind.Utc);
      result.IsOwner = (BaseCollection.LoginUser.UserID == this.CreatedBy);
			if(BaseCollection.LoginUser.UserID > 0) result.CanDelete = (BaseCollection.LoginUser.GetUser().IsSystemAdmin || result.IsOwner);
       
      return result;
    }
  }
}
