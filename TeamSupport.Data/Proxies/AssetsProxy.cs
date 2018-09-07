using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class Asset : BaseItem
  {
    public AssetProxy GetProxy()
    {
      AssetProxy result = new AssetProxy();
      result.ImportFileID = this.ImportFileID;
      result.NeedsIndexing = this.NeedsIndexing;
      result.ProductVersionID = this.ProductVersionID;
      result.ImportID = this.ImportID;
      result.Status = this.Status;
      result.SubPartOf = this.SubPartOf;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.AssignedTo = this.AssignedTo;
      result.ProductID = this.ProductID;
      result.Notes = this.Notes;
      result.Location = this.Location;
      result.Name = this.Name;
      result.SerialNumber = this.SerialNumber;
      result.OrganizationID = this.OrganizationID;
      result.AssetID = this.AssetID;
       
       
      result.DateModified = this.DateModifiedUtc == null ? this.DateModifiedUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedUtc, DateTimeKind.Utc); 
      result.DateCreated = this.DateCreatedUtc == null ? this.DateCreatedUtc : DateTime.SpecifyKind((DateTime)this.DateCreatedUtc, DateTimeKind.Utc); 
      result.WarrantyExpiration = this.WarrantyExpirationUtc == null ? this.WarrantyExpirationUtc : DateTime.SpecifyKind((DateTime)this.WarrantyExpirationUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
