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
            ExpandoObject[] tickets = SqlExecutor.GetExpandoObject(TSAuthentication.GetLoginUser(), command);
            WriteJson(context, tickets);
        }

    }
}
