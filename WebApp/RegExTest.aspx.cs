using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Web.Services;
using TeamSupport.Data;

public partial class RegExTest : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }

  [WebMethod(true)]
  public static string GetRegEx(string pattern, string replacement, string input)
  {
    return HttpUtility.HtmlEncode(Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase)); 
  }

  [WebMethod(true)]
  public static string Test(string pattern, string replacement, string input)
  {
    List<string> values = new List<string>();
    Match match = Regex.Match(input, @"\{\{Actions:\d*\}\}", RegexOptions.IgnoreCase);
    while (match.Success)
    {
      values.Add(match.Value);
      match = match.NextMatch();
    }

    StringBuilder builder = new StringBuilder();
    foreach (string value in values)
    {
      if (value.Length < 11) continue;
      int end = value.IndexOf('}');
      if (end < 11) continue;
      string output = value.Substring(10, end - 10);
      builder.Append("<br />X" + output +"X<br />");
    }

    return builder.ToString();



  }

}
