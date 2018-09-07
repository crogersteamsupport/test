using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class SlaNotification : BaseItem
  {
    public SlaNotificationProxy GetProxy()
    {
      SlaNotificationProxy result = new SlaNotificationProxy();
      result.TicketID = this.TicketID;
       
       
      result.InitialResponseWarningDate = this.InitialResponseWarningDateUtc == null ? this.InitialResponseWarningDateUtc : DateTime.SpecifyKind((DateTime)this.InitialResponseWarningDateUtc, DateTimeKind.Utc); 
      result.LastActionWarningDate = this.LastActionWarningDateUtc == null ? this.LastActionWarningDateUtc : DateTime.SpecifyKind((DateTime)this.LastActionWarningDateUtc, DateTimeKind.Utc); 
      result.TimeClosedWarningDate = this.TimeClosedWarningDateUtc == null ? this.TimeClosedWarningDateUtc : DateTime.SpecifyKind((DateTime)this.TimeClosedWarningDateUtc, DateTimeKind.Utc); 
      result.InitialResponseViolationDate = this.InitialResponseViolationDateUtc == null ? this.InitialResponseViolationDateUtc : DateTime.SpecifyKind((DateTime)this.InitialResponseViolationDateUtc, DateTimeKind.Utc); 
      result.LastActionViolationDate = this.LastActionViolationDateUtc == null ? this.LastActionViolationDateUtc : DateTime.SpecifyKind((DateTime)this.LastActionViolationDateUtc, DateTimeKind.Utc); 
      result.TimeClosedViolationDate = this.TimeClosedViolationDateUtc == null ? this.TimeClosedViolationDateUtc : DateTime.SpecifyKind((DateTime)this.TimeClosedViolationDateUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
