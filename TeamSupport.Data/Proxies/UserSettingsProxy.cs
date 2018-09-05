using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class UserSetting : BaseItem
  {
    public UserSettingProxy GetProxy()
    {
      UserSettingProxy result = new UserSettingProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.SettingValue = this.SettingValue;
      result.SettingKey = this.SettingKey;
      result.UserID = this.UserID;
      result.UserSettingID = this.UserSettingID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
