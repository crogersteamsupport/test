using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace TeamSupport.Data
{
  public partial class EmailTemplate
  {
    private int _organizationID = -1;

    public EmailTemplate ReplaceActions(TicketsViewItem ticket, bool publicOnly)
    {
      return ReplaceActions(ticket, publicOnly, true);
    }

    public EmailTemplate ReplaceActions(TicketsViewItem ticket, bool publicOnly, bool isIncluded)
    {
      if (!isIncluded) 
      {
        ReplaceParameter("Actions", "");
      }
      else
      {
        ReplaceParameter("Actions", GetActionsText(ticket, publicOnly, 0));
      }
      
      List<string> values = new List<string>();

      Match match = Regex.Match(Body, @"\{\{Actions:\d*\}\}", RegexOptions.IgnoreCase);
      while (match.Success)
      {
        values.Add(match.Value);
        match = match.NextMatch();
      }

      foreach (string value in values)
      {
        try
        {
          if (value.Length < 11) continue;
          int end = value.IndexOf('}');
          if (end < 11) continue;
          int max = int.Parse(value.Substring(10, end - 10));
          ReplaceParameter("Actions:" + max.ToString(), !isIncluded ? "" : GetActionsText(ticket, publicOnly, max));
        }
        catch (Exception)
        {
        }
      }

      return this;
    }

    private string GetActionsText(TicketsViewItem ticket, bool publicOnly, int maxActions)
    {
      ActionsView actions = new ActionsView(Collection.LoginUser);
      actions.LoadByTicketID(ticket.TicketID, publicOnly);

      int productFamilyID = -1;
      Organization organization = Organizations.GetOrganization(Collection.LoginUser, ticket.OrganizationID);
      if (organization.UseProductFamilies && ticket.ProductFamilyID != null)
      {
        productFamilyID = (int)ticket.ProductFamilyID;
      }

      EmailTemplate actionTemplate = EmailTemplates.GetTemplate(Collection.LoginUser, ticket.OrganizationID, 15, productFamilyID);
      string body = actionTemplate.Body;

      StringBuilder builder = new StringBuilder();
      int count = 0;
      foreach (ActionsViewItem action in actions)
      {
        if (action.TicketID != ticket.TicketID) continue;
        actionTemplate.Body = body;
        actionTemplate.ReplaceCommonParameters().ReplaceFields("Action", action);
        builder.AppendLine(actionTemplate.Body);
        count++;
        if (count >= maxActions && maxActions > 0) break;
      }

      return count < 1 ? "" : builder.ToString();
    }

    public EmailTemplate ReplaceIntroduction(string introduction)
    {
      ReplaceParameter("Introduction", GetIntroductionText(introduction));
      return this;
    }

    private string GetIntroductionText(string introduction)
    {
      StringBuilder builder = new StringBuilder();

      if (String.IsNullOrEmpty(introduction))
      {
        builder.Append(".");
      }
      else
      {
        builder.AppendLine(".<br/><br/>");
        builder.AppendLine();
        builder.AppendLine("\"" + introduction + "\"");
      }

      return builder.ToString();
    }

    private bool DoesParameterExist(string parameterName)
    {
      parameterName = "{{" + parameterName + "}}";
      return Body.IndexOf(parameterName) > -1 || Subject.IndexOf(parameterName) > -1;
    }

    private void ReplaceField(string objectName, string fieldName, string value)
    {
      ReplaceParameter(objectName + "." + fieldName, value);
    }

    public static string ReplaceFieldText(string objectName, string fieldName, string value, string text)
    {
      return ReplaceParameterText(objectName + "." + fieldName, value, text);
    }

    public static string ReplaceParameterText(string parameterName, string value, string text)
    {
      parameterName = "{{" + parameterName + "}}";
      if (text.ToLower().Contains(parameterName.ToLower()))
      {
        value = value ?? "";
        value = value.Replace("$", "$$");
        return Regex.Replace(text, parameterName, value, RegexOptions.IgnoreCase);
      }
      return text;
    }

    public EmailTemplate ReplaceParameter(string parameterName, string value)
    {
      Subject = ReplaceParameterText(parameterName, value, Subject);
      Body = ReplaceParameterText(parameterName, value, Body);
      return this;
    }

    public EmailTemplate ReplaceCommonParameters()
    {
      OrganizationsViewItem item = OrganizationsView.GetOrganizationsViewItem(BaseCollection.LoginUser, _organizationID);
      ReplaceFields("MyCompany", item);
      return this;
    }

    public EmailTemplate ReplaceFields(string objectName, BaseItem baseItem)
    {
      if (baseItem != null) ReplaceFields(objectName, baseItem.Row);
      return this;
    }

    public EmailTemplate ReplaceFields(string objectName, DataRow row)
    {
      if (row != null)
      {
        int orgID = BaseCollection.LoginUser.UserID < 1 ? _organizationID : BaseCollection.LoginUser.OrganizationID;
        LoginUser loginUser = new LoginUser(BaseCollection.LoginUser.ConnectionString, BaseCollection.LoginUser.UserID, orgID, null);

        foreach (DataColumn column in row.Table.Columns)
        {
          if (column.DataType == System.Type.GetType("System.DateTime") && row[column] != DBNull.Value)
            try 
	          {
              ReplaceField(objectName, column.ColumnName, DataUtils.DateToLocal(loginUser, (DateTime)row[column]).ToString("g", loginUser.OrganizationCulture));
            }
	          catch (Exception ex)
	          {
              ExceptionLogs.LogException(BaseCollection.LoginUser, ex, "EmailTemplates");
              ReplaceField(objectName, column.ColumnName, DataUtils.DateToLocal(loginUser, (DateTime)row[column]).ToString("g"));
            }
          else if (objectName == "Ticket" && column.ColumnName.ToString().ToLower() == "hoursspent")
          {
            try
            {
              //Tickets save the time spent in hours (decimal value), so we need to convert it to minutes (*60)
              double timeSpentInDecimal = double.Parse(row[column].ToString()) * 60;
              string dateTimeValue = ConvertToTimeText(timeSpentInDecimal.ToString());
              ReplaceField(objectName, column.ColumnName, dateTimeValue);
            }
            catch (Exception ex)
            {
              ReplaceField(objectName, column.ColumnName, row[column].ToString());
            }
          }
          else if (objectName == "Action" && column.ColumnName.ToString().ToLower() == "timespent")
          {
            //Actions save the time spent in minutes pass it as it is.
            string dateTimeValue = ConvertToTimeText(row[column].ToString());
            ReplaceField(objectName, column.ColumnName, dateTimeValue);
          }
          else
            ReplaceField(objectName, column.ColumnName, row[column].ToString());
        }
      }
      else
      {
        Subject = ClearFieldPlaceHolders(objectName, Subject);
        Body = ClearFieldPlaceHolders(objectName, Body);
      }
      return this;
    }

    public static void ReplaceMessageFields(LoginUser loginUser, string objectName, BaseItem baseItem, MailMessage message, int localUserID, int localOrgID) 
    {
      ReplaceMessageFields(loginUser, objectName, baseItem.Row, message, localUserID, localOrgID);
    }

    public static void ReplaceMessageFields(LoginUser loginUser, string objectName, DataRow row, MailMessage message, int localUserID, int localOrgID)
    {
      if (row != null)
      {
        LoginUser localUser = new LoginUser(loginUser.ConnectionString, localUserID, localOrgID, null);

        foreach (DataColumn column in row.Table.Columns)
        {
          if (column.DataType == System.Type.GetType("System.DateTime") && row[column] != DBNull.Value)
            try
            {
              message.Body = ReplaceFieldText(objectName, column.ColumnName, DataUtils.DateToLocal(localUser, (DateTime)row[column]).ToString("g", localUser.OrganizationCulture), message.Body);
              message.Subject = ReplaceFieldText(objectName, column.ColumnName, DataUtils.DateToLocal(localUser, (DateTime)row[column]).ToString("g", localUser.OrganizationCulture), message.Subject);
            }
            catch (Exception ex)
            {
              ExceptionLogs.LogException(localUser, ex, "EmailTemplates");
              message.Body = ReplaceFieldText(objectName, column.ColumnName, DataUtils.DateToLocal(localUser, (DateTime)row[column]).ToString("g"), message.Body);
              message.Subject = ReplaceFieldText(objectName, column.ColumnName, DataUtils.DateToLocal(localUser, (DateTime)row[column]).ToString("g"), message.Subject);
            }
          else
          {
            message.Body = ReplaceFieldText(objectName, column.ColumnName, row[column].ToString(), message.Body);
            message.Subject = ReplaceFieldText(objectName, column.ColumnName, row[column].ToString(), message.Subject);
          }
        }
      }
      else
      {
        message.Body = ClearFieldPlaceHolders(objectName, message.Body);
        message.Subject = ClearFieldPlaceHolders(objectName, message.Subject);
      }
    }

    public static void ReplaceMessageParameter(LoginUser loginUser, string objectName, string value, MailMessage message, int localUserID, int localOrgID)
    {
        message.Body = ReplaceParameterText(objectName, value, message.Body);
        message.Subject = ReplaceParameterText(objectName, value, message.Subject);
    }

    public static string ClearFieldPlaceHolders(string objectName, string text)
    {
      return Regex.Replace(text, "({{"+objectName+"\\..*}})+", "", RegexOptions.IgnoreCase);
    }

    public EmailTemplate ReplaceContacts(TicketsViewItem ticket)
    {
      if (!DoesParameterExist("ContactEmails")) return this;
      try
      {
        StringBuilder builder = new StringBuilder();
        ContactsView contacts = new ContactsView(ticket.Collection.LoginUser);
        contacts.LoadByTicketID(ticket.TicketID);

        foreach (ContactsViewItem contact in contacts)
        {
          if (builder.Length > 0) builder.Append(", ");
          if (IsHtml)
            builder.Append(System.Web.HttpUtility.HtmlEncode(string.Format("{0} {1} <{2}>", contact.FirstName, contact.LastName, contact.Email)));
          else
            builder.Append(string.Format("{0} {1} <{2}>", contact.FirstName, contact.LastName, contact.Email));
        }

        if (builder.Length > 0)
        {
          ReplaceParameter("ContactEmails", builder.ToString());
        }


      }
      catch (Exception)
      {

      }
      return this;
    }

    public MailMessage GetMessage()
    {
      Organization organization = Organizations.GetOrganization(BaseCollection.LoginUser, _organizationID);
      MailMessage message = new MailMessage();
      message.Subject = Subject.Replace('\r', ' ').Replace('\n', ' ');
      message.From = organization.GetReplyToMailAddress();
      message.IsBodyHtml = IsHtml;
      message.Body = Body;
      return message;
    }

    public void UpdateForOrganization(int organizationID, int productFamilyID)
    {
      _organizationID = organizationID;
      OrganizationEmails emails = new OrganizationEmails(BaseCollection.LoginUser);
      emails.LoadByTemplateAndProductFamily(organizationID, EmailTemplateID, productFamilyID);
      if (!emails.IsEmpty)
      {
        Body = emails[0].Body;
        IsHtml = emails[0].IsHtml;
        Subject = emails[0].Subject;
        Footer = emails[0].Footer;
        Header = emails[0].Header;
        UseGlobalTemplate = emails[0].UseGlobalTemplate;
      }

      if (string.IsNullOrWhiteSpace(Body)) return;

      if (IsEmail && UseGlobalTemplate)
      {
        EmailTemplate global = GetGlobalTemplate(BaseCollection.LoginUser, organizationID);

        string content = Body;
        Body = global.Body;
        ReplaceParameter("Header", Header);
        ReplaceParameter("Footer", Footer);
        ReplaceParameter("Body", content);
      }

      if (IncludeDelimiter)
      {
        Organization organization = Organizations.GetOrganization(BaseCollection.LoginUser, _organizationID);
        string delimiter;
        if (IsHtml) delimiter = string.IsNullOrEmpty(organization.EmailDelimiter) ? "<div style=\"margin: 10px auto; text-align: center; font-size: 14px; font-weight: bold; color: #5e5e5e; background: #f2f2f2; padding: 7px 0;\">--- Please reply above this line ---</div>" : organization.EmailDelimiter;
        else delimiter = string.IsNullOrEmpty(organization.EmailDelimiter) ? "--- Please reply above this line ---" : organization.EmailDelimiter;
        string text = InsertAfterTag("body", Body, delimiter);
        if (text == "") text = InsertAfterTag("html", Body, delimiter);
        if (text == "") text = Body.Insert(0, delimiter);
        Body = text;
      }
    }

    private static string InsertAfterTag(string tag, string text, string value)
    {
      int i = text.ToLower().IndexOf("<" + tag.ToLower());
      if (i < 0) return "";
      int j = text.IndexOf(">", i);
      return text.Insert(j + 1, value);
    }

    public static EmailTemplate GetGlobalTemplate(LoginUser loginUser, int organizationID)
    {
      EmailTemplate template = EmailTemplates.GetEmailTemplate(loginUser, 18);

      OrganizationEmails emails = new OrganizationEmails(loginUser);
      emails.LoadByTemplateAndProductFamily(organizationID, 18, -1);
      if (!emails.IsEmpty) template.Body = emails[0].Body;
      return template;
    }

    private static string ConvertToTimeText(string decimalTextValue)
    {
      string value = string.Empty;

      try
      {
        double decimalValue = double.Parse(decimalTextValue);
        var timeSpan = RoundUpToMinute(TimeSpan.FromMinutes(decimalValue));
        int hours = timeSpan.Hours;
        int minutes = timeSpan.Minutes;
        string hourString = string.Empty;
        string minuteString = string.Empty;

        if (minutes > 0)
        {
          minuteString = minutes == 1 ? "Minute" : "Minutes";
          minuteString = string.Format("{0} {1} ", minutes, minuteString);
        }

        if (hours > 0)
        {
          hourString = hours == 1 ? "Hour" : "Hours";
          hourString = string.Format("{0} {1}", hours, hourString);
        }

        value = string.Format("{0} {1}", hourString, minuteString);
      }
      catch
      {
        value = decimalTextValue;
      }

      return value;
    }

    public static TimeSpan RoundUpToMinute(TimeSpan input)
    {
      if (input < TimeSpan.Zero)
      {
        return -RoundUpToMinute(-input);
      }

      int minutes = (int)input.TotalMinutes;

      if (input.Seconds > 0)
      {
        minutes++;
      }
      return TimeSpan.FromMinutes(minutes);
    }
  }

  public partial class EmailTemplates
  {
    public void LoadAll(bool includeTSOnly)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT * FROM EmailTemplates WHERE IsTSOnly = @IsTSOnly OR IsTSOnly = 0 ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@IsTSOnly", includeTSOnly);
        Fill(command);
      }
    }

    /// <summary>
    /// Adds parameters for {{ToFirstName}},{{ToLastName}},{{ToEmailAddress}}
    /// </summary>
    /// <param name="message">MailMessage for params to be replaced</param>


    public static void ReplaceMailAddressParameters(MailMessage message)
    {
      if (message.To.Count != 1)
      {
        ReplaceParameter(message, "ToEmailAddress", "");
        ReplaceParameter(message, "ToFirstName", "");
        ReplaceParameter(message, "ToLastName", "");
        return;
      }


      ReplaceParameter(message, "ToEmailAddress",  message.To[0].Address.Trim());
      if (!string.IsNullOrWhiteSpace(message.To[0].DisplayName))
      {
        List<string> names = new List<string>(message.To[0].DisplayName.Trim().Split(' '));
        if (names.Count > 0)
        {
          ReplaceParameter(message, "ToFirstName", names[0]);

          if (names.Count > 1)
          {
            ReplaceParameter(message, "ToLastName", names[names.Count-1]);
          }
          else
          {
            ReplaceParameter(message, "ToLastName", "");
          }
        }
        else
        {
          ReplaceParameter(message, "ToFirstName", "");
          ReplaceParameter(message, "ToLastName", "");
        }
      }
      else
      {
        ReplaceParameter(message, "ToFirstName", "");
        ReplaceParameter(message, "ToLastName", "");
      }

    }

    public static void ReplaceParameter(MailMessage message, string parameterName, string value)
    {
      try
      {
        value = value ?? "";
        parameterName = "{{" + parameterName + "}}";
        message.Subject = Regex.Replace(message.Subject, parameterName, value, RegexOptions.IgnoreCase);
        message.Body = Regex.Replace(message.Body, parameterName, value, RegexOptions.IgnoreCase);
      }
      catch (Exception)
      {
      }
    }


    #region Utilities

    public static EmailTemplate GetTemplate(LoginUser loginUser, int organizationID, int emailTemplateID, int productFamilyID)
    {
      EmailTemplate result = EmailTemplates.GetEmailTemplate(loginUser, emailTemplateID);
      result.UpdateForOrganization(organizationID, productFamilyID);
      return result;
    }

    private static int GetParentOrganizationID(UsersViewItem portalUser)
    {
      Organization organization = Organizations.GetOrganization(portalUser.Collection.LoginUser, portalUser.OrganizationID);
      return organization != null && organization.ParentID != null ? (int)organization.ParentID : -1;
    }


    #endregion

    #region Mail Messages

    public static MailMessage GetNewTicketBasicPortal(LoginUser loginUser, MailAddress creatorAddress, TicketsViewItem ticket)
    {
      int productFamilyID = -1;
      Organization organization = Organizations.GetOrganization(loginUser, ticket.OrganizationID);
      if (organization.UseProductFamilies && ticket.ProductFamilyID != null)
      {
        productFamilyID = (int)ticket.ProductFamilyID;
      }

      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 0, productFamilyID);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.PortalUrl);
      template.ReplaceParameter("CreatorAddress", creatorAddress.ToString());
      template.ReplaceActions(ticket, true);
      template.ReplaceContacts(ticket);
      return template.GetMessage();
    }

    public static MailMessage GetNewTicketAdvPortal(LoginUser loginUser, UsersViewItem creator, TicketsViewItem ticket)
    {
      int productFamilyID = -1;
      Organization organization = Organizations.GetOrganization(loginUser, ticket.OrganizationID);
      if (organization.UseProductFamilies && ticket.ProductFamilyID != null)
      {
        productFamilyID = (int)ticket.ProductFamilyID;
      }

      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 1, productFamilyID);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.PortalUrl);
      template.ReplaceActions(ticket, true);
      if (creator != null) template.ReplaceFields("Creator", creator);
      template.ReplaceContacts(ticket);
      return template.GetMessage();
    }

    public static MailMessage GetNewTicketInternal(LoginUser loginUser, UsersViewItem creator, TicketsViewItem ticket)
    {
      int productFamilyID = -1;
      Organization organization = Organizations.GetOrganization(loginUser, ticket.OrganizationID);
      if (organization.UseProductFamilies && ticket.ProductFamilyID != null)
      {
        productFamilyID = (int)ticket.ProductFamilyID;
      }

      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 2, productFamilyID);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.TicketUrl);
      if (creator != null) template.ReplaceFields("Creator", creator);
      template.ReplaceActions(ticket, false);
      template.ReplaceContacts(ticket);
      return template.GetMessage();
    }

    public static MailMessage GetTicketAssignmentUser(LoginUser loginUser, string assignor, TicketsViewItem ticket)
    {
      int productFamilyID = -1;
      Organization organization = Organizations.GetOrganization(loginUser, ticket.OrganizationID);
      if (organization.UseProductFamilies && ticket.ProductFamilyID != null)
      {
        productFamilyID = (int)ticket.ProductFamilyID;
      }

      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 3, productFamilyID);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.TicketUrl);
      if (ticket.UserID != null)
      {
        UsersViewItem assignee = UsersView.GetUsersViewItem(loginUser, (int)ticket.UserID);
        if (assignee != null) template.ReplaceFields("Assignee", assignee);
      }

      template.ReplaceActions(ticket, false).ReplaceParameter("Assignor", assignor);
      template.ReplaceContacts(ticket);
      return template.GetMessage();
    }

    public static MailMessage GetTicketAssignmentGroup(LoginUser loginUser, string assignor, TicketsViewItem ticket)
    {
      int productFamilyID = -1;
      Organization organization = Organizations.GetOrganization(loginUser, ticket.OrganizationID);
      if (organization.UseProductFamilies && ticket.ProductFamilyID != null)
      {
        productFamilyID = (int)ticket.ProductFamilyID;
      }

      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 4, productFamilyID);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.TicketUrl);
      if (ticket.GroupID != null)
      {
        Group group = Groups.GetGroup(loginUser, (int)ticket.GroupID);
        if (group != null) template.ReplaceFields("Group", group);
      }

      template.ReplaceActions(ticket, false).ReplaceParameter("Assignor", assignor);
      template.ReplaceContacts(ticket);
      return template.GetMessage();
    }

    public static MailMessage GetTicketUpdateUser(LoginUser loginUser, string modifierName, TicketsViewItem ticket, string changeText, bool includeActions)
    {
      int productFamilyID = -1;
      Organization organization = Organizations.GetOrganization(loginUser, ticket.OrganizationID);
      if (organization.UseProductFamilies && ticket.ProductFamilyID != null)
      {
        productFamilyID = (int)ticket.ProductFamilyID;
      }

      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 5, productFamilyID);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.TicketUrl);

      template.ReplaceParameter("ModifierName", modifierName);
      template.ReplaceActions(ticket, false);
      template.ReplaceParameter("Changes", changeText);
      template.ReplaceContacts(ticket);
      return template.GetMessage();
    }

    public static MailMessage GetTicketUpdateBasicPortal(LoginUser loginUser, string modifierName, TicketsViewItem ticket, bool includeActions)
    {
      int productFamilyID = -1;
      Organization organization = Organizations.GetOrganization(loginUser, ticket.OrganizationID);
      if (organization.UseProductFamilies && ticket.ProductFamilyID != null)
      {
        productFamilyID = (int)ticket.ProductFamilyID;
      }

      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 6, productFamilyID);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.PortalUrl);

      template.ReplaceParameter("ModifierName", modifierName);
      template.ReplaceActions(ticket, true);
      template.ReplaceContacts(ticket);
      return template.GetMessage();
    }

    public static MailMessage GetTicketUpdateAdvPortal(LoginUser loginUser, string modifierName, TicketsViewItem ticket, bool includeActions)
    {
      int productFamilyID = -1;
      Organization organization = Organizations.GetOrganization(loginUser, ticket.OrganizationID);
      if (organization.UseProductFamilies && ticket.ProductFamilyID != null)
      {
        productFamilyID = (int)ticket.ProductFamilyID;
      }

      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 7, productFamilyID);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.PortalUrl);

      template.ReplaceParameter("ModifierName", modifierName);
      template.ReplaceActions(ticket, true);
      template.ReplaceContacts(ticket);
      return template.GetMessage();
    }

    public static MailMessage GetTicketClosed(LoginUser loginUser, string modifierName, TicketsViewItem ticket, bool includeActions)
    {
      int productFamilyID = -1;
      Organization organization = Organizations.GetOrganization(loginUser, ticket.OrganizationID);
      if (organization.UseProductFamilies && ticket.ProductFamilyID != null)
      {
        productFamilyID = (int)ticket.ProductFamilyID;
      }

      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 20, productFamilyID);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.PortalUrl);

      template.ReplaceParameter("ModifierName", modifierName);
      template.ReplaceActions(ticket, true);
      template.ReplaceContacts(ticket);
      return template.GetMessage();
    }

    public static MailMessage GetWelcomePortalUser(LoginUser loginUser, UsersViewItem portalUser, string password)
    {
      EmailTemplate template = GetTemplate(loginUser, GetParentOrganizationID(portalUser), 8, -1);
      OrganizationsViewItem company = OrganizationsView.GetOrganizationsViewItem(loginUser, portalUser.OrganizationID);
      template.ReplaceCommonParameters().ReplaceFields("PortalUser", portalUser).ReplaceFields("Company", company).ReplaceParameter("Password", password);
      return template.GetMessage();
    }

    public static MailMessage GetWelcomeTSUser(LoginUser loginUser, UsersViewItem user, string password)
    {
      EmailTemplate template = GetTemplate(loginUser, user.OrganizationID, 9, -1);
      template.ReplaceCommonParameters().ReplaceFields("User", user).ReplaceParameter("Password", password);
      return template.GetMessage();
    }

    public static MailMessage GetWelcomeNewSignUp(LoginUser loginUser, UsersViewItem user, string password, string expiration, int templateID)
    {
      EmailTemplate template = GetTemplate(loginUser, 1078, templateID, -1);
      Organization customer = Organizations.GetOrganization(loginUser, user.OrganizationID);
      template.ReplaceCommonParameters().ReplaceFields("User", user).ReplaceParameter("Password", password).ReplaceParameter("Expiration", expiration).ReplaceFields("Company", customer).ReplaceFields("Company", customer.GetOrganizationView());
      return template.GetMessage();
    }

    public static MailMessage GetWelcomeNewSignUp(LoginUser loginUser, UsersViewItem user, string password, string expiration)
    {
      return GetWelcomeNewSignUp(loginUser, user, password, expiration, 10);
    }

    public static MailMessage GetChangedPasswordPortal(LoginUser loginUser, UsersViewItem portalUser)
    {
      EmailTemplate template = GetTemplate(loginUser, GetParentOrganizationID(portalUser), 11, -1);
      template.ReplaceCommonParameters().ReplaceFields("PortalUser", portalUser);
      return template.GetMessage();
    }

    public static MailMessage GetChangedPasswordTS(LoginUser loginUser, UsersViewItem user)
    {
      EmailTemplate template = GetTemplate(loginUser, user.OrganizationID, 12, -1);
      template.ReplaceCommonParameters().ReplaceFields("User", user);
      return template.GetMessage();
    }

	 public static MailMessage GetNewDeviceEmail(LoginUser loginUser, UsersViewItem user)
	 {
		 EmailTemplate template = GetTemplate(loginUser, user.OrganizationID, 30, -1);
		 template.ReplaceCommonParameters().ReplaceFields("User", user);
		 return template.GetMessage();
	 }

	 public static MailMessage GetTooManyAttempts(LoginUser loginUser, UsersViewItem user)
	 {
		 EmailTemplate template = GetTemplate(loginUser, user.OrganizationID, 31, -1);
		 template.ReplaceCommonParameters().ReplaceFields("User", user);
		 return template.GetMessage();
	 }

	 public static MailMessage GetResetPasswordPortal(LoginUser loginUser, UsersViewItem portalUser, string password)
    {
      EmailTemplate template = GetTemplate(loginUser, GetParentOrganizationID(portalUser), 13, -1);
      template.ReplaceCommonParameters().ReplaceFields("PortalUser", portalUser).ReplaceParameter("Password", password);
      return template.GetMessage();
    }

    public static MailMessage GetResetPasswordTS(LoginUser loginUser, UsersViewItem user, string password)
    {
      EmailTemplate template = GetTemplate(loginUser, 1078, 14, -1);
      template.ReplaceCommonParameters().ReplaceFields("User", user).ReplaceParameter("Password", password); ;
      return template.GetMessage();
    }

    public static MailMessage GetTicketUpdateRequest(LoginUser loginUser, UsersViewItem requestor, TicketsViewItem ticket, bool includeActions)
    {
      int productFamilyID = -1;
      Organization organization = Organizations.GetOrganization(loginUser, ticket.OrganizationID);
      if (organization.UseProductFamilies && ticket.ProductFamilyID != null)
      {
        productFamilyID = (int)ticket.ProductFamilyID;
      }

      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 16, productFamilyID);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.TicketUrl);

      if (requestor != null) template.ReplaceFields("Requestor", requestor);
      template.ReplaceActions(ticket, false);
      template.ReplaceContacts(ticket);
      return template.GetMessage();
    }

    public static MailMessage GetTicketSendEmail(LoginUser loginUser, UsersViewItem sender, TicketsViewItem ticket, string recipient, string introduction)
    {
      int productFamilyID = -1;
      Organization organization = Organizations.GetOrganization(loginUser, ticket.OrganizationID);
      if (organization.UseProductFamilies && ticket.ProductFamilyID != null)
      {
        productFamilyID = (int)ticket.ProductFamilyID;
      }

      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 21, productFamilyID);
      template.ReplaceCommonParameters().ReplaceFields("Sender", sender).ReplaceFields("Ticket", ticket);
      template.ReplaceIntroduction(introduction);
      template.ReplaceActions(ticket, true).ReplaceParameter("Recipient", recipient); ;
      template.ReplaceContacts(ticket);
      return template.GetMessage();
    }

    public static MailMessage GetReminderTicketEmail(LoginUser loginUser, Reminder reminder, UsersViewItem user, TicketsViewItem ticket)
    {
      int productFamilyID = -1;
      Organization organization = Organizations.GetOrganization(loginUser, ticket.OrganizationID);
      if (organization.UseProductFamilies && ticket.ProductFamilyID != null)
      {
        productFamilyID = (int)ticket.ProductFamilyID;
      }

      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 22, productFamilyID);
      template.ReplaceCommonParameters().ReplaceFields("User", user).ReplaceFields("Ticket", ticket);
      template.ReplaceParameter("ReminderDescription", reminder.Description).ReplaceParameter("ReminderDueDate", reminder.DueDate.ToString("g", loginUser.OrganizationCulture));
      template.ReplaceActions(ticket, false);
      template.ReplaceContacts(ticket);
      return template.GetMessage();
    }

    public static MailMessage GetReminderCustomerEmail(LoginUser loginUser, Reminder reminder, UsersViewItem user, OrganizationsViewItem company)
    {
      EmailTemplate template = GetTemplate(loginUser, reminder.OrganizationID, 23, -1);
      template.ReplaceCommonParameters().ReplaceFields("User", user).ReplaceFields("Company", company);
      template.ReplaceParameter("ReminderDescription", reminder.Description).ReplaceParameter("ReminderDueDate", reminder.DueDate.ToString("g", loginUser.OrganizationCulture));
      return template.GetMessage();
    }

    public static MailMessage GetReminderContactEmail(LoginUser loginUser, Reminder reminder, UsersViewItem user, ContactsViewItem contact)
    {
      EmailTemplate template = GetTemplate(loginUser, reminder.OrganizationID, 24, -1);
      template.ReplaceCommonParameters().ReplaceFields("User", user).ReplaceFields("Contact", contact);
      template.ReplaceParameter("ReminderDescription", reminder.Description).ReplaceParameter("ReminderDueDate", reminder.DueDate.ToString("g", loginUser.OrganizationCulture));
      return template.GetMessage();
    }

    public static MailMessage GetSlaEmail(LoginUser loginUser, TicketsViewItem ticket, string violationType, bool isWarning)
    {
      int productFamilyID = -1;
      Organization organization = Organizations.GetOrganization(loginUser, ticket.OrganizationID);
      if (organization.UseProductFamilies && ticket.ProductFamilyID != null)
      {
        productFamilyID = (int)ticket.ProductFamilyID;
      }

      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, isWarning ? 26 : 25, productFamilyID);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceActions(ticket, false).ReplaceParameter("ViolationType", violationType).ReplaceParameter("TicketUrl", ticket.TicketUrl); ;
      template.ReplaceContacts(ticket);
      return template.GetMessage();
    }

    public static MailMessage GetSignUpNotification(LoginUser loginUser, User user)
    {
      Organization company = Organizations.GetOrganization(loginUser, user.OrganizationID);
      EmailTemplate template = GetTemplate(loginUser, 1078, 17, -1);

      string search = string.Format("http://www.google.com/search?hl=en&q={0}&btnG=Search", company.Name.Replace(" ", "+"));
      string website = string.Format("http://{0}", user.Email.Substring(user.Email.IndexOf("@") + 1));
      PhoneNumbers numbers = new PhoneNumbers(loginUser);
      numbers.LoadByMyOrganization(company.OrganizationID);

      string phone = numbers.IsEmpty ? "N/A" : numbers[0].Number;

      template.ReplaceCommonParameters().ReplaceFields("User", user).ReplaceFields("Organization", company).ReplaceParameter("SearchUrl", search).ReplaceParameter("Phone", phone).ReplaceParameter("WebsiteUrl", website).ReplaceFields("Organization", company.GetOrganizationView()).ReplaceParameter("ProductType", DataUtils.ProductTypeString(company.ProductType));
      return template.GetMessage();
    }

    #endregion

  }

}
