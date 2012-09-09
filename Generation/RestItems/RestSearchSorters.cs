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
  
  public class RestSearchSorters
  {
    public static string GetSearchSorter(RestCommand command, int sorterID)
    {
      SearchSorter searchSorter = SearchSorters.GetSearchSorter(command.LoginUser, sorterID);
      if (searchSorter.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return searchSorter.GetXml("SearchSorter", true);
    }
    
    public static string GetSearchSorters(RestCommand command)
    {
      SearchSorters searchSorters = new SearchSorters(command.LoginUser);
      searchSorters.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return searchSorters.GetXml("SearchSorters", "SearchSorter", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
