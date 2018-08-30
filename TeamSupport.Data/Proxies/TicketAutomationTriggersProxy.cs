using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TicketAutomationTrigger : BaseItem
  {
    public TicketAutomationTriggerProxy GetProxy()
    {
      TicketAutomationTriggerProxy result = new TicketAutomationTriggerProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.CustomSQL = this.CustomSQL;
      result.UseCustomSQL = this.UseCustomSQL;
      result.OrganizationID = this.OrganizationID;
      result.Position = this.Position;
      result.Active = this.Active;
      result.Name = this.Name;
      result.TriggerID = this.TriggerID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
