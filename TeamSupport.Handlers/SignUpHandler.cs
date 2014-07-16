using System;
using System.Collections.Generic;
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
      context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      context.Response.AddHeader("Expires", "-1");
      context.Response.AddHeader("Pragma", "no-cache");

      using (Stream receiveStream = context.Request.InputStream)
      {
        using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
        {
          string requestContent = readStream.ReadToEnd();
          System.Collections.Specialized.NameValueCollection values = HttpUtility.ParseQueryString(requestContent);
          string name = GetValueString(values["name"]);
          string email = GetValueString(values["email"]);
          string company = GetValueString(values["company"]);
          string phone = GetValueString(values["phone"]);
          string password = GetValueString(values["password"]);
          string promo = GetValueString(values["promo"]);
          string product = GetValueString(values["product"]);

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
              version = (int)ProductType.Enterprise;
            }
          
          }


          if (IsCompanyValid(company))
          {

            string[] names = name.Split(' ');
            string fname = names[0];
            string lname = string.Join(" ", names.Skip(1).ToArray());

            User user = Organizations.SetupNewAccount(fname, lname, email, company, phone, (ProductType)version, password, promo, "", "", "");
          }
        }
      }
    }

    private static string GetValueString(string value)
    {
      return string.IsNullOrWhiteSpace(value) ? "" : HttpUtility.HtmlEncode(value);

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


  }
}