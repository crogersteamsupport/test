using System;
using System.Linq;
using System.Net.Mail;
using System.Collections.Generic;
//using Microsoft.AspNet.SignalR.Client;//ToDo //vv Not Yet. Per Kevin, services service need to be able to access the signalr server first. To do at a future date.
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
    [Serializable]
    public class SlaCalculator : ServiceThread
    {
        public SlaCalculator()
        {
        }

        public override void Run()
        {
            try
            {
                SlaTickets slaTickets = new SlaTickets(LoginUser);
                slaTickets.LoadPending();

                if (slaTickets != null && slaTickets.Count > 0)
                {
                    Logs.WriteEvent(string.Format("{0} pending tickets to calculate the SLA values for.", slaTickets.Count));
                }

                foreach (SlaTicket slaTicket in slaTickets)
                {
                    if (IsStopped) break;
                    UpdateHealth();

                    try
                    {
                        Ticket ticket = Tickets.GetTicket(LoginUser, slaTicket.TicketId);

                        if (ticket != null)
                        {
                            bool isClosed = ticket.DateClosed != null;
                            DateTime? newSlaViolationTimeClosed = null;
                            DateTime? newSlaWarningTimeClosed = null;
                            DateTime? newSlaViolationLastAction = null;
                            DateTime? newSlaWarningLastAction = null;
                            DateTime? newSlaViolationInitialResponse = null;
                            DateTime? newSlaWarningInitialResponse = null;

                            if (!isClosed)
                            {
                                DateTime? lastActionDateCreated = Actions.GetLastActionDateCreated(LoginUser, ticket.TicketID);
                                int totalActions = Actions.TotalActionsForSla(LoginUser, ticket.TicketID);

                                Organization organization = Organizations.GetOrganization(LoginUser, ticket.OrganizationID);
                                SlaTrigger slaTrigger = SlaTriggers.GetSlaTrigger(LoginUser, slaTicket.SlaTriggerId);

                                if (slaTrigger != null)
                                {
                                    Logs.WriteEventFormat("Trigger {0} not found.", slaTicket.SlaTriggerId);
                                }

                                SlaTickets.BusinessHours businessHours = new SlaTickets.BusinessHours()
                                {
                                    DayStartUtc = organization.BusinessDayStartUtc,
                                    DayEndUtc = organization.BusinessDayEndUtc,
                                    BusinessDays = organization.BusinessDays
                                };

                                //Check if we should use SLA's business hours instead of Account's
                                if (!slaTrigger.UseBusinessHours
                                    && !slaTrigger.NoBusinessHours
                                    && slaTrigger.DayStartUtc.HasValue
                                    && slaTrigger.DayEndUtc.HasValue)
                                {
                                    businessHours.DayStartUtc = slaTrigger.DayStartUtc.Value;
                                    businessHours.DayEndUtc = slaTrigger.DayEndUtc.Value;
                                    businessHours.BusinessDays = slaTrigger.Weekdays;
                                }
                                else if (!slaTrigger.UseBusinessHours && !slaTrigger.NoBusinessHours)
                                {
                                    Logs.WriteEventFormat("Using Account's business hours {0} to {1} because while the trigger is set to use sla's business hours one of them has no value. Sla DayStartUtc {2}, Sla DayEndUtc {3}",
                                        organization.BusinessDayStartUtc.ToShortTimeString(),
                                        organization.BusinessDayEndUtc.ToShortTimeString(),
                                        slaTrigger.DayStartUtc.HasValue ? slaTrigger.DayStartUtc.Value.ToShortTimeString() : "NULL",
                                        slaTrigger.DayEndUtc.HasValue ? slaTrigger.DayEndUtc.Value.ToShortTimeString() : "NULL");
                                }

                                Logs.WriteEvent(string.Format("Ticket #{0} id {1}. LastAction: {2}, TotalActions: {3}, Org({4}) {5}, SlaTriggerId {6}.", ticket.TicketNumber,
                                                                                                                                                        ticket.TicketID,
                                                                                                                                                        lastActionDateCreated == null ? "none" : lastActionDateCreated.Value.ToString(),
                                                                                                                                                        totalActions,
                                                                                                                                                        organization.OrganizationID,
                                                                                                                                                        organization.Name,
                                                                                                                                                        slaTrigger.SlaTriggerID));
                                List<DateTime> daysToPause = SlaTriggers.GetSpecificDaysToPause(slaTrigger.SlaTriggerID);
                                bool pauseOnHoliday = slaTrigger.PauseOnHoliday;
                                CalendarEvents holidays = new CalendarEvents(LoginUser);

                                if (pauseOnHoliday)
                                {
                                    holidays.LoadHolidays(organization.OrganizationID);
                                }

                                Dictionary<int, double> businessPausedTimes = new Dictionary<int, double>();
                                TimeSpan pausedTimeSpan = SlaTickets.CalculatePausedTime(ticket.TicketID, organization, businessHours, slaTrigger, daysToPause, holidays, LoginUser, businessPausedTimes, Logs);
                                Logs.WriteEventFormat("Total Paused Time: {0}", pausedTimeSpan.ToString());

                                UpdateBusinessPausedTimes(LoginUser, businessPausedTimes); //vv

                                newSlaViolationTimeClosed = SlaTickets.CalculateSLA(ticket.DateCreatedUtc, businessHours, slaTrigger, slaTrigger.TimeToClose, pausedTimeSpan, daysToPause, holidays);

                                if (newSlaViolationTimeClosed != null)
                                {
                                    newSlaWarningTimeClosed = SlaTickets.CalculateSLAWarning(newSlaViolationTimeClosed.Value,
                                                                                    businessHours,
                                                                                    slaTrigger.NoBusinessHours,
                                                                                    slaTrigger.TimeToClose,
                                                                                    slaTrigger.WarningTime,
                                                                                    daysToPause,
                                                                                    holidays);
                                }

                                if (lastActionDateCreated == null)
                                {
                                    newSlaViolationLastAction = null;
                                    newSlaWarningLastAction = null;
                                }
                                else
                                {
                                    newSlaViolationLastAction = SlaTickets.CalculateSLA(lastActionDateCreated.Value,
                                                                                businessHours,
                                                                                slaTrigger,
                                                                                slaTrigger.TimeLastAction,
                                                                                pausedTimeSpan,
                                                                                daysToPause,
                                                                                holidays);

                                    if (newSlaViolationLastAction != null)
                                    {
                                        newSlaWarningLastAction = SlaTickets.CalculateSLAWarning((DateTime)newSlaViolationLastAction.Value,
                                                                                        businessHours,
                                                                                        slaTrigger.NoBusinessHours,
                                                                                        slaTrigger.TimeLastAction,
                                                                                        slaTrigger.WarningTime,
                                                                                        daysToPause,
                                                                                        holidays);
                                    }
                                }

                                if (slaTrigger.TimeInitialResponse < 1 || totalActions > 0)
                                {
                                    newSlaViolationInitialResponse = null;
                                    newSlaWarningInitialResponse = null;
                                }
                                else
                                {
                                    newSlaViolationInitialResponse = SlaTickets.CalculateSLA(ticket.DateCreatedUtc,
                                                                                    businessHours,
                                                                                    slaTrigger,
                                                                                    slaTrigger.TimeInitialResponse,
                                                                                    pausedTimeSpan,
                                                                                    daysToPause,
                                                                                    holidays);

                                    if (newSlaViolationInitialResponse != null)
                                    {
                                        newSlaWarningInitialResponse = SlaTickets.CalculateSLAWarning((DateTime)newSlaViolationInitialResponse.Value,
                                                                                            businessHours,
                                                                                            slaTrigger.NoBusinessHours,
                                                                                            slaTrigger.TimeInitialResponse,
                                                                                            slaTrigger.WarningTime,
                                                                                            daysToPause,
                                                                                            holidays);
                                    }
                                }
                            }
                            else
                            {
                                Logs.WriteEvent("Ticket is Closed, clearing its SLA values.");

                                //TODO //vv Do we want to delete the paused times and slaticket record if the ticket is closed? how is it done today?
                                /*
                                  if (isClosed)
                                  {
                                    slaTicket.Delete();
                                    SlaPausedTimes slaPausedTimes = new SlaPausedTimes(LoginUser);
                                    slaPausedTimes.LoadByTicketId(ticket.TicketID);
                                    slaPausedTimes.DeleteAll();
                                    //vv slaPausedTimes.Save(); //Check if we need this!
                                  }
                                  else
                                  {
                                    slaTicket.IsPending = false;
                                  }
                                 */
                            }

                            if (HasAnySlaChanges(ticket,
                                                newSlaViolationTimeClosed,
                                                newSlaWarningTimeClosed,
                                                newSlaViolationLastAction,
                                                newSlaWarningLastAction,
                                                newSlaViolationInitialResponse,
                                                newSlaWarningInitialResponse))
                            {
                                ticket.SlaViolationTimeClosed = newSlaViolationTimeClosed;
                                ticket.SlaWarningTimeClosed = newSlaWarningTimeClosed;
                                ticket.SlaViolationLastAction = newSlaViolationLastAction;
                                ticket.SlaWarningLastAction = newSlaWarningLastAction;
                                ticket.SlaViolationInitialResponse = newSlaViolationInitialResponse;
                                ticket.SlaWarningInitialResponse = newSlaWarningInitialResponse;
                                Tickets.UpdateTicketSla(LoginUser,
                                                        ticket.TicketID,
                                                        newSlaViolationInitialResponse,
                                                        newSlaViolationLastAction,
                                                        newSlaViolationTimeClosed,
                                                        newSlaWarningInitialResponse,
                                                        newSlaWarningLastAction,
                                                        newSlaWarningTimeClosed);
                                Logs.WriteEvent("Ticket SLA calculation completed.");

                                string signalRUrl = Settings.ReadString("SignalRUrl");

                                if (!string.IsNullOrEmpty(signalRUrl))
                                {
                                    //Dictionary<string, string> queryStringData = new Dictionary<string, string>();
                                    //queryStringData.Add("userID", "-1");
                                    //queryStringData.Add("organizationID", ticket.OrganizationID.ToString());
                                    //HubConnection connection = new HubConnection(signalRUrl, queryStringData);
                                    //IHubProxy signalRConnection = connection.CreateHubProxy("TicketSocket");

                                    //try
                                    //{
                                    //    connection.Start().Wait();
                                    //    signalRConnection.Invoke("RefreshSLA", ticket.TicketNumber);
                                    //}
                                    //catch (Exception ex)
                                    //{
                                    //    Logs.WriteEvent("Could not send signalR to refresh the SLA. Message: " + ex.Message);
                                    //}
                                }
                            }
                            else
                            {
                                Logs.WriteEvent("Ticket SLA calculation completed. SLA values did not change, ticket was not updated.");
                            }

                            slaTicket.IsPending = false;
                            slaTicket.Collection.Save();
                        }
                        else
                        {
                            SlaPausedTimes slaPausedTimes = new SlaPausedTimes(LoginUser);
                            slaPausedTimes.LoadByTicketId(slaTicket.TicketId);
                            slaPausedTimes.DeleteAll();
                            slaPausedTimes.Save();

                            slaTicket.Delete();
                            slaTicket.Collection.Save();
                            Logs.WriteEventFormat("Ticket id {0} does not exist anymore, deleted from SlaTickets.", slaTicket.TicketId);
                        }

                        System.Threading.Thread.Sleep(100);
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogs.LogException(LoginUser, ex, "SLA Calculator", "Sync");
                        Logs.WriteEventFormat("Exception. Message {0}{1}StackTrace {2}", ex.Message, Environment.NewLine, ex.StackTrace);

                        slaTicket.IsPending = false;
                        slaTicket.Collection.Save();
                        Logs.WriteEventFormat("SlaTicket: TicketId {0} TriggerId {1} set to not pending.", slaTicket.TicketId, slaTicket.SlaTriggerId);
                    }
                }

                if (slaTickets != null && slaTickets.Count > 0)
                {
                    Logs.WriteEvent(string.Format("Completed processing the {0} pending tickets to calculate the SLA values for.", slaTickets.Count));
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser, ex, "SLA Calculator", "Sync");
                Logs.WriteEvent(string.Format("Exception. Message {0}{1}StackTrace {2}", ex.Message, Environment.NewLine, ex.StackTrace));
            }
        }

        private static bool HasAnySlaChanges(Ticket ticket,
                                            DateTime? newSlaViolationTimeClosed,
                                            DateTime? newSlaWarningTimeClosed,
                                            DateTime? newSlaViolationLastAction,
                                            DateTime? newSlaWarningLastAction,
                                            DateTime? newSlaViolationInitialResponse,
                                            DateTime? newSlaWarningInitialResponse)
        {
            bool hasChanges = false;

            if (!hasChanges && DateTime.Compare(ticket.SlaViolationTimeClosed == null ? new DateTime() : ticket.SlaViolationTimeClosed.Value, 
                                                newSlaViolationTimeClosed == null ? new DateTime() : newSlaViolationTimeClosed.Value) != 0)
            {
                hasChanges = true;
            }

            if (!hasChanges && DateTime.Compare(ticket.SlaWarningTimeClosed == null ? new DateTime() : ticket.SlaWarningTimeClosed.Value,
                                                newSlaWarningTimeClosed == null ? new DateTime() : newSlaWarningTimeClosed.Value) != 0)
            {
                hasChanges = true;
            }

            if (!hasChanges && DateTime.Compare(ticket.SlaViolationLastAction == null ? new DateTime() : ticket.SlaViolationLastAction.Value,
                                                newSlaViolationLastAction == null ? new DateTime() : newSlaViolationLastAction.Value) != 0)
            {
                hasChanges = true;
            }

            if (!hasChanges && DateTime.Compare(ticket.SlaWarningLastAction == null ? new DateTime() : ticket.SlaWarningLastAction.Value,
                                                newSlaWarningLastAction == null ? new DateTime() : newSlaWarningLastAction.Value) != 0)
            {
                hasChanges = true;
            }

            if (!hasChanges && DateTime.Compare(ticket.SlaViolationInitialResponse == null ? new DateTime() : ticket.SlaViolationInitialResponse.Value,
                                                newSlaViolationInitialResponse == null ? new DateTime() : newSlaViolationInitialResponse.Value) != 0)
            {
                hasChanges = true;
            }

            if (!hasChanges && DateTime.Compare(ticket.SlaWarningInitialResponse == null ? new DateTime() : ticket.SlaWarningInitialResponse.Value,
                                                newSlaWarningInitialResponse == null ? new DateTime() : newSlaWarningInitialResponse.Value) != 0)
            {
                hasChanges = true;
            }

            return hasChanges;
        }

        private static void UpdateBusinessPausedTimes(LoginUser loginUser, Dictionary<int, double> businessPausedTimes)
        {
            foreach(KeyValuePair<int, double> pair in businessPausedTimes)
            {
                SlaPausedTimes.UpdateBusinessPausedTime(loginUser, pair.Key, pair.Value);
            }
        }
    }

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
          if (DateTime.Now.Subtract(_lastDLSAdjustment).TotalMinutes > 60 && DateTime.Now.Minute > 2 && DateTime.Now.Minute < 30)
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

            bool isPaused = false;
            SlaTicket slaTicket = SlaTickets.GetSlaTicket(LoginUser, ticket.TicketID);
            
            if (slaTicket != null)
            {
                isPaused = ticket.IsSlaPaused(slaTicket.SlaTriggerId, ticket.OrganizationID);
            }

            if (!isPaused)
            {
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
        case SlaViolationType.InitialResponse: violationType = "Initial Response"; break;
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

      ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, violationType + " SLA " + (isWarning ? "warning" : "violation") + " occurred");

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
        //if (customer.OrganizationID != 13679) continue;
        TimeZoneInfo tz = System.TimeZoneInfo.Local;
        if (!string.IsNullOrWhiteSpace(customer.TimeZoneID))
        {
          tz = System.TimeZoneInfo.FindSystemTimeZoneById(customer.TimeZoneID);
        }

        DateTime today = DateTime.Now;
        DateTime start = TimeZoneInfo.ConvertTimeFromUtc(customer.BusinessDayStartUtc, tz); 
        DateTime end = TimeZoneInfo.ConvertTimeFromUtc(customer.BusinessDayEndUtc, tz);

        customer.BusinessDayStart = TimeZoneInfo.ConvertTimeToUtc(new DateTime(today.Year, today.Month, today.Day, start.Hour, start.Minute, 0), tz);
        customer.BusinessDayEnd = TimeZoneInfo.ConvertTimeToUtc(new DateTime(today.Year, today.Month, today.Day, end.Hour, end.Minute, 0), tz);
      }
      customers.Save();
    }
  }
}
