using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TicketLinkToTFSItem : BaseItem
  {
    public TicketLinkToTFSItemProxy GetProxy()
    {
      TicketLinkToTFSItemProxy result = new TicketLinkToTFSItemProxy();
      result.CrmLinkID = this.CrmLinkID;
      result.CreatorID = this.CreatorID;
      result.TFSState = this.TFSState;
      result.TFSURL = this.TFSURL;
      result.TFSTitle = this.TFSTitle;
      result.TFSID = this.TFSID;
      result.SyncWithTFS = this.SyncWithTFS;
      result.TicketID = this.TicketID;
      result.id = this.id;
       
       
      result.DateModifiedByTFSSync = this.DateModifiedByTFSSyncUtc == null ? this.DateModifiedByTFSSyncUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedByTFSSyncUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
