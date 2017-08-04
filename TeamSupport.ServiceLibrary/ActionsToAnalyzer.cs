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

public class ActionsToAnalyzer
{

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


                    String SQLCommandText = ConfigurationManager.AppSettings.Get("SQLSelectFromActions");
                    
                    //SQLCommandText = "SELECT top 1 a.[ActionID], a.[TicketID],a.[Description],  t.[CreatorID], a.[CreatorID], u.[OrganizationID], (CASE WHEN (Select [OrganizationID] From Users where userid = t.[CreatorID] ) = u.[OrganizationID] THEN 0 ELSE 1 END ) as [IsAgent] FROM Actions a INNER JOIN Users u ON a.[CreatorID] = u.[UserID] INNER JOIN Organizations o ON o.[OrganizationID] = u.[OrganizationID] INNER JOIN Tickets t ON a.[TicketID] = t.[TicketID] WHERE  a.[IsVisibleOnPortal] = 0 AND t.[IsVisibleOnPortal] = 0 AND ( SELECT [ProductType] FROM Organizations where OrganizationId in (Select [OrganizationID] From Users where userid = t.[CreatorID] ) ) =2 AND a.[CreatorID] != -1 AND ActionID > (SELECT Max([ActionID]) from [ActionSentiments]) ORDER BY [ActionID] Asc";                 
                    cmd.CommandText = SQLCommandText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = sqlConnection1;
                    sqlConnection1.Open();
                    

                    //opens up a reading channel and selects a row for reading
                    using (reader = cmd.ExecuteReader())
                    {
                   
                        while (reader.Read())
                        {
                            
                            if (!(reader["Description"].ToString() == ""))
                            {

                                String Description = reader["Description"].ToString();

                                String Description1 = CleanStringV2(Description); //cleans the string of all the html and special charecters
                                if (Description1.Length >= 500)
                                {
                                    Description1 = Description1.Substring(0, 499);
                                }
                                if (Description1.Length > 50)
                                {
                                    PostEntry(Description1, reader["ActionID"].ToString(), reader["TicketID"].ToString(), reader["CreatorID"].ToString(), reader["OrganizationID"].ToString(), reader["IsAgent"].ToString()); //posts entry to the actiontoanalyze table
                                }
                                //else
                               // {
                                //    EventLog.WriteEntry("Application", "Action Skipped - description too short: " + Description1);
                               // }
                                    //Console.WriteLine("After");
                            }
                            else
                            {
                                PostEntry(" ", reader["ActionID"].ToString(), reader["TicketID"].ToString(), reader["CreatorID"].ToString(), reader["OrganizationID"].ToString(), reader["IsAgent"].ToString());
                            }

                        }

                    }
                    sqlConnection1.Close();
                    reader.Close();
                }
            }
            
        }
        catch(Exception e)
        {
            string sSource = "Application";
            EventLog.WriteEntry(sSource, "Exception while reading from action table:" + e.ToString() + " ----- STACK: " + e.StackTrace.ToString());
            Console.WriteLine(e.ToString());
        }
    }

    public static void PostEntry(String Description,String ActionID, String TicketID, String UserID, String OrganizationID,String IsAgent)
    {
        String ConnectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
        using (SqlConnection sqlConnection1 = new SqlConnection(ConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SB1 = new StringBuilder();
                //enters the GET querry from the action table and saves the response 
                SB1.AppendLine("IF NOT EXISTS (SELECT [ActionID] FROM [ActionToAnalyze] WHERE [ActionID] =" + ActionID + " AND [UserID]=" + UserID + " ) BEGIN ");
                SB1.AppendLine(" INSERT INTO [dbo].[ActionToAnalyze] ([ActionID],[TicketID],[UserID],[OrganizationID],[IsAgent],[DateCreated], [ActionDescription]) VALUES(" + ActionID + "," + TicketID + "," + UserID + "," + OrganizationID + "," + IsAgent + "," + "GETDATE(),\'" + Description + "\')");
                SB1.AppendLine("END ");

                cmd.CommandText = SB1.ToString();
                
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection1;

                sqlConnection1.Open();
                cmd.ExecuteNonQuery();

                sqlConnection1.Close();
            }
        }
    }
    
    static String CleanStringV2(String RawHtml)
    {
        String Html = Regex.Replace(RawHtml, @"<[^>]*>", String.Empty); //removes html tags
        Html = Regex.Replace(Html, "nbsp;", " "); //removes strange nbsp tags
        Html = Regex.Replace(Html, @"[\d-]", " "); //removes all digits
        Html = Regex.Replace(Html, @"[\w\d]+\@[\w\d]+\.com", " "); //removes email adresses
        StringBuilder sb = new StringBuilder();
        char previous = ' ';
        foreach (char c in Html)  //goes through every charecter in the string and only passes on valid charecters to the new string
        {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' && previous!='.' || c==' ' && previous != ' '|| c== '?')
            {
                sb.Append(c);
            }
            previous = c;
        }
        return sb.ToString();
    }

}
