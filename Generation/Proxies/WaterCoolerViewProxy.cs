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
    [DataMember] public string Message { get; set; }
    [DataMember] public int? MessageParent { get; set; }
    [DataMember] public bool IsDeleted { get; set; }
    [DataMember] public DateTime LastModified { get; set; }
    [DataMember] public int? RefType { get; set; }
    [DataMember] public int? AttachmentID { get; set; }
          
  }
  
  public partial class WaterCoolerViewItem : BaseItem
  {
    public WaterCoolerViewItemProxy GetProxy()
    {
      WaterCoolerViewItemProxy result = new WaterCoolerViewItemProxy();
      result.AttachmentID = this.AttachmentID;
      result.RefType = this.RefType;
      result.IsDeleted = this.IsDeleted;
      result.MessageParent = this.MessageParent;
      result.Message = this.Message;
      result.OrganizationID = this.OrganizationID;
      result.UserID = this.UserID;
      result.MessageID = this.MessageID;
       
      result.TimeStamp = DateTime.SpecifyKind(this.TimeStampUtc, DateTimeKind.Utc);
      result.LastModified = DateTime.SpecifyKind(this.LastModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
