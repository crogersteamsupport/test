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
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net;



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

            int userID = -1;
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

            dynamic data = JObject.Parse(ReadJsonData(context));
            Organization org = Organizations.GetOrganization(LoginUser.Anonymous, parentID);

            if (data["Token"].ToString() != org.WebServiceID)
            {
                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.ClearContent();
                context.Response.End();
                return;
            }

            if (data["UserID"] != null)
            {
                userID = (int)data["UserID"];
                User user = Users.GetUser(LoginUser.Anonymous, userID);
                if (user.OrganizationID != parentID)
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Response.ClearContent();
                    context.Response.End();
                }
            }


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

        private void WriteJson(HttpContext context, object payload)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.AddHeader("Expires", "-1");
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Write(JsonConvert.SerializeObject(payload));
        }

        private static string ReadJsonData(HttpContext context)
        {
            string result = "";
            using (Stream receiveStream = context.Request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    result = readStream.ReadToEnd();
                }
            }
            return result;
        }

        #endregion



    }
}
