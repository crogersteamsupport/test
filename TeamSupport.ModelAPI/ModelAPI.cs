using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using TeamSupport.Data;
using TeamSupport.Model;
using System.Security.Authentication;
using System.Data.SqlClient;
using System.Data.Linq;

namespace TeamSupport.ModelAPI
{
    public enum EModelAPI
    {
        Create,
        Read,
        Update,
        Delete,
        ReadAttachmentsFromTicketByFilename,
        ReadAttachmentsFromTicketByKB,
        AttachmentIDFromGUID,
        MergeTickets,
        CreateActionAttachments
    }

    /// <summary>
    /// CRUD interface to DataAPI - Create, Read, Update, Delete proxy
    /// </summary>
    public static class ModelAPI
    {

        public static object Command(FormsAuthenticationTicket authenticationTicket, EModelAPI command, params object[] args)
        {
            try
            {
                object result = null;
                using (ConnectionContext connection = new ConnectionContext(authenticationTicket))
                {
                    TicketModel ticketModel;
                    Guid guid;

                    switch (command)
                    {
                        case EModelAPI.AttachmentIDFromGUID:
                            guid = (Guid)args[0];
                            result = connection._db.ExecuteQuery<int>($"SELECT AttachmentID FROM Attachments WHERE AttachmentGUID={guid}").Min();
                            break;
                        case EModelAPI.MergeTickets:
                            //MergeTickets((int)args[0], (int)args[1]);
                            break;
                        case EModelAPI.CreateActionAttachments:
                            //CreateActionAttachments((int)args[0], (HttpContext)args[1]);
                            break;
                        case EModelAPI.ReadAttachmentsFromTicketByFilename:
                            ticketModel = connection.Ticket((int)args[0]);
                            //DataAPI.DataAPI.ReadActionAttachmentsForTicket(ticketModel, ticketActionAttachments, out attachments);
                            break;
                        case EModelAPI.ReadAttachmentsFromTicketByKB:
                            ticketModel = connection.Ticket((int)args[0]);
                            //DataAPI.DataAPI.ReadActionAttachmentsForTicket(ticketModel, ticketActionAttachments, out attachments);
                            break;
                    }
                }

                //TODO - log success
                return result;
            }
            catch (AuthenticationException ex)
            {
                // TODO - tell user they don't have permission
            }
            catch(System.Data.ConstraintException ex)
            {
                // TODO - data integrity failure
                // TODO - log something
            }
            catch (Exception ex)
            {
                // TODO - tell user the request failed
                // TODO - log something
            }

            return null;
        }

