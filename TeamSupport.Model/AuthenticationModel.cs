using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Configuration;
using System.Web.Configuration;

namespace TeamSupport.Model
{
    public class AuthenticationModel
    {
        public int UserID { get; private set; }
        public int OrganizationID { get; private set; }
        public bool IsBackdoor { get; private set; }
        public bool IsSystemAdmin { get; private set; }
        public string SessionID { get; private set; }
        public string ConnectionString { get; private set; }
        public Data.LoginUser LoginUser { get; private set; }

        public AuthenticationModel(FormsAuthenticationTicket authentication)
        {
            string[] data = authentication.UserData.Split('|');
            UserID = int.Parse(data[0]);
            OrganizationID = int.Parse(data[1]);
            IsBackdoor = (data.Length < 3) ? false : data[2] == "1";
            IsSystemAdmin = data[4] == "1";
            SessionID = (data.Length < 4) ? "" : data[3];

            LoginUser = new Data.LoginUser(UserID, OrganizationID);

            ConnectionStringSettings cStrings = WebConfigurationManager.ConnectionStrings["MainConnection"];
            ConnectionString = (cStrings != null) ? cStrings.ConnectionString : ConfigurationManager.AppSettings["ConnectionString"];
        }

        /// <summary> static helper for logging </summary>
        public static Data.LoginUser GetLoginUser(FormsAuthenticationTicket authentication)
        {
            string[] data = authentication.UserData.Split('|');
            return new Data.LoginUser(int.Parse(data[0]), int.Parse(data[1]));
        }
    }
}
