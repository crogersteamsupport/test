using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using TeamSupport.Data;
using System.Net;

namespace TeamSupport.Api
{
  public class RestUsers
  {

    public static string GetUser(RestCommand command, int userID)
    {
      UsersViewItem item = UsersView.GetUsersViewItem(command.LoginUser, userID);
      if (item.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      return item.GetXml("User", true);
    }

    public static string GetUsers(RestCommand command)
    {
      UsersView items = new UsersView(command.LoginUser);
      items.LoadByOrganizationID(command.Organization.OrganizationID, false);
      return items.GetXml("Users", "User", true, command.Filters);
    }

    public static string CreateUser(RestCommand command)
    {

      if (Organizations.GetUserCount(command.LoginUser, command.LoginUser.OrganizationID) >= command.Organization.UserSeats)
      {
        throw new RestException(HttpStatusCode.Forbidden, "You are already at your maximum user seat count for your license.");
      }
      Users users = new Users(command.LoginUser);
      User user = users.AddNewUser();
      user.ReadFromXml(command.Data, true);
      user.OrganizationID = command.LoginUser.OrganizationID;
      user.ActivatedOn = DateTime.UtcNow;
      user.LastLogin = DateTime.UtcNow.AddHours(-1);
      user.LastActivity = DateTime.UtcNow.AddHours(-1);
      user.Collection.Save();
      user.UpdateCustomFieldsFromXml(command.Data);
      return UsersView.GetUsersViewItem(command.LoginUser, user.UserID).GetXml("User", true);
    }

    public static string UpdateUser(RestCommand command, int userID)
    {
      User user = Users.GetUser(command.LoginUser, userID);
      if (user == null) throw new RestException(HttpStatusCode.BadRequest);
      Organization organization = Organizations.GetOrganization(command.LoginUser, user.OrganizationID);
      if (organization.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      user.ReadFromXml(command.Data, false);
      user.Collection.Save();
      user.UpdateCustomFieldsFromXml(command.Data);
      return UsersView.GetUsersViewItem(command.LoginUser, user.UserID).GetXml("User", true);
    }

    public static string DeleteUser(RestCommand command, int userID)
    {
      UsersViewItem user = UsersView.GetUsersViewItem(command.LoginUser, userID);
      if (user == null) throw new RestException(HttpStatusCode.BadRequest);
      Organization organization = Organizations.GetOrganization(command.LoginUser, user.OrganizationID);
      if (organization.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      string result = user.GetXml("User", true);
      Users.MarkUserDeleted(command.LoginUser, userID);
      return result;
    }


  }
}
