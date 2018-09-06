using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class LoginHistoryItem : BaseItem
  {
    public LoginHistoryItemProxy GetProxy()
    {
      LoginHistoryItemProxy result = new LoginHistoryItemProxy();
      result.URL = this.URL;
      result.ScreenWidth = this.ScreenWidth;
      result.ScreenHeight = this.ScreenHeight;
      result.PixelDepth = this.PixelDepth;
      result.Language = this.Language;
      result.UserAgent = this.UserAgent;
      result.Platform = this.Platform;
      result.CookiesEnabled = this.CookiesEnabled;
      result.MajorVersion = this.MajorVersion;
      result.Version = this.Version;
      result.Browser = this.Browser;
      result.IPAddress = this.IPAddress;
      result.UserID = this.UserID;
      result.LoginHistoryID = this.LoginHistoryID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
