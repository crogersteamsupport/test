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
    public class Deflector : System.Web.Services.WebService {

        [WebMethod]
        public string TestDeflectorAPI(string tag) {
            return CheckDeflectorAPI(tag);
        }

        public string IndexDeflector (string json) {
            string responseText    = null;
            string PingUrl         = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/index";
        	HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
        	request.Method         = "POST";
        	request.KeepAlive      = false;
        	request.ContentType    = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                if (request.HaveResponse && response != null) {
   					using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
   					    responseText = reader.ReadToEnd();
   					}
   				}
   			}
   			return responseText;
   		}

        [WebMethod]
        public string HydrateOrganization (int organizationID) {
            string response = TeamSupport.Data.Deflector.GetOrganizationIndeces(TSAuthentication.GetLoginUser(), organizationID);
            HydrateDeflector(response);
            return response;
        }

        [WebMethod]
        public string HydratePod() {
            List<String> indeceses = TeamSupport.Data.Deflector.GetPodIndeces(TSAuthentication.GetLoginUser());
            foreach (string index in indeceses) {
                try {
                    HydrateDeflector(index);
                } catch (Exception e) {
                }
            }
            return null;
        }

        private string HydrateDeflector (string json) {
            string responseText    = null;
            string PingUrl         = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/index/bulkindex";
        	HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
        	request.Method         = "POST";
            request.Timeout        = 600000;
        	request.KeepAlive      = false;
        	request.ContentType    = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                if (request.HaveResponse && response != null) {
   					using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
   					    responseText = reader.ReadToEnd();
   					}
   				}
   			}
   			return responseText;
   		}

        //private async Task<string> GetDeflectionsAPIAsync(string text) {
        //    string responseText = null;
        //    string PingUrl = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/get/" + text;
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
        //    request.Method = "GET";
        //    request.KeepAlive = false;
        //    request.ContentType = "application/json";

        //    using (WebResponse response = await request.GetResponseAsync()) {
        //        if (request.HaveResponse && response != null)
        //        {
        //            using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
        //            {
        //                return reader.ReadToEnd();
        //            }
        //        }
        //        else {
        //            return "error";
        //        }
        //    }
        //}

        //[WebMethod]
        //public async Task<string> GetDeflections(string text)
        //{
        //    var deflectionResult = await GetDeflectionsAPIAsync(text);

        //    return deflectionResult;
        //}

        public string DeleteDeflector(int organizationID, string value) {
            string responseText = null;
            string PingUrl = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/organization/" + organizationID + "/tag/" + value;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
            request.Method = "DELETE";
            request.KeepAlive = false;
            request.ContentType = "application/json";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                if (request.HaveResponse && response != null) {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
                        responseText = reader.ReadToEnd();
                    }
                }
            }
            return responseText;
        }

        public string RenameTag (int OrganizationId, int TagId, string Value) {
            var item = new DeflectorItem {
                OrganizationID = OrganizationId,
                TagID = TagId,
                Value = Value
            };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(item);

            string responseText    = null;
            string PingUrl         = ConfigurationManager.AppSettings["DeflectorBaseURL"] + OrganizationId + "/tag/rename/" + TagId;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
            request.Method         = "POST";
            request.KeepAlive      = false;
            request.ContentType    = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                if (request.HaveResponse && response != null) {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
                        ResponseText = reader.ReadToEnd();
                    }
                }
            }
            return ResponseText;
        }

        private string CheckDeflectorAPI(string tag) {
            string responseText    = null;
            string PingUrl         = "http://localhost:64871/api/deflector/check";
        	HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
        	request.Method         = "GET";
        	request.KeepAlive      = false;
        	request.ContentType    = "application/json";
        	using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                if (request.HaveResponse && response != null) {
   					using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
   					    responseText = reader.ReadToEnd();
                        // dynamic responseObject = JObject.Parse(responseText);
                        // string success = responseObject.success;
   					}
   				}
   			}
   			return responseText;
   		}

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
