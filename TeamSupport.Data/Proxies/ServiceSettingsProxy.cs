using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ServiceSetting : BaseItem
  {
    public ServiceSettingProxy GetProxy()
    {
      ServiceSettingProxy result = new ServiceSettingProxy();
      result.SettingValue = this.SettingValue;
      result.SettingKey = this.SettingKey;
      result.ServiceID = this.ServiceID;
      result.ServiceSettingID = this.ServiceSettingID;
       
       
       
      return result;
    }	
  }
}
