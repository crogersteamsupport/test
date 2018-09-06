using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class AssetsViewItem : BaseItem
  {
    public AssetsViewItemProxy GetProxy()
    {
      AssetsViewItemProxy result = new AssetsViewItemProxy();
      result.ImportID = this.ImportID;
      result.Status = this.Status;
      result.SubPartOf = this.SubPartOf;
      result.ModifierName = this.ModifierName;
      result.ModifierID = this.ModifierID;
      result.CreatorName = this.CreatorName;
      result.CreatorID = this.CreatorID;
      result.Notes = this.Notes;
      result.Location = this.Location;
      result.Name = this.Name;
      result.SerialNumber = this.SerialNumber;
      result.OrganizationID = this.OrganizationID;
      result.ProductVersionNumber = this.ProductVersionNumber;
      result.ProductVersionID = this.ProductVersionID;
      result.ProductName = this.ProductName;
      result.ProductID = this.ProductID;
      result.AssetID = this.AssetID;
       
       
      result.DateModified = this.DateModifiedUtc == null ? this.DateModifiedUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedUtc, DateTimeKind.Utc); 
      result.DateCreated = this.DateCreatedUtc == null ? this.DateCreatedUtc : DateTime.SpecifyKind((DateTime)this.DateCreatedUtc, DateTimeKind.Utc); 
      result.WarrantyExpiration = this.WarrantyExpirationUtc == null ? this.WarrantyExpirationUtc : DateTime.SpecifyKind((DateTime)this.WarrantyExpirationUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
