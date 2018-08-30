using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class CRMLinkResult : BaseItem
  {
    public CRMLinkResultProxy GetProxy()
    {
      CRMLinkResultProxy result = new CRMLinkResultProxy();
      result.AttemptResult = this.AttemptResult;
      result.OrganizationID = this.OrganizationID;
      result.CRMResultsID = this.CRMResultsID;
       
       
      result.AttemptDateTime = this.AttemptDateTimeUtc == null ? this.AttemptDateTimeUtc : DateTime.SpecifyKind((DateTime)this.AttemptDateTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