        /// <summary> Read the proxy corresponding to the ID </summary>
        public static T Read<T>(FormsAuthenticationTicket authenticationTicket, int id) where T : class
        {
            T t = default(T);   // null since T is a class
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authenticationTicket))
                {
                    switch(typeof(T).Name)
                    {
                        case "ActionProxy":
                            t = DataAPI.DataAPI.Read(new ActionModel(connection, id)) as T;
                            break;
                        case "AttachmentProxy":
                            t = DataAPI.DataAPI.Read(new ActionAttachment(connection, id)) as T;
                            break;
                    }
                }
            }
            catch (AuthenticationException ex)
            {
                // TODO - tell user they don't have permission
            }
            catch (Exception ex)
            {
                // TODO - tell user we failed to read
                // TODO - log something?
            }
            return t;
        }

        /// <summary> Read the proxy children of the parent record (ticket actions, action attachments...) </summary>
        public static void Read<T>(FormsAuthenticationTicket authenticationTicket, int id, out T[] proxies) where T : class
        {
            proxies = default(T[]); // null since T is a class
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authenticationTicket))
                {
                    switch(typeof(T).Name)
                    {
                        case "ActionProxy": // actions for TicketID
                            ActionProxy[] actionProxies;
                            DataAPI.DataAPI.Read(new TicketModel(connection, id), out actionProxies);
                            proxies = actionProxies as T[];
                            break;
                        case "AttachmentProxy": // attachments for ActionID
                            AttachmentProxy[] attachmentProxies;
                            DataAPI.DataAPI.Read(new ActionModel(connection, id), out attachmentProxies);
                            proxies = attachmentProxies as T[];
                            break;
                    }
                }
            }
            catch (AuthenticationException ex)
            {
                // TODO - tell user they don't have permission
            }
            catch (Exception ex)
            {
                // TODO - tell user we failed to read
                // TODO - log something?
            }
        }

        /// <summary> Create a child </summary>
        public static void Create<T>(FormsAuthenticationTicket authentication, T proxy)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authentication))
                {
                    switch (typeof(T).Name)
                    {
                        case "ActionProxy":
                            ActionProxy actionProxy = proxy as ActionProxy;
                            DataAPI.DataAPI.Create(connection.Ticket(actionProxy.TicketID), ref actionProxy);
                            break;
                        case "AttachmentProxy":
                            AttachmentProxy attachmentProxy = proxy as AttachmentProxy;
                            ActionModel actionModel = new ActionModel(connection, attachmentProxy.RefID);
                            DataAPI.DataAPI.Create(actionModel, attachmentProxy);
                            break;
                    }
                }
            }
            catch (AuthenticationException ex)
            {
                // TODO - tell user they don't have permission
            }
            catch (Exception ex)
            {
                // TODO - tell user we failed to create
                // TODO - log something?
            }
        }


        /// <summary> ??? </summary>
        public static int AttachmentIDFromGUID(FormsAuthenticationTicket authenticationTicket, Guid guid)
        {
            using (ConnectionContext connection = new ConnectionContext(authenticationTicket))
            {
                return connection._db.ExecuteQuery<int>($"SELECT AttachmentID FROM Attachments WHERE AttachmentGUID={guid}").Min();
            }
        }

        #region Tickets
        public static string MergeTickets(FormsAuthenticationTicket authenticationTicket, int destinationTicketID, int sourceTicketID)
        {
            if (!ConnectionContext.IsEnabled) return String.Empty;
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authenticationTicket, true))    // use transaction
                {
                    try
                    {
                        TicketMerge merge = new TicketMerge(connection, connection.Ticket(destinationTicketID), connection.Ticket(sourceTicketID));
                        merge.Merge();
                        connection.Commit();
                        return String.Empty;
                    }
                    catch (Exception ex)
                    {
                        connection.Rollback();
                        DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authenticationTicket), ActionLogType.Update, ReferenceType.Attachments, destinationTicketID, $"failed to merge {destinationTicketID} <= {sourceTicketID}", ex);
                        //TODO : Work on the messages
                        return "Failed to merge";
                    }
                }
            }
            catch (Exception ex)
            {
                DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authenticationTicket), ActionLogType.Update, ReferenceType.Attachments, destinationTicketID, $"failed to merge {destinationTicketID} <= {sourceTicketID}", ex);
                //TODO : Work on the messages
                return "Failed to merge";
            }
        }
        #endregion


        #region ActionAttachments
        /// <summary> Create Action Attachments </summary>
        public static List<AttachmentProxy> CreateActionAttachments(FormsAuthenticationTicket authenticationTicket, int actionID, HttpContext context)
        {
            List<AttachmentProxy> results = new List<AttachmentProxy>();
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authenticationTicket))
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
                        DataAPI.DataAPI.Create(actionModel, attachmentProxy);
                        results.Add(attachmentProxy);
                    }
                }
            }
            catch (Exception ex)
            {
                DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authenticationTicket), ActionLogType.Insert, ReferenceType.Actions, actionID, "Unable to save attachments on action", ex);
            }
            return results;
        }

        /// <summary> Delete Action Attachment /// </summary>
        public static void DeleteActionAttachment(FormsAuthenticationTicket authenticationTicket, int attachmentID)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authenticationTicket))
                {
                    // user have permission to modify this action?
                    ActionAttachment attachment = new ActionAttachment(connection, attachmentID);
                    if (!attachment.Action.CanEdit())
                        return;

                    AttachmentProxy proxy = DataAPI.DataAPI.Read(attachment);
                    AttachmentFile file = new AttachmentFile(attachment, proxy);
                    DataAPI.DataAPI.Delete(attachment); // remove from database
                    file.Delete();  // delete file
                }
            }
            catch (Exception ex)
            {
                DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authenticationTicket), ActionLogType.Delete, ReferenceType.Attachments, attachmentID, "Unable to delete attachment", ex);
            }
        }

        /// <summary> Create Action Attachments </summary>
        public static void ReadActionAttachmentsForTicket(FormsAuthenticationTicket authenticationTicket, int ticketID, ActionAttachmentsByTicketID ticketActionAttachments, out AttachmentProxy[] attachments)
        {
            attachments = null;
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authenticationTicket))
                {
                    TicketModel ticketModel = connection.Ticket(ticketID);
                    DataAPI.DataAPI.ReadActionAttachmentsForTicket(ticketModel, ticketActionAttachments, out attachments);
                }
            }
            catch (Exception ex)
            {
                DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authenticationTicket), ActionLogType.Delete, ReferenceType.Attachments, ticketID, "failed to read action attachments", ex);
            }
        }


        #endregion

    }
}
