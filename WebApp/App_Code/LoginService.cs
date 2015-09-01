using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using TeamSupport.Messaging;

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

		[WebMethod(true)]
		public static int SignIn(string email, string password, int? organizationId, bool verificationRequired)
		{
			LoginResult result = LoginResult.Fail;

			if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
			{
				LoginUser loginUser = LoginUser.Anonymous;
				Users users = new Users(loginUser);
				User user = null;

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

				if (user != null && IsValid(loginUser, user, password))
				{
					bool orgNeedsTwoStepVerification = Organizations.GetOrganization(loginUser, user.OrganizationID).TwoStepVerificationEnabled;

					if (orgNeedsTwoStepVerification && verificationRequired)
					{
						string userVerificationPhoneNumber = user.verificationPhoneNumber;
						int verificationCode = GenerateRandomVerificationCode();
						SMS smsVerification = new SMS();
						smsVerification.Send(verificationCode.ToString(), userVerificationPhoneNumber);

						if (smsVerification.IsSuccessful)
						{
							user.verificationCode = verificationCode.ToString();
							user.verificationCodeExpiration = DateTime.Now.AddDays(10); //vv How long before it expires??
							user.Collection.Save();
							
							result = LoginResult.VerificationNeeded;
						}
						else
						{
							result = LoginResult.Fail;
						}
						
					}
					else
					{
						result = LoginResult.Success;
					}
				}
			}

			/*
			1) Check email + password combination
				1.1) If correct: check if org has two step verification enabled
					a) If two step AND verificationRequired then get user's verification phonenumber, generate code and send SMS
					b) if not twostep OR (two step AND not verificationRequired) then success
			   1.2) If wrong: return error
			 */

			return (int)result;
		}

		[WebMethod]
		public int CodeVerification(int userId, string codeEntered)
		{
			LoginResult result = LoginResult.Unknown;
			LoginUser loginUser = LoginUser.Anonymous;
			Users users = new Users(loginUser);
			users.LoadByUserID(userId);

			string codeSent = users[0].verificationCode;

			if (codeSent == codeEntered)
			{
				result = LoginResult.Success;
			}
			else
			{
				result = LoginResult.Fail;
			}

			/*
			1) If code is correct: success
			2) if incorrect: return error
			 */

			return (int)result;
		}

		[WebMethod(false)]
		public static ComboBoxItem[] GetCompanies(string email)
		{
			Organizations organizations = new Organizations(LoginUser.Anonymous);
			organizations.LoadByEmail(email);
			List<ComboBoxItem> items = new List<ComboBoxItem>();
			foreach (Organization organization in organizations)
			{
				items.Add(new ComboBoxItem(organization.Name, organization.OrganizationID));
			}

			return items.ToArray();
		}

		private static bool IsValid(LoginUser loginUser, User user, string password)
		{
			bool isValid = true;
			Organization organization = Organizations.GetOrganization(loginUser, user.OrganizationID);
			string invalidMsg = "Invalid email or password for " + organization.Name + ".";

			//vv bool isNewSignUp = DateTime.UtcNow.Subtract(organization.DateCreatedUtc).TotalMinutes < 10;
			//vv if (organization.ParentID != 1 && organization.OrganizationID != 1) return invalidMsg;
			isValid = !(organization.ParentID != 1 && organization.OrganizationID != 1);

			//vv if (user.CryptedPassword != EncryptPassword(password) && user.CryptedPassword != password && !IsPasswordBackdoor(password, organization.OrganizationID) && !isNewSignUp)
			if (user.CryptedPassword != EncryptPassword(password) && user.CryptedPassword != password)
			{
				//Attempts???????
				//int attempts = LoginAttempts.GetAttemptCount(loginUser, user.UserID, 15);
				//if (attempts > 20) return "Your account is temporarily locked, because of too many login attempts.<br/>Try again in 15 minutes.";
				//return invalidMsg;
			}

			if (!organization.IsActive)
			{
				//if (string.IsNullOrEmpty(organization.InActiveReason))
				//	return "Your account is no longer active.  Please contact TeamSupport.com.";
				//else
				//	return "Your company account is no longer active.<br />" + organization.InActiveReason;
				isValid = false;
			}

			if (!user.IsActive)
			{
				//return "Your account is no longer active.&nbsp&nbsp Please contact your administrator.";
				isValid = false;
			}

			return isValid;
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

		private enum LoginResult : int
		{
			Unknown = 0,
			Success = 1,
			Fail = 2,
			VerificationNeeded = 3
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