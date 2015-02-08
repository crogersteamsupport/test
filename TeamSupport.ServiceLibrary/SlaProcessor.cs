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
    DateTime _lastDLSAdjustment = DateTime.MinValue;

    public SlaProcessor()
    {
    }

    public override void Run()
    {
      try
      {
        try
        {
          UpdateBusinessHoursForDLSFix();
          if (DateTime.Now.Subtract(_lastDLSAdjustment).TotalMinutes > 60 && DateTime.Now.Minute > 5 && DateTime.Now.Minute < 30)
          {
            Logs.WriteEvent("Update business hours for DSL");
            _lastDLSAdjustment = DateTime.Now;
            UpdateBusinessHoursForDLSFix();
          }
        }
        catch (Exception)
        {
        }


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
      Logs.WriteEvent("***** Processing TicketID: " + ticketID.ToString());
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

      try
      {


        TimeZoneInfo tz = null;
        Organization org = Organizations.GetOrganization(LoginUser, ticket.OrganizationID);
        if (org.TimeZoneID != null) { tz = System.TimeZoneInfo.FindSystemTimeZoneById(org.TimeZoneID); }
        if (tz == null)
        {
          Logs.WriteEvent("Timezone is null, using system's");
          tz = System.TimeZoneInfo.Local;
        }
        Logs.WriteEvent(tz.DisplayName);
        Logs.WriteEvent("Supports DLS: " + tz.SupportsDaylightSavingTime.ToString());
        Logs.WriteEvent("Is DLS: " + tz.IsDaylightSavingTime(DateTime.UtcNow).ToString());
        Logs.WriteEvent("UTC: " + DateTime.UtcNow.ToString());
        Logs.WriteEvent(string.Format("NOTIFYING TicketID:{0}  TicketNumber:{1}  OrganizationID:{2} ", ticket.TicketID.ToString(), ticket.TicketNumber.ToString(), ticket.OrganizationID.ToString()));
        Logs.WriteEvent(string.Format("User:{1}  Group:{2}  IsWarning:{3}  NoficationType:{4}", ticketID.ToString(), useUser.ToString(), useGroup.ToString(), isWarning.ToString(), slaViolationType));
        Logs.WriteEvent("Ticket Data:");
        Logs.WriteData(ticket.Row);
        Logs.WriteEvent("Notification Data:");
        Logs.WriteData(notification.Row);
        Logs.WriteEvent("Organization Data:");
        Logs.WriteData(org.Row);
        Organizations customers = new Organizations(LoginUser);
        customers.LoadByTicketID(ticket.TicketID);

        foreach (Organization customer in customers)
        {
          Logs.WriteEvent("-- Customer: " + customer.Name);
          if (customer.SlaLevelID != null)
          {
            SlaLevel level = SlaLevels.GetSlaLevel(LoginUser, (int)customer.SlaLevelID);
            Logs.WriteEvent("SLA Level: " + level.Name);
            SlaTriggers triggers = new SlaTriggers(LoginUser);
            triggers.LoadByTicketTypeAndSeverity(level.SlaLevelID, ticket.TicketTypeID, ticket.TicketSeverityID);

            foreach (SlaTrigger trigger in triggers)
            {
              Logs.WriteData(trigger.Row);
            }          
          }
          else
          {
            Logs.WriteEvent("No SLA Level Assigned to " + customer.Name);
          }
  
        }

        if (org.InternalSlaLevelID != null)
        {
          Logs.WriteEvent("Internal SLA:");

          SlaTriggers triggers = new SlaTriggers(LoginUser);
          triggers.LoadByTicketTypeAndSeverity((int)org.InternalSlaLevelID, ticket.TicketTypeID, ticket.TicketSeverityID);
          foreach (SlaTrigger trigger in triggers)
          {
            Logs.WriteData(trigger.Row);
          }
        }
        else
        {
          Logs.WriteEvent("No Internal SLA");
        }
      }
      catch (Exception ex)
      {
        Logs.WriteEvent("Logging Exception:");
        Logs.WriteException(ex);
      }

    }
    
    private void UpdateBusinessHoursForDLSFix()
    {
      Organizations customers = new Organizations(LoginUser);
      customers.LoadTeamSupportCustomers();

      foreach (Organization customer in customers)
      {
        if (customer.OrganizationID == 13679)
        {
          DateTime today = DateTime.Now;
          DateTime start = customer.BusinessDayEnd;
          DateTime end = customer.BusinessDayStart;
          customer.BusinessDayStart = (new DateTime(today.Year, today.Month, today.Day, start.Hour, start.Minute, 0)).ToUniversalTime();
          customer.BusinessDayEnd = (new DateTime(today.Year, today.Month, today.Day, end.Hour, end.Minute, 0)).ToUniversalTime();

        }
      }
      customers.Save();
    }
  }
}
