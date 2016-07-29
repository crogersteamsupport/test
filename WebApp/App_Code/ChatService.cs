using System.Web.Services;
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
        public bool CheckChatStatus(string chatGuid)
        {
            Organization org = GetOrganization(chatGuid);
            bool isAvailable = ChatRequests.IsOperatorAvailable(LoginUser.Anonymous, org.OrganizationID);
            return isAvailable;
        }

        [WebMethod]
        public string GetChatInfo(int chatID)
        {
            ChatViewObject model = new ChatViewObject();
            model.Chat = GetChatRequest(chatID).GetProxy();
            model.Initiator = GetParticipant(model.Chat.RequestorID).GetProxy();
            return JsonConvert.SerializeObject(model);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string RequestChat(string chatGuid, string fName, string lName, string email, string description)
        {
            Organization org = GetOrganization(chatGuid);
            ChatRequest request = ChatRequests.RequestChat(LoginUser.Anonymous, org.OrganizationID, fName, lName, email, description, Context.Request.UserHostAddress); 
            pusher.Trigger("chat-requests", "new-chat-request", new { message = string.Format("{0} {1} is requesting a chat!", fName, lName) });
            return JsonConvert.SerializeObject(request.GetProxy());
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void Auth(string channel_name, string socket_id, int chatId, int participantID)
        {
            ChatClient client = ChatClients.GetChatClient(loginUser, participantID);

            var channelData = new PresenceChannelData()
            {
                user_id = client.ChatClientID.ToString(),
                user_info = new
                {
                    name = client.FirstName +  ' ' + client.LastName,
                    company = client.CompanyName
                }

            };

            var auth = pusher.Authenticate(channel_name, socket_id, channelData);
            var json = auth.ToJson();
            Context.Response.Write(json);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddMessage(string channelName, string message, int chatID, int userID)
        {
            Chat chat = GetChat(chatID);
            ChatClient client = ChatClients.GetChatClient(loginUser, userID);


            var result = pusher.Trigger(channelName, "new-comment", new { message = message, userName = client.FirstName + "  " +  client.LastName, userID = client.LinkedUserID });
            return JsonConvert.SerializeObject(true);
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

        private Organization GetOrganization(string orgGuid)
        {
            Organizations organizations = new Organizations(LoginUser.Anonymous);
            organizations.LoadByChatID(new Guid(orgGuid));

            if (organizations.IsEmpty) return null;
            else return organizations[0];
        }

        private Chat GetChat(int chatID)
        {
            Chats chats = new Chats(loginUser);
            chats.LoadByChatID(chatID);

            if (!chats.IsEmpty) return chats[0];
            else return null;
        }

        private ChatClient GetParticipant(int participantID)
        {
            return ChatClients.GetChatClient(loginUser, participantID);
        }

        private ChatRequest GetChatRequest(int chatID)
        {
            ChatRequests requests = new ChatRequests(loginUser);
            requests.LoadByChatID(chatID, ChatRequestType.External);
            return requests[0];
        }

        public class ChatViewObject
        {
            public ChatRequestProxy Chat { get; set; }
            public ChatClientProxy Initiator { get; set; }
        }
    }
}