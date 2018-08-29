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
        string[] _userData;

        //public FormsAuthenticationTicket AuthenticationTicket { get; private set; }
        public int UserID { get { return int.Parse(_userData[0]); } }
        public int OrganizationID { get { return int.Parse(_userData[1]); } }
        public bool IsBackdoor { get { return (_userData[2] == "1"); } }
        public bool IsSystemAdmin { get { return (_userData[4] == "1"); } }
        public string SessionID { get { return _userData[3]; } }
        public string ConnectionString { get; private set; }

        public AuthenticationModel()
        {
            // Authentication from HttpContext
            if ((HttpContext.Current.User == null) || !(HttpContext.Current.User.Identity is FormsIdentity))
                throw new AuthenticationException("Authentication error - No user identity");
            FormsAuthenticationTicket AuthenticationTicket = (HttpContext.Current.User.Identity as FormsIdentity).Ticket;

            // Extract custom user data
            _userData = AuthenticationTicket.UserData.Split('|');

            // Connection string
            ConnectionStringSettings cStrings = WebConfigurationManager.ConnectionStrings["MainConnection"];
            ConnectionString = (cStrings != null) ? cStrings.ConnectionString : ConfigurationManager.AppSettings["ConnectionString"];
        }

    }
}
