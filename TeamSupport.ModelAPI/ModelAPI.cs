using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using TeamSupport.Data;
using TeamSupport.Model;

namespace TeamSupport.ModelAPI
{
    public static class ModelAPI
    {
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


        #region Actions
        /// <summary> Create Action </summary>
        public static void Create(FormsAuthenticationTicket authentication, ActionProxy actionProxy)
        {
            if (!ConnectionContext.IsEnabled) return;
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authentication))
                {
                    DataAPI.DataAPI.Create(connection.Ticket(actionProxy.TicketID), ref actionProxy);
                }
            }
            catch (Exception ex)
            {
                DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authentication), ActionLogType.Insert, ReferenceType.Actions, actionProxy.ActionID, "Unable to insert action", ex);
            }
        }
        #endregion


        #region ActionAttachments
        /// <summary> Create Action Attachments </summary>
        public static List<AttachmentProxy> CreateActionAttachments(FormsAuthenticationTicket authenticationTicket, int actionID, HttpContext context)
        {
            if (!ConnectionContext.IsEnabled) return null;
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
            if (!ConnectionContext.IsEnabled) return;
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
        public static void ReadActionAttachments(FormsAuthenticationTicket authenticationTicket, int actionID, out AttachmentProxy[] attachmentProxies)
        {
            if (!ConnectionContext.IsEnabled)
            {
                attachmentProxies = null;
                return;
            }
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authenticationTicket))
                {
                    ActionModel actionModel = new ActionModel(connection, actionID);
                    DataAPI.DataAPI.Read(actionModel, out attachmentProxies);
                }
            }
            catch (Exception ex)
            {
                attachmentProxies = null;
                DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authenticationTicket), ActionLogType.Delete, ReferenceType.Attachments, actionID, "failed to read action attachments", ex);
            }
        }

        ///// <summary> Create Action Attachments </summary>
        //public static void ReadActionAttachments(FormsAuthenticationTicket authenticationTicket, int ticketID, out AttachmentProxy[] attachmentProxies)
        //{
        //    try
        //    {
        //        using (ConnectionContext connection = new ConnectionContext(authenticationTicket))
        //        {
        //            TicketModel ticketModel = connection.Ticket(ticketID);
        //            DataAPI.DataAPI.Read(ticketModel, out attachmentProxies);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        attachmentProxies = null;
        //        DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authenticationTicket), ActionLogType.Delete, ReferenceType.Attachments, ticketID, "failed to read ticket action attachments", ex);
        //    }
        //}

        /// <summary> Create Action Attachments </summary>
        public static AttachmentProxy ReadActionAttachment(FormsAuthenticationTicket authenticationTicket, int attachmentID)
        {
            if (!ConnectionContext.IsEnabled) return null;
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authenticationTicket))
                {
                    ActionAttachment attachment = new ActionAttachment(connection, attachmentID);
                    return DataAPI.DataAPI.Read(attachment);
                }
            }
            catch (Exception ex)
            {
                DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authenticationTicket), ActionLogType.Delete, ReferenceType.Attachments, attachmentID, "failed to read action attachments", ex);
                return null;
            }
        }


        #endregion

    }
}
