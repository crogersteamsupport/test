using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ChatSetting : BaseItem
  {
    public ChatSettingProxy GetProxy()
    {
      ChatSettingProxy result = new ChatSettingProxy();
      result.ClientCss = this.ClientCss;
      result.UseCss = this.UseCss;
      result.OrganizationID = this.OrganizationID;
       
       
       
      return result;
    }	
  }
}
