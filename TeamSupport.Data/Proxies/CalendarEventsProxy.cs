using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class CalendarEvent : BaseItem
  {
    public CalendarEventProxy GetProxy()
    {
      CalendarEventProxy result = new CalendarEventProxy();
      result.IsHoliday = this.IsHoliday;
      result.AllDay = this.AllDay;
      result.CreatorID = this.CreatorID;
      result.RepeatFrequency = this.RepeatFrequency;
      result.Repeat = this.Repeat;
      result.Description = this.Description;
      result.Title = this.Title;
      result.OrganizationID = this.OrganizationID;
      result.CalendarID = this.CalendarID;
       
      result.StartDate = DateTime.SpecifyKind(this.StartDateUtc, DateTimeKind.Utc);
      result.LastModified = DateTime.SpecifyKind(this.LastModifiedUtc, DateTimeKind.Utc);
       
      result.EndDateUTC = this.EndDateUTCUtc == null ? this.EndDateUTCUtc : DateTime.SpecifyKind((DateTime)this.EndDateUTCUtc, DateTimeKind.Utc); 
      result.StartDateUTC = this.StartDateUTCUtc == null ? this.StartDateUTCUtc : DateTime.SpecifyKind((DateTime)this.StartDateUTCUtc, DateTimeKind.Utc); 
      result.EndDate = this.EndDateUtc == null ? this.EndDateUtc : DateTime.SpecifyKind((DateTime)this.EndDateUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
