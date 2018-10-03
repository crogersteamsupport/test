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
            string ResponseText    = null;
            string PingUrl         = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/index/index";
        	HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
        	request.Method         = "POST";
        	request.KeepAlive      = false;
        	request.ContentType    = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            try {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    if (request.HaveResponse && response != null) {
   		    			using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
   			    		    ResponseText = reader.ReadToEnd();
   				    	}
   				    }
   			    }
      			return ResponseText;
            } catch (Exception e) {
                return e.Message;
            }
   		}

        [WebMethod]
        public string HydrateOrganization (int organizationID) {
            string response = TeamSupport.Data.Deflector.GetOrganizationIndeces(TSAuthentication.GetLoginUser(), organizationID);
            try {
                HydrateDeflector(response);
                return response;
            } catch (Exception e) {
                return e.Message;
            }
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
            string ResponseText    = null;
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

            try {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    if (request.HaveResponse && response != null) {
   		      			using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
   			    		    ResponseText = reader.ReadToEnd();
   				    	}
   				    }
   			    }
                return ResponseText;
            } catch (Exception e) {
                return e.Message;
            }
   		}

        private async Task<string> GetDeflectionsAPIAsync(int organization, string phrase) {
            string responseText = null;
            string PingUrl = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/fetch/" + organization + "/" + phrase;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
            request.Method = "GET";
            request.KeepAlive = false;
            request.ContentType = "application/json";
            try {
                using (WebResponse response = await request.GetResponseAsync()) {
                    if (request.HaveResponse && response != null) {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
                            return reader.ReadToEnd();
                        }
                    } else {
                        return "error";
                    }
                }
            } catch (Exception e) {
                return e.Message;
            }
        }

        [WebMethod]
        public async Task<string> GetDeflections(int organization, string phrase) {
            try {
                return await GetDeflectionsAPIAsync(organization, phrase);
            } catch (Exception ex) {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
                return null;
            }

        }

        public string DeleteDeflector(int organizationID, string value) {
            string PingUrl = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/organization/" + organizationID + "/tag/" + value;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
            request.Method = "DELETE";
            request.KeepAlive = false;
            request.ContentType = "application/json";
            try {
                string ResponseText = null;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    if (request.HaveResponse && response != null) {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
                            ResponseText = reader.ReadToEnd();
                        }
                    }
                }
                return ResponseText;
            } catch (Exception e) {
                return e.Message;
            }

        }

        public string RenameTag (int OrganizationId, int TagId, string Value) {
            var item = new DeflectorItem {
                OrganizationID = OrganizationId,
                TagID = TagId,
                Value = Value
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
            string ResponseText    = null;
            string PingUrl         = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/update/organization/" + OrganizationId + "/tag/" + TagId + "/rename";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
            request.Method         = "POST";
            request.KeepAlive      = false;
            request.ContentType    = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            try {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    if (request.HaveResponse && response != null) {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
                            ResponseText = reader.ReadToEnd();
                        }
                    }
                }
                return ResponseText;
            } catch (Exception e) {
                return e.Message;
            }
        }

        public string MergeTag (int OrganizationId, int TagId, string Value) {
            var item = new DeflectorItem {
                OrganizationID = OrganizationId,
                TagID = TagId,
                Value = Value
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
            string ResponseText    = null;
            string PingUrl         = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/update/organization/" + OrganizationId + "/tag/" + TagId + "/merge";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
            request.Method         = "POST";
            request.KeepAlive      = false;
            request.ContentType    = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            try {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    if (request.HaveResponse && response != null) {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
                            ResponseText = reader.ReadToEnd();
                        }
                    }
                }
                return ResponseText;
            } catch (Exception e) {
                return e.Message;
            }
        }

        public string DeleteTag (int OrganizationId, string Value) {
            var item = new DeflectorItem {
                OrganizationID = OrganizationId,
                Value = Value
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
            string ResponseText    = null;
            string PingUrl         = ConfigurationManager.AppSettings["DeflectorBaseURL"] + "/delete/organization/" + OrganizationId + "/tag/" + Value;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
            request.Method         = "DELETE";
            request.KeepAlive      = false;
            request.ContentType    = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            try {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    if (request.HaveResponse && response != null) {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
                            ResponseText = reader.ReadToEnd();
                        }
                    }
                }
                return ResponseText;
            } catch (Exception e) {
                return e.Message;
            }
        }

        private string CheckDeflectorAPI(string tag) {
            string ResponseText    = null;
            string PingUrl         = "http://localhost:64871/api/deflector/check";
        	HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
        	request.Method         = "GET";
        	request.KeepAlive      = false;
        	request.ContentType    = "application/json";
            try {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    if (request.HaveResponse && response != null) {
   					    using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
   					        ResponseText = reader.ReadToEnd();
   					    }
      				}
     			}
   	    		return ResponseText;
            } catch (Exception e) {
                return e.Message;
            }
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
