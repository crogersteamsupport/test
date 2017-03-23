using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;

namespace TeamSupport.Data
{
    public partial class Chat
    {
        public string GetHtml(bool includeTimeStamps, CultureInfo culture)
        {
            ChatParticipant initiator = ChatParticipants.GetChatParticipant(Collection.LoginUser, InitiatorID, InitiatorType, ChatID);
            ChatMessages messages = new ChatMessages(Collection.LoginUser);
            StringBuilder builder = new StringBuilder("<div class=\"chat-messages\">");
            builder.Append(string.Format("<p class=\"chat-notification\">Initiated On: {0}<p>", DateCreated.ToString("g", culture)));
            if (initiator != null)
            {
                builder.Append(string.Format("<p class=\"chat-notification\">Initiated By: {0} {1}, {2} (<a href=\"mailto:{3}\">{3}</a>)<p>", initiator.FirstName, initiator.LastName, initiator.CompanyName, initiator.Email));
            }

            messages.LoadByChatID(ChatID);
            foreach (ChatMessage message in messages)
            {
                string time = message.DateCreated.ToString("h:mm");
                string timeFormat = includeTimeStamps ? time + ": " : "";

                if (message.IsNotification)
                    builder.Append(string.Format("<p class=\"chat-notification\">{0}{1}<p>", includeTimeStamps ? time + ": " : "", message.Message));
                else if (message.PosterType == ChatParticipantType.User)
                {
                    if (message.PosterID == Collection.LoginUser.UserID)
                        builder.Append(string.Format("<p class=\"chat-message-self\"><span class=\"chat-name\">{1}{0}: </span>{2}</p>", message.Row["PosterName"].ToString(), timeFormat, message.Message));
                    else
                        builder.Append(string.Format("<p class=\"chat-message-user\"><span class=\"chat-name\">{1}{0}: </span>{2}</p>", message.Row["PosterName"].ToString(), timeFormat, message.Message));
                }
                else
                    builder.Append(string.Format("<p class=\"chat-message-client\"><span class=\"chat-name\">{1}{0}: </span>{2}</p>", message.Row["PosterName"].ToString(), timeFormat, message.Message));
            }
            builder.Append("</div>");
            return builder.ToString();
        }

        public int GetInitiatorLinkedUserID()
        {
            int? userID = null;
            if (InitiatorType == ChatParticipantType.External)
            {
                ChatClients clients = new ChatClients(Collection.LoginUser);
                clients.LoadByChatClientID(InitiatorID);
                if (!clients.IsEmpty) userID = clients[0].LinkedUserID;
            }

            return userID == null ? -1 : (int)userID;
        }
    }

    public partial class Chats
    {
        public static void UpdateAction(LoginUser loginUser, int chatID)
        {
            Chat chat = Chats.GetChat(loginUser, chatID);
            if (chat == null || chat.ActionID == null) return;

            Action action = Actions.GetAction(loginUser, (int)chat.ActionID);
            if (action == null) return;

            action.Description = chat.GetHtml(true, loginUser.OrganizationCulture);
            action.Collection.Save();
        }

        public static void JoinChat(LoginUser loginUser, int id, ChatParticipantType type, int chatID, string ipAddress)
        {
            ChatMessages messages = new ChatMessages(loginUser);
            ChatParticipant part = (new ChatParticipants(loginUser)).AddNewChatParticipant();
            part.ChatID = chatID;
            part.DateJoined = DateTime.UtcNow;
            part.ParticipantID = id;
            part.ParticipantType = type;
            part.IPAddress = ipAddress;
            part.LastTyped = DateTime.UtcNow.AddYears(-10);
            part.LastMessageID = messages.GetLastMessageID(chatID);
            part.Collection.Save();

            part = ChatParticipants.GetChatParticipant(loginUser, id, type, chatID);
            if (part == null) return;

            AddNotification(loginUser, chatID, id, type, string.Format("{0} {1} has joined the chat.", part.FirstName, part.LastName));
        }

        public static ChatMessageProxy LeaveChat(LoginUser loginUser, int id, ChatParticipantType type, int chatID)
        {
            Chat chat = Chats.GetChat(loginUser, chatID);
            ChatParticipant self = ChatParticipants.GetChatParticipant(loginUser, id, type, chatID);
            if (self == null) return null;

            self.DateLeft = DateTime.UtcNow;
            self.Collection.Save();

            ChatMessageProxy message = AddNotification(loginUser, chatID, id, type, string.Format("{0} {1} has left the chat.", self.FirstName, self.LastName));

            if (self.ParticipantType != ChatParticipantType.User) return message;

            ChatParticipants participants = new ChatParticipants(loginUser);
            participants.LoadByChatID(chatID);

            bool allUsersGone = true;
            foreach (ChatParticipant item in participants)
            {
                if (item.DateLeft == null && item.ParticipantType == ChatParticipantType.User)
                {
                    allUsersGone = false;
                    break;
                }
            }

            if (allUsersGone)
            {
                foreach (ChatParticipant item in participants)
                {
                    if (item.DateLeft == null)
                    {
                        LeaveChat(loginUser, item.ParticipantID, item.ParticipantType, chatID);
                    }
                }
            }

            return message;

        }

        public static ChatMessageProxy AddNotification(LoginUser loginUser, int chatID, int id, ChatParticipantType type, string message)
        {
            ChatMessage chatMessage = (new ChatMessages(loginUser)).AddNewChatMessage();
            chatMessage.ChatID = chatID;
            chatMessage.Message = message;
            chatMessage.IsNotification = true;
            chatMessage.PosterID = id;
            chatMessage.PosterType = type;
            chatMessage.Collection.Save();
            return chatMessage.GetProxy();
        }

        public static void KickOutDisconnectedClients(LoginUser loginUser, int organizationID)
        {
            ChatParticipants participants = new ChatParticipants(loginUser);
            participants.LoadExternalDisconnected(organizationID);
            foreach (ChatParticipant participant in participants)
            {
                participant.DateLeft = DateTime.UtcNow;
                participant.Collection.Save();
                AddNotification(loginUser, participant.ChatID, participant.ParticipantID, participant.ParticipantType, string.Format("{0} {1} has left the chat.", participant.FirstName, participant.LastName));
            }

        }

        public void LoadByUserID(int userID, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
SELECT DISTINCT c.* FROM Chats c 
LEFT JOIN ChatParticipants cp ON cp.ChatID = c.ChatID
WHERE cp.ParticipantID = @UserID
AND cp.ParticipantType = 0
AND c.OrganizationID = @OrganizationID
AND cp.DateLeft IS NULL
";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

    }

}
