using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Net;
using System.Web.SessionState;
using System.Drawing;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Security;
using TeamSupport.Data;


namespace TeamSupport.Handlers
{

  public class SignUpHandler : IHttpHandler
  {
    public bool IsReusable
    {
      get { return false; }
    }

    /// <summary>
    /// Processes HTTP web requests directed to this HttpHandler.
    /// </summary>
    /// <param name="context">An HttpContext object that provides references 
    /// to the intrinsic server objects (for example, Request, Response, 
    /// Session, and Server) used to service HTTP requests.</param>
    public void ProcessRequest(HttpContext context)
    {
      //http://trunk.tsdev.com/signup/validateCompany?name=Muroc%20Systems,%20Inc.
      string fn = context.Request.Url.Segments[2].ToLower();
      if (fn == "fn/")
      {
        string segment = context.Request.Url.Segments[3].ToLower();
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.AddHeader("Expires", "-1");
        context.Response.AddHeader("Pragma", "no-cache");


        if (segment == "validatecompany")
        {
          ValidateCompany(context);
        }
        else if (segment == "post")
        {
          try
          {
            using (Stream receiveStream = context.Request.InputStream)
            {
              using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
              {
                string requestContent = readStream.ReadToEnd();
                try
                {
                  User user = ProcessSignUp(context, requestContent);
                  context.Response.Redirect("http://www.teamsupport.com/thank-you-for-trying-teamsupport/?userid=" + user.UserID.ToString(), false);

                }
                catch (Exception ex)
                {
                  ExceptionLogs.LogException(LoginUser.Anonymous, ex, "SIGN UP", requestContent);
                  context.Response.Redirect(GetErrorUrl(context), false);
                }
              }
            }
          }
          catch (Exception ex2)
          {
            ExceptionLogs.LogException(LoginUser.Anonymous, ex2, "SIGN UP");
            context.Response.Redirect(GetErrorUrl(context), false);
          }
        }
      }
    }

    private static string GetErrorUrl(HttpContext context, string requestContent = null)
    {
      string result = "https://www.teamsupport.com/web-help-desk-support-free-trial";//context.Request.UrlReferrer.AbsoluteUri;
      if (requestContent != null)
      {
        NameValueCollection values = HttpUtility.ParseQueryString(requestContent);
        result = appendUrlParam(result, "_name", GetValueString(values["name"]));
        result = appendUrlParam(result, "_email", GetValueString(values["email"]));
        result = appendUrlParam(result, "_company", GetValueString(values["company"]));
        result = appendUrlParam(result, "_phone", GetValueString(values["phone"]));
        result = appendUrlParam(result, "_product", GetValueString(values["product"]));
        result = appendUrlParam(result, "_promo", GetValueString(values["promo"]));
      }

      if (result.IndexOf("suerror=1") < 0) result = appendUrlParam(result, "suerror", "1");

      return result;
    }

    private static string appendUrlParam(string url, string param, string value)
    {
      if (string.IsNullOrWhiteSpace(value)) return url;
      return url + (url.IndexOf("?") > -1 ? "&" : "?") + param + "=" + HttpUtility.UrlEncode(value);
    }

    private static void ValidateCompany(HttpContext context)
    {

      context.Response.ContentType = "application/json; charset=utf-8";
      NameValueCollection values = context.Request.QueryString;
      context.Response.Write(string.Format("{0}({{\"isValid\": {1}}})", values["callback"], IsCompanyValid(values["name"]) ? "true" : "false"));

    }

