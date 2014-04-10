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
  [KnownType(typeof(UserNoteSettingProxy))]
  public class UserNoteSettingProxy
  {
    public UserNoteSettingProxy() {}
    [DataMember] public int UserID { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
    [DataMember] public bool IsDismissed { get; set; }
    [DataMember] public bool IsSnoozed { get; set; }
    [DataMember] public DateTime SnoozeTime { get; set; }
          
  }
  
  public partial class UserNoteSetting : BaseItem
  {
    public UserNoteSettingProxy GetProxy()
    {
      UserNoteSettingProxy result = new UserNoteSettingProxy();
      result.IsSnoozed = this.IsSnoozed;
      result.IsDismissed = this.IsDismissed;
      result.RefType = this.RefType;
      result.RefID = this.RefID;
      result.UserID = this.UserID;
       
      result.SnoozeTime = DateTime.SpecifyKind(this.SnoozeTimeUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
