using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class SystemSetting : BaseItem
  {
    public SystemSettingProxy GetProxy()
    {
      SystemSettingProxy result = new SystemSettingProxy();
      result.SettingValue = this.SettingValue;
      result.SettingKey = this.SettingKey;
      result.SystemSettingID = this.SystemSettingID;
       
       
       
      return result;
    }	
  }
}
