using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
	public partial class CustomerHubView
	{
	}

	public partial class CustomerHubViews
	{
		public void loadAll()
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText =
					@"SELECT [CustomerHubViewID]
					  ,[Name]
					  ,[Route]
					  ,[IsActive]
					  ,[DateCreated]
					FROM[TeamSupport].[dbo].[CustomerHubViews]";
				command.CommandType = CommandType.Text;
				Fill(command);
			}
		}
	}

}
