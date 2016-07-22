﻿using System.Web.Services;
using System.Web.Script.Services;
using System.Data;
using PusherServer;
using Newtonsoft.Json;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Web.Script.Serialization;
using System;
using System.Web;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ChatService : System.Web.Services.WebService
    {
        PusherOptions options = new PusherOptions();
        Pusher pusher;
        LoginUser loginUser;
        //Organization parentOrganization;
        //int parentOrganizationID;
        public ChatService() {
            options.Encrypted = true;
            pusher = new Pusher("223753", "0cc6bf2df4f20b16ba4d", "119f91ed19272f096383", options);
            loginUser = TSAuthentication.GetLoginUser();
            //parentOrganization = GetOrganization();
            //parentOrganizationID = (parentOrganization != null) ? parentOrganization.OrganizationID : 0;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddMessage(string channelName, string message)
        {

            var result = pusher.Trigger(channelName, "new-comment", new { message = message, userName = loginUser.GetUserFullName() });
            return true.ToString();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetContact(string chatGuid, string fName, string lName, string email)
        {
            Organization org = GetOrganization(chatGuid);
            Users users = new Users(loginUser);
            users.LoadByEmail(org.OrganizationID, email);

            if (users.IsEmpty) return null;
            else return JsonConvert.SerializeObject(users[0].GetProxy());
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void Auth(string channel_name, string socket_id)
        {
            var channelData = new PresenceChannelData()
            {
                user_id = loginUser.UserID.ToString(),
                user_info = new
                {
                    name = loginUser.GetUserFullName()
                }

            };

            var auth = pusher.Authenticate(channel_name, socket_id, channelData);
            var json = auth.ToJson();
            Context.Response.Write(json);
        }

        private Organization GetOrganization(string orgGuid)
        {
            Organizations organizations = new Organizations(LoginUser.Anonymous);
            organizations.LoadByChatID(new Guid(orgGuid));

            if (organizations.IsEmpty) return null;
            else return organizations[0];
        }
    }
}