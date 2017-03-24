using System;
using System.Collections.Generic;
using System.Web;
using System.Dynamic;
using TeamSupport.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace TeamSupport.Handlers
{
    public class RouteHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string[] segments = Routes.BaseRoute.GetSegments(context);
            try
            {
                switch (context.Request.HttpMethod.ToUpper())
                {
                    case "GET": ProcessGetRoute(context, segments); break;
                    case "POST": ProcessPostRoute(context, segments); break;
                    case "PUT": ProcessPutRoute(context, segments); break;
                    case "DELETE": ProcessDeleteRoute(context, segments); break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "text/html";
                if (context.IsDebuggingEnabled) context.Response.Write(ex.Message + "<br />" + ex.StackTrace); else context.Response.Write(ex.Message);
            }
            context.Response.End();
        }

        private void ProcessGetRoute(HttpContext context, string[] segments)
        {
            /*
             * Three types of routes
             * 
             * Query string will include filters
             * 
             * Examples for verb GET:
             * 
             * 1. /rt/Tickets  -- retrieves all tickets.  This would call Routes.TicketsRoute.GetTickets
             *  Method Called: Routes.[RouteName]Route.[Verb][RouteName]
             * 
             * 2. /rt/Tickets/1234  -- retrieves a single ticket based on id.  This would call Routes.TicketsRoute.GetTicketsByID
             *  Method Called: Routes.[RouteName]Route.[Verb][RouteName]ByID
             * 
             * 3. /rt/Tickets/Board -- retrieves custom json data based on "Board".  This will call Routes.TicketsRoute.GetTicketsBoard
             *  Method Called: Routes.[RouteName]Route.[Verb][MethodName]
             * 
             */

            Type type = Type.GetType("TeamSupport.Handlers.Routes." + segments[0] + "Route");
            if (type == null) throw new Exception("Type not implemented.");

            MethodInfo method;
            Object[] parms;

            int id;
            if (segments.Length == 1)
            {
                method = type.GetMethod("Get" + segments[0]);
                parms = new object[] { context };
            }
            else if (int.TryParse(segments[1], out id))
            {
                method = type.GetMethod("Get" + segments[0] + "ByID");
                parms = new object[] { context, id };
            }
            else
            {
                method = type.GetMethod("Get" + segments[1]);
                parms = new object[] { context };
            }

            if (method == null) throw new Exception("Method not implemented.");
            method.Invoke(null, parms);

        }

        private void ProcessPostRoute(HttpContext context, string[] segments)
        {

            /*
            * All routes can pass data and query string via context.
            * 
            * Examples for verb POST:
            * 
            * 1. /rt/Tickets  -- creates a new ticket.  This would call Routes.TicketsRoute.PostTickets
            *  Method Called: Routes.[RouteName]Route.Post[RouteName]
            *  Post data in body
            * 
            */

            Object[] parms = new object[] { context };
            Type type = Type.GetType("TeamSupport.Handlers.Routes." + segments[0] + "Route");
            if (type == null) throw new Exception("Type not implemented.");
            MethodInfo method = type.GetMethod("Post" + segments[0]); 
            if (method == null) throw new Exception("Method not implemented.");
            method.Invoke(null, parms);
        }

        private void ProcessPutRoute(HttpContext context, string[] segments)
        {
            /*
            * All routes can pass data and query string via context.
            * 
            * Examples for verb PUT:
            * 
            * 1. /rt/Tickets/1234  -- updates ticket by TicketID 1234.  This would call Routes.TicketsRoute.PutTickets
            *  Method Called: Routes.[RouteName]Route.Put[RouteName]
            *  Update data would be in body
            * 
            */

            int id = int.Parse(segments[1]);
            Object[] parms = new object[] { context, id };
            Type type = Type.GetType("TeamSupport.Handlers.Routes." + segments[0] + "Route");
            if (type == null) throw new Exception("Type not implemented.");
            MethodInfo method = type.GetMethod("Put" + segments[0]);
            if (method == null) throw new Exception("Method not implemented.");
            method.Invoke(null, parms);
        }

        private void ProcessDeleteRoute(HttpContext context, string[] segments)
        {
            /*
            * All routes can pass data and query string via context.
            * 
            * Examples for verb DELETE:
            * 
            * 1. /rt/Tickets/1234  -- deletes ticket by TicketID 1234.  This would call Routes.TicketsRoute.DeleteTickets
            *  Method Called: Routes.[RouteName]Route.Delete[RouteName]
            * 
            */

            int id = int.Parse(segments[1]);
            Object[] parms = new object[] { context, id };
            Type type = Type.GetType("TeamSupport.Handlers.Routes." + segments[0] + "Route");
            if (type == null) throw new Exception("Type not implemented.");
            MethodInfo method = type.GetMethod("Delete" + segments[0]);
            if (method == null) throw new Exception("Method not implemented.");
            method.Invoke(null, parms);
        }
    }
}
