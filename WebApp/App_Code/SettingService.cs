﻿using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace TSWebServices
{
  [ScriptService]
  [WebService(Namespace = "http://teamsupport.com/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class SettingService : System.Web.Services.WebService
  {

    public SettingService()
    {

      //Uncomment the following line if using designed components 
      //InitializeComponent(); 
    }

    [WebMethod]
    public string ReadUserSetting(string key, string defaultValue)
    {
      return Settings.UserDB.ReadString(key, defaultValue);
    }

    [WebMethod]
    public void WriteUserSetting(string key, string value)
    {
      Settings.UserDB.WriteString(key, value);
    }

    [WebMethod(true)]
    public string ReadSessionSetting(string key, string defaultValue)
    {
      return Settings.Session.ReadString(key, defaultValue);
    }

    [WebMethod(true)]
    public void WriteSessionSetting(string key, string value)
    {
      Settings.Session.WriteString(key, value);
    }

    [WebMethod(true)]
    public string ReadOrganizationSetting(string key, string defaultValue)
    {
      return Settings.OrganizationDB.ReadString(key, defaultValue);
    }

    [WebMethod(true)]
    public void WriteOrganizationSetting(string key, string value)
    {
      Settings.OrganizationDB.WriteString(key, value);
    }

    [WebMethod(true)]
    public string ReadSystemSetting(string key, string defaultValue)
    {
      return Settings.SystemDB.ReadString(key, defaultValue);
    }

    [WebMethod(true)]
    public void WriteSystemSetting(string key, string value)
    {
      Settings.SystemDB.WriteString(key, value);
    }

    [WebMethod(EnableSession = true)]
    public void SetMoxieManagerSessionVariables()
    {
      if (Session["isLoggedIn"] == null)
      {
        Session["isLoggedIn"] = "true";
        Session["user"] = UserSession.LoginUser.UserID.ToString();
        Directory.CreateDirectory("C:/TSData/WikiDocs/" + UserSession.LoginUser.OrganizationID + "/images");
        Directory.CreateDirectory("C:/TSData/WikiDocs/" + UserSession.LoginUser.OrganizationID + "/documents");

        Session["moxiemanager.filesystem.rootpath"] = "C:/TSData/WikiDocs/" + UserSession.LoginUser.OrganizationID;
        Session["moxiemanager.filesystem.local.wwwroot"] = "C:/TSData/WikiDocs/" + UserSession.LoginUser.OrganizationID;
        Session["moxiemanager.filesystem.local.urlprefix"] = "{proto}://{host}/Wiki/WikiDocs/" + UserSession.LoginUser.OrganizationID;
      }
    }


  }
}