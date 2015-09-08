using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Ganss.XSS;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(AssetProxy))]
  public class AssetProxy
  {
    public AssetProxy() {}
    [DataMember] public int AssetID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string SerialNumber { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Location { get; set; }
    [DataMember] public string Notes { get; set; }
    [DataMember] public int? ProductID { get; set; }
    [DataMember] public DateTime? WarrantyExpiration { get; set; }
    [DataMember] public int? AssignedTo { get; set; }
    [DataMember] public DateTime? DateCreated { get; set; }
    [DataMember] public DateTime? DateModified { get; set; }
    [DataMember] public int? CreatorID { get; set; }
    [DataMember] public int? ModifierID { get; set; }
    [DataMember] public int? SubPartOf { get; set; }
    [DataMember] public string Status { get; set; }
    [DataMember] public string ImportID { get; set; }
    [DataMember] public int? ProductVersionID { get; set; }
    [DataMember] public bool NeedsIndexing { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
          
  }
  
  public partial class Asset : BaseItem
  {
    public AssetProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      AssetProxy result = new AssetProxy();
      result.ImportFileID = this.ImportFileID;
      result.NeedsIndexing = this.NeedsIndexing;
      result.ProductVersionID = this.ProductVersionID;
      result.ImportID = sanitizer.Sanitize(this.ImportID);
      result.Status = sanitizer.Sanitize(this.Status);
      result.SubPartOf = this.SubPartOf;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.AssignedTo = this.AssignedTo;
      result.ProductID = this.ProductID;
      result.Notes = sanitizer.Sanitize(this.Notes);
      result.Location = sanitizer.Sanitize(this.Location);
      result.Name = sanitizer.Sanitize(this.Name);
      result.SerialNumber = sanitizer.Sanitize(this.SerialNumber);
      result.OrganizationID = this.OrganizationID;
      result.AssetID = this.AssetID;
       
       
      result.DateModified = this.DateModifiedUtc == null ? this.DateModifiedUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedUtc, DateTimeKind.Utc); 
      result.DateCreated = this.DateCreatedUtc == null ? this.DateCreatedUtc : DateTime.SpecifyKind((DateTime)this.DateCreatedUtc, DateTimeKind.Utc); 
      result.WarrantyExpiration = this.WarrantyExpirationUtc == null ? this.WarrantyExpirationUtc : DateTime.SpecifyKind((DateTime)this.WarrantyExpirationUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
