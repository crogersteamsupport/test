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
    public string DisplayName
    {
      get
      {
        if (String.IsNullOrEmpty(this.Name))
        {
          if (String.IsNullOrEmpty(this.SerialNumber))
          {
            return this.AssetID.ToString();
          }
          else
          {
            return this.SerialNumber;
          }
        }
        else
        {
          return this.Name;
        }
      }
    }
  }
  
  public partial class AssetsView
  {
    public void LoadByRefID(int refID, ReferenceType refType)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
          SELECT
            a.* 
          FROM
            AssetsView a
            JOIN AssetHistory h
              ON a.AssetID = h.AssetID
            JOIN AssetAssignments aa
              ON h.HistoryID = aa.HistoryID
          WHERE 
            h.ShippedTo = @RefID
            AND h.RefType = @RefType
          ORDER BY 
            aa.AssetAssignmentsID DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefID", refID);
        command.Parameters.AddWithValue("@RefType", refType);
        Fill(command);
      }
    }

    public void LoadByLikeAssetDisplayName(int organizationID, string name, int maxRows)
    {
      using (SqlCommand command = new SqlCommand())
      {
        StringBuilder text = new StringBuilder(@"
        SELECT 
          TOP (@MaxRows) * 
        FROM 
          Assets 
        WHERE 
          OrganizationID = @OrganizationID
          AND Location = 2
          AND (Name LIKE '%'+@Name+'%' OR SerialNumber LIKE '%'+@Name+'%')
        ");
        command.CommandText = text.ToString();
        command.CommandType = CommandType.Text;

        command.Parameters.AddWithValue("@Name", name.Trim());
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@MaxRows", maxRows);
        Fill(command);
      }
    }
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
      location              = item.Location;  
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
    public string location { get; set; }
    public string notes { get; set; }
    public DateTime? warrantyExpiration { get; set; }
    public DateTime? dateCreated { get; set; }
    public DateTime? dateModified { get; set; }
    public string creatorName { get; set; }
    public string modifierName { get; set; }
  }
}
