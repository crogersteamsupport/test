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
        public static void Create<TProxy, TModel>(TModel tModel, TProxy tProxy) where TProxy : class where TModel : class
        {
            string now = ToSql(DateTime.UtcNow);
            string command = String.Empty;
            switch (typeof(TProxy).Name) // alphabetized list
            {
                case "SubscriptionProxy":
                    {
                        TicketModel model = tModel as TicketModel;
                        int userID = model.Connection.User.UserID;
                        SubscriptionProxy proxy = tProxy as SubscriptionProxy;
                        command = $"INSERT INTO Subscriptions (RefType, RefID, UserID, DateCreated, DateModified, CreatorID, ModifierID)" +
                                $"SELECT 17, {model.TicketID}, {proxy.UserID}, '{now}','{now}', {userID}, {userID} " +
                                $"WHERE NOT EXISTS(SELECT * FROM Subscriptions WHERE reftype = 17 AND RefID = {model.TicketID} AND UserID = {proxy.UserID})";
                        model.Connection._db.ExecuteCommand(command);
                    }
                    break;
            }
            // TODO - log
        }

        /// <summary> 
        /// READ - read proxy given a model 
        /// </summary>
        public static TProxy Read<TProxy, TModel>(TModel tModel) where TProxy : class where TModel : class
        {
            TProxy tProxy = default(TProxy);
            switch (typeof(TProxy).Name) // alphabetized list
            {
                case "ActionProxy": // action
                    {
                        ActionModel model = tModel as ActionModel;
                        Table<ActionProxy> table = model.Connection._db.GetTable<ActionProxy>();
                        tProxy = table.Where(t => t.ActionID == model.ActionID).First() as TProxy;
                    }
                    break;
                case "ActionProxy[]":   // ticket actions
                    {
                        TicketModel model = tModel as TicketModel;
                        Table<ActionProxy> table = model.Connection._db.GetTable<ActionProxy>();
                        tProxy = table.Where(t => t.TicketID == model.TicketID).ToArray() as TProxy;
                    }
                    break;
                case "AttachmentProxy": // action attachment (organization attachments?)
                    {
                        ActionAttachment model = tModel as ActionAttachment;
                        string query = SelectActionAttachmentProxy + $"WHERE ActionAttachmentID = {model.ActionAttachmentID}";
                        tProxy = model.Connection._db.ExecuteQuery<AttachmentProxy>(query).First() as TProxy;
                    }
                    break;
                case "AttachmentProxy[]": // action attachments
                    {
                        ActionModel model = tModel as ActionModel;
                        string query = SelectActionAttachmentProxy + $"WHERE ActionID = {model.ActionID}";
                        tProxy = model.Connection._db.ExecuteQuery<AttachmentProxy>(query).ToArray() as TProxy;
                    }
                    break;
                case "SubscriptionModel[]":
                    {
                        TicketModel model = tModel as TicketModel;
                        string query = $"SELECT RefType,RefID,UserID,DateCreated,DateModified,CreatorID,ModifierID FROM Subscriptions " +
                            $"WHERE Reftype = 17 and Refid = {model.TicketID} and MarkDeleted = 0";
                        tProxy = model.Connection._db.ExecuteQuery<SubscriptionModel>(query).ToArray() as TProxy;
                    }
                    break;
                case "TicketProxy": // ticket
                    {
                        TicketModel model = tModel as TicketModel;
                        Table<TicketProxy> table = model.Connection._db.GetTable<TicketProxy>();
                        tProxy = table.Where(t => t.TicketID == model.TicketID).First() as TProxy;
                    }
                    break;
                case "TagLinkProxy[]":
                    {
                        //query = $"SELECT TagLinkID, TagID, RefType, RefID, DateCreated, CreatorID FROM TagLinks WHERE RefType = 17 AND RefID = {model.TicketID}";
                        TicketModel model = tModel as TicketModel;
                        Table<TagLinkProxy> table = model.Connection._db.GetTable<TagLinkProxy>();
                        tProxy = table.Where(t => (t.RefType == ReferenceType.Tickets) && (t.RefID == model.TicketID)).ToArray() as TProxy;
                    }
                    break;
                default:
                    throw new Exception("Bad call to DataApi.Read");
            }
            return tProxy;
        }

        /// <summary> 
        /// UPDATTE - update a model with the proxy data 
        /// </summary>
        public static void Update<TProxy, TModel>(TModel tModel, TProxy tProxy) where TProxy : class where TModel : class
        {
            string command = String.Empty;
            switch (typeof(TProxy).Name) // alphabetized list
            {
                case "TagLinkProxy":    // ticket tag links
                    TagLinkModel model = tModel as TagLinkModel;
                    TagLinkProxy proxy = tProxy as TagLinkProxy;
                    command = $"UPDATE TagLinks WITH(ROWLOCK) SET TagLinkID={proxy.TagLinkID}, TagID={proxy.TagID}, RefType={proxy.RefType}, RefID={proxy.RefID}  WHERE TagLinkID={model.TagLinkID}";
                    model.Connection._db.ExecuteCommand(command);
                    break;
            }
        }

        /// <summary> 
        /// DELETE - delete a model </summary>
        public static void Delete<T>(T t) where T : class
        {
            string command = String.Empty;
            switch (typeof(T).Name) // alphabetized list
            {
                case "TagLinkModel":
                    {
                        TagLinkModel model = t as TagLinkModel;
                        command = $"DELETE FROM TagLinks WITH (ROWLOCK) WHERE TagLinkID={model.TagLinkID}";
                        model.Connection._db.ExecuteCommand(command);
                    }
                    break;
                case "SubscriptionModel":
                    {
                        SubscriptionModel model = t as SubscriptionModel;
                        command = $"DELETE FROM Subscriptions WITH (ROWLOCK) WHERE UserID={model.UserID}";
                        model.Connection._db.ExecuteCommand(command);
                    }
                    break;
            }
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

        /// <summary> Create Action </summary>
        public static void Create(TicketModel ticketModel, ref ActionProxy actionProxy)
        {
            AuthenticationModel authentication = ticketModel.Connection.Authentication;
            Data.Action.Create(ticketModel.Connection._db, authentication.OrganizationID, authentication.UserID, ticketModel.TicketID, ref actionProxy);
            LogMessage(ActionLogType.Insert, ReferenceType.Actions, actionProxy.ActionID, "Created Action");
        }


        #region ActionAttachments


        /// <summary> Create Action Attachment </summary>
        public static void Create(ActionModel actionModel, AttachmentProxy proxy)
        {
            // hard code all the numbers, parameterize all the strings so they are SQL-Injection checked
            string query = "INSERT INTO ActionAttachments(OrganizationID, FileName, FileType, FileSize, Path, DateCreated, DateModified, CreatorID, ModifierID, ActionID, SentToJira, SentToTFS, SentToSnow, FilePathID) " +
                $"VALUES({actionModel.Connection.Organization.OrganizationID}, {{0}}, {{1}}, {proxy.FileSize}, {{2}}, '{ToSql(proxy.DateCreated)}', '{ToSql(proxy.DateModified)}', {proxy.CreatorID}, {proxy.ModifierID}, {actionModel.ActionID}, {ToSql(proxy.SentToJira)}, {ToSql(proxy.SentToTFS)}, {ToSql(proxy.SentToSnow)}, {proxy.FilePathID})" +
                "SELECT SCOPE_IDENTITY()";
            decimal value = actionModel.Connection._db.ExecuteQuery<decimal>(query, proxy.FileName, proxy.FileType, proxy.Path).Min();
            proxy.AttachmentID = Decimal.ToInt32(value);
        }

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

        /// <summary> Update Action Attachment </summary>

        /// <summary> Delete Action Attachment </summary>
        public static void Delete(ActionAttachment actionAttachment)
        {
            string query = $"DELETE FROM ActionAttachments WHERE ActionAttachmentID = {actionAttachment.ActionAttachmentID}";
            actionAttachment.Connection._db.ExecuteCommand(query);
            LogMessage(ActionLogType.Delete, ReferenceType.Actions, actionAttachment.ActionAttachmentID, "Deleted Action Attachment");
        }
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
