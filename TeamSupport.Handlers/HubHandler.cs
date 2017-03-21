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
using dtSearch.Engine;

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

            if (data["Token"].ToString() != org.WebServiceID.ToString())
            {
                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.ClearContent();
                context.Response.End();
                return;
            }

            //if (data["UserID"] != null)
            //{
            //	userID = (int)data["UserID"];
            //	User user = Users.GetUser(LoginUser.Anonymous, userID);
            //	Organization customer = Organizations.GetOrganization(LoginUser.Anonymous, user.OrganizationID);
            //	if (customer.ParentID != parentID)
            //	{
            //		context.Response.ContentType = "text/plain";
            //		context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            //		context.Response.ClearContent();
            //		context.Response.End();
            //	}
            //}

            if (data["UserID"] != null)
            {
                userID = (int)data["UserID"];
                if (userID == 0) userID = -1;
            }
            else userID = -1;

            string searchTerm = data["q"];

            //Route to the proper method, passing ParentID and UserID (if unauthenticated -1)
            try
            {
                ProcessRoute(context, route, parentID, userID, searchTerm);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "text/html";
                context.Response.Write(ex.Message + "<br />" + ex.StackTrace);
            }
            context.Response.End();
        }

        private void ProcessRoute(HttpContext context, string route, int parentID, int userID, string searchTerm)
        {
            switch (route)
            {
                case "search/kb": ProcessKBSearch(context, parentID, userID, searchTerm); break;
                case "search/wiki": ProcessWikiSearch(context, parentID, userID, searchTerm); break;
                case "search/ticket": ProcessTicketSearch(context, parentID, userID, searchTerm); break;
                default:
                    break;
            }
        }


        #endregion

        private void SAMPLEProcessKBSearch(HttpContext context, int parentID, int userID)
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

        private void ProcessKBSearch(HttpContext context, int parentID, int userID, string searchTerm)
        {
            SearchResults kbResults = TicketsView.GetHubSearchKBResults(searchTerm, LoginUser.Anonymous, parentID);
            List<KBSearchItem> result = GetKBResults(kbResults, LoginUser.Anonymous, userID, parentID);
            WriteJson(context, result);
        }

        private List<KBSearchItem> GetKBResults(SearchResults results, LoginUser loginUser, int userID, int parentID)
        {
            List<KBSearchItem> items = new List<KBSearchItem>();
            int customerID = 0;
            User user = Users.GetUser(loginUser, userID);
            if (user != null) customerID = user.OrganizationID;

            for (int i = 0; i < results.Count; i++)
            {
                results.GetNthDoc(i);
                int ticketID = int.Parse(results.CurrentItem.Filename);
                if (ticketID > 0)
                {
                    TicketsView ticketsViewHelper = new TicketsView(loginUser);
                    ticketsViewHelper.LoadHubKBByID(ticketID, parentID, customerID);

                    if (ticketsViewHelper.Any())
                    {
                        KBSearchItem item = new KBSearchItem();
                        item.HitRating = results.CurrentItem.ScorePercent;
                        item.Article = ticketsViewHelper[0].GetProxy();

                        TicketRatings ratings = new TicketRatings(loginUser);
                        ratings.LoadByTicketID(ticketID);

                        if (ratings.Any())
                        {
                            TicketRating rating = ratings[0];
                            item.VoteRating = rating.ThumbsUp;
                        }

                        items.Add(item);
                    }
                }
            }
            return items;
        }

        private void ProcessWikiSearch(HttpContext context, int parentID, int userID, string searchTerm)
        {
            SearchResults wikiResults = WikiArticlesView.GetPortalSearchWikiResults(searchTerm, LoginUser.Anonymous, parentID);
            List<WikiSearchItem> result = GetWikiResults(wikiResults, LoginUser.Anonymous, userID, parentID);
            WriteJson(context, result);
        }

        private List<WikiSearchItem> GetWikiResults(SearchResults results, LoginUser loginUser, int userID, int parentID)
        {
            List<WikiSearchItem> items = new List<WikiSearchItem>();
            WikiArticlesView articles = new WikiArticlesView(loginUser);
            articles.LoadByOrganizationID(parentID);

            for (int i = 0; i < results.Count; i++)
            {
                results.GetNthDoc(i);
                int articleID = int.Parse(results.CurrentItem.Filename);
                if (articleID > 0)
                {
                    WikiSearchItem item = new WikiSearchItem();
                    item.HitRating = results.CurrentItem.ScorePercent;
                    item.Article = articles.FindByArticleID(articleID).GetProxy();
                    items.Add(item);
                }
            }
            return items;
        }

        private void ProcessTicketSearch(HttpContext context, int parentID, int userID, string searchTerm)
        {
            SearchResults ticketResults = TicketsView.GetHubSearchTicketResults(searchTerm, LoginUser.Anonymous, parentID);
            List<TicketSearchItem> result = GetTicketResults(ticketResults, LoginUser.Anonymous, userID, parentID);
            WriteJson(context, result);
        }

        private List<TicketSearchItem> GetTicketResults(SearchResults results, LoginUser loginUser, int userID, int parentID)
        {
            List<TicketSearchItem> items = new List<TicketSearchItem>();
            int customerID = 0;
            User user = Users.GetUser(loginUser, userID);
            if (user != null) customerID = user.OrganizationID;

            for (int i = 0; i < results.Count; i++)
            {
                results.GetNthDoc(i);
                int ticketID = int.Parse(results.CurrentItem.Filename);
                if (ticketID > 0)
                {
                    List<CustomPortalColumnProxy> portalColumns = CustomPortalColumns.GetDefaultColumns(parentID);

                    TicketLoadFilter filters = new TicketLoadFilter();
                    filters.CustomerID = loginUser.OrganizationID;
                    filters.IsVisibleOnPortal = true;
                    filters.ForumCategoryID = null;

                    TicketsView ticketsViewHelper = new TicketsView(loginUser);
                    ticketsViewHelper.LoadHubtickets(loginUser, parentID, filters, portalColumns, 0, 100000000);

                    if (ticketsViewHelper.Any())
                    {
                        TicketSearchItem item = new TicketSearchItem();
                        item.HitRating = results.CurrentItem.ScorePercent;

                        items.Add(item);
                    }
                }
            }
            return items;
        }

        #region classes

        public class TicketSearchItem
        {
            public int HitRating { get; set; }
            public TicketsViewItemProxy Ticket { get; set; }
        }

        public class KBSearchItem
        {
            public int HitRating { get; set; }
            public int? VoteRating { get; set; }
            public int Views { get; set; }
            public TicketsViewItemProxy Article { get; set; }
        }

        public class WikiSearchItem
        {
            public int HitRating { get; set; }
            public WikiArticlesViewItemProxy Article { get; set; }
        }

        #endregion


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
