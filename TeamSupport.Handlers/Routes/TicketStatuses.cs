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
    public class TicketStatusesRoute : BaseRoute   
    {
        public static void GetTicketStatuses(HttpContext context)
        {
            int ticketTypeID;
            bool filterByType = int.TryParse(context.Request.QueryString["TicketTypeID"], out ticketTypeID);

            SqlCommand command = new SqlCommand();
            if (filterByType)
            {
                command.CommandText = "SELECT * FROM TicketStatuses WHERE TicketTypeID = @TicketTypeID AND OrganizationID = @OrganizationID";
                command.Parameters.AddWithValue("TicketTypeID", ticketTypeID);
            }
            else
            {
                command.CommandText = "SELECT * FROM TicketStatuses WHERE OrganizationID = @OrganizationID";
            }
            command.Parameters.AddWithValue("OrganizationID", TSAuthentication.OrganizationID);
            WriteCommand(context, command);
        }

        public static void GetTicketStatusesByID(HttpContext context, int id)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT * FROM TicketStatuses WHERE TicketStatusID = @TicketStatusID AND OrganizationID = @OrganizationID";
            command.Parameters.AddWithValue("TicketStatusID", id);
            command.Parameters.AddWithValue("OrganizationID", TSAuthentication.OrganizationID);
            WriteCommand(context, command);
        }
    }
}
