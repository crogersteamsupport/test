using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Configuration;
using System.Web.Configuration;
using System.Web;
using System.Security.Authentication;

namespace TeamSupport.Proxy
{
    public class AuthenticationModel
    {
        public FormsAuthenticationTicket AuthenticationTicket { get; private set; }
        public int UserID { get; private set; }
        public int OrganizationID { get; private set; }
        public bool IsBackdoor { get; private set; }
        public bool IsSystemAdmin { get; private set; }
        public string SessionID { get; private set; }
        public string ConnectionString { get; private set; }

        public AuthenticationModel()
        {
            // Authentication from HttpContext
            if ((HttpContext.Current.User == null) || !(HttpContext.Current.User.Identity is FormsIdentity))
                throw new AuthenticationException("Authentication error - No user identity");
            AuthenticationTicket = (HttpContext.Current.User.Identity as FormsIdentity).Ticket;

            // Extract custom user data
            string[] data = AuthenticationTicket.UserData.Split('|');
            UserID = int.Parse(data[0]);
            OrganizationID = int.Parse(data[1]);
            IsBackdoor = (data[2] == "1");
            SessionID = data[3];
            IsSystemAdmin = (data[4] == "1");

            // Connection string
            ConnectionStringSettings cStrings = WebConfigurationManager.ConnectionStrings["MainConnection"];
            ConnectionString = (cStrings != null) ? cStrings.ConnectionString : ConfigurationManager.AppSettings["ConnectionString"];
        }

    }
}
