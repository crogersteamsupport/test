using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using TeamSupport.Data;
using System.Net;
using System.Web.Security;

namespace TeamSupport.Api
{
  public class RestContacts
  {

    public static string GetItem(RestCommand command, int id)
    {
      ContactsViewItem item = ContactsView.GetContactsViewItem(command.LoginUser, id);
      if (item.OrganizationParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      return item.GetXml("Contact", true);
    }

    public static string GetItems(RestCommand command, bool orderByDateCreated = false)
    {
      ContactsView items = new ContactsView(command.LoginUser);
      if (orderByDateCreated)
      {
        items.LoadByParentOrganizationID(command.Organization.OrganizationID, "DateCreated DESC");
      }
      else
      {
        items.LoadByParentOrganizationID(command.Organization.OrganizationID);
      }
      return items.GetXml("Contacts", "Contact", true, command.Filters);
    }

    public static string GetItems(RestCommand command, int organizationID, bool orderByDateCreated = false)
    {
      if (Organizations.GetOrganization(command.LoginUser, organizationID).ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      ContactsView items = new ContactsView(command.LoginUser);
      if (orderByDateCreated)
      {
        items.LoadByOrganizationID(organizationID, "DateCreated DESC");
      }
      else
      {
        items.LoadByOrganizationID(organizationID);
      }
      return items.GetXml("Contacts", "Contact", true, command.Filters);
    }

    public static string GetTicketContacts(RestCommand command, int ticketID, bool orderByDateCreated = false)
    {
      Ticket ticket = Tickets.GetTicket(command.LoginUser, ticketID);
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      ContactsView contacts = new ContactsView(command.LoginUser);
      if (orderByDateCreated)
      {
        contacts.LoadByTicketID(ticketID, "ut.DateCreated DESC");
      }
      else
      {
        contacts.LoadByTicketID(ticketID);
      }
      return contacts.GetXml("Contacts", "Contact", true, command.Filters);
    }

    public static string AddTicketContact(RestCommand command, int ticketID, int contactID)
    {
      Ticket ticket = Tickets.GetTicket(command.LoginUser, ticketID);
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      User user = Users.GetUser(command.LoginUser, contactID);
      Organization organization = Organizations.GetOrganization(command.LoginUser, user.OrganizationID);

      if (organization == null || organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      Tickets tickets = new Tickets(command.LoginUser);
      tickets.AddContact(user.UserID, ticketID);
      return ContactsView.GetContactsViewItem(command.LoginUser, user.UserID).GetXml("Contact", true);
    }

    public static string RemoveTicketContact(RestCommand command, int ticketID, int contactID)
    {
      Ticket ticket = Tickets.GetTicket(command.LoginUser, ticketID);
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      User user = Users.GetUser(command.LoginUser, contactID);
      Organization organization = Organizations.GetOrganization(command.LoginUser, user.OrganizationID);
      if (organization == null || organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      Tickets tickets = new Tickets(command.LoginUser);
      tickets.RemoveOrganization(user.UserID, ticketID);
      return ContactsView.GetContactsViewItem(command.LoginUser, user.UserID).GetXml("Contact", true);
    }

    public static string CreateContact(RestCommand command, int organizationID)
    {
      Users users = new Users(command.LoginUser);
      User user = users.AddNewUser();
      user.ReadFromXml(command.Data, true);
      user.OrganizationID = organizationID;
      user.ActivatedOn = DateTime.UtcNow;
      user.LastLogin = DateTime.UtcNow;
      user.LastActivity = DateTime.UtcNow.AddHours(-1);
      user.IsPasswordExpired = true;
      user.Collection.Save();
      user.UpdateCustomFieldsFromXml(command.Data);
      user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(user.CryptedPassword, "MD5");
      return ContactsView.GetContactsViewItem(command.LoginUser, user.UserID).GetXml("Contact", true);
    }

    public static string UpdateContact(RestCommand command, int id)
    {
      User user = Users.GetUser(command.LoginUser, id);
      User oldUser = Users.GetUser(command.LoginUser, id);
      if (user == null) throw new RestException(HttpStatusCode.BadRequest);
      Organization organization = Organizations.GetOrganization(command.LoginUser, user.OrganizationID);
      if (organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      user.ReadFromXml(command.Data, false);
      if (user.CryptedPassword != oldUser.CryptedPassword) 
        user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(user.CryptedPassword, "MD5");
      user.Collection.Save();
      user.UpdateCustomFieldsFromXml(command.Data);
      return ContactsView.GetContactsViewItem(command.LoginUser, user.UserID).GetXml("Contact", true);
    }

    public static string DeleteContact(RestCommand command, int id)
    {
      ContactsViewItem user = ContactsView.GetContactsViewItem(command.LoginUser, id);
      if (user == null) throw new RestException(HttpStatusCode.BadRequest);
      Organization organization = Organizations.GetOrganization(command.LoginUser, user.OrganizationID);
      if (organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      string result = user.GetXml("Contact", true);
      Users.MarkUserDeleted(command.LoginUser, id);
      return result;
    }
  }
}
