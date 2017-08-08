using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net.Http.Formatting;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Xml;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Specialized;




public class WatsonAnalyzer
{

    const string EVENT_SOURCE = "Application";

    static public void GetAction()
    {

        //EventLog.WriteEntry(EVENT_SOURCE, "GetAction");

        try
        {
            //opens a sqlconnection at the specified location
            String SQLConnectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
            using (SqlConnection sqlConnection1 = new SqlConnection(SQLConnectionString))
            {

                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                //enters the GET querry from the action table and saves the response 
                cmd.CommandText = ConfigurationManager.AppSettings.Get("SQLSelectFromActionsToAnalyze");
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection1;
                sqlConnection1.Open();

                //opens up a reading channel and selects a row for reading
                using (reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        String TicketID = reader["TicketID"].ToString(); //reads the TicketID and CreatorID
                        String UserID = reader["UserID"].ToString();
                        String OrganizationID = reader["OrganizationID"].ToString();
                        //String OrganizationID = reader["OrganizationID"].ToString();
                        String IsAgent = reader["IsAgent"].ToString();
                        //String ActionSentimentID = reader["ActionSentimentID"].ToString();

                        //Calls the Watson Post function
                        String Username = ConfigurationManager.AppSettings.Get("WatsonUsername");
                        String Password = ConfigurationManager.AppSettings.Get("WatsonPassword");
                        //EventLog.WriteEntry(EVENT_SOURCE, "Posting to Watson");
                        if (reader["ActionDescription"].ToString().Length >= 500)
                        {

                            HTTP_POST(reader["ActionID"].ToString(), reader["ActionDescription"].ToString().Substring(0, 499), Username, Password,
                                (result) => PublishToTable(result, TicketID, UserID, OrganizationID, IsAgent));
                        }
                        else
                        {

                            HTTP_POST(reader["ActionID"].ToString(), reader["ActionDescription"].ToString(), Username, Password,
                                (result) => PublishToTable(result, TicketID, UserID, OrganizationID, IsAgent));
                        }
                    }
                }

                sqlConnection1.Close();
            }
        }
        catch (SqlException e1)
        {
            EventLog.WriteEntry(EVENT_SOURCE, "There was an issues with the sql server:" + e1.ToString() + " ----- STACK: " + e1.StackTrace.ToString());
            throw (e1);
        }
        catch (Exception e2)
        {
            EventLog.WriteEntry(EVENT_SOURCE, "Exception caught at select from ACtionsToAnalyze or HttpPOST:" + e2.Message + " ----- STACK: " + e2.StackTrace.ToString());
            Console.WriteLine(e2.ToString());
        }

    }

    static void MakeSqlRequest(String CommandText1, String CommandText2, String AverageText1, String AverageText2, String AverageText3, String ActionID, String SentimentID, String SentimentScore, Action<String> callback)
    {
        try
        {
            String SQLConnectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
            using (SqlConnection sqlConnection2 = new SqlConnection(SQLConnectionString))
            {
                CommandText1 += "; SELECT CAST(scope_identity() AS int)";
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = CommandText1;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = sqlConnection2;
                   // EventLog.WriteEntry("Application", "SQL:" + cmd.CommandText);
                    sqlConnection2.Open();
                    var result = cmd.ExecuteScalar().ToString();
                    if (result != null)
                    {
                        string ActionSentimentID = result.ToString();
                        if (!String.IsNullOrEmpty(ActionSentimentID))
                        {
                            CommandText2 += result.ToString() + "," + SentimentID + "," + SentimentScore + ")";
                            cmd.CommandText = CommandText2;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    if (!(AverageText1 == null))
                    {
                        cmd.CommandText = AverageText1;
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = AverageText2;
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = AverageText3;
                        cmd.ExecuteNonQuery(); 
                    }
                    sqlConnection2.Close();
                }
            }
            callback(ActionID);
        }
        catch (Exception Y)
        {
            EventLog.WriteEntry("Application", String.Format("SQL : {0}  :  {1}  : {2}  : {3}", AverageText1, AverageText2, AverageText3));
            EventLog.WriteEntry("Application", "Error while making a SQL Request:" + Y.ToString() + " ----- STACK: " + Y.StackTrace.ToString()); 
        }

    }
    static void MakeSqlDelete(String CommandText)
    {
        try
        {
            String SQLConnectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
            using (SqlConnection sqlConnection1 = new SqlConnection(SQLConnectionString))
            {

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = CommandText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = sqlConnection1;
                    sqlConnection1.Open();
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Deleted");
                    sqlConnection1.Close();
                }
            }
        }
        catch (Exception Y)
        {
            EventLog.WriteEntry("Application", "Error while making a SQL call :" + Y.ToString() + " ----- STACK: " + Y.StackTrace.ToString());
        }

    }

    static void PublishToTable(Response result, String TicketID, String UserID, String OrganizationID, String IsAgent)
    {
        //Converts the serialized json response into a list of tone objects 
        Console.WriteLine("Trying");
        try
        {
            if (IsAgent == "False")
            {
                IsAgent = "0";
            }
            else
            {
                IsAgent = "1";
            }
            //EventLog.WriteEntry(EVENT_SOURCE, "PublishToTable1:" + result.ToString());
            //EventLog.WriteEntry(EVENT_SOURCE, "PublishToTable2:" + result.WatsonResponse.Length.ToString());

            List<Char> Test = result.WatsonResponse.ToList();

            ToneList response = JsonConvert.DeserializeObject<ToneList>(result.WatsonResponse);
            if (response != null)
            {

                if (response.utterances_tone != null)
                {

                    //EventLog.WriteEntry(EVENT_SOURCE, "PublishToTable4: response.utterances_tone != null");

                    if (response.utterances_tone[0] != null)
                    {
                        //EventLog.WriteEntry(EVENT_SOURCE, "PublishToTable5: response.utterances_tone[0] !=null");
                        List<Tones> ToneList = response.utterances_tone[0].tones; //creates the list of tones to be added to the database
                                                                                  //List<Tones> ToneList = new List<Tones>();
                        if (!ToneList.Any())
                        {

                            String CommandText1 = "INSERT INTO[dbo].[ActionSentiments]([ActionID],[TicketID],[UserID],[OrganizationID],[IsAgent],[DateCreated]) VALUES(" + result.ActionID + "," + TicketID + "," + UserID + "," + OrganizationID + "," + IsAgent + ",GETDATE() )";
                            Console.WriteLine(CommandText1.ToString());
                            String CommandText2 = "INSERT INTO [dbo].[ActionSentimentScores]([ActionSentimentID], [SentimentID], [SentimentScore] ) VALUES(";
                            String DeleteText = "DELETE  FROM [dbo].[ActionToAnalyze] WHERE ActionID =";
                            String SentimentID = "0";
                            String Score = "0";
                            StringBuilder SB1 = new StringBuilder();

                            SB1.Append(" IF EXISTS (SELECT[TicketID] FROM [TicketAverageSentiment] WHERE[TicketID] =" + TicketID + " AND [SentimentID]=" + SentimentID + " ) BEGIN ");
                            SB1.Append("UPDATE [TicketAverageSentiment] SET [RecordCount] = [RecordCount] +1, [SentimentScore] = (SELECT((([SentimentScore] * [RecordCount]) +" + Score + ") / ([RecordCount] + 1))  AS Total FROM[TicketAverageSentiment] WHERE[TicketID] =" + TicketID + "AND [SentimentID] =" + SentimentID + ") WHERE [TicketID] =" + TicketID);
                            SB1.Append(" END ELSE BEGIN INSERT INTO[dbo].[TicketAverageSentiment]  ([TicketID], [SentimentID], [SentimentScore], [RecordCount]) VALUES(" + TicketID + "," + SentimentID + "," + Score + ",1) END");

                            String AverageCommandText1 = SB1.ToString();
                            SB1.Clear();

                            String AverageCommandText2 = string.Empty;
                            if (IsAgent == "0")
                            {
                                SB1.Append("IF EXISTS (SELECT[UserID] FROM [CustomerAverageSentiment] WHERE[UserID] =" + UserID + " AND [SentimentID]=" + SentimentID + " ) BEGIN ");
                                SB1.Append("UPDATE [CustomerAverageSentiment] SET [RecordCount] = [RecordCount] +1, [SentimentScore] = (SELECT((([SentimentScore] * [RecordCount]) +" + Score + ") / ([RecordCount] + 1))  AS Total FROM[CustomerAverageSentiment] WHERE[UserID] =" + UserID + "AND [SentimentID] =" + SentimentID + ") WHERE [UserID] =" + UserID);
                                SB1.Append(" END ELSE BEGIN INSERT INTO[dbo].[CustomerAverageSentiment]  ([UserID], [SentimentID], [SentimentScore], [RecordCount]) VALUES(" + UserID + "," + SentimentID + "," + Score + ",1) END");

                                AverageCommandText2 = SB1.ToString();
                                SB1.Clear();
                            }
                            else
                            {
                                SB1.Append("IF EXISTS (SELECT[UserID] FROM [AgentAverageSentiment] WHERE[UserID] =" + UserID + " AND [SentimentID]=" + SentimentID + " ) BEGIN ");
                                SB1.Append("UPDATE [AgentAverageSentiment] SET [RecordCount] = [RecordCount] +1, [SentimentScore] = (SELECT((([SentimentScore] * [RecordCount]) +" + Score + ") / ([RecordCount] + 1))  AS Total FROM[AgentAverageSentiment] WHERE[UserID] =" + UserID + "AND [SentimentID] =" + SentimentID + ") WHERE [UserID] =" + UserID);
                                SB1.Append(" END ELSE BEGIN INSERT INTO[dbo].[AgentAverageSentiment]  ([UserID], [SentimentID], [SentimentScore], [RecordCount]) VALUES(" + UserID + "," + SentimentID + "," + Score + ",1) END");

                                AverageCommandText2 = SB1.ToString();
                                SB1.Clear();
                            }

                            SB1.Append("IF EXISTS (SELECT[OrganizationID] FROM [OrganizationsAverageSentiment] WHERE[OrganizationID] =" + OrganizationID + " AND [SentimentID]=" + SentimentID + " ) BEGIN ");
                            SB1.Append("UPDATE [OrganizationsAverageSentiment] SET [RecordCount] = [RecordCount] +1, [SentimentScore] = (SELECT((([SentimentScore] * [RecordCount]) +" + Score + ") / ([RecordCount] + 1))  AS Total FROM[OrganizationsAverageSentiment] WHERE[OrganizationID] =" + OrganizationID + "AND [SentimentID] =" + SentimentID + ") WHERE [OrganizationID] =" + OrganizationID);
                            SB1.Append(" END ELSE BEGIN INSERT INTO[dbo].[OrganizationsAverageSentiment]  ([OrganizationID], [SentimentID], [SentimentScore], [RecordCount]) VALUES(" + OrganizationID + "," + SentimentID + "," + Score + ",1) END");

                            String AverageCommandText3 = SB1.ToString();
                            SB1.Clear();

                            MakeSqlRequest(CommandText1, CommandText2, AverageCommandText1, AverageCommandText2, AverageCommandText3, result.ActionID, SentimentID.ToString(), Score, (action) => MakeSqlDelete(DeleteText + action));

                        }
                        else
                        {
                            foreach (Tones item in ToneList)
                            {
                                int SentimentID = FindSentimentID(item.tone_id); //Finds the id of the sentiment returned by Watson and puts it into an ID form for the Sentiment Table

                                String SQLConnectionString = ConfigurationManager.AppSettings.Get("ConnectionString");

                                StringBuilder SB1 = new StringBuilder();
                                SB1.Append(" IF NOT EXISTS (SELECT TOP 1 [TicketID] FROM [ActionSentiments] WHERE [ActionID] =" + result.ActionID + " AND [TicketID] =" + TicketID + " AND [OrganizationID]=" + OrganizationID + "  AND [UserID]=" + UserID + " ) BEGIN ");
                                SB1.Append("INSERT INTO[dbo].[ActionSentiments]([ActionID],[TicketID],[UserID],[OrganizationID],[IsAgent],[DateCreated]) VALUES(" + result.ActionID + "," + TicketID + "," + UserID + "," + OrganizationID + "," + IsAgent + ",GETDATE() )");
                                SB1.Append(" END ");

                                String CommandText1 = SB1.ToString();
                                SB1.Clear();
                                 
                                String CommandText2 = "INSERT INTO [dbo].[ActionSentimentScores]([ActionSentimentID], [SentimentID], [SentimentScore] ) VALUES(";
                                String DeleteText = "DELETE  FROM [dbo].[ActionToAnalyze] WHERE ActionID =";
                                

                                SB1.Append(" IF EXISTS (SELECT TOP 1 [TicketID] FROM [TicketAverageSentiment] WHERE[TicketID] =" + TicketID + " AND [SentimentID]=" + SentimentID + " ) BEGIN ");
                                SB1.Append("UPDATE [TicketAverageSentiment] SET [RecordCount] = [RecordCount] +1, [SentimentScore] = (SELECT((([SentimentScore] * [RecordCount]) +" + item.score + ") / ([RecordCount] + 1))  AS Total FROM[TicketAverageSentiment] WHERE[TicketID] =" + TicketID + "AND [SentimentID] =" + SentimentID + ") WHERE [TicketID] =" + TicketID + "AND [SentimentID] =" + SentimentID);
                                SB1.Append(" END ELSE BEGIN INSERT INTO[dbo].[TicketAverageSentiment]  ([TicketID], [SentimentID], [SentimentScore], [RecordCount]) VALUES(" + TicketID + "," + SentimentID + "," + item.score + ",1) END");

                                String AverageCommandText1 = SB1.ToString();
                                SB1.Clear();

                                String AverageCommandText2 = string.Empty;
                                if (IsAgent == "0")
                                {
                                    SB1.Append("IF EXISTS (SELECT TOP 1 [UserID] FROM [CustomerAverageSentiment] WHERE[UserID] =" + UserID + " AND [SentimentID]=" + SentimentID + " ) BEGIN ");
                                    SB1.Append("UPDATE [CustomerAverageSentiment] SET [RecordCount] = [RecordCount] +1, [SentimentScore] = (SELECT((([SentimentScore] * [RecordCount]) +" + item.score + ") / ([RecordCount] + 1))  AS Total FROM[CustomerAverageSentiment] WHERE   [UserID] =" + UserID + "AND [SentimentID] =" + SentimentID + ") WHERE [UserID] =" + UserID + "AND [SentimentID] =" + SentimentID);
                                    SB1.Append(" END ELSE BEGIN INSERT INTO[dbo].[CustomerAverageSentiment]  ([UserID], [SentimentID], [SentimentScore], [RecordCount]) VALUES(" + UserID + "," + SentimentID + "," + item.score + ",1) END");

                                    AverageCommandText2 = SB1.ToString();
                                    SB1.Clear();
                                }
                                else
                                {
                                    SB1.Append("IF EXISTS (SELECT TOP 1 [UserID] FROM [AgentAverageSentiment] WHERE[UserID] =" + UserID + " AND [SentimentID]=" + SentimentID + " ) BEGIN ");
                                    SB1.Append("UPDATE [AgentAverageSentiment] SET [RecordCount] = [RecordCount] +1, [SentimentScore] = (SELECT((([SentimentScore] * [RecordCount]) +" + item.score + ") / ([RecordCount] + 1))  AS Total FROM[AgentAverageSentiment] WHERE   [UserID] =" + UserID + "AND [SentimentID] =" + SentimentID + ") WHERE [UserID] =" + UserID + "AND [SentimentID] =" + SentimentID);
                                    SB1.Append(" END ELSE BEGIN INSERT INTO[dbo].[AgentAverageSentiment]  ([UserID], [SentimentID], [SentimentScore], [RecordCount]) VALUES(" + UserID + "," + SentimentID + "," + item.score + ",1) END");

                                    AverageCommandText2 = SB1.ToString();
                                    SB1.Clear();
                                }

                                SB1.Append("IF EXISTS (SELECT TOP 1 [OrganizationID] FROM [OrganizationsAverageSentiment] WHERE[OrganizationID] =" + OrganizationID + " AND [SentimentID]=" + SentimentID + " ) BEGIN ");
                                SB1.Append("UPDATE [OrganizationsAverageSentiment] SET [RecordCount] = [RecordCount] +1, [SentimentScore] = (SELECT((([SentimentScore] * [RecordCount]) +" + item.score + ") / ([RecordCount] + 1))  AS Total FROM[OrganizationsAverageSentiment] WHERE  [OrganizationID] =" + OrganizationID + "AND [SentimentID] =" + SentimentID + ") WHERE [OrganizationID] =" + OrganizationID + "AND [SentimentID] =" + SentimentID );
                                SB1.Append(" END ELSE BEGIN INSERT INTO[dbo].[OrganizationsAverageSentiment]  ([OrganizationID], [SentimentID], [SentimentScore], [RecordCount]) VALUES(" + OrganizationID + "," + SentimentID + "," + item.score + ",1) END");

                                String AverageCommandText3 = SB1.ToString();
                                SB1.Clear();

                                MakeSqlRequest(CommandText1, CommandText2, AverageCommandText1, AverageCommandText2, AverageCommandText3, result.ActionID, SentimentID.ToString(), item.score.ToString(), (action) => MakeSqlDelete(DeleteText + action));
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            EventLog.WriteEntry(EVENT_SOURCE, "********************PublishToTable: Exception at insert into Action Sentiment:" + e.Message + " ### " + e.Source + " ### " + e.StackTrace.ToString());
            Console.WriteLine("Exception at insert into Action Sentiment:" + e.Message + "###" + e.Source + " ----- STACK: " + e.StackTrace.ToString());
            throw (e);
        }

    }

    static async void HTTP_POST(String UserID, String InputText, String Username, String Password, Action<Response> callback)
    {
        //Create Json Readable String with user input:    
        try
        {
            if (InputText != null || InputText != "")
            {

                //This is the format that Watson excepts for the Json Input. The two text fields have to be formatted without any protected charecters
                String jsonString = "{\r\n  \"utterances\": [\r\n    {\r\n      \"text\":" + "\"" + InputText + "\"" + ",\r\n      \"user\":" + "\"" + UserID + "\"" + "\r\n  }\r\n  ]\r\n}\r\n";
                //EventLog.WriteEntry(EVENT_SOURCE, "****HTTP_POST1" + jsonString);

                using (HttpClient client = new HttpClient())
                {   //Establish client
                    var TargetUrl = ConfigurationManager.AppSettings.Get("WatsonGatewayUrl");

                    //Concatonate credentials and pass authorization to the client header
                    var Auth = Username + ":" + Password;
                    var byteArray = Encoding.ASCII.GetBytes(Auth);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    //add header with input type: json
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Make Post call and await response
                    using (var response = await client.PostAsJsonAsync(TargetUrl, JObject.Parse(jsonString)))
                    {
                        HttpContent content = response.Content;

                        //EventLog.WriteEntry(EVENT_SOURCE, "****HTTP_POST2" + content.ToString());
                        //Format response and write to console (should be changed eventually to post to table using sql protocol
                        var formatted = response.Content.ReadAsStringAsync().Result ?? " ";
                        string result = await content.ReadAsStringAsync() ?? " ";

                        //Create result object to organize response
                        var ResultResponse = new Response();
                        ResultResponse.ActionID = UserID;
                        ResultResponse.InputText = InputText;

                        //EventLog.WriteEntry(EVENT_SOURCE, "****HTTP_POST3" + InputText.ToString());
                        ResultResponse.WatsonResponse = result;

                        //EventLog.WriteEntry(EVENT_SOURCE, "****HTTP_POST4" + content.ToString());
                        callback(ResultResponse); //returns the response object to pass on to the postSQL class
                        //EventLog.WriteEntry(EVENT_SOURCE, "****HTTP_POST5" + ResultResponse.WatsonResponse.ToString());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            EventLog.WriteEntry(EVENT_SOURCE, String.Format("********************HTTP_POST: Input: {0} -------------- Error durring watson analysis: {1}  ----STACK:{2} ", InputText, ex.Message, ex.StackTrace.ToString()));
            System.Threading.Thread.Sleep(1000);
        }
    }

    static int FindSentimentID(string Sentiment)
    {
        if (Sentiment == null)
        {
            return 0;
        }
        int SentimentID;
        if (Sentiment == "sad")
        {
            SentimentID = 1;
        }
        else if (Sentiment == "frustrated")
        {
            SentimentID = 2;
        }
        else if (Sentiment == "satisfied")
        {
            SentimentID = 3;
        }
        else if (Sentiment == "excited")
        {
            SentimentID = 4;
        }
        else if (Sentiment == "polite")
        {
            SentimentID = 5;
        }
        else if (Sentiment == "impolite")
        {
            SentimentID = 6;
        }
        else if (Sentiment == "sympathetic")
        {
            SentimentID = 7;
        }
        else
        {
            SentimentID = 0;
        }
        return SentimentID;
    }

}
//creates an object to format the Watson Response
public class Response
{
    public String ActionID { get; set; }
    public String InputText { get; set; }
    public String WatsonResponse { get; set; }
}

//Creates the deserialize object for Json Returning from Watson
public class ToneList
{
    public List<Utterance> utterances_tone { get; set; }
}
public class Utterance
{
    public List<Tones> tones { get; set; }
}
public class Tones
{
    public float score { get; set; }
    public String tone_id { get; set; }

}




