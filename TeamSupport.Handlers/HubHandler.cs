using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using System.Web.Security;
using System.Dynamic;
using TeamSupport.Data;
using System.Data.SqlClient;



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

            int userID = GetUserID(context);
            int parentID = -1;


            //Parse the URL and get the route and ParentID
            StringBuilder routeBuilder = new StringBuilder();
            bool parentFlag = false;
            bool routeFlag = false;
            for (int i = 0; i < context.Request.Url.Segments.Length; i++)
            {
                if (parentFlag)
                {
                    parentID = int.Parse(context.Request.Url.Segments[i].TrimEnd('/'));
                    routeFlag = true;
                }
                else if (routeFlag)
                {
                    routeBuilder.Append(context.Request.Url.Segments[i]);
                }
                parentFlag = context.Request.Url.Segments[i].ToLower() == "hub/";
            }
            string route = routeBuilder.ToString().ToLower().TrimEnd('/');

            //Route to the proper method, passing ParentID and UserID (if unauthenticated -1)
            try
            {

                switch (route)
                {
                    case "search/kb": ProcessKBSearch(context, parentID, userID); break;
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

        private void ProcessKBSearch(HttpContext context, int parentID, int userID)
        {
            string term = context.Request.QueryString["q"];

            //Sample code to show you how to easily get SQL to JSON
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT Name, TicketNumber, TicketID FROM Tickets WHERE TicketID = @TicketID";
            command.Parameters.AddWithValue("TicketID", 1637556);
            ExpandoObject[] tickets = SqlExecutor.GetExpandoObject(LoginUser.Anonymous, command);

            command = new SqlCommand();
            command.CommandText = "SELECT Description FROM Actions WHERE TicketID = @TicketID";
            command.Parameters.AddWithValue("TicketID", 1637556);
            ExpandoObject[] actions = SqlExecutor.GetExpandoObject(LoginUser.Anonymous, command);

            (tickets[0] as dynamic).Actions = actions;

            dynamic result = new ExpandoObject();
            result.Term = term;
            result.UserID = userID;
            result.ParentID = parentID;
            result.Tickets = tickets;

            WriteJson(context, result);
        }


        #region Utility Methods

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
                    if (!authTicket.Expired) return int.Parse(authTicket.UserData.Split('|')[0]);
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
