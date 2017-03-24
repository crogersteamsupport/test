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
    public class TicketTypesRoute : BaseRoute   
    {
        public static void GetTicketTypes(HttpContext context)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT * FROM TicketTypes WHERE OrganizationID = @OrganizationID";
            command.Parameters.AddWithValue("OrganizationID", TSAuthentication.OrganizationID);
            WriteCommand(context, command);
        }

        public static void GetTicketTypesByID(HttpContext context, int id)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT * FROM TicketTypes WHERE TicketTypeID = @TicketTypeID AND OrganizationID = @OrganizationID";
            command.Parameters.AddWithValue("TicketTypeID", id);
            command.Parameters.AddWithValue("OrganizationID", TSAuthentication.OrganizationID);
            WriteCommand(context, command);
        }
    }
}
