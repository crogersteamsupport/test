using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class AssetHistoryItem
  {
  }
  
  public partial class AssetHistory
  {
    // This probably needs to be moved to AssetHistoryView
    public void LoadByAssetID(int assetID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT 
          a.actiondescription as 'Action Description', 
          a.actiontime as 'Time', 
          u.lastname+', '+u.firstname as 'User', 
          isnull(a.comments,'') as 'Comments', 
          isnull(o1.name,'') as 'Shipped From', 
          isnull(o2.name,'') as 'Shipped To', 
          a.shippingmethod as 'Shipping Method', 
          a.trackingnumber as 'Tracking Number', 
          isnull(a.ReferenceNum,'') as 'Reference Number'
        FROM
          AssetHistory as a 
          LEFT JOIN Organizations as o1 
            ON a.shippedfrom = o1.organizationid 
          LEFT JOIN Organizations as o2 
            ON a.shippedto = o2.organizationid 
          LEFT JOIN users as u 
            ON a.actor = u.userid
        WHERE
          a.AssetID = @AssetID 
          AND a.OrganizationID = @OrganizationID
        ORDER BY
          a.DateCreated desc";
        command.CommandText = InjectCustomFields(command.CommandText, "UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        //command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

  }
  
}
