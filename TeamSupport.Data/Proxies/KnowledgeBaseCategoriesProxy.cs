using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class KnowledgeBaseCategory : BaseItem
  {
    public KnowledgeBaseCategoryProxy GetProxy()
    {
      KnowledgeBaseCategoryProxy result = new KnowledgeBaseCategoryProxy();
      result.ProductFamilyID = this.ProductFamilyID;
      result.VisibleOnPortal = this.VisibleOnPortal;
      result.Position = this.Position;
      result.OrganizationID = this.OrganizationID;
      result.CategoryDesc = this.CategoryDesc;
      result.CategoryName = this.CategoryName;
      result.ParentID = this.ParentID;
      result.CategoryID = this.CategoryID;
       
       
       
      return result;
    }	
  }
}
