using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Data.Linq;
using TeamSupport.Data.BusinessObjects;

namespace TeamSupport.Data
{

    public partial class Deflector
    {
        public static List<string> GetPodIndeces(LoginUser loginUser) {
            List<string> jsonResultList = new List<string>();
            List<int> OrganizationList = GetActiveOrganizationList(loginUser);

            foreach (int organizationID in OrganizationList) {
                jsonResultList.Add(GetOrganizationIndeces(loginUser, organizationID));
            };

            return jsonResultList;
        }

        private static List<int> GetActiveOrganizationList(LoginUser loginUser)
        {
            List<int> result = new List<int>();
            try
            {
                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT OrganizationID FROM dbo.Organizations WITH (NOLOCK)";
                        command.CommandText += "WHERE IsActive = 1 and ParentID = 1";
                        connection.Open();

                        using (SqlDataReader dr = command.ExecuteReader())
                        {
                            var orgTable = new DataTable();
                            orgTable.Load(dr);

                            foreach (DataRow row in orgTable.Rows)
                            {
                                result.Add((int)row[0]);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = null;
            }

            return result;
        }

        public static string GetOrganizationIndeces(LoginUser loginUser, int organizationID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText =  "SELECT tickets.TicketID, tickets.Name, tickets.OrganizationID, tickets.ProductID, tickets.IsVisibleOnPortal, tags.Value FROM dbo.tickets AS Tickets WITH (NOLOCK)";
                        command.CommandText += "INNER JOIN dbo.TagLinks AS TagLinks WITH (NOLOCK) ON TagLinks.RefType = 17 AND TagLinks.RefID = Tickets.TicketID ";
                        command.CommandText += "INNER JOIN dbo.Tags AS Tags WITH (NOLOCK) ON Tags.TagID = TagLinks.TagID ";
                        command.CommandText += "WHERE tickets.organizationID = @organizationID and tickets.IsKnowledgeBase = 1 and tickets.IsVisibleOnPortal = 1";
                        command.CommandText += "FOR JSON PATH";

                        command.Parameters.AddWithValue("@organizationID", organizationID);
                        connection.Open();

                        var jsonResult = new StringBuilder();
                        SqlDataReader reader = command.ExecuteReader();

                        if (!reader.HasRows)
                        {
                            jsonResult.Append("[]");
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                jsonResult.Append(reader.GetValue(0).ToString());
                            }
                        }

                        reader.Close();
                        return jsonResult.ToString();
                    }
                }
            }
            catch (SqlException e)
            {
                return "error";
                // return e.Message;
            }
            catch (Exception e)
            {
                return "error";
            }

        }

        public static List<DataRow> GetWhiteListHubTicketPaths(int customerHubID)
        {
            List<DataRow> result = new List<DataRow>();

            using (SqlConnection connection = new SqlConnection(LoginUser.Anonymous.ConnectionString))
            {
                DataTable dt = new DataTable();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = @"SELECT TicketID, CNameURL, PortalName
                                        FROM [Tickets] AS Tickets
                                        INNER JOIN [CustomerHubs] AS Hubs
	                                        ON Tickets.OrganizationID = Hubs.OrganizationID
                                        INNER JOIN [CustomerHubFeatureSettings] AS HubFeatures
	                                        ON Hubs.CustomerHubID = HubFeatures.CustomerHubID
		                                        AND HubFeatures.EnableKnowledgeBase = 1 
		                                        AND HubFeatures.EnableAnonymousProductAssociation = 0
		                                        AND HubFeatures.EnableCustomerSpecificKB = 0
                                        LEFT JOIN Products AS Products
	                                        ON Products.ProductFamilyID = Hubs.ProductFamilyID
                                        WHERE 
	                                        IsKnowledgeBase = 1 
	                                        AND IsVisibleOnPortal = 1
	                                        AND Hubs.CustomerHubID = @CustomerHubID
	                                        AND (Hubs.ProductFamilyID IS NULL OR Products.ProductFamilyID IS NOT NULL)";
                command.Parameters.AddWithValue("@CustomerHubID", customerHubID);
                connection.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    try
                    {
                        adapter.Fill(dt);
                        result.AddRange(dt.AsEnumerable().Select(x => x).ToList());
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
                        return null;
                    }
                }
            }

            return result;
        }

    }
}
