using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;


namespace TeamSupport.Data
{
  public partial class Tag : BaseItem
  {
    public TagProxy GetProxy()
    {
      TagProxy result = new TagProxy();

      result.CreatorID = this.CreatorID;
      result.Value = this.Value;
      result.OrganizationID = this.OrganizationID;
      result.TagID = this.TagID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
