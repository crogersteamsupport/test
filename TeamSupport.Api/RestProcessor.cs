using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using System.IO;
using System.Net;
using System.Xml;
using System.Data;

namespace TeamSupport.Api
{
  public class RestProcessor
  {
    RestCommand _command;

    public RestProcessor(RestCommand command)
    {
      _command = command;
    }

    public void Process()
    {
      string uriTemplate = GetURITemplate(_command.Segments);

      string data = "";
      switch (_command.Method)
      {
        case HttpMethod.Get: data = ProcessGet(uriTemplate); break;
        case HttpMethod.Put: data = ProcessPut(uriTemplate); break;
        case HttpMethod.Post: data = ProcessPost(uriTemplate); break;
        case HttpMethod.Delete: data = ProcessDelete(uriTemplate); break;
        case HttpMethod.Unsupported: throw new RestException(HttpStatusCode.MethodNotAllowed);
        default: throw new RestException(HttpStatusCode.MethodNotAllowed); 
      }

      _command.Context.Response.ContentType = "text/xml";
      _command.Context.Response.Write(data);
    }

    public string ProcessGet(string uriTemplate)
    {
      string data = "";
      _command.StatusCode = HttpStatusCode.OK;

      if (!_command.IsCustomerOnly)
      {
        switch (uriTemplate)
        {
          case "/tickets/": data = RestTickets.GetTickets(_command); break;
          case "/tickets/{id}/": data = RestTickets.GetTicket(_command, GetId(1)); break;
          case "/tickets/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Tickets, GetId(1)); break;
          case "/tickets/{id}/customers/": data = RestOrganizations.GetTicketOrganizations(_command, GetId(1)); break;
          case "/tickets/{id}/customers/{id}/": data = RestOrganizations.GetOrganization(_command, GetId(3)); break;
          case "/tickets/{id}/contacts/": data = RestContacts.GetTicketContacts(_command, GetId(1)); break;
          case "/tickets/{id}/contacts/{id}/": data = RestContacts.GetItem(_command, GetId(3)); break;
          case "/tickets/{id}/actions/": data = RestActions.GetActions(_command, GetId(1)); break;
          case "/tickets/{id}/actions/{id}/": data = RestActions.GetAction(_command, GetId(3)); break;
          case "/customers/": data = RestOrganizations.GetOrganizations(_command); break;
          case "/customers/{id}/": data = RestOrganizations.GetOrganization(_command, GetId(1)); break;
          case "/customers/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Organizations, GetId(1)); break;
          case "/customers/{id}/tickets/": data = RestTickets.GetTicketsByCustomerID(_command, GetId(1)); break;
          case "/customers/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Organizations, GetId(1)); break;
          case "/customers/{id}/phonenumbers/{id}/": data = RestPhoneNumbers.GetPhoneNumber(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
          case "/customers/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Organizations, GetId(1)); break;
          case "/customers/{id}/addresses/{id}/": data = RestAddresses.GetAddress(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
          case "/customers/{id}/notes/": data = RestNotes.GetNotes(_command, ReferenceType.Organizations, GetId(1)); break;
          case "/customers/{id}/notes/{id}/": data = RestNotes.GetNote(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
          case "/customers/{id}/contacts/": data = RestContacts.GetItems(_command, GetId(1)); break;
          case "/customers/{id}/contacts/{id}/": data = RestContacts.GetItem(_command, GetId(3)); break;
          case "/customers/{id}/contacts/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Users, GetId(3)); break;
          case "/customers/{id}/contacts/{id}/addresses/{id}/": data = RestAddresses.GetAddress(_command, ReferenceType.Users, GetId(3), GetId(5)); break;
          case "/customers/{id}/contacts/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Users, GetId(3)); break;
          case "/customers/{id}/contacts/{id}/phonenumbers/{id}/": data = RestPhoneNumbers.GetPhoneNumber(_command, ReferenceType.Users, GetId(3), GetId(5)); break;
          case "/customers/{id}/products/": data = RestProducts.GetOrganizationProducts(_command, GetId(1)); break;
          case "/customers/{id}/products/{id}/": data = RestProducts.GetProduct(_command, GetId(3)); break;
          case "/customers/{id}/products/{id}/versions/": data = RestVersions.GetOrganizationVersions(_command, GetId(3), GetId(1)); break;
          case "/customers/{id}/products/{id}/versions/{id}/": data = RestVersions.GetItem(_command, GetId(5)); break;
          case "/contacts/": data = RestContacts.GetItems(_command); break;
          case "/contacts/{id}/": data = RestContacts.GetItem(_command, GetId(1)); break;
          case "/contacts/{id}/tickets/": data = RestTickets.GetTicketsByContactID(_command, GetId(1)); break;
          case "/contacts/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Users, GetId(1)); break;
          case "/contacts/{id}/addresses/{id}/": data = RestAddresses.GetAddress(_command, ReferenceType.Users, GetId(1), GetId(3)); break;
          case "/contacts/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Users, GetId(1)); break;
          case "/contacts/{id}/phonenumbers/{id}/": data = RestPhoneNumbers.GetPhoneNumber(_command, ReferenceType.Users, GetId(1), GetId(3)); break;
          case "/products/": data = RestProducts.GetProducts(_command); break;
          case "/products/{id}/": data = RestProducts.GetProduct(_command, GetId(1)); break;
          case "/products/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Products, GetId(1)); break;
          case "/products/{id}/customers/": data = RestOrganizations.GetProductOrganizations(_command, GetId(1)); break;
          case "/products/{id}/customers/{id}/": data = RestOrganizations.GetOrganization(_command, GetId(3)); break;
          case "/products/{id}/versions/": data = RestVersions.GetItems(_command, GetId(1)); break;
          case "/products/{id}/versions/{id}/": data = RestVersions.GetItem(_command, GetId(3)); break;
          case "/products/{id}/versions/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.ProductVersions, GetId(3)); break;
          case "/products/{id}/versions/{id}/customers/": data = RestOrganizations.GetVersionOrganizations(_command, GetId(3)); break;
          case "/products/{id}/versions/{id}/customers/{id}/": data = RestOrganizations.GetOrganization(_command, GetId(5)); break;
          case "/groups/": data = RestGroups.GetGroups(_command); break;
          case "/groups/{id}/": data = RestGroups.GetGroup(_command, GetId(1)); break;
          case "/users/": data = RestUsers.GetUsers(_command); break;
          case "/users/{id}/": data = RestUsers.GetUser(_command, GetId(1)); break;
          case "/users/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Users, GetId(1)); break;
          case "/wiki/": data = RestWikiArticles.GetWikiArticles(_command); break;
          case "/wiki/{id}/": data = RestWikiArticles.GetWikiArticle(_command, GetId(1)); break;
          case "/properties/actiontypes/": data = RestProperties.GetActionTypes(_command); break;
          case "/properties/phonetypes/": data = RestProperties.GetPhoneTypes(_command); break;
          case "/properties/productversionstatuses/": data = RestProperties.GetProductVersionStatuses(_command); break;
          case "/properties/ticketseverities/": data = RestProperties.GetTicketSeverities(_command); break;
          case "/properties/ticketstatuses/": data = RestProperties.GetTicketStatuses(_command); break;
          case "/properties/tickettypes/": data = RestProperties.GetTicketTypes(_command); break;
          default: throw new RestException(HttpStatusCode.NotFound);
        }
      }
      else
      {
        switch (uriTemplate)
        {
          case "/tickets/": data = RestTickets.GetCustomerTickets(_command); break;
          case "/tickets/{id}/": data = RestTickets.GetCustomerTicket(_command, GetId(1)); break;
          //case "/tickets/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Tickets, GetId(1)); break;
          //case "/tickets/{id}/customers/": data = RestOrganizations.GetTicketOrganizations(_command, GetId(1)); break;
          //case "/tickets/{id}/customers/{id}/": data = RestOrganizations.GetOrganization(_command, GetId(3)); break;
          //case "/tickets/{id}/contacts/": data = RestContacts.GetTicketContacts(_command, GetId(1)); break;
          //case "/tickets/{id}/contacts/{id}/": data = RestContacts.GetItem(_command, GetId(3)); break;
          case "/tickets/{id}/actions/": data = RestActions.GetCustomerActions(_command, GetId(1)); break;
          case "/tickets/{id}/actions/{id}/": data = RestActions.GetCustomerAction(_command, GetId(3)); break;
          //case "/customers/": data = RestOrganizations.GetOrganizations(_command); break;
          //case "/customers/{id}/": data = RestOrganizations.GetOrganization(_command, GetId(1)); break;
          //case "/customers/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Organizations, GetId(1)); break;
          //case "/customers/{id}/tickets/": data = RestTickets.GetTicketsByCustomerID(_command, GetId(1)); break;
          //case "/customers/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Organizations, GetId(1)); break;
          //case "/customers/{id}/phonenumbers/{id}/": data = RestPhoneNumbers.GetPhoneNumber(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
          //case "/customers/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Organizations, GetId(1)); break;
          //case "/customers/{id}/addresses/{id}/": data = RestAddresses.GetAddress(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
          //case "/customers/{id}/notes/": data = RestNotes.GetNotes(_command, ReferenceType.Organizations, GetId(1)); break;
          //case "/customers/{id}/notes/{id}/": data = RestNotes.GetNote(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
          //case "/customers/{id}/contacts/": data = RestContacts.GetItems(_command, GetId(1)); break;
          //case "/customers/{id}/contacts/{id}/": data = RestContacts.GetItem(_command, GetId(3)); break;
          //case "/customers/{id}/contacts/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Users, GetId(3)); break;
          //case "/customers/{id}/contacts/{id}/addresses/{id}/": data = RestAddresses.GetAddress(_command, ReferenceType.Users, GetId(3), GetId(5)); break;
          //case "/customers/{id}/contacts/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Users, GetId(3)); break;
          //case "/customers/{id}/contacts/{id}/phonenumbers/{id}/": data = RestPhoneNumbers.GetPhoneNumber(_command, ReferenceType.Users, GetId(3), GetId(5)); break;
          //case "/customers/{id}/products/": data = RestProducts.GetOrganizationProducts(_command, GetId(1)); break;
          //case "/customers/{id}/products/{id}/": data = RestProducts.GetProduct(_command, GetId(3)); break;
          //case "/customers/{id}/products/{id}/versions/": data = RestVersions.GetOrganizationVersions(_command, GetId(3), GetId(1)); break;
          //case "/customers/{id}/products/{id}/versions/{id}/": data = RestVersions.GetItem(_command, GetId(5)); break;
          //case "/contacts/": data = RestContacts.GetItems(_command); break;
          //case "/contacts/{id}/": data = RestContacts.GetItem(_command, GetId(1)); break;
          //case "/contacts/{id}/tickets/": data = RestTickets.GetTicketsByContactID(_command, GetId(1)); break;
          //case "/contacts/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Users, GetId(1)); break;
          //case "/contacts/{id}/addresses/{id}/": data = RestAddresses.GetAddress(_command, ReferenceType.Users, GetId(1), GetId(3)); break;
          //case "/contacts/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Users, GetId(1)); break;
          //case "/contacts/{id}/phonenumbers/{id}/": data = RestPhoneNumbers.GetPhoneNumber(_command, ReferenceType.Users, GetId(1), GetId(3)); break;
          case "/products/": data = RestProducts.GetProducts(_command); break;
          case "/products/{id}/": data = RestProducts.GetProduct(_command, GetId(1)); break;
          //case "/products/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Products, GetId(1)); break;
          //case "/products/{id}/customers/": data = RestOrganizations.GetProductOrganizations(_command, GetId(1)); break;
          //case "/products/{id}/customers/{id}/": data = RestOrganizations.GetOrganization(_command, GetId(3)); break;
          case "/products/{id}/versions/": data = RestVersions.GetItems(_command, GetId(1)); break;
          case "/products/{id}/versions/{id}/": data = RestVersions.GetItem(_command, GetId(3)); break;
          //case "/products/{id}/versions/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.ProductVersions, GetId(3)); break;
          //case "/products/{id}/versions/{id}/customers/": data = RestOrganizations.GetVersionOrganizations(_command, GetId(3)); break;
          //case "/products/{id}/versions/{id}/customers/{id}/": data = RestOrganizations.GetOrganization(_command, GetId(5)); break;
          //case "/groups/": data = RestGroups.GetGroups(_command); break;
          //case "/groups/{id}/": data = RestGroups.GetGroup(_command, GetId(1)); break;
          //case "/users/": data = RestUsers.GetUsers(_command); break;
          //case "/users/{id}/": data = RestUsers.GetUser(_command, GetId(1)); break;
          //case "/users/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Users, GetId(1)); break;
          //case "/wiki/": data = RestWikiArticles.GetWikiArticles(_command); break;
          //case "/wiki/{id}/": data = RestWikiArticles.GetWikiArticle(_command, GetId(1)); break;
          case "/properties/actiontypes/": data = RestProperties.GetActionTypes(_command); break;
          case "/properties/phonetypes/": data = RestProperties.GetPhoneTypes(_command); break;
          case "/properties/productversionstatuses/": data = RestProperties.GetProductVersionStatuses(_command); break;
          case "/properties/ticketseverities/": data = RestProperties.GetTicketSeverities(_command); break;
          case "/properties/ticketstatuses/": data = RestProperties.GetTicketStatuses(_command); break;
          case "/properties/tickettypes/": data = RestProperties.GetTicketTypes(_command); break;
          default: throw new RestException(HttpStatusCode.NotFound);
        }

      }
      return data;


    }

    public string ProcessPost(string uriTemplate)
    {
      string data = "";
      _command.StatusCode = HttpStatusCode.Created;

      if (!_command.IsCustomerOnly)
      {
        switch (uriTemplate)
        {
          case "/tickets/": data = RestTickets.CreateTicket(_command); break;
          case "/tickets/{id}/customers/{id}/": data = RestOrganizations.AddTicketOrganization(_command, GetId(1), GetId(3)); break;
          case "/tickets/{id}/contacts/{id}/": data = RestContacts.AddTicketContact(_command, GetId(1), GetId(3)); break;
          case "/tickets/{id}/actions/": data = RestActions.CreateAction(_command, GetId(1)); break;
          case "/customers/": data = RestOrganizations.CreateOrganization(_command); break;
          case "/customers/{id}/phonenumbers/": data = RestPhoneNumbers.AddPhoneNumber(_command, ReferenceType.Organizations, GetId(1)); break;
          case "/customers/{id}/addresses/": data = RestAddresses.AddAddress(_command, ReferenceType.Organizations, GetId(1)); break;
          case "/customers/{id}/notes/": data = RestNotes.AddNote(_command, ReferenceType.Organizations, GetId(1)); break;
          case "/customers/{id}/contacts/": data = RestContacts.CreateContact(_command, GetId(1)); break;
          case "/contacts/{id}/addresses/": data = RestAddresses.AddAddress(_command, ReferenceType.Users, GetId(1)); break;
          case "/contacts/{id}/phonenumbers/": data = RestPhoneNumbers.AddPhoneNumber(_command, ReferenceType.Users, GetId(1)); break;
          case "/products/": data = RestProducts.CreateProduct(_command); break;
          case "/products/{id}/versions/": data = RestVersions.CreateVersion(_command, GetId(1)); break;
          case "/users/": data = RestUsers.CreateUser(_command); break;
          default: throw new RestException(HttpStatusCode.NotFound);
        }
      }
      else
      {
        switch (uriTemplate)
        {
          case "/tickets/": data = RestTickets.CreateCustomerTicket(_command); break;
          //case "/tickets/{id}/customers/{id}/": data = RestOrganizations.AddTicketOrganization(_command, GetId(1), GetId(3)); break;
          //case "/tickets/{id}/contacts/{id}/": data = RestContacts.AddTicketContact(_command, GetId(1), GetId(3)); break;
          case "/tickets/{id}/actions/": data = RestActions.CreateCustomerAction(_command, GetId(1)); break;
          //case "/customers/": data = RestOrganizations.CreateOrganization(_command); break;
          //case "/customers/{id}/phonenumbers/": data = RestPhoneNumbers.AddPhoneNumber(_command, ReferenceType.Organizations, GetId(1)); break;
          //case "/customers/{id}/addresses/": data = RestAddresses.AddAddress(_command, ReferenceType.Organizations, GetId(1)); break;
          //case "/customers/{id}/notes/": data = RestNotes.AddNote(_command, ReferenceType.Organizations, GetId(1)); break;
          //case "/customers/{id}/contacts/": data = RestContacts.CreateContact(_command, GetId(1)); break;
          //case "/contacts/{id}/addresses/": data = RestAddresses.AddAddress(_command, ReferenceType.Users, GetId(1)); break;
          //case "/contacts/{id}/phonenumbers/": data = RestPhoneNumbers.AddPhoneNumber(_command, ReferenceType.Users, GetId(1)); break;
          //case "/products/": data = RestProducts.CreateProduct(_command); break;
          //case "/products/{id}/versions/": data = RestVersions.CreateVersion(_command, GetId(1)); break;
          //case "/users/": data = RestUsers.CreateUser(_command); break;
          default: throw new RestException(HttpStatusCode.NotFound);
        }
      }
      return data;

    }

    public string ProcessPut(string uriTemplate)
    {
      string data = "";
      _command.StatusCode = HttpStatusCode.Accepted;
      if (!_command.IsCustomerOnly)
      {
        switch (uriTemplate)
        {
          case "/tickets/{id}/": data = RestTickets.UpdateTicket(_command, GetId(1)); break;
          case "/tickets/{id}/actions/{id}/": data = RestActions.UpdateAction(_command, GetId(3)); break;
          case "/customers/{id}/": data = RestOrganizations.UpdateOrganization(_command, GetId(1)); break;
          case "/customers/{id}/contacts/{id}/": data = RestContacts.UpdateContact(_command, GetId(3)); break;
          case "/customers/{id}/phonenumbers/{id}/": RestPhoneNumbers.UpdatePhoneNumber(_command, GetId(3)); break;
          case "/customers/{id}/addresses/{id}/": RestAddresses.UpdateAddress(_command, GetId(3)); break;
          case "/contacts/{id}/": data = RestContacts.UpdateContact(_command, GetId(1)); break;
          case "/contacts/{id}/phonenumbers/{id}/": RestPhoneNumbers.UpdatePhoneNumber(_command, GetId(3)); break;
          case "/contacts/{id}/addresses/{id}/": RestAddresses.UpdateAddress(_command, GetId(3)); break;
          case "/users/{id}/": data = RestUsers.UpdateUser(_command, GetId(1)); break;
          case "/products/{id}/": data = RestProducts.UpdateProduct(_command, GetId(1)); break;
          case "/products/{id}/versions/{id}/": data = RestVersions.UpdateVersion(_command, GetId(3)); break;
          default: throw new RestException(HttpStatusCode.NotFound);
        }
      }
      else
      {
        switch (uriTemplate)
        {
          case "/tickets/{id}/": data = RestTickets.UpdateCustomerTicket(_command, GetId(1)); break;
          case "/tickets/{id}/actions/{id}/": data = RestActions.UpdateCustomerAction(_command, GetId(3)); break;
          //case "/customers/{id}/": data = RestOrganizations.UpdateOrganization(_command, GetId(1)); break;
          //case "/customers/{id}/contacts/{id}/": data = RestContacts.UpdateContact(_command, GetId(3)); break;
          //case "/customers/{id}/phonenumbers/{id}/": RestPhoneNumbers.UpdatePhoneNumber(_command, GetId(3)); break;
          //case "/customers/{id}/addresses/{id}/": RestAddresses.UpdateAddress(_command, GetId(3)); break;
          //case "/contacts/{id}/": data = RestContacts.UpdateContact(_command, GetId(1)); break;
          //case "/contacts/{id}/phonenumbers/{id}/": RestPhoneNumbers.UpdatePhoneNumber(_command, GetId(3)); break;
          //case "/contacts/{id}/addresses/{id}/": RestAddresses.UpdateAddress(_command, GetId(3)); break;
          //case "/users/{id}/": data = RestUsers.UpdateUser(_command, GetId(1)); break;
          //case "/products/{id}/": data = RestProducts.UpdateProduct(_command, GetId(1)); break;
          //case "/products/{id}/versions/{id}/": data = RestVersions.UpdateVersion(_command, GetId(3)); break;
          default: throw new RestException(HttpStatusCode.NotFound);
        }
      }
      return data;

    }

    public string ProcessDelete(string uriTemplate)
    {
      string data = "";
      if (!_command.IsCustomerOnly)
      {
        switch (uriTemplate)
        {
          case "/tickets/{id}/customers/{id}/": RestOrganizations.RemoveTicketOrganization(_command, GetId(1), GetId(3)); break;
          case "/tickets/{id}/contacts/{id}/": RestContacts.RemoveTicketContact(_command, GetId(1), GetId(3)); break;
          case "/customers/{id}/phonenumbers/{id}/": RestPhoneNumbers.RemovePhoneNumber(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
          case "/customers/{id}/addresses/{id}/": RestAddresses.RemoveAddress(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
          case "/customers/{id}/notes/": RestNotes.RemoveNote(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
          case "/customers/{id}/": RestOrganizations.DeleteOrganization(_command, GetId(1)); break;
          case "/contacts/{id}/": RestContacts.DeleteContact(_command, GetId(1)); break;
          case "/contacts/{id}/phonenumbers/{id}/": RestPhoneNumbers.RemovePhoneNumber(_command, ReferenceType.Users, GetId(1), GetId(3)); break;
          case "/contacts/{id}/addresses/{id}/": RestAddresses.RemoveAddress(_command, ReferenceType.Users, GetId(1), GetId(3)); break;
          case "/users/{id}/": RestUsers.DeleteUser(_command, GetId(1)); break;
          default: throw new RestException(HttpStatusCode.NotFound);
        }
      }
      return data;

    }

    private int GetId(int index)
    {
      return int.Parse(_command.Segments[index]);
    }

    private string GetURITemplate(List<string> segments)
    {
      StringBuilder builder = new StringBuilder("/");

      foreach (string s in segments)
      {
        int id = -1;
        if (!int.TryParse(s, out id))
        {
          builder.Append(s + "/");
        }
        else
        {
          builder.Append("{id}/");
        }
      }

      return builder.ToString();

    }








  }
}
