using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using TeamSupport.Data;
using TeamSupport.IDTree;
using System.Security.Authentication;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Diagnostics;
using TeamSupport.DataAPI;

namespace TeamSupport.ModelAPI
{
    /// <summary> CRUD interface to DataAPI - Create, Read, Update, Delete proxy </summary>
    public static class Model_API
    {
        /// <summary> 
        /// CREATE
        /// </summary>
        public static void Create<T>(T proxy)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    switch (typeof(T).Name)
                    {
                        case "ActionProxy":
                            ActionProxy actionProxy = proxy as ActionProxy;
                            Data_API.Create(connection.Ticket(actionProxy.TicketID), actionProxy);
                            break;
                        case "AttachmentProxy":
                            AttachmentProxy attachmentProxy = proxy as AttachmentProxy;
                            ActionModel actionModel = new ActionModel(connection, ((ActionAttachmentProxy)attachmentProxy).ActionID);
                            Data_API.Create(actionModel, attachmentProxy);
                            break;
                    }
                }
            }
            catch (AuthenticationException ex)
            {
                // TODO - tell user they don't have permission
                Data_API.LogMessage(ActionLogType.Insert, ReferenceType.None, 0, "choke", ex);
            }
            catch (System.Data.ConstraintException ex)
            {
                // TODO - data integrity failure
                Data_API.LogMessage(ActionLogType.Insert, ReferenceType.None, 0, "choke", ex);
            }
            catch (Exception ex)
            {
                // TODO - tell user we failed to read
                Data_API.LogMessage(ActionLogType.Insert, ReferenceType.None, 0, "choke", ex);
            }
        }

        /// <summary>
        /// READ - read the proxy corresponding to the ID 
        /// </summary>
        public static T Read<T>(int id) where T : class
        {
            T t = null;
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    switch(typeof(T).Name) // alphabetized list
                    {
                        case "ActionProxy":
                            {
                                Table<ActionProxy> table = connection._db.GetTable<ActionProxy>();
                                var q = from action in table where action.ActionID == id select action;
                                t = q.Single() as T;
                            }

                            //string query = $"SET NOCOUNT OFF; SELECT [ActionID], [ActionTypeID], [SystemActionTypeID], [Name], [TimeSpent], [DateStarted], [IsVisibleOnPortal], [IsKnowledgeBase], [ImportID], [DateCreated], [DateModified], [CreatorID], [ModifierID], [TicketID], [ActionSource], [DateModifiedBySalesForceSync], [SalesForceID], [DateModifiedByJiraSync], [JiraID], [Pinned], [Description], [IsClean], [ImportFileID] FROM [dbo].[Actions] WITH (NOLOCK) WHERE ([ActionID] = {id})";
                            //IEnumerable<ActionProxy> stuff = connection._db.ExecuteQuery<ActionProxy>(query);

                            //ActionProxy proxy = connection._db.ExecuteQuery<ActionProxy>(query).Min();

                            //t = new ActionNode(connection, id).ActionProxy() as T;
                            ////t = DataAPI.DataAPI.Read<T>(new ActionNode(connection, id));
                            break;
                        case "ActionProxy[]":
                            t = new TicketModel(connection, id).ActionProxies() as T;
                            //t = DataAPI.DataAPI.Read<T>(new TicketNode(connection, id));
                            break;
                        case "AttachmentProxy":
                            t = Data_API.Read<T>(new ActionAttachmentModel(connection, id));
                            break;
                        case "AttachmentProxy[]":
                            t = Data_API.Read<T>(new ActionModel(connection, id));
                            break;
                        case "CustomValueProxy":
                            t = Data_API.Read<T>(new TicketModel(connection, id));
                            break;
                        case "TicketProxy":
                            t = Data_API.Read<T>(new TicketModel(connection, id));
                            break;
                        case "UserProxy":
                            {
                                Table<UserProxy> table = connection._db.GetTable<UserProxy>();
                                var q = from user in table where user.UserID == id select user;
                                t = q.Single() as T;
                            }                            //t = DataAPI.DataAPI.Read<T>(new UserNode(connection, id));
                            break;
                        default:
                            if (Debugger.IsAttached) Debugger.Break();
                            break;
                    }
                }
            }
            catch (AuthenticationException ex)
            {
                // TODO - tell user they don't have permission
                Data_API.LogMessage(ActionLogType.Insert, ReferenceType.None, id, "choke", ex);
            }
            catch (System.Data.ConstraintException ex)
            {
                // TODO - data integrity failure
                Data_API.LogMessage(ActionLogType.Insert, ReferenceType.None, id, "choke", ex);
            }
            catch (Exception ex)
            {
                int logid = Data_API.LogException(new Proxy.AuthenticationModel(), ex, "Ticket Merge Exception:" + ex.Source);
            }
            return t;
        }

        /// <summary>
        /// UPDATE
        /// </summary>
        public static void Update<T>(T proxy)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    switch (typeof(T).Name)
                    {
                        case "ActionProxy":
                            ActionProxy actionProxy = proxy as ActionProxy;
                            //DataAPI.DataAPI.Update(new ActionNode(connection, actionProxy.ActionID), actionProxy);
                            break;
                        case "AttachmentProxy":
                            AttachmentProxy attachmentProxy = proxy as AttachmentProxy;
                            //DataAPI.DataAPI.Update(new ActionAttachment(connection, attachmentProxy.AttachmentID), attachmentProxy);
                            break;
                    }
                }
            }
            catch (AuthenticationException ex)
            {
                // TODO - tell user they don't have permission
                Data_API.LogMessage(ActionLogType.Delete, ReferenceType.None, 0, "choke", ex);
            }
            catch (System.Data.ConstraintException ex)
            {
                // TODO - data integrity failure
                Data_API.LogMessage(ActionLogType.Delete, ReferenceType.None, 0, "choke", ex);
            }
            catch (Exception ex)
            {
                // TODO - tell user we failed to read
                int logid = DataAPI.DataAPI.LogException(connection.Authentication, ex, "Ticket Merge Exception:" + ex.Source);
                return $"Error merging tickets. Exception #{logid}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.";
            }
        }

        /// <summary> 
        /// DELETE
        /// </summary>
        public static void Delete<T>(T proxy)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    switch (typeof(T).Name)
                    {
                        case "ActionProxy":
                            ActionProxy actionProxy = proxy as ActionProxy;
                            Data_API.Delete(new ActionModel(connection, actionProxy.ActionID));
                            break;
                        case "AttachmentProxy":
                            AttachmentProxy attachmentProxy = proxy as AttachmentProxy;
                            Data_API.Delete(new ActionAttachmentModel(connection, attachmentProxy.AttachmentID));
                            break;
                    }
                }
            }
            catch (AuthenticationException ex)
            {
                // TODO - tell user they don't have permission
                Data_API.LogMessage(ActionLogType.Delete, ReferenceType.None, 0, "choke", ex);
            }
            catch (System.Data.ConstraintException ex)
            {
                // TODO - data integrity failure
                Data_API.LogMessage(ActionLogType.Delete, ReferenceType.None, 0, "choke", ex);
            }
            catch (Exception ex)
            {
                int logid = DataAPI.DataAPI.LogException(new Proxy.AuthenticationModel(authenticationTicket), ex, "Ticket Merge Exception:" + ex.Source);
                return $"Error merging tickets. Exception #{logid}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.";
            }
        }


        /// <summary> ??? </summary>
        public static int AttachmentIDFromGUID(Guid guid)
        {
            using (ConnectionContext connection = new ConnectionContext())
            {
                return connection._db.ExecuteQuery<int>($"SELECT AttachmentID FROM Attachments WHERE AttachmentGUID={guid}").Min();
            }
        }

        #region Tickets
        public static string MergeTickets(int destinationTicketID, int sourceTicketID)
        {

            try
            {
                using (ConnectionContext connection = new ConnectionContext(true))    // use transaction
                {
                    try
                    {
                        TicketMerge merge = new TicketMerge(connection, connection.Ticket(destinationTicketID), connection.Ticket(sourceTicketID));
                        merge.Merge1();
                        connection.Commit();
                        return String.Empty;
                    }
                    catch (Exception ex)
                    {
                        connection.Rollback();
                        int logid = Data_API.LogException(connection.Authentication, ex, "Ticket Merge Exception:" + ex.Source);
                        return $"Error merging tickets. Exception #{logid}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.";
                    }
                }
            }
            catch (Exception ex)
            {
                int logid = Data_API.LogException(new Proxy.AuthenticationModel(), ex, "Ticket Merge Exception:" + ex.Source);
                return $"Error merging tickets. Exception #{logid}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.";
            }
        }
        #endregion


        #region ActionAttachments
        /// <summary> Create Action Attachments </summary>
        public static List<AttachmentProxy> CreateActionAttachments(int actionID, HttpContext context)
        {
            List<AttachmentProxy> results = new List<AttachmentProxy>();
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    ActionModel actionModel = new ActionModel(connection, actionID);
                    HttpFileCollection files = context.Request.Files;
                    for (int i = 0; i < files.Count; i++)   // foreach returns strings?
                    {
                        // create the file
                        if (files[i].ContentLength == 0)
                            continue;
                        AttachmentFile attachmentFile = new AttachmentFile(actionModel, files[i]);

                        // send proxy to DB
                        AttachmentProxy attachmentProxy = attachmentFile.AsAttachmentProxy(context.Request, actionModel);
                        Data_API.Create(actionModel, attachmentProxy);
                        results.Add(attachmentProxy);
                    }
                }
            }
            catch (Exception ex)
            {
                Data_API.LogMessage(ActionLogType.Insert, ReferenceType.Actions, actionID, "Unable to save attachments on action", ex);
            }
            return results;
        }

        /// <summary> Delete Action Attachment /// </summary>
        public static void DeleteActionAttachment(int attachmentID)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    // user have permission to modify this action?
                    ActionAttachmentModel attachment = new ActionAttachmentModel(connection, attachmentID);
                    if (!attachment.Action.CanEdit())
                        return;

                    AttachmentProxy proxy = Data_API.Read<AttachmentProxy>(attachment);
                    AttachmentFile file = new AttachmentFile(attachment, proxy);
                    Data_API.Delete(attachment); // remove from database
                    file.Delete();  // delete file
                }
            }
            catch (Exception ex)
            {
                Data_API.LogMessage(ActionLogType.Delete, ReferenceType.Attachments, attachmentID, "Unable to delete attachment", ex);
            }
        }

        /// <summary> Create Action Attachments </summary>
        public static void ReadActionAttachmentsForTicket(int ticketID, ActionAttachmentsByTicketID ticketActionAttachments, out AttachmentProxy[] attachments)
        {
            attachments = null;
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    TicketModel ticketModel = connection.Ticket(ticketID);
                    Data_API.ReadActionAttachmentsForTicket(ticketModel, ticketActionAttachments, out attachments);
                }
            }
            catch (Exception ex)
            {
                Data_API.LogMessage(ActionLogType.Delete, ReferenceType.Attachments, ticketID, "failed to read action attachments", ex);
            }
        }


        #endregion

    }
}
