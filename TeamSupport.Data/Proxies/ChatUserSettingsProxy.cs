using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ChatUserSetting : BaseItem
  {
    public ChatUserSettingProxy GetProxy()
    {
      ChatUserSettingProxy result = new ChatUserSettingProxy();
      result.LastChatRequestID = this.LastChatRequestID;
      result.IsAvailable = this.IsAvailable;
      result.CurrentChatID = this.CurrentChatID;
      result.UserID = this.UserID;
       
       
       
      return result;
    }	
  }
}
