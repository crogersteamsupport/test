using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TicketLinkToJiraItem : BaseItem
  {
    public TicketLinkToJiraItemProxy GetProxy()
    {
      TicketLinkToJiraItemProxy result = new TicketLinkToJiraItemProxy();
      result.CrmLinkID = this.CrmLinkID;
      result.CreatorID = this.CreatorID;
      result.JiraStatus = this.JiraStatus;
      result.JiraLinkURL = this.JiraLinkURL;
      result.JiraKey = this.JiraKey;
      result.JiraID = this.JiraID;
      result.SyncWithJira = this.SyncWithJira;
      result.TicketID = this.TicketID;
      result.id = this.id;
       
       
      result.DateModifiedByJiraSync = this.DateModifiedByJiraSyncUtc == null ? this.DateModifiedByJiraSyncUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedByJiraSyncUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
