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
        Organization parentOrganization;
        public ChatService() {
            options.Encrypted = true;
            pusher = new Pusher("223753", "0cc6bf2df4f20b16ba4d", "119f91ed19272f096383", options);
            loginUser = TSAuthentication.GetLoginUser();
        }

        #region ClientServices

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
                    name = client.FirstName + ' ' + client.LastName,
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

        #endregion

        #region AgentServices

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetChats()
        {
            ChatRequests requests = new ChatRequests(loginUser);
            requests.LoadWaitingRequests(loginUser.UserID, loginUser.OrganizationID);
            return JsonConvert.SerializeObject(requests.GetChatRequestProxies());
        }

        [WebMethod]
        public int AcceptRequest(int chatRequestID)
        {
            int chatID = ChatRequests.AcceptRequest(loginUser, loginUser.UserID, chatRequestID, HttpContext.Current.Request.UserHostAddress);
            return chatID;
        }

        [WebMethod]
        public int AddTicket(int chatID, int ticketID)
        {
            Chat chat = Chats.GetChat(loginUser, chatID);
            if (chat != null)
            {
                Actions actions = new Actions(loginUser);
                TeamSupport.Data.Action chatAction = actions.AddNewAction();
                chatAction.ActionTypeID = null;
                chatAction.Name = "Chat";
                chatAction.ActionSource = "Chat";
                chatAction.SystemActionTypeID = SystemActionType.Chat;
                chatAction.Description = chat.GetHtml(true, loginUser.OrganizationCulture);
                chatAction.IsVisibleOnPortal = false;
                chatAction.IsKnowledgeBase = false;
                chatAction.TicketID = ticketID;
                actions.Save();
                chat.ActionID = chatAction.ActionID;
                chat.Collection.Save();

                (new Tickets(loginUser).AddContact(chat.GetInitiatorLinkedUserID(), ticketID);
            }
            return ticketID;
        }

        [WebMethod]
        public void RequestTransfer(int chatID, int userID)
        {
            ChatRequests.RequestTransfer(loginUser, chatID, userID);
        }

        [WebMethod]
        public void RequestInvite(int chatID, int userID)
        {
            ChatRequests.RequestInvite(loginUser, chatID, userID);
        }

        [WebMethod]
        public void PostMessage(string message, int chatID)
        {
            Chat chat = Chats.GetChat(loginUser, chatID);
            if (chat.OrganizationID != loginUser.OrganizationID) return;
            ChatMessage chatMessage = (new ChatMessages(loginUser)).AddNewChatMessage();
            chatMessage.Message = message;
            chatMessage.ChatID = chatID;
            chatMessage.PosterID = loginUser.UserID;
            chatMessage.PosterType = ChatParticipantType.User;
            chatMessage.Collection.Save();
            Users.UpdateUserActivityTime(loginUser, loginUser.UserID);
        }

        [WebMethod]
        public bool CloseChat(int chatID)
        {
            Chat chat = Chats.GetChat(loginUser, chatID);
            if (chat.OrganizationID != loginUser.OrganizationID) return false;
            Chats.LeaveChat(loginUser, loginUser.UserID, ChatParticipantType.User, chatID);
            return true;
        }

        [WebMethod]
        public bool ToggleAvailable()
        {
            ChatUserSetting setting = ChatUserSettings.GetSetting(loginUser, loginUser.UserID);
            setting.IsAvailable = !setting.IsAvailable;
            setting.Collection.Save();
            return setting.IsAvailable;
        }

        #endregion

        #region PrivateMethods

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

        #endregion

        #region Classes

        public class ChatViewObject
        {
            public ChatRequestProxy Chat { get; set; }
            public ChatClientProxy Initiator { get; set; }
        }
        #endregion
    }
}