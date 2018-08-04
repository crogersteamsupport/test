using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Configuration;
using System.Web.Configuration;
using TeamSupport.Proxy;

namespace TeamSupport.Model
{
    class AuthenticationModel
    {
        public int UserID { get; private set; }
        public int OrganizationID { get; private set; }
        public bool IsBackdoor { get; private set; }
        public bool IsSystemAdmin { get; private set; }
        public string SessionID { get; private set; }
        public string ConnectionString { get; private set; }
        public OrganizationUser OrganizationUser { get; private set; }

        public AuthenticationModel(FormsAuthenticationTicket authentication)
        {
            string[] data = authentication.UserData.Split('|');
            UserID = int.Parse(data[0]);
            OrganizationID = int.Parse(data[1]);
            IsBackdoor = (data.Length < 3) ? false : data[2] == "1";
            IsSystemAdmin = data[4] == "1";
            SessionID = (data.Length < 4) ? "" : data[3];

            OrganizationUser = new OrganizationUser(UserID, OrganizationID);

            ConnectionStringSettings cStrings = WebConfigurationManager.ConnectionStrings["MainConnection"];
            ConnectionString = (cStrings != null) ? cStrings.ConnectionString : ConfigurationManager.AppSettings["ConnectionString"];
        }

        /// <summary> static helper for logging </summary>
        public static OrganizationUser GetLoginUser(FormsAuthenticationTicket authentication)
        {
            string[] data = authentication.UserData.Split('|');
            return new OrganizationUser(int.Parse(data[0]), int.Parse(data[1]));
        }
    }
}
