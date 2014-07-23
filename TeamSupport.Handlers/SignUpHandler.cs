﻿using System;
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
      context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      context.Response.AddHeader("Expires", "-1");
      context.Response.AddHeader("Pragma", "no-cache");
      //http://trunk.tsdev.com/signup/validateCompany?name=Muroc%20Systems,%20Inc.
      string segment = context.Request.Url.Segments[context.Request.Url.Segments.Length - 1].ToLower();

        
      if (segment == "validatecompany")
      {
        ValidateCompany(context);
      }
      else if (segment == "processsignup")
      {
        User user = ProcessSignUp(context);
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.Write("{ \"result\": "+user.UserID.ToString()+" }");
      }
      else
      {
        User user = ProcessSignUp(context);
        context.Response.Redirect("http://www.teamsupport.com/thank-you-for-trying-teamsupport/?userid=" + user.UserID.ToString(), false);
      }


    }

    private static void ValidateCompany(HttpContext context)
    {
      context.Response.ContentType = "application/json; charset=utf-8";

      if (IsCompanyValid(context.Request.QueryString["name"]))
      {
        context.Response.Write("{ \"result\": true }");
      }
      else
      {
        context.Response.Write("{ \"result\": false }");
      }
    }

    private static User ProcessSignUp(HttpContext context)
    {
      using (Stream receiveStream = context.Request.InputStream)
      {
        using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
        {
          string requestContent = readStream.ReadToEnd();
          NameValueCollection values = HttpUtility.ParseQueryString(requestContent);
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
            HttpCookieCollection cookies = context.Request.Cookies;
            SignUpParams prams = new SignUpParams();
            if (cookies["_tsm"] != null)
            {
              try
              {
                prams.utmSource = cookies["_tsm"]["src"] ?? "";
                prams.utmCampaign = cookies["_tsm"]["camp"] ?? "";
                prams.utmContent = cookies["_tsm"]["cont"] ?? "";
                prams.utmMedium = cookies["_tsm"]["med"] ?? "";
                prams.utmTerm = cookies["_tsm"]["term"] ?? "";
              }
              catch (Exception)
              {
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

                if (parseGAString(utmz, "utmgclid") != "")
                {
                  prams.gaSource = "Google";
                  prams.gaMedium = "cpc";
                }
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

            User user = Organizations.SetupNewAccount(fname, lname, email, company, phone, (ProductType)version, prams);
            return user;
          }
        }
      }
      return null;
      
    }
    
    private static string parseGAString(string cookieValue, string key)
    {
      //252527244.1405639771.1.1.utmcsr=GetApp|utmccn=GetApp|utmcmd=cpc
      int i = cookieValue.IndexOf(key + "=");
      if (i < 0) return "";
      string s = cookieValue.Substring(i + key.Length + 1);
      int j = s.IndexOf("|");
      if (j > -1) { s = s.Substring(0, j); }

      return s;
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


  }
}