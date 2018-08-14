using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Configuration;
using System.Web.Configuration;

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

        public AuthenticationModel(FormsAuthenticationTicket authentication)
        {
            AuthenticationTicket = authentication;
            string[] data = authentication.UserData.Split('|');
            UserID = int.Parse(data[0]);
            OrganizationID = int.Parse(data[1]);
            IsBackdoor = (data.Length < 3) ? false : data[2] == "1";
            IsSystemAdmin = data[4] == "1";
            SessionID = (data.Length < 4) ? "" : data[3];

            ConnectionStringSettings cStrings = WebConfigurationManager.ConnectionStrings["MainConnection"];
            ConnectionString = (cStrings != null) ? cStrings.ConnectionString : ConfigurationManager.AppSettings["ConnectionString"];
        }
    }
}
