using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.IO;
using TeamSupport.Proxy;
using System.Web.Security;
using System.Diagnostics;
using TeamSupport.Data;
using System.Web;
using TeamSupport.Model;

namespace TeamSupport.DataAPI
{
    /// <summary>
    /// CRUD Interface (Create, Read, Update, Delete) on verified connection context
    /// </summary>
    public static class DataAPI
    {
        // Tickets --------------------------
        public static void Create(ConnectionContext connection, TicketProxy ticketProxy)
        {
            // TODO - create ticket
            LogMessage(connection.Authentication, ActionLogType.Insert, ReferenceType.Tickets, ticketProxy.TicketID, "Created Ticket");
        }

        public static TicketProxy Read(ConnectionContext connection, TicketModel ticketModel)
        {
            Table<TicketProxy> table = connection._db.GetTable<TicketProxy>();
            return table.Where(t => t.TicketID == ticketModel.TicketID).First();
        }

        public static void Update(ConnectionContext connection, TicketModel ticketModel, TicketProxy ticketProxy)
        {
            // TODO - update ticket
            LogMessage(connection.Authentication, ActionLogType.Update, ReferenceType.Tickets, ticketModel.TicketID, "Updated Ticket");
        }

        public static void Delete(ConnectionContext connection, TicketModel ticketModel)
        {
            // TODO - delete ticket
            LogMessage(connection.Authentication, ActionLogType.Delete, ReferenceType.Tickets, ticketModel.TicketID, "Deleted Ticket");
        }


        // Actions --------------------------
        public static void Create(ConnectionContext connection, TicketModel ticketModel, ActionProxy actionProxy)
        {
            // TODO - create action
            LogMessage(connection.Authentication, ActionLogType.Insert, ReferenceType.Actions, actionProxy.ActionID, "Created Action");
        }

        public static ActionProxy Read(ConnectionContext connection, ActionModel actionModel)
        {
            Table<ActionProxy> table = connection._db.GetTable<ActionProxy>();
            return table.Where(t => t.ActionID == actionModel.ActionID).First();
        }

        public static void Read(ConnectionContext connection, TicketModel ticketModel, out ActionProxy[] actionProxies)
        {
            Table<ActionProxy> table = connection._db.GetTable<ActionProxy>();
            actionProxies = table.Where(t => t.TicketID == ticketModel.TicketID).ToArray();
        }

        public static void Update(ConnectionContext connection, ActionModel actionModel, ActionProxy actionProxy)
        {
            // TODO - update action
            LogMessage(connection.Authentication, ActionLogType.Update, ReferenceType.Actions, actionModel.ActionID, "Updated Action");
        }

        public static void Delete(ConnectionContext connection, ActionModel actionModel)
        {
            // TODO - delete action
            LogMessage(connection.Authentication, ActionLogType.Delete, ReferenceType.Actions, actionModel.ActionID, "Deleted Action");
        }


        // Action Attachments --------------------------
        public static void Create(ConnectionContext connection, ActionModel action, AttachmentProxy attachmentProxy)
        {
            // TODO - create action attachment
            LogMessage(connection.Authentication, ActionLogType.Insert, ReferenceType.Attachments, attachmentProxy.AttachmentID, "Created Ticket");
        }

        public static AttachmentProxy Read(ConnectionContext connection, ActionAttachment actionAttachment)
        {
            Table<AttachmentProxy> table = connection._db.GetTable<AttachmentProxy>();
            return table.Where(t => t.AttachmentID == actionAttachment.ActionAttachmentID).First();
        }

        public static void Read(ConnectionContext connection, ActionModel actionModel, out AttachmentProxy[] attachments)
        {
            Table<AttachmentProxy> table = connection._db.GetTable<AttachmentProxy>();
            attachments = table.Where(t => (t.RefID == actionModel.ActionID) && (t.RefType == ReferenceType.Actions)).ToArray();
        }

        public static void Update(ConnectionContext connection, ActionAttachment actionAttachmentModel, AttachmentProxy attachment)
        {
            // TODO - update action attachment
            LogMessage(connection.Authentication, ActionLogType.Update, ReferenceType.Attachments, actionAttachmentModel.ActionAttachmentID, "Updated Action Attachment");
        }

        public static void Delete(ConnectionContext connection, ActionAttachment actionAttachmentModel)
        {
            // TODO - delete action attachment
            LogMessage(connection.Authentication, ActionLogType.Delete, ReferenceType.Actions, actionAttachmentModel.ActionAttachmentID, "Deleted Action Attachment");
        }


