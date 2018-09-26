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
                        command.CommandText = " SELECT tickets.TicketID, tickets.Name, tickets.OrganizationID, tickets.ProductID, tickets.IsVisibleOnPortal, tags.Value FROM dbo.tickets AS Tickets WITH (NOLOCK)";
                        command.CommandText += "INNER JOIN dbo.TagLinks AS TagLinks WITH (NOLOCK) ON TagLinks.RefType = 17 AND TagLinks.RefID = Tickets.TicketID ";
                        command.CommandText += "INNER JOIN dbo.Tags AS Tags WITH (NOLOCK) ON Tags.TagID = TagLinks.TagID ";
                        command.CommandText += "WHERE tickets.organizationID = @organizationID and tickets.IsKnowledgeBase = 1 ";
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

    }
}
