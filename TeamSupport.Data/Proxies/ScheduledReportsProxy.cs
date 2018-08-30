using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ScheduledReport : BaseItem
  {
    public ScheduledReportProxy GetProxy()
    {
      ScheduledReportProxy result = new ScheduledReportProxy();
      result.FilePathID = this.FilePathID;
      result.LockProcessId = this.LockProcessId;
      result.ModifierId = this.ModifierId;
      result.CreatorId = this.CreatorId;
      result.IsSuccessful = this.IsSuccessful;
      result.Monthday = this.Monthday;
      result.Weekday = this.Weekday;
      result.Every = this.Every;
      result.RecurrencyId = this.RecurrencyId;
      result.IsActive = this.IsActive;
      result.RunCount = this.RunCount;
      result.OrganizationId = this.OrganizationId;
      result.ReportId = this.ReportId;
      result.EmailRecipients = this.EmailRecipients;
      result.EmailBody = this.EmailBody;
      result.EmailSubject = this.EmailSubject;
      result.Id = this.Id;
       
      result.StartDate = DateTime.SpecifyKind(this.StartDateUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
      result.DateModified = this.DateModifiedUtc == null ? this.DateModifiedUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedUtc, DateTimeKind.Utc); 
      result.NextRun = this.NextRunUtc == null ? this.NextRunUtc : DateTime.SpecifyKind((DateTime)this.NextRunUtc, DateTimeKind.Utc); 
      result.LastRun = this.LastRunUtc == null ? this.LastRunUtc : DateTime.SpecifyKind((DateTime)this.LastRunUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
