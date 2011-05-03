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
  
  public class RestWikiArticles
  {
    public static string GetWikiArticle(RestCommand command, int articleID)
    {
      WikiArticlesViewItem wikiArticle = WikiArticlesView.GetWikiArticlesViewItem(command.LoginUser, articleID);
      if (wikiArticle.OrganizationID != command.Organization.OrganizationID)
      {
        throw new RestException(HttpStatusCode.Unauthorized);
      }

      if (wikiArticle.Private == true)
      {
        throw new RestException(HttpStatusCode.Forbidden, "Private wiki articles cannot be retrieved by the API");
      }

      return wikiArticle.GetXml("Article", true);
    }
    
    public static string GetWikiArticles(RestCommand command)
    {

      WikiArticlesView wikiArticles = new WikiArticlesView(command.LoginUser);
      wikiArticles.LoadByOrganizationID(command.Organization.OrganizationID);


      if (command.Format == RestFormat.XML)
      {
        return wikiArticles.GetXml("Wiki", "Article", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
