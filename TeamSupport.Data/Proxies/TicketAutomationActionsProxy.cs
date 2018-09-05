using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TicketAutomationAction : BaseItem
  {
    public TicketAutomationActionProxy GetProxy()
    {
      TicketAutomationActionProxy result = new TicketAutomationActionProxy();
      result.ActionValue2 = this.ActionValue2;
      result.ActionValue = this.ActionValue;
      result.ActionID = this.ActionID;
      result.TriggerID = this.TriggerID;
      result.TicketActionID = this.TicketActionID;
       
       
       
      return result;
    }	
  }
}
