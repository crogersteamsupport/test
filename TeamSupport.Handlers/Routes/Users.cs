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
    public class UsersRoute : BaseRoute   
    {
        public static void GetUsers(HttpContext context)
        {
            SqlCommand command = new SqlCommand();
            if (context.Request.QueryString["d"] == "1") command.CommandText = "SELECT FirstName, LastName, Email, UserID FROM Users WHERE OrganizationID = @OrganizationID";
            else command.CommandText = "SELECT * FROM Users WHERE OrganizationID = @OrganizationID";
            command.Parameters.AddWithValue("OrganizationID", TSAuthentication.OrganizationID);
            ExpandoObject[] users = SqlExecutor.GetExpandoObject(TSAuthentication.GetLoginUser() , command);
            WriteJson(context, users);
        }

        public static void GetUsersByID(HttpContext context, int id)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT * FROM Users WHERE UserID = @UserID AND OrganizationID = @OrganizationID";
            command.Parameters.AddWithValue("UserID", id);
            command.Parameters.AddWithValue("OrganizationID", TSAuthentication.OrganizationID);
            ExpandoObject[] users = SqlExecutor.GetExpandoObject(TSAuthentication.GetLoginUser(), command);
            WriteJson(context, users);
        }
    }
}
