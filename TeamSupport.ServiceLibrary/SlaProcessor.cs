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
    enum NotificationType { InitialResponse, LastAction, TimeClosed }

    public SlaProcessor()
    {
    }

    public override void Run()
    {
      try
      {

        TicketSlaView view = new TicketSlaView(LoginUser);
        view.LoadAllUnnotifiedAndExpired();

        foreach (TicketSlaViewItem item in view)
        {
          if (IsStopped) break;
          ProcessTicket(item);
          System.Threading.Thread.Sleep(0);
        }
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "SLA Processor", "Sync");
      }
    }


    private void ProcessTicket(TicketSlaViewItem ticketSlaViewItem)
    {
      UpdateHealth();
      SlaTriggersView triggers = new SlaTriggersView(LoginUser);
      triggers.LoadByTicket(ticketSlaViewItem.TicketID);

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

      SlaNotification notification = SlaNotifications.GetSlaNotification(LoginUser, ticketSlaViewItem.TicketID);
      if (notification == null)
      {
        notification = (new SlaNotifications(LoginUser)).AddNewSlaNotification();
        notification.TicketID = ticketSlaViewItem.TicketID;
      }

      DateTime notifyTime;

      if (ticketSlaViewItem.ViolationInitialResponse != null && ticketSlaViewItem.ViolationInitialResponse <= 0)
      {
        
        notifyTime = DateTime.UtcNow.AddMinutes((int)ticketSlaViewItem.ViolationInitialResponse);
        if (!IsTooOld(notifyTime))
        {
          if (notification.InitialResponseViolationDate == null || Math.Abs((notification.DateToUtc((DateTime)notification.InitialResponseViolationDate) - notifyTime).TotalMinutes) > 5)
          {
            NotifyViolation(ticketSlaViewItem.TicketID, vioUser, vioGroup, false, NotificationType.InitialResponse);
            notification.InitialResponseViolationDate = notifyTime;
          }
        }
      }
      else if (ticketSlaViewItem.WarningInitialResponse != null && ticketSlaViewItem.WarningInitialResponse <= 0)
      {
        notifyTime = DateTime.UtcNow.AddMinutes((int)ticketSlaViewItem.WarningInitialResponse);

        if (!IsTooOld(notifyTime))
        {
          if (notification.InitialResponseWarningDate == null || Math.Abs((notification.DateToUtc((DateTime)notification.InitialResponseWarningDate) - notifyTime).TotalMinutes) > 5)
          {
            NotifyViolation(ticketSlaViewItem.TicketID, warnUser, warnGroup, true, NotificationType.InitialResponse);
            notification.InitialResponseWarningDate = notifyTime;
          }
        }
      }


      if (ticketSlaViewItem.ViolationLastAction != null && ticketSlaViewItem.ViolationLastAction <= 0)
      {
        notifyTime = DateTime.UtcNow.AddMinutes((int)ticketSlaViewItem.ViolationLastAction);

        if (!IsTooOld(notifyTime))
        {
          if (notification.LastActionViolationDate == null || Math.Abs((notification.DateToUtc((DateTime)notification.LastActionViolationDate) - notifyTime).TotalMinutes) > 5)
          {
            NotifyViolation(ticketSlaViewItem.TicketID, vioUser, vioGroup, false, NotificationType.LastAction);
            notification.LastActionViolationDate = notifyTime;
          }
        }
      }
      else if (ticketSlaViewItem.WarningLastAction != null && ticketSlaViewItem.WarningLastAction <= 0)
      {
        notifyTime = DateTime.UtcNow.AddMinutes((int)ticketSlaViewItem.WarningLastAction);

        if (!IsTooOld(notifyTime))
        {
          if (notification.LastActionWarningDate == null || Math.Abs((notification.DateToUtc((DateTime)notification.LastActionWarningDate) - notifyTime).TotalMinutes) > 5)
          {
            NotifyViolation(ticketSlaViewItem.TicketID, warnUser, warnGroup, true, NotificationType.LastAction);
            notification.LastActionWarningDate = notifyTime;
          }
        }
      }


      if (ticketSlaViewItem.ViolationTimeClosed != null && ticketSlaViewItem.ViolationTimeClosed <= 0)
      {
        notifyTime = DateTime.UtcNow.AddMinutes((int)ticketSlaViewItem.ViolationTimeClosed);

        if (!IsTooOld(notifyTime))
        {
          if (notification.TimeClosedViolationDate == null || Math.Abs((notification.DateToUtc((DateTime)notification.TimeClosedViolationDate) - notifyTime).TotalMinutes) > 5)
          {
            NotifyViolation(ticketSlaViewItem.TicketID, vioUser, vioGroup, false, NotificationType.TimeClosed);
            notification.TimeClosedViolationDate = notifyTime;
          }
        }
      }
      else if (ticketSlaViewItem.WarningTimeClosed != null && ticketSlaViewItem.WarningTimeClosed <= 0)
      {
        notifyTime = DateTime.UtcNow.AddMinutes((int)ticketSlaViewItem.WarningTimeClosed);

        if (!IsTooOld(notifyTime))
        {
          if (notification.TimeClosedWarningDate == null || Math.Abs((notification.DateToUtc((DateTime)notification.TimeClosedWarningDate) - notifyTime).TotalMinutes) > 5)
          {
            NotifyViolation(ticketSlaViewItem.TicketID, warnUser, warnGroup, true, NotificationType.TimeClosed);
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


    private void NotifyViolation(int ticketID, bool useUser, bool useGroup, bool isWarning, NotificationType notificationType)
    {

      Users users = new Users(LoginUser);
      User user = null;

      
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(LoginUser, ticketID);
      if (ticket == null) return;

      string violationType = "";
      switch (notificationType)
      {
        case NotificationType.InitialResponse: violationType = "Initial Resoponse"; break;
        case NotificationType.LastAction: violationType = "Last Action"; break;
        case NotificationType.TimeClosed: violationType = "Time to Close"; break;
        default: break;
      }

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
        message.To.Add(new MailAddress(user.Email, user.FirstLastName));

      if (message.To.Count > 0)
      {
        Emails.AddEmail(LoginUser, ticket.OrganizationID, null, "Sla Message", message);
      }

    }
    /*
    private void NotifyViolationOld(int ticketID, bool useUser, bool useGroup, bool isWarning, NotificationType notificationType)
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
        switch (notificationType)
        {
          case NotificationType.InitialResponse: 
            builder.Append("<div>The following ticket has violated an initial response Service Level Agreement.</div>");
            description = "SLA Violation: Initial Resoponse";
            break;
          case NotificationType.LastAction: 
            builder.Append("<div>The following ticket has violated a last action Service Level Agreement.</div>");
            description = "SLA Violation: Last Action";
            break;
          case NotificationType.TimeClosed: 
            builder.Append("<div>The following ticket has violated a time to close Service Level Agreement.</div>");
            description = "SLA Violation: Time to Close";
            break;
          default:
            break;
        }
        
      }
      else
      {
        switch (notificationType)
        {
          case NotificationType.InitialResponse: 
            builder.Append("<div>The following ticket is about to violate an initial response Service Level Agreement.</div>"); 
            description = "SLA Warning: Initial Response";
            break;
          case NotificationType.LastAction: 
            builder.Append("<div>The following ticket is about to violate a last action Service Level Agreement.</div>"); 
            description = "SLA Warning: Last Action";
            break;
          case NotificationType.TimeClosed: 
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
