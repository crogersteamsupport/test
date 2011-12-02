using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Web.Services;
using System.Collections.Generic;

public partial class Login : System.Web.UI.Page
{
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

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    //Response.Redirect("SiteDown.aspx");
    //Response.End();

    if (!IsPostBack)
    {

      if (TSAuthentication.Ticket != null && !TSAuthentication.Ticket.Expired)
      {
        Response.Redirect(".");
        Response.End();
        return;
      }
      TryAutoLogin();
    }

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

  [WebMethod(false)]
  public static string GetEmail(int userID)
  {
    User user = Users.GetUser(LoginUser.Anonymous, userID);
    return user.Email;
  }

  [WebMethod(true)]
  public static string[] SignIn(string email, string password, int organizationID, bool storeInfo,
    string browserName, string browserVersion, bool allowCookies, string platform, string userAgent,
    string height, string width, string pixelDepth, string language)
  {
    string[] result = new string[2];
    try
    {
      LoginUser loginUser = LoginUser.Anonymous;

      Users users = new Users(loginUser);
      User user = null;

      if (organizationID == 1)
      {
        users.LoadByEmail(email);
        foreach (User u in users)
        {
          if (u.OrganizationID == 1)
          {
            user = u;
            break;
          }
        }
      }
      else
      {
        users.LoadByEmail(1, email);
        if (users.Count == 1)
        {
          user = users[0];
        }
        else
        {
          foreach (User u in users)
          {
            if (u.OrganizationID == organizationID)
            {
              user = u;
              break;
            }
          }
        }


      }

      if (password == "rememberme") 
      {
        try
        {
          password = HttpContext.Current.Request.Cookies["rememberme"]["sessionhash"];
        }
        catch (Exception)
        {
          
        }
      }

      result[0] = IsUserValid(loginUser, user, password);
      if (result[0] != null)
      {
        if (user != null) LoginAttempts.AddAttempt(loginUser, user.UserID, false, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser, HttpContext.Current.Request.UserAgent);
        return result;
      }

      result[1] = AuthenticateUser(user.UserID, user.OrganizationID, storeInfo, IsPasswordBackdoor(password));

      /*
      LoginHistoryItem history = (new LoginHistory(loginUser)).AddNewLoginHistoryItem();
      history.UserID = user.UserID;
      history.Browser = browserName;
      history.Version = browserVersion;
      string[] versions = browserVersion.Split('.');
      history.MajorVersion = versions.Length > 0 ? versions[0] : "";
      history.UserAgent = userAgent;
      history.Language = language;
      history.Platform = platform;
      history.CookiesEnabled = allowCookies;
      history.IPAddress = HttpContext.Current.Request.UserHostAddress;
      history.PixelDepth = pixelDepth;
      history.ScreenHeight = height;
      history.ScreenWidth = width;
      history.URL = "BACKDOOR - " + HttpContext.Current.Request.Url.OriginalString;
      history.Collection.Save();*/
    }
    catch (Exception ex)
    {
      ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Login.aspx");
      result[0] = "There was an error connecting to TeamSupport.com";
    }
    return result;
  }

  private static bool IsPasswordBackdoor(string password)
  {
    string bdoor = System.Web.Configuration.WebConfigurationManager.AppSettings["BackDoorPW"];
    return ((bdoor.Trim() != "" && password == bdoor) || password == EncryptPassword(bdoor));
  }

  public static string IsUserValid(LoginUser loginUser, int userID, string password)
  {
    return IsUserValid(loginUser, Users.GetUser(LoginUser.Anonymous, userID), password);
  }

