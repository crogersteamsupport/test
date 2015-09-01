using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Script.Serialization;
using System.Web.Script.Services;

namespace TSWebServices
{
	[ScriptService]
	[WebService(Namespace = "http://teamsupport.com/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class LoginService : System.Web.Services.WebService
	{
		public LoginService()
		{

			//Uncomment the following line if using designed components 
			//InitializeComponent(); 
		}

		[WebMethod]
		public int SignIn(string email, string password, int? organizationId, bool verificationRequired)
		{
			int result = 0;

			return result;
		}

	}
}