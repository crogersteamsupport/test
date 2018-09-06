using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class BackdoorLogin : BaseItem
  {
    public BackdoorLoginProxy GetProxy()
    {
      BackdoorLoginProxy result = new BackdoorLoginProxy();
      result.ContactID = this.ContactID;
      result.Token = this.Token;
      result.UserID = this.UserID;
      result.BackdoorLoginID = this.BackdoorLoginID;
       
      result.DateIssued = DateTime.SpecifyKind(this.DateIssuedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
