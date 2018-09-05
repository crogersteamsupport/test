using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class UserAssignmentHistoryItem : BaseItem
  {
    public UserAssignmentHistoryItemProxy GetProxy()
    {
      UserAssignmentHistoryItemProxy result = new UserAssignmentHistoryItemProxy();
      result.UserID = this.UserID;
      result.TicketID = this.TicketID;
      result.UserAssignmentHistoryID = this.UserAssignmentHistoryID;
       
      result.DateAssigned = DateTime.SpecifyKind(this.DateAssignedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
