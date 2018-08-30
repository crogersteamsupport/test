using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class KBStat : BaseItem
  {
    public KBStatProxy GetProxy()
    {
      KBStatProxy result = new KBStatProxy();
      result.SearchTerm = this.SearchTerm;
      result.ViewIP = this.ViewIP;
      result.KBArticleID = this.KBArticleID;
      result.OrganizationID = this.OrganizationID;
      result.KBViewID = this.KBViewID;
       
       
      result.ViewDateTime = this.ViewDateTimeUtc == null ? this.ViewDateTimeUtc : DateTime.SpecifyKind((DateTime)this.ViewDateTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
