using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class CompanyParentsViewItem : BaseItem
  {
    public CompanyParentsViewItemProxy GetProxy()
    {
      CompanyParentsViewItemProxy result = new CompanyParentsViewItemProxy();
      result.ParentName = this.ParentName;
      result.ParentID = this.ParentID;
      result.ChildID = this.ChildID;
       
       
       
      return result;
    }	
  }
}
