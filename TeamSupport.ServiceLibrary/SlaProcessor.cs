﻿using System;
using TeamSupport.Data;
using System.Linq;
using System.Net.Mail;

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
                    Ticket ticket = Tickets.GetTicket(LoginUser, slaTicket.TicketId);

                    if (ticket != null)
                    {
                        bool isPaused = false; //vv ticket.IsSlaPaused;
                        bool isClosed = ticket.DateClosed != null;
                        DateTime? newSlaViolationTimeClosed = null;
                        DateTime? newSlaWarningTimeClosed = null;
                        DateTime? newSlaViolationLastAction = null;
                        DateTime? newSlaWarningLastAction = null;
                        DateTime? newSlaViolationInitialResponse = null;
                        DateTime? newSlaWarningInitialResponse = null;

                        if (!isPaused && !isClosed)
                        {
                            DateTime? lastActionDateCreated = Actions.GetLastActionDateCreated(LoginUser, ticket.TicketID);
                            int totalActions = Actions.TotalActionsForSla(LoginUser, ticket.TicketID);

                            Organization organization = Organizations.GetOrganization(LoginUser, ticket.OrganizationID);
                            SlaTrigger slaTrigger = SlaTriggers.GetSlaTrigger(LoginUser, slaTicket.SlaTriggerId);

                            Logs.WriteEvent(string.Format("Ticket #{0} id {1}. LastAction: {2}, TotalActions: {3}, Org({4}) {5}, SlaTriggerId {6}.", ticket.TicketNumber,
                                                                                                                                                    ticket.TicketID,
                                                                                                                                                    lastActionDateCreated.Value.ToString(),
                                                                                                                                                    totalActions,
                                                                                                                                                    organization.OrganizationID,
                                                                                                                                                    organization.Name,
                                                                                                                                                    slaTrigger.SlaTriggerID));
                            TimeSpan pausedTimeSpan = new TimeSpan(); //vv CalculatePausedTime(ticket.TicketID, organization.BusinessDayStartUtc, organization.BusinessDayEndUtc, organization.BusinessDays, slaTrigger.UseBusinessHours, Logs, LoginUser);
                            //vv Logs.WriteEvent("Total Paused Time: " + pausedTimeSpan.ToString());

                            newSlaViolationTimeClosed = CalculateSLA(ticket.DateCreatedUtc, organization.BusinessDayStartUtc, organization.BusinessDayEndUtc, organization.BusinessDays, slaTrigger.UseBusinessHours, slaTrigger.TimeToClose, 0, pausedTimeSpan, Logs);
                            newSlaWarningTimeClosed = CalculateSLA(ticket.DateCreatedUtc, organization.BusinessDayStartUtc, organization.BusinessDayEndUtc, organization.BusinessDays, slaTrigger.UseBusinessHours, slaTrigger.TimeToClose, slaTrigger.WarningTime, pausedTimeSpan, Logs);

                            newSlaViolationLastAction = CalculateSLA(lastActionDateCreated.Value, organization.BusinessDayStartUtc, organization.BusinessDayEndUtc, organization.BusinessDays, slaTrigger.UseBusinessHours, slaTrigger.TimeLastAction, 0, pausedTimeSpan, Logs);
                            newSlaWarningLastAction = CalculateSLA(lastActionDateCreated.Value, organization.BusinessDayStartUtc, organization.BusinessDayEndUtc, organization.BusinessDays, slaTrigger.UseBusinessHours, slaTrigger.TimeLastAction, slaTrigger.WarningTime, pausedTimeSpan, Logs);

                            if (slaTrigger.TimeInitialResponse < 1 || totalActions > 0)
                            {
                                newSlaViolationInitialResponse = null;
                                newSlaWarningInitialResponse = null;
                            }
                            else
                            {
                                newSlaViolationInitialResponse = CalculateSLA(ticket.DateCreatedUtc, organization.BusinessDayStartUtc, organization.BusinessDayEndUtc, organization.BusinessDays, slaTrigger.UseBusinessHours, slaTrigger.TimeInitialResponse, 0, pausedTimeSpan, Logs);
                                newSlaWarningInitialResponse = CalculateSLA(ticket.DateCreatedUtc, organization.BusinessDayStartUtc, organization.BusinessDayEndUtc, organization.BusinessDays, slaTrigger.UseBusinessHours, slaTrigger.TimeInitialResponse, slaTrigger.WarningTime, pausedTimeSpan, Logs);
                            }
                        }
                        else
                        {
                            Logs.WriteEventFormat("Ticket is {0}, clearing its SLA values.", isClosed ? "closed" : "paused");

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

                        if (HasAnySlaChanges(ticket, newSlaViolationTimeClosed, newSlaWarningTimeClosed, newSlaViolationLastAction, newSlaWarningLastAction, newSlaViolationInitialResponse, newSlaWarningInitialResponse))
                        {
                            ticket.SlaViolationTimeClosed = newSlaViolationTimeClosed;
                            ticket.SlaWarningTimeClosed = newSlaWarningTimeClosed;
                            ticket.SlaViolationLastAction = newSlaViolationLastAction;
                            ticket.SlaWarningLastAction = newSlaWarningLastAction;
                            ticket.SlaViolationInitialResponse = newSlaViolationInitialResponse;
                            ticket.SlaWarningInitialResponse = newSlaWarningInitialResponse;
                            ticket.Collection.Save(); 
                            Logs.WriteEvent("Ticket SLA calculation completed.");
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
                        //SlaPausedTimes slaPausedTimes = new SlaPausedTimes(LoginUser);
                        //slaPausedTimes.LoadByTicketId(ticket.TicketID);
                        //slaPausedTimes.DeleteAll();
                        //vv slaPausedTimes.Save(); //Check if we need this!

                        slaTicket.Delete();
                        slaTicket.Collection.Save();
                        Logs.WriteEventFormat("Ticket #{0} (id: {1}) for org {2} does not exist anymore, deleted from SlaTickets.", ticket.TicketNumber, ticket.TicketID, ticket.OrganizationID);
                    }

                    System.Threading.Thread.Sleep(100);
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

        public static DateTime? CalculateSLA(DateTime DateCreated,//startdate
                                            DateTime? organizationBusinessDayStart,
                                            DateTime? organizationBusinessDayEnd,
                                            int organizationBusinessDays,
                                            bool slaUseBusinessHours,
                                            int minutes,
                                            int slaWarningTime,
                                            TimeSpan pausedTimeSpan,
                                            Logs logs)
        {
            //vv DateTime organizationBusinessDayStartHour = default(DateTime).Add(organizationBusinessDayStart.TimeOfDay);
            //vv DateTime organizationBusinessDayEndHour = default(DateTime).Add(organizationBusinessDayEnd.TimeOfDay);
            DateTime? ExpireDate = new DateTime();
            minutes = minutes - slaWarningTime;

            if (organizationBusinessDayStart == null || organizationBusinessDayEnd == null || organizationBusinessDays < 1 || minutes < 1)
            {
                ExpireDate = null;
            }
            else
            {
                if (organizationBusinessDays == 0 || !slaUseBusinessHours)
                {
                    ExpireDate = DateCreated.AddMinutes(minutes);
                }
                else
                {
                    //full calculation
                    int startOfDayMinutes = organizationBusinessDayStart.Value.Minute + (organizationBusinessDayStart.Value.Hour * 60); //@Start
                    int endOfDayMinutes = organizationBusinessDayEnd.Value.Minute + (organizationBusinessDayEnd.Value.Hour * 60); //@End
                    int cur = DateCreated.Minute + (DateCreated.Hour * 60);
                    int minutesInOneDay = (24 * 60);

                    //Make sure endOfDayMinutes is greater than startOfDayMinutes
                    if (startOfDayMinutes >= endOfDayMinutes)
                    {
                        //Add 1 day worth of minutes
                        endOfDayMinutes = endOfDayMinutes + minutesInOneDay;
                    }

                    int minutesInBusinessDay = endOfDayMinutes - startOfDayMinutes;

                    //Make sure the start time falls within business hours
                    if (cur > endOfDayMinutes)
                    {
                        //Reset the time to start of bussiness day AND add 1 day
                        DateCreated = DateCreated.Date.AddDays(1).Add(organizationBusinessDayStart.Value.TimeOfDay);
                    }
                    else if (cur < startOfDayMinutes)
                    {
                        DateCreated = DateCreated.Date.Add(organizationBusinessDayStart.Value.TimeOfDay);
                    }

                    //Repeat until we find the first business day
                    while ((organizationBusinessDays & (int)Math.Pow(2, (double)DateCreated.DayOfWeek)) != Math.Pow(2, (double)DateCreated.DayOfWeek))
                    {
                        DateCreated = DateCreated.AddDays(1);
                    }

                    //DateCreated now contains a valid date to start SLA with business days
                    cur = DateCreated.Minute + (DateCreated.Hour * 60);

                    if ((cur + minutes) <= endOfDayMinutes)
                    {
                        //Expiration falls within same business day, no need to find next business day
                        ExpireDate = DateCreated.AddMinutes(minutes);
                    }
                    else
                    {
                        //Offset the minutes used in the current day
                        minutes = minutes - (endOfDayMinutes - cur);
                        ExpireDate = DateCreated.Date.AddDays(1).Add(organizationBusinessDayStart.Value.TimeOfDay);

                        //Loop to find the business when all the minutes are gone
                        while (minutes > 0)
                        {
                            //Add a day if this is a business day and decrease minutes
                            if ((organizationBusinessDays & (int)Math.Pow(2, (double)ExpireDate.Value.DayOfWeek)) == Math.Pow(2, (double)ExpireDate.Value.DayOfWeek))
                            {
                                if (minutes <= minutesInBusinessDay)
                                {
                                    //Is this the end?
                                    ExpireDate = ExpireDate.Value.AddMinutes(minutes);
                                    minutes = 0;
                                }
                                else
                                {
                                    //This is a business day, substract it from total minutes and include the day
                                    minutes = minutes - minutesInOneDay;
                                    ExpireDate = ExpireDate.Value.AddDays(1);
                                }
                            }
                            else
                            {
                                //This is not a business day, just move on to the next day
                                ExpireDate = ExpireDate.Value.AddDays(1);
                            }
                        }

                        //Process the leftover minutes
                        minutes = minutes * (-1);

                        if (minutes > 0)
                        {
                            int days = 0;
                            days = (minutes / minutesInBusinessDay) + 1;

                            while (days > 0)
                            {
                                ExpireDate = ExpireDate.Value.AddDays(-1);

                                if ((organizationBusinessDays & (int)Math.Pow(2, (double)ExpireDate.Value.DayOfWeek)) == Math.Pow(2, (double)ExpireDate.Value.DayOfWeek))
                                {
                                    days--;
                                }
                            }

                            ExpireDate = ExpireDate.Value.AddDays(days * (-1));
                            minutes = minutes % minutesInBusinessDay;
                            minutes = minutesInBusinessDay - minutes;
                            ExpireDate = ExpireDate.Value.AddMinutes(minutes);
                        }
                    }
                }

                //vv ExpireDate = AddPausedTime((DateTime)ExpireDate, pausedTimeSpan, organizationBusinessDays, organizationBusinessDayStart, organizationBusinessDayEnd, slaUseBusinessHours);
            }

            return ExpireDate;
        }

        public static TimeSpan CalculatePausedTime(int ticketId,
                                            DateTime? organizationBusinessDayStart,
                                            DateTime? organizationBusinessDayEnd,
                                            int organizationBusinessDays,
                                            bool slaUseBusinessHours,
                                            Logs logs,
                                            LoginUser loginUser)
        {
            TimeSpan totalPausedTime = new TimeSpan();
            //DateTime pausedOn = new DateTime();
            //DateTime resumedOn = new DateTime();

            //SlaPausedTimes slaPausedTimes = new SlaPausedTimes(loginUser);
            //slaPausedTimes.LoadByTicketId(ticketId);

            //foreach(SlaPausedTime slaPausedTime in slaPausedTimes)
            //{
            //    pausedOn = slaPausedTime.PausedOnUtc; //vv new DateTime(2016, 8, 09, 17, 11, 10);
            //    resumedOn = (DateTime)slaPausedTime.ResumedOnUtc; //vv new DateTime(2016, 8, 10, 13, 22, 05);

            //    if ((!IsBusinessDay(pausedOn, organizationBusinessDays) && !IsBusinessDay(resumedOn, organizationBusinessDays))
            //        && pausedOn.Date == resumedOn.Date && slaUseBusinessHours)
            //    {
            //        logs.WriteEvent("Paused and Resumed on the same non-business day, so no time to add.");
            //    }
            //    else
            //    {
            //        if (slaUseBusinessHours)
            //        {
            //            int startOfDayMinutes = organizationBusinessDayStart.Value.Minute + (organizationBusinessDayStart.Value.Hour * 60); //@Start
            //            int endOfDayMinutes = organizationBusinessDayEnd.Value.Minute + (organizationBusinessDayEnd.Value.Hour * 60); //@End
            //            int minutesInOneDay = (24 * 60);

            //            //Make sure endOfDayMinutes is greater than startOfDayMinutes
            //            if (startOfDayMinutes >= endOfDayMinutes)
            //            {
            //                //Add 1 day worth of minutes
            //                endOfDayMinutes = endOfDayMinutes + minutesInOneDay;
            //            }

            //            int minutesInBusinessDay = endOfDayMinutes - startOfDayMinutes;

            //            //Make sure the pausedon and resumedon are business days
            //            if (!IsBusinessDay(pausedOn, organizationBusinessDays))
            //            {
            //                pausedOn = GetNextBusinessDay(pausedOn, organizationBusinessDays);
            //            }

            //            if (!IsBusinessDay(resumedOn, organizationBusinessDays))
            //            {
            //                resumedOn = GetNextBusinessDay(resumedOn, organizationBusinessDays);
            //            }

            //            int minutesPaused = 0;

            //            while (pausedOn.Date < resumedOn.Date)
            //            {
            //                //Check if it the minutes where it was paused/resumed is inside business days, if not then set to startminutes
            //                int pausedOnMinute = pausedOn.Minute + (pausedOn.Hour * 60);

            //                if (pausedOnMinute < startOfDayMinutes || pausedOnMinute > endOfDayMinutes)
            //                {
            //                    pausedOnMinute = startOfDayMinutes;
            //                }

            //                minutesPaused = endOfDayMinutes - pausedOnMinute;
            //                totalPausedTime = totalPausedTime.Add(TimeSpan.FromMinutes(minutesPaused));
            //                pausedOn = GetNextBusinessDay(pausedOn, organizationBusinessDays);
            //                pausedOn = new DateTime(pausedOn.Year, pausedOn.Month, pausedOn.Day, organizationBusinessDayStart.Value.Hour, organizationBusinessDayStart.Value.Minute, 0);
            //            }

            //            //same day
            //            if (pausedOn.Date == resumedOn.Date && pausedOn.TimeOfDay < resumedOn.TimeOfDay)
            //            {
            //                int resumedOnMinute = resumedOn.Minute + (resumedOn.Hour * 60);

            //                if (resumedOnMinute < startOfDayMinutes)
            //                {
            //                    resumedOnMinute = startOfDayMinutes;
            //                }

            //                if (resumedOnMinute > endOfDayMinutes)
            //                {
            //                    resumedOnMinute = endOfDayMinutes;
            //                }

            //                minutesPaused = resumedOnMinute - ((pausedOn.Hour * 60) + pausedOn.Minute);
            //                totalPausedTime = totalPausedTime.Add(TimeSpan.FromMinutes(minutesPaused));
            //            }
            //        }
            //        else
            //        {
            //            totalPausedTime = resumedOn - pausedOn;
            //        }

            //        logs.WriteEvent(string.Format("Ticket was paused by {0} hours", totalPausedTime.ToString()));
            //    }
            //}

            return totalPausedTime;
        }

        public static bool IsBusinessDay(DateTime inputDate, int organizationBusinessDays)
        {
            bool isBusinessDay = false;

            isBusinessDay = (organizationBusinessDays & (int)Math.Pow(2, (double)inputDate.DayOfWeek)) == Math.Pow(2, (double)inputDate.DayOfWeek);

            return isBusinessDay;
        }

        public static DateTime GetNextBusinessDay(DateTime inputDate, int organizationBusinessDays)
        {
            DateTime businessDay = inputDate;

            businessDay = businessDay.AddDays(1);

            while ((organizationBusinessDays & (int)Math.Pow(2, (double)businessDay.DayOfWeek)) != Math.Pow(2, (double)businessDay.DayOfWeek))
            {
                businessDay = businessDay.AddDays(1);
            }

            return businessDay;
        }

        public static DateTime AddPausedTime(DateTime inputDate,
                                            TimeSpan pausedTimeSpan,
                                            int organizationBusinessDays,
                                            DateTime? organizationBusinessDayStart,
                                            DateTime? organizationBusinessDayEnd,
                                            bool slaUseBusinessHours)
        {
            DateTime slaWithPausedTime = new DateTime();
            int startOfDayMinutes = organizationBusinessDayStart.Value.Minute + (organizationBusinessDayStart.Value.Hour * 60);
            int endOfDayMinutes = organizationBusinessDayEnd.Value.Minute + (organizationBusinessDayEnd.Value.Hour * 60);
            int minutesInOneDay = (24 * 60);

            //Make sure endOfDayMinutes is greater than startOfDayMinutes
            if (startOfDayMinutes >= endOfDayMinutes)
            {
                //Add 1 day worth of minutes
                endOfDayMinutes = endOfDayMinutes + minutesInOneDay;
            }

            int minutesInBusinessDay = endOfDayMinutes - startOfDayMinutes;

            if (slaUseBusinessHours)
            {
                slaWithPausedTime = inputDate;
                //get full days first
                int fullDays = (int)(pausedTimeSpan.TotalMinutes / minutesInBusinessDay);
                pausedTimeSpan = pausedTimeSpan.Subtract(TimeSpan.FromMinutes(fullDays * minutesInBusinessDay));

                while (pausedTimeSpan.TotalMinutes > 0)
                {
                    if ((slaWithPausedTime.Minute + (slaWithPausedTime.Hour * 60)) + pausedTimeSpan.TotalMinutes <= endOfDayMinutes)
                    {
                        slaWithPausedTime = slaWithPausedTime.AddMinutes(pausedTimeSpan.TotalMinutes);
                        pausedTimeSpan = pausedTimeSpan.Subtract(TimeSpan.FromMinutes(pausedTimeSpan.TotalMinutes));
                    }
                    else
                    {
                        int addMinutes = endOfDayMinutes - (slaWithPausedTime.Minute + (slaWithPausedTime.Hour * 60));

                        if (slaWithPausedTime.TimeOfDay == organizationBusinessDayStart.Value.TimeOfDay)
                        {
                            addMinutes = (int)pausedTimeSpan.TotalMinutes;
                        }

                        slaWithPausedTime = slaWithPausedTime.AddMinutes(addMinutes);
                        pausedTimeSpan = pausedTimeSpan.Subtract(TimeSpan.FromMinutes(addMinutes));

                        if (pausedTimeSpan.TotalMinutes > 0)
                        {
                            slaWithPausedTime = GetNextBusinessDay(slaWithPausedTime, organizationBusinessDays);
                            slaWithPausedTime = slaWithPausedTime.Date + organizationBusinessDayStart.Value.TimeOfDay;
                        }
                    }
                }

                for (int i = 0; i < fullDays; i++)
                {
                    do
                    {
                        slaWithPausedTime = slaWithPausedTime.AddDays(1);
                    } while (!IsBusinessDay(slaWithPausedTime, organizationBusinessDays));
                }
            }
            else
            {
                slaWithPausedTime = inputDate.Add(pausedTimeSpan);
            }

            return slaWithPausedTime;
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

            //vv If it is paused, skip
            //TicketStatuses statuses = new TicketStatuses(LoginUser);
            //statuses.LoadByOrganizationID(ticket.OrganizationID);

            //vv by checking if the ticket is in the paused table and not resumed yet
            /*
                SlaPausedTimes slaPausedTimes = new SlaPausedTimes(LoginUser);
                slaPausedTimes.LoadByTicketId(ticket.TicketID);

                if (slaPausedTimes == null || (slaPausedTimes.Count > 0 && !slaPausedTimes.Where(p => p.ResumedOn == null).Any()))
            */

            //vv by checking the status (if it is a sla paused status)
            //vv if (statuses != null && !statuses.Where(p => p.TicketStatusID == ticket.TicketStatusID && p.PauseSLA).Any())
            if (true) //vv !ticket.IsSlaPaused) //vv
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
