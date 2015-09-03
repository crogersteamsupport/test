using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Ganss.XSS;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(ChatSettingProxy))]
  public class ChatSettingProxy
  {
    public ChatSettingProxy() {}
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public bool UseCss { get; set; }
    [DataMember] public string ClientCss { get; set; }
          
  }
  
  public partial class ChatSetting : BaseItem
  {
    public ChatSettingProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      ChatSettingProxy result = new ChatSettingProxy();
      result.ClientCss = sanitizer.Sanitize(this.ClientCss);
      result.UseCss = this.UseCss;
      result.OrganizationID = this.OrganizationID;
       
       
       
      return result;
    }	
  }
}
