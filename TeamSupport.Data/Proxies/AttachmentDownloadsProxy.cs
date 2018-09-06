using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class AttachmentDownload : BaseItem
  {
    public AttachmentDownloadProxy GetProxy()
    {
      AttachmentDownloadProxy result = new AttachmentDownloadProxy();
      result.UserID = this.UserID;
      result.AttachmentID = this.AttachmentID;
      result.AttachmentDownloadID = this.AttachmentDownloadID;
       
      result.DateDownloaded = DateTime.SpecifyKind(this.DateDownloadedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
