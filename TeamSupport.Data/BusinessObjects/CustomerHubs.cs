using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CustomerHub
  {
  }
  
  public partial class CustomerHubs
  {
		public void LoadByOrganizationID(int organizationID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SELECT * FROM [CustomerHubs] WHERE OrganizationID = @OrganizationID";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@OrganizationID", organizationID);
				Fill(command);
			}
		}

		public void LoadByHubName(string name, string cnameURL, bool isActive = true)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"SELECT * FROM [CustomerHubs] WHERE (Lower(PortalName) = @Name OR
					Lower(CNameURL) = @cnameURL) AND IsActive = @IsActive";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@Name", name);
				command.Parameters.AddWithValue("@CNameURL", cnameURL);
				command.Parameters.AddWithValue("@IsActive", isActive);
				Fill(command);
			}
		}

        public static void DeleteByCustomerHubID(LoginUser loginUser, int customerHubID)
        {
            CustomerHubs customerHubs = new CustomerHubs(loginUser);

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "Delete FROM dbo.CustomerHubs WHERE CustomerHubID = @CustomerHubID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@CustomerHubID", customerHubID);
                customerHubs.ExecuteNonQuery(command, "CustomerHubs");
            }
        }
    }
  
}
