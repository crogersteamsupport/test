using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class Group : BaseItem
  {
    public GroupProxy GetProxy()
    {
      GroupProxy result = new GroupProxy();
      result.ProductFamilyID = this.ProductFamilyID;
      result.Description = this.Description;
      result.Name = this.Name;
      result.GroupID = this.GroupID;
      
      Groups groups = new Groups(BaseCollection.LoginUser);
	  result.TicketCount = this.TicketCount;
       
      return result;
    }	
  }
}
