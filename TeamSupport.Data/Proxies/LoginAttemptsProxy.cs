using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class LoginAttempt : BaseItem
  {
    public LoginAttemptProxy GetProxy()
    {
      LoginAttemptProxy result = new LoginAttemptProxy();
      result.UserAgent = this.UserAgent;
      result.Platform = this.Platform;
      result.CookiesEnabled = this.CookiesEnabled;
      result.MajorVersion = this.MajorVersion;
      result.Version = this.Version;
      result.Browser = this.Browser;
      result.IPAddress = this.IPAddress;
      result.Successful = this.Successful;
      result.UserID = this.UserID;
      result.LoginAttemptID = this.LoginAttemptID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
