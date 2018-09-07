using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class SlaTriggersViewItem : BaseItem
  {
    public SlaTriggersViewItemProxy GetProxy()
    {
      SlaTriggersViewItemProxy result = new SlaTriggersViewItemProxy();
      result.OrganizationID = this.OrganizationID;
      result.Position = this.Position;
      result.TicketType = this.TicketType;
      result.Severity = this.Severity;
      result.LevelName = this.LevelName;
      result.TimeZone = this.TimeZone;
      result.Weekdays = this.Weekdays;
      result.PauseOnHoliday = this.PauseOnHoliday;
      result.NoBusinessHours = this.NoBusinessHours;
      result.UseBusinessHours = this.UseBusinessHours;
      result.WarningTime = this.WarningTime;
      result.NotifyGroupOnViolation = this.NotifyGroupOnViolation;
      result.NotifyUserOnViolation = this.NotifyUserOnViolation;
      result.NotifyGroupOnWarning = this.NotifyGroupOnWarning;
      result.NotifyUserOnWarning = this.NotifyUserOnWarning;
      result.TimeToClose = this.TimeToClose;
      result.TimeLastAction = this.TimeLastAction;
      result.TimeInitialResponse = this.TimeInitialResponse;
      result.TicketSeverityID = this.TicketSeverityID;
      result.TicketTypeID = this.TicketTypeID;
      result.SlaLevelID = this.SlaLevelID;
      result.SlaTriggerID = this.SlaTriggerID;
       
       
      result.DayEnd = this.DayEndUtc == null ? this.DayEndUtc : DateTime.SpecifyKind((DateTime)this.DayEndUtc, DateTimeKind.Utc); 
      result.DayStart = this.DayStartUtc == null ? this.DayStartUtc : DateTime.SpecifyKind((DateTime)this.DayStartUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
