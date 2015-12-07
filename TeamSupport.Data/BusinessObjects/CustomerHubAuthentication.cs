using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CustomerHubAuthenticationItem
  {
  }
  
  public partial class CustomerHubAuthentication
  {
		public void LoadByCustomerHubID(int customerHubID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SELECT * FROM [CustomerHubAuthentication] WHERE CustomerHubID = @CustomerHubID";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@CustomerHubID", customerHubID);
				Fill(command);
			}
		}
	}
  
}
