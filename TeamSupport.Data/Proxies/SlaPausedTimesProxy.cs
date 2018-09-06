using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class SlaPausedTime : BaseItem
  {
    public SlaPausedTimProxy GetProxy()
    {
      SlaPausedTimProxy result = new SlaPausedTimProxy();
      result.BusinessPausedTime = this.BusinessPausedTime;
      result.SlaTriggerId = this.SlaTriggerId;
      result.TicketStatusId = this.TicketStatusId;
      result.TicketId = this.TicketId;
      result.Id = this.Id;
       
      result.PausedOn = DateTime.SpecifyKind(this.PausedOnUtc, DateTimeKind.Utc);
       
      result.ResumedOn = this.ResumedOnUtc == null ? this.ResumedOnUtc : DateTime.SpecifyKind((DateTime)this.ResumedOnUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
