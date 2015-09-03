using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Ganss.XSS;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(EmailProxy))]
  public class EmailProxy
  {
    public EmailProxy() {}
    [DataMember] public int EmailID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public string FromAddress { get; set; }
    [DataMember] public string ToAddress { get; set; }
    [DataMember] public string CCAddress { get; set; }
    [DataMember] public string BCCAddress { get; set; }
    [DataMember] public string Subject { get; set; }
    [DataMember] public string Body { get; set; }
    [DataMember] public string Attachments { get; set; }
    [DataMember] public int Size { get; set; }
    [DataMember] public bool IsSuccess { get; set; }
    [DataMember] public bool IsWaiting { get; set; }
    [DataMember] public bool IsHtml { get; set; }
    [DataMember] public int Attempts { get; set; }
    [DataMember] public DateTime NextAttempt { get; set; }
    [DataMember] public DateTime? DateSent { get; set; }
    [DataMember] public string LastFailedReason { get; set; }
    [DataMember] public int? EmailPostID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
          
  }
  
  public partial class Email : BaseItem
  {
    public EmailProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      EmailProxy result = new EmailProxy();
      result.EmailPostID = this.EmailPostID;
      result.LastFailedReason = sanitizer.Sanitize(this.LastFailedReason);
      result.Attempts = this.Attempts;
      result.IsHtml = this.IsHtml;
      result.IsWaiting = this.IsWaiting;
      result.IsSuccess = this.IsSuccess;
      result.Size = this.Size;
      result.Attachments = sanitizer.Sanitize(this.Attachments);
      result.Body = sanitizer.Sanitize(this.Body);
      result.Subject = sanitizer.Sanitize(this.Subject);
      result.BCCAddress = sanitizer.Sanitize(this.BCCAddress);
      result.CCAddress = sanitizer.Sanitize(this.CCAddress);
      result.ToAddress = sanitizer.Sanitize(this.ToAddress);
      result.FromAddress = sanitizer.Sanitize(this.FromAddress);
      result.Description = sanitizer.Sanitize(this.Description);
      result.OrganizationID = this.OrganizationID;
      result.EmailID = this.EmailID;
       
      result.NextAttempt = DateTime.SpecifyKind(this.NextAttemptUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
      result.DateSent = this.DateSentUtc == null ? this.DateSentUtc : DateTime.SpecifyKind((DateTime)this.DateSentUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
