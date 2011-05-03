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

    public static string HtmlToText(string html)
    {
      if (string.IsNullOrEmpty(html))
      {
        return string.Empty;
      }
      else
      {
        string result = html;
        //Remove line breaks and spaces
        result = result.Replace("\r", " ");
        result = result.Replace("\n", " ");
        result = result.Replace("\t", string.Empty);
        result = Regex.Replace(result, " +", " ");
        
        //Remove the header, scripts, and styles
        result = Regex.Replace(result, "<head[^>]*>[\\s\\S]*?</head>", string.Empty, RegexOptions.IgnoreCase);
        result = Regex.Replace(result, "<script[^>]*>[\\s\\S]*?</script>", string.Empty, RegexOptions.IgnoreCase);
        result = Regex.Replace(result, "<style[^>]*>[\\s\\S]*?</style>", string.Empty, RegexOptions.IgnoreCase);
        result = Regex.Replace(result, "<!--[\\s\\S]*?-->", string.Empty, RegexOptions.IgnoreCase);

        //add tabs for table cells
        result = Regex.Replace(result, "<(td|th)[^>]*>", "\t", RegexOptions.IgnoreCase);
        //add new lines for some tags
        result = Regex.Replace(result, "<(br|li|h1|h2|h3|h4|h5|h6)[^>]*>", "\n", RegexOptions.IgnoreCase);
        //and double new lines for others
        result = Regex.Replace(result, "<(div|tr|p)[^>]*>", "\n\n", RegexOptions.IgnoreCase);

        //remove remaining tags 
        result = Regex.Replace(result, "<[^>]*>", string.Empty, RegexOptions.IgnoreCase);

        // replace special characters:
        result = Regex.Replace(result, @"\&[^;]+;", new MatchEvaluator(delegate(Match m)
        {
          return HttpContext.Current.Server.HtmlDecode(m.Value);
        }), RegexOptions.IgnoreCase);

        // Remove extra line breaks and spacing:
        result = Regex.Replace(result, "^[\\n\\s]+", string.Empty, RegexOptions.IgnoreCase);
        result = Regex.Replace(result, "[\\n\\s]+$", string.Empty, RegexOptions.IgnoreCase);
        result = Regex.Replace(result, "\\n[\\s]+\\n", "\n\n", RegexOptions.IgnoreCase);
        result = Regex.Replace(result, "\\n\\n[\\n]+", "\n\n", RegexOptions.IgnoreCase);

        return result;
      }
    }

  }
}
