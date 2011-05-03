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
  [KnownType(typeof(AttachmentDownloadProxy))]
  public class AttachmentDownloadProxy
  {
    public AttachmentDownloadProxy() {}
    [DataMember] public int AttachmentDownloadID { get; set; }
    [DataMember] public int AttachmentID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public DateTime DateDownloaded { get; set; }
          
  }
  
  public partial class AttachmentDownload : BaseItem
  {
    public AttachmentDownloadProxy GetProxy()
    {
      AttachmentDownloadProxy result = new AttachmentDownloadProxy();
      result.UserID = this.UserID;
      result.AttachmentID = this.AttachmentID;
      result.AttachmentDownloadID = this.AttachmentDownloadID;
       
      result.DateDownloaded = DateTime.SpecifyKind(this.DateDownloaded, DateTimeKind.Local);
       
       
      return result;
    }	
  }
}
