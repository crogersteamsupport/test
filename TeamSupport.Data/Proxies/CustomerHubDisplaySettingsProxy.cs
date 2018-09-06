using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class CustomerHubDisplaySetting : BaseItem
  {
    public CustomerHubDisplaySettingProxy GetProxy()
    {
      CustomerHubDisplaySettingProxy result = new CustomerHubDisplaySettingProxy();
      result.ModifierID = this.ModifierID;
      result.Color5 = this.Color5;
      result.Color4 = this.Color4;
      result.Color3 = this.Color3;
      result.Color2 = this.Color2;
      result.Color1 = this.Color1;
      result.FontColor = this.FontColor;
      result.FontFamily = this.FontFamily;
      result.CustomerHubID = this.CustomerHubID;
      result.CustomerHubDisplaySettingID = this.CustomerHubDisplaySettingID;
       
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
