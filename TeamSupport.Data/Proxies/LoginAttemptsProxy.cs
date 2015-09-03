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
  [KnownType(typeof(LoginAttemptProxy))]
  public class LoginAttemptProxy
  {
    public LoginAttemptProxy() {}
    [DataMember] public int LoginAttemptID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public bool Successful { get; set; }
    [DataMember] public string IPAddress { get; set; }
    [DataMember] public string Browser { get; set; }
    [DataMember] public string Version { get; set; }
    [DataMember] public string MajorVersion { get; set; }
    [DataMember] public bool? CookiesEnabled { get; set; }
    [DataMember] public string Platform { get; set; }
    [DataMember] public string UserAgent { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
          
  }
  
  public partial class LoginAttempt : BaseItem
  {
    public LoginAttemptProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      LoginAttemptProxy result = new LoginAttemptProxy();
      result.UserAgent = sanitizer.Sanitize(this.UserAgent);
      result.Platform = sanitizer.Sanitize(this.Platform);
      result.CookiesEnabled = this.CookiesEnabled;
      result.MajorVersion = sanitizer.Sanitize(this.MajorVersion);
      result.Version = sanitizer.Sanitize(this.Version);
      result.Browser = sanitizer.Sanitize(this.Browser);
      result.IPAddress = sanitizer.Sanitize(this.IPAddress);
      result.Successful = this.Successful;
      result.UserID = this.UserID;
      result.LoginAttemptID = this.LoginAttemptID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
