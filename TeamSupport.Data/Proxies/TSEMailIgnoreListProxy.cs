using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{

  
  public partial class TSEMailIgnoreListItem : BaseItem
  {
    public TSEMailIgnoreListItemProxy GetProxy()
    {
      TSEMailIgnoreListItemProxy result = new TSEMailIgnoreListItemProxy();
      result.ToAddress = this.ToAddress;
      result.FromAddress = this.FromAddress;
      result.IgnoreID = this.IgnoreID;
       
       
       
      return result;
    }	
  }
}
