using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TicketLinkToSnowItem : BaseItem
  {
    public TicketLinkToSnowItemProxy GetProxy()
    {
      TicketLinkToSnowItemProxy result = new TicketLinkToSnowItemProxy();
      result.CrmLinkID = this.CrmLinkID;
      result.CreatorID = this.CreatorID;
      result.State = this.State;
      result.URL = this.URL;
      result.Number = this.Number;
      result.AppId = this.AppId;
      result.Sync = this.Sync;
      result.TicketID = this.TicketID;
      result.Id = this.Id;
       
       
      result.DateModifiedBySync = this.DateModifiedBySyncUtc == null ? this.DateModifiedBySyncUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedBySyncUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
