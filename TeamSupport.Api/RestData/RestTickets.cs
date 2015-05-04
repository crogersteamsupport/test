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
      TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketID);
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
          
          try
          {
            tickets.LoadByTicketTypeID(ticketTypeID, command.Organization.OrganizationID, command.Filters);
          }
          catch (Exception ex)
          {
            //if something fails use the old method
            tickets.LoadByTicketTypeID(ticketTypeID);
          }
          
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
          try
		      {
            tickets.LoadByTicketTypeID(ticketType.TicketTypeID, command.Organization.OrganizationID, command.Filters);
          }
          catch (Exception ex)
          {
            //if something fails use the old method
            tickets.LoadByTicketTypeID(ticketType.TicketTypeID);
          }
          
          foreach (DataRow row in tickets.Table.Rows)
          {
            tickets.WriteXml(writer, row, "Ticket", true, command.Filters);
          }
        }
        
        xml = Tickets.EndXmlWrite(writer);
	    }

      return xml;
    }

    public static string GetZapierTickets(RestCommand command, int limitNumber)
    {
      string xml = "";

      TicketsView tickets = new TicketsView(command.LoginUser);
      XmlTextWriter writer = Tickets.BeginXmlWrite("Tickets");

      tickets.LoadByOrganizationIDOrderByNumberDESC(command.Organization.OrganizationID, limitNumber);
      foreach (DataRow row in tickets.Table.Rows)
      {
        tickets.WriteXml(writer, row, "Ticket", true, command.Filters);
      }

      xml = Tickets.EndXmlWrite(writer);

      //return tickets.GetXml("Tickets", "Ticket", command.Filters["TicketTypeID"] != null, command.Filters);
      return xml;
    }

    public static string GetTicketsByCustomerID(RestCommand command, int customerID, bool orderByDateCreated = false)
    {
      TicketsView tickets = new TicketsView(command.LoginUser);
      if (orderByDateCreated)
      {
        tickets.LoadByCustomerID(customerID, "ot.DateCreated DESC");
      }
      else
      {
        tickets.LoadByCustomerID(customerID);
      }

      return tickets.GetXml("Tickets", "Ticket", command.Filters["TicketTypeID"] != null, command.Filters);
    }

    public static string GetTicketsByContactID(RestCommand command, int contactID, bool orderByDateCreated = false)
    {
      TicketsView tickets = new TicketsView(command.LoginUser);

      if (orderByDateCreated)
      {
        tickets.LoadByContactID(contactID, "ut.DateCreated DESC");
      }
      else
      {
        tickets.LoadByContactID(contactID);
      }

      return tickets.GetXml("Tickets", "Ticket", command.Filters["TicketTypeID"] != null, command.Filters);
    }

    public static string CreateTicket(RestCommand command)
    {
      Tickets tickets = new Tickets(command.LoginUser);
      Ticket ticket = tickets.AddNewTicket();
      ticket.OrganizationID = command.Organization.OrganizationID;
      string description = string.Empty;
      int? contactID = null;
      ticket.FullReadFromXml(command.Data, true, ref description, ref contactID);
      ticket.TicketSource = "API";
      ticket.NeedsIndexing = true;
      ticket.Collection.Save();
      ticket.UpdateCustomFieldsFromXml(command.Data);

      if (contactID != null)
      {
        ticket.Collection.AddContact((int)contactID, ticket.TicketID);
      }

      Actions actions = new Actions(command.LoginUser);
      TeamSupport.Data.Action action = actions.AddNewAction();
      action.ActionTypeID = null;
      action.Name = "Description";
      action.SystemActionTypeID = SystemActionType.Description;
      action.Description = description;
      action.IsVisibleOnPortal = ticket.IsVisibleOnPortal;
      action.IsKnowledgeBase = ticket.IsKnowledgeBase;
      action.TicketID = ticket.TicketID;
      actions.Save();
      return TicketsView.GetTicketsViewItem(command.LoginUser, ticket.TicketID).GetXml("Ticket", true);
    }

    public static string UpdateTicket(RestCommand command, int ticketIDOrNumber)
    {
      TicketsViewItem ticketViewItem = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);
      Ticket ticket = Tickets.GetTicket(command.LoginUser, ticketViewItem.TicketID);
      if (ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      ticket.ReadFromXml(command.Data, false);
      ticket.Collection.Save();
      ticket.UpdateCustomFieldsFromXml(command.Data);
      ticket = Tickets.GetTicket(command.LoginUser, ticket.TicketID);
      return TicketsView.GetTicketsViewItem(command.LoginUser, ticket.TicketID).GetXml("Ticket", true);
    }

    public static string DeleteTicket(RestCommand command, int ticketIDOrNumber)
    {
      TicketsViewItem ticketViewItem = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);
      Ticket ticket = Tickets.GetTicket(command.LoginUser, ticketViewItem.TicketID);
      string result = ticketViewItem.GetXml("Ticket", true);
      if (ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      ticket.Delete();
      ticket.Collection.Save();
      return result;
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

      return tickets.GetXml("Tickets", "Ticket", command.Filters["TicketTypeID"] != null, command.Filters);
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

    public static string GetRelatedTickets(RestCommand command, int ticketIDOrTicketNumber)
    {
      TicketsView tickets = new TicketsView(command.LoginUser);
      tickets.LoadRelated(ticketIDOrTicketNumber);
      if (tickets.IsEmpty)
      {
        tickets = new TicketsView(command.LoginUser);
        tickets.LoadRelatedByTicketNumber(ticketIDOrTicketNumber, command.Organization.OrganizationID);
      }
      if (tickets.Count > 0 && tickets[0].OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      return tickets.GetXml("Tickets", "Ticket", true, command.Filters);
    }

    public static string GetTicketsByAssetID(RestCommand command, int assetID, bool orderByDateCreated = false)
    {
      TicketsView tickets = new TicketsView(command.LoginUser);
      if (orderByDateCreated)
      {
        tickets.LoadByAssetID(assetID, "at.DateCreated DESC");
      }
      else
      {
        tickets.LoadByAssetID(assetID);
      }

      return tickets.GetXml("Tickets", "Ticket", command.Filters["TicketTypeID"] != null, command.Filters);
    }
  }
}
