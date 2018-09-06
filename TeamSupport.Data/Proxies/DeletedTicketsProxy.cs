using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class DeletedTicket : BaseItem
  {
    public DeletedTicketProxy GetProxy()
    {
      DeletedTicketProxy result = new DeletedTicketProxy();
      result.DeleterID = this.DeleterID;
      result.Name = this.Name;
      result.OrganizationID = this.OrganizationID;
      result.TicketNumber = this.TicketNumber;
      result.TicketID = this.TicketID;
      result.ID = this.ID;
       
      result.DateDeleted = DateTime.SpecifyKind(this.DateDeletedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
