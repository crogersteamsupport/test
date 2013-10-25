using Newtonsoft.Json;
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
      if (_command.Format == RestFormat.XML || _command.Format == RestFormat.JSON)
      {
        if (_command.Format == RestFormat.JSON && _command.Data != string.Empty)
        {
          XmlDocument xmlData = new XmlDocument();
          xmlData = JsonConvert.DeserializeXmlNode(_command.Data);
          _command.Data = xmlData.InnerXml;
        }

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

        if (data != "attachment")
        {
          _command.Context.Response.ContentType = "text/xml";
          if (_command.Format == RestFormat.JSON)
          {
            _command.Context.Response.ContentType = "application/json";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n", string.Empty));
            if (_command.Method == HttpMethod.Get)
            {
              data = FixJsonFormatError(JsonConvert.SerializeXmlNode(doc), uriTemplate);
            }
            else
            {
              data =  JsonConvert.SerializeXmlNode(doc);
            }
          }
          _command.Context.Response.Write(data);
        }
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
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
          case "/tickets/{id}/actions/{id}/attachments/": data = RestAttachments.GetAttachments(_command, GetId(3)); break;
          case "/tickets/{id}/actions/{id}/attachments/{id}/": data = RestAttachments.GetAttachment(_command, GetId(5)); break;
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
          case "/customers/{id}/products/": data = RestOrganizationProducts.GetOrganizationProducts(_command, GetId(1)); break;
          case "/customers/{id}/products/{id}/": data = RestOrganizationProducts.GetOrganizationProductItems(_command, GetId(1), GetId(3)); break;
          case "/customers/{id}/products/{id}/customerproduct/{id}/": data = RestOrganizationProducts.GetOrganizationProductItem(_command, GetId(1), GetId(3), GetId(5)); break;
          case "/customers/{id}/products/{id}/versions/": data = RestVersions.GetOrganizationVersions(_command, GetId(3), GetId(1)); break;
          case "/customers/{id}/products/{id}/versions/{id}/": data = RestOrganizationProducts.GetOrganizationProductVersionItems(_command, GetId(1), GetId(3), GetId(5)); break;
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
          case "/products/{id}/customers/{id}/": data = RestOrganizationProducts.GetOrganizationProductItems(_command, GetId(3), GetId(1)); break;
          case "/products/{id}/versions/": data = RestVersions.GetItems(_command, GetId(1)); break;
          case "/products/{id}/versions/{id}/": data = RestVersions.GetItem(_command, GetId(3)); break;
          case "/products/{id}/versions/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.ProductVersions, GetId(3)); break;
          case "/products/{id}/versions/{id}/customers/": data = RestOrganizations.GetVersionOrganizations(_command, GetId(3)); break;
          case "/products/{id}/versions/{id}/customers/{id}/": data = RestOrganizationProducts.GetOrganizationProductVersionItems(_command, GetId(5), GetId(1), GetId(3)); break;
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
          case "/properties/knowledgebasecategories/": data = RestProperties.GetKnowledgeBaseCategories(_command); break;

          case "/zapier/tickets/": data = RestTickets.GetZapierTickets(_command); break;
          case "/zapier/tickets/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Tickets, GetId(2), true); break;
          case "/zapier/tickets/{id}/customers/": data = RestOrganizations.GetTicketOrganizations(_command, GetId(2), true); break;
          case "/zapier/tickets/{id}/contacts/": data = RestContacts.GetTicketContacts(_command, GetId(2), true); break;
          // TicketActions is already sorted by DateCreated DESC nevertheless I added a Zapier accesspoint for consistency.
          case "/zapier/tickets/{id}/actions/": data = RestActions.GetActions(_command, GetId(2)); break;
          case "/zapier/tickets/{id}/actions/{id}/attachments/": data = RestAttachments.GetAttachments(_command, GetId(4), true); break;
          case "/zapier/customers/": data = RestOrganizations.GetOrganizations(_command, true); break;
          case "/zapier/customers/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Organizations, GetId(2), true); break;
          case "/zapier/customers/{id}/tickets/": data = RestTickets.GetTicketsByCustomerID(_command, GetId(2), true); break;
          case "/zapier/customers/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Organizations, GetId(2), true); break;
          case "/zapier/customers/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Organizations, GetId(2), true); break;
          case "/zapier/customers/{id}/notes/": data = RestNotes.GetNotes(_command, ReferenceType.Organizations, GetId(2), true); break;
          case "/zapier/customers/{id}/contacts/": data = RestContacts.GetItems(_command, GetId(2), true); break;
          // I did not declared a trigger for customer-contact-addresses and customer-contact-phonenumbers in zapier because can be done with contact-addresses
          // and contact-phonenumbers respectively
          case "/zapier/customers/{id}/contacts/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Users, GetId(4), true); break;
          case "/zapier/customers/{id}/contacts/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Users, GetId(4), true); break;

          case "/zapier/customers/{id}/products/": data = RestOrganizationProducts.GetOrganizationProducts(_command, GetId(2), true); break;

          // I did not declared a trigger for customer-product and customer-product-versions in zapier because can be done with customer-products
          // and product-versions respectively.
          case "/zapier/customers/{id}/products/{id}/": data = RestOrganizationProducts.GetOrganizationProductItems(_command, GetId(2), GetId(4), true); break;
          case "/zapier/customers/{id}/products/{id}/versions/": data = RestVersions.GetOrganizationVersions(_command, GetId(4), GetId(2), true); break;

          case "/zapier/contacts/": data = RestContacts.GetItems(_command, true); break;
          case "/zapier/contacts/{id}/tickets/": data = RestTickets.GetTicketsByContactID(_command, GetId(2), true); break;
          case "/zapier/contacts/{id}/addresses/": data = RestAddresses.GetAddresses(_command, ReferenceType.Users, GetId(2), true); break;
          case "/zapier/contacts/{id}/phonenumbers/": data = RestPhoneNumbers.GetPhoneNumbers(_command, ReferenceType.Users, GetId(2), true); break;
          case "/zapier/products/": data = RestProducts.GetProducts(_command, true); break;
          case "/zapier/products/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Products, GetId(2), true); break;
          // ProductCustomers do a SELECT DISTINCT Organizations and cannot be sorted by OrganizationProducts.DateCreated DESC.
          //case "/zapier/products/{id}/customers/": data = RestOrganizations.GetProductOrganizations(_command, GetId(2), true); break;
          case "/zapier/products/{id}/versions/": data = RestVersions.GetItems(_command, GetId(2), true); break;
          case "/zapier/products/{id}/versions/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.ProductVersions, GetId(4), true); break;
          case "/zapier/products/{id}/versions/{id}/customers/": data = RestOrganizations.GetVersionOrganizations(_command, GetId(4), true); break;
          case "/zapier/groups/": data = RestGroups.GetGroups(_command, true); break;
          case "/zapier/users/": data = RestUsers.GetUsers(_command, true); break;
          case "/zapier/users/{id}/history/": data = RestActionLogs.GetItems(_command, ReferenceType.Users, GetId(2), true); break;
          case "/zapier/wiki/": data = RestWikiArticles.GetWikiArticles(_command, true); break;
          case "/zapier/properties/actiontypes/": data = RestProperties.GetActionTypes(_command, true); break;
          case "/zapier/properties/phonetypes/": data = RestProperties.GetPhoneTypes(_command, true); break;
          case "/zapier/properties/productversionstatuses/": data = RestProperties.GetProductVersionStatuses(_command, true); break;
          case "/zapier/properties/ticketseverities/": data = RestProperties.GetTicketSeverities(_command, true); break;
          case "/zapier/properties/ticketstatuses/": data = RestProperties.GetTicketStatuses(_command, true); break;
          case "/zapier/properties/tickettypes/": data = RestProperties.GetTicketTypes(_command, true); break;

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
          case "/tickets/{id}/actions/{id}/attachments/": data = RestAttachments.CreateAttachment(_command, GetId(1), GetId(3)); break;
          case "/customers/": data = RestOrganizations.CreateOrganization(_command); break;
          case "/customers/{id}/phonenumbers/": data = RestPhoneNumbers.AddPhoneNumber(_command, ReferenceType.Organizations, GetId(1)); break;
          case "/customers/{id}/addresses/": data = RestAddresses.AddAddress(_command, ReferenceType.Organizations, GetId(1)); break;
          case "/customers/{id}/notes/": data = RestNotes.AddNote(_command, ReferenceType.Organizations, GetId(1)); break;
          case "/customers/{id}/contacts/": data = RestContacts.CreateContact(_command, GetId(1)); break;
          case "/customers/{id}/products/{id}/": data = RestOrganizationProducts.CreateOrganizationProduct(_command, GetId(1), GetId(3)); break;
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
          case "/customers/{id}/products/{id}/customerproduct/{id}/": data = RestOrganizationProducts.UpdateOrganizationProductItem(_command, GetId(1), GetId(3), GetId(5)); break;
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
          case "/tickets/{id}/": RestTickets.DeleteTicket(_command, GetId(1)); break;
          case "/tickets/{id}/customers/{id}/": RestOrganizations.RemoveTicketOrganization(_command, GetId(1), GetId(3)); break;
          case "/tickets/{id}/contacts/{id}/": RestContacts.RemoveTicketContact(_command, GetId(1), GetId(3)); break;
          case "/customers/{id}/phonenumbers/{id}/": RestPhoneNumbers.RemovePhoneNumber(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
          case "/customers/{id}/addresses/{id}/": RestAddresses.RemoveAddress(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
          case "/customers/{id}/notes/{id}/": RestNotes.RemoveNote(_command, ReferenceType.Organizations, GetId(1), GetId(3)); break;
          //Commented in ticket 8867.
          //case "/customers/{id}/products/{id}/": data = RestProducts.RemoveOrganizationProduct(_command, GetId(1), GetId(3)); break;
          case "/customers/{id}/products/{id}/customerproduct/{id}/": data = RestOrganizationProducts.DeleteOrganizationProductItem(_command, GetId(1), GetId(3), GetId(5)); break;
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

    private string FixJsonFormatError(string input, string uriTemplate)
    {
      int squareBracketIndex = 0;
      string stringToRemove = GetStringToRemove(uriTemplate, ref squareBracketIndex);
      if (stringToRemove != string.Empty)
      {
        if (input.Substring(squareBracketIndex, 1) != "[")
        {
          input = input.Replace(stringToRemove, "[");
          input = input.Remove(input.Length - 2);
        }
        else
        {
          input = input.Replace(stringToRemove, string.Empty);
          input = input.Remove(input.Length - 3);
        }
        input = input + "]}";
      }
      return input;
    }

    private string GetStringToRemove(string uriTemplate, ref int squareBracketIndex)
    {
      string result = string.Empty;
      switch (uriTemplate)
      {
        case "/zapier/tickets/{id}/actions/": 
        case "/tickets/{id}/actions/":
          result = "{\"Action\":";
          squareBracketIndex = "{\"Actions\":{\"Action\":".Length;
          break;
        case "/zapier/properties/actiontypes/":
        case "/properties/actiontypes/":
          result = "{\"ActionType\":";
          squareBracketIndex = "{\"ActionTypes\":{\"ActionType\":".Length;
          break;
        case "/zapier/customers/{id}/addresses/":
        case "/zapier/customers/{id}/contacts/{id}/addresses/":
        case "/zapier/contacts/{id}/addresses/":
        case "/customers/{id}/addresses/":
        case "/customers/{id}/contacts/{id}/addresses/":
        case "/contacts/{id}/addresses/":
          result = "{\"Address\":";
          squareBracketIndex = "{\"Addresses\":{\"Address\":".Length;
          break;
        case "/zapier/tickets/{id}/actions/{id}/attachments/":
        case "/tickets/{id}/actions/{id}/attachments/":
          result = "{\"Attachment\":";
          squareBracketIndex = "{\"Attachments\":{\"Attachment\":".Length;
          break;
        case "/zapier/tickets/{id}/contacts/":
        case "/zapier/customers/{id}/contacts/":
        case "/zapier/contacts/":
        case "/tickets/{id}/contacts/":
        case "/customers/{id}/contacts/":
        case "/contacts/":
          result = "{\"Contact\":";
          squareBracketIndex = "{\"Contacts\":{\"Contact\":".Length;
          break;
        case "/zapier/tickets/{id}/customers/":
        case "/zapier/products/{id}/versions/{id}/customers/":
        case "/zapier/customers/":
        case "/tickets/{id}/customers/":
        case "/products/{id}/versions/{id}/customers/":
        case "/customers/":
          result = "{\"Customer\":";
          squareBracketIndex = "{\"Customers\":{\"Customer\":".Length;
          break;
        case "/zapier/groups/":
        case "/groups/":
          result = "{\"Group\":";
          squareBracketIndex = "{\"Groups\":{\"Group\":".Length;
          break;
        case "/zapier/tickets/{id}/history/":
        case "/zapier/customers/{id}/history/":
        case "/zapier/products/{id}/history/":
        case "/zapier/products/{id}/versions/{id}/history/":
        case "/zapier/users/{id}/history/":
        case "/tickets/{id}/history/":
        case "/customers/{id}/history/":
        case "/products/{id}/history/":
        case "/products/{id}/versions/{id}/history/":
        case "/users/{id}/history/":
          result = "{\"ActionItem\":";
          squareBracketIndex = "{\"History\":{\"ActionItem\":".Length;
          break;
        case "/zapier/customers/{id}/notes/":
        case "/customers/{id}/notes/":
          result = "{\"Note\":";
          squareBracketIndex = "{\"Notes\":{\"Note\":".Length;
          break;
        case "/zapier/customers/{id}/products/":
        case "/zapier/customers/{id}/products/{id}/":
        case "/customers/{id}/products/":
        case "/customers/{id}/products/{id}/":
          result = "{\"OrganizationProduct\":";
          squareBracketIndex = "{\"OrganizationProducts\":{\"OrganizationProduct\":".Length;
          break;
        case "/zapier/customers/{id}/phonenumbers/":
        case "/zapier/customers/{id}/contacts/{id}/phonenumbers/":
        case "/zapier/contacts/{id}/phonenumbers/":
        case "/customers/{id}/phonenumbers/":
        case "/customers/{id}/contacts/{id}/phonenumbers/":
        case "/contacts/{id}/phonenumbers/":
          result = "{\"PhoneNumber\":";
          squareBracketIndex = "{\"PhoneNumbers\":{\"PhoneNumber\":".Length;
          break;
        case "/zapier/properties/phonetypes/":
        case "/properties/phonetypes/":
          result = "{\"PhoneType\":";
          squareBracketIndex = "{\"PhoneTypes\":{\"PhoneType\":".Length;
          break;
        case "/zapier/products/":
        case "/products/":
          result = "{\"Product\":";
          squareBracketIndex = "{\"Products\":{\"Product\":".Length;
          break;
        case "/zapier/properties/productversionstatuses/":
        case "/properties/productversionstatuses/":
          result = "{\"ProductVersionStatus\":";
          squareBracketIndex = "{\"ProductVersionStatuses\":{\"ProductVersionStatus\":".Length;
          break;
        case "/zapier/tickets/":
        case "/zapier/customers/{id}/tickets/":
        case "/zapier/contacts/{id}/tickets/":
        case "/tickets/":
        case "/customers/{id}/tickets/":
        case "/contacts/{id}/tickets/":
          result = "{\"Ticket\":";
          squareBracketIndex = "{\"Tickets\":{\"Ticket\":".Length;
          break;
        case "/zapier/properties/ticketseverities/":
        case "/properties/ticketseverities/":
          result = "{\"TicketSeverity\":";
          squareBracketIndex = "{\"TicketSeverities\":{\"TicketSeverity\":".Length;
          break;
        case "/zapier/properties/ticketstatuses/":
        case "/properties/ticketstatuses/":
          result = "{\"TicketStatus\":";
          squareBracketIndex = "{\"TicketStatuses\":{\"TicketStatus\":".Length;
          break;
        case "/zapier/properties/tickettypes/":
        case "/properties/tickettypes/":
          result = "{\"TicketType\":";
          squareBracketIndex = "{\"TicketTypes\":{\"TicketType\":".Length;
          break;
        case "/zapier/users/":
        case "/users/":
          result = "{\"User\":";
          squareBracketIndex = "{\"Users\":{\"User\":".Length;
          break;
        case "/zapier/customers/{id}/products/{id}/versions/":
        case "/zapier/products/{id}/versions/":
        case "/customers/{id}/products/{id}/versions/":
        case "/products/{id}/versions/":
          result = "{\"Version\":";
          squareBracketIndex = "{\"Versions\":{\"Version\":".Length;
          break;
        case "/zapier/wiki/":
        case "/wiki/":
          result = "{\"Article\":";
          squareBracketIndex = "{\"Wiki\":{\"Article\":".Length;
          break;
      }
      return result; 
    }
  }
}
