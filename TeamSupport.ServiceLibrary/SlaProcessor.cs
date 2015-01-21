using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using System.Data.SqlClient;
using System.Data;
using System.Net.Mail;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  public class SlaProcessor : ServiceThread
  {
    public SlaProcessor()
    {
    }

    public override void Run()
    {
      try
      {

        Tickets tickets = new Tickets(LoginUser);
        tickets.LoadAllUnnotifiedAndExpiredSla();

        foreach (Ticket ticket in tickets)
        {
          if (IsStopped) break;
          ProcessTicket(ticket);
          System.Threading.Thread.Sleep(0);
        }
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "SLA Processor", "Sync");
      }
    }


    private void ProcessTicket(Ticket ticket)
    {
      UpdateHealth();
      SlaTriggersView triggers = new SlaTriggersView(LoginUser);
      triggers.LoadByTicket(ticket.TicketID);
      bool warnGroup = false;
      bool warnUser = false;
      bool vioGroup = false;
      bool vioUser = false;

      foreach (SlaTriggersViewItem item in triggers)
      {
        warnGroup = item.NotifyGroupOnWarning || warnGroup;
        warnUser = item.NotifyUserOnWarning || warnUser;
        vioGroup = item.NotifyGroupOnViolation || vioGroup;
        vioUser = item.NotifyUserOnViolation || vioUser;
      }

      SlaNotification notification = SlaNotifications.GetSlaNotification(LoginUser, ticket.TicketID);
      if (notification == null)
      {
        notification = (new SlaNotifications(LoginUser)).AddNewSlaNotification();
        notification.TicketID = ticket.TicketID;
      }

      DateTime notifyTime;

      if (ticket.SlaViolationInitialResponse != null && ticket.SlaViolationInitialResponseUtc <= DateTime.UtcNow)
      {

        notifyTime = (DateTime)ticket.SlaViolationInitialResponseUtc;
        if (!IsTooOld(notifyTime))
        {
          if (notification.InitialResponseViolationDate == null || Math.Abs(((DateTime)notification.InitialResponseViolationDateUtc - notifyTime).TotalMinutes) > 5)
          {
            NotifyViolation(ticket.TicketID, vioUser, vioGroup, false, SlaViolationType.InitialResponse, notification);
            notification.InitialResponseViolationDate = notifyTime;
          }
        }
      }
      else if (ticket.SlaWarningInitialResponse != null && ticket.SlaWarningInitialResponseUtc <= DateTime.UtcNow)
      {
        notifyTime = (DateTime)ticket.SlaWarningInitialResponseUtc;

        if (!IsTooOld(notifyTime))
        {
          if (notification.InitialResponseWarningDate == null || Math.Abs(((DateTime)notification.InitialResponseWarningDateUtc - notifyTime).TotalMinutes) > 5)
          {
            NotifyViolation(ticket.TicketID, warnUser, warnGroup, true, SlaViolationType.InitialResponse, notification);
            notification.InitialResponseWarningDate = notifyTime;
          }
        }
      }


      if (ticket.SlaViolationLastAction != null && ticket.SlaViolationLastActionUtc <= DateTime.UtcNow)
      {
        notifyTime = (DateTime)ticket.SlaViolationLastActionUtc;

        if (!IsTooOld(notifyTime))
        {
          if (notification.LastActionViolationDate == null || Math.Abs(((DateTime)notification.LastActionViolationDateUtc - notifyTime).TotalMinutes) > 5)
          {
            NotifyViolation(ticket.TicketID, vioUser, vioGroup, false, SlaViolationType.LastAction, notification);
            notification.LastActionViolationDate = notifyTime;
          }
        }
      }
      else if (ticket.SlaWarningLastAction != null && ticket.SlaWarningLastActionUtc <= DateTime.UtcNow)
      {
        notifyTime = (DateTime)ticket.SlaWarningLastActionUtc;

        if (!IsTooOld(notifyTime))
        {
          if (notification.LastActionWarningDate == null || Math.Abs(((DateTime)notification.LastActionWarningDateUtc - notifyTime).TotalMinutes) > 5)
          {
            NotifyViolation(ticket.TicketID, warnUser, warnGroup, true, SlaViolationType.LastAction, notification);
            notification.LastActionWarningDate = notifyTime;
          }
        }
      }


      if (ticket.SlaViolationTimeClosed != null && ticket.SlaViolationTimeClosedUtc <= DateTime.UtcNow)
      {
        notifyTime = (DateTime)ticket.SlaViolationTimeClosedUtc;

        if (!IsTooOld(notifyTime))
        {
          if (notification.TimeClosedViolationDate == null || Math.Abs(((DateTime)notification.TimeClosedViolationDateUtc - notifyTime).TotalMinutes) > 5)
          {
            NotifyViolation(ticket.TicketID, vioUser, vioGroup, false, SlaViolationType.TimeClosed, notification);
            notification.TimeClosedViolationDate = notifyTime;
          }
        }
      }
      else if (ticket.SlaWarningTimeClosed != null && ticket.SlaWarningTimeClosedUtc <= DateTime.UtcNow)
      {
        notifyTime = (DateTime)ticket.SlaWarningTimeClosedUtc;

        if (!IsTooOld(notifyTime))
        {
          if (notification.TimeClosedWarningDate == null || Math.Abs(((DateTime)notification.TimeClosedWarningDateUtc - notifyTime).TotalMinutes) > 5)
          {
            NotifyViolation(ticket.TicketID, warnUser, warnGroup, true, SlaViolationType.TimeClosed, notification);
            notification.TimeClosedWarningDate = notifyTime;
          }
        }
      }

      notification.Collection.Save();
    }

    private bool IsTooOld(DateTime notifyTime)
    {
      return (DateTime.UtcNow - notifyTime).TotalDays >= 1;
    }


    private void NotifyViolation(int ticketID, bool useUser, bool useGroup, bool isWarning, SlaViolationType slaViolationType, SlaNotification notification)
    {
      Users users = new Users(LoginUser);
      User user = null;

      string violationType = "";
      switch (slaViolationType)
      {
        case SlaViolationType.InitialResponse: violationType = "Initial Resoponse"; break;
        case SlaViolationType.LastAction: violationType = "Last Action"; break;
        case SlaViolationType.TimeClosed: violationType = "Time to Close"; break;
        default: break;
      }

      Logs.WriteLine();
      Logs.WriteEvent("***** Procdessing TicketID: " + ticketID.ToString());
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(LoginUser, ticketID);
      if (ticket == null) { Logs.WriteEvent("Ticket is NULL, exiting"); return; }

      
      if (!isWarning)
      {
        SlaViolationHistoryItem history = (new SlaViolationHistory(LoginUser)).AddNewSlaViolationHistoryItem();
        history.DateViolated = DateTime.UtcNow;
        history.GroupID = ticket.GroupID;
        history.UserID = ticket.UserID;
        history.ViolationType = slaViolationType;
        history.TicketID = ticket.TicketID;
        history.Collection.Save();
      }

      Actions actions = new Actions(LoginUser);
      actions.LoadLatestByTicket(ticket.TicketID, false);
      



      Logs.WriteEvent(string.Format("NOTIFYING TicketID:{0}  TicketNumber:{1}  OrganizationID:{2} ", ticket.TicketID.ToString(), ticket.TicketNumber.ToString(), ticket.OrganizationID.ToString()));
      Logs.WriteEvent(string.Format("User:{1}  Group:{2}  IsWarning:{3}  NoficationType:{4}", ticketID.ToString(), useUser.ToString(), useGroup.ToString(), isWarning.ToString(), slaViolationType));
      Logs.WriteEvent("Ticket Data:");
      Logs.WriteData(ticket.Row);
      Logs.WriteEvent("Notification Data:");
      Logs.WriteData(notification.Row);

      ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, violationType + " SLA violation occured");

      MailMessage message = EmailTemplates.GetSlaEmail(LoginUser, ticket, violationType, isWarning);


      if (ticket.GroupID != null && useGroup)
      {
        users.LoadByGroupID((int)ticket.GroupID);
      }

      if (ticket.UserID != null && useUser && users.FindByUserID((int)ticket.UserID) == null)
      {
        user = Users.GetUser(LoginUser, (int)ticket.UserID);
      }

      foreach (User item in users)
      {
        message.To.Add(new MailAddress(item.Email, item.FirstLastName));
      }

      if (user != null)
      {
        message.To.Add(new MailAddress(user.Email, user.FirstLastName));
        Logs.WriteEvent(string.Format("Adding Main User, Name:{0}  Email:{1}  UserID:{2}", user.FirstLastName, user.Email, user.UserID.ToString()));

      }

      if (message.To.Count > 0)
      {
        Email email = Emails.AddEmail(LoginUser, ticket.OrganizationID, null, "Sla Message", message);
        Logs.WriteEvent("Email Added (EmailID: " + email.EmailID.ToString() + ")", true);        
      }

    }
    /*
    private void NotifyViolationOld(int ticketID, bool useUser, bool useGroup, bool isWarning, SlaViolationType SlaViolationType)
    {
      MailMessage message = new MailMessage();

      Users users = new Users(LoginUser);
      User user = null;
      Ticket ticket = Tickets.GetTicket(LoginUser, ticketID);
      if (ticket == null) return;

      if (ticket.GroupID != null && useGroup)
      {
        users.LoadByGroupID((int)ticket.GroupID);
      }

      if (ticket.UserID != null && useUser && users.FindByUserID((int)ticket.UserID) == null)
      {
        user = Users.GetUser(LoginUser, (int)ticket.UserID);
      }

      message.From = new MailAddress(Organizations.GetOrganization(LoginUser, ticket.OrganizationID).GetReplyToAddress());
      foreach (User item in users)
      {
        message.To.Add(new MailAddress(item.Email, item.FirstLastName));
      }

      if (user != null)
        message.To.Add(new MailAddress(user.Email, user.FirstLastName));

      if (isWarning)
        message.Subject = "Warning: Ticket [{0}] is about to violate an SLA";
      else
        message.Subject = "Violation: Ticket [{0}] has violated an SLA";

      message.Subject = string.Format(message.Subject, ticket.TicketNumber);

      string link = "<div><a href=\"" + SystemSettings.ReadString(LoginUser, "AppDomain", "https://app.teamsupport.com") + "?TicketID={0}\" target=\"TSMain\">Ticket {1}: {2}</a></div>";
      StringBuilder builder = new StringBuilder();
      string description = "";
      if (!isWarning)
      {
        switch (SlaViolationType)
        {
          case SlaViolationType.InitialResponse: 
            builder.Append("<div>The following ticket has violated an initial response Service Level Agreement.</div>");
            description = "SLA Violation: Initial Resoponse";
            break;
          case SlaViolationType.LastAction: 
            builder.Append("<div>The following ticket has violated a last action Service Level Agreement.</div>");
            description = "SLA Violation: Last Action";
            break;
          case SlaViolationType.TimeClosed: 
            builder.Append("<div>The following ticket has violated a time to close Service Level Agreement.</div>");
            description = "SLA Violation: Time to Close";
            break;
          default:
            break;
        }
        
      }
      else
      {
        switch (SlaViolationType)
        {
          case SlaViolationType.InitialResponse: 
            builder.Append("<div>The following ticket is about to violate an initial response Service Level Agreement.</div>"); 
            description = "SLA Warning: Initial Response";
            break;
          case SlaViolationType.LastAction: 
            builder.Append("<div>The following ticket is about to violate a last action Service Level Agreement.</div>"); 
            description = "SLA Warning: Last Action";
            break;
          case SlaViolationType.TimeClosed: 
            builder.Append("<div>The following ticket is about to violate a time to close Service Level Agreement.</div>");
            description = "SLA Warning: Time to Close";
            break;
          default:
            break;
        }
        
      }

      builder.Append(string.Format(link, ticket.TicketID, ticket.TicketNumber, ticket.Name));

      message.IsBodyHtml = true;
      message.Body = builder.ToString();
      SendMessage(ticket.OrganizationID,  description, message);

    }

    private void SendMessage(int organizationID, string description, MailMessage message)
    {
      StringBuilder builder = new StringBuilder();
      builder.AppendLine("<span class=\"TeamSupportStart\">&nbsp</span>");

      builder.AppendLine(message.Body);
      builder.AppendLine("<span class=\"TeamSupportEnd\">&nbsp</span>");

      message.Body = builder.ToString();

      if (message.To.Count > 0)
      { 
        Emails.AddEmail(LoginUser, organizationID, null, description, message);
      }
    }
*/
  }
}
