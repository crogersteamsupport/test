using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TicketStatus : BaseItem
  {
    public TicketStatusProxy GetProxy()
    {
      TicketStatusProxy result = new TicketStatusProxy();
      result.PauseSLA = this.PauseSLA;
      //result.ModifierID = this.ModifierID;
      //result.CreatorID = this.CreatorID;
      //result.OrganizationID = this.OrganizationID;
      result.IsEmailResponse = this.IsEmailResponse;
      result.IsClosedEmail = this.IsClosedEmail;
      result.IsClosed = this.IsClosed;
      result.TicketTypeID = this.TicketTypeID;
      result.Position = this.Position;
      result.Description = this.Description;
      result.Name = this.Name;
      result.TicketStatusID = this.TicketStatusID;
       
       
       
      return result;
    }	
  }
}
