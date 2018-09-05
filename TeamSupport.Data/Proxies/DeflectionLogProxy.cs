using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class DeflectionLogItem : BaseItem
  {
    public DeflectionLogItemProxy GetProxy()
    {
      DeflectionLogItemProxy result = new DeflectionLogItemProxy();
      result.OrgID = this.OrgID;
      result.UserID = this.UserID;
      result.Helpful = this.Helpful;
      result.Source = this.Source;
      result.TicketID = this.TicketID;
      result.Id = this.Id;
       
      result.Date = DateTime.SpecifyKind(this.DateUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