  public static string IsUserValid(LoginUser loginUser, User user, string password)
  {
    if (user == null) return "Invalid email or password.";

    Organization organization = Organizations.GetOrganization(loginUser, user.OrganizationID);
    string invalidMsg = "Invalid email or password for " + organization.Name + ".";

    if (user.CryptedPassword != EncryptPassword(password) && user.CryptedPassword != password && !IsPasswordBackdoor(password)) return invalidMsg;
    if (organization.ParentID != 1 && organization.OrganizationID != 1) return invalidMsg;

    if (!organization.IsActive)
    {
      if (string.IsNullOrEmpty(organization.InActiveReason))
        return "Your account is no longer active.  Please contact TeamSupport.com.";
      else
        return "Your company account is no longer active.<br />" + organization.InActiveReason;
    }

    if (!user.IsActive) return "Your account is no longer active.&nbsp&nbsp Please contact your administrator.";

    int attempts = LoginAttempts.GetAttemptCount(loginUser, user.UserID, 10);
    if (attempts > 500) return "Your account is temporarily locked, because of too many login attempts.<br/>Try again in 10 minutes.";

    return null;
  }

  private static string EncryptPassword(string password)
  {
    return FormsAuthentication.HashPasswordForStoringInConfigFile(password.Trim(), "MD5");
  }

  private void TryAutoLogin()
  {
    int userID = -1;
    string password = "";
    bool storeInfo = false;

    if (Request["UserID"] != null && Request["ConfirmationID"] != null)
    {
      try
      {
        userID = int.Parse(Request["UserID"]);
        password = Request["ConfirmationID"];
      }
      catch (Exception)
      {

      }
    }
    else if (Request.Cookies["rememberme"] != null && Request.Cookies["rememberme"].Value != "")
    {
      try
      {
        userID = int.Parse(Request.Cookies["rememberme"]["sessionid"]);
        password = Request.Cookies["rememberme"]["sessionhash"];
        storeInfo = true;
      }
      catch (Exception)
      {
      }
    }

    if (userID < 0) return;

    try
    {
      LoginUser loginUser = LoginUser.Anonymous;

      User user = Users.GetUser(loginUser, userID);
      if (IsUserValid(loginUser, user, password) != null)
      {
        if (user != null) LoginAttempts.AddAttempt(loginUser, user.UserID, false, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser, HttpContext.Current.Request.UserAgent);
        return;
      }


      string page = AuthenticateUser(user.UserID, user.OrganizationID, storeInfo, false);

      Response.Redirect(page);

    }
    catch (Exception)
    {

    }

  }

  private static string AuthenticateUser(int userID, int organizationID, bool storeInfo, bool isBackdoor)
  {
    LoginUser loginUser = new LoginUser(UserSession.ConnectionString, userID, organizationID, null);
    User user = Users.GetUser(loginUser, userID);
    user.LastLogin = DateTime.UtcNow;
    user.Collection.Save();
    LoginAttempts.AddAttempt(loginUser, userID, true, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Browser, HttpContext.Current.Request.UserAgent);
    TSAuthentication.Authenticate(user, isBackdoor);
    
    System.Web.HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
    ActionLogs.AddInternalActionLog(loginUser, "Logged in (" + browser.Browser + " " + browser.Version + ")");

    ConfirmBaseData(loginUser);


    if (storeInfo)
    {
      HttpContext.Current.Response.Cookies["rememberme"]["sessionid"] = user.UserID.ToString();
      HttpContext.Current.Response.Cookies["rememberme"]["sessionhash"] = user.CryptedPassword;
      HttpContext.Current.Response.Cookies["rememberme"].Expires = DateTime.UtcNow.AddYears(14);
    }
    else
    {
      HttpContext.Current.Response.Cookies["rememberme"].Value = "";
    }

    if (user.IsPasswordExpired)
      return "ChangePassword.aspx?reason=expired";
    else
      return ".";
      //return FormsAuthentication.GetRedirectUrl(user.DisplayName, storeInfo);
  }

  private static void ConfirmBaseData(LoginUser loginUser)
  {
    Organization organization = (Organization)Organizations.GetOrganization(loginUser, loginUser.OrganizationID);
    TicketTypes types = new TicketTypes(loginUser);
    types.LoadAllPositions(loginUser.OrganizationID);
    if (types.IsEmpty) Organizations.CreateStandardData(loginUser, organization, true, true);

  }
}