    private static User ProcessSignUp(HttpContext context, string requestContent)
    {
      NameValueCollection values = HttpUtility.ParseQueryString(requestContent);
      string name = GetValueString(values["name"]);
      string email = GetValueString(values["email"]);
      string company = GetValueString(values["company"]);
      string phone = GetValueString(values["phone"]);
      string password = GetValueString(values["password"]);
      string promo = GetValueString(values["promo"]);
      string product = GetValueString(values["product"]);
      string source = "";

      int version = (int)ProductType.Enterprise;
      if (product != "")
      {
        if (int.TryParse(product, out version))
        {
          if (version != (int)ProductType.Enterprise || version != (int)ProductType.HelpDesk)
          {
            version = (int)ProductType.Enterprise;
          }
        }
        else
        {
          if (product.ToLower().Trim() == "support desk") version = (int)ProductType.HelpDesk;
        }
      }


      if (IsCompanyValid(company))
      {

        string[] names = name.Split(' ');
        string fname = names[0];
        string lname = string.Join(" ", names.Skip(1).ToArray());
        HttpCookieCollection cookies = context.Request.Cookies;
        SignUpParams prams = new SignUpParams();
        if (cookies["_tsm"] != null)
        {

          try
          {
            MarketingCookie mc = JsonConvert.DeserializeObject<MarketingCookie>(HttpUtility.UrlDecode(cookies["_tsm"].Value));
            prams.utmCampaign = mc.Campaign;
            prams.utmContent = mc.Content;
            prams.utmMedium = mc.Medium;
            prams.utmSource = mc.Source;
            prams.utmTerm = mc.Term;
            source = (source.Trim().Length > 0) ? source : mc.Source;
          }
          catch (Exception)
          {
            prams.utmCampaign = "Error";
          }
        }

        if (cookies["_tsmi"] != null)
        {

          try
          {
            MarketingCookie mc = JsonConvert.DeserializeObject<MarketingCookie>(HttpUtility.UrlDecode(cookies["_tsmi"].Value));
            prams.initialCampaign = mc.Campaign;
            prams.initialContent = mc.Content;
            prams.initialMedium = mc.Medium;
            prams.initialSource = mc.Source;
            prams.initialTerm = mc.Term;
            source = (source.Trim().Length > 0) ? source : mc.Source;
          }
          catch (Exception)
          {
            prams.initialCampaign = "Error";
          }
        }

        if (cookies["__utmz"] != null)
        {
          try
          {
            string utmz = cookies["__utmz"].Value;
            //string utmz = "252527244.1405639771.1.1.utmcsr=GetApp|utmccn=GetApp|utmcmd=cpc";
            prams.gaCampaign = parseGAString(utmz, "utmccn");
            prams.gaContent = parseGAString(utmz, "utmcct");
            prams.gaTerm = parseGAString(utmz, "utmctr");
            prams.gaMedium = parseGAString(utmz, "utmcmd");
            prams.gaSource = parseGAString(utmz, "utmcsr");
            source = (source.Trim().Length > 0) ? source : parseGAString(utmz, "utmcsr");
            /*
            if (parseGAString(utmz, "utmgclid") != "")
            {
              prams.gaSource = "AdWords";
              prams.gaMedium = "cpc";
            }
             */
             
          }
          catch (Exception)
          {

          }
        }

        if (cookies["__utma"] != null)
        {
          try
          {
            string utma = cookies["__utma"].Value;
            //string utma = "252527244.1199382232.1405639771.1405639771.1405639771.1";
            string[] sessionValues = utma.Split('.');
            prams.gaVisits = int.Parse(sessionValues[5]);
          }
          catch (Exception)
          {

          }
        }
        HubspotPost(fname, lname, email, company, phone, promo, source, cookies);
        return Organizations.SetupNewAccount(fname, lname, email, company, phone, (ProductType)version, prams);
      }
      else
      {
        throw new Exception("Invalid Company: " + company);
      }
    }
    private static void HubspotPost(string fname, string lname, string email, string company, string phone, string promo, string source, HttpCookieCollection cookies)
    {
       
        Dictionary <string, string> dictFormValues = new Dictionary < string, string > ();
		dictFormValues.Add("firstname", fname);
		dictFormValues.Add("lastname", lname);
		dictFormValues.Add("email", email);
		dictFormValues.Add("phone", phone);
        dictFormValues.Add("company", company);
        dictFormValues.Add("campaign", promo);
        dictFormValues.Add("marketingsource", source);
        dictFormValues.Add("lifecyclestage", "Sales Qualified Lead");

        int intPortalID = 448936; 
		string strFormGUID = "95bb0d19-4eb4-4fec-8b83-9ab00a3efde2"; 

		// Tracking Code Variables
		string strHubSpotUTK = cookies["hubspotutk"].Value;
		string strIpAddress = System.Web.HttpContext.Current.Request.UserHostAddress;

		// Page Variables
		string strPageTitle = "";
		string strPageURL = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;

		// Do the post, returns true/false
		string strError = "";
		bool blnRet = Do_Post_To_HubSpot_FormsAPI(intPortalID, strFormGUID, dictFormValues, strHubSpotUTK, strIpAddress, strPageTitle, strPageURL, ref strError);
		if (!blnRet) {
			ExceptionLogs.LogException(LoginUser.Anonymous, new Exception("Error Posting To HUB SPOT"), "HUB SPOT", dictFormValues.ToDebugString());			
		}

    }
    private static bool Do_Post_To_HubSpot_FormsAPI(int intPortalID, string strFormGUID, Dictionary<string, string> dictFormValues, string strHubSpotUTK, string strIpAddress, string strPageTitle, string strPageURL, ref string strMessage)
    {

        // Build Endpoint URL
        string strEndpointURL = string.Format("https://forms.hubspot.com/uploads/form/v2/{0}/{1}", intPortalID, strFormGUID);

        // Setup HS Context Object
        Dictionary<string, string> hsContext = new Dictionary<string, string>();
        hsContext.Add("hutk", strHubSpotUTK);
        hsContext.Add("ipAddress", strIpAddress);
        hsContext.Add("pageUrl", strPageURL);
        hsContext.Add("pageName", strPageTitle);

        // Serialize HS Context to JSON (string)
        System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        string strHubSpotContextJSON = json.Serialize(hsContext);

        // Create string with post data
        string strPostData = "";

        // Add dictionary values
        foreach (var d in dictFormValues)
        {
            strPostData += d.Key + "=" + HttpUtility.UrlEncode(d.Value) + "&";
        }

        // Append HS Context JSON
        strPostData += "hs_context=" + HttpUtility.UrlEncode(strHubSpotContextJSON);

        // Create web request object
        System.Net.HttpWebRequest r = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(strEndpointURL);

        // Set headers for POST
        r.Method = "POST";
        r.ContentType = "application/x-www-form-urlencoded";
        r.ContentLength = strPostData.Length;
        r.KeepAlive = false;

        // POST data to endpoint
        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(r.GetRequestStream()))
        {
            try
            {
                sw.Write(strPostData);
            }
            catch (Exception ex)
            {
                // POST Request Failed
                strMessage = ex.Message;
                return false;
            }
        }

