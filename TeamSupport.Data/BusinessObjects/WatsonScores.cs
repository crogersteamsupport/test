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

    public partial class WatsonScores {

        public static string PullSummary(LoginUser loginUser, int ticketID) {
            try {
                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString)) {
                    using (SqlCommand command = new SqlCommand()) {
                        command.Connection   = connection;
                        command.CommandType  = CommandType.Text;
                        command.CommandText  = "SELECT ActionSentimentScores.* FROM dbo.ActionSentimentScores ";
                        command.CommandText += "INNER JOIN dbo.ActionSentiments ON ActionSentiments.ActionSentimentID = ActionSentimentScores.ActionSentimentID ";
                        command.CommandText += "WHERE ActionSentiments.TicketID = @TicketID ";
                        command.CommandText += "GROUP BY ActionSentimentScores.SentimentID ";
                        command.CommandText += "FOR JSON PATH, ROOT('scores') ";
                        command.Parameters.AddWithValue("@TicketID", ticketID);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows && reader.Read()) {
                            return reader.GetValue(0).ToString();
                        } else {
                            return "nothing";
                        }
                    }
                }
            }
            catch (SqlException e) {
                return "negative";
            }
            catch (Exception e) {
                return "negative";
            }
        }

    }
}
