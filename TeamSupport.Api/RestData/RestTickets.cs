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

      return tickets.GetXml("Tickets", "Ticket", true, command.Filters);
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
      int? customerID = null;
      ticket.FullReadFromXml(command.Data, true, ref description, ref contactID, ref customerID);
      ticket.TicketSource = "API";
      ticket.NeedsIndexing = true;
      ticket.Collection.Save();
      ticket.UpdateCustomFieldsFromXml(command.Data);

      if (contactID != null)
      {
        ticket.Collection.AddContact((int)contactID, ticket.TicketID);
      }

      if (customerID != null)
      {
        ticket.Collection.AddOrganization((int)customerID, ticket.TicketID);
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
		UpdateFieldsOfSeparateTable(command, ticket.TicketID);

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
		TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumberForCustomer(command.LoginUser, (int)command.Organization.ParentID, ticketID);
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

		return tickets.GetXml("Tickets", "Ticket", true, command.Filters);
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

		/// <summary>
		/// Update the Ticket related fields that live in their own table.
		/// </summary>
		/// <param name="command">Command received in the request to read and process the data in the request body.</param>
		/// <param name="ticketId">TicketId to update its record.</param>
		private static void UpdateFieldsOfSeparateTable(RestCommand command, int ticketId)
		{
			try
			{
				//Add as necessary to the list and then to the switch-case below for the work to update it.
				List<string> fields = new List<string>() { "jirakey" };

				foreach (string field in fields.Select(p => p.ToLower()).ToList())
				{
					XmlDocument xml = new XmlDocument();
					xml.LoadXml(command.Data);
					string query = "*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='ticket']" +
									"/*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='{0}']";
					query = string.Format(query, field);
					XmlNode node = xml.SelectSingleNode(query);

					//If node not found and the request includes a top level of collection items then try again with it
					if (node == null)
					{
						query = "*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='tickets']" +
								"/*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='ticket']" +
								"/*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='{0}']";
						query = string.Format(query, field);
						XmlNodeList nodeList = xml.SelectNodes(query);

						if (nodeList != null && nodeList.Count > 0)
						{
							node = nodeList[0];
						}
					}

					if (node != null)
					{
						switch (field)
						{
							case "jirakey":
								string jiraKey = node.InnerText;
								TicketLinkToJira ticketLinkToJira = new TicketLinkToJira(command.LoginUser);
								ticketLinkToJira.LoadByTicketID(ticketId);

								if (ticketLinkToJira != null
									&& ticketLinkToJira.Any())
								{
									string oldJiraKey = ticketLinkToJira[0].JiraKey;
									ticketLinkToJira[0].JiraKey = jiraKey;
									ticketLinkToJira.Save();
									ActionLogs.AddActionLog(command.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticketId, string.Format("Changed JiraKey from '{0}' to '{1}'.", oldJiraKey, jiraKey));
								}
								break;
							default:
								break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				ExceptionLogs.LogException(command.LoginUser, ex, "API", string.Format("OrgID: {0}{1}Verb: {2}{1}Url: {3}{1}Body: {4}", command.Organization.OrganizationID, Environment.NewLine, command.Method, command.Method, command.Data));
			}
		}
	}
}