        return true; //POST Succeeded

    }

    private static string parseGAString(string cookieValue, string key)
    {
      //252527244.1405639771.1.1.utmcsr=GetApp|utmccn=GetApp|utmcmd=cpc
      int i = cookieValue.IndexOf(key + "=");
      if (i < 0) return "";
      string s = cookieValue.Substring(i + key.Length + 1);
      int j = s.IndexOf("|");
      if (j > -1) { s = s.Substring(0, j); }

      return HttpUtility.UrlDecode(s);
    }

    private static string GetValueString(string value)
    {
      return string.IsNullOrWhiteSpace(value) ? "" : value;
    }

    public static bool IsCompanyValid(string company)
    {
      Organizations organizations = new Organizations(LoginUser.Anonymous);
      organizations.LoadByOrganizationName(company.Trim());
      if (!organizations.IsEmpty)
      {
        return false;
      }
      return true;
    }

    public class MarketingCookie
    {
      public string Source { get; set; }
      public string Medium { get; set; }
      public string Term { get; set; }
      public string Content { get; set; }
      public string Campaign { get; set; }

    }
  }

  public static class ExtensionMethods
  {
      public static string ToDebugString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
      {
          return "{" + string.Join(",", dictionary.Select(kv => kv.Key.ToString() + "=" + kv.Value.ToString()).ToArray()) + "}";
      }
  }

}