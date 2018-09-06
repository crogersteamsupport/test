using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ActionType : BaseItem
  {
    public ActionTypeProxy GetProxy()
    {
      ActionTypeProxy result = new ActionTypeProxy();
      result.Position = this.Position;
      result.IsTimed = this.IsTimed;
      result.Description = this.Description;
      result.Name = this.Name;
      result.ActionTypeID = this.ActionTypeID;
       
      return result;
    }	
  }
}
