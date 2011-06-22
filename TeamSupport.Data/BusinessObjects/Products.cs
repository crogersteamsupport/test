using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class Product 
  {
  }

  public partial class Products
  {

    partial void BeforeRowDelete(int productID)
    {
      Product product = (Product)Products.GetProduct(LoginUser, productID);
      string description = "Deleted product '" + product.Name + "'";
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Products, productID, description);
    }

    partial void BeforeRowEdit(Product product)
    {
      string description;

      Product oldProduct = (Product) Products.GetProduct(LoginUser, product.ProductID);

      if (oldProduct.Description != product.Description)
      {
        description = "Changed description for product '" + product.Description + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Products, product.ProductID, description);
      }

      if (oldProduct.Name != product.Name)
      {
        description = "Changed product name '" + oldProduct.Description + "' to '" + product.Name + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Products, product.ProductID, description);
      }
    }

    partial void AfterRowInsert(Product product)
    {
      string description = "Created product '" + product.Name + "'";
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Products, product.ProductID, description);
    }

    public void LoadByProductForGrid(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "uspSelectProductsForGrid";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Products WHERE OrganizationID = @OrganizationID ORDER BY Name";
        command.CommandText = InjectCustomFields(command.CommandText, "ProductID", ReferenceType.Products);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT p.* FROM Products p WHERE p.ProductID IN 
                                (SELECT DISTINCT op.ProductID FROM OrganizationProducts op 
                                 LEFT JOIN OrganizationTickets ot ON ot.OrganizationID = op.OrganizationID
                                 LEFT JOIN Tickets t ON t.TicketID = ot.TicketID
                                 WHERE t.TicketID = @TicketID)
                               UNION 
                               SELECT p.* FROM Products p LEFT JOIN Tickets t ON t.ProductID = p.ProductID WHERE  t.TicketID = @TicketID  
                               ORDER BY p.Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadByCustomerID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT p.* FROM Products p WHERE p.ProductID IN 
                                (SELECT DISTINCT op.ProductID FROM OrganizationProducts op 
                                 WHERE op.OrganizationID = @OrganizationID)
                              ORDER BY p.Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }
    public void RemoveCustomer(int organizationProductID)
    {
      OrganizationProducts organizationProducts = new OrganizationProducts(LoginUser);
      organizationProducts.LoadItemInfo(organizationProductID);
      if (organizationProducts.IsEmpty) return;
      OrganizationProduct organizationProduct = organizationProducts[0];
      
      /*using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM OrganizationProducts WHERE (OrganizationProductID = @OrganizationProductID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationProductID", organizationProductID);
        ExecuteNonQuery(command, "OrganizationProducts");
      }*/

      string description;

      if (organizationProduct.ProductVersionID != null)
      {
        description = "Removed product '" + (string)organizationProduct.Row["ProductName"] + "' version '" + (string)organizationProduct.Row["VersionNumber"] + "' from customer '" +
                              (string)organizationProduct.Row["OrganizationName"] + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.ProductVersions, (int)organizationProduct.ProductVersionID, description);
      }
      else
	    {
        description = "Removed product '" + (string)organizationProduct.Row["ProductName"] + "' from customer '" +
                              (string)organizationProduct.Row["OrganizationName"] + "'";

	    }
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Organizations, organizationProduct.OrganizationID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Products, organizationProduct.ProductID, description);

      organizationProducts.DeleteFromDB(organizationProductID);
      
    }

    public void AddCustomer(int organizationID, int productID, int versionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"INSERT INTO 
          OrganizationProducts 
          (OrganizationID, ProductID, ProductVersionID, IsVisibleOnPortal, DateCreated, DateModified, CreatorID, ModifierID) 
          VALUES 
          (@OrganizationID, @ProductID, @ProductVersionID, 1, @DateCreated, @DateModified, @CreatorID, @ModifierID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@ProductVersionID", versionID);
        command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
        command.Parameters.AddWithValue("@DateModified", DateTime.UtcNow);
        command.Parameters.AddWithValue("@CreatorID", LoginUser.UserID);
        command.Parameters.AddWithValue("@ModifierID", LoginUser.UserID);
        ExecuteNonQuery(command, "OrganizationProducts");
      }

      string organizationName = ((Organization)Organizations.GetOrganization(LoginUser, organizationID)).Name;
      string productName = ((Product)Products.GetProduct(LoginUser, productID)).Name;
      string versionNumber = ((ProductVersion)ProductVersions.GetProductVersion(LoginUser, (int)versionID)).VersionNumber;

      string description = "Added product '" + productName + "' version '" + versionNumber + "' to customer '" + organizationName + "'";
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.ProductVersions, (int)versionID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Products, productID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Organizations, organizationID, description);


    }

    public void AddCustomer(int organizationID, int productID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"INSERT INTO 
          OrganizationProducts 
          (OrganizationID, ProductID, IsVisibleOnPortal, DateCreated, DateModified, CreatorID, ModifierID) 
          VALUES 
          (@OrganizationID, @ProductID, 1, @DateCreated, @DateModified, @CreatorID, @ModifierID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
        command.Parameters.AddWithValue("@DateModified", DateTime.UtcNow);
        command.Parameters.AddWithValue("@CreatorID", LoginUser.UserID);
        command.Parameters.AddWithValue("@ModifierID", LoginUser.UserID);
        ExecuteNonQuery(command, "OrganizationProducts");
      }

      string organizationName = ((Organization)Organizations.GetOrganization(LoginUser, organizationID)).Name;
      string productName = ((Product)Products.GetProduct(LoginUser, productID)).Name;

      string description = "Added product '" + productName + "' to customer '" + organizationName + "'";

      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Products, productID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Organizations, organizationID, description);
    }

    public Product FindByImportID(string importID)
    {
      importID = importID.Trim().ToLower();
      foreach (Product product in this)
      {
        if ((product.ImportID != null && product.ImportID.ToLower().Trim() == importID) || product.Name.ToLower().Trim() == importID)
        {
          return product;
        }
      }
      return null;
    }

    public Product FindByName(string name) {
        foreach (Product product in this) {
            if (product.Name != null && product.Name.ToLower().Trim() == name.ToLower().Trim()) {
                return product;
            }
        }
        return null;
    }

    public static void DeleteProduct(LoginUser loginUser, int productID)
    {
      Products products = new Products(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM OrganizationProducts WHERE (ProductID = @ProductID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@ProductID", productID);
        products.ExecuteNonQuery(command, "OrganizationProducts");

        command.CommandText = "UPDATE Tickets SET ProductID = null, ReportedVersionID = null, SolvedVersionID = null WHERE (ProductID = @ProductID)";
        command.CommandType = CommandType.Text;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@ProductID", productID);
        products.ExecuteNonQuery(command, "Tickets");

      }


      products.LoadByProductID(productID);
      if (!products.IsEmpty) products[0].Delete();
      products.Save();

    }



  }
}
