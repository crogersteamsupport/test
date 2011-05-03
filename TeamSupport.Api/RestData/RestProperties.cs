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
  public class RestProperties
  {

    public static string GetActionTypes(RestCommand command)
    {
      ActionTypes items = new ActionTypes(command.LoginUser);
      items.LoadByOrganizationID(command.Organization.OrganizationID);
      return items.GetXml("ActionTypes", "ActionType", true, command.Filters);
    }

    public static string GetPhoneTypes(RestCommand command)
    {
      PhoneTypes items = new PhoneTypes(command.LoginUser);
      items.LoadByOrganizationID(command.Organization.OrganizationID);
      return items.GetXml("PhoneTypes", "PhoneType", true, command.Filters);
    }

    public static string GetTicketStatuses(RestCommand command)
    {
      TicketStatuses items = new TicketStatuses(command.LoginUser);
      items.LoadByOrganizationID(command.Organization.OrganizationID);
      return items.GetXml("TicketStatuses", "TicketStatus", true, command.Filters);
    }
    public static string GetTicketSeverities(RestCommand command)
    {
      TicketSeverities items = new TicketSeverities(command.LoginUser);
      items.LoadByOrganizationID(command.Organization.OrganizationID);
      return items.GetXml("TicketSeverities", "TicketSeverity", true, command.Filters);
    }

    public static string GetTicketTypes(RestCommand command)
    {
      TicketTypes items = new TicketTypes(command.LoginUser);
      items.LoadByOrganizationID(command.Organization.OrganizationID);
      return items.GetXml("TicketTypes", "TicketType", true, command.Filters);
    }

    public static string GetProductVersionStatuses(RestCommand command)
    {
      ProductVersionStatuses items = new ProductVersionStatuses(command.LoginUser);
      items.LoadByOrganizationID(command.Organization.OrganizationID);
      return items.GetXml("ProductVersionStatuses", "ProductVersionStatus", true, command.Filters);
    }

  }
}
