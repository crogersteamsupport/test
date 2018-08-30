using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class UserDevice : BaseItem
  {
    public UserDeviceProxy GetProxy()
    {
      UserDeviceProxy result = new UserDeviceProxy();
      result.IsActivated = this.IsActivated;
      result.DeviceID = this.DeviceID;
      result.UserID = this.UserID;
      result.UserDeviceID = this.UserDeviceID;
       
      result.DateActivated = DateTime.SpecifyKind(this.DateActivatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
