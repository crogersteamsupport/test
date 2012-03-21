using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using Microsoft.Win32;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;


namespace TeamSupport.ServiceLibrary
{
  public class EmailProcessor : ServiceThread
  {
    class UserEmail
    {
      public UserEmail(int userID, string name, string address)
      {
        _userID = userID;
        _name = name;
        _address = address;
      }

      private string _address;
      private string _name;
      private int _userID;

      public string Address
      {
        get { return _address; }
        set { _address = value; }
      }

      public string Name
      {
        get { return _name; }
        set { _name = value; }
      }

      public int UserID
      {
        get { return _userID; }
        set { _userID = value; }
      }
    }

    private bool _isDebug = false;
    private int _currentEmailPostID;

    public EmailProcessor()
    {
    }

    public override string ServiceName
    {
      get { return "EmailProcessor"; }
    }

    public override void Run()
    {
      try
      {
        _isDebug = Settings.ReadBool("Debug", false);

        EmailPosts emailPosts = new EmailPosts(LoginUser);
        emailPosts.LoadAll();

        foreach (EmailPost emailPost in emailPosts)
        {
          if (DateTime.UtcNow > ((DateTime)emailPost.Row["DateCreated"]).AddSeconds(emailPost.HoldTime) || _isDebug)
          {
            try
            {
              SetTimeZone(emailPost);
              ProcessEmail(emailPost);
            }
            catch (Exception ex)
            {
              ExceptionLogs.LogException(LoginUser, ex, "Email", emailPost.Row);
            }
            emailPost.Collection.DeleteFromDB(emailPost.EmailPostID);
          }
          System.Threading.Thread.Sleep(0);
          if (IsStopped) break;
        }
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "Email", "Error processing emails");
      }

    }

    public void SetTimeZone(EmailPost emailPost)
    {
      LoginUser.TimeZoneInfo = null;
      Organization organization = Users.GetTSOrganization(LoginUser, emailPost.CreatorID);
      if (organization != null)
      {
        try
        {
          LoginUser.TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(organization.TimeZoneID);
        }
        catch (Exception)
        {
          LoginUser.TimeZoneInfo = null;
        }

        try
        {
          LoginUser.OrganizationCulture = new CultureInfo(organization.CultureName);
        }
        catch (Exception)
        {
          LoginUser.OrganizationCulture = new CultureInfo("en-US");
        }
      }

      if (LoginUser.TimeZoneInfo == null)
      {
        LoginUser.TimeZoneInfo = TimeZoneInfo.Utc;
      }

    }

    public void ProcessEmail(EmailPost emailPost)
    {
      _currentEmailPostID = emailPost.EmailPostID;
      switch (emailPost.EmailPostType)
      {
        case EmailPostType.TicketModified:
          
          List<int> actionList = new List<int>();

          if (!string.IsNullOrEmpty(emailPost.Param6))
          {
            string[] actions = emailPost.Param6.Split(';');
            foreach (string s in actions)
            {
              try
              {
                actionList.Add(int.Parse(s.Trim()));
              }
              catch (Exception)
              {
              }
            }
          }

          List<int> userList = new List<int>();

          if (!string.IsNullOrEmpty(emailPost.Param8))
          {
            string[] users = emailPost.Param8.Split(';');
            foreach (string s in users)
            {
              try
              {
                userList.Add(int.Parse(s.Trim()));
              }
              catch (Exception)
              {
              }
            }
          }

          ProcessTicketModified(GetIntParam(emailPost.Param1),
            GetNullIntParam(emailPost.Param2),
            GetNullIntParam(emailPost.Param3),
            GetNullIntParam(emailPost.Param4),
            GetNullIntParam(emailPost.Param5),
            actionList.ToArray(),
            userList.ToArray(),
            emailPost.CreatorID,
            GetIntParam(emailPost.Param7) == 1
            );
          break;

        case EmailPostType.TicketUpdateRequest:
          ProcessTicketUpdateRequest(GetIntParam(emailPost.Param1), GetIntParam(emailPost.Param2));
          break;
        case EmailPostType.TicketSendEmail:
          ProcessTicketSendEmail(GetIntParam(emailPost.Param1), GetIntParam(emailPost.Param2), emailPost.Param3);
          break;
        case EmailPostType.WelcomeNewSignup:
          ProcessWelcomeNewSignup(GetIntParam(emailPost.Param1), emailPost.Param2);
          break;
        case EmailPostType.WelcomeTSUser:
          ProcessWelcomeTSUser(GetIntParam(emailPost.Param1), emailPost.Param2);
          break;
        case EmailPostType.WelcomePortalUser:
          ProcessWelcomePortalUser(GetIntParam(emailPost.Param1), emailPost.Param2);
          break;
        case EmailPostType.ResetTSPassword:
          ProcessResetTSPassword(GetIntParam(emailPost.Param1), emailPost.Param2);
          break;
        case EmailPostType.ResetPortalPassword:
          ProcessResetPortalPassword(GetIntParam(emailPost.Param1), emailPost.Param2);
          break;
        case EmailPostType.ChangedTSPassword:
          ProcessChangedTSPassword(GetIntParam(emailPost.Param1));
          break;
        case EmailPostType.ChangedPortalPassword:
          ProcessChangedPortalPassword(GetIntParam(emailPost.Param1));
          break;
        case EmailPostType.InternalSignupNotification:
          ProcessSignUpNotification(GetIntParam(emailPost.Param1));
          break;
        default:
          break;
      }
    }

