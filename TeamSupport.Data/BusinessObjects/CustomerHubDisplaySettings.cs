using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CustomerHubDisplaySetting
  {
  }
  
  public partial class CustomerHubDisplaySettings
  {
		public void LoadByCustomerHubID(int customerHubID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SELECT * FROM [CustomerHubDisplaySettings] WHERE CustomerHubID = @CustomerHubID";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@CustomerHubID", customerHubID);
				Fill(command);
			}
		}
	}
  
}
