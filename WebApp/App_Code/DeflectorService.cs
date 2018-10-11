using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Runtime.Serialization;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Diagnostics;
using ImageResizer;
using System.Net;
using System.IO;
using System.Dynamic;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Threading.Tasks;
using System.Runtime;

namespace TSWebServices {

    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class DeflectorService : System.Web.Services.WebService {

        [WebMethod]
        public async Task<string> TestDeflectorAPI(string tag) {
            try
            {
                return await TestDeflectorAPIAsync(tag);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
                return null;
            }
        }

        [WebMethod]
        public async Task<string> FetchDeflections(int organization, string phrase, int? customerHubID)
        {
            try
            {
                List<DeflectorReturn> filteredList = new List<DeflectorReturn>();

                //Customer Hub
                if (customerHubID != null)
                {
                    var deflectorMatches = await FetchDeflectionsAsync(organization, phrase);
                    filteredList = GetHubDeflectionResults((int)customerHubID, JsonConvert.DeserializeObject<List<DeflectorReturn>>(deflectorMatches));
                }
                ////Portal
                //else if (!String.IsNullOrEmpty(portalName))
                //{

                //}

                return JsonConvert.SerializeObject(filteredList);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
                return "[]";
            }
        }

        private List<DeflectorReturn> GetHubDeflectionResults(int customerHubID, List<DeflectorReturn> deflectorMatches) {
            string baseHubURL = SystemSettings.GetHubURL();
            List<DeflectorReturn> result = new List<DeflectorReturn>();

            //if we want to open this up to multiple hubs, we can pass the parent organizationID and modify the query to "unlock" the record set
            List<DataRow> whiteListTickets = GetWhiteListHubTicketPaths((int)customerHubID);
            
            result = deflectorMatches.Join(whiteListTickets,
                        deflectorMatch => deflectorMatch.TicketID,
                        whiteListItem => (int)whiteListItem["TicketID"],
                        (deflectorMatch, whiteListItem) => new DeflectorReturn {
                            TicketID = deflectorMatch.TicketID,
                            Name = deflectorMatch.Name,
                            ReturnURL = formatHubKBURL(deflectorMatch.TicketID, (string)whiteListItem["CNameURL"], (string)whiteListItem["PortalName"], baseHubURL)
                        }).ToList();

            //a ranking system can be utilized here if we open this up to multiple hubs

            return result;
        }

        private string formatHubKBURL(int ticketID, string cnameURL, string portalName, string baseHubURL) {
            return "https://" + (!string.IsNullOrEmpty(cnameURL) ? cnameURL : portalName + "." + baseHubURL) + "/knowledgeBase/" + ticketID.ToString();
        }

        private List<DataRow> GetWhiteListHubTicketPaths(int customerHubID) {
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

        public async Task<string> IndexDeflector(string deflectorIndex)
        {
            try
            {
                return await IndexDeflectorAsync(deflectorIndex);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
                return null;
            }
        }

        public async Task<string> IndexTicket(int ticketID)
        {
            Ticket Ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            Tags Tags = new Tags(TSAuthentication.GetLoginUser());
            Tags.LoadByReference(ReferenceType.Tickets, ticketID);
            DeflectorService Deflection = new DeflectorService();

            List<DeflectorIndex> deflectorIndexList = new List<DeflectorIndex>();

            foreach (var Tag in Tags)
            {
                deflectorIndexList.Add(new DeflectorIndex
                {
                    TicketID = Ticket.TicketID,
                    Name = Ticket.Name,
                    OrganizationID = Ticket.OrganizationID,
                    TagID = Tag.TagID,
                    Value = Tag.Value,
                    ProductID = Ticket.ProductID
                });
                
            }

            try
            {
                return await Deflection.BulkIndexDeflectorAsync(Newtonsoft.Json.JsonConvert.SerializeObject(deflectorIndexList));
            }
            catch (Exception ex){
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
                return null;
            }
        }

        [WebMethod]
        public async Task<string> HydrateOrganization(int organizationID)
        {
            string response = TeamSupport.Data.Deflector.GetOrganizationIndeces(TSAuthentication.GetLoginUser(), organizationID);

            try
            {
                return await BulkIndexDeflectorAsync(response);
            }
            catch (Exception ex) {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
                return null;
            }
        }

        [WebMethod]
        public async Task<string> HydratePod()
        {
            List<String> indeceses = TeamSupport.Data.Deflector.GetPodIndeces(TSAuthentication.GetLoginUser());
            foreach (string index in indeceses)
            {
                try
                {
                    await BulkIndexDeflectorAsync(index);
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
                }
            }

            return null;
        }

        public async Task<string> DeleteTicket(int ticketID)
        {

            try
            {
                return await DeleteTicketAsync(ticketID);
            }
            catch (Exception ex) {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
            }
            return null;
        }

        public async Task<string> DeleteTag(int organizationID, string value)
        {

            try
            {
                return await DeleteTagAsync(organizationID, value);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
            }
            return null;
        }

        private async Task<string> IndexDeflectorAsync(string deflectionIndex) {
            string ResponseText    = null;
            string PingUrl         = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/index/index";
        	HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
        	request.Method         = "POST";
        	request.KeepAlive      = false;
        	request.ContentType    = "application/json";

            try
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(deflectionIndex);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                using (WebResponse response = request.GetResponse())
                {
                    if (request.HaveResponse && response != null)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
                        {
                            ResponseText = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex) {

            }

               return ResponseText;
   		}

        private async Task<string> BulkIndexDeflectorAsync(string json) {
            string ResponseText    = null;
            string PingUrl         = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/index/bulkindex";
        	HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
        	request.Method         = "POST";
            request.Timeout        = 800000;
        	request.KeepAlive      = false;
        	request.ContentType    = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            using (WebResponse response = request.GetResponse()) {
                if (request.HaveResponse && response != null) {
   					using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
   					    ResponseText = reader.ReadToEnd();
   					}
   				}
   			}
   			return ResponseText;
   		}

        private async Task<string> FetchDeflectionsAsync(int organization, string phrase) {
            string PingUrl = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/fetch/" + organization + "/" + phrase;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
            request.Method = "GET";
            request.KeepAlive = false;
            request.ContentType = "application/json";

            using (WebResponse response = request.GetResponse())
            {
                if (request.HaveResponse && response != null)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
                else
                {
                    return "error";
                }
            }
        }

        public async Task<string> DeleteTicketAsync(int ticketID) {
            string ResponseText = null;
            string PingUrl = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/delete/ticket/" + ticketID;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
            request.Method = "DELETE";
            request.KeepAlive = false;
            request.ContentType = "application/json";
            using (WebResponse response = request.GetResponse()) {
                if (request.HaveResponse && response != null) {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
                        ResponseText = reader.ReadToEnd();
                    }
                }
            }
            return ResponseText;
        }

        public async Task<string> DeleteTagAsync(int organizationID, string value)
        {
            var item = new DeflectorIndex
            {
                OrganizationID = organizationID,
                Value = value
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
            string ResponseText = null;
            string PingUrl = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/delete/organization/" + organizationID + "/tag/" + value;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
            request.Method = "DELETE";
            request.KeepAlive = false;
            request.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            try
            {
                using (WebResponse response = await request.GetResponseAsync())
                {
                    if (request.HaveResponse && response != null)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
                        {
                            ResponseText = reader.ReadToEnd();
                        }
                    }
                }
                return ResponseText;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        //private async Task<string> RenameTag(string jsonUpdate) {
        //    string ResponseText    = null;
        //    string PingUrl         = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/update/organization/" + OrganizationId + "/tag/" + TagId + "/rename";

        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
        //    request.Method         = "POST";
        //    request.KeepAlive      = false;
        //    request.ContentType    = "application/json";

        //    using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
        //        streamWriter.Write(jsonUpdate);
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //    }

        //    using (WebResponse response = request.GetResponse()) {
        //        if (request.HaveResponse && response != null) {
        //            using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
        //                ResponseText = reader.ReadToEnd();
        //            }
        //        }
        //    }
        //    return ResponseText;
        //}

        private async Task<string> TestDeflectorAPIAsync(string tag)
        {
            string ResponseText = null;
            string PingUrl = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/deflector/check";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
            request.Method = "GET";
            request.KeepAlive = false;
            request.ContentType = "application/json";
            using (WebResponse response = request.GetResponse())
            {
                if (request.HaveResponse && response != null)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
                    {
                        ResponseText = reader.ReadToEnd();
                        // dynamic responseObject = JObject.Parse(ResponseText);
                        // string success = responseObject.success;
                    }
                }
            }
            return ResponseText;
        }

        //private async Task<string> MergeTag (int OrganizationId, int TagId, string Value) {
        //    var item = new DeflectorItem {
        //        OrganizationID = OrganizationId,
        //        TagID = TagId,
        //        Value = Value
        //    };
        //    string json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
        //    string ResponseText    = null;
        //    string PingUrl         = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/update/organization/" + OrganizationId + "/tag/" + TagId + "/merge";

        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
        //    request.Method         = "POST";
        //    request.KeepAlive      = false;
        //    request.ContentType    = "application/json";

        //    using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
        //        streamWriter.Write(json);
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //    }

        //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
        //        if (request.HaveResponse && response != null) {
        //            using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
        //                ResponseText = reader.ReadToEnd();
        //            }
        //        }
        //    }
        //    return ResponseText;
        //}

        [DataContract]
        public class DeflectorIndex {
            [DataMember]
            public int TicketID { get; set; }
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public int OrganizationID { get; set; }
            [DataMember]
            public int? ProductID { get; set; }
            [DataMember]
            public int TagID { get; set; }
            [DataMember]
            public string Value { get; set; }
        }

        [DataContract]
        public class DeflectorReturn
        {
            [DataMember]
            public int TicketID { get; set; }
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public string ReturnURL { get; set; }
        }
    }

}
