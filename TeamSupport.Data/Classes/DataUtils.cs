using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace TeamSupport.Data
{
  public class DataUtils
  {

    private static string[] WordBreakString(string text)
    {
      List<string> list = new List<string>();
      text = text.Trim();
      char? qoute = null;
      StringBuilder builder = new StringBuilder();

      foreach (char c in text)
      {

        if (qoute == null)
        {
          if (c == ',' || c == ';') continue;

          if (c == ')' || c == '(')
          {
            if (builder.Length > 0)
            {
              list.Add(builder.ToString().Trim());
              builder.Remove(0, builder.Length);
            }
            list.Add(c.ToString());
          }
          else if (c == '"' || c == '\'')
          {
            qoute = c;
            if (builder.Length > 0)
            {
              list.Add(builder.ToString().Trim());
              builder.Remove(0, builder.Length);
            }
          }
          else if (c == ' ')
          {
            if (builder.Length > 0)
            {
              list.Add(builder.ToString().Trim());
              builder.Remove(0, builder.Length);
            }
          }
          else
          {
            builder.Append(c);
          }
        }
        else
        {
          if (c == qoute)
          {
            list.Add('"' + builder.ToString().Trim() + '"');
            builder.Remove(0, builder.Length);
            qoute = null;
          }
          else
          {
            builder.Append(c);
          }
        }
      }

      if (builder.Length > 0) list.Add(builder.ToString().Trim());

      return list.ToArray();
    }

    public static string BuildSearchString(string text, bool matchAll)
    {
      return BuildSearchString(text, matchAll, true);
    }

    public static string BuildSearchString(string text, bool matchAll, bool appendWildcard)
    {
      text = text.Replace("%20", " ");
      text = text.Replace("'", "\"");
      text = text.Replace(")", " ").Replace("(", " ");
      bool hasParenth = text.IndexOf("(") > -1;

      int quoteCount = 0;
      for (int i = 0; i < text.Length; i++)
      {
        if (text[i] == '"') quoteCount++;
      }

      if (quoteCount % 2 == 1) text = text + "\"";



      List<string> list = new List<string>();
      list.AddRange(WordBreakString(text));
      string con;

      StringBuilder builder = new StringBuilder();

      for (int i = 0; i < list.Count; i++)
      {
        if (list[i] == "(" || list[i] == ")")
        {
          builder.Append(list[i]);
          i++;
          if (i >= list.Count) return builder.ToString();
        }

        con = matchAll ? "AND" : "OR"; ;
        string conTest = list[i].ToUpper();

        if (conTest == "OR" || conTest == "AND")
        {
          con = conTest;
          i++;

          if (con == "AND" && list[i].ToUpper() == "NOT")
          {
            con = "AND NOT";
            i++;
          }
        }

        string term = list[i];
        if (appendWildcard && term.IndexOf('%') < 0 && term.IndexOf('"') < 0) term = term + '*';
        if (i != 0)
        {
          string parenthTest = builder.ToString();
          if ((parenthTest.Length > 0 && parenthTest[parenthTest.Length - 1] != '('))
          {
            builder.Append(" " + con + " ");
          }

          builder.Append(QuoteSearchTerm(term));
        }
        else
        {
          builder.Append(QuoteSearchTerm(term));
        }
      }

      return builder.ToString();
    }

    private static string QuoteSearchTerm(string text)
    {
      if (text.Length > 0 && text[0] != '"')
      {
       // if (text.IndexOf('*') > -1) return "\"" + text + "\"";
      }
      return text;
    }

    public static string ProductTypeString(ProductType type)
    {
      string result = "";
      switch (type)
      {
        case ProductType.Express:
          result = "Express";
          break;
        case ProductType.HelpDesk:
          result = "Support Desk";
          break;
        case ProductType.Enterprise:
          result = "Enterprise";
          break;
        case ProductType.BugTracking:
          result = "Bug Tracking";
          break;
        default:
          break;
      }
      return result;
    }

    public static string CustomFieldTypeString(CustomFieldType type)
    {
      string result = "";
      switch (type)
      {
        case CustomFieldType.Text: result = "Text"; break;
        case CustomFieldType.DateTime: result = "Date Time"; break;
        case CustomFieldType.Boolean: result = "True or False"; break;
        case CustomFieldType.Number: result = "Number"; break;
        case CustomFieldType.PickList: result = "Pick List"; break;
        default: break;
      }
      return result;
    }

    public static string GetCustomFieldColumns(LoginUser loginUser, ReferenceType refType, int? auxID, int organizationID, string refIDFieldName, int max, bool allowSpaces)
    {
      CustomFields fields = new CustomFields(loginUser);
      fields.LoadByReferenceType(organizationID, refType, auxID);
      int count = 0;
      StringBuilder builder = new StringBuilder();

      foreach (CustomField field in fields)
      {

        if (count >= max) break;
        builder.Append(", ");
        builder.Append(GetCustomFieldColumn(loginUser, field, refIDFieldName, allowSpaces));
        //,(SELECT CustomValue FROM CustomValues WHERE CustomFieldID = 246 AND RefID = op.OrganizationProductID) AS [Serial Number],

        /*builder.Append(", (SELECT CustomValue FROM CustomValues WHERE (CustomFieldID = ");
        builder.Append(field.CustomFieldID.ToString());
        builder.Append(") AND (RefID = ");
        builder.Append(refIDFieldName);
        builder.Append(")) AS [");
        if (allowSpaces)
          builder.Append(field.Name);
        else
          builder.Append(field.Name.Replace(' ', '_'));
        builder.Append("]");*/

        count++;
      }

      return builder.ToString();
    }

    public static string GetCustomFieldColumns(LoginUser loginUser, ReferenceType refType, int organizationID, string refIDFieldName, int max, bool allowSpaces)
    {
      return GetCustomFieldColumns(loginUser, refType, -1, organizationID, refIDFieldName, max, allowSpaces);
    }

    public static string GetCustomFieldColumn(LoginUser loginUser, CustomField field, string refIDFieldName, bool allowSpaces, bool writeAlias)
    {
      StringBuilder builder = new StringBuilder();
      builder.Append("(SELECT CAST(NULLIF(RTRIM(CustomValue), '') AS ");
      switch (field.FieldType)
      {
        case CustomFieldType.DateTime:
          builder.Append("datetime");
          break;
        case CustomFieldType.Boolean:
          builder.Append("bit");
          break;
        case CustomFieldType.Number:
          builder.Append("decimal");
          break;
        default:
          builder.Append("varchar(8000)");
          break;
      }
      builder.Append(") FROM CustomValues WHERE (CustomFieldID = ");
      builder.Append(field.CustomFieldID.ToString());
      builder.Append(") AND (RefID = ");
      builder.Append(refIDFieldName);
      builder.Append(")) ");
      

      //"(SELECT CustomValue FROM CustomValues WHERE (CustomFieldID = 654) AND (RefID = TicketsView.TicketID)) AS [Approved By Manager]"
      if (field.FieldType == CustomFieldType.PickList)
      {
        string[] items = field.ListValues.Split('|');
        if (items.Length > 0)
        {
          builder.Insert(0, "ISNULL(");
          builder.Append(", '" + items[0].Replace("'", "''") + "')");
        }
      }

      if (writeAlias)
      {
        builder.Append("AS [");
        if (allowSpaces)
          builder.Append(field.Name);
        else
          builder.Append(field.Name.Replace(' ', '_'));
        builder.Append("]");
      }
      return builder.ToString();
    }

    public static string GetCustomFieldColumn(LoginUser loginUser, CustomField field, string refIDFieldName, bool allowSpaces)
    {
      return GetCustomFieldColumn(loginUser, field, refIDFieldName, allowSpaces, true);
    }

    public static string GetReportPrimaryKeyFieldName(ReferenceType refType)
    {
      string result = "";
      switch (refType)
      {
        case ReferenceType.Organizations:
          result = "OrganizationsView.OrganizationID";
          break;
        case ReferenceType.Products:
          result = "Products.ProductID";
          break;
        case ReferenceType.ProductVersions:
          result = "ProductVersionsView.ProductVersionID";
          break;
        case ReferenceType.Users:
          result = "UsersView.UserID";
          break;
        case ReferenceType.Contacts:
          result = "ContactsView.UserID";
          break;
        case ReferenceType.Tickets:
          result = "TicketsView.TicketID";
          break;
        case ReferenceType.OrganizationProducts:
          result = "OrganizationProductsView.OrganizationProductID";
          break;
        default:
          result = "";
          break;
      }
      return result;

    }

    public static string FixStringFormat(string text)
    {
      return text.Replace("{", "{{").Replace("}", "}}");
    }

    public static DateTime DateToLocal(LoginUser loginUser, DateTime dateTime)
    {
      return TimeZoneInfo.ConvertTimeFromUtc(dateTime, loginUser.TimeZoneInfo);
    }

    public static DateTime? DateToLocal(LoginUser loginUser, DateTime? dateTime)
    {
      if (dateTime == null)
        return null;
      else
      {
        return DateToLocal(loginUser, (DateTime)dateTime);
      }
    }

    public static DateTime DateToUtc(LoginUser loginUser, DateTime dateTime)
    {
      return TimeZoneInfo.ConvertTimeToUtc(dateTime, loginUser.TimeZoneInfo);
    }

    public static DateTime? DateToUtc(LoginUser loginUser, DateTime? dateTime)
    {
      if (dateTime == null)
        return null;
      else
      {
        return DateToUtc(loginUser, (DateTime)dateTime);
      }
    }

    public static ReferenceType TableNameToReferenceType(string tableName)
    {
      ReferenceType result = ReferenceType.None;
      switch (tableName.ToLower().Trim())
      {
        case "actionlogs": result = ReferenceType.ActionLogs; break;
        case "actions": result = ReferenceType.Actions; break;
        case "actiontypes": result = ReferenceType.ActionTypes; break;
        case "addresses": result = ReferenceType.Addresses; break;
        case "attachments": result = ReferenceType.Attachments; break;
        case "billinginfo": result = ReferenceType.BillingInfo; break;
        case "creditcards": result = ReferenceType.CreditCards; break;
        case "crmlinkresults": result = ReferenceType.None; break;
        case "crmlinktable": result = ReferenceType.None; break;
        case "customfields": result = ReferenceType.CustomFields; break;
        case "customvalues": result = ReferenceType.CustomValues; break;
        case "emailactiontable": result = ReferenceType.None; break;
        case "emailposts": result = ReferenceType.None; break;
        case "exceptionlogs": result = ReferenceType.ExceptionLogs; break;
        case "groups": result = ReferenceType.Groups; break;
        case "groupusers": result = ReferenceType.GroupUsers; break;
        case "invoices": result = ReferenceType.Invoices; break;
        case "loginhistory": result = ReferenceType.None; break;
        case "notes": result = ReferenceType.None; break;
        case "organizationproducts": result = ReferenceType.OrganizationProducts; break;
        case "organizations": result = ReferenceType.Organizations; break;
        case "organizationsettings": result = ReferenceType.None; break;
        case "organizationtickets": result = ReferenceType.OrganizationTickets; break;
        case "phonenumbers": result = ReferenceType.PhoneNumbers; break;
        case "phonetypes": result = ReferenceType.PhoneTypes; break;
        case "portalloginhistory": result = ReferenceType.None; break;
        case "portaloptions": result = ReferenceType.None; break;
        case "products": result = ReferenceType.Products; break;
        case "productversions": result = ReferenceType.ProductVersions; break;
        case "productversionstatuses": result = ReferenceType.ProductVersionStatuses; break;
        case "reportdata": result = ReferenceType.None; break;
        case "reportfields": result = ReferenceType.None; break;
        case "reports": result = ReferenceType.None; break;
        case "reportsubcategories": result = ReferenceType.None; break;
        case "reporttablefields": result = ReferenceType.None; break;
        case "reporttables": result = ReferenceType.None; break;
        case "sourcecommitlog": result = ReferenceType.None; break;
        case "subscriptions": result = ReferenceType.Subscriptions; break;
        case "systemsettings": result = ReferenceType.SystemSettings; break;
        case "techdocs": result = ReferenceType.TechDocs; break;
        case "ticketnextstatuses": result = ReferenceType.TicketNextStatuses; break;
        case "ticketqueue": result = ReferenceType.TicketQueue; break;
        case "tickets": result = ReferenceType.Tickets; break;
        case "ticketseverities": result = ReferenceType.TicketSeverities; break;
        case "ticketstatuses": result = ReferenceType.TicketStatuses; break;
        case "tickettypes": result = ReferenceType.TicketTypes; break;
        case "tsemailignorelist": result = ReferenceType.None; break;
        case "users": result = ReferenceType.Users; break;
        case "usersettings": result = ReferenceType.UserSettings; break;
        case "actionsview": result = ReferenceType.Actions; break;
        case "contactsview": result = ReferenceType.Contacts; break;
        case "exceptionlogview": result = ReferenceType.ExceptionLogs; break;
        case "notesview": result = ReferenceType.None; break;
        case "organizationsview": result = ReferenceType.Organizations; break;
        case "phonenumbersview": result = ReferenceType.PhoneNumbers; break;
        case "productversionsview": result = ReferenceType.ProductVersions; break;
        case "ticketgridview": result = ReferenceType.Tickets; break;
        case "ticketsview": result = ReferenceType.Tickets; break;
        case "usersview": result = ReferenceType.Users; break;

        default: result = ReferenceType.None;
          break;
      }
      return result;
    }

    public static int GetIDFromXml(string data, string fieldName)
    {
      StringReader reader = new StringReader(data);
      DataSet dataSet = new DataSet();
      dataSet.ReadXml(reader);
      return (int)dataSet.Tables[0].Rows[0][fieldName];
    }

    public static bool IsReferenceValid(LoginUser loginUser, ReferenceType refType, int refID)
    {
      bool result = false;

      if (refType == ReferenceType.Organizations)
      {
        result = Organizations.GetOrganization(loginUser, refID).ParentID == loginUser.OrganizationID;
      }
      else if (refType == ReferenceType.Contacts || refType == ReferenceType.Users)
      {
        result = Users.GetUser(loginUser, refID).OrganizationID == loginUser.OrganizationID;
        if (!result) result = Organizations.GetOrganization(loginUser, Users.GetUser(loginUser, refID).OrganizationID).ParentID == loginUser.OrganizationID;
      }
      else if (refType == ReferenceType.Tickets)
      {
        result = Users.GetUser(loginUser, refID).OrganizationID == loginUser.OrganizationID;
        if (!result) result = Organizations.GetOrganization(loginUser, Users.GetUser(loginUser, refID).OrganizationID).ParentID == loginUser.OrganizationID;
      }
      return result;
    }

    public static string StripHtml(string text)
    {
      return text;
      /*
      string pattern = @"</?(?i:script|embed|object|frameset|frame|iframe|meta|link|style|body|html|div|style)(.|\n)*?>";//  @"<(.|\n)*?>";
      string result = text;

      result = Regex.Replace(result, pattern, string.Empty);
      //result = result.Replace("<", "&lt;").Replace(">", "&gt;");
      //result = result.Replace("&nbsp;", " ").Replace("&nbsp", " ");
      //result = HttpUtility.HtmlEncode(result);
      return result;*/
    }

    public static string GetBrowserName(string userAgent)
    {
      string browser = "Unknown";
      userAgent = userAgent.ToLower();

      if (userAgent.IndexOf("opera") > -1)
        browser = "Opera";
      else if (userAgent.IndexOf("konqueror") > -1)
        browser = "Konqueror";
      else if (userAgent.IndexOf("firefox") > -1)
        browser = "Firefox";
      else if (userAgent.IndexOf("netscape") > -1)
        browser = "Netscape";
      else if (userAgent.IndexOf("msie") > -1)
        browser = "Internet Explorer";
      else if (userAgent.IndexOf("chrome") > -1)
        browser = "Chrome";
      else if (userAgent.IndexOf("safari") > -1)
        browser = "Safari";

      return browser;
    }

    /// <summary>
    /// Converts a DataContract Object to Json
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>

    public static string ObjectToJson(object o)
    {
      DataContractJsonSerializer serializer = new DataContractJsonSerializer(o.GetType());
      MemoryStream stream = new MemoryStream();
      serializer.WriteObject(stream, o);
      string json = Encoding.Default.GetString(stream.ToArray());
      stream.Dispose();
      return json;
    }

    /// <summary>
    /// THIS IS NOT TESTED
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public static T JsonToObject<T>(string s)
    {
      T result = Activator.CreateInstance<T>();
      MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(s));
      DataContractJsonSerializer serializer = new DataContractJsonSerializer(result.GetType());
      result = (T)serializer.ReadObject(stream);
      stream.Close();
      stream.Dispose();
      return result;
    }


    public static string ObjectToString(object o)
    {
      LosFormatter formatter = new LosFormatter();
      StringWriter writer = new StringWriter();
      formatter.Serialize(writer, o);
      return writer.ToString();
    }

    public static object StringToObject(string value)
    {
      if (value == string.Empty) return null;

      try
      {
        LosFormatter formatter = new LosFormatter();
        return formatter.Deserialize(value);
      }
      catch
      {
        return null;
      }
    }

    public static string VerifyUniqueFileName(string directory, string fileName)
    {
      string path = Path.Combine(directory, fileName);
      string result = fileName;
      int i = 0;
      while (File.Exists(path))
      {
        i++;
        if (i > 20) break;
        string name = Path.GetFileNameWithoutExtension(fileName);
        string ext = Path.GetExtension(fileName);
        result = name + " (" + i.ToString() + ")" + ext;
        path = Path.Combine(directory, result);
      }

      return result;
    }

    public static string VerifyUniqueUrlFileName(string directory, string fileName)
    {
      string path = Path.Combine(directory, fileName);
      string result = fileName;
      int i = 0;
      while (File.Exists(path))
      {
        i++;
        if (i > 20) break;
        string name = Path.GetFileNameWithoutExtension(fileName);
        string ext = Path.GetExtension(fileName);
        result = name + "_" + i.ToString() + ext;
        path = Path.Combine(directory, result);
      }

      return result;
    }

    public static string GenerateRandomPassword()
    {
      Random random = new Random();
      string[] chars = { "abcdefgijkmnopqrstwxyz", "ABCDEFGHJKLMNPQRSTWXYZ", "123456789", "*$-+?_&=!%{}/" };
      StringBuilder builder = new StringBuilder();
      int length = random.Next(8, 12);

      for (int i = 0; i < length; i++)
      {
        string s = chars[random.Next(0, 3)];
        char c = s[random.Next(0, s.Length)];
        builder.Append(c);
      }

      return builder.ToString();
    }

    public static string GenerateRandomKey(int length)
    {
      RNGCryptoServiceProvider crytpo = new RNGCryptoServiceProvider();
      byte[] buffer = new byte[length / 2];
      crytpo.GetBytes(buffer);
      StringBuilder builder = new StringBuilder(length);
      for (int i = 0; i < buffer.Length; i++)
      {
        builder.Append(string.Format("{0:X2}", buffer[i]));
      }
      return builder.ToString();
    }

    public static CreditCardType GetCreditCardType(string cardNumber)
    {
      CreditCardType ccType = CreditCardType.Invalid;
      if (cardNumber.Trim().Length < 1) return ccType;

      byte[] number = new byte[16]; // number to validate

      // Remove non-digits
      int len = 0;
      for (int i = 0; i < cardNumber.Length; i++)
      {
        if (!char.IsDigit(cardNumber, i))
        {
          return ccType;
        }
        else
        {
          if (len == 16) return ccType; // number has too many digits
          number[len++] = byte.Parse(cardNumber[i].ToString());
        }
      }

      // Use Luhn Algorithm to validate
      int sum = 0;
      for (int i = len - 1; i >= 0; i--)
      {
        if (i % 2 == len % 2)
        {
          int n = number[i] * 2;
          sum += (n / 10) + (n % 10);
        }
        else
          sum += number[i];
      }

      if (sum % 10 == 0)
      {
        switch (cardNumber.Substring(0, 1))
        {
          case "3": ccType = CreditCardType.AmericanExpress; break;
          case "4": ccType = CreditCardType.Visa; break;
          case "5": ccType = CreditCardType.MasterCard; break;
          case "6": ccType = CreditCardType.Discover; break;
          default: ccType = CreditCardType.Unknown; break;
        }
      }

      return ccType;
    }

    public static string CreditCardTypeString(CreditCardType type)
    {
      string result = "";
      switch (type)
      {
        case CreditCardType.Unknown:
          result = "Unknown";
          break;
        case CreditCardType.Invalid:
          result = "Invalid";
          break;
        case CreditCardType.Visa:
          result = "Visa";
          break;
        case CreditCardType.MasterCard:
          result = "Master Card";
          break;
        case CreditCardType.Discover:
          result = "Discover";
          break;
        case CreditCardType.AmericanExpress:
          result = "American Express";
          break;
        case CreditCardType.BetaTest:
          result = "Beta Test";
          break;
        default:
          result = "ERROR";
          break;
      }
      return result;
    }

    public static string MinutesToDisplayTime(int minutes, string format)
    {
      bool isNeg = minutes < 0;
      string result = "0 Minutes";

      minutes = Math.Abs(minutes);

      if (minutes < 60)
      {
        result = minutes.ToString() + " Minutes";
      }
      else if (minutes < 24 * 60)
      {
        if (format == "0")
        {
          int i = minutes / 60;
          int j = minutes % 60;
          result = string.Format("{0} hours {1} minutes", i, j);
        }
        else
        {
          double d = minutes / (60.0);
          result = d.ToString(format) + " Hours";
        }
      }
      else
      {
        if (format == "0")
        {
          int m = minutes % 60;
          int h = (minutes / 60) % 24;
          int d = (minutes / 60 / 24);
          result = string.Format("{0} days {1} hours {2} minutes", d, h, m);
        }
        else
        {
          double d = minutes / (60.0 * 24.0);
          result = d.ToString(format) + " Days";
        }
      }

      if (isNeg) result = "- " + result;
      return result;
    }

    public static string MinutesToDisplayTime(int minutes)
    {
      return MinutesToDisplayTime(minutes, "0.00");
    }

    public static void LogException(LoginUser loginUser, Exception exception)
    {
      try
      {
        ExceptionLogs logs = new ExceptionLogs(loginUser);
        ExceptionLog log = logs.AddNewExceptionLog();


        log.Browser = HttpContext.Current.Request.Browser.Browser + HttpContext.Current.Request.Browser.Version;
        log.ExceptionName = exception.ToString();
        log.Message = exception.Message;
        log.PageInfo = "";
        log.StackTrace = exception.StackTrace;
        log.URL = "";
        logs.Save();

      }
      catch (Exception)
      {

      }
    }

    public static string DataRowToString(DataRow row)
    {
      if (row == null) return "[Row is NULL]";
      StringBuilder builder = new StringBuilder();
      string format = "<strong>{0}:</strong> \"{1}\"";
      foreach (DataColumn column in row.Table.Columns)
      {
        if (builder.Length > 0) builder.Append(", ");
        builder.Append(string.Format(format, column.ColumnName, row[column].ToString()));
      }
      return string.Format("[{0}] - {1}", row.Table.TableName, builder.ToString());
    }

    public static string RemoveHTMLTag(string[] tags, string text)
    {
      foreach (string tag in tags)
      {
        string theTag = tag.ToLower();
        int start = text.ToLower().IndexOf("<" + theTag);

        while (start > -1)
        {
          int end = text.ToLower().IndexOf("</" + theTag + ">");
          if (end < 0)
            end = text.Length;
          else
            end = end + theTag.Length + 3;

          end = end - start;

          text = text.Remove(start, end);
          start = text.ToLower().IndexOf("<" + theTag);
        }
      }
      return text;
    }

    public static string StripHTMLTag(string[] tags, string text)
    {
      foreach (string tag in tags)
      {
        string theTag = tag.ToLower();
        int start = text.ToLower().IndexOf("<" + theTag);

        while (start > -1)
        {
          int end = text.ToLower().IndexOf("</" + theTag + ">");
          if (end < 0)
            end = text.Length;
          else
            end = end + theTag.Length + 3;

          end = end - start;

          text = text.Remove(start, end);
          start = text.ToLower().IndexOf("<" + theTag);
        }
      }
      return text;
    }

    public static string GetMailLink(LoginUser loginUser, int userID, int ticketID)
    {
      string href = GetMailLinkHRef(loginUser, userID, ticketID);
      if (href == "") return "";

      return "<a class=\"mailLink\" href=\"" + href + "\">&nbsp</a>";
    }

    public static string GetMailLinkHRef(LoginUser loginUser, int userID, int ticketID)
    {
      User user = Users.GetUser(loginUser, userID);
      Organization organization = Organizations.GetOrganization(loginUser, loginUser.OrganizationID);
      Ticket ticket = Tickets.GetTicket(loginUser, ticketID);
      if (user == null || organization == null || ticket == null) return "";

      StringBuilder builder = new StringBuilder();
      builder.Append("mailto:" + user.Email);
      builder.Append("?cc=" + organization.GetReplyToAddress());
      builder.Append("&subject=" + HttpUtility.UrlPathEncode(ticket.Name).Replace("#", "%23") + " - [" + ticket.TicketNumber + "]");
      return builder.ToString();
    }

    public static bool ResetPassword(LoginUser loginUser, User user, bool isPortalUser)
    {
      string password = GenerateRandomPassword();
      user.IsPasswordExpired = true;
      user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
      try
      {
        if (isPortalUser)
          EmailPosts.SendResetPortalPassword(loginUser, user.UserID, password);
        else
          EmailPosts.SendResetTSPassword(loginUser, user.UserID, password);
        user.Collection.Save();
      }
      catch (Exception ex)
      {
        return false;
      }
      return true;
    }

    public static string ReplaceTicketLinks(string text)
    {
      int id = -1;
      while (true)
      {
        id = FindTicketID(text, "https://app.teamsupport.com/ticket.aspx?ticketid=");
        text = RemoveTicketTarget(text, "https://app.teamsupport.com/ticket.aspx?ticketid=");
        if (id < 0) break;
        string s = id.ToString();
        text = Regex.Replace(text, Regex.Escape("https://app.teamsupport.com/ticket.aspx?ticketid=" + s), "javascript:top.Ts.MainPage.openTicket(" + s + ",true);", RegexOptions.IgnoreCase);
      }

      while (true)
      {
        id = FindTicketID(text, "../?ticketid");
        text = RemoveTicketTarget(text, "../?ticketid");
        if (id < 0) break;
        string s = id.ToString();
        text = Regex.Replace(text, Regex.Escape("../?ticketid" + s), "javascript:top.Ts.MainPage.openTicket(" + s + ",true);", RegexOptions.IgnoreCase);
      }
      return text;
    }

    private static int FindTicketID(string text, string url)
    {
      text = text.ToLower();
      int i = text.IndexOf(url);
      if (i < 0) return -1;
      i += url.Length;
      int j = text.IndexOf('"', i);
      if (j < i) return -1;
      string s = text.Substring(i, j - i);
      try
      {
        return int.Parse(s);
      }
      catch
      {
        return -1;
      }
    }

    private static string RemoveTicketTarget(string text, string url)
    {
      string low = text.ToLower();
      int i = low.IndexOf(url);
      if (i - 20 < 0) return text;
      int j = low.IndexOf("_blank", i - 20);
      return text.Remove(j, 6);


    }

    public static string TableToCsv(DataTable table)
    {
      return TableToCsv(table, false);
    }

    public static string TableToCsv(DataTable table, bool replaceNewLineWithHtml)
    {
      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < table.Columns.Count; i++)
      {
        if (i > 0) builder.Append(",");
        builder.Append(table.Columns[i].ColumnName);
      }
      foreach (DataRow row in table.Rows)
      {
        builder.AppendLine();
        for (int i = 0; i < table.Columns.Count; i++)
        {
          if (i > 0) builder.Append(",");
          string value = row[i].ToString();
          if (value.Length > 8000) value = value.Substring(0, 8000);

          value = "\"" + value.Replace("\"", "\"\"") + "\"";
          if (replaceNewLineWithHtml) value = value.Replace(Environment.NewLine, "<br />");
          Encoding ascii = Encoding.GetEncoding("us-ascii", new EncoderReplacementFallback("*"), new DecoderReplacementFallback("*"));
          builder.Append(ascii.GetString(ascii.GetBytes(value)));
        }
      }
      return builder.ToString();
    }

    public static string CommandToCsv(LoginUser loginUser, SqlCommand command, bool replaceNewLineWithHtml)
    {
      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        connection.Open();
        command.Connection = connection;
        StringBuilder builder = new StringBuilder();
        SqlDataReader reader = command.ExecuteReader();

        for (int i = 0; i < reader.FieldCount; i++)
        {
          if (i > 0) builder.Append(",");
          builder.Append(reader.GetName(i));
        }

        while (reader.Read())
        {
          builder.AppendLine();
          for (int i = 0; i < reader.FieldCount; i++)
          {
            if (i > 0) builder.Append(",");
            string value = reader[i].ToString();
            if (value.Length > 8000) value = value.Substring(0, 8000);
            
            value = "\"" + value.Replace("\"", "\"\"") + "\"";
            if (replaceNewLineWithHtml) value = value.Replace(Environment.NewLine, "<br />");
            Encoding ascii = Encoding.GetEncoding("us-ascii", new EncoderReplacementFallback("*"), new DecoderReplacementFallback("*"));
            //Encoding utf8 = Encoding.GetEncoding("utf-8", new EncoderReplacementFallback("*"), new DecoderReplacementFallback("*"));
            builder.Append(ascii.GetString(ascii.GetBytes(value)));
          }
        }
        return builder.ToString();
      }
    }

    public static string ReplaceEx(string original, string pattern, string replacement)
    {
      int count, position0, position1;
      count = position0 = position1 = 0;
      string upperString = original.ToUpper();
      string upperPattern = pattern.ToUpper();
      int inc = (original.Length / pattern.Length) *
                (replacement.Length - pattern.Length);
      char[] chars = new char[original.Length + Math.Max(0, inc)];
      while ((position1 = upperString.IndexOf(upperPattern,
                                        position0)) != -1)
      {
        for (int i = position0; i < position1; ++i)
          chars[count++] = original[i];
        for (int i = 0; i < replacement.Length; ++i)
          chars[count++] = replacement[i];
        position0 = position1 + pattern.Length;
      }
      if (position0 == 0) return original;
      for (int i = position0; i < original.Length; ++i)
        chars[count++] = original[i];
      return new string(chars, 0, count);
    }

    public static string CreateLinks(string text, string caption, string href, string cssClass, string target)
    {
      try
      {
        string result = text;
        MatchCollection matches = Regex.Matches(result, "\\b" + caption + "\\b", RegexOptions.IgnoreCase);
        string link = "<a href=\"{0}\" class=\"{1}\" target=\"{2}\">{3}</a>";
        for (int i = matches.Count - 1; i >= 0; i--)
        {
          Match match = matches[i];

          string tag = GetParentHtmlTag(result, match.Index);
          if (tag != null && (tag == "top" || tag == "p" || tag == "div" || tag == "span"))
          {
            result = result.Remove(match.Index, match.Length).Insert(match.Index, string.Format(link, href, cssClass, target, match.Value));
          }
        }
        return result;
      }
      catch (Exception)
      {
        return text;
      }
    }

    public static string GetParentHtmlTag(string html, int startIndex)
    {
      bool closed = false;
      /*<a href="Ticket">Ticket</a><p>Ticket</p>*/
      for (int i = startIndex; i >= 0; i--)
      {
        char c = html[i];
        if (c == '<')
        {
          if (closed)
          {
            int j = i + 1;
            StringBuilder result = new StringBuilder();

            while (html[j] == ' ') j++;

            while (j < startIndex)
            {
              if (html[j] == ' ') break;
              result.Append(html[j]);
              j++;
            }

            return result.ToString().ToLower();

          }
          else
          {
            return null;
          }
        }
        else if (c == '>')
        {
          closed = true;
        }

      }
      return "top";
    }

  }
}
