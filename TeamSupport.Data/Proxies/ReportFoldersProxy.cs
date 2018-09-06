using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ReportFolder : BaseItem
  {
    public ReportFolderProxy GetProxy()
    {
      ReportFolderProxy result = new ReportFolderProxy();
      result.CreatorID = this.CreatorID;
      result.ParentID = this.ParentID;
      result.Name = (this.Name);
      result.FolderID = this.FolderID;
      result.OrganizationID = this.OrganizationID;
       
       
      result.DateCreated = this.DateCreatedUtc == null ? this.DateCreatedUtc : DateTime.SpecifyKind((DateTime)this.DateCreatedUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
