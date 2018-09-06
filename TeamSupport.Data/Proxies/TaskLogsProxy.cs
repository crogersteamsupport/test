using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TaskLog : BaseItem
  {
    public TaskLogProxy GetProxy()
    {
      TaskLogProxy result = new TaskLogProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.Description = this.Description;
      result.TaskID = this.TaskID;
      result.TaskLogID = this.TaskLogID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
