using System.Web.Services;
using System.Web.Script.Services;
using System.Data;
using PusherServer;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ChatService : System.Web.Services.WebService
    {

        public ChatService() { }

        [WebMethod]
        public void AddMessage(string message)
        {
            var options = new PusherOptions();
            options.Encrypted = true;

            var pusher = new Pusher("223753", "0cc6bf2df4f20b16ba4d", "119f91ed19272f096383", options);

            var result = pusher.Trigger("test_channel", "my_event", new { message = "hello world" });
        }
    }
}