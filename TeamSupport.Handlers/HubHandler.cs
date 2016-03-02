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
using OfficeOpenXml;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using DDay.iCal.Serialization;
using DDay.iCal;
using DDay.Collections;
using System.Drawing.Text;
using System.Collections.Specialized;
using System.Dynamic;



namespace TeamSupport.Handlers
{
	public class HubHandler : IHttpHandler
	{
		#region IHttpHandler Members

		public bool IsReusable
		{
			get { return false; }
		}


		public void ProcessRequest(HttpContext context)
		{

            //SAMPLE: http://localhost/hub/search/kb?q=search%20term
            try
            {

                switch (GetSegment(context, 0))
                {
                    case "search":
                        ProcessSearch(context);
                        break;
                    default:
                        break;
                }
			}
			catch (Exception ex)
			{
				context.Response.ContentType = "text/html";
				context.Response.Write(ex.Message + "<br />" + ex.StackTrace);
			}
			context.Response.End();
		}


        #endregion


        private string GetSegment(HttpContext context, int index)
        {

            for (int i = 0; i < context.Request.Url.Segments.Length; i++)
            {
                if (context.Request.Url.Segments[i].ToLower() == "hub/")
                {
                    return context.Request.Url.Segments[index + ++i].TrimEnd('/');
                }

            }
            return "";
        }
		private void ProcessSearch(HttpContext context)
		{
            string term = context.Request.QueryString["q"];
            int userID = -1;
            int parentID = -1;


            // the name of this cookie Hub_Session is set up on the hub, in the web.config.  
            // The same value of the machineKey in the web.config must be the same in the main app and the hub
            HttpCookie cookie = context.Request.Cookies["Hub_Session"];
            if (cookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(cookie.Value);
                try
                {
                    userID = int.Parse(authTicket.UserData.Split('|')[0]);
                    parentID = int.Parse(authTicket.UserData.Split('|')[1]);
                }
                catch (Exception)
                {
                    userID = -1;
                    parentID = -1;
                }
            }

            switch (GetSegment(context, 1))
            {
                case "kb":
                    ProcessKBSearch(context, term);
                    break;
                default:
                    break;
            }
        }

        private void ProcessKBSearch(HttpContext context, string term)
        {
            List<object> items = new List<object>();
            dynamic item = new ExpandoObject();

            dynamic item1 = new ExpandoObject();
            item1.id = 1;
            item1.num = 100;
            item1.name = "Sample Ticket 1";
            items.Add(item1);

            dynamic item2 = new ExpandoObject();
            item2.id = 2;
            item2.num = 200;
            item2.name = "Sample Ticket 2";
            items.Add(item2);

            dynamic result = new ExpandoObject();
            result.term = term;
            result.items = items.ToArray();

            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.AddHeader("Expires", "-1");
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Write(JsonConvert.SerializeObject(result));
        }



    }
}
