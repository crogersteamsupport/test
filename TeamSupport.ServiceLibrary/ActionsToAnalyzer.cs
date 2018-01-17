﻿using System;
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
                    cmd.CommandText = "dbo.ActionsGetForWatson";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ActionsBatchSize", ConfigurationManager.AppSettings.Get("ActionsBatchSize"));
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
                                else
                                {

                                    using (SqlConnection sqlConnection2 = new SqlConnection(ConnectionString))
                                    {
                                        using (SqlCommand cmd2 = new SqlCommand())
                                        {
                                            String CommandText1 = "INSERT INTO[dbo].[ActionSentiments]([ActionID],[TicketID],[UserID],[OrganizationID],[IsAgent],[DateCreated]) VALUES(" + reader["ActionID"].ToString() + ",0,0,0,0,GETDATE() )";

                                            cmd2.CommandText = CommandText1;
                                            cmd2.CommandType = CommandType.Text;
                                            cmd2.Connection = sqlConnection2;
                                            sqlConnection2.Open();
                                            cmd2.ExecuteNonQuery();
                                        }
                                    }
                                }

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
