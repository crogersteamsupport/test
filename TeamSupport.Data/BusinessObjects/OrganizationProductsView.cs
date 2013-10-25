using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class OrganizationProductsViewItem
  {
  }
  
  public partial class OrganizationProductsView
  {
    public void LoadByOrganizationID(int organizationID, string orderBy = "OrganizationProductID")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM OrganizationProductsView WHERE (OrganizationID = @OrganizationID) ORDER BY " + orderBy;
        command.CommandText = InjectCustomFields(command.CommandText, "OrganizationProductID", ReferenceType.OrganizationProducts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByOrganizationAndProductIDs(int organizationID, int productID, string orderBy = "OrganizationProductID")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = 
          @"
            SELECT 
              * 
            FROM
              OrganizationProductsView 
            WHERE
              OrganizationID = @OrganizationID 
              AND ProductID = @ProductID
            ORDER BY
          " + orderBy;
        command.CommandText = InjectCustomFields(command.CommandText, "OrganizationProductID", ReferenceType.OrganizationProducts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@ProductID", productID);
        Fill(command);
      }
    }

    public void LoadByOrganizationProductAndVersionIDs(int organizationID, int productID, int productVersionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
          @"
            SELECT 
              * 
            FROM
              OrganizationProductsView 
            WHERE
              OrganizationID = @OrganizationID 
              AND ProductID = @ProductID 
              AND ProductVersionID = @ProductVersionID
            ORDER BY 
              OrganizationProductID
          ";
        command.CommandText = InjectCustomFields(command.CommandText, "OrganizationProductID", ReferenceType.OrganizationProducts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
        Fill(command);
      }
    }

    public void LoadByOrganizationProductID(int organizationProductID, bool includeCustomFields)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
          @"
            SELECT 
              * 
            FROM
              OrganizationProductsView 
            WHERE
              OrganizationProductID = @OrganizationProductID 
            ORDER BY 
              OrganizationProductID
          ";
        if (includeCustomFields)
        {
          command.CommandText = InjectCustomFields(command.CommandText, "OrganizationProductID", ReferenceType.OrganizationProducts);
        }
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationProductID", organizationProductID);
        Fill(command);
      }
    }
  }
  
}
