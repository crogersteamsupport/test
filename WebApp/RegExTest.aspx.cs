using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
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
    return DataUtils.CreateLinks(input, "Ticket", "http://google.com", "", "_blank");
  }
}
