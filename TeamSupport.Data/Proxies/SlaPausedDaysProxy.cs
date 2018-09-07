using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class SlaPausedDay : BaseItem
  {
    public SlaPausedDayProxy GetProxy()
    {
      SlaPausedDayProxy result = new SlaPausedDayProxy();
      result.SlaTriggerId = this.SlaTriggerId;
      result.Id = this.Id;
       
      result.DateToPause = DateTime.SpecifyKind(this.DateToPauseUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
