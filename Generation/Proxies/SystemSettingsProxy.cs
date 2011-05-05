using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(SystemSettingProxy))]
  public class SystemSettingProxy
  {
    public SystemSettingProxy() {}
    [DataMember] public int SystemSettingID { get; set; }
    [DataMember] public string SettingKey { get; set; }
    [DataMember] public string SettingValue { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
          
  }
  
  public partial class SystemSetting : BaseItem
  {
    public SystemSettingProxy GetProxy()
    {
      SystemSettingProxy result = new SystemSettingProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.SettingValue = this.SettingValue;
      result.SettingKey = this.SettingKey;
      result.SystemSettingID = this.SystemSettingID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreated, DateTimeKind.Local);
      result.DateModified = DateTime.SpecifyKind(this.DateModified, DateTimeKind.Local);
       
       
      return result;
    }	
  }
}
