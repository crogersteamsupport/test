using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class Product : BaseItem
  {
    public ProductProxy GetProxy()
    {
      ProductProxy result = new ProductProxy();
      result.TFSProjectName = this.TFSProjectName;
      result.SlaLevelID = this.SlaLevelID;
      result.EmailReplyToAddress = this.EmailReplyToAddress;
      result.ImportFileID = this.ImportFileID;
      result.JiraProjectKey = this.JiraProjectKey;
      result.ProductFamilyID = this.ProductFamilyID;
      result.NeedsIndexing = this.NeedsIndexing;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.ImportID = this.ImportID;
      result.Description = this.Description;
      result.Name = this.Name;
      result.OrganizationID = this.OrganizationID;
      result.ProductID = this.ProductID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
