﻿using System;
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

namespace TeamSupport.ModelAPI
{
    /// <summary> CRUD interface to DataAPI - Create, Read, Update, Delete proxy </summary>
    public static class ModelAPI
    {
        /// <summary> 
        /// CREATE
        /// </summary>
        public static void Create<T>(T proxy)
        {
            try
            {
                using (ClientRequest connection = new ClientRequest())
                {
                    switch (typeof(T).Name)
                    {
                        case "ActionProxy":
                            ActionProxy actionProxy = proxy as ActionProxy;
                            DataAPI.DataAPI.Create(connection.Ticket(actionProxy.TicketID), actionProxy);
                            break;
                        case "AttachmentProxy":
                            AttachmentProxy attachmentProxy = proxy as AttachmentProxy;
                            ActionNode actionModel = new ActionNode(connection, ((ActionAttachmentProxy)attachmentProxy).ActionID);
                            DataAPI.DataAPI.Create(actionModel, attachmentProxy);
                            break;
                    }
                }
            }
            catch (AuthenticationException ex)
            {
                // TODO - tell user they don't have permission
                DataAPI.DataAPI.LogMessage(ActionLogType.Insert, ReferenceType.None, 0, "choke", ex);
            }
            catch (System.Data.ConstraintException ex)
            {
                // TODO - data integrity failure
                DataAPI.DataAPI.LogMessage(ActionLogType.Insert, ReferenceType.None, 0, "choke", ex);
            }
            catch (Exception ex)
            {
                // TODO - tell user we failed to read
                DataAPI.DataAPI.LogMessage(ActionLogType.Insert, ReferenceType.None, 0, "choke", ex);
            }
        }

