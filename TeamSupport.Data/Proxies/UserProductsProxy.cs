using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class UserProduct : BaseItem
  {
    public UserProductProxy GetProxy()
    {
      UserProductProxy result = new UserProductProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.ImportID = this.ImportID;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.ProductVersionID = this.ProductVersionID;
      result.ProductID = this.ProductID;
      result.UserID = this.UserID;
      result.UserProductID = this.UserProductID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      result.SupportExpiration = this.SupportExpirationUtc == null ? this.SupportExpirationUtc : DateTime.SpecifyKind((DateTime)this.SupportExpirationUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
