using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class SourceCommitLog : BaseItem
  {
    public SourceCommitLogProxy GetProxy()
    {
      SourceCommitLogProxy result = new SourceCommitLogProxy();
      result.Status = this.Status;
      result.RawCommitText = this.RawCommitText;
      result.Tickets = this.Tickets;
      result.Revision = this.Revision;
      result.Description = this.Description;
      result.UserName = this.UserName;
      result.VersionID = this.VersionID;
      result.ProductID = this.ProductID;
      result.OrganizationID = this.OrganizationID;
      result.CommitID = this.CommitID;
       
       
      result.CommitDateTime = this.CommitDateTimeUtc == null ? this.CommitDateTimeUtc : DateTime.SpecifyKind((DateTime)this.CommitDateTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
