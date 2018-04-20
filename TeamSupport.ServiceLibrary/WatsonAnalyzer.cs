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

namespace WatsonToneAnalyzer
{
    /// <summary>
    /// Class for the watson service to get the actions to analyze, send
    /// them to Watson, and to insert the results back into the DB
    /// </summary>
    public class WatsonAnalyzer
    {
        static int WatsonUtterancePerAPICall = Int32.Parse(ConfigurationManager.AppSettings.Get("WatsonUtterancePerAPICall"));

        /// <summary>
        /// Get the actions to analyze (dbo.ActionToAnalyze) and post to Watson on the BlueMix account
        /// </summary>
        static public void AnalyzeActions()
        {
            // without this the HTTP message to Watson returns 405 - failure on send
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            // keep a handle to all the async transactions we start
            List<Task> asyncTransactionsInProcess = new List<Task>();

            try
            {
                //opens a sqlconnection at the specified location
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    Table<ActionSentiment> actionSentimentTable = db.GetTable<ActionSentiment>();
                    List<ActionToAnalyze> actionsForAPICall = new List<ActionToAnalyze>();

                    Table<ActionToAnalyze> actionToAnalyzeTable = db.GetTable<ActionToAnalyze>();
                    IQueryable<ActionToAnalyze> actionToAnalyzeQuery = from action in actionToAnalyzeTable select action;
                    foreach (ActionToAnalyze actionToAnalyze in actionToAnalyzeQuery)
                    {
                        // ActionSentiment record already exists?
                        if (actionSentimentTable.Where(u => u.ActionID == actionToAnalyze.ActionID).Any())
                        {
                            actionToAnalyze.DeleteOnSubmit(db);
                            db.SubmitChanges();
                            WatsonEventLog.WriteEntry("duplicate ActionID in ActionSentiment table " + actionToAnalyze.ActionID, EventLogEntryType.Error);
                            continue;
                        }

                        // pack actionsToAnalyze into a single billable API call
                        actionsForAPICall.Add(actionToAnalyze);
                        if (actionsForAPICall.Count >= WatsonUtterancePerAPICall)
                        {
                            // send them all off to watson - async
                            List<ActionToAnalyze> tmp = actionsForAPICall;
                            actionsForAPICall = new List<ActionToAnalyze>(); // new list of 50 (preserves old list for async call)
                            asyncTransactionsInProcess.Add(HTTP_POST(tmp, (result) => PublishToTable(result)));
                        }
                    }

                    // the remainder can wait to be packed into a call when we have more...
                }
            }
            catch (SqlException e1)
            {
                WatsonEventLog.WriteEntry("There was an issues with the sql server:", e1);
                Console.WriteLine(e1.ToString());
            }
            catch (Exception e2)
            {
                WatsonEventLog.WriteEntry("Exception caught at select from ACtionsToAnalyze or HttpPOST:", e2);
                Console.WriteLine(e2.ToString());
            }
            finally
            {
                // wait until all the tone chat messages have been received and recorded
                Task.WaitAll(asyncTransactionsInProcess.ToArray(), 5 * 60 * 1000);  // 5 minute timeout just in case...
            }
        }

        static void PublishToTable(Response response)
        {
            foreach (Utterance utterance in response.WatsonResponse.utterances_tone)
            {
                // missmatch?
                if ((utterance.utterance_id < 0) || (utterance.utterance_id >= response.ActionsToAnalyze.Count))
                {
                    WatsonEventLog.WriteEntry("utterance_id " + utterance.utterance_id + " out of range", EventLogEntryType.Error);
                    continue;
                }

                PublishToTable(response.ActionsToAnalyze[utterance.utterance_id], utterance);
            }
        }

        static int _actionsAnalyzed = 0;
        public static int ActionsAnalyzed { get { return _actionsAnalyzed; } }

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
                WatsonEventLog.WriteEntry("Watson analysis failed - system will retry", e2);
                Console.WriteLine(e2.ToString());
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
                _singleThreadedTransactions.ReleaseMutex();
                ++_actionsAnalyzed;
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
        static async Task HTTP_POST(List<ActionToAnalyze> analyzeList, Action<Response> callback)
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
                    toJson.Add(actionToAnalyze.ActionID.ToString(), actionToAnalyze.WatsonText());  // extract the first 500 characters of raw text
                string jsonString = toJson.ToString();

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

                        //Format response and write to console (should be changed eventually to post to table using sql protocol
                        string result = await content.ReadAsStringAsync() ?? " ";

                        //Create result object to organize response
                        var ResultResponse = new Response();
                        ResultResponse.ActionsToAnalyze = analyzeList;
                        ResultResponse.WatsonResponse = JsonConvert.DeserializeObject<UtteranceToneList>(result);

                        callback(ResultResponse); //returns the response object to pass on to the postSQL class
                    }
                }
            }
            catch (Exception ex)
            {
                WatsonEventLog.WriteEntry("********************: Error durring watson analysis:", ex);
                Console.WriteLine(ex.ToString());
            }
        }
    }

    // send the transaction data back to the action sentiment strategy
    class Response
    {
        public List<ActionToAnalyze> ActionsToAnalyze { get; set; }
        public UtteranceToneList WatsonResponse { get; set; }
    }

    //Creates the deserialize object for Json Returning from Watson
    public class UtteranceToneList
    {
        public List<Utterance> utterances_tone { get; set; }
    }

    public class Utterance
    {
        public int utterance_id;    // index [0, 49] corresponding to the index of the utterance in the request
        public string utterance_text;   // text that was processed
        public List<Tones> tones { get; set; }  // tones detected in text (possibly 0)
    }

    public class Tones
    {
        public float score { get; set; }    // likelihood of this perception
        public String tone_id { get; set; } // sad, frustrated, satisfied, excited, polite, impolite, sympathetic
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
