using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TicketAutomationTriggerLogicItem : BaseItem
  {
    public TicketAutomationTriggerLogicItemProxy GetProxy()
    {
      TicketAutomationTriggerLogicItemProxy result = new TicketAutomationTriggerLogicItemProxy();
      result.MatchAll = this.MatchAll;
      result.TestValue = this.TestValue;
      result.Measure = this.Measure;
      result.FieldID = this.FieldID;
      result.TableID = this.TableID;
      result.TriggerID = this.TriggerID;
      result.TriggerLogicID = this.TriggerLogicID;
       
       
       
      return result;
    }	
  }
}
