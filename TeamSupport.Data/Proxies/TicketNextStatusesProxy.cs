using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TicketNextStatus : BaseItem
  {
    public TicketNextStatusProxy GetProxy()
    {
      TicketNextStatusProxy result = new TicketNextStatusProxy();
      result.Position = this.Position;
      result.NextStatusID = this.NextStatusID;
      result.CurrentStatusID = this.CurrentStatusID;
      result.TicketNextStatusID = this.TicketNextStatusID;
       
       
       
      return result;
    }	
  }
}
