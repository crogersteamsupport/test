using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Xml;
using System.Data;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Specialized;
using System.Configuration.Assemblies;
using System.Data.Linq;

namespace WatsonToneAnalyzer
{
    /// <summary>
    /// Global class to search the entire DB for actions which need to be Watson analyzed
    /// </summary>
    public class ActionsToAnalyzer
    {
        const string EVENT_SOURCE = "Application";

        public static void GetHTML()
        {
            Console.WriteLine("GettingHTML");
            //opens a sqlconnection at the specified location
            try
            {
                String ConnectionString = ConfigurationManager.AppSettings.Get("ConnectionString");

                using (SqlConnection sqlConnection1 = new SqlConnection(ConnectionString))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        SqlDataReader reader;
                        //enters the GET querry from the action table and saves the response 
                        cmd.CommandText = "dbo.ActionsGetForWatson";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ActionsBatchSize", ConfigurationManager.AppSettings.Get("ActionsBatchSize"));
                        cmd.Connection = sqlConnection1;
                        sqlConnection1.Open();


                        //opens up a reading channel and selects a row for reading
                        using (reader = cmd.ExecuteReader())
                        {
                            DataContext db = new DataContext(sqlConnection1);
                            Table<ActionToAnalyze> table = db.GetTable<ActionToAnalyze>();

                            while (reader.Read())
                            {
                                String Description = reader["Description"].ToString();
                                Description = ActionToAnalyze.CleanStringV2(Description);
                                // use Linq to avoid escape character problems with the SQL text interface...
                                ActionToAnalyze actionToAnalyze = new ActionToAnalyze()
                                {
                                    ActionID = (int)reader["ActionID"],
                                    TicketID = (int)reader["TicketID"],
                                    UserID = (int)reader["CreatorID"],
                                    OrganizationID = (int)reader["OrganizationID"],
                                    IsAgent = (reader["IsAgent"].ToString() == "1"),
                                    DateCreated = DateTime.Now,
                                    ActionDescription = Description.ToString()
                                };

                                // insert if not found
                                if (!table.Where(u => u.ActionID == actionToAnalyze.ActionID).Any())
                                    table.InsertOnSubmit(actionToAnalyze);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                EventLog.WriteEntry(EVENT_SOURCE, "Exception while reading from action table:" + e.ToString() + " ----- STACK: " + e.StackTrace.ToString());
                Console.WriteLine(e.ToString());
            }
        }
    }

}