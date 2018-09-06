using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class AgentRatingUser : BaseItem
  {
    public AgentRatingUserProxy GetProxy()
    {
      AgentRatingUserProxy result = new AgentRatingUserProxy();
      result.UserID = this.UserID;
      result.AgentRatingID = this.AgentRatingID;
       
       
       
      return result;
    }	
  }
}
