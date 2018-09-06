using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ForumCategory : BaseItem
  {
    public ForumCategoryProxy GetProxy()
    {
      ForumCategoryProxy result = new ForumCategoryProxy();
      result.ProductFamilyID = this.ProductFamilyID;
      result.ProductID = this.ProductID;
      result.GroupID = this.GroupID;
      result.TicketType = this.TicketType;
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
