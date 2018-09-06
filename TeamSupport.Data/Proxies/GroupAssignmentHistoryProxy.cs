using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class GroupAssignmentHistoryItem : BaseItem
  {
    public GroupAssignmentHistoryItemProxy GetProxy()
    {
      GroupAssignmentHistoryItemProxy result = new GroupAssignmentHistoryItemProxy();
      result.GroupID = this.GroupID;
      result.TicketID = this.TicketID;
      result.GroupAssignmentHistoryID = this.GroupAssignmentHistoryID;
       
      result.DateAssigned = DateTime.SpecifyKind(this.DateAssignedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
