using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using TeamSupport.Messaging;
using Newtonsoft.Json;

namespace TSWebServices
{
	[ScriptService]
	[WebService(Namespace = "http://teamsupport.com/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class LoginService : System.Web.Services.WebService
	{
		private const int MAXLOGINATTEMPTS = 10;
		private const int MINUTESTOEXPIREVERIFICATIONCODE = 5;
		private static bool _skipVerification = false;

		public LoginService()
		{

			//Uncomment the following line if using designed components 
			//InitializeComponent(); 
		}

		[WebMethod]
		[ScriptMethod(ResponseFormat=ResponseFormat.Json)]
		public string SignIn(string email, string password, int? organizationId, bool verificationRequired)
		{
			SignInResult result = new SignInResult();
			LoginUser	loginUser	= LoginUser.Anonymous;
			User			user			= null;
			Organization organization = null;

			if (password == "sl")
			{
				try
				{
					password = HttpContext.Current.Request.Cookies["sl"]["b"];
				}
				catch (Exception)
				{
					//vv error reading password from cookie
					password = string.Empty;
				}
			}

			_skipVerification = false;
			result = IsValid(loginUser, email, password, organizationId, ref user, ref organization);

			if (result.Result == LoginResult.Success)
			{
				if (organization.TwoStepVerificationEnabled && verificationRequired && !_skipVerification)
				{
					string userVerificationPhoneNumber	= user.verificationPhoneNumber;

					if (!string.IsNullOrEmpty(userVerificationPhoneNumber))
					{
						int verificationCode = SendAndGetVerificationCode(userVerificationPhoneNumber);

						if (verificationCode > 0)
						{
							user.verificationCode = verificationCode.ToString();
							user.verificationCodeExpiration = DateTime.Now.AddMinutes(MINUTESTOEXPIREVERIFICATIONCODE);
							user.Collection.Save();

							result.Result = LoginResult.VerificationNeeded;
						}
						else
						{
							result.Error = "Verification Code failed to be generated or sent.";
							result.Result = LoginResult.Fail;
						}
					}
					else
					{
						result.Error = "Organization requires two step verification and user does not have a verification phone number setup.p";
						result.Result = LoginResult.VerificationSetupNeeded;
					}
				}
				else
				{
					string authenticateResult = AuthenticateUser(user.UserID, user.OrganizationID, true);

					result.UserId = user.UserID;
					result.OrganizationId = user.OrganizationID;
				}
			}

			return JsonConvert.SerializeObject(result);
		}

		[WebMethod]
		public string CodeVerification(int userId, string codeEntered)
		{
			SignInResult result = new SignInResult();
			LoginUser loginUser = LoginUser.Anonymous;

			try
			{
				Users users = new Users(loginUser);
				users.LoadByUserID(userId);
				result.UserId = userId;

				if (users.Count > 0)
				{
					result.OrganizationId = users[0].OrganizationID;

					string codeSent = users[0].verificationCode;

					if (codeSent == codeEntered)
					{
						if (users[0].verificationCodeExpirationUtc > DateTime.UtcNow)
						{
							users[0].verificationCode = null;
							users[0].verificationCodeExpiration = null;
							users.Save();
							result.Result = LoginResult.Success;
						}
						else
						{
							result.Error = "Verification Code has expired.";
							result.Result = LoginResult.Fail;
						}
					}
					else
					{
						result.Error = "Invalid Verification Code.";
						result.Result = LoginResult.Fail;
					}
				}
				else
				{
					result.Error = "User not found.";
					result.Result = LoginResult.Fail;
				}
			}
			catch (Exception ex)
			{
				result.Error = ex.Message;
				result.Result = LoginResult.Fail;
			}

			return JsonConvert.SerializeObject(result);
		}

		[WebMethod]
		public string SetupVerificationPhoneNumber(int userId, string phoneNumber)
		{
			SignInResult result = new SignInResult();
			LoginUser loginUser = LoginUser.Anonymous;
			result.UserId = userId;

			try
			{
				Users users = new Users(loginUser);
				users.LoadByUserID(userId);

				if (users.Count > 0)
				{
					result.OrganizationId = users[0].OrganizationID;
					users[0].verificationPhoneNumber = phoneNumber;
					users.Save();

					int verificationCode = SendAndGetVerificationCode(phoneNumber);

					if (verificationCode > 0)
					{
						result.Result = LoginResult.Success;	
					}
					else
					{
						result.Error = "Verification Phone Number updated but the Verification Code failed to be generated or sent.";
						result.Result = LoginResult.Fail;
					}
				}
				else
				{
					result.Error = "User not found.";
					result.Result = LoginResult.Fail;
				}
			}
			catch (Exception ex)
			{
				result.Error = ex.Message;
				result.Result = LoginResult.Fail;
			}

			return JsonConvert.SerializeObject(result);
		}

		[WebMethod]
		[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
		public string GetCompanies(string email)
		{
			Organizations organizations = new Organizations(LoginUser.Anonymous);
			organizations.LoadByEmail(email);
			List<ComboBoxItem> items = new List<ComboBoxItem>();
			foreach (Organization organization in organizations)
			{
				items.Add(new ComboBoxItem(organization.Name, organization.OrganizationID));
			}

			return JsonConvert.SerializeObject(items);
		}

    [WebMethod(true)]
    public string GetEmail(int userID)
    {
      User user = Users.GetUser(LoginUser.Anonymous, userID);
      return user.Email;
    }

		private static SignInResult IsValid(LoginUser loginUser, string email, string password, int? organizationId, ref User user, ref Organization organization)
		{
			SignInResult validation = new SignInResult();

			if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
			{
				Users users = new Users(loginUser);

				users.LoadByEmail(1, email);

				if (users.Count == 1)
				{
					user = users[0];
				}
				else
				{
					foreach (User u in users)
					{
						if (u.OrganizationID == organizationId)
						{
							user = u;
							break;
						}
					}
				}

				if (user != null)
				{
					organization = Organizations.GetOrganization(loginUser, user.OrganizationID);

					if (IsSupportImpersonation(password))
					{
						_skipVerification = false;
						validation.Result = LoginResult.Success;
						validation.Error = string.Empty;
						//vv Log this information!
					}
					else
					{
						//vv bool isNewSignUp = DateTime.UtcNow.Subtract(organization.DateCreatedUtc).TotalMinutes < 10;
						//vv if (organization.ParentID != 1 && organization.OrganizationID != 1) return invalidMsg;

						//vv if (user.CryptedPassword != EncryptPassword(password) && user.CryptedPassword != password && !IsPasswordBackdoor(password, organization.OrganizationID) && !isNewSignUp)
						if ((organization.ParentID == 1 && organization.OrganizationID != 1) && user.CryptedPassword != EncryptPassword(password) && user.CryptedPassword != password)
						{
							validation.Error = "Invalid email or password.";
							int attempts = LoginAttempts.GetAttemptCount(loginUser, user.UserID, 15);

							if (attempts > MAXLOGINATTEMPTS)
							{
								validation.Error = "Your account is temporarily locked, because of too many login attempts.<br/>Try again in 15 minutes.";
							}
						}

						if (!organization.IsActive)
						{
							if (string.IsNullOrEmpty(organization.InActiveReason))
								validation.Error = "Your account is no longer active.  Please contact TeamSupport.com.";
							else
								validation.Error = "Your company account is no longer active.<br />" + organization.InActiveReason;
						}

						if (!user.IsActive)
						{
							validation.Error = "Your account is no longer active.&nbsp&nbsp Please contact your administrator.";
						}
					}
				}
				else
				{
					validation.Error = "Invalid email or password.";
				}
			}
			else
			{
				validation.Error = "Invalid email or password.";
			}

			if (!string.IsNullOrEmpty(validation.Error))
			{
				validation.Result = LoginResult.Fail;
				LoginAttempts.AddAttempt(loginUser, user.UserID, false, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser, HttpContext.Current.Request.UserAgent, GetDeviceID());
			}
			else
			{
				validation.Result = LoginResult.Success;
			}

			return validation;
		}

		/// <summary>
		/// Check if password entered is a code to impersonate other account's user.
		/// </summary>
		/// <returns>true/false</returns>
		private static bool IsSupportImpersonation(string password)
		{
			bool isImpersonation = false;

			return isImpersonation;
		}

		private static int SendAndGetVerificationCode(string userVerificationPhoneNumber)
		{
			int verificationCode = GenerateRandomVerificationCode();
			SMS smsVerification = new SMS();
			smsVerification.Send(verificationCode.ToString(), userVerificationPhoneNumber);

			if (!smsVerification.IsSuccessful)
			{
				verificationCode = 0;
			}

			return verificationCode;
		}

		private static int GenerateRandomVerificationCode()
		{
			Random rand = new Random((int)System.DateTime.Now.Ticks);
			return rand.Next(10000000, 99999999);
		}

		private static string EncryptPassword(string password)
		{
			return FormsAuthentication.HashPasswordForStoringInConfigFile(password.Trim(), "MD5");
		}

		private static string AuthenticateUser(int userId, int organizationId, bool storeInfo)
		{
			string result = string.Empty;
			LoginUser loginUser = new LoginUser(UserSession.ConnectionString, userId, organizationId, null);
			User user = Users.GetUser(loginUser, userId);
			string deviceID = GetDeviceID();

			if (deviceID == "")
			{
				deviceID = Guid.NewGuid().ToString();
				HttpCookie deviceCookie = new HttpCookie("di", deviceID);
				deviceCookie.Expires = DateTime.Now.AddYears(14);
				HttpContext.Current.Response.Cookies.Add(deviceCookie);
			}

			LoginAttempts.AddAttempt(loginUser, userId, true, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser, HttpContext.Current.Request.UserAgent, deviceID);
			TSAuthentication.Authenticate(user, false, deviceID);
			System.Web.HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
			ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Users, userId, "Logged in (" + browser.Browser + " " + browser.Version + ")");

			ConfirmBaseData(loginUser);

			if (storeInfo)
			{
				HttpContext.Current.Response.Cookies["sl"]["a"] = user.UserID.ToString();
				HttpContext.Current.Response.Cookies["sl"]["b"] = user.CryptedPassword;
				HttpContext.Current.Response.Cookies["sl"].Expires = DateTime.UtcNow.AddYears(14);
			}
			else
			{
				HttpContext.Current.Response.Cookies["sl"].Value = "";
			}

			if (user.IsPasswordExpired)
				result = "ChangePassword.aspx?reason=expired";
			else
			{
				string rawQueryString = null;

				try
				{
					rawQueryString = HttpContext.Current.Request.UrlReferrer.Query;
				}
				catch (Exception)
				{
					//vv
				}

				if (!string.IsNullOrEmpty(rawQueryString))
				{
					string urlRedirect = GetQueryStringValue(rawQueryString, "ReturnUrl");

					if (!string.IsNullOrEmpty(urlRedirect) && urlRedirect.Trim().Length > 0)
						result = urlRedirect;
					else
						result = ".";
				}
				else
				{
					result = ".";
				}
			}

			return result;
		}

		private static string GetDeviceID()
		{
			if (HttpContext.Current.Request.Cookies["di"] != null && HttpContext.Current.Request.Cookies["di"].Value != "")
			{
				return HttpContext.Current.Request.Cookies["di"].Value.Trim();
			}
			else
			{
				return "";
			}
		}

		private static string GetQueryStringValue(string queryStr, string key)
		{
			string returnValue = null;
			NameValueCollection queryStrPairs = HttpUtility.ParseQueryString(queryStr);

			if (queryStrPairs != null && queryStrPairs.AllKeys.Contains(key))
			{
				returnValue = queryStrPairs[key];
			}

			return returnValue;
		}

		private static void ConfirmBaseData(LoginUser loginUser)
		{
			Organization organization = (Organization)Organizations.GetOrganization(loginUser, loginUser.OrganizationID);
			TicketTypes types = new TicketTypes(loginUser);
			types.LoadAllPositions(loginUser.OrganizationID);

			if (types.IsEmpty)
			{
				Organizations.CreateStandardData(loginUser, organization, true, true);
			}
		}

		public class SignInResult
		{
			public int UserId { get; set; }
			public int OrganizationId { get; set; }
			public LoginResult Result { get; set; }
			public string ResultValue
			{
				get
				{
					return Result.ToString();
				}
			}
			public string Error { get; set; }
		}

		public enum LoginResult : int
		{
			Unknown = 0,
			Success = 1,
			Fail = 2,
			VerificationNeeded = 3,
			VerificationSetupNeeded = 4
		}

		[Serializable]
		public class ComboBoxItem
		{
			public ComboBoxItem(string label, int id)
			{
				Label = label;
				ID = id;
			}
			public string Label { get; set; }
			public int ID { get; set; }
		}
	}
}