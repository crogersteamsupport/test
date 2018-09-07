using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class OrganizationProductsViewItem : BaseItem
  {
    public OrganizationProductsViewItemProxy GetProxy()
    {
      OrganizationProductsViewItemProxy result = new OrganizationProductsViewItemProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.ProductVersionID = this.ProductVersionID;
      result.ProductID = this.ProductID;
      result.OrganizationName = (this.OrganizationName);
      result.OrganizationID = this.OrganizationID;
      result.OrganizationProductID = this.OrganizationProductID;
      result.Description = (this.Description);
      result.IsReleased = this.IsReleased;
      result.ProductVersionStatusID = this.ProductVersionStatusID;
      result.VersionNumber = this.VersionNumber;
      result.IsDiscontinued = this.IsDiscontinued;
      result.IsShipping = this.IsShipping;
      result.VersionStatus = this.VersionStatus;
      result.Product = this.Product;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      result.SupportExpiration = this.SupportExpirationUtc == null ? this.SupportExpirationUtc : DateTime.SpecifyKind((DateTime)this.SupportExpirationUtc, DateTimeKind.Utc); 
      result.ReleaseDate = this.ReleaseDateUtc == null ? this.ReleaseDateUtc : DateTime.SpecifyKind((DateTime)this.ReleaseDateUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
