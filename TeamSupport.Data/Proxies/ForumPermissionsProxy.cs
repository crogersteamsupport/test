using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ForumPermission : BaseItem
  {
    public ForumPermissionProxy GetProxy()
    {
      ForumPermissionProxy result = new ForumPermissionProxy();
      result.FilterOrgID = this.FilterOrgID;
      result.FilterUserID = this.FilterUserID;
      result.OrganizationID = this.OrganizationID;
      result.CategoryID = this.CategoryID;
       
       
       
      return result;
    }	
  }
}