        // Log --------------------------
        public static void LogMessage(AuthenticationModel authentication, ActionLogType logType, ReferenceType refType, int? refID, string message)
        {
            LoginUser user = new LoginUser(authentication.UserID, authentication.OrganizationID);
            ActionLogs.AddActionLog(user, logType, refType, refID.HasValue ? refID.Value : 0, message);  // 0 if no ID?
        }

        public static void LogMessage(AuthenticationModel authentication, ActionLogType logType, ReferenceType refType, int? refID, string message, Exception ex)
        {
            string fullMessage = message + ex.ToString() + " ----- STACK: " + ex.StackTrace.ToString();
            if (Debugger.IsAttached)
            {
                Debug.WriteLine(fullMessage);   // debug output window (very fast)
                Debugger.Break();   // something is wrong - fix the code!
            }

            LogMessage(authentication, logType, refType, refID, fullMessage);
        }

        //public static void LogMessage(FormsAuthenticationTicket authentication, ActionLogType logType, ReferenceType refType, int? refID, string message, EventLogEntryType type = EventLogEntryType.Information)
        //{
        //    LoginUser user = WebUtils.TSAuthentication.GetLoginUser();
        //    ActionLogs.AddActionLog(user, logType, refType, refID.HasValue ? refID.Value : 0, message);  // 0 if no TicketID?
        //}

        //public static void LogMessage(AuthenticationModel authentication, ActionLogType logType, ReferenceType refType, int? refID, string message, EventLogEntryType type = EventLogEntryType.Information)
        //{
        //    LogMessage(authentication.AuthenticationTicket, logType, refType, refID, message, type);
        //}

        //public static void LogMessage(FormsAuthenticationTicket authentication, ActionLogType logType, ReferenceType refType, int? refID, string message, Exception ex)
        //{
        //    if (Debugger.IsAttached)
        //    {
        //        Debug.WriteLine(message);   // debug output window (very fast)
        //        Debugger.Break();   // something is wrong - fix the code!
        //    }
        //    LogMessage(authentication, logType, refType, refID, message + ex.ToString() + " ----- STACK: " + ex.StackTrace.ToString(), EventLogEntryType.Error);
        //}


        // verified connection and action
        public static void Create(ConnectionContext connection, ActionModel action, List<ActionAttachment> attachments)
        {
            if (attachments.Count == 0)
                return;

            LogMessage(connection.Authentication, ActionLogType.Insert, ReferenceType.Attachments, attachments[0].Action.ActionID, "Attachments Saved");
        }

        #region Actions
        /// <summary> extracted from ts-app\WebApp\App_Code\TicketPageService.cs UpdateAction(ActionProxy proxy) </summary>
        public static ActionProxy InsertAction(AuthenticationModel loginUser, ActionProxy proxy, DataContext db)
        {
            return null;
            //Action action = (new Actions(WebUtils.TSAuthentication.GetLoginUser())).AddNewAction();
            //action.TicketID = proxy.TicketID;
            //action.CreatorID = loginUser.UserID;
            //action.Description = proxy.Description;

            //// add signature?
            //string signature = db.ExecuteQuery<string>($"SELECT [Signature] FROM Users WITH (NOLOCK) WHERE UserID={loginUser.UserID} AND OrganizationID={loginUser.OrganizationID}").FirstOrDefault();    // 175915
            //if (!string.IsNullOrWhiteSpace(signature) && proxy.IsVisibleOnPortal && !proxy.IsKnowledgeBase && proxy.ActionID == -1 &&
            //    (!proxy.Description.Contains(signature)))
            //{
            //    action.Description += "<br/><br/>" + signature;
            //}

            //action.ActionTypeID = proxy.ActionTypeID;
            //action.DateStarted = proxy.DateStarted;
            //action.TimeSpent = proxy.TimeSpent;
            //action.IsKnowledgeBase = proxy.IsKnowledgeBase;
            //action.IsVisibleOnPortal = proxy.IsVisibleOnPortal;
            //action.Collection.Save();
            //return action.GetProxy();
        }

        /// <summary> extracted from ts-app\webapp\app_code\ticketservice.cs </summary>
        //public static ActionProxy InsertAction(Ticket ticket, ActionProxy proxy, User user)
        //{
        //    Action action = (new Actions(ticket.Collection.LoginUser)).AddNewAction();
        //    action.ActionTypeID = null;
        //    action.Name = "Description";
        //    action.SystemActionTypeID = SystemActionType.Description;
        //    action.ActionSource = ticket.TicketSource;
        //    action.Description = proxy.Description;
        //    if (!string.IsNullOrWhiteSpace(user.Signature) && proxy.IsVisibleOnPortal)
        //        action.Description = action.Description + "<br/><br/>" + user.Signature;

