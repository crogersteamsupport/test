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
        public static void Create<TProxy>(IdInterface iModel, TProxy tProxy) where TProxy : class
        {
            string now = ToSql(DateTime.UtcNow);
            int userID = iModel.Connection.User.UserID;

            string command = String.Empty;
            switch (typeof(TProxy).Name) // alphabetized list
            {
                case "ActionAttachment":
                    {
                        ActionModel model = (ActionModel)iModel;
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
                        TicketModel model = (TicketModel)iModel;
                        ActionProxy proxy = tProxy as ActionProxy;
                        AuthenticationModel authentication = model.Connection.Authentication;
                        Data.Action.Create(model.Connection._db, authentication.OrganizationID, authentication.UserID, model.TicketID, ref proxy);
                    }
                    break;
                case "ContactProxy":
                    {
                        TicketModel model = (TicketModel)iModel;
                        ContactProxy proxy = tProxy as ContactProxy;
                        command = $"INSERT INTO UserTickets (TicketID, UserID, DateCreated, CreatorID)" +
                            $"SELECT {model.TicketID}, {proxy.UserID}, '{now}', {userID} ";
                    }
                    break;
                case "CustomerProxy":
                    {
                        TicketModel model = (TicketModel)iModel;
                        CustomerProxy proxy = tProxy as CustomerProxy;
                        command = $"INSERT INTO OrganizationTickets (TicketID, OrganizationID, DateCreated, CreatorID, DateModified, ModifierID)" +
                            $"SELECT {model.TicketID}, {proxy.OrganizationID}, '{now}', {userID}, '{now}', {userID}"; 
                    }
                    break;
                case "SubscriptionProxy":
                    {
                        TicketModel model = (TicketModel)iModel;
                        SubscriptionProxy proxy = tProxy as SubscriptionProxy;
                        command = $"INSERT INTO Subscriptions (RefType, RefID, UserID, DateCreated, DateModified, CreatorID, ModifierID)" +
                                $"SELECT 17, {model.TicketID}, {proxy.UserID}, '{now}','{now}', {userID}, {userID} ";
                    }
                    break;
            }

            if(!String.IsNullOrEmpty(command))
                iModel.Connection._db.ExecuteCommand(command);
            // TODO - log
        }

        /// <summary> 
        /// READ - read proxy given a model 
        /// </summary>
        public static TProxy Read<TProxy>(IdInterface iModel) where TProxy : class
        {
            TProxy tProxy = default(TProxy);
            switch (typeof(TProxy).Name) // alphabetized list
            {
                case "ActionProxy": // action
                    {
                        ActionModel model = (ActionModel)iModel;
                        Table<ActionProxy> table = model.Connection._db.GetTable<ActionProxy>();
                        tProxy = table.Where(t => t.ActionID == model.ActionID).First() as TProxy;
                    }
                    break;
                case "ActionProxy[]":   // ticket actions
                    {
                        TicketModel model = (TicketModel)iModel;
                        Table<ActionProxy> table = model.Connection._db.GetTable<ActionProxy>();
                        tProxy = table.Where(t => t.TicketID == model.TicketID).ToArray() as TProxy;
                    }
                    break;
                case "AttachmentProxy": // action attachment (organization attachments?)
                    {
                        ActionAttachment model = (ActionAttachment)iModel;
                        string query = SelectActionAttachmentProxy + $"WHERE ActionAttachmentID = {model.ActionAttachmentID}";
                        tProxy = model.Connection._db.ExecuteQuery<AttachmentProxy>(query).First() as TProxy;
                    }
                    break;
                case "AttachmentProxy[]": // action attachments
                    {
                        ActionModel model = (ActionModel)iModel;
                        string query = SelectActionAttachmentProxy + $"WHERE ActionID = {model.ActionID}";
                        tProxy = model.Connection._db.ExecuteQuery<AttachmentProxy>(query).ToArray() as TProxy;
                    }
                    break;
                case "ReminderProxy":
                    {
                        ReminderModel model = (ReminderModel)iModel;
                        string query = $"SELECT ReminderID, OrganizationID, RefType, RefID, Description, DueDate, UserID, IsDismissed, HasEmailSent, CreatorID, DateCreated " +
                            $"FROM Reminders WHERE ReminderID = {model.ReminderID}";
                        tProxy = model.Connection._db.ExecuteQuery<ReminderProxy>(query).ToArray() as TProxy;
                    }
                    break;
                case "SubscriptionModel[]":
                    {
                        TicketModel model = (TicketModel)iModel;
                        string query = $"SELECT RefType,RefID,UserID,DateCreated,DateModified,CreatorID,ModifierID FROM Subscriptions " +
                            $"WHERE Reftype = 17 and Refid = {model.TicketID} and MarkDeleted = 0";
                        tProxy = model.Connection._db.ExecuteQuery<SubscriptionModel>(query).ToArray() as TProxy;
                    }
                    break;
                case "TaskAssociationProxy":
                    {
                        TaskAssociationModel model = (TaskAssociationModel)iModel;
                        string query = $"SELECT TaskID, RefID, RefType,CreatorID, DateCreated FROM TaskAssociations WHERE TaskID = {model.TaskID} AND RefID = {model.Ticket.TicketID} AND RefType = 17";
                        tProxy = model.Connection._db.ExecuteQuery<TaskAssociationProxy>(query).First() as TProxy;
                    }
                    break;
                case "TagLinkProxy[]":
                    {
                        //query = $"SELECT TagLinkID, TagID, RefType, RefID, DateCreated, CreatorID FROM TagLinks WHERE RefType = 17 AND RefID = {model.TicketID}";
                        TicketModel model = (TicketModel)iModel;
                        Table<TagLinkProxy> table = model.Connection._db.GetTable<TagLinkProxy>();
                        tProxy = table.Where(t => (t.RefType == ReferenceType.Tickets) && (t.RefID == model.TicketID)).ToArray() as TProxy;
                    }
                    break;
                case "TicketProxy": // ticket
                    {
                        TicketModel model = (TicketModel)iModel;
                        Table<TicketProxy> table = model.Connection._db.GetTable<TicketProxy>();
                        tProxy = table.Where(t => t.TicketID == model.TicketID).First() as TProxy;
                    }
                    break;
                default:
                    throw new Exception("Bad call to DataApi.Read");
            }
            return tProxy;
        }

        /// <summary> 
        /// UPDATE - update a model with the proxy data 
        /// 
        /// TODO:
        ///     ModifierID, DateTimeModified
        ///     Logging
        /// </summary>
        public static void Update<TProxy>(IdInterface iModel, TProxy tProxy) where TProxy : class
        {
            string command = String.Empty;
            switch (typeof(TProxy).Name) // alphabetized list
            {
                case "AssetTicketProxy":
                    {
                        //SELECT[TicketID],[AssetID],[DateCreated],[CreatorID],[ImportFileID] FROM[AssetTickets]
                        TagLinkModel model = (TagLinkModel)iModel;
                        TagLinkProxy proxy = tProxy as TagLinkProxy;
                        command = $"UPDATE TagLinks WITH(ROWLOCK) SET TagID={proxy.TagID}, RefType=17, RefID={proxy.RefID} WHERE TagLinkID={model.TagLinkID}";
                    }
                    break;
                case "TagLinkProxy":    // ticket tag links
                    {
                        TagLinkModel model = (TagLinkModel)iModel;
                        TagLinkProxy proxy = tProxy as TagLinkProxy;
                        command = $"UPDATE TagLinks WITH(ROWLOCK) SET TagID={proxy.TagID}, RefType=17, RefID={proxy.RefID} WHERE TagLinkID={model.TagLinkID}";
                    }
                    break;
                case "TaskAssociationProxy":
                    {
                        TicketModel model = (TicketModel)iModel;
                        TaskAssociationProxy proxy = tProxy as TaskAssociationProxy;
                        command = $"UPDATE TaskAssociations SET RefID = {model.TicketID} WHERE(TaskId = {proxy.TaskID})";
                    }
                    break;
                case "ReminderProxy":    // ticket reminder
                    {
                        ReminderModel model = (ReminderModel)iModel;
                        ReminderProxy proxy = tProxy as ReminderProxy;
                        command = $" UPDATE Reminders WITH(ROWLOCK) SET RefID ={proxy.RefID} " +
                            $"WHERE(ReminderId = {model.ReminderID})";
                    }
                    break;
            }

            iModel.Connection._db.ExecuteCommand(command);
        }

        /// <summary> 
        /// DELETE - delete a model </summary>
        public static void Delete(IdInterface iModel)
        {
            string command = String.Empty;
            switch (iModel.GetType().Name) // alphabetized list
            {
                case "ActionAttachment":
                    {
                        ActionAttachment model = (ActionAttachment)iModel;
                        command = $"DELETE FROM ActionAttachments WHERE ActionAttachmentID = {model.ActionAttachmentID}";
                    }
                    break;
                case "AssetTicketModel":
                    {
                        AssetTicketModel model = (AssetTicketModel)iModel;
                        command = $"DELETE FROM AssetTickets WHERE TicketID = {model.AssetID} AND AssetID = {model.AssetID}";
                    }
                    break;
                case "Contact":
                    {
                        Contact model = (Contact)iModel;
                        command = $"DELETE FROM UserTickets Where TicketID={model.Ticket.TicketID} AND UserId = {model.UserID}";
                    }
                    break;
                case "Customer":
                    {
                        Customer model = (Customer)iModel;
                        command = $"DELETE FROM OrganizationTickets WHERE TicketID={model.Ticket.TicketID} AND OrganizationId = {model.OrganizationID}";
                    }
                    break;
                case "TagLinkModel":
                    {
                        TagLinkModel model = (TagLinkModel)iModel;
                        command = $"DELETE FROM TagLinks WHERE TagLinkID={model.TagLinkID}";
                    }
                    break;
                case "SubscriptionModel":
                    {
                        SubscriptionModel model = (SubscriptionModel)iModel;
                        command = $"DELETE FROM Subscriptions WHERE RefType=17 AND RefID={model.Ticket.TicketID} AND UserID={model.UserID}";
                    }
                    break;
            }
            iModel.Connection._db.ExecuteCommand(command);
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
        public static void ReadActionAttachmentsForTicket(TicketModel ticketModel, ActionAttachmentsByTicketID ticketActionAttachments, out AttachmentProxy[] mostRecentByFilename)
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
