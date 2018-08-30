using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class WatercoolerLike : BaseItem
  {
    public WatercoolerLikeProxy GetProxy()
    {
      WatercoolerLikeProxy result = new WatercoolerLikeProxy();
      result.UserID = this.UserID;
      result.MessageID = this.MessageID;

      result.UserName = Users.GetUserFullName(BaseCollection.LoginUser, this.UserID);        
         
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
