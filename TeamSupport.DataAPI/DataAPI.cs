﻿using System;
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
using TeamSupport.IDTree;

namespace TeamSupport.DataAPI
{
    /// <summary>
    /// CRUD Interface (Create, Read, Update, Delete) on verified connection context and verified model objects
    /// 
    /// Log all changes to DB here!!  Thanks :)
    /// </summary>
    public static class DataAPI
    {
        /// <summary> default ToString() doesn't work in some cases </summary>
        static string ToSql(DateTime dateTime) { return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"); }
        static char ToSql(bool value) { return value ? '1' : '0'; }

        /// <summary>
        /// CREATE - create proxy child for model parent
        /// </summary>
        public static void Create<TProxy>(IDNode idNode, TProxy tProxy) where TProxy : class
        {
            string modification = $", CreatorID={idNode.Connection.UserID}, DateCreated={ToSql(DateTime.UtcNow)}";

            string now = ToSql(DateTime.UtcNow);
            int creatorID = idNode.Connection.UserID;

            string command = String.Empty;
            switch (typeof(TProxy).Name) // alphabetized list
            {
                case "ActionAttachment":
                    {
                        ActionNode model = (ActionNode)idNode;
                        AttachmentProxy proxy = tProxy as AttachmentProxy;
                        string query = "SET Context_Info 0x55555; " + 
                            "INSERT INTO ActionAttachments(OrganizationID, FileName, FileType, FileSize, Path, DateCreated, DateModified, CreatorID, ModifierID, ActionID, SentToJira, SentToTFS, SentToSnow, FilePathID) " +
                            $"VALUES({model.Connection.Organization.OrganizationID}, {{0}}, {{1}}, {proxy.FileSize}, {{2}}, '{ToSql(proxy.DateCreated)}', '{ToSql(proxy.DateModified)}', {proxy.CreatorID}, {proxy.ModifierID}, {model.ActionID}, {ToSql(proxy.SentToJira)}, {ToSql(proxy.SentToTFS)}, {ToSql(proxy.SentToSnow)}, {proxy.FilePathID})" +
                            "SELECT SCOPE_IDENTITY()";
                        decimal value = model.Connection._db.ExecuteQuery<decimal>(query, proxy.FileName, proxy.FileType, proxy.Path).Min();
                        proxy.AttachmentID = Decimal.ToInt32(value);
                    }
                    break;
                case "ActionProxy":
                    {
                        TicketNode model = (TicketNode)idNode;
                        ActionProxy proxy = tProxy as ActionProxy;
                        AuthenticationModel authentication = model.Connection.Authentication;
                        Data.Action.Create(model.Connection._db, authentication.OrganizationID, authentication.UserID, model.TicketID, ref proxy);
                    }
                    break;
                case "ContactProxy":
                    {
                        TicketNode model = (TicketNode)idNode;
                        ContactProxy proxy = tProxy as ContactProxy;
                        command = $"INSERT INTO UserTickets (TicketID, UserID, DateCreated, CreatorID)" +
                            $"SELECT {model.TicketID}, {proxy.UserID}, '{now}', {creatorID} ";
                    }
                    break;
                case "CustomerProxy":
                    {
                        TicketNode model = (TicketNode)idNode;
                        CustomerProxy proxy = tProxy as CustomerProxy;
                        command = $"INSERT INTO OrganizationTickets (TicketID, OrganizationID, DateCreated, CreatorID, DateModified, ModifierID)" +
                            $"SELECT {model.TicketID}, {proxy.OrganizationID}, '{now}', {creatorID}, '{now}', {creatorID}"; 
                    }
                    break;
                case "SubscriptionProxy":
                    {
                        TicketNode model = (TicketNode)idNode;
                        SubscriptionProxy proxy = tProxy as SubscriptionProxy;
                        command = $"INSERT INTO Subscriptions (RefType, RefID, UserID, DateCreated, DateModified, CreatorID, ModifierID)" +
                                $"SELECT 17, {model.TicketID}, {proxy.UserID}, '{now}','{now}', {creatorID}, {creatorID} ";
                    }
                    break;
                default:
                    if (Debugger.IsAttached) Debugger.Break();
                    break;
            }

            if (!String.IsNullOrEmpty(command))
                idNode.Connection._db.ExecuteCommand(command);
            // TODO - log
        }

        /// <summary> 
        /// READ - read proxy given a model 
        /// </summary>
        public static TProxy Read<TProxy>(IDNode node) where TProxy : class
        {
            TProxy tProxy = default(TProxy);
            switch (typeof(TProxy).Name) // alphabetized list
            {
                case "ActionProxy": // action
                    {
                        ActionNode action = (ActionNode)node;
                        Table<ActionProxy> table = action.Connection._db.GetTable<ActionProxy>();
                        tProxy = table.Where(t => t.ActionID == action.ActionID).First() as TProxy;
                    }
                    break;
                case "ActionProxy[]":   // ticket actions
                    {
                        TicketNode ticket = (TicketNode)node;
                        Table<ActionProxy> table = ticket.Connection._db.GetTable<ActionProxy>();
                        tProxy = table.Where(t => t.TicketID == ticket.TicketID).ToArray() as TProxy;
                    }
                    break;
                case "AttachmentProxy": // action attachment (organization attachments?)
                    {
                        ActionAttachmentNode attachment = (ActionAttachmentNode)node;
                        string query = SelectActionAttachmentProxy + $"WHERE ActionAttachmentID = {attachment.ActionAttachmentID}";
                        tProxy = attachment.Connection._db.ExecuteQuery<AttachmentProxy>(query).First() as TProxy;
                    }
                    break;
                case "AttachmentProxy[]": // action attachments
                    {
                        ActionNode action = (ActionNode)node;
                        string query = SelectActionAttachmentProxy + $"WHERE ActionID = {action.ActionID}";
                        tProxy = action.Connection._db.ExecuteQuery<AttachmentProxy>(query).ToArray() as TProxy;
                    }
                    break;
                case "ReminderProxy":
                    {
                        TicketReminderNode reminder = (TicketReminderNode)node;
                        string query = $"SELECT ReminderID, OrganizationID, RefType, RefID, Description, DueDate, UserID, IsDismissed, HasEmailSent, CreatorID, DateCreated " +
                            $"FROM Reminders WHERE ReminderID = {reminder.ReminderID} AND RefType=17";
                        tProxy = reminder.Connection._db.ExecuteQuery<ReminderProxy>(query).ToArray() as TProxy;
                    }
                    break;
                case "SubscriptionModel[]":
                    {
                        TicketNode ticket = (TicketNode)node;
                        string query = $"SELECT RefType,RefID,UserID,DateCreated,DateModified,CreatorID,ModifierID FROM Subscriptions " +
                            $"WHERE Reftype = 17 and RefID = {ticket.TicketID} and MarkDeleted = 0";
                        tProxy = ticket.Connection._db.ExecuteQuery<SubscriptionNode>(query).ToArray() as TProxy;
                    }
                    break;
                case "TaskAssociationProxy":
                    {
                        TaskAssociationNode taskAssociation = (TaskAssociationNode)node;
                        string query = String.Empty;// = $"SELECT TaskID, RefID, RefType,CreatorID, DateCreated FROM TaskAssociations WHERE TaskID = {model.TaskID} AND RefID = {model.Ticket.TicketID} AND RefType = 17";
                        tProxy = taskAssociation.Connection._db.ExecuteQuery<TaskAssociationProxy>(query).First() as TProxy;
                    }
                    break;
                case "TagLinkProxy[]":
                    {
                        TicketNode ticket = (TicketNode)node;
                        //string query = $"SELECT * FROM TagLinks WHERE RefType = 17 AND RefID = {ticket.TicketID}";
                        //tProxy = ticket.Connection._db.ExecuteQuery<TProxy>(query).ToArray() as TProxy;

                        Table<TagLinkProxy> table = ticket.Connection._db.GetTable<TagLinkProxy>();
                        tProxy = table.Where(t => (t.RefType == ReferenceType.Tickets) && (t.RefID == ticket.TicketID)).ToArray() as TProxy;
                    }
                    break;
                case "TicketProxy": // ticket
                    {
                        TicketNode ticket = (TicketNode)node;
                        Table<TicketProxy> table = ticket.Connection._db.GetTable<TicketProxy>();
                        tProxy = table.Where(t => t.TicketID == ticket.TicketID).First() as TProxy;
                    }
                    break;
                case "TicketRelationshipProxy[]":
                    {
                        TicketNode model = (TicketNode)node;
                        //string query = $"SELECT * FROM TicketRelationships WHERE Ticket1ID={model.TicketID} OR Ticket2ID={model.TicketID}";
                        string query = $"SELECT TicketRelationshipID, OrganizationID, Ticket1ID, Ticket2ID, CreatorID, DateCreated, ImportFileID FROM TicketRelationships " +
                            $"WHERE Ticket1ID={model.TicketID} OR Ticket2ID={model.TicketID}";
                        tProxy = model.Connection._db.ExecuteQuery<TicketRelationshipProxy>(query).ToArray() as TProxy;
                    }
                    break;
                default:
                    if (Debugger.IsAttached) Debugger.Break();
                    break;
            }
            return tProxy;
        }

        public static void Update(IDNode node, UpdateArguments args)
        {
            string modification = $", ModifierID={node.Connection.UserID}, DateModified={ToSql(DateTime.UtcNow)}";

            int id;
            string command = String.Empty;
            switch (node.GetType().Name) // alphabetized list
            {
                case "ActionNode":
                    id = ((ActionNode)node).ActionID;
                    command = $"UPDATE Actions WITH(ROWLOCK) SET {args.ToString()} WHERE ActionID={id}";
                    break;
                case "TagLinkNode":
                    id = ((TagLinkNode)node).TagLinkID;
                    command = $"UPDATE TagLinks WITH(ROWLOCK) SET {args.ToString()} WHERE TagLinkID={id} AND RefType=17";
                    break;
                case "TaskAssociationNode":
                    id = ((TaskAssociationNode)node).Task.TaskID;
                    command = $"UPDATE TaskAssociations SET {args.ToString()} WHERE TaskID={id} AND RefType=17";
                    break;
                case "TicketNode":
                    id = ((TicketNode)node).TicketID;
                    command = $"UPDATE Tickets SET {args.ToString()} WHERE TicketID= {id}";
                    break;
                case "TicketReminderNode":
                    id = ((TicketReminderNode)node).ReminderID;
                    command = $" UPDATE Reminders WITH(ROWLOCK) SET {args.ToString()} WHERE ReminderID={id} AND RefType=17";
                    break;
                default:
                    if(Debugger.IsAttached) Debugger.Break();
                    break;
            }
            node.Connection._db.ExecuteCommand(command);
            // TODO - Log
        }

        /// <summary> 
        /// UPDATE - update a model with the proxy data 
        /// 
        /// TODO:
        ///     ModifierID, DateTimeModified
        ///     Logging
        /// </summary>
        //private static void Update<TProxy>(IDNode iModel, TProxy tProxy) where TProxy : class
        //{
        //    string command = String.Empty;
        //    switch (typeof(TProxy).Name) // alphabetized list
        //    {
        //        case "TagLinkProxy":    // ticket tag links
        //            {
        //                TagLinkNode model = (TagLinkNode)iModel;
        //                TagLinkProxy proxy = tProxy as TagLinkProxy;
        //                command = $"UPDATE TagLinks WITH(ROWLOCK) SET TagID={proxy.TagID}, RefType=17, RefID={proxy.RefID} WHERE TagLinkID={model.TagLinkID}";
        //            }
        //            break;
        //        case "TaskAssociationProxy":
        //            {
        //                TicketNode model = (TicketNode)iModel;
        //                TaskAssociationProxy proxy = tProxy as TaskAssociationProxy;
        //                command = $"UPDATE TaskAssociations SET RefID = {model.TicketID} WHERE(TaskId = {proxy.TaskID})";
        //            }
        //            break;
        //        case "TicketProxy":
        //            {
        //                TicketNode model = (TicketNode)iModel;
        //                TicketProxy proxy = tProxy as TicketProxy;
        //                command = $"UPDATE Tickets SET RefID = {model.TicketID} WHERE(TicketID= {model.TicketID})";
        //            }
        //            break;
        //        case "ReminderProxy":    // ticket reminder
        //            {
        //                TicketReminderNode model = (TicketReminderNode)iModel;
        //                ReminderProxy proxy = tProxy as ReminderProxy;
        //                command = $" UPDATE Reminders WITH(ROWLOCK) SET RefID ={proxy.RefID} " +
        //                    $"WHERE(ReminderId = {model.ReminderID})";
        //            }
        //            break;
        //        default:
        //            if (Debugger.IsAttached) Debugger.Break();
        //            break;
        //    }

        //    iModel.Connection._db.ExecuteCommand(command);
        //}

        /// <summary> 
        /// DELETE - delete a model </summary>
        public static void Delete(ref IDNode node)
        {
            int modifierID = node.Connection.UserID;

            string command = String.Empty;
            switch (node.GetType().Name) // alphabetized list
            {
                case "ActionAttachmentNode":
                    {
                        ActionAttachmentNode model = (ActionAttachmentNode)node;
                        command = $"DELETE FROM ActionAttachments WHERE ActionAttachmentID = {model.ActionAttachmentID}";
                    }
                    break;
                case "AssetTicketNode":
                    {
                        AssetTicketNode model = (AssetTicketNode)node;
                        //command = $"DELETE FROM AssetTickets WHERE TicketID = {model.AssetID} AND AssetID = {model.AssetID}";
                    }
                    break;
                case "UserTicketNode":
                    {
                        UserTicketNode model = (UserTicketNode)node;
                        //command = $"DELETE FROM UserTickets Where TicketID={model.Ticket.TicketID} AND UserId = {model.UserID}";
                    }
                    break;
                case "OrganizationTicketNode":
                    {
                        OrganizationTicketNode model = (OrganizationTicketNode)node;
                        command = $"DELETE FROM OrganizationTickets WHERE TicketID={model.Ticket.TicketID} AND OrganizationId = {model.Organization.OrganizationID}";
                    }
                    break;
                case "TagLinkNode":
                    {
                        TagLinkNode model = (TagLinkNode)node;
                        command = $"DELETE FROM TagLinks WHERE TagLinkID={model.TagLinkID}";
                    }
                    break;
                case "TicketNode":
                    {
                        TicketNode model = (TicketNode)node;
                        command = $"DELETE FROM Tickets WHERE TicketID={model.TicketID}";
                    }
                    break;
                case "TicketRelationshipNode":
                    {
                        TicketRelationshipNode model = (TicketRelationshipNode)node;
                        command = $"DELETE FROM TicketRelationships WHERE TicketID={model.TicketRelationshipID}";
                    }
                    break;
                case "SubscriptionNode":
                    {
                        SubscriptionNode model = (SubscriptionNode)node;
                        command = $"DELETE FROM Subscriptions WHERE RefType=17 AND RefID={model.Ticket.TicketID} AND UserID={model.User.UserID}";
                    }
                    break;
                default:
                    if (Debugger.IsAttached) Debugger.Break();
                    break;
            }
            node.Connection._db.ExecuteCommand(command);
            node = null;    // it's gone :)
            // TODO - log
        }

        //class FullName
        //{
        //    public string FirstName;
        //    public string LastName;
        //}
        //public static string UserFullName(DataContext db, int organizationID, int userID)
        //{
        //    string query = $"SELECT FirstName, LastName FROM Users  WITH (NOLOCK) WHERE UserID={userID} AND OrganizationID={organizationID}";
        //    FullName fullName = db.ExecuteQuery<FullName>(query).First();  // throws if it fails
        //    return $"{fullName.FirstName} {fullName.LastName}";
        //}


        #region ActionAttachments


        // load action attachments into attachment proxy
        const string SelectActionAttachmentProxy = "SELECT a.*, a.ActionAttachmentID as AttachmentID, a.ActionAttachmentGUID as AttachmentGUID, (u.FirstName + ' ' + u.LastName) AS CreatorName, a.ActionID as RefID " +
            "FROM ActionAttachments a LEFT JOIN Users u ON u.UserID = a.CreatorID ";

        /// <summary> Read most recent filenames for this ticket </summary>
        public static void ReadActionAttachmentsForTicket(TicketNode ticketModel, ActionAttachmentsByTicketID ticketActionAttachments, out AttachmentProxy[] mostRecentByFilename)
        {
            switch (ticketActionAttachments)
            {
                case ActionAttachmentsByTicketID.ByFilename:
                    {
                        string query = SelectActionAttachmentProxy + $"WHERE ActionID IN (SELECT ActionID FROM Actions WHERE TicketID = {ticketModel.TicketID}) ORDER BY DateCreated DESC";
                        AttachmentProxy[] allAttachments = ticketModel.Connection._db.ExecuteQuery<AttachmentProxy>(query).ToArray();
                        List<AttachmentProxy> tmp = new List<AttachmentProxy>();
                        foreach (AttachmentProxy attachment in allAttachments)
                        {
                            if (!tmp.Exists(a => a.FileName == attachment.FileName))
                                tmp.Add(attachment);
                        }
                        mostRecentByFilename = tmp.ToArray();
                    }
                    break;
                case ActionAttachmentsByTicketID.KnowledgeBase:
                    {
                        string query = SelectActionAttachmentProxy + $"JOIN Actions ac ON a.ActionID = ac.ActionID WHERE ac.TicketID = {ticketModel.TicketID} AND ac.IsKnowledgeBase = 1";
                        mostRecentByFilename = ticketModel.Connection._db.ExecuteQuery<AttachmentProxy>(query).ToArray();
                    }
                    break;
                default:
                    mostRecentByFilename = null;
                    break;
            }
        }


        ///// <summary> Read most recent filenames for this ticket </summary>
        //public static void ReadActionAttachmentsByFilenameAndTicket(TicketModel ticketModel, out AttachmentProxy[] mostRecentByFilename)
        //{
        //    string query = SelectActionAttachmentProxy + $"WHERE ActionID IN (SELECT ActionID FROM Actions WHERE TicketID = {ticketModel.TicketID}) ORDER BY DateCreated DESC";
        //    AttachmentProxy[] allAttachments = ticketModel.Connection._db.ExecuteQuery<AttachmentProxy>(query).ToArray();
        //    List<AttachmentProxy> tmp = new List<AttachmentProxy>();
        //    foreach (AttachmentProxy attachment in allAttachments)
        //    {
        //        if (!tmp.Exists(a => a.FileName == attachment.FileName))
        //            tmp.Add(attachment);
        //    }
        //    mostRecentByFilename = tmp.ToArray();
        //}

        ///// <summary> Read most recent filenames for this ticket </summary>
        //public static void ReadKBActionAttachmentsByTicket(TicketModel ticketModel, out AttachmentProxy[] mostRecentByFilename)
        //{
        //    string query = SelectActionAttachmentProxy + $"JOIN Actions ac ON a.ActionID = ac.ActionID WHERE ac.TicketID = {ticketModel.TicketID} AND ac.IsKnowledgeBase = 1";
        //    mostRecentByFilename = ticketModel.Connection._db.ExecuteQuery<AttachmentProxy>(query).ToArray();
        //}

        #endregion


        #region Log
        /// <summary> Log Message </summary>
        public static void LogMessage(ActionLogType logType, ReferenceType refType, int? refID, string message)
        {
            AuthenticationModel authentication = new AuthenticationModel();
            LoginUser user = new LoginUser(authentication.UserID, authentication.OrganizationID);
            ActionLogs.AddActionLog(user, logType, refType, refID.HasValue ? refID.Value : 0, message);  // 0 if no ID?
        }

        public static void LogMessage(ActionLogType logType, ReferenceType refType, int? refID, string message, Exception ex)
        {
            // log to ExceptionLogs or New Relic, or windows event log?

            string fullMessage = message + ex.ToString() + " ----- STACK: " + ex.StackTrace.ToString();
            if (Debugger.IsAttached)
            {
                Debug.WriteLine(fullMessage);   // see the error in the debug output window
                Debugger.Break();   // something is wrong - fix the code!
            }

            LogMessage(logType, refType, refID, fullMessage);
        }
        #endregion

    }
}
