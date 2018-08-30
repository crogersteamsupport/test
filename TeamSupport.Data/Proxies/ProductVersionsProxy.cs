using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ProductVersion : BaseItem
  {
    public ProductVersionProxy GetProxy()
    {
      ProductVersionProxy result = new ProductVersionProxy();
      result.TFSProjectName = this.TFSProjectName;
      result.ImportFileID = this.ImportFileID;
      result.JiraProjectKey = this.JiraProjectKey;
      result.NeedsIndexing = this.NeedsIndexing;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.ImportID = this.ImportID;
      result.Description = (this.Description);
      result.IsReleased = this.IsReleased;
      result.VersionNumber = this.VersionNumber;
      result.ProductVersionStatusID = this.ProductVersionStatusID;
      result.ProductID = this.ProductID;
      result.ProductVersionID = this.ProductVersionID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      result.ReleaseDate = this.ReleaseDateUtc == null ? this.ReleaseDateUtc : DateTime.SpecifyKind((DateTime)this.ReleaseDateUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
