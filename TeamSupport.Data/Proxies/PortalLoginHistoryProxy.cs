using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class PortalLoginHistoryItem : BaseItem
  {
    public PortalLoginHistoryItemProxy GetProxy()
    {
      PortalLoginHistoryItemProxy result = new PortalLoginHistoryItemProxy();
      result.Source = this.Source;
      result.UserID = this.UserID;
      result.Browser = this.Browser;
      result.IPAddress = this.IPAddress;
      result.Success = this.Success;
      result.OrganizationName = this.OrganizationName;
      result.OrganizationID = this.OrganizationID;
      result.UserName = this.UserName;
      result.PortalLoginID = this.PortalLoginID;
       
       
      result.LoginDateTime = this.LoginDateTimeUtc == null ? this.LoginDateTimeUtc : DateTime.SpecifyKind((DateTime)this.LoginDateTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
