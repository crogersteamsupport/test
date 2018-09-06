using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class FacebookOption : BaseItem
  {
    public FacebookOptionProxy GetProxy()
    {
      FacebookOptionProxy result = new FacebookOptionProxy();
      result.DisplayKB = this.DisplayKB;
      result.DisplayArticles = this.DisplayArticles;
      result.OrganizationID = this.OrganizationID;
       
       
       
      return result;
    }	
  }
}
