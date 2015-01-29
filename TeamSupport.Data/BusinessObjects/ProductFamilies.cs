using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ProductFamily
  {
  }
  
  public partial class ProductFamilies
  {
      public void LoadBySearchTerm(string searchTerm, int start, int organizationID)
      {
          int end = start + 20;
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = @"
                SELECT
                  *
                FROM
                (
                  SELECT
                    *, 
                    ROW_NUMBER() OVER (ORDER BY Name) AS rownum  
                  FROM
                  (
                    SELECT
                      *
                    FROM
                      ProductFamilies
                    WHERE 
                      OrganizationID = @OrganizationID
                      AND (@SearchTerm = '' OR (Name LIKE '%' + @SearchTerm + '%') OR (Description LIKE '%' + @SearchTerm + '%'))
                  ) as temp
                ) as results
                WHERE
                  rownum between @start and @end
				        ORDER BY
                  rownum ASC
                ";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@OrganizationID", organizationID);
              command.Parameters.AddWithValue("@SearchTerm", searchTerm);
              command.Parameters.AddWithValue("@start", start + 1);
              command.Parameters.AddWithValue("@end", end);
              Fill(command);
          }
      }

      public static void DeleteProductFamily(LoginUser loginUser, int productFamilyID)
      {
          ProductFamilies productFamilies = new ProductFamilies(loginUser);
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = "UPDATE Products SET ProductFamilyID = null WHERE ProductFamilyID = @ProductFamilyID AND OrganizationID = @OrganizationID";
              command.CommandType = CommandType.Text;
              command.Parameters.Clear();
              command.Parameters.AddWithValue("@ProductFamilyID", productFamilyID);
              command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
              productFamilies.ExecuteNonQuery(command, "Products");

              command.CommandText = "DELETE RecentlyViewedItems WHERE RefType = 44 AND RefID = @ProductFamilyID";
              command.CommandType = CommandType.Text;
              command.Parameters.Clear();
              command.Parameters.AddWithValue("@ProductFamilyID", productFamilyID);
              productFamilies.ExecuteNonQuery(command, "Products");

              command.CommandText = "DELETE ProductFamilies WHERE ProductFamilyID = @ProductFamilyID AND OrganizationID = @OrganizationID";
              command.CommandType = CommandType.Text;
              command.Parameters.Clear();
              command.Parameters.AddWithValue("@ProductFamilyID", productFamilyID);
              command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
              productFamilies.ExecuteNonQuery(command, "ProductFamilies");

          }


          productFamilies.LoadByProductFamilyID(productFamilyID);
          if (!productFamilies.IsEmpty) productFamilies[0].Delete();
          productFamilies.Save();

      }

      public void LoadByOrganizationID(int organizationID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = "SELECT * FROM ProductFamilies WHERE OrganizationID = @OrganizationID ORDER BY Name";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@OrganizationID", organizationID);
              Fill(command);
          }
      }

  
  }
  
}
