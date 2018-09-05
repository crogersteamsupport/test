using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ForumLog : BaseItem
  {
    public ForumLogProxy GetProxy()
    {
      ForumLogProxy result = new ForumLogProxy();
      result.OrgID = this.OrgID;
      result.UserID = this.UserID;
      result.TopicID = this.TopicID;
      result.ForumLogID = this.ForumLogID;
       
      result.ViewTime = DateTime.SpecifyKind(this.ViewTimeUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
