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
            string PingUrl         = "http://localhost:64871/api/deflector/index";
        	HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
        	request.Method         = "POST";
        	request.KeepAlive      = false;
        	request.ContentType    = "application/json";
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
            var results = new List<string>();
            string response = TeamSupport.Data.Deflector.PullOrganization(TSAuthentication.GetLoginUser(), organizationID);
            // string output = JsonConvert.SerializeObject(response);
            return response;
        }

        private string HydrateDeflector (string json) {
            string responseText    = null;
            string PingUrl         = "http://localhost:64871/api/deflector/bulkindex";
        	HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PingUrl);
        	request.Method         = "POST";
        	request.KeepAlive      = false;
        	request.ContentType    = "application/json";
        	using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                if (request.HaveResponse && response != null) {
   					using (StreamReader reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8)) {
   					    responseText = reader.ReadToEnd();
   					}
   				}
   			}
   			return responseText;
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

    }

}
