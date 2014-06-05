using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class AssetsViewItem
  {
  }
  
  public partial class AssetsView
  {
  }
  
  public class InventorySearchAsset
  {
    public InventorySearchAsset() { }
    public InventorySearchAsset(AssetsViewItem item)
    {
      assetID               = item.AssetID;
      organizationID        = item.OrganizationID;
      productName           = item.ProductName;
      productVersionNumber  = item.ProductVersionNumber;
      serialNumber          = item.SerialNumber;
      name                  = item.Name;
      notes                 = item.Notes;
      warrantyExpiration    = item.WarrantyExpiration;
      dateCreated           = item.DateCreated;
      dateModified          = item.DateModified;
      creatorName           = item.CreatorName;
      modifierName          = item.ModifierName;

    }

    public int assetID { get; set; }
    public int organizationID { get; set; }
    public string productName { get; set; }
    public string productVersionNumber { get; set; }
    public string serialNumber { get; set; }
    public string name { get; set; }
    public string notes { get; set; }
    public DateTime? warrantyExpiration { get; set; }
    public DateTime? dateCreated { get; set; }
    public DateTime? dateModified { get; set; }
    public string creatorName { get; set; }
    public string modifierName { get; set; }
  }
}
