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
        return this;
      }

      ReplaceParameter("Actions", GetActionsText(ticket, publicOnly, 0));
      
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
          ReplaceParameter("Actions:" + max.ToString(), GetActionsText(ticket, publicOnly, max));
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

      EmailTemplate actionTemplate = EmailTemplates.GetTemplate(Collection.LoginUser, ticket.OrganizationID, 15);
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

      return builder.ToString();
    }




    public EmailTemplate ReplaceParameter(string parameterName, string value)
    {
      parameterName = "{{" + parameterName + "}}";
      Subject = Regex.Replace(Subject, parameterName, value, RegexOptions.IgnoreCase);
      Body = Regex.Replace(Body, parameterName, value, RegexOptions.IgnoreCase);
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
            ReplaceField(objectName, column.ColumnName, DataUtils.DateToLocal(loginUser, (DateTime)row[column]).ToString("g", loginUser.OrganizationCulture));
          else
            ReplaceField(objectName, column.ColumnName, row[column].ToString());
        }
      }
      return this;
    }

    private void ReplaceField(string objectName, string fieldName, string value)
    {
      ReplaceParameter(objectName + "." + fieldName, value);
    }

    public MailMessage GetMessage()
    {
      Organization organization = Organizations.GetOrganization(BaseCollection.LoginUser, _organizationID);
      MailMessage message = new MailMessage();
      message.Subject = Subject.Replace('\r', ' ').Replace('\n', ' ');
      message.From = new MailAddress(organization.GetReplyToAddress());
      message.IsBodyHtml = IsHtml;
      message.Body = Body;
      return message;
    }

    public void UpdateForOrganization(int organizationID)
    {
      _organizationID = organizationID;
      OrganizationEmails emails = new OrganizationEmails(BaseCollection.LoginUser);
      emails.LoadByTemplate(organizationID, EmailTemplateID);
      if (!emails.IsEmpty)
      {
        Body = emails[0].Body;
        IsHtml = emails[0].IsHtml;
        Subject = emails[0].Subject;
        Footer = emails[0].Footer;
        Header = emails[0].Header;
        UseGlobalTemplate = emails[0].UseGlobalTemplate;
      }

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
        if (IsHtml) delimiter = string.IsNullOrEmpty(organization.EmailDelimiter) ? "<div style=\"margin:0px auto;text-align:center;font-size:12px;color:#a4a4a4; padding-bottom:7px\">--- Please reply above this line ---</div>" : organization.EmailDelimiter;
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
      emails.LoadByTemplate(organizationID, 18);
      if (!emails.IsEmpty) template.Body = emails[0].Body;
      return template;
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

    #region Utilities

    public static EmailTemplate GetTemplate(LoginUser loginUser, int organizationID, int emailTemplateID)
    {
      EmailTemplate result = EmailTemplates.GetEmailTemplate(loginUser, emailTemplateID);
      result.UpdateForOrganization(organizationID);
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
      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 0);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.PortalUrl);
      template.ReplaceParameter("CreatorAddress", creatorAddress.ToString());
      template.ReplaceActions(ticket, true);
      return template.GetMessage();
    }

    public static MailMessage GetNewTicketAdvPortal(LoginUser loginUser, UsersViewItem creator, TicketsViewItem ticket)
    {
      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 1);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.PortalUrl);
      template.ReplaceActions(ticket, true);
      if (creator != null) template.ReplaceFields("Creator", creator);
      return template.GetMessage();
    }

    public static MailMessage GetNewTicketInternal(LoginUser loginUser, UsersViewItem creator, TicketsViewItem ticket)
    {
      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 2);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.TicketUrl);
      if (creator != null) template.ReplaceFields("Creator", creator);
      template.ReplaceActions(ticket, false);
      return template.GetMessage();
    }

    public static MailMessage GetTicketAssignmentUser(LoginUser loginUser, string assignor, TicketsViewItem ticket)
    {
      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 3);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.TicketUrl);
      if (ticket.UserID != null)
      {
        UsersViewItem assignee = UsersView.GetUsersViewItem(loginUser, (int)ticket.UserID);
        if (assignee != null) template.ReplaceFields("Assignee", assignee);
      }


      template.ReplaceActions(ticket, false).ReplaceParameter("Assignor", assignor);
      return template.GetMessage();
    }

    public static MailMessage GetTicketAssignmentGroup(LoginUser loginUser, string assignor, TicketsViewItem ticket)
    {
      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 4);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.TicketUrl);
      if (ticket.GroupID != null)
      {
        Group group = Groups.GetGroup(loginUser, (int)ticket.GroupID);
        if (group != null) template.ReplaceFields("Group", group);
      }

      template.ReplaceActions(ticket, false).ReplaceParameter("Assignor", assignor);
      return template.GetMessage();
    }

    public static MailMessage GetTicketUpdateUser(LoginUser loginUser, string modifierName, TicketsViewItem ticket, string changeText, bool includeActions)
    {
      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 5);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.TicketUrl);

      template.ReplaceParameter("ModifierName", modifierName);
      template.ReplaceActions(ticket, false, includeActions);
      template.ReplaceParameter("Changes", changeText);
      return template.GetMessage();
    }

    public static MailMessage GetTicketUpdateBasicPortal(LoginUser loginUser, string modifierName, TicketsViewItem ticket, bool includeActions)
    {
      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 6);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.PortalUrl);

      template.ReplaceParameter("ModifierName", modifierName);
      template.ReplaceActions(ticket, true, includeActions);
      return template.GetMessage();
    }

    public static MailMessage GetTicketUpdateAdvPortal(LoginUser loginUser, string modifierName, TicketsViewItem ticket, bool includeActions)
    {
      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 7);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.PortalUrl);

      template.ReplaceParameter("ModifierName", modifierName);
      template.ReplaceActions(ticket, true, includeActions);
      return template.GetMessage();
    }

    public static MailMessage GetTicketClosed(LoginUser loginUser, string modifierName, TicketsViewItem ticket, bool includeActions)
    {
      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 20);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.PortalUrl);

      template.ReplaceParameter("ModifierName", modifierName);
      template.ReplaceActions(ticket, true, includeActions);
      return template.GetMessage();
    }

    public static MailMessage GetWelcomePortalUser(LoginUser loginUser, UsersViewItem portalUser, string password)
    {
      EmailTemplate template = GetTemplate(loginUser, GetParentOrganizationID(portalUser), 8);
      OrganizationsViewItem company = OrganizationsView.GetOrganizationsViewItem(loginUser, portalUser.OrganizationID);
      template.ReplaceCommonParameters().ReplaceFields("PortalUser", portalUser).ReplaceFields("Company", company).ReplaceParameter("Password", password);
      return template.GetMessage();
    }

    public static MailMessage GetWelcomeTSUser(LoginUser loginUser, UsersViewItem user, string password)
    {
      EmailTemplate template = GetTemplate(loginUser, user.OrganizationID, 9);
      template.ReplaceCommonParameters().ReplaceFields("User", user).ReplaceParameter("Password", password);
      return template.GetMessage();
    }

    public static MailMessage GetWelcomeNewSignUp(LoginUser loginUser, UsersViewItem user, string password, string expiration, int templateID)
    {
      EmailTemplate template = GetTemplate(loginUser, 1078, templateID);
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
      EmailTemplate template = GetTemplate(loginUser, GetParentOrganizationID(portalUser), 11);
      template.ReplaceCommonParameters().ReplaceFields("PortalUser", portalUser);
      return template.GetMessage();
    }

    public static MailMessage GetChangedPasswordTS(LoginUser loginUser, UsersViewItem user)
    {
      EmailTemplate template = GetTemplate(loginUser, user.OrganizationID, 12);
      template.ReplaceCommonParameters().ReplaceFields("User", user);
      return template.GetMessage();
    }

    public static MailMessage GetResetPasswordPortal(LoginUser loginUser, UsersViewItem portalUser, string password)
    {
      EmailTemplate template = GetTemplate(loginUser, GetParentOrganizationID(portalUser), 13);
      template.ReplaceCommonParameters().ReplaceFields("PortalUser", portalUser).ReplaceParameter("Password", password);
      return template.GetMessage();
    }

    public static MailMessage GetResetPasswordTS(LoginUser loginUser, UsersViewItem user, string password)
    {
      EmailTemplate template = GetTemplate(loginUser, 1078, 14);
      template.ReplaceCommonParameters().ReplaceFields("User", user).ReplaceParameter("Password", password); ;
      return template.GetMessage();
    }

    public static MailMessage GetTicketUpdateRequest(LoginUser loginUser, UsersViewItem requestor, TicketsViewItem ticket, bool includeActions)
    {
      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 16);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceParameter("TicketUrl", ticket.TicketUrl);

      if (requestor != null) template.ReplaceFields("Requestor", requestor);
      template.ReplaceActions(ticket, false, includeActions);
      return template.GetMessage();
    }

    public static MailMessage GetTicketSendEmail(LoginUser loginUser, UsersViewItem sender, TicketsViewItem ticket, string recipient)
    {
      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 21);
      template.ReplaceCommonParameters().ReplaceFields("Sender", sender).ReplaceFields("Ticket", ticket);


      template.ReplaceActions(ticket, true).ReplaceParameter("Recipient", recipient); ;
      return template.GetMessage();
    }

    public static MailMessage GetReminderTicketEmail(LoginUser loginUser, Reminder reminder, UsersViewItem user, TicketsViewItem ticket)
    {
      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, 22);
      template.ReplaceCommonParameters().ReplaceFields("User", user).ReplaceFields("Ticket", ticket);
      template.ReplaceParameter("ReminderDescription", reminder.Description).ReplaceParameter("ReminderDueDate", reminder.DueDate.ToString("g", loginUser.OrganizationCulture));
      template.ReplaceActions(ticket, false);
      return template.GetMessage();
    }

    public static MailMessage GetReminderCustomerEmail(LoginUser loginUser, Reminder reminder, UsersViewItem user, OrganizationsViewItem company)
    {
      EmailTemplate template = GetTemplate(loginUser, reminder.OrganizationID, 23);
      template.ReplaceCommonParameters().ReplaceFields("User", user).ReplaceFields("Company", company);
      template.ReplaceParameter("ReminderDescription", reminder.Description).ReplaceParameter("ReminderDueDate", reminder.DueDate.ToString("g", loginUser.OrganizationCulture));
      return template.GetMessage();
    }

    public static MailMessage GetReminderContactEmail(LoginUser loginUser, Reminder reminder, UsersViewItem user, ContactsViewItem contact)
    {
      EmailTemplate template = GetTemplate(loginUser, reminder.OrganizationID, 24);
      template.ReplaceCommonParameters().ReplaceFields("User", user).ReplaceFields("Contact", contact);
      template.ReplaceParameter("ReminderDescription", reminder.Description).ReplaceParameter("ReminderDueDate", reminder.DueDate.ToString("g", loginUser.OrganizationCulture));
      return template.GetMessage();
    }

    public static MailMessage GetSlaEmail(LoginUser loginUser, TicketsViewItem ticket, string violationType, bool isWarning)
    {
      EmailTemplate template = GetTemplate(loginUser, ticket.OrganizationID, isWarning ? 26 : 25);
      template.ReplaceCommonParameters().ReplaceFields("Ticket", ticket).ReplaceActions(ticket, false).ReplaceParameter("ViolationType", violationType);
      return template.GetMessage();
    }

    public static MailMessage GetSignUpNotification(LoginUser loginUser, User user)
    {
      Organization company = Organizations.GetOrganization(loginUser, user.OrganizationID);
      EmailTemplate template = GetTemplate(loginUser, 1078, 17);

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
