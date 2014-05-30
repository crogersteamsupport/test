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
using System.Configuration;


namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  public class EmailProcessor : ServiceThreadPoolProcess
  {
    class UserEmail
    {
      public UserEmail(int userID, string fname, string lname, string address, bool sendTicketsOnlyAfterHours)
      {
        this.UserID = userID;
        this.Name = fname + " " + lname;
        this.FirstName = fname;
        this.LastName = lname;
        this.Address = address;
        this.SendTicketsOnlyAfterHours = sendTicketsOnlyAfterHours;
      }

      public bool SendTicketsOnlyAfterHours { get; set; }
      public string Address { get; set; }
      public string Name { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public int UserID { get; set; }

      public override string ToString()
      {
        return string.Format("{0} <{1}> ({2}) SendTicketsOnlyAfterHours: {3}", Name, Address, UserID.ToString(), SendTicketsOnlyAfterHours.ToString()); 
      }
    }

      private bool _isDebug = false;
    private bool _logEnabled = false;
    private int _currentEmailPostID;


    private static object _staticLock = new object();

    private static EmailPost GetNextEmailPost(string connectionString, int lockID)
    {
      EmailPost result;
      LoginUser loginUser = new LoginUser(connectionString, -1, -1, null);
      lock (_staticLock) { result = EmailPosts.GetDebugNextWaiting(loginUser, lockID.ToString(), 13679); }
      return result;
    }

    public override void ReleaseAllLocks()
    {
      EmailPosts.UnlockAll(LoginUser);
    }

    public override void Run()
    {
      while (!IsStopped)
      {
        EmailPost emailPost = GetNextEmailPost(LoginUser.ConnectionString, (int)_threadPosition);
        if (emailPost == null) return; 

        try
        {
          try
          {
            if (emailPost.CreatorID != -2)
            {
              _isDebug = Settings.ReadBool("Debug", false);
              _logEnabled = ConfigurationManager.AppSettings["LoggingEnabled"] != null && ConfigurationManager.AppSettings["LoggingEnabled"] == "1";

              Logs.WriteLine();
              Logs.WriteEvent("***********************************************************************************");
              Logs.WriteEvent("Processing Email Post  EmailPostID: " + emailPost.EmailPostID.ToString());
              Logs.WriteData(emailPost.Row);
              Logs.WriteLine();
              Logs.WriteEvent("***********************************************************************************");
              Logs.WriteLine();
              SetTimeZone(emailPost);
              ProcessEmail(emailPost);
            }
          }
          catch (Exception ex)
          {
            ExceptionLogs.LogException(LoginUser, ex, "Email", emailPost.Row);
          }
          finally
          {
            Logs.WriteEvent("Deleting from DB");
            emailPost.Collection.DeleteFromDB(emailPost.EmailPostID);
          }
          Logs.WriteEvent("Updating Health");
          UpdateHealth();
        }
        catch (Exception ex)
        {
          ExceptionLogs.LogException(LoginUser, ex, "Email", emailPost.Row);
        }
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
          Logs.WriteEvent("TimeZoneID: " + organization.TimeZoneID);
        }
        catch (Exception)
        {
          LoginUser.TimeZoneInfo = null;
        }

        try
        {
          LoginUser.OrganizationCulture = new CultureInfo(organization.CultureName);
          Logs.WriteEvent("Culture: " + organization.CultureName);
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
      Logs.WriteEvent("EmailPostType: " + emailPost.EmailPostType.ToString());
      switch (emailPost.EmailPostType)
      {
        case EmailPostType.TicketModified:
          List<int> actionList = new List<int>();

          if (!string.IsNullOrEmpty(emailPost.Param6))
          {
            Logs.WriteEvent("Actions (Param6): " + emailPost.Param6);
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
          else
          {
            Logs.WriteEvent("Actions: None");

          }

          List<int> userList = new List<int>();

          if (!string.IsNullOrEmpty(emailPost.Param8))
          {
            Logs.WriteEvent("Users (Param8): " + emailPost.Param8);
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
          else
          {
            Logs.WriteEvent("Users: None");
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
          ProcessTicketSendEmail(GetIntParam(emailPost.Param1), GetIntParam(emailPost.Param2), emailPost.Param3, emailPost.Text1);
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

    private void AddMessage(int organizationID, string description, MailMessage message, string replyToAddress = null, string[] attachments = null, DateTime? timeToSend = null)
    {
      Organization organization = Organizations.GetOrganization(LoginUser, organizationID);
      string replyAddress = organization.GetReplyToAddress(replyToAddress).ToLower().Trim();
      
      int i = 0; 
      while (i < message.To.Count)
      {
        MailAddress address = message.To[i];
        if (address.Address.ToLower().Trim() == message.From.Address.ToLower().Trim() || address.Address.ToLower().Trim() == replyAddress || address.Address.ToLower().Trim() == organization.SystemEmailID.ToString().Trim().ToLower() + "@teamsupport.com")
        {
          message.To.RemoveAt(i);
        }
        else
        {
          i++;
        }
      }

      if (message.To.Count < 1) return;

      StringBuilder builder = new StringBuilder();
      //builder.AppendLine("<span class=\"TeamSupportStart\">&nbsp</span>");
      builder.AppendLine(message.Body);

      message.Body = builder.ToString();
      List<MailAddress> addresses = message.To.ToList();
      string body = message.Body;
      string subject = message.Subject;
      foreach (MailAddress address in addresses)
      {
        message.To.Clear();
        message.To.Add(address);
        message.Body = body;
        message.Subject = subject;
        message.From = new MailAddress(replyAddress);
        EmailTemplates.ReplaceMailAddressParameters(message);
        Emails.AddEmail(LoginUser, organizationID, _currentEmailPostID, description, message, attachments, timeToSend);
        if (message.Subject == null) message.Subject = "";
        Logs.WriteEvent(string.Format("Queueing email [{0}] - {1}  Subject: {2}", description, address.ToString(), message.Subject));

      }
    }

    #region Ticket Processing

    public void ProcessTicketModified(int ticketID, int? oldUserID, int? oldGroupID, int? oldTicketStatusID, int? oldTicketSeverityID, int[] modifiedActions, int[] users, int modifierID, bool isNew)
    {
      Logs.WriteEvent(string.Format("TicketID: {0}", ticketID.ToString()));
      Logs.WriteEvent(string.Format("OldUserID: {0}",  (oldUserID == null ? "NULL" : oldUserID.ToString())));
      Logs.WriteEvent(string.Format("OldGroupID: {0}", (oldGroupID == null ? "NULL" : oldGroupID.ToString())));
      Logs.WriteEvent(string.Format("OldTicketStatusID: {0}", (oldTicketStatusID == null ? "NULL" : oldTicketStatusID.ToString())));
      Logs.WriteEvent(string.Format("OldTicketSeverityID: {0}", (oldTicketSeverityID == null ? "NULL" : oldTicketSeverityID.ToString())));
      Logs.WriteEvent(string.Format("ModifierID: {0}", modifierID.ToString()));
      Logs.WriteEvent(string.Format("IsNew: {0}", isNew.ToString()));
      try
      {

        Ticket ticket = Tickets.GetTicket(LoginUser, ticketID);
        if (ticket == null)
        {
          Logs.WriteEvent("Ticket is NULL, canceling.");
          return;
        }

        Logs.WriteLine();
        Logs.WriteEvent("***********************************************************************************");
        Logs.WriteEvent("Processing Ticket Email Post  TicketID: " + ticket.TicketID.ToString());
        Logs.WriteData(ticket.Row);
        Logs.WriteLine();
        Logs.WriteEvent("***********************************************************************************");
        Logs.WriteLine();


        User modifier = Users.GetUser(LoginUser, modifierID);
        string modifierName = modifier == null ? GetOrganizationName(ticket.OrganizationID) : modifier.FirstLastName;
        Logs.WriteEvent("Modifier: " + modifierName);

        Organization ticketOrganization = Organizations.GetOrganization(LoginUser, ticket.OrganizationID);
        Logs.WriteEvent("OrganizationID: " + ticketOrganization.OrganizationID.ToString());

        Logs.WriteEvent("DisableStatusNotification: " + OrganizationSettings.ReadString(LoginUser, ticket.OrganizationID, "DisableStatusNotification", "False"));
        if (oldTicketStatusID != null && bool.Parse(OrganizationSettings.ReadString(LoginUser, ticket.OrganizationID, "DisableStatusNotification", "False")))
        {
          TicketStatus status = TicketStatuses.GetTicketStatus(LoginUser, ticket.TicketStatusID);
          Logs.WriteEvent(string.Format("TicketStatusID: {0}  IsClosedEmail: {1}", ticket.TicketStatusID.ToString(), status.IsClosed.ToString()));
          if (!status.IsClosedEmail) oldTicketStatusID = null;
        }

        Actions actions = new Actions(LoginUser);
        actions.LoadByActionIDs(modifiedActions);
        Logs.WriteEvent(string.Format("{0} Actions Loaded", actions.Count.ToString()));

        int publicActionCount = 0;
        foreach (TeamSupport.Data.Action action in actions)
        {
          if (action.IsVisibleOnPortal) publicActionCount++;
        }
        Logs.WriteEvent(string.Format("{0} Public Actions Loaded", publicActionCount.ToString()));

        Logs.WriteEvent("Processing Ticket Assignment");
        AddMessageTicketAssignment(ticket, oldUserID, oldGroupID, isNew, modifier, ticketOrganization);
        Logs.WriteEvent("Processing Advanced Portal");
        AddMessagePortalTicketModified(ticket, isNew, oldTicketStatusID, publicActionCount > 0, users, modifierName, modifierID, ticketOrganization, false);
        Logs.WriteEvent("Processing Basic Portal");
        AddMessagePortalTicketModified(ticket, isNew, oldTicketStatusID, publicActionCount > 0, users, modifierName, modifierID, ticketOrganization, true);
        Logs.WriteEvent("Processing Internal Modified");
        AddMessageInternalTicketModified(ticket, oldUserID, oldGroupID, isNew, oldTicketStatusID, oldTicketSeverityID, !actions.IsEmpty, modifierName, modifier == null ? -1 : modifier.UserID, ticketOrganization);
        if (!isNew)
        {
        }
        else
        {
          Logs.WriteEvent("Processing New Ticket");
          AddMessageNewTicket(ticket, modifier, ticketOrganization);
        }

      }
      catch (Exception ex)
      {
        Logs.WriteException(ex);
        throw;
      }

    }

    private string GetOrganizationName(int organizationID)
    {
      Organization org = Organizations.GetOrganization(LoginUser, organizationID);
      return org == null ? "" : org.Name;
    }

    private void AddMessageTicketAssignment(Ticket ticket, int? oldUserID, int? oldGroupID, bool isNew, User modifier, Organization ticketOrganization)
    {

      if (oldUserID == null && oldGroupID == null && !isNew)
      {
        Logs.WriteEvent("OldUserID = NULL AND OldGroupID = NULL && NOT IsNew, returning");
        return;
      }

      int modifierID = modifier == null ? -1 : modifier.UserID;
      string modifierName = modifier == null ? GetOrganizationName(ticket.OrganizationID) : modifier.FirstLastName;
      Logs.WriteEventFormat("Modifier Name: {0}", modifierName);

      Actions actions = new Actions(LoginUser);
      actions.LoadLatestByTicket(ticket.TicketID, false);
      Logs.WriteEventFormat("{0} Latest actions loaded", actions.Count.ToString());

      string subject = " [pvt]";
      Logs.WriteEvent("Subject = [pvt]");
      if (ticket.IsVisibleOnPortal && !actions.IsEmpty && actions[0].IsVisibleOnPortal) subject = "";
      Logs.WriteEvent("Subject = '', VisibleOnPortal=True AND Actions not empty AND Action[0] is visible on portal");

      List<string> fileNames = new List<string>();
      if (!actions.IsEmpty)
      {
        Attachments attachments = actions[0].GetAttachments();
        foreach (TeamSupport.Data.Attachment attachment in attachments)
        {
          fileNames.Add(attachment.Path);
          Logs.WriteEventFormat("Adding Attachment   AttachmentID:{0}, ActionID:{1}, Path:{2}", attachment.AttachmentID.ToString(), actions[0].ActionID.ToString(), attachment.Path);
        }
      }

      if (ticket.UserID != null && oldUserID != null)
      {
        Logs.WriteEvent("Ticket.UserID is not null AND old userid is not null");
        User owner = Users.GetUser(LoginUser, (int)ticket.UserID);
        Logs.WriteEventFormat("Owner: {0} ({1}) - ReceiveTicketNotifications:{2}", owner.DisplayName, owner.UserID.ToString(), owner.ReceiveTicketNotifications.ToString());
        Logs.WriteParam("ModifierID", modifierID.ToString());
        if (modifierID != owner.UserID && owner.ReceiveTicketNotifications)
        {
          Logs.WriteEvent("Getting Ticket Assignment Email");
          MailMessage message = EmailTemplates.GetTicketAssignmentUser(LoginUser, modifierName, ticket.GetTicketView());
          message.To.Add(new MailAddress(owner.Email, owner.FirstLastName));
          message.Subject = message.Subject + subject;
          Logs.WriteEvent("Added to ticket log");
          ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "Ticket assignment email sent to " + message.To[0].DisplayName);
          AddMessage(ticketOrganization.OrganizationID, "Ticket Assignment [" + ticket.TicketNumber.ToString() + "]", message, ticket.EmailReplyToAddress, fileNames.ToArray());
        }
      }

      if (ticket.GroupID != null && oldGroupID != null)
      {
        Logs.WriteEvent("Ticket.GroupID is not null AND oldGroupID is not null");
        Group owner = Groups.GetGroup(LoginUser, (int)ticket.GroupID);
        Logs.WriteEventFormat("Group Owner: {0} ({1})", owner.Name, owner.GroupID.ToString());
        List<UserEmail> list = new List<UserEmail>();
        AddTicketOwners(list, ticket);
        if (ticket.UserID != null)
        {
          RemoveUser(list, (int)ticket.UserID);
          Logs.WriteParam("Removing UserID from list: ", ticket.UserID.ToString());
        }
        MailMessage message = EmailTemplates.GetTicketAssignmentGroup(LoginUser, modifierName, ticket.GetTicketView());
        AddUsersToAddresses(message.To, list, modifierID);
        message.Subject = message.Subject + subject;
        foreach (MailAddress mailAddress in message.To)
        {
          ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "Ticket assignment email sent to " + mailAddress.Address);
        }
        AddMessage(ticketOrganization.OrganizationID, "Ticket Assignment [" + ticket.TicketNumber.ToString() + "]", message, ticket.EmailReplyToAddress, fileNames.ToArray());

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
      foreach (MailAddress mailAddress in message.To)
      {
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "Ticket modified email sent to " + mailAddress.Address);
      }
      AddMessage(ticketOrganization.OrganizationID, "Internal Ticket Modified [" + ticket.TicketNumber.ToString() + "]", message, ticket.EmailReplyToAddress, fileNames.ToArray());

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
        Logs.WriteEvent("Ticket not visible on portal, returning.");

        return;
      }
      
      if (oldTicketStatusID == null && !includeActions && users.Length < 1)
      {
        Logs.WriteEvent("Old status id is null AND there are no actions AND no users specified, returning.");
        return;
      }

      List<UserEmail> userList;
      List<UserEmail> advUsers = new List<UserEmail>();
      AddAdvancedPortalUsers(advUsers, ticket);


      if (isBasic)
      {
        Logs.WriteEvent("Processing basic portal code");
        userList = new List<UserEmail>();
        AddBasicPortalUsers(userList, ticket);
        Logs.WriteEvent("Adding Basic Portal Users");
        foreach (UserEmail basicUserLog in userList) { Logs.WriteEvent("  + " + basicUserLog.ToString());}


        if (!string.IsNullOrEmpty(ticket.PortalEmail))
        {
          Logs.WriteEvent(string.Format("Ticket.PortalEmail: {0}", ticket.PortalEmail));
          UserEmail email = FindByUserEmail(ticket.PortalEmail, userList);
          if (email == null)
          {
            Logs.WriteEvent("Adding portal email to list");

            userList.Add(new UserEmail(-2, "", "", ticket.PortalEmail, false));
          }
        }
        else
	      {
          Logs.WriteEvent("Ticket.PortalEmail: None");
        }

        foreach (UserEmail item in advUsers)
        {
          UserEmail email = FindByUserEmail(item.Address, userList);
          if (email != null)
          {
            userList.Remove(email);
            Logs.WriteEvent(string.Format("Removing Advance Portal User Email From List: {0}", email));
          }
        }
      }
      else
      {
        userList = advUsers;
        Logs.WriteEvent("Adding Advanced Portal Users");
        foreach (UserEmail advuserlog in advUsers) { Logs.WriteEvent("  + " + advuserlog.ToString()); }

      }

      if (oldTicketStatusID == null && !includeActions)
      {
        Logs.WriteEvent("Old Status ID is NULL AND no actions included, making new list from EmailPost.Param8 (Users)");
        List<UserEmail> newList = new List<UserEmail>();
        foreach (UserEmail email in userList)
        {
          if (users.Contains(email.UserID))
          {
            newList.Add(email);
            Logs.WriteEvent(string.Format("Added Email: {0}", email));

          }
        }
        userList = newList;
      }

      TicketStatus status = TicketStatuses.GetTicketStatus(LoginUser, ticket.TicketStatusID);
      //if (isNew || (status.IsEmailResponse == true && oldTicketStatusID != null)) 
      //{
      RemoveUser(userList, modifierID);
      Logs.WriteEvent(string.Format("Removing Modifier user from list: {0}", modifierID.ToString()));

      //}


      if (userList.Count < 1)
      {
        Logs.WriteEvent("User list has count of 0, returning");

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
            Logs.WriteEvent(string.Format("Adding Attachment   AttachmentID:{0}, ActionID:{1}, Path:{2}", attachment.AttachmentID.ToString(), actions[0].ActionID.ToString(), attachment.Path));

          }

        }
      }

      MailMessage message = null;
      string messageType;
      if (status.IsClosedEmail)
      {
        Logs.WriteEvent("Status is closed email, getting template");

        messageType = "Ticket closed email";
        message = EmailTemplates.GetTicketClosed(LoginUser, modifierName, ticket.GetTicketView(), includeActions);
      
      }
      else
      {
        Logs.WriteEvent("Getting portal email template");

        message = isBasic ? EmailTemplates.GetTicketUpdateBasicPortal(LoginUser, modifierName, ticket.GetTicketView(), includeActions) :
                            EmailTemplates.GetTicketUpdateAdvPortal(LoginUser, modifierName, ticket.GetTicketView(), includeActions);
        messageType = isBasic ? "Basic portal email" : "Advanced portal email";
      }

      if (ticketOrganization.AddEmailViaTS) message.Body = message.Body + GetViaTSHtmlAd(ticketOrganization.Name);

      string subject = message.Subject;
      string body = message.Body;

      foreach (UserEmail userEmail in userList)
	    {
        Logs.WriteEvent("Adding address to email message.");
        try
        {
          if (userEmail != null && modifierID != userEmail.UserID)
          {
            message.Body = body;
            message.Subject = subject;
            message.To.Clear();
            MailAddress mailAddress = new MailAddress(userEmail.Address, userEmail.Name);
            message.To.Add(mailAddress);
            //collection.Add(new MailAddress(userEmail.Address, userEmail.Name));
            ContactsViewItem contact = ContactsView.GetContactsViewItem(_loginUser, userEmail.UserID);
            EmailTemplate.ReplaceMessageFields(_loginUser, "Recipient", contact, message, -1, ticket.OrganizationID);
            AgentRatingsOptions options = new AgentRatingsOptions(_loginUser);
            options.LoadByOrganizationID(ticket.OrganizationID);
            string baseUrl = SystemSettings.ReadString(_loginUser, "AppDomain", "https://app.teamsupport.com");
            string redirectUrl = "https://portal.teamsupport.com/rating.aspx";
            string positiveUrl = baseUrl + "/vcr/1_9_0/images/face-positive.png";
            string neutralUrl = baseUrl + "/vcr/1_9_0/images/face-neutral.png";
            string negativeUrl = baseUrl + "/vcr/1_9_0/images/face-negative.png";

            if (!options.IsEmpty)
            {
              AgentRatingsOption option = options[0];
              if (!string.IsNullOrWhiteSpace(option.RedirectURL)) redirectUrl = option.RedirectURL;
              if (!string.IsNullOrWhiteSpace(option.PositiveImage)) positiveUrl = baseUrl + option.PositiveImage;
              if (!string.IsNullOrWhiteSpace(option.NeutralImage)) neutralUrl = baseUrl + option.NeutralImage;
              if (!string.IsNullOrWhiteSpace(option.NegativeImage)) negativeUrl = baseUrl + option.NegativeImage;
            }

            StringBuilder ratingLink = new StringBuilder();
            ratingLink.Append("<a href=\"");
            ratingLink.Append(redirectUrl);
            ratingLink.Append(redirectUrl.IndexOf("?") > -1 ? "&" : "?");
            ratingLink.Append("OrganizationID=" + ticket.OrganizationID.ToString());
            ratingLink.Append("&TicketID=" + ticket.TicketID.ToString());
            ratingLink.Append("&Rating={0}");
            ratingLink.Append("&CustomerID=" + (contact == null ? "-1" : contact.UserID.ToString()));
            ratingLink.Append("\"><img src=\"{1}\" width=\"50\" height=\"50\" /></a>");
            string link = ratingLink.ToString();
            EmailTemplate.ReplaceMessageParameter(_loginUser, "AgentRatingsImageLink.Positive", string.Format(link, "1", positiveUrl), message, -1, ticket.OrganizationID);
            EmailTemplate.ReplaceMessageParameter(_loginUser, "AgentRatingsImageLink.Neutral", string.Format(link, "0", neutralUrl), message, -1, ticket.OrganizationID);
            EmailTemplate.ReplaceMessageParameter(_loginUser, "AgentRatingsImageLink.Negative", string.Format(link, "-1", negativeUrl), message, -1, ticket.OrganizationID);
            Logs.WriteEvent(string.Format("Email: {0} <{1}>", userEmail.Name, userEmail.Address));
            AddMessage(ticketOrganization.OrganizationID, "Portal Ticket Modified [" + ticket.TicketNumber.ToString() + "]", message, ticket.EmailReplyToAddress, fileNames.ToArray());
            Logs.WriteEvent("Adding action log to ticket");
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, messageType + " sent to " + mailAddress.Address);
          }
        }
        catch
        {
        }
	    }

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

      foreach (MailAddress mailAddress in message.To)
      {
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "Ticket update request email sent to " + mailAddress.Address);
      }
      AddMessage(ticketOrganization.OrganizationID, "Ticket Update Request [" + ticket.TicketNumber.ToString() + "]", message, ticket.EmailReplyToAddress);

    }

    private void ProcessTicketSendEmail(int userID, int ticketID, string addresses, string introduction)
    {
      Ticket ticket = Tickets.GetTicket(LoginUser, ticketID);
      User sender = Users.GetUser(LoginUser, userID);
      if (ticket == null || sender == null || ticket.OrganizationID != sender.OrganizationID) return;
      LoginUser loginUser = new LoginUser(LoginUser.ConnectionString, sender.UserID, sender.OrganizationID, null);

      Actions actions = new Actions(LoginUser);
      actions.LoadByTicketID(ticket.TicketID);

      List<string> fileNames = new List<string>();

      foreach (TeamSupport.Data.Action action in actions)
      {
        if (!action.IsVisibleOnPortal) continue;
        Attachments attachments = action.GetAttachments();
        foreach (TeamSupport.Data.Attachment attachment in attachments)
        {
          fileNames.Add(attachment.Path);
          Logs.WriteEventFormat("Adding Attachment   AttachmentID:{0}, ActionID:{1}, Path:{2}", attachment.AttachmentID.ToString(), actions[0].ActionID.ToString(), attachment.Path);
        }
      }


      char split = ';';
      if (addresses.IndexOf(';') < 0) split = ',';

      string[] list = addresses.Split(split);
      foreach (string item in list)
      {
        try
        {
          MailMessage message = EmailTemplates.GetTicketSendEmail(LoginUser, sender.GetUserView(), ticket.GetTicketView(), item.Trim(), introduction);
          message.To.Add(new MailAddress(item.Trim()));
          message.Subject = message.Subject;

          foreach (MailAddress mailAddress in message.To)
          {
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "Ticket email sent to " + mailAddress.Address);
          }
          AddMessage(sender.OrganizationID, "Ticket Email [" + ticket.TicketNumber.ToString() + "] to " + item, message, ticket.EmailReplyToAddress, fileNames.ToArray());

        }
        catch (Exception ex) {
          ExceptionLogs.LogException(loginUser, ex, "Email Processor", "Email Ticket");
        }
      }

      //ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "Sent this ticket via email to: " + addresses);

    }

    private void AddMessageNewTicket(Ticket ticket, User modifier, Organization ticketOrganization)
    {
      if (string.IsNullOrEmpty(ticket.TicketSource)) return;

      string source = ticket.TicketSource.ToLower().Trim();
      if (source != "web" && source != "email" && source != "chatoffline") return;

      if (!ticket.IsVisibleOnPortal || ticket.GetTicketView().IsClosed) return;

      if (modifier == null && string.IsNullOrEmpty(ticket.PortalEmail)) return;
      MailAddress modifierAddress = (modifier == null) ? new MailAddress(ticket.PortalEmail) : new MailAddress(modifier.Email, modifier.FirstLastName);

      if (modifier != null && modifier.OrganizationID == ticketOrganization.OrganizationID) // internal
      {
        MailMessage message = EmailTemplates.GetNewTicketInternal(LoginUser, modifier.GetUserView(), ticket.GetTicketView());
        message.To.Add(modifierAddress);
        foreach (MailAddress mailAddress in message.To)
        {
          ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "New ticket email sent to " + mailAddress.Address);
        }
        AddMessage(ticket.OrganizationID, "Internal New Ticket [" + ticket.TicketNumber.ToString() + "]", message, ticket.EmailReplyToAddress);

      }
      else // portal
      {
        MailMessage message = modifier != null && modifier.IsPortalUser ? EmailTemplates.GetNewTicketAdvPortal(LoginUser, modifier.GetUserView(), ticket.GetTicketView()) :
                                                                          EmailTemplates.GetNewTicketBasicPortal(LoginUser, modifierAddress, ticket.GetTicketView());
        message.To.Add(modifierAddress);
        foreach (MailAddress mailAddress in message.To)
        {
          ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "New ticket portal email sent to " + mailAddress.Address);
        }
        AddMessage(ticketOrganization.OrganizationID, "New Ticket [" + ticket.TicketNumber.ToString() + "]", message, ticket.EmailReplyToAddress);

      }
      

    }

    #endregion

    public void ProcessSignUpNotification(int userID)
    {
      User user = Users.GetUser(LoginUser, userID);
      Organization organization = Organizations.GetOrganization(LoginUser, user.OrganizationID);
      MailMessage message = EmailTemplates.GetSignUpNotification(LoginUser, user);
      message.From = new MailAddress("sales@teamsupport.com", "TeamSupport.com");
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
      message.To.Add(new MailAddress(user.Email));
      message.Bcc.Add(new MailAddress("dropbox@79604342.murocsystems.highrisehq.com"));
      message.From = new MailAddress(from);
      AddMessage(1078, "New Sign Up - Welcome [" + organization.Name + "]", message);

      message = EmailTemplates.GetWelcomeNewSignUp(LoginUser, user.GetUserView(), password, DateTime.Now.AddDays(14).ToString("MMMM d, yyyy"), 27);
      message.To.Add(new MailAddress(user.Email));
      message.Bcc.Add(new MailAddress("eharrington@teamsupport.com"));
      message.From = new MailAddress("eharrington@teamsupport.com", "Eric Harrington");
      AddMessage(1078, "New Sign Up - Check In[" + organization.Name + "]", message, null, null, DateTime.UtcNow.AddDays(1));

      message = EmailTemplates.GetWelcomeNewSignUp(LoginUser, user.GetUserView(), password, DateTime.Now.AddDays(14).ToString("MMMM d, yyyy"), 28);
      message.To.Add(new MailAddress(user.Email));
      message.Bcc.Add(new MailAddress("eharrington@teamsupport.com"));
      message.From = new MailAddress(from);
      AddMessage(1078, "New Sign Up - Notice [" + organization.Name + "]", message, null, null, DateTime.UtcNow.AddDays(10));
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
      if (user == null || user.Email == null) return;
      if (IsUserAlreadyInList(list, user.UserID) || (!user.ReceiveTicketNotifications && honorTicketNotifications)) return;
      list.Add(new UserEmail(user.UserID, user.FirstName, user.LastName, user.Email, user.OnlyEmailAfterHours));
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

    private void RemoveBusinessHoursUsers(List<UserEmail> list, Ticket ticket)
    {
      Logs.WriteEvent("Removing Busniess Hours User");
      Organization organization = Organizations.GetOrganization(LoginUser, ticket.OrganizationID);

      Logs.WriteParam("Ticket Organization", ticket.OrganizationID.ToString());
      Logs.WriteParam("Ticket DateCreated UTC", ticket.DateCreatedUtc.ToString());
      Logs.WriteParam("Business Day End", organization.BusinessDayEndUtc.ToString());
      Logs.WriteParam("Business Day Start", organization.BusinessDayStartUtc.ToString());
      Logs.WriteParam("Business Days", organization.BusinessDaysText);
      Logs.WriteParam("Now", DateTime.UtcNow.ToString());
      if (!organization.IsInBusinessHours(ticket.DateCreatedUtc))
      {
        Logs.WriteEvent("Ticket not created in business hours, returning"); 
         return;
      }
      List<UserEmail> removals = new List<UserEmail>();
      foreach (UserEmail item in list)
      {
        if (item.SendTicketsOnlyAfterHours && (ticket.UserID == null || item.UserID != ticket.UserID)) 
        {
          ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, "Email not sent to " + item.Name + " during business hours");
          removals.Add(item);
          Logs.WriteEventFormat("Removing Business Hours User: {0} ({1}) <{2}> -> (item.SentTicketsOnlyAfterHours: {3} AND (ticket.UserID == null OR item.UserID != ticket.UserID))", item.Name, item.UserID.ToString(),
            item.Address, item.SendTicketsOnlyAfterHours.ToString());
        }
      }

      foreach (UserEmail item in removals)
      {
        list.Remove(item);
      }
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
      Logs.WriteEvent("Adding Ticket Owners");
      if (ticket.UserID != null)
      {
        User user =  Users.GetUser(LoginUser, (int)ticket.UserID);
        AddUser(userList, user, true);
        Logs.WriteEventFormat("Adding Assigned User: {0} ({1})", user.DisplayName, user.UserID.ToString());
      }
      
      if (ticket.GroupID != null)
      {
        Logs.WriteEvent("Ticket.GroupID is NOT NULL");
        Logs.WriteEvent("Loading ticket group users");
        Users users = new Users(LoginUser);
        users.LoadByGroupID((int)ticket.GroupID);
        foreach (User user in users) {
          if (ticket.UserHasRights(user) && (user.ReceiveAllGroupNotifications || ticket.UserID == null))
          {
            AddUser(userList, user, true);
            Logs.WriteEventFormat("{0} ({1}) <{2}> was added to the list", user.DisplayName, user.UserID.ToString(), user.Email);
          }
          else
          {
            Logs.WriteEventFormat("{0} ({1}) <{2}> was DENIED to the list. || Ticket.UserHasRights = {3} AND (ReceiveAllGroupNotifications = {4} OR ticket.UserID is NULL)", user.DisplayName, user.UserID.ToString(), user.Email, ticket.UserHasRights(user).ToString(), user.ReceiveAllGroupNotifications.ToString() ,(ticket.UserID==null).ToString());
          }
        }
        if (ticket.UserID != null) RemoveBusinessHoursUsers(userList, ticket);
      }
    }

    private void AddTicketSubscribers(List<UserEmail> userList, Ticket ticket)
    {
      Users users;

      // Ticket Subscribers
      users = new Users(LoginUser);
      users.LoadByTicketSubscription(ticket.TicketID);
      foreach (User user in users) { 
        if (ticket.UserHasRights(user)) AddUser(userList, user); 
      }

      // Customer Subscribers
      users = new Users(LoginUser);
      users.LoadByCustomerSubscription(ticket.TicketID);
      foreach (User user in users) {
        if (ticket.UserHasRights(user)) AddUser(userList, user); 
      }
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
      Logs.WriteEvent("Adding addresses to email message.");
      foreach (UserEmail user in users)
      {
        try
        {
          if (user != null && modifierID != user.UserID)
          {
            collection.Add(new MailAddress(user.Address, user.Name));
            Logs.WriteEvent(string.Format("Email: {0} <{1}>", user.Name, user.Address));

          }
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

    private string GetViaTSHtmlAd(string orgName)
    { 
      return string.Format(@"
<hr style=""border:none 0;border-top:1px solid #dedede;min-height:1px;margin:15px 0 10px 0"">
<div style=""margin:0px auto;text-align:left;font-size:12px;color:#aaaaaa;padding-bottom:6px;font-family:Arial,Helvetica,sans-serif;"">This email was sent from {0} via <a href=""http://www.teamsupport.com/?utm_source=tsemail&utm_medium=email&utm_campaign=custmail"" title=""TeamSupport.com"" style=""color:#2774a6;text-decoration:none;font-family:Arial,Helvetica,sans-serif;font-size:12px;"" target=""_blank"">TeamSupport</a></div>
", orgName);
    
    
    }

    #endregion

 
  }
}