        /// <summary>
        /// READ - read the proxy corresponding to the ID 
        /// </summary>
        public static T Read<T>(int id) where T : class
        {
            T t = default(T);   // null since T is a class
            try
            {
                using (ClientRequest connection = new ClientRequest())
                {
                    switch(typeof(T).Name) // alphabetized list
                    {
                        case "ActionProxy": // action
                            t = DataAPI.DataAPI.Read<T>(new ActionNode(connection, id));
                            break;
                        case "ActionProxy[]": // ticket actions
                            t = DataAPI.DataAPI.Read<T>(new TicketNode(connection, id));
                            break;
                        case "AttachmentProxy": // attachment
                            t = DataAPI.DataAPI.Read<T>(new ActionAttachmentNode(connection, id));
                            break;
                        case "AttachmentProxy[]": // action attachments
                            t = DataAPI.DataAPI.Read<T>(new ActionNode(connection, id));
                            break;
                        case "TicketProxy": // ticket
                            t = DataAPI.DataAPI.Read<T>(new TicketNode(connection, id));
                            break;
                        default:
                            throw new Exception("bad call to ModelAPI.Read");
                    }
                }
            }
            catch (AuthenticationException ex)
            {
                // TODO - tell user they don't have permission
                DataAPI.DataAPI.LogMessage(ActionLogType.Insert, ReferenceType.None, id, "choke", ex);
            }
            catch (System.Data.ConstraintException ex)
            {
                // TODO - data integrity failure
                DataAPI.DataAPI.LogMessage(ActionLogType.Insert, ReferenceType.None, id, "choke", ex);
            }
            catch (Exception ex)
            {
                // TODO - tell user we failed to read
                DataAPI.DataAPI.LogMessage(ActionLogType.Insert, ReferenceType.None, id, "choke", ex);
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
                using (ClientRequest connection = new ClientRequest())
                {
                    switch (typeof(T).Name)
                    {
                        case "ActionProxy":
                            ActionProxy actionProxy = proxy as ActionProxy;
                            DataAPI.DataAPI.Update(new ActionNode(connection, actionProxy.ActionID), actionProxy);
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
                DataAPI.DataAPI.LogMessage(ActionLogType.Delete, ReferenceType.None, 0, "choke", ex);
            }
            catch (System.Data.ConstraintException ex)
            {
                // TODO - data integrity failure
                DataAPI.DataAPI.LogMessage(ActionLogType.Delete, ReferenceType.None, 0, "choke", ex);
            }
            catch (Exception ex)
            {
                // TODO - tell user we failed to read
                DataAPI.DataAPI.LogMessage(ActionLogType.Delete, ReferenceType.None, 0, "choke", ex);
            }
        }

        /// <summary> 
        /// DELETE
        /// </summary>
        public static void Delete<T>(T proxy)
        {
            try
            {
                using (ClientRequest connection = new ClientRequest())
                {
                    switch (typeof(T).Name)
                    {
                        case "ActionProxy":
                            ActionProxy actionProxy = proxy as ActionProxy;
                            DataAPI.DataAPI.Delete(new ActionNode(connection, actionProxy.ActionID));
                            break;
                        case "AttachmentProxy":
                            AttachmentProxy attachmentProxy = proxy as AttachmentProxy;
                            DataAPI.DataAPI.Delete(new ActionAttachmentNode(connection, attachmentProxy.AttachmentID));
                            break;
                    }
                }
            }
            catch (AuthenticationException ex)
            {
                // TODO - tell user they don't have permission
                DataAPI.DataAPI.LogMessage(ActionLogType.Delete, ReferenceType.None, 0, "choke", ex);
            }
            catch (System.Data.ConstraintException ex)
            {
                // TODO - data integrity failure
                DataAPI.DataAPI.LogMessage(ActionLogType.Delete, ReferenceType.None, 0, "choke", ex);
            }
            catch (Exception ex)
            {
                // TODO - tell user we failed to read
                DataAPI.DataAPI.LogMessage(ActionLogType.Delete, ReferenceType.None, 0, "choke", ex);
            }
        }


        /// <summary> ??? </summary>
        public static int AttachmentIDFromGUID(Guid guid)
        {
            using (ClientRequest connection = new ClientRequest())
            {
                return connection._db.ExecuteQuery<int>($"SELECT AttachmentID FROM Attachments WHERE AttachmentGUID={guid}").Min();
            }
        }

        #region Tickets
        public static string MergeTickets(int destinationTicketID, int sourceTicketID)
        {
            try
            {
                using (ClientRequest connection = new ClientRequest(true))    // use transaction
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
                        DataAPI.DataAPI.LogMessage(ActionLogType.Update, ReferenceType.Attachments, destinationTicketID, $"failed to merge {destinationTicketID} <= {sourceTicketID}", ex);
                        //TODO : Work on the messages
                        return "Failed to merge";
                    }
                }
            }
            catch (Exception ex)
            {
                DataAPI.DataAPI.LogMessage(ActionLogType.Update, ReferenceType.Attachments, destinationTicketID, $"failed to merge {destinationTicketID} <= {sourceTicketID}", ex);
                //TODO : Work on the messages
                return "Failed to merge";
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
                using (ClientRequest connection = new ClientRequest())
                {
                    ActionNode actionModel = new ActionNode(connection, actionID);
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
                DataAPI.DataAPI.LogMessage(ActionLogType.Insert, ReferenceType.Actions, actionID, "Unable to save attachments on action", ex);
            }
            return results;
        }

        /// <summary> Delete Action Attachment /// </summary>
        public static void DeleteActionAttachment(int attachmentID)
        {
            try
            {
                using (ClientRequest connection = new ClientRequest())
                {
                    // user have permission to modify this action?
                    ActionAttachmentNode attachment = new ActionAttachmentNode(connection, attachmentID);
                    if (!attachment.Action.CanEdit())
                        return;

                    AttachmentProxy proxy = DataAPI.DataAPI.Read<AttachmentProxy>(attachment);
                    AttachmentFile file = new AttachmentFile(attachment, proxy);
                    DataAPI.DataAPI.Delete(attachment); // remove from database
                    file.Delete();  // delete file
                }
            }
            catch (Exception ex)
            {
                DataAPI.DataAPI.LogMessage(ActionLogType.Delete, ReferenceType.Attachments, attachmentID, "Unable to delete attachment", ex);
            }
        }

        /// <summary> Create Action Attachments </summary>
        public static void ReadActionAttachmentsForTicket(int ticketID, ActionAttachmentsByTicketID ticketActionAttachments, out AttachmentProxy[] attachments)
        {
            attachments = null;
            try
            {
                using (ClientRequest connection = new ClientRequest())
                {
                    TicketNode ticketModel = connection.Ticket(ticketID);
                    DataAPI.DataAPI.ReadActionAttachmentsForTicket(ticketModel, ticketActionAttachments, out attachments);
                }
            }
            catch (Exception ex)
            {
                DataAPI.DataAPI.LogMessage(ActionLogType.Delete, ReferenceType.Attachments, ticketID, "failed to read action attachments", ex);
            }
        }


        #endregion

    }
}
