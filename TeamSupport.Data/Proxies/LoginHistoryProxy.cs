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
  [KnownType(typeof(LoginHistoryItemProxy))]
  public class LoginHistoryItemProxy
  {
    public LoginHistoryItemProxy() {}
    [DataMember] public int LoginHistoryID { get; set; }
    [DataMember] public int? UserID { get; set; }
    [DataMember] public string IPAddress { get; set; }
    [DataMember] public string Browser { get; set; }
    [DataMember] public string Version { get; set; }
    [DataMember] public string MajorVersion { get; set; }
    [DataMember] public bool? CookiesEnabled { get; set; }
    [DataMember] public string Platform { get; set; }
    [DataMember] public string UserAgent { get; set; }
    [DataMember] public string Language { get; set; }
    [DataMember] public string PixelDepth { get; set; }
    [DataMember] public string ScreenHeight { get; set; }
    [DataMember] public string ScreenWidth { get; set; }
    [DataMember] public string URL { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
          
  }
  
  public partial class LoginHistoryItem : BaseItem
  {
    public LoginHistoryItemProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      LoginHistoryItemProxy result = new LoginHistoryItemProxy();
      result.URL = sanitizer.Sanitize(this.URL);
      result.ScreenWidth = sanitizer.Sanitize(this.ScreenWidth);
      result.ScreenHeight = sanitizer.Sanitize(this.ScreenHeight);
      result.PixelDepth = sanitizer.Sanitize(this.PixelDepth);
      result.Language = sanitizer.Sanitize(this.Language);
      result.UserAgent = sanitizer.Sanitize(this.UserAgent);
      result.Platform = sanitizer.Sanitize(this.Platform);
      result.CookiesEnabled = this.CookiesEnabled;
      result.MajorVersion = sanitizer.Sanitize(this.MajorVersion);
      result.Version = sanitizer.Sanitize(this.Version);
      result.Browser = sanitizer.Sanitize(this.Browser);
      result.IPAddress = sanitizer.Sanitize(this.IPAddress);
      result.UserID = this.UserID;
      result.LoginHistoryID = this.LoginHistoryID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
