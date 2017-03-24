using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TeamSupport.Data;
using Newtonsoft.Json;
using System.Dynamic;
using System.Data.SqlClient;
using TeamSupport.WebUtils;

namespace TeamSupport.Handlers.Routes
{
    public class TicketsRoute : BaseRoute   
    {
        public static void GetTickets(HttpContext context)
        {
            SqlCommand command = new SqlCommand();
            if (IsDisplay(context)) command.CommandText = "SELECT TicketID, TicketNumber, Name FROM Tickets WHERE OrganizationID = @OrganizationID";
            else command.CommandText = "SELECT * FROM Tickets WHERE OrganizationID = @OrganizationID";
            command.Parameters.AddWithValue("OrganizationID", TSAuthentication.OrganizationID);
            WriteCommand(context, command);
        }

        public static void GetTicketsByID(HttpContext context, int id)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT * FROM Tickets WHERE TicketID = @TicketID AND OrganizationID = @OrganizationID";
            command.Parameters.AddWithValue("TicketID", id);
            command.Parameters.AddWithValue("OrganizationID", TSAuthentication.OrganizationID);
            ExpandoObject[] tickets = SqlExecutor.GetExpandoObject(TSAuthentication.GetLoginUser(), command);

            command = new SqlCommand();
            command.CommandText = "SELECT Description FROM Actions WHERE TicketID = @TicketID";
            command.Parameters.AddWithValue("TicketID", id);
            ExpandoObject[] actions = SqlExecutor.GetExpandoObject(TSAuthentication.GetLoginUser(), command);

            (tickets[0] as dynamic).Actions = actions;
            WriteJson(context, tickets);
        }
    }
}
