using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Runtime.Serialization;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class WikiService : System.Web.Services.WebService
    {

      public WikiService() { }

      [WebMethod]
      public WikiArticleProxy GetWiki(int wikiID)
      {
        WikiArticle article = WikiArticles.GetWikiArticle(TSAuthentication.GetLoginUser(), wikiID);
        if (article != null) return article.GetProxy();
        return null;
      }

   }


}