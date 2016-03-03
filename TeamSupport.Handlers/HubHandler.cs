using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using System.Web.Security;
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

            //SAMPLE: http://localhost/hub/1078/search/kb?q=search%20term
            //SAMPLE: http://localhost/hub/[PARENTID]/search/kb?q=[SEARCH_STRING]
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



        private void ProcessSearch(HttpContext context)
		{
            string term = context.Request.QueryString["q"];
            int userID = GetUserID(context);
            //userid will be > -1 if authenticated

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
            result.parentID = GetParentID(context);
            result.items = items.ToArray();


            // build an object w/ the results and write it to jason
            WriteJson(context, result);
        }


        #region Utility Methods
        private string GetSegment(HttpContext context, int index)
        {
            for (int i = 0; i < context.Request.Url.Segments.Length; i++)
            {
                if (context.Request.Url.Segments[i].ToLower() == "hub/")
                {
                    return context.Request.Url.Segments[index + i + 2].TrimEnd('/');
                }
            }
            return "";
        }

        private int GetParentID(HttpContext context)
        {
            return int.Parse(GetSegment(context, -1));
        }

        private int GetUserID(HttpContext context)
        {
            // the name of this cookie Hub_Session is set up on the hub, in the web.config.  
            // The same value of the machineKey in the web.config must be the same in the main app and the hub
            HttpCookie cookie = context.Request.Cookies["Support_Hub_Teamsupport"];
            if (cookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(cookie.Value);
                try
                {
                    return int.Parse(authTicket.UserData.Split('|')[0]);
                }
                catch (Exception)
                {
                    return -1;
                }
            }
            return -1;
        }

        private void WriteJson(HttpContext context, object payload)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.AddHeader("Expires", "-1");
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Write(JsonConvert.SerializeObject(payload));
        }

        #endregion



    }
}
