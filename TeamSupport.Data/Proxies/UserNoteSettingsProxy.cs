using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
    public partial class UserNoteSetting : BaseItem
    {
        public UserNoteSettingProxy GetProxy()
        {
            UserNoteSettingProxy result = UserNoteSettingProxy.ClassFactory((UserNoteSettingProxy.References)this.RefType); ;
            result.IsSnoozed = this.IsSnoozed;
            result.IsDismissed = this.IsDismissed;
            result.RefID = this.RefID;
            result.UserID = this.UserID;

            result.SnoozeTime = DateTime.SpecifyKind(this.SnoozeTimeUtc, DateTimeKind.Utc);


            return result;
        }
    }
}
