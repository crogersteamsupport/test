using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TokStorageItem : BaseItem
  {
    public TokStorageItemProxy GetProxy()
    {
      TokStorageItemProxy result = new TokStorageItemProxy();
      result.ArchiveID = this.ArchiveID;
      result.CreatorID = this.CreatorID;
      result.AmazonPath = this.AmazonPath;
      result.OrganizationID = this.OrganizationID;
      result.Transcoded = this.Transcoded;
      result.CreatedDate = DateTime.SpecifyKind(this.CreatedDateUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
