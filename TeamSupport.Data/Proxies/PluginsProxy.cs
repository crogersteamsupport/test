using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class Plugin : BaseItem
  {
    public PluginProxy GetProxy()
    {
      PluginProxy result = new PluginProxy();
      result.CreatorID = this.CreatorID;
      result.Code = this.Code;
      result.Name = this.Name;
      result.OrganizationID = this.OrganizationID;
      result.PluginID = this.PluginID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
