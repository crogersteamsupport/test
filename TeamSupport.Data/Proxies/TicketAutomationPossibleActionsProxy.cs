using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TicketAutomationPossibleAction : BaseItem
  {
    public TicketAutomationPossibleActionProxy GetProxy()
    {
      TicketAutomationPossibleActionProxy result = new TicketAutomationPossibleActionProxy();
      result.Active = this.Active;
      result.ValueList2 = this.ValueList2;
      result.ValueList = this.ValueList;
      result.RequireValue = this.RequireValue;
      result.ActionName = this.ActionName;
      result.DisplayName = this.DisplayName;
      result.ActionID = this.ActionID;
       
       
       
      return result;
    }	
  }
}