    private void AddMessage(int organizationID, string description, MailMessage message)
    {
      AddMessage(organizationID, description, message, null);
    }

    private void AddMessage(int organizationID, string description, MailMessage message, string[] attachments)
    {
      Organization organization = Organizations.GetOrganization(LoginUser, organizationID);
      string replyAddress = organization.GetReplyToAddress();

      try
      {
        foreach (MailAddress address in message.To)
        {
          if (address.Address.ToLower().Trim() == message.From.Address.ToLower().Trim() || address.Address.ToLower().Trim() == replyAddress)
          {
            message.To.Remove(address);
          }
        }
      }
      catch { }

      if (message.To.Count < 1) return;

      StringBuilder builder = new StringBuilder();
      //builder.AppendLine("<span class=\"TeamSupportStart\">&nbsp</span>");
      builder.AppendLine(message.Body);

      message.Body = builder.ToString();
      List<MailAddress> addresses = message.To.ToList();

      foreach (MailAddress address in addresses)
      {
        message.To.Clear();
        message.To.Add(address);
        Emails.AddEmail(LoginUser, organizationID, _currentEmailPostID, description, message, attachments);
      }
    }

    #region Ticket Processing

    public void ProcessTicketModified(int ticketID, int? oldUserID, int? oldGroupID, int? oldTicketStatusID, int? oldTicketSeverityID, int[] modifiedActions, int[] users, int modifierID, bool isNew)
    {

      Ticket ticket = Tickets.GetTicket(LoginUser, ticketID);
      if (ticket == null) return;

      User modifier = Users.GetUser(LoginUser, modifierID);
      string modifierName = modifier == null ? "Anonymous User" : modifier.FirstLastName;

      Organization ticketOrganization = Organizations.GetOrganization(LoginUser, ticket.OrganizationID);


      
      if (bool.Parse(OrganizationSettings.ReadString(LoginUser, ticket.OrganizationID, "DisableStatusNotification", "False")))
      {
        oldTicketStatusID = null;
      }

      Actions actions = new Actions(LoginUser);
      actions.LoadByActionIDs(modifiedActions);

      int publicActionCount = 0;
      foreach (TeamSupport.Data.Action action in actions)
      {
        if (action.IsVisibleOnPortal) publicActionCount++;
      }

      AddMessageTicketAssignment(ticket, oldUserID, oldGroupID, isNew, modifier, ticketOrganization);
      AddMessagePortalTicketModified(ticket, isNew, oldTicketStatusID, publicActionCount > 0, users, modifierName, modifierID, ticketOrganization, false);
      AddMessagePortalTicketModified(ticket, isNew, oldTicketStatusID, publicActionCount > 0, users, modifierName, modifierID, ticketOrganization, true);
      AddMessageInternalTicketModified(ticket, oldUserID, oldGroupID, isNew, oldTicketStatusID, oldTicketSeverityID, !actions.IsEmpty, modifierName, modifier == null ? -1 : modifier.UserID, ticketOrganization);
      if (!isNew)
      {
      }
      else
      {
        AddMessageNewTicket(ticket, modifier, ticketOrganization);
      }
    }