        //    action.IsVisibleOnPortal = ticket.IsVisibleOnPortal;
        //    action.IsKnowledgeBase = ticket.IsKnowledgeBase;
        //    action.TicketID = ticket.TicketID;
        //    action.TimeSpent = proxy.TimeSpent;
        //    action.DateStarted = proxy.DateStarted;
        //    action.Collection.Save();
        //    return action.GetProxy();
        //}

        public static int ActionGetTicketID(DataContext db, int actionID) { return db.ExecuteQuery<int>($"SELECT TicketID FROM Actions WITH (NOLOCK) WHERE ActionID = {actionID}").Min(); }

        public static int ActionCreatorID(DataContext db, int actionID) { return db.ExecuteQuery<int>($"SELECT CreatorID FROM Actions WITH (NOLOCK) WHERE ActionID={actionID}").Min(); }
        #endregion

        #region ActionAttachments

        public static void InsertActionAttachment(DataContext db, int ticketID, ref AttachmentProxy proxy)
        {
            int organizationID = proxy.OrganizationID;
            int actionID = proxy.RefID;

            // hard code all the numbers, parameterize all the strings so they are SQL-Injection checked
            string query = "INSERT INTO ActionAttachments(OrganizationID, FileName, FileType, FileSize, Path, DateCreated, DateModified, CreatorID, ModifierID, ActionID, SentToJira, SentToTFS, SentToSnow, FilePathID) " +
                $"VALUES({organizationID}, {{0}}, {{1}}, {proxy.FileSize}, {{2}}, '{proxy.DateCreated}', '{proxy.DateModified}', {proxy.CreatorID}, {proxy.ModifierID}, {actionID}, {(proxy.SentToJira ? 1 : 0)}, {(proxy.SentToTFS ? 1 : 0)}, {(proxy.SentToSnow ? 1 : 0)}, {proxy.FilePathID})" +
                "SELECT SCOPE_IDENTITY()";
            decimal value = db.ExecuteQuery<decimal>(query, proxy.FileName, proxy.FileType, proxy.Path).Min();
            proxy.AttachmentID = Decimal.ToInt32(value);
        }

        public static void DeleteActionAttachment(AuthenticationModel user, int organizationID, int ticketID, int actionID, int attachmentID)
        {
            // set WITH (ROWLOCK) 
            LoginUser loginUser = WebUtils.TSAuthentication.GetLoginUser();
            Attachment attachment = Attachments.GetAttachment(loginUser, attachmentID);
            attachment.DeleteFile();
            attachment.Delete();
            attachment.Collection.Save();
        }

        public static string AttachmentPath(DataContext db, int id)
        {
            string path = db.ExecuteQuery<string>($"SELECT Value FROM FilePaths WITH(NOLOCK) WHERE ID = {id}").FirstOrDefault();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public static int ActionAttachmentActionID(DataContext db, int attachmentID)
        {
            return db.ExecuteQuery<int>($"SELECT ActionID FROM ActionAttachments WITH(NOLOCK) WHERE AttachmentID = {attachmentID}").Min();
        }

        //public static AttachmentProxy[] GetActionAttachmentProxies(DataContext db, int attachmentID)
        //{
        //    return db.ExecuteQuery<AttachmentProxy>($"SELECT AttachmentID, OrganizationID, FileName, FileType, FileSize, Path, Description, DateCreated, DateModified, CreatorID, ModifierID, 0, ActionID, SentToJira, SentToTFS, SentToSnow, FilePathID " +
        //        $"FROM dbo.ActionAttachments WHERE AttachmentID = {attachmentID}").ToArray();
        //}

        public static int[] ActionAttachmentIDs(DataContext db, int organizationID, int ticketID, int actionID) { return db.ExecuteQuery<int>($"SELECT AttachmentID FROM ActionAttachments WHERE ActionID={actionID}").ToArray(); }

        #endregion

        #region Users

        //class FullName
        //{
        //    public string FirstName;
        //    public string LastName;
        //}
        //public static string UserFullName(DataContext db, int organizationID, int userID)
        //{
        //    string query = $"SELECT FirstName + ' ' + LastName FROM Users  WITH (NOLOCK) WHERE UserID={userID} AND OrganizationID={organizationID}";
        //    FullName fullName = db.ExecuteQuery<FullName>(query).First();  // throws if it fails
        //    return $"{fullName.FirstName} {fullName.LastName}";
        //}

        #endregion

        #region Tickets
        public static int[] TicketSelectActionIDs(DataContext db, int organizationID, int ticketID) { return db.ExecuteQuery<int>($"SELECT ActionID FROM Actions a WHERE a.TicketID={ticketID}").ToArray(); }
        #endregion


    }
}
