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
                //if (customerHubID != null) {
                //   GetHubProductsList((int)customerHubID);
                //}

                //Step 1: Check for the CustomerHubID
                //Step 2: Look up the hub 
                //Step 4: Check for the product line and lookup products
                //Step 5: Check the hub setting for product association flag

                //List<int> products = new List<int>();
                //Get product list from the hub to match against and ship with an optional array
                return await FetchDeflectionsAsync(organization, phrase);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
                return "[]";
            }

        }

        public string GetHubDeflectionProductsList(int customerHubID) {
            string result = "";

            //use the customer hub to enforce product line association - for the record I think this is a bad idea
            CustomerHubs hubHelper = new CustomerHubs(LoginUser.Anonymous);
            hubHelper.LoadByCustomerHubID((int)customerHubID);

            CustomerHubFeatureSettings hubFeatureSettingsHelper = new CustomerHubFeatureSettings(LoginUser.Anonymous);
            hubFeatureSettingsHelper.LoadByCustomerHubID(customerHubID);

            if (hubHelper.Any() && hubHelper[0].ProductFamilyID != null && hubFeatureSettingsHelper[0].EnableAnonymousProductAssociation)
            {
                Products productHelper = new Products(LoginUser.Anonymous);
                productHelper.LoadByProductFamilyID((int)hubHelper[0].ProductFamilyID);
            }

            //if (hubHelper.Any() && hubHelper[0].ProductFamilyID != null)
            //{
            //    Products productHelper = new Products(LoginUser.Anonymous);
            //    productHelper.LoadByProductFamilyID((int)hubHelper[0].ProductFamilyID);
            //}

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

            List<DeflectorItem> deflectorIndexList = new List<DeflectorItem>();

            foreach (var Tag in Tags)
            {
                deflectorIndexList.Add(new DeflectorItem
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
            var item = new DeflectorItem
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
            string PingUrl = "http://localhost:64871/api/deflector/check";
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
        public class DeflectorItem {
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

    }

}
