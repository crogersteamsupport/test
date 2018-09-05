using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ActionLinkToSnowItem : BaseItem
  {
    public ActionLinkToSnowItemProxy GetProxy()
    {
      ActionLinkToSnowItemProxy result = new ActionLinkToSnowItemProxy();
      result.AppId = this.AppId;
      result.ActionID = this.ActionID;
      result.Id = this.Id;
       
       
      result.DateModifiedBySync = this.DateModifiedBySyncUtc == null ? this.DateModifiedBySyncUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedBySyncUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
