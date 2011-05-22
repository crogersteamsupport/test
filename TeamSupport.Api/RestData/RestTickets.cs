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
  public class RestTickets
  {

    public static string GetTicket(RestCommand command, int ticketID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(command.LoginUser, ticketID);
      if (ticket.OrganizationID != command.Organization.OrganizationID)
      {
        throw new RestException(HttpStatusCode.Unauthorized);
      }

      return ticket.GetXml("Ticket", true);
    }

    public static string GetTickets(RestCommand command)
    {
      string xml = "";

      if (command.Filters["TicketTypeID"] != null)
      { 
        try 
	      {	        
          TicketsView tickets = new TicketsView(command.LoginUser);
          int ticketTypeID = int.Parse(command.Filters["TicketTypeID"]);
          TicketType ticketType = TicketTypes.GetTicketType(command.LoginUser, ticketTypeID);
          if (ticketType.OrganizationID != command.Organization.OrganizationID) throw new Exception();
          tickets.LoadByTicketTypeID(ticketTypeID);
          xml = tickets.GetXml("Tickets", "Ticket", true, command.Filters);
	      }
	      catch (Exception ex)
	      {
		      throw new RestException(HttpStatusCode.NotAcceptable, "Invalid TicketTypeID to filter.", ex);
	      }
      }
      else
	    {
        TicketTypes ticketTypes = new TicketTypes(command.LoginUser);
        ticketTypes.LoadByOrganizationID(command.Organization.OrganizationID);

        TicketsView tickets = new TicketsView(command.LoginUser);
        XmlTextWriter writer = Tickets.BeginXmlWrite("Tickets");

        foreach (TicketType ticketType in ticketTypes)
        {
          tickets.LoadByTicketTypeID(ticketType.TicketTypeID);
          //writer.WriteStartElement(ticketType.Name);
          foreach (DataRow row in tickets.Table.Rows)
          {
            tickets.WriteXml(writer, row, "Ticket", true, command.Filters);
          }
          //writer.WriteEndElement();
        }
        
        xml = Tickets.EndXmlWrite(writer);
	    }

      if (command.Format == RestFormat.XML)
      {
        //return tickets.GetXml("Tickets", "Ticket", command.Filters["TicketTypeID"] != null, command.Filters);
        return xml;
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }

    public static string GetTicketsByCustomerID(RestCommand command, int customerID)
    {
      TicketsView tickets = new TicketsView(command.LoginUser);

      tickets.LoadByCustomerID(customerID);

      if (command.Format == RestFormat.XML)
      {
        return tickets.GetXml("Tickets", "Ticket", command.Filters["TicketTypeID"] != null, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
    }

    public static string GetTicketsByContactID(RestCommand command, int contactID)
    {
      TicketsView tickets = new TicketsView(command.LoginUser);

      tickets.LoadByContactID(contactID);

      if (command.Format == RestFormat.XML)
      {
        return tickets.GetXml("Tickets", "Ticket", command.Filters["TicketTypeID"] != null, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
    }

    public static string CreateTicket(RestCommand command)
    {
      Tickets tickets = new Tickets(command.LoginUser);
      Ticket ticket = tickets.AddNewTicket();
      ticket.OrganizationID = command.Organization.OrganizationID;
      ticket.ReadFromXml(command.Data, true);
      ticket.Collection.Save();
      ticket.UpdateCustomFieldsFromXml(command.Data);

      Actions actions = new Actions(command.LoginUser);
      TeamSupport.Data.Action action = actions.AddNewAction();
      action.ActionTypeID = null;
      action.Name = "Description";
      action.SystemActionTypeID = SystemActionType.Description;
      action.Description = "";
      action.IsVisibleOnPortal = ticket.IsVisibleOnPortal;
      action.IsKnowledgeBase = ticket.IsKnowledgeBase;
      action.TicketID = ticket.TicketID;
      actions.Save();
      return TicketsView.GetTicketsViewItem(command.LoginUser, ticket.TicketID).GetXml("Ticket", true);
    }

    public static string UpdateTicket(RestCommand command, int ticketID)
    {
      Ticket ticket = Tickets.GetTicket(command.LoginUser, ticketID);
      if (ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      ticket.ReadFromXml(command.Data, false);
      ticket.Collection.Save();
      ticket.UpdateCustomFieldsFromXml(command.Data);
      ticket = Tickets.GetTicket(command.LoginUser, ticket.TicketID);
      return TicketsView.GetTicketsViewItem(command.LoginUser, ticket.TicketID).GetXml("Ticket", true);
    }


    // Customer Only Methods
    
    public static string GetCustomerTicket(RestCommand command, int ticketID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(command.LoginUser, ticketID);
      if (ticket.OrganizationID != command.Organization.ParentID || !ticket.GetIsCustomer(command.Organization.OrganizationID)) 
      {
        throw new RestException(HttpStatusCode.Unauthorized);
      }

      return ticket.GetXml("Ticket", true);
    }

    public static string GetCustomerTickets(RestCommand command)
    {

      TicketsView tickets = new TicketsView(command.LoginUser);

      if (command.Filters["TicketTypeID"] != null)
      {
        try
        {
          int ticketTypeID = int.Parse(command.Filters["TicketTypeID"]);
          TicketType ticketType = TicketTypes.GetTicketType(command.LoginUser, ticketTypeID);
          if (ticketType.OrganizationID != command.Organization.ParentID) throw new Exception();
          tickets.LoadByCustomerTicketTypeID(command.Organization.OrganizationID, ticketTypeID);
        }
        catch (Exception ex)
        {
          throw new RestException(HttpStatusCode.NotAcceptable, ex.Message);
          throw new RestException(HttpStatusCode.NotAcceptable, "Invalid TicketTypeID to filter.", ex);
        }
      }
      else
      {
        tickets.LoadByCustomerID(command.Organization.OrganizationID);
      }

      if (command.Format == RestFormat.XML)
      {
        return tickets.GetXml("Tickets", "Ticket", command.Filters["TicketTypeID"] != null, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
    }

    public static string CreateCustomerTicket(RestCommand command)
    {
      Tickets tickets = new Tickets(command.LoginUser);
      Ticket ticket = tickets.AddNewTicket();
      ticket.OrganizationID = (int)command.Organization.ParentID;
      ticket.ReadFromXml(command.Data, true);
      ticket.Collection.Save();
      ticket.UpdateCustomFieldsFromXml(command.Data);

      Actions actions = new Actions(command.LoginUser);
      TeamSupport.Data.Action action = actions.AddNewAction();
      action.ActionTypeID = null;
      action.Name = "Description";
      action.SystemActionTypeID = SystemActionType.Description;
      action.Description = "";
      action.IsVisibleOnPortal = ticket.IsVisibleOnPortal;
      action.IsKnowledgeBase = ticket.IsKnowledgeBase;
      action.TicketID = ticket.TicketID;
      actions.Save();

      tickets.AddOrganization(command.Organization.OrganizationID, ticket.TicketID);
      return TicketsView.GetTicketsViewItem(command.LoginUser, ticket.TicketID).GetXml("Ticket", true);
    }
    
    public static string UpdateCustomerTicket(RestCommand command, int ticketID)
    {
      Ticket ticket = Tickets.GetTicket(command.LoginUser, ticketID);
      if (ticket.OrganizationID != command.Organization.ParentID || !ticket.GetTicketView().GetIsCustomer(command.Organization.OrganizationID)) throw new RestException(HttpStatusCode.Unauthorized);
      ticket.ReadFromXml(command.Data, false);
      ticket.Collection.Save();
      ticket.UpdateCustomFieldsFromXml(command.Data);
      ticket = Tickets.GetTicket(command.LoginUser, ticket.TicketID);
      return TicketsView.GetTicketsViewItem(command.LoginUser, ticket.TicketID).GetXml("Ticket", true);
    }
  }
}
