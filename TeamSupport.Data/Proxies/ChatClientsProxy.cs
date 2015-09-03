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
  [KnownType(typeof(ChatClientProxy))]
  public class ChatClientProxy
  {
    public ChatClientProxy() {}
    [DataMember] public int ChatClientID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string FirstName { get; set; }
    [DataMember] public string LastName { get; set; }
    [DataMember] public string Email { get; set; }
    [DataMember] public string CompanyName { get; set; }
    [DataMember] public DateTime LastPing { get; set; }
    [DataMember] public int? LinkedUserID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
          
  }
  
  public partial class ChatClient : BaseItem
  {
    public ChatClientProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      ChatClientProxy result = new ChatClientProxy();
      result.LinkedUserID = this.LinkedUserID;
      result.CompanyName = sanitizer.Sanitize(this.CompanyName);
      result.Email = sanitizer.Sanitize(this.Email);
      result.LastName = sanitizer.Sanitize(this.LastName);
      result.FirstName = sanitizer.Sanitize(this.FirstName);
      result.OrganizationID = this.OrganizationID;
      result.ChatClientID = this.ChatClientID;
       
      result.LastPing = DateTime.SpecifyKind(this.LastPingUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
