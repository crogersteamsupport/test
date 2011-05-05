using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

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
          
  }
  
  public partial class Asset : BaseItem
  {
    public AssetProxy GetProxy()
    {
      AssetProxy result = new AssetProxy();
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
       
       
      result.DateModified = this.DateModified == null ? this.DateModified : DateTime.SpecifyKind((DateTime)this.DateModified, DateTimeKind.Local); 
      result.DateCreated = this.DateCreated == null ? this.DateCreated : DateTime.SpecifyKind((DateTime)this.DateCreated, DateTimeKind.Local); 
      result.WarrantyExpiration = this.WarrantyExpiration == null ? this.WarrantyExpiration : DateTime.SpecifyKind((DateTime)this.WarrantyExpiration, DateTimeKind.Local); 
       
      return result;
    }	
  }
}
