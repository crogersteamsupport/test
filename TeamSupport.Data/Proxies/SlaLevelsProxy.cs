using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class SlaLevel : BaseItem
  {
    public SlaLevelProxy GetProxy()
    {
      SlaLevelProxy result = new SlaLevelProxy();
      result.Name = this.Name;
      result.OrganizationID = this.OrganizationID;
      result.SlaLevelID = this.SlaLevelID;
       
       
       
      return result;
    }	
  }
}
