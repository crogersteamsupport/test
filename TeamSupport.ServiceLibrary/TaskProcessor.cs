﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSupport.Data;
using System.Globalization;
using System.Net.Mail;

namespace TeamSupport.ServiceLibrary
{
    public class TaskProcessor : ServiceThreadPoolProcess
    {
        //private Logs logs;
        private bool _isDebug = false;
        private bool _logEnabled = false;
        private int _currentTaskEmailPostID;

        private static object _staticLock = new object();

        private static TaskEmailPost GetNextTaskEmailPost(string connectionString, int lockID, bool isDebug)
        {
            TaskEmailPost result;
            LoginUser loginUser = new LoginUser(connectionString, -1, -1, null);
            lock (_staticLock)
            {
                if (isDebug)
                {
                    SqlExecutor.ExecuteNonQuery(loginUser, "UPDATE TaskEmailPosts SET HoldTime=30 WHERE HoldTime > 30");
                }

                result = TaskEmailPosts.GetNextWaiting(loginUser, lockID.ToString());
            }
            return result;
        }

        public void SetTimeZone(TaskEmailPost taskEmailPost)
        {
            LoginUser.TimeZoneInfo = null;
            Organization organization = Users.GetTSOrganization(LoginUser, taskEmailPost.CreatorID);
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

        public void ProcessEmail(TaskEmailPost taskEmailPost)
        {
            _currentTaskEmailPostID = taskEmailPost.TaskEmailPostID;
            Logs.WriteEvent("TaskEmailPostType: " + taskEmailPost.TaskEmailPostType.ToString());

            switch ((TaskEmailPostType)taskEmailPost.TaskEmailPostType)
            {
                case TaskEmailPostType.Modified:
                    ProcessTaskModified(taskEmailPost.ReminderID, taskEmailPost.CreatorID);
                    break;
                case TaskEmailPostType.Assigned:
                    ProcessTaskAssigned(taskEmailPost.ReminderID, taskEmailPost.CreatorID);
                    break;
                case TaskEmailPostType.Complete:
                    ProcessTaskComplete(taskEmailPost.ReminderID, taskEmailPost.CreatorID);
                    break;
                default:
                    break;
            }
        }

        public override void Run()
        {
            TaskEmailPosts.UnlockThread(LoginUser, (int)_threadPosition);
            while (!IsStopped)
            {
                TaskEmailPost taskEmailPost = GetNextTaskEmailPost(LoginUser.ConnectionString, (int)_threadPosition, _isDebug);
                if (taskEmailPost == null)
                {
                    System.Threading.Thread.Sleep(10000);
                    continue;
                }

                try
                {
                    try
                    {
                        if (taskEmailPost.CreatorID != -5)
                        {
                            _isDebug = Settings.ReadBool("Debug", false);
                            _logEnabled = Settings.ReadInt("LoggingEnabled", 0) == 1;

                            Logs.WriteLine();
                            Logs.WriteEvent("***********************************************************************************");
                            Logs.WriteEvent("Processing Task Email Post  TaskEmailPostID: " + taskEmailPost.TaskEmailPostID.ToString());
                            Logs.WriteData(taskEmailPost.Row);
                            Logs.WriteEvent("***********************************************************************************");
                            SetTimeZone(taskEmailPost);
                            ProcessEmail(taskEmailPost);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogs.LogException(LoginUser, ex, "Email", taskEmailPost.Row);
                    }
                    finally
                    {
                        Logs.WriteEvent("Deleting from DB");
                        taskEmailPost.Collection.DeleteFromDB(taskEmailPost.TaskEmailPostID);
                    }

                    Logs.WriteEvent("Updating Health");
                    UpdateHealth();
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(LoginUser, ex, "Email", taskEmailPost.Row);
                }
            }
        }

        private void ProcessTaskModified(int reminderID, int modifierID)
        {
            TasksViewItem task = TasksView.GetTasksViewItem(LoginUser, reminderID);
            try
            {

                User modifier = Users.GetUser(LoginUser, modifierID);
                if (task == null)
                {
                    Logs.WriteEvent("Unable to find Task, ReminderID: " + reminderID.ToString());
                    return;
                }

                if (modifier == null)
                {
                    Logs.WriteEvent("Unable to find Modifying User, UserID: " + modifierID.ToString());
                    return;
                }

                if (task.UserID == null)
                {
                    Logs.WriteEvent("Assigned Task User is null");
                    return;
                }

                Organization taskOrganization = Organizations.GetOrganization(LoginUser, task.OrganizationID);

                if (taskOrganization == null)
                {
                    Logs.WriteEvent("Task's Organization IS NULL!!!!");
                    return;
                }

                User owner = Users.GetUser(LoginUser, (int)task.UserID);

                MailMessage message = EmailTemplates.GetTaskModified(LoginUser, UsersView.GetUsersViewItem(LoginUser, modifierID), UsersView.GetUsersViewItem(LoginUser, (int)task.UserID), task);
                message.To.Add(GetMailAddress(owner.Email, owner.FirstLastName));
                //message.Subject = message.Subject + " [pvt]";

                String description = String.Format("Task modified notification sent to {0} for Task {1}", message.To.ToString(), task.TaskName);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tasks, task.ReminderID, description);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, (int)task.UserID, description);

                //string emailReplyToAddress = GetEmailReplyToAddress(LoginUser, ticket);
                //AddMessage(taskOrganization, "Ticket Update Request [" + ticket.TicketNumber.ToString() + "]", message, emailReplyToAddress);
            }
            catch (Exception ex)
            {
                Logs.WriteEvent("Error with ProcessTaskModified");
                Logs.WriteException(ex);
                ExceptionLogs.LogException(LoginUser, ex, "ProcessTaskModified", task.Row);
            }
        }

