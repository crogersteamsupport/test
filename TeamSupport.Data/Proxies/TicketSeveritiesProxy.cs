using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;


namespace TeamSupport.Data
{
  public partial class TicketSeverity : BaseItem
  {
    public TicketSeverityProxy GetProxy()
    {
      TicketSeverityProxy result = new TicketSeverityProxy();
      //result.ModifierID = this.ModifierID;
      //result.CreatorID = this.CreatorID;
      //result.OrganizationID = this.OrganizationID;
      result.Position = this.Position;
      result.Description = (this.Description);
      result.Name = (this.Name);
      result.TicketSeverityID = this.TicketSeverityID;
      result.VisibleOnPortal = this.VisibleOnPortal;
       
       
      return result;
    }	
  }
}
