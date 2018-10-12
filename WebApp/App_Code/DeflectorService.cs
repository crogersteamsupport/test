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
                DeflectorAPI deflectorAPI = new DeflectorAPI();
                return await deflectorAPI.TestDeflectorAPIAsync(tag);
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
                    DeflectorAPI deflectorAPI = new DeflectorAPI();
                    var deflectorMatches = await deflectorAPI.FetchDeflectionsAsync(organization, phrase);
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

        public async Task<string> IndexDeflector(string deflectorIndex)
        {
            try
            {
                DeflectorAPI deflectorAPI = new DeflectorAPI();
                return await deflectorAPI.IndexDeflectorAsync(deflectorIndex);
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
                DeflectorAPI deflectorAPI = new DeflectorAPI();
                return await deflectorAPI.BulkIndexDeflectorAsync(Newtonsoft.Json.JsonConvert.SerializeObject(deflectorIndexList)).ConfigureAwait(false);
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
                DeflectorAPI deflectorAPI = new DeflectorAPI();
                return await deflectorAPI.BulkIndexDeflectorAsync(response);
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
                    DeflectorAPI deflectorAPI = new DeflectorAPI();
                    await deflectorAPI.BulkIndexDeflectorAsync(index);
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
                DeflectorAPI deflectorAPI = new DeflectorAPI();
                await deflectorAPI.DeleteTicketAsync(ticketID);
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
                DeflectorAPI deflectorAPI = new DeflectorAPI();
                await deflectorAPI.DeleteTagAsync(organizationID, value);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
            }
            return null;
        }

        private List<DeflectorReturn> GetHubDeflectionResults(int customerHubID, List<DeflectorReturn> deflectorMatches)
        {

            List<DeflectorReturn> result = new List<DeflectorReturn>();
            string baseHubURL = SystemSettings.GetHubURL();

            //if we want to open this up to multiple hubs, we can pass the parent organizationID and modify the query to "unlock" the record set
            List<DataRow> whiteListTickets = Deflector.GetWhiteListHubTicketPaths((int)customerHubID);

            result = deflectorMatches.Join(whiteListTickets,
                        deflectorMatch => deflectorMatch.TicketID,
                        whiteListItem => (int)whiteListItem["TicketID"],
                        (deflectorMatch, whiteListItem) => new DeflectorReturn
                        {
                            TicketID = deflectorMatch.TicketID,
                            Name = deflectorMatch.Name,
                            ReturnURL = formatHubKBURL(deflectorMatch.TicketID, (string)whiteListItem["CNameURL"], (string)whiteListItem["PortalName"], baseHubURL)
                        }).ToList();

            //an additional step with a ranking system can be utilized here if we open this up to multiple hubs

            return result;
        }

        private string formatHubKBURL(int ticketID, string cnameURL, string portalName, string baseHubURL)
        {
            return "https://" + (!string.IsNullOrEmpty(cnameURL) ? cnameURL : portalName + "." + baseHubURL) + "/knowledgeBase/" + ticketID.ToString();
        }

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
