using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class JiraInstanceProduct : BaseItem
  {
    public JiraInstanceProductProxy GetProxy()
    {
      JiraInstanceProductProxy result = new JiraInstanceProductProxy();
      result.ProductId = this.ProductId;
      result.CrmLinkId = this.CrmLinkId;
      result.JiraInstanceProductsId = this.JiraInstanceProductsId;
       
       
       
      return result;
    }	
  }
}
