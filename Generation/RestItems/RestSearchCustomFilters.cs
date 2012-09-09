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
  
  public class RestSearchCustomFilters
  {
    public static string GetSearchCustomFilter(RestCommand command, int customFilterID)
    {
      SearchCustomFilter searchCustomFilter = SearchCustomFilters.GetSearchCustomFilter(command.LoginUser, customFilterID);
      if (searchCustomFilter.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return searchCustomFilter.GetXml("SearchCustomFilter", true);
    }
    
    public static string GetSearchCustomFilters(RestCommand command)
    {
      SearchCustomFilters searchCustomFilters = new SearchCustomFilters(command.LoginUser);
      searchCustomFilters.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return searchCustomFilters.GetXml("SearchCustomFilters", "SearchCustomFilter", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
