using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class RecentlyViewedItem
  {
  }
  
  public partial class RecentlyViewedItems
  {
      public void LoadRecent(int userID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText =
                @"SELECT TOP 5 * FROM RecentlyViewedItems
                WHERE (UserID = @UserID) 
                ORDER BY DateViewed Desc";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@UserID", userID);
              Fill(command);
          }
      }

      public void DeleteRecentOrg(int organizationID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText =
                @"DELETE FROM RecentlyViewedItems
                WHERE (refID = @orgID) AND (refType = 1)";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@orgID", organizationID);
              command.ExecuteNonQuery();
          }
      }

  }
  
}