    private void AddMessageTicketAssignment(Ticket ticket, int? oldUserID, int? oldGroupID, bool isNew, User modifier, Organization ticketOrganization)
    {
      if (oldUserID == null && oldGroupID == null && !isNew) return;

      int modifierID = modifier == null ? -1 : modifier.UserID;
      string modifierName = modifier == null ? "Anonymous User" : modifier.FirstLastName;

      Actions actions = new Actions(LoginUser);
      actions.LoadLatestByTicket(ticket.TicketID, false);
      string subject = " [pvt]";
      if (ticket.IsVisibleOnPortal && !actions.IsEmpty && actions[0].IsVisibleOnPortal) subject = "";


      if (ticket.UserID != null && oldUserID != null)
      {
        User owner = Users.GetUser(LoginUser, (int)ticket.UserID);
        if (modifierID != owner.UserID && owner.ReceiveTicketNotifications)
        {

          MailMessage message = EmailTemplates.GetTicketAssignmentUser(LoginUser, modifierName, ticket.GetTicketView());
          message.To.Add(new MailAddress(owner.Email, owner.FirstLastName));
          message.Subject = message.Subject + subject;
          AddMessage(ticketOrganization.OrganizationID, "Ticket Assignment [" + ticket.TicketNumber.ToString() + "]", message);
        }
      }

      if (ticket.GroupID != null && oldGroupID != null)
      {
        Group owner = Groups.GetGroup(LoginUser, (int)ticket.GroupID);
        List<UserEmail> list = new List<UserEmail>();
        AddTicketOwners(list, ticket);
        if (ticket.UserID != null) RemoveUser(list, (int)ticket.UserID);

        MailMessage message = EmailTemplates.GetTicketAssignmentGroup(LoginUser, modifierName, ticket.GetTicketView());
        AddUsersToAddresses(message.To, list, modifierID);
        message.Subject = message.Subject + subject;
        AddMessage(ticketOrganization.OrganizationID, "Ticket Assignment [" + ticket.TicketNumber.ToString() + "]", message);

      }


    }

    private void AddMessageInternalTicketModified(Ticket ticket, int? oldUserID, int? oldGroupID, bool isNew, int? oldTicketStatusID, int? oldTicketSeverityID, bool includeActions, string modifierName, int modifierID, Organization ticketOrganization)
    {
      if (oldTicketSeverityID == null && oldTicketStatusID == null && !includeActions) return;
      List<UserEmail> userList = new List<UserEmail>();
      AddTicketOwners(userList, ticket);
      AddTicketSubscribers(userList, ticket);
      List<string> fileNames = new List<string>();

      if (isNew || oldUserID != null || oldGroupID != null) // assignment already sent
      {
        List<UserEmail> removeList = new List<UserEmail>();
        AddTicketOwners(removeList, ticket);
        foreach (UserEmail item in removeList)
        {
          RemoveUser(userList, item.UserID);
        }
      }

      if (oldUserID != null && ticket.UserID != null) RemoveUser(userList, (int)ticket.UserID);
      if (userList.Count < 1) return;

      StringBuilder builder = new StringBuilder();
      if (oldTicketStatusID != null) builder.Append(string.Format("<div>The status changed from {0} to {1}.</div>", TicketStatuses.GetTicketStatus(LoginUser, (int)oldTicketStatusID).Name, TicketStatuses.GetTicketStatus(LoginUser, (int)ticket.TicketStatusID).Name));
      if (oldTicketSeverityID != null) builder.Append(string.Format("<div>The severity changed from {0} to {1}.</div>", TicketSeverities.GetTicketSeverity(LoginUser, (int)oldTicketSeverityID).Name, TicketSeverities.GetTicketSeverity(LoginUser, (int)ticket.TicketSeverityID).Name));

      MailMessage message = EmailTemplates.GetTicketUpdateUser(LoginUser, modifierName, ticket.GetTicketView(), builder.ToString(), includeActions);

      if (includeActions)
      {
        Actions actions = new Actions(LoginUser);
        actions.LoadLatestByTicket(ticket.TicketID, false);
        if (!actions.IsEmpty)
        {
          Attachments attachments = actions[0].GetAttachments();
          foreach (TeamSupport.Data.Attachment attachment in attachments)
          {
            fileNames.Add(attachment.Path);
          }
        }
        
        foreach (TeamSupport.Data.Action item in actions)
        {
          if (!item.IsVisibleOnPortal)
          {
            message.Subject = message.Subject + " [pvt]";
            break;
          }
        }
      }

      AddUsersToAddresses(message.To, userList, modifierID);
      AddMessage(ticketOrganization.OrganizationID, "Internal Ticket Modified [" + ticket.TicketNumber.ToString() + "]", message, fileNames.ToArray());
    }

