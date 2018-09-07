using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class FullContactUpdatesItem : BaseItem
  {
    public FullContactUpdatesItemProxy GetProxy()
    {
      FullContactUpdatesItemProxy result = new FullContactUpdatesItemProxy();
      result.OrganizationId = this.OrganizationId;
      result.UserId = this.UserId;
      result.Id = this.Id;
       
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
