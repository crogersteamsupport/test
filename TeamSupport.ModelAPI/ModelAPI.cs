﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using TeamSupport.Data;
using TeamSupport.Model;
using System.Security.Authentication;

namespace TeamSupport.ModelAPI
{
    public static class ModelAPI
    {
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

        /// <summary> Read the children of the parent record (ticket actions, action attachments...) </summary>
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


        #region Tickets
        public static void MergeTickets(FormsAuthenticationTicket authenticationTicket, int destinationTicketID, int sourceTicketID)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authenticationTicket, true))    // use transaction
                {
                    TicketMerge merge = new TicketMerge(connection, connection.Ticket(destinationTicketID), connection.Ticket(sourceTicketID));
                    merge.Merge();
                }
            }
            catch (Exception ex)
            {
                DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authenticationTicket), ActionLogType.Update, ReferenceType.Attachments, destinationTicketID, $"failed to merge {destinationTicketID} <= {sourceTicketID}", ex);
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
        public static void ReadActionAttachmentsByFilenameAndTicket(FormsAuthenticationTicket authenticationTicket, int ticketID, out AttachmentProxy[] mostRecentByFilename)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authenticationTicket))
                {
                    TicketModel ticketModel = connection.Ticket(ticketID);
                    DataAPI.DataAPI.Read(ticketModel, out mostRecentByFilename);
                }
            }
            catch (Exception ex)
            {
                mostRecentByFilename = null;
                DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authenticationTicket), ActionLogType.Delete, ReferenceType.Attachments, ticketID, "failed to read action attachments", ex);
            }
        }


        #endregion

    }
}
