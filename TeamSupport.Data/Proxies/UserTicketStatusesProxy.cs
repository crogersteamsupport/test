using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class UserTicketStatus : BaseItem
  {
    public UserTicketStatusProxy GetProxy()
    {
      UserTicketStatusProxy result = new UserTicketStatusProxy();
      result.IsFlagged = this.IsFlagged;
      result.UserID = this.UserID;
      result.TicketID = this.TicketID;
      result.UserTicketStatusID = this.UserTicketStatusID;
       
      result.DateRead = DateTime.SpecifyKind(this.DateReadUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
