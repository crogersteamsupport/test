using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class WaterCoolerItem : BaseItem
  {
    public WaterCoolerItemProxy GetProxy()
    {
      WaterCoolerItemProxy result = new WaterCoolerItemProxy();

      result.MessageType = this.MessageType;
      result.Message = (this.Message);
      result.ReplyTo = this.ReplyTo;
      result.GroupFor = this.GroupFor;
      result.OrganizationID = this.OrganizationID;
      result.UserID = this.UserID;
      result.MessageID = this.MessageID;
       
      result.TimeStamp = DateTime.SpecifyKind(this.TimeStampUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
