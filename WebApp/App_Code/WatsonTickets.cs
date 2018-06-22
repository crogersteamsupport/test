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
    public class WatsonTickets : System.Web.Services.WebService {

        public WatsonTickets() { }

        [WebMethod]
        public string WatsonTest (int ticketID) {
            return "negative";
        }

        [WebMethod]
        public string WatsonTicket (int ticketID) {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            string json = Actions.WatsonPullTicket(loginUser, ticketID);
            if (json != "nothing" && json != "negative") {
                return json;
            } else {
                return "negative";
            }
        }

        [WebMethod]
        public string WatsonAction (int ticketID, int actionID) {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            string json = Actions.WatsonPullAction(loginUser, ticketID, actionID);
            if (json != "nothing" && json != "negative") {
                return json;
            } else {
                return "negative";
            }
        }

        [WebMethod]
        public string PullReactions(int ticketID, int actionID) {
            TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User author = Users.GetUser(loginUser, action.CreatorID);
            if (author != null) {
                if (loginUser.OrganizationID == author.OrganizationID) {
                    string json1 = Actions.CountReactions(loginUser, ticketID, actionID);
                    string json2 = Actions.CheckReaction(loginUser, ticketID, actionID);
                    if (json1 == "negative" || json2 == "negative") {
                        return "negative";
                    } else if (json1 == "nothing" && json2 == "nothing") {
                        return "nothing";
                    } else if (json1 != "nothing" && json2 != "nothing") {
                        return string.Format("[{0},{1}]", json1, json2);
                    } else if (json1 != "nothing") {
                        return json1;
                    } else if (json2 != "nothing") {
                        return json2;
                    } else {
                        return "negative";
                    }
                } else {
                    return "hidden";
                }
            } else {
                return "hidden";
            }
        }

        [WebMethod]
        public string PullUserList(int ticketID) {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            return Actions.PullUserList(loginUser, ticketID);
        }

        [WebMethod]
        public string ListReactions(int ticketID, int actionID) {
            TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User user = TSAuthentication.GetUser(loginUser);
            User author = Users.GetUser(loginUser, action.CreatorID);
            return Actions.ListReactions(loginUser, ticketID, actionID);
        }

        [WebMethod]
        public string UpdateReaction(int ticketID, int actionID, int value) {
            string updateReaction = string.Empty;
            TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            int receiverID = Convert.ToInt32(action.CreatorID);

            updateReaction = Actions.UpdateReaction(loginUser, receiverID, ticketID, actionID, value);

            if (updateReaction == "positive" && value > 0 && action.CreatorID != loginUser.UserID) {
                // EmailReaction(loginUser, receiverID, ticketID);
            }
            return updateReaction;
        }



    }
}
