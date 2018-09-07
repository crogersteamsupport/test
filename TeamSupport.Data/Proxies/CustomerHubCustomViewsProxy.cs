using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class CustomerHubCustomView : BaseItem
  {
    public CustomerHubCustomViewProxy GetProxy()
    {
      CustomerHubCustomViewProxy result = new CustomerHubCustomViewProxy();
      result.ModifierID = this.ModifierID;
      result.IsActive = this.IsActive;
      result.CustomView = this.CustomView;
      result.CustomerHubViewID = this.CustomerHubViewID;
      result.CustomerHubID = this.CustomerHubID;
      result.CustomerHubCustomViewID = this.CustomerHubCustomViewID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