    private bool IsModifierTSUser(int modifierID)
    {
      bool result = false;
      User modifier = Users.GetUser(LoginUser, modifierID);
      if (modifier != null)
      {
        Organization organization = Organizations.GetOrganization(LoginUser, modifier.OrganizationID);
        result = organization != null && organization.ParentID == 1;
      }

      return result;
    }

    private void AddMessagePortalTicketModified(Ticket ticket, bool isNew, int? oldTicketStatusID, bool includeActions, int[] users, string modifierName, int modifierID, Organization ticketOrganization, bool isBasic)
    {

      if (!ticket.IsVisibleOnPortal)
      {
        return;
      }
      
      if (oldTicketStatusID == null && !includeActions && users.Length < 1)
      {
        return;
      }

      TicketStatus status = TicketStatuses.GetTicketStatus(LoginUser, ticket.TicketStatusID);
      if (status.IsEmailResponse == true && oldTicketStatusID != null && !IsModifierTSUser(modifierID))
      {
        return;
      }

      List<UserEmail> userList;
      List<UserEmail> advUsers = new List<UserEmail>();
      AddAdvancedPortalUsers(advUsers, ticket);

      if (isBasic)
      {
        userList = new List<UserEmail>();
        AddBasicPortalUsers(userList, ticket);
        if (!string.IsNullOrEmpty(ticket.PortalEmail))
        {
          UserEmail email = FindByUserEmail(ticket.PortalEmail, userList);
          if (email == null)
          {
            userList.Add(new UserEmail(-2, "", ticket.PortalEmail));
          }
        }

        foreach (UserEmail item in advUsers)
        {
          UserEmail email = FindByUserEmail(item.Address, userList);
          if (email != null)
          {
            userList.Remove(email);
          }
        }
      }
      else
      {
        userList = advUsers;
      }

      if (oldTicketStatusID == null && !includeActions)
      {
        List<UserEmail> newList = new List<UserEmail>();
        foreach (UserEmail email in userList)
        {
          if (users.Contains(email.UserID))
          {
            newList.Add(email);
          }
        }
        userList = newList;
      }


      if (isNew) // new already sent
      {
        RemoveUser(userList, modifierID);
      }


      if (userList.Count < 1)
      {
        return;
      }

      List<string> fileNames = new List<string>();
      if (includeActions)
      {
        Actions actions = new Actions(LoginUser);
        actions.LoadLatestByTicket(ticket.TicketID, true);
        if (!actions.IsEmpty)
        {
          Attachments attachments = actions[0].GetAttachments();
          foreach (TeamSupport.Data.Attachment attachment in attachments)
          {
            fileNames.Add(attachment.Path);
          }

        }
      }

      MailMessage message = null;

      if (status.IsClosedEmail)
      {
        message = EmailTemplates.GetTicketClosed(LoginUser, modifierName, ticket.GetTicketView(), includeActions);
      
      }
      else
      {
        message = isBasic ? EmailTemplates.GetTicketUpdateBasicPortal(LoginUser, modifierName, ticket.GetTicketView(), includeActions) :
                            EmailTemplates.GetTicketUpdateAdvPortal(LoginUser, modifierName, ticket.GetTicketView(), includeActions);
      }

      AddUsersToAddresses(message.To, userList, modifierID);
      AddMessage(ticketOrganization.OrganizationID, "Portal Ticket Modified [" + ticket.TicketNumber.ToString() + "]", message, fileNames.ToArray());
    }

