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
        #region Tickets
        /// <summary> Create Ticket </summary>
        public static void Create(ConnectionContext connection, TicketProxy ticketProxy)
        {
            // TODO - create ticket
            LogMessage(connection.Authentication, ActionLogType.Insert, ReferenceType.Tickets, ticketProxy.TicketID, "Created Ticket");
        }

        /// <summary> Read Ticket </summary>
        public static TicketProxy Read(ConnectionContext connection, TicketModel ticketModel)
        {
            Table<TicketProxy> table = connection._db.GetTable<TicketProxy>();
            return table.Where(t => t.TicketID == ticketModel.TicketID).First();
        }

        /// <summary> Update Ticket </summary>
        public static void Update(ConnectionContext connection, TicketModel ticketModel, TicketProxy ticketProxy)
        {
            // TODO - update ticket
            LogMessage(connection.Authentication, ActionLogType.Update, ReferenceType.Tickets, ticketModel.TicketID, "Updated Ticket");
        }

        /// <summary> Delete Ticket</summary>
        public static void Delete(ConnectionContext connection, TicketModel ticketModel)
        {
            // TODO - delete ticket
            LogMessage(connection.Authentication, ActionLogType.Delete, ReferenceType.Tickets, ticketModel.TicketID, "Deleted Ticket");
        }
        #endregion


        #region Actions
        /// <summary> Create Action </summary>
        public static void Create(ConnectionContext connection, TicketModel ticketModel, ref ActionProxy actionProxy)
        {
            AuthenticationModel authentication = connection.Authentication;
            Data.Action.Create(connection._db, authentication.OrganizationID, authentication.UserID, ticketModel.TicketID, ref actionProxy);
            LogMessage(connection.Authentication, ActionLogType.Insert, ReferenceType.Actions, actionProxy.ActionID, "Created Action");
        }

        /// <summary> Read Action </summary>
        public static ActionProxy Read(ConnectionContext connection, ActionModel actionModel)
        {
            Table<ActionProxy> table = connection._db.GetTable<ActionProxy>();
            return table.Where(t => t.ActionID == actionModel.ActionID).First();
        }

        /// <summary> Read ticket Actions </summary>
        public static void Read(ConnectionContext connection, TicketModel ticketModel, out ActionProxy[] actionProxies)
        {
            Table<ActionProxy> table = connection._db.GetTable<ActionProxy>();
            actionProxies = table.Where(t => t.TicketID == ticketModel.TicketID).ToArray();
        }

        /// <summary> Update Action </summary>
        public static void Update(ConnectionContext connection, ActionModel actionModel, ActionProxy actionProxy)
        {
            // TODO - update action
            LogMessage(connection.Authentication, ActionLogType.Update, ReferenceType.Actions, actionModel.ActionID, "Updated Action");
        }

        /// <summary> Delete Action </summary>
        public static void Delete(ConnectionContext connection, ActionModel actionModel)
        {
            // TODO - delete action
            LogMessage(connection.Authentication, ActionLogType.Delete, ReferenceType.Actions, actionModel.ActionID, "Deleted Action");
        }
        #endregion


        #region ActionAttachments
        /// <summary> Create Action Attachment </summary>
        public static void Create(ConnectionContext connection, ActionModel action, AttachmentProxy attachmentProxy)
        {
            Attachment.CreateActionAttachment(attachmentProxy);
            LogMessage(connection.Authentication, ActionLogType.Insert, ReferenceType.Attachments, attachmentProxy.AttachmentID, "Created Ticket");
        }

        /// <summary> Read Action Attachment </summary>
        public static AttachmentProxy Read(ConnectionContext connection, ActionAttachment actionAttachment)
        {
            Table<AttachmentProxy> table = connection._db.GetTable<AttachmentProxy>();
            return table.Where(t => t.AttachmentID == actionAttachment.ActionAttachmentID).First();
        }

        /// <summary> Create Action Attachments </summary>
        public static void Read(ConnectionContext connection, ActionModel actionModel, out AttachmentProxy[] attachments)
        {
            Table<AttachmentProxy> table = connection._db.GetTable<AttachmentProxy>();
            attachments = table.Where(t => (t.RefID == actionModel.ActionID) && (t.RefType == ReferenceType.Actions)).ToArray();
        }

        /// <summary> Update Action Attachment </summary>
        public static void Update(ConnectionContext connection, ActionAttachment actionAttachmentModel, AttachmentProxy attachment)
        {
            // TODO - update action attachment
            LogMessage(connection.Authentication, ActionLogType.Update, ReferenceType.Attachments, actionAttachmentModel.ActionAttachmentID, "Updated Action Attachment");
        }

        /// <summary> Delete Action Attachment </summary>
        public static void Delete(ConnectionContext connection, ActionAttachment actionAttachmentModel)
        {
            // TODO - delete action attachment
            LogMessage(connection.Authentication, ActionLogType.Delete, ReferenceType.Actions, actionAttachmentModel.ActionAttachmentID, "Deleted Action Attachment");
        }
        #endregion


        #region Log
        /// <summary> Log Message </summary>
        public static void LogMessage(AuthenticationModel authentication, ActionLogType logType, ReferenceType refType, int? refID, string message)
        {
            LoginUser user = new LoginUser(authentication.UserID, authentication.OrganizationID);
            ActionLogs.AddActionLog(user, logType, refType, refID.HasValue ? refID.Value : 0, message);  // 0 if no ID?
        }

        public static void LogMessage(AuthenticationModel authentication, ActionLogType logType, ReferenceType refType, int? refID, string message, Exception ex)
        {
            // log to ExceptionLogs or New Relic, or windows event log?

            string fullMessage = message + ex.ToString() + " ----- STACK: " + ex.StackTrace.ToString();
            if (Debugger.IsAttached)
            {
                Debug.WriteLine(fullMessage);   // debug output window (very fast)
                Debugger.Break();   // something is wrong - fix the code!
            }

            LogMessage(authentication, logType, refType, refID, fullMessage);
        }
        #endregion

        // verified connection and action
        public static void Create(ConnectionContext connection, ActionModel action, List<ActionAttachment> attachments)
        {
            if (attachments.Count == 0)
                return;

            LogMessage(connection.Authentication, ActionLogType.Insert, ReferenceType.Attachments, attachments[0].Action.ActionID, "Attachments Saved");
        }

        public static int ActionGetTicketID(DataContext db, int actionID) { return db.ExecuteQuery<int>($"SELECT TicketID FROM Actions WITH (NOLOCK) WHERE ActionID = {actionID}").Min(); }

        public static int ActionCreatorID(DataContext db, int actionID) { return db.ExecuteQuery<int>($"SELECT CreatorID FROM Actions WITH (NOLOCK) WHERE ActionID={actionID}").Min(); }

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


        public static int[] TicketSelectActionIDs(DataContext db, int organizationID, int ticketID) { return db.ExecuteQuery<int>($"SELECT ActionID FROM Actions a WHERE a.TicketID={ticketID}").ToArray(); }

    }
}
