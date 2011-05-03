using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ProductVersionsViewItem
  {
  }
  
  public partial class ProductVersionsView
  {
    public void LoadByProductID(int productID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ProductVersionsView WHERE ProductID = @ProductID ORDER BY VersionNumber DESC";
        command.CommandText = InjectCustomFields(command.CommandText, "ProductVersionID", ReferenceType.ProductVersions);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        Fill(command);
      }
    }

    public void LoadByProductAndCustomer(int productID, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT pv.* FROM ProductVersionsView pv WHERE pv.ProductID = @ProductID AND pv.ProductVersionID IN 
                                (SELECT DISTINCT op.ProductVersionID FROM OrganizationProducts op 
                                 WHERE op.OrganizationID = @OrganizationID)
                                 ORDER BY pv.VersionNumber DESC";

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }


  }
  
}
