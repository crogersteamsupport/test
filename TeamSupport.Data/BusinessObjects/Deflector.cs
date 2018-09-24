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

namespace TeamSupport.Data {

    public partial class Deflector {

        public static string PullOrganization (LoginUser loginUser, int organizationID) {
            try {
                //using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString)) {
                //    using (SqlCommand command = new SqlCommand()) {
                //        command.Connection   = connection;
                //        command.CommandType  = CommandType.Text;
                //        command.CommandText  = "SELECT tickets.TicketID, tickets.Name, tickets.OrganizationID, tickets.ProductID, tickets.IsVisibleOnPortal, tags.Value FROM dbo.tickets AS Tickets ";
                //        command.CommandText += "INNER JOIN dbo.TagLinks AS TagLinks ON TagLinks.RefType = 17 AND TagLinks.RefID = Tickets.TicketID ";
                //        command.CommandText += "INNER JOIN dbo.Tags AS Tags ON Tags.TagID = TagLinks.TagID ";
                //        command.CommandText += "WHERE tickets.organizationID = @organizationID and tickets.IsKnowledgeBase = 1 ";
                //        command.CommandText += "FOR JSON PATH, ROOT('results')";
                //        command.Parameters.AddWithValue("@organizationID", organizationID);
                //        connection.Open();
                //        SqlDataReader reader = command.ExecuteReader();
                //        if (reader.HasRows && reader.Read()) {
                //            return reader.GetValue(0).ToString();
                //        } else {
                //            return "nothing";
                //        }
                //    }
                //}

                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
                {
                    var query = @"Select 
	                            tickets.TicketID,
	                            tickets.Name,
	                            tickets.OrganizationID,
	                            tickets.ProductID,
	                            tickets.IsVisibleOnPortal,
	                            tags.Value
                              From dbo.tickets as Tickets
	                            inner join dbo.TagLinks as TagLinks
		                            on TagLinks.RefType = 17 and TagLinks.RefID = tickets.TicketID
	                            inner join dbo.Tags as Tags
		                            on Tags.TagID = TagLinks.TagID
                              Where tickets.IsKnowledgeBase = 1
                              For JSON Auto";
                    var cmd = new SqlCommand(query, connection);
                    connection.Open();

                    var jsonResult = new StringBuilder();
                    SqlDataReader reader = cmd.ExecuteReader();

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
            catch (SqlException e) {
                return "error";
                // return e.Message;
            }
            catch (Exception e) {
                return "error";
            }

        }

    }
}
