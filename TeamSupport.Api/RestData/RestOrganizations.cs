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
  public class RestOrganizations
  {

    public static string GetOrganization(RestCommand command, int organizationID)
    {
      OrganizationsViewItem organization = OrganizationsView.GetOrganizationsViewItem(command.LoginUser, organizationID);
      if (organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      return organization.GetXml("Customer", true);
    }

    public static string GetOrganizations(RestCommand command, bool orderByDateCreated = false, int? limitNumber = null)
    {
      OrganizationsView organizations = new OrganizationsView(command.LoginUser);
      if (orderByDateCreated)
      {
        organizations.LoadByParentID(command.Organization.OrganizationID, true, "DateCreated DESC", limitNumber);
      }
      else
      {
        try
        {
          organizations.LoadByParentID(command.Organization.OrganizationID, true, command.Filters);
        }
        catch (Exception e)
        {
          organizations = new OrganizationsView(command.LoginUser);
          organizations.LoadByParentID(command.Organization.OrganizationID, true);
        }
      }
      return organizations.GetXml("Customers", "Customer", true, command.Filters);
    }

    public static string CreateOrganization(RestCommand command)
    {
      Addresses addresses = new Addresses(command.LoginUser);
      Address address = addresses.AddNewAddress();
      
      PhoneNumbers phoneNumbers = new PhoneNumbers(command.LoginUser);
      PhoneNumber phoneNumber = phoneNumbers.AddNewPhoneNumber();

      Organizations organizations = new Organizations(command.LoginUser);
      Organization organization = organizations.AddNewOrganization();
      organization.ParentID = command.Organization.OrganizationID;
      organization.IsActive = true;
      organization.FullReadFromXml(command.Data, true, ref phoneNumber, ref address);
      organization.NeedsIndexing = true;
      organization.Collection.Save();
      organization.UpdateCustomFieldsFromXml(command.Data);

      if (!String.IsNullOrEmpty(phoneNumber.Number) || !String.IsNullOrEmpty(phoneNumber.Extension))
      {
        phoneNumber.RefType = ReferenceType.Organizations;
        phoneNumber.RefID = organization.OrganizationID;
        phoneNumbers.Save();
      }

      if (!String.IsNullOrEmpty(address.Description)
      || !String.IsNullOrEmpty(address.Addr1)
      || !String.IsNullOrEmpty(address.Addr2)
      || !String.IsNullOrEmpty(address.Addr3)
      || !String.IsNullOrEmpty(address.City)
      || !String.IsNullOrEmpty(address.Country)
      || !String.IsNullOrEmpty(address.Description)
      || !String.IsNullOrEmpty(address.State)
      || !String.IsNullOrEmpty(address.Zip))
      {
        address.RefType = ReferenceType.Organizations;
        address.RefID = organization.OrganizationID;
        addresses.Save();
      }

      return OrganizationsView.GetOrganizationsViewItem(command.LoginUser, organization.OrganizationID).GetXml("Customer", true);
    }

    public static string UpdateOrganization(RestCommand command, int organizationID)
    {
      Organization organization = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (organization == null) throw new RestException(HttpStatusCode.BadRequest);
      if (organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      organization.ReadFromXml(command.Data, false);
      organization.Collection.Save();
      organization.UpdateCustomFieldsFromXml(command.Data);
      return OrganizationsView.GetOrganizationsViewItem(command.LoginUser, organization.OrganizationID).GetXml("Customer", true);
    }

    public static string DeleteOrganization(RestCommand command, int organizationID)
    {
      Organization organization = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (organization == null) throw new RestException(HttpStatusCode.BadRequest);
      if (organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      string result = organization.GetXml("Customer", true);
      Organizations.DeleteOrganizationAndAllReleatedData(command.LoginUser, organizationID);
      return result;
    }

    public static string GetTicketOrganizations(RestCommand command, int ticketIDOrNumber, bool orderByDateCreated = false)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      OrganizationsView organizations = new OrganizationsView(command.LoginUser);
      if (orderByDateCreated)
      {
        organizations.LoadByTicketID(ticket.TicketID, "ot.DateCreated DESC");
      }
      else
      {
        organizations.LoadByTicketID(ticket.TicketID);
      }
      return organizations.GetXml("Customers", "Customer", true, command.Filters);
    }

    public static string AddTicketOrganization(RestCommand command, int ticketIDOrNumber, int organizationID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Organization organization = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (organization == null || organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      Tickets tickets = new Tickets(command.LoginUser);
      tickets.AddOrganization(organizationID, ticket.TicketID);
      return OrganizationsView.GetOrganizationsViewItem(command.LoginUser, organizationID).GetXml("Customer", true);
    }

    public static string RemoveTicketOrganization(RestCommand command, int ticketIDOrNumber, int organizationID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Organization organization = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (organization == null || organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      Tickets tickets = new Tickets(command.LoginUser);
      tickets.RemoveOrganization(organizationID, ticket.TicketID);
      return OrganizationsView.GetOrganizationsViewItem(command.LoginUser, organizationID).GetXml("Customer", true);
    }

    public static string GetProductOrganizations(RestCommand command, int productID, bool orderByDateCreated = false)
    {
      Product item = Products.GetProduct(command.LoginUser, productID);
      if (item == null || item.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      OrganizationsView organizations = new OrganizationsView(command.LoginUser);
      if (orderByDateCreated)
      {
        organizations.LoadByProductID(productID, "op.DateCreated DESC");
      }
      else
      {
        organizations.LoadByProductID(productID);
      }
      return organizations.GetXml("Customers", "Customer", true, command.Filters);
    }

    public static string GetVersionOrganizations(RestCommand command, int versionID, bool orderByDateCreated = false)
    {
      ProductVersionsViewItem item = ProductVersionsView.GetProductVersionsViewItem(command.LoginUser, versionID);
      if (item == null || item.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      OrganizationsView organizations = new OrganizationsView(command.LoginUser);
      if (orderByDateCreated) 
      {
        organizations.LoadByVersionID(versionID, "op.DateCreated DESC");        
      }
      else
      {
        organizations.LoadByVersionID(versionID);
      }
      return organizations.GetXml("Customers", "Customer", true, command.Filters);
    }

  }
}
