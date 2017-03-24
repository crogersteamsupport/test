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
    public class GroupsRoute : BaseRoute   
    {
        public static void GetGroups(HttpContext context)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT * FROM Groups WHERE OrganizationID = @OrganizationID";
            command.Parameters.AddWithValue("OrganizationID", TSAuthentication.OrganizationID);
            WriteCommand(context, command);
        }

        public static void GetGroupsByID(HttpContext context, int id)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT * FROM Groups WHERE GroupID = @GroupID AND OrganizationID = @OrganizationID";
            command.Parameters.AddWithValue("GroupID", id);
            command.Parameters.AddWithValue("OrganizationID", TSAuthentication.OrganizationID);
            WriteCommand(context, command);
        }
    }
}
