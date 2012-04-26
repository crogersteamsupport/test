using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace TeamSupport.Data
{

  public class HtmlCleaner
  {
    public static string CleanHtml(string html)
    {
      return RemoveScript(RemoveStyles(RemoveHeader(RemoveComments(html))));
    }

    public static string RemoveScript(string html)
    {
      if (!String.IsNullOrEmpty(html))
      {
        //this is a copy of the client-side strip scripts filter.
        //html = Regex.Replace(html, "<(SCRIPT)([^>]*)/>", "", RegexOptions.IgnoreCase);
        //html = Regex.Replace(html, "<(SCRIPT)([^>]*)>[\\s\\S]*?</(SCRIPT)([^>]*)>", "", RegexOptions.IgnoreCase);
        //html = Regex.Replace(html, "<script[^>]*>[\\s\\S]*?</script>", string.Empty, RegexOptions.IgnoreCase);
        html = Regex.Replace(html, "(<SCRIPT\b[^>]*>(.*?)</SCRIPT>)|(<SCRIPT\b[^>]*/>)", string.Empty, RegexOptions.IgnoreCase);
        html = Regex.Replace(html, "(<BASE\b[^>]*>(.*?)</BASE>)|(<BASE\b[^>]*/>)", string.Empty, RegexOptions.IgnoreCase);
        
      }
      return html;
    }

    public static string RemoveHeader(string html)
    {
       return Regex.Replace(html, "<head[^>]*>[\\s\\S]*?</head>", string.Empty, RegexOptions.IgnoreCase);
    }

    public static string RemoveStyles(string html)
    {
      return Regex.Replace(html, "<style[^>]*>[\\s\\S]*?</style>", string.Empty, RegexOptions.IgnoreCase);
    }

    public static string RemoveComments(string html)
    {
      return Regex.Replace(html, "<!--[\\s\\S]*?-->", string.Empty, RegexOptions.IgnoreCase);
    }

  }
}
