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
  [KnownType(typeof(WaterCoolerViewItemProxy))]
  public class WaterCoolerViewItemProxy
  {
    public WaterCoolerViewItemProxy() {}
    [DataMember] public int MessageID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public DateTime TimeStamp { get; set; }
    [DataMember] public int? GroupFor { get; set; }
    [DataMember] public int? ReplyTo { get; set; }
    [DataMember] public string Message { get; set; }
    [DataMember] public string MessageType { get; set; }
    [DataMember] public string UserName { get; set; }
    [DataMember] public string GroupName { get; set; }
          
  }
  
  public partial class WaterCoolerViewItem : BaseItem
  {
    public WaterCoolerViewItemProxy GetProxy()
    {
      WaterCoolerViewItemProxy result = new WaterCoolerViewItemProxy();
      result.GroupName = this.GroupName;
      result.UserName = this.UserName;
      result.MessageType = this.MessageType;
      result.Message = this.Message;
      result.ReplyTo = this.ReplyTo;
      result.GroupFor = this.GroupFor;
      result.OrganizationID = this.OrganizationID;
      result.UserID = this.UserID;
      result.MessageID = this.MessageID;
       
      result.TimeStamp = DateTime.SpecifyKind(this.TimeStampUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
