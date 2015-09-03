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
  [KnownType(typeof(EmailActionProxy))]
  public class EmailActionProxy
  {
    public EmailActionProxy() {}
    [DataMember] public int EMailActionID { get; set; }
    [DataMember] public DateTime? DateTime { get; set; }
    [DataMember] public string EMailFrom { get; set; }
    [DataMember] public string EMailTo { get; set; }
    [DataMember] public string EMailSubject { get; set; }
    [DataMember] public string EMailBody { get; set; }
    [DataMember] public string OrganizationGUID { get; set; }
    [DataMember] public bool? ActionAdded { get; set; }
    [DataMember] public string Status { get; set; }
    [DataMember] public int? TicketID { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
          
  }
  
  public partial class EmailAction : BaseItem
  {
    public EmailActionProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      EmailActionProxy result = new EmailActionProxy();
      result.OrganizationID = this.OrganizationID;
      result.TicketID = this.TicketID;
      result.Status = sanitizer.Sanitize(this.Status);
      result.ActionAdded = this.ActionAdded;
      result.OrganizationGUID = sanitizer.Sanitize(this.OrganizationGUID);
      result.EMailBody = sanitizer.Sanitize(this.EMailBody);
      result.EMailSubject = sanitizer.Sanitize(this.EMailSubject);
      result.EMailTo = sanitizer.Sanitize(this.EMailTo);
      result.EMailFrom = sanitizer.Sanitize(this.EMailFrom);
      result.EMailActionID = this.EMailActionID;
       
       
      //result.DateTime = this.DateTimeUtc == null ? this.DateTimeUtc : DateTime.SpecifyKind((DateTime)this.DateTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