        private void ProcessTaskAssigned(int reminderID, int modifierID)
        {
            TasksViewItem task = TasksView.GetTasksViewItem(LoginUser, reminderID);
            try
            {

                User modifier = Users.GetUser(LoginUser, modifierID);
                if (task == null)
                {
                    Logs.WriteEvent("Unable to find Task, ReminderID: " + reminderID.ToString());
                    return;
                }

                if (modifier == null)
                {
                    Logs.WriteEvent("Unable to find Modifying User, UserID: " + modifierID.ToString());
                    return;
                }

                if (task.UserID == null)
                {
                    Logs.WriteEvent("Assigned Task User is null");
                    return;
                }

                Organization taskOrganization = Organizations.GetOrganization(LoginUser, task.OrganizationID);

                if (taskOrganization == null)
                {
                    Logs.WriteEvent("Task's Organization IS NULL!!!!");
                    return;
                }

                User owner = Users.GetUser(LoginUser, (int)task.UserID);

                MailMessage message = EmailTemplates.GetTaskAssigned(LoginUser, UsersView.GetUsersViewItem(LoginUser, modifierID), UsersView.GetUsersViewItem(LoginUser, (int)task.UserID), task);
                message.To.Add(GetMailAddress(owner.Email, owner.FirstLastName));
                //message.Subject = message.Subject + " [pvt]";

                String description = String.Format("Task assigned notification sent to {0} for Task {1}", message.To.ToString(), task.TaskName);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tasks, task.ReminderID, description);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, (int)task.UserID, description);

                //string emailReplyToAddress = GetEmailReplyToAddress(LoginUser, ticket);
                //AddMessage(taskOrganization, "Ticket Update Request [" + ticket.TicketNumber.ToString() + "]", message, emailReplyToAddress);
            }
            catch (Exception ex)
            {
                Logs.WriteEvent("Error with ProcessTaskAssigned");
                Logs.WriteException(ex);
                ExceptionLogs.LogException(LoginUser, ex, "ProcessTaskModified", task.Row);
            }
        }

        private void ProcessTaskComplete(int reminderID, int modifierID)
        {
            TasksViewItem task = TasksView.GetTasksViewItem(LoginUser, reminderID);
            try
            {

                User modifier = Users.GetUser(LoginUser, modifierID);
                if (task == null)
                {
                    Logs.WriteEvent("Unable to find Task, ReminderID: " + reminderID.ToString());
                    return;
                }

                if (modifier == null)
                {
                    Logs.WriteEvent("Unable to find Modifying User, UserID: " + modifierID.ToString());
                    return;
                }

                if (task.UserID == null)
                {
                    Logs.WriteEvent("Assigned Task User is null");
                    return;
                }

                Organization taskOrganization = Organizations.GetOrganization(LoginUser, task.OrganizationID);

                if (taskOrganization == null)
                {
                    Logs.WriteEvent("Task's Organization IS NULL!!!!");
                    return;
                }

                User owner = Users.GetUser(LoginUser, (int)task.UserID);

                MailMessage message = EmailTemplates.GetTaskComplete(LoginUser, UsersView.GetUsersViewItem(LoginUser, modifierID), UsersView.GetUsersViewItem(LoginUser, (int)task.UserID), task);
                message.To.Add(GetMailAddress(owner.Email, owner.FirstLastName));
                //message.Subject = message.Subject + " [pvt]";
                //EmailTemplates.ReplaceEmailRecipientParameters(LoginUser, message, ticket, owner.UserID, owner.OnlyEmailAfterHours);

                String description = String.Format("Task complete notification sent to {0} for Task {1}", message.To.ToString(), task.TaskName);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tasks, task.ReminderID, description);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, (int)task.UserID, description);

                //string emailReplyToAddress = GetEmailReplyToAddress(LoginUser, ticket);
                //AddMessage(taskOrganization, "Ticket Update Request [" + ticket.TicketNumber.ToString() + "]", message, emailReplyToAddress);
            }
            catch (Exception ex)
            {
                Logs.WriteEvent("Error with ProcessTaskComplete");
                Logs.WriteException(ex);
                ExceptionLogs.LogException(LoginUser, ex, "ProcessTaskComplete", task.Row);
            }
        }

        private MailAddress GetMailAddress(string address, string displayName)
        {
            return new MailAddress(FixMailAddress(address), FixMailName(displayName));
        }

        private string FixMailAddress(string address)
        {
            return address.Replace("<", "").Replace(">", "").Replace("|", " ");
        }

        private string FixMailName(string displayName)
        {
            return displayName.Replace("<", "").Replace(">", "").Replace("|", " ");
        }

        public override void ReleaseAllLocks()
        {
            EmailPosts.UnlockAll(LoginUser);
        }
    }
}
