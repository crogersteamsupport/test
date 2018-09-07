using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TicketAutomationHistoryItem : BaseItem
  {
    public TicketAutomationHistoryItemProxy GetProxy()
    {
      TicketAutomationHistoryItemProxy result = new TicketAutomationHistoryItemProxy();
      result.ActionType = this.ActionType;
      result.OrganizationID = this.OrganizationID;
      result.TriggerID = this.TriggerID;
      result.TicketID = this.TicketID;
      result.HistoryID = this.HistoryID;
       
       
      result.TriggerDateTime = this.TriggerDateTimeUtc == null ? this.TriggerDateTimeUtc : DateTime.SpecifyKind((DateTime)this.TriggerDateTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
