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
using System.Web.Security;
using System.Security.Principal;
using TeamSupport.WebUtils;
using System.IO;




namespace TeamSupport.Handlers
{
  class SessionModule : IHttpModule
  {

    public void Dispose()
    {
    }

    public void Init(HttpApplication app)
    {
        app.AuthenticateRequest += new EventHandler(context_AuthenticateRequest);
    }


    void context_AuthenticateRequest(object sender, EventArgs e)
    {
      HttpApplication app = (HttpApplication)sender;

      var request = app.Context.Request;

      string path = request.RawUrl.ToLower().Trim();
      if (path.IndexOf("services/") > -1 && !(path.EndsWith("/js") || path.EndsWith("/jsdebug")))
      {
        app.Context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //check for old webservice privateservices
        if (path.IndexOf("privateservices.asmx") > -1) return;
        if (path.IndexOf("getmainpageupdate") > -1) return;
        if (path.IndexOf("signout") > -1) return;
        

        var bytes = new byte[request.InputStream.Length];
        request.InputStream.Read(bytes, 0, bytes.Length);
        request.InputStream.Position = 0;
        string content = Encoding.ASCII.GetString(bytes);

        if (TSAuthentication.Ticket == null || TSAuthentication.Ticket.Expired || content.ToLower().IndexOf(TSAuthentication.SessionID.ToLower() ) < 0)
        {
          app.Response.StatusCode = (int)HttpStatusCode.NoContent;
          app.Response.End();
          app.CompleteRequest();
        }
      }
    }



  }
}