    private void ProcessTicketUpdateRequest(int ticketID, int modifierID)
    {
      Ticket ticket = Tickets.GetTicket(LoginUser, ticketID);
      User modifier = Users.GetUser(LoginUser, modifierID);
      if (ticket == null || modifier == null || ticket.UserID == null) return;
      Organization ticketOrganization = Organizations.GetOrganization(LoginUser, ticket.OrganizationID);
      if (ticketOrganization == null) return;

      User owner = Users.GetUser(LoginUser, (int)ticket.UserID);

      MailMessage message = EmailTemplates.GetTicketUpdateRequest(LoginUser, UsersView.GetUsersViewItem(LoginUser, modifierID), ticket.GetTicketView(), true);
      message.To.Add(new MailAddress(owner.Email, owner.FirstLastName));
      message.Subject = message.Subject + " [pvt]";

      AddMessage(ticketOrganization.OrganizationID, "Ticket Update Request [" + ticket.TicketNumber.ToString() + "]", message);
    }

    private void ProcessTicketSendEmail(int userID, int ticketID, string addresses)
    {
      Ticket ticket = Tickets.GetTicket(LoginUser, ticketID);
      User sender = Users.GetUser(LoginUser, userID);
      if (ticket == null || sender == null || ticket.OrganizationID != sender.OrganizationID) return;
      LoginUser loginUser = new LoginUser(LoginUser.ConnectionString, sender.UserID, sender.OrganizationID, null);


      char split = ';';
      if (addresses.IndexOf(';') < 0) split = ',';

      string[] list = addresses.Split(split);
      foreach (string item in list)
      {
        try
        {
          MailMessage message = EmailTemplates.GetTicketSendEmail(LoginUser, sender.GetUserView(), ticket.GetTicketView(), item.Trim());
          message.To.Add(new MailAddress(item.Trim()));
          message.Subject = message.Subject;

          AddMessage(sender.OrganizationID, "Ticket Email [" + ticket.TicketNumber.ToString() + "] to " + item, message);

        }
        catch (Exception ex) {
          ExceptionLogs.LogException(loginUser, ex, "Email Processor", "Email Ticket");
        }
      }

      ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "Sent this ticket via email to: " + addresses);

    }

    private void AddMessageNewTicket(Ticket ticket, User modifier, Organization ticketOrganization)
    {
      if (string.IsNullOrEmpty(ticket.TicketSource)) return;

      string source = ticket.TicketSource.ToLower().Trim();
      if (source != "web" && source != "email" && source != "chatoffline") return;

      if (modifier == null && string.IsNullOrEmpty(ticket.PortalEmail)) return;
      MailAddress modifierAddress = (modifier == null) ? new MailAddress(ticket.PortalEmail) : new MailAddress(modifier.Email, modifier.FirstLastName);

      if (modifier != null && modifier.OrganizationID == ticketOrganization.OrganizationID) // internal
      {
        MailMessage message = EmailTemplates.GetNewTicketInternal(LoginUser, modifier.GetUserView(), ticket.GetTicketView());
        message.To.Add(modifierAddress);
        AddMessage(ticket.OrganizationID, "Internal New Ticket [" + ticket.TicketNumber.ToString() + "]", message);
      }
      else // portal
      {
        MailMessage message = modifier != null && modifier.IsPortalUser ? EmailTemplates.GetNewTicketAdvPortal(LoginUser, modifier.GetUserView(), ticket.GetTicketView()) :
                                                                          EmailTemplates.GetNewTicketBasicPortal(LoginUser, modifierAddress, ticket.GetTicketView());
        message.To.Add(modifierAddress);
        AddMessage(ticketOrganization.OrganizationID, "New Ticket [" + ticket.TicketNumber.ToString() + "]", message);
      }


    }

    #endregion

    public void ProcessSignUpNotification(int userID)
    {
      User user = Users.GetUser(LoginUser, userID);
      Organization organization = Organizations.GetOrganization(LoginUser, user.OrganizationID);
      MailMessage message = EmailTemplates.GetSignUpNotification(LoginUser, user);
      message.To.Add(new MailAddress("kjones@teamsupport.com"));
      message.To.Add(new MailAddress("rjohnson@teamsupport.com"));
      message.To.Add(new MailAddress("eharrington@teamsupport.com"));
      message.To.Add(new MailAddress("jhathaway@teamsupport.com"));
      message.To.Add(new MailAddress("jharada@teamsupport.com"));
      AddMessage(1078, "New Internal Sign Up", message);
    }


    public void ProcessWelcomeNewSignup(int userID, string password)
    {
      User user = Users.GetUser(LoginUser, userID);
      Organization organization = Organizations.GetOrganization(LoginUser, user.OrganizationID);

      string from = "sales@teamsupport.com";
      MailMessage message = EmailTemplates.GetWelcomeNewSignUp(LoginUser, user.GetUserView(), password, DateTime.Now.AddDays(14).ToString("MMMM d, yyyy"), 10);
      message.To.Add(new MailAddress(user.Email, user.FirstLastName));
      message.Bcc.Add(new MailAddress("dropbox@79604342.murocsystems.highrisehq.com"));
      message.From = new MailAddress(from);
      AddMessage(1078, "New Sign Up [" + organization.Name + "]", message);
    }

    public void ProcessWelcomeTSUser(int userID, string password)
    {
      User user = Users.GetUser(LoginUser, userID);
      MailMessage message = EmailTemplates.GetWelcomeTSUser(LoginUser, user.GetUserView(), password);
      message.To.Add(new MailAddress(user.Email, user.FirstLastName));
      AddMessage(user.OrganizationID, "Welcome New User [" + user.FirstLastName + "]", message);
    }

    public void ProcessWelcomePortalUser(int userID, string password)
    {
      User user = Users.GetUser(LoginUser, userID);
      Organization organization = (Organization)Organizations.GetOrganization(LoginUser, (int)Organizations.GetOrganization(LoginUser, user.OrganizationID).ParentID);
      MailMessage message = EmailTemplates.GetWelcomePortalUser(LoginUser, user.GetUserView(), password);
      message.To.Add(new MailAddress(user.Email, user.FirstLastName));
      AddMessage(organization.OrganizationID, "Welcome New Portal User [" + user.FirstLastName + "]", message);
    }

    public void ProcessResetTSPassword(int userID, string password)
    {
      User user = Users.GetUser(LoginUser, userID);

      MailMessage message = EmailTemplates.GetResetPasswordTS(LoginUser, user.GetUserView(), password);
      message.To.Add(new MailAddress(user.Email, user.FirstLastName));
      message.From = new MailAddress("support@teamsupport.com", "TeamSupport.com");
      AddMessage(user.OrganizationID, "Reset TS Password [" + user.FirstLastName + "]", message);
    }

    public void ProcessResetPortalPassword(int userID, string password)
    {
      User user = (User)Users.GetUser(LoginUser, userID);
      Organization organization = (Organization)Organizations.GetOrganization(LoginUser, (int)Organizations.GetOrganization(LoginUser, user.OrganizationID).ParentID);
      MailMessage message = EmailTemplates.GetResetPasswordPortal(LoginUser, user.GetUserView(), password);
      message.To.Add(new MailAddress(user.Email, user.FirstLastName));
      AddMessage(organization.OrganizationID, "Reset Portal Password [" + user.FirstLastName + "]", message);
    }

    public void ProcessChangedPortalPassword(int userID)
    {
      User user = (User)Users.GetUser(LoginUser, userID);
      Organization organization = (Organization)Organizations.GetOrganization(LoginUser, (int)Organizations.GetOrganization(LoginUser, user.OrganizationID).ParentID);
      MailMessage message = EmailTemplates.GetChangedPasswordPortal(LoginUser, user.GetUserView());
      message.To.Add(new MailAddress(user.Email, user.FirstLastName));
      AddMessage(organization.OrganizationID, "Portal Password Changed [" + user.FirstLastName + "]", message);
    }

    public void ProcessChangedTSPassword(int userID)
    {
      User user = (User)Users.GetUser(LoginUser, userID);
      MailMessage message = EmailTemplates.GetChangedPasswordTS(LoginUser, user.GetUserView());
      message.To.Add(new MailAddress(user.Email, user.FirstLastName));
      message.From = new MailAddress("support@teamsupport.com", "TeamSupport.com");
      AddMessage(user.OrganizationID, "TS Password Changed [" + user.FirstLastName + "]", message);
    }

    #region Utility Methods

    private int GetIntParam(string param)
    {
      if (string.IsNullOrEmpty(param)) return -1;
      try
      {
        return int.Parse(param);
      }
      catch (Exception)
      {
        return -1;
      }
    }

    private int? GetNullIntParam(string param)
    {
      if (string.IsNullOrEmpty(param)) return null;
      try
      {
        return int.Parse(param);
      }
      catch (Exception)
      {
        return null;
      }
    }

    private void AddUser(List<UserEmail> list, User user)
    {
      AddUser(list, user, false);
    }

    private void AddUser(List<UserEmail> list, User user, bool honorTicketNotifications)
    {
      if (IsUserAlreadyInList(list, user.UserID) || (!user.ReceiveTicketNotifications && honorTicketNotifications)) return;
      list.Add(new UserEmail(user.UserID, user.FirstName + " " + user.LastName, user.Email));
    }

    private void RemoveUser(List<UserEmail> list, int userID)
    {
      UserEmail email = null;
      foreach (UserEmail item in list)
      {
        if (item.UserID == userID)
        {
          email = item;
          break;
        }
      }

      if (email != null) list.Remove(email);
    }

    private bool IsUserAlreadyInList(List<UserEmail> list, int userID)
    {
      foreach (UserEmail item in list)
      {
        if (item.UserID == userID)
        {
          return true;
        }
      }
      return false;
    }

    private void AddTicketOwners(List<UserEmail> userList, Ticket ticket)
    {
      Users users;
      if (ticket.UserID != null)
      {
        AddUser(userList, Users.GetUser(LoginUser, (int)ticket.UserID), true);
      }
      
      
      if (ticket.GroupID != null)
      {
        users = new Users(LoginUser);
        users.LoadByGroupID((int)ticket.GroupID);
        foreach (User user in users) { 
          if (user.ReceiveAllGroupNotifications || ticket.UserID == null) AddUser(userList, user, true);
        }
      }
    }

    private void AddTicketSubscribers(List<UserEmail> userList, Ticket ticket)
    {
      Users users;

      // Ticket Subscribers
      users = new Users(LoginUser);
      users.LoadByTicketSubscription(ticket.TicketID);
      foreach (User user in users) { AddUser(userList, user); }

      // Customer Subscribers
      users = new Users(LoginUser);
      users.LoadByCustomerSubscription(ticket.TicketID);
      foreach (User user in users) { AddUser(userList, user); }
    }

    private void AddBasicPortalUsers(List<UserEmail> userList, Ticket ticket)
    {
      Users users;
      users = new Users(LoginUser);
      users.LoadBasicPortalUsers(ticket.TicketID);
      foreach (User user in users) { AddUser(userList, user); }
    }

    private void AddAdvancedPortalUsers(List<UserEmail> userList, Ticket ticket)
    {
      Users users;
      users = new Users(LoginUser);
      users.LoadAdvancedPortalUsers(ticket.TicketID);
      foreach (User user in users) { AddUser(userList, user); }
    }

    private void AddUsersToAddresses(MailAddressCollection collection, List<UserEmail> users, int modifierID)
    {
      foreach (UserEmail user in users)
      {
        try
        {
          if (user != null && modifierID != user.UserID) collection.Add(new MailAddress(user.Address, user.Name));
          //if (modifierID != -1 && modifierID != user.UserID) collection.Add(new MailAddress(user.Address, user.Name));
        }
        catch
        {
        }
      }
    }

    private UserEmail FindByUserEmail(string address, List<UserEmail> list)
    {
      address = address.Trim().ToLower();
      foreach (UserEmail item in list)
      {
        if (item.Address.ToLower().Trim() == address) return item;
      }
      return null;
    }

    #endregion
  }
}
