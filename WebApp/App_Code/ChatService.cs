using System.Web.Services;
using System.Web.Script.Services;
using System.Data;
using PusherServer;
using Newtonsoft.Json;
using TeamSupport.Data;
using TeamSupport.WebUtils;

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
        public ChatService() {
            options.Encrypted = true;
            pusher = new Pusher("223753", "0cc6bf2df4f20b16ba4d", "119f91ed19272f096383", options);
            loginUser = TSAuthentication.GetLoginUser();
        }

        [WebMethod]
        public void AddMessage(string message, string channelName)
        {

            var result = pusher.Trigger(channelName, "NewMessage", new { message = message });
        }

        [WebMethod]
        public string Auth(string channel_name, string socket_id)
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
            var returnObject = new { Content = json, ContentType = "application/json" };

            return JsonConvert.SerializeObject(returnObject, Formatting.Indented);
        }
    }
}