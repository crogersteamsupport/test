using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class Email : BaseItem
  {
    public EmailProxy GetProxy()
    {
      EmailProxy result = new EmailProxy();
      result.EmailPostID = this.EmailPostID;
      result.LastFailedReason = this.LastFailedReason;
      result.Attempts = this.Attempts;
      result.IsHtml = this.IsHtml;
      result.IsWaiting = this.IsWaiting;
      result.IsSuccess = this.IsSuccess;
      result.Size = this.Size;
      result.Attachments = this.Attachments;
      result.Body = this.Body;
      result.Subject = this.Subject;
      result.BCCAddress = this.BCCAddress;
      result.CCAddress = this.CCAddress;
      result.ToAddress = this.ToAddress;
      result.FromAddress = this.FromAddress;
      result.Description = this.Description;
      result.OrganizationID = this.OrganizationID;
      result.EmailID = this.EmailID;
       
      result.NextAttempt = DateTime.SpecifyKind(this.NextAttemptUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
      result.DateSent = this.DateSentUtc == null ? this.DateSentUtc : DateTime.SpecifyKind((DateTime)this.DateSentUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
