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
using System.Data.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WatsonToneAnalyzer
{
    /// <summary>
    /// Class for the watson service to get the actions to analyze, send
    /// them to Watson, and to insert the results back into the DB
    /// </summary>
    public class WatsonAnalyzer
    {
        const string EVENT_SOURCE = "Application";
        const int MaxUtteranceCount = 50;
        const int MaxTotalTextContentLength = 128000;

        /// <summary>
        /// Get the actions to analyze (dbo.ActionToAnalyze) and post to Watson on the BlueMix account
        /// </summary>
        static public void AnalyzeActions()
        {
            //TicketSentiment.StressTest();
            //int orgSentiment = TicketSentiment.OrganizationSentiment(1078);


            // without this the HTTP message to Watson returns 405 - failure on send
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            //EventLog.WriteEntry(EVENT_SOURCE, "GetAction");
            try
            {
                //opens a sqlconnection at the specified location
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // use linq to get the actions to send to watson
                    using (DataContext db = new DataContext(connection))
                    {
                        Table<ActionSentiment> actionSentimentTable = db.GetTable<ActionSentiment>();
                        List<ActionToAnalyze> actionsToAnalyzeList = new List<ActionToAnalyze>();
                        int utteranceCount = 0;
                        int textSize = 0;

                        Table<ActionToAnalyze> actionToAnalyzeTable = db.GetTable<ActionToAnalyze>();
                        IQueryable<ActionToAnalyze> actionToAnalyzeQuery = from action in actionToAnalyzeTable select action;
                        foreach (ActionToAnalyze actionToAnalyze in actionToAnalyzeQuery)
                        {
                            // ActionSentiment record already exists?
                            if (actionSentimentTable.Where(u => u.ActionID == actionToAnalyze.ActionID).Any())
                            {
                                actionToAnalyze.DeleteOnSubmit(db);
                                db.SubmitChanges();
                                continue;
                            }

                            // pack actionsToAnalyze into a single billable API call
                            actionToAnalyze.ActionDescription = actionToAnalyze.WatsonText();   // clean and truncate
                            textSize += actionToAnalyze.ActionDescription.Length;
                            if ((++utteranceCount > MaxUtteranceCount) || (textSize > MaxTotalTextContentLength))    // watson API call restrictions
                            {
                                // send them all off to watson - async
                                HTTP_POST(actionsToAnalyzeList, (result) => PublishToTable(result));
                                utteranceCount = 0;
                                textSize = 0;
                                actionsToAnalyzeList = new List<ActionToAnalyze>();
                            }

                            actionsToAnalyzeList.Add(actionToAnalyze);
                        }

                        // the remainder can wait to be packed into a call when we have more...
                    }
                }
            }
            catch (SqlException e1)
            {
                EventLog.WriteEntry(EVENT_SOURCE, "There was an issues with the sql server:" + e1.ToString() + " ----- STACK: " + e1.StackTrace.ToString());
                Console.WriteLine(e1.ToString());
            }
            catch (Exception e2)
            {
                EventLog.WriteEntry(EVENT_SOURCE, "Exception caught at select from ACtionsToAnalyze or HttpPOST:" + e2.Message + " ----- STACK: " + e2.StackTrace.ToString());
                Console.WriteLine(e2.ToString());
            }
            //finally()

        }

        static void PublishToTable(Response response)
        {
            for (int i = 0; i < response.ActionsToAnalyze.Count; ++i)
            {
                PublishToTable(response.ActionsToAnalyze[i], response.WatsonResponse.utterances_tone[i]);
            }
        }

        /// <summary>
        /// Async callback from HTTP_POST to put the watson response into the db
        /// </summary>
        /// <param name="result">Watson results</param>
        /// <param name="actionToAnalyze">ActionToAnalyze record we are processing</param>
        static Mutex _singleThreadedTransactions = new Mutex(false);
        static void PublishToTable(ActionToAnalyze actionToAnalyze, Utterance result)
        {
            // Process The ActionToAnalyze results
            WatsonTransaction transaction = null;  // Transaction that can be rolled back
            try
            {
                // 1. Insert ActionSentiment and ActionSentimentScores
                // 2. run the TicketSentimentStrategy to create TicketSentimentScore
                // 3. delete the ActionToAnalyze
                _singleThreadedTransactions.WaitOne();  // connection does not support parallel transactions
                transaction = new WatsonTransaction();
                transaction.RecordWatsonResults(result, actionToAnalyze);
                transaction.Commit();
            }
            catch (Exception e2)
            {
                if (transaction != null)
                    transaction.Rollback();
                EventLog.WriteEntry(EVENT_SOURCE, "Watson analysis failed - system will retry" + e2.Message + " ----- STACK: " + e2.StackTrace.ToString());
                Console.WriteLine(e2.ToString());
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
                _singleThreadedTransactions.ReleaseMutex();
            }

            // update the corresponding ticket sentiment
            TicketSentiment.TicketSentimentStrategy(actionToAnalyze.TicketID, actionToAnalyze.OrganizationID, actionToAnalyze.IsAgent);
        }

        /// <summary>
        /// Post to IBM watson with Authorization and JSON formatted utterances to process
        /// </summary>
        /// <param name="UserID">DB ID of user inserting the action</param>
        /// <param name="InputText">Action text</param>
        /// <param name="callback">Callback function</param>
        static async void HTTP_POST(List<ActionToAnalyze> analyzeList, Action<Response> callback)
        {
            string WatsonGatewayUrl = ConfigurationManager.AppSettings.Get("WatsonGatewayUrl");
            string WatsonUsername = ConfigurationManager.AppSettings.Get("WatsonUsername");
            string WatsonPassword = ConfigurationManager.AppSettings.Get("WatsonPassword");

            //Create Json Readable String with user input:                
            try
            {
                if (analyzeList.Count == 0)
                    return;

                //This is the format that Watson excepts for the Json Input. The two text fields have to be formatted without any protected charecters
                WatsonPostContent toJson = new WatsonPostContent();
                foreach (ActionToAnalyze actionToAnalyze in analyzeList)
                    toJson.Add(actionToAnalyze.ActionID.ToString(), actionToAnalyze.ActionDescription);
                string jsonString = toJson.ToString();

                //EventLog.WriteEntry(EVENT_SOURCE, "****HTTP_POST1" + jsonString);

                using (HttpClient client = new HttpClient())
                {   //Establish client
                    //Concatonate credentials and pass authorization to the client header
                    var Auth = WatsonUsername + ":" + WatsonPassword;
                    var byteArray = Encoding.ASCII.GetBytes(Auth);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    //add header with input type: json
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Make Post call and await response
                    using (var response = await client.PostAsJsonAsync(WatsonGatewayUrl, JObject.Parse(jsonString)))
                    {
                        HttpContent content = response.Content;

                        //EventLog.WriteEntry(EVENT_SOURCE, "****HTTP_POST2" + content.ToString());
                        //Format response and write to console (should be changed eventually to post to table using sql protocol
                        var formatted = response.Content.ReadAsStringAsync().Result ?? " ";
                        string result = await content.ReadAsStringAsync() ?? " ";

                        //Create result object to organize response
                        var ResultResponse = new Response();
                        ResultResponse.ActionsToAnalyze = analyzeList;
                        ResultResponse.WatsonResponse = JsonConvert.DeserializeObject<UtteranceToneList>(result);

                        //EventLog.WriteEntry(EVENT_SOURCE, "****HTTP_POST4" + content.ToString());
                        callback(ResultResponse); //returns the response object to pass on to the postSQL class
                                                  //EventLog.WriteEntry(EVENT_SOURCE, "****HTTP_POST5" + ResultResponse.WatsonResponse.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(EVENT_SOURCE, String.Format("********************: Error durring watson analysis: {0}  ----STACK:{1} ", ex.Message, ex.StackTrace.ToString()));
                Console.WriteLine(ex.ToString());
                System.Threading.Thread.Sleep(1000);
            }
        }
    }

    //creates an object to format the Watson Response
    class Response
    {
        public List<ActionToAnalyze> ActionsToAnalyze { get; set; }
        public UtteranceToneList WatsonResponse { get; set; }
    }

    //Creates the deserialize object for Json Returning from Watson
    public class UtteranceToneList
    {
        public List<Utterance> utterances_tone { get; set; }

        public Utterance First
        {
            get { return (utterances_tone != null) ? utterances_tone[0] : null; }
        }
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
}

/*
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;

    /// <summary>TODO - use MQ instead of dbo.ActionToAnalyze</summary>
    public static void RabbitMQ()
    {
        using (WatsonToneAnalyzer.Connection connection = new WatsonToneAnalyzer.Connection())
        {
            connection.CreateChannel(GetActionMessage);
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine(); // keep the channel open!!!
        }
    }

    public static void GetActionMessage(object source, BasicDeliverEventArgs eventArgs)
    {
        // extract message
        var body = eventArgs.Body;
        var message = Encoding.UTF8.GetString(body);

        // json equivalent to SQLSelectFromActionsToAnalyze
        ActionToAnalyze action = ActionToAnalyze.Factory(message);

        // process message
        Console.WriteLine(" [x] Received {0}", message);

        string username = ConfigurationManager.AppSettings.Get("WatsonUsername");
        string password = ConfigurationManager.AppSettings.Get("WatsonPassword");
        HTTP_POST(action._actionID, action._actionDescription, username, password, (result) => PublishToTable(result, action._ticketID, action._userID, action._organizationID, action._isAgent));

        // acknowledge fully processed so release from MQ
        EventingBasicConsumer consumer = (EventingBasicConsumer)source;
        consumer.Model.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
    }*/
