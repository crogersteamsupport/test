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


        #region Actions
        /// <summary> Create Action </summary>
        public static void Create(FormsAuthenticationTicket authentication, ActionProxy actionProxy)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authentication))
                {
                    DataAPI.DataAPI.Create(connection, connection.Ticket(actionProxy.TicketID), ref actionProxy);
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
        public static List<AttachmentProxy> CreateActionAttachments(FormsAuthenticationTicket authenticationTicket, int? ticketID, int? actionID, HttpContext context)
        {
            List<AttachmentProxy> results = new List<AttachmentProxy>();
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authenticationTicket))
                {
                    if (!ticketID.HasValue)
                        ticketID = DataAPI.DataAPI.ActionGetTicketID(connection._db, actionID.Value);
                    ActionModel actionModel = connection.Ticket(ticketID.Value).Action(actionID.Value);
                    HttpFileCollection files = context.Request.Files;
                    for (int i = 0; i < files.Count; i++)   // foreach returns strings?
                    {
                        AttachmentFile attachmentFile = actionModel.CreateAttachmentFile(files[i]);
                        if (attachmentFile == null)
                            continue;
                        AttachmentProxy attachmentProxy = attachmentFile.AsAttachmentProxy(context.Request, actionModel);
                        DataAPI.DataAPI.Create(connection, actionModel, attachmentProxy);
                        results.Add(attachmentProxy);
                    }
                }
            }
            catch (Exception ex)
            {
                DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authenticationTicket), ActionLogType.Insert, ReferenceType.Attachments, ticketID, "Unable to save attachments", ex);
            }
            return results;
        }

        /// <summary> Delete Action Attachment /// </summary>
        public static void DeleteActionAttachment(FormsAuthenticationTicket authenticationTicket, int? ticketID, int? actionID, int attachmentID)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authenticationTicket))
                {
                    if (!actionID.HasValue)
                        actionID = DataAPI.DataAPI.ActionAttachmentActionID(connection._db, attachmentID);
                    if(!ticketID.HasValue)
                        ticketID = DataAPI.DataAPI.ActionGetTicketID(connection._db, actionID.Value);

                    // user have permission to modify this action?
                    ActionModel actionModel = connection.Ticket(ticketID.Value).Action(actionID.Value);
                    if (!actionModel.CanEdit())
                        return;

                    ActionAttachment attachment = actionModel.Attachment(attachmentID);
                    AttachmentProxy proxy = DataAPI.DataAPI.Read(connection, attachment);
                    AttachmentFile file = new AttachmentFile(attachment, proxy);
                    DataAPI.DataAPI.Delete(connection, attachment); // remove from database
                    file.Delete();  // delete file
                }
            }
            catch (Exception ex)
            {
                DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authenticationTicket), ActionLogType.Delete, ReferenceType.Attachments, attachmentID, "Unable to delete attachment", ex);
            }
        }

        /// <summary> Create Action Attachments </summary>
        public static void ReadActionAttachments(FormsAuthenticationTicket authenticationTicket, int? ticketID, int actionID, out AttachmentProxy[] attachmentProxies)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authenticationTicket))
                {
                    if (!ticketID.HasValue)
                        ticketID = DataAPI.DataAPI.ActionGetTicketID(connection._db, actionID);
                    ActionModel actionModel = connection.Ticket(ticketID.Value).Action(actionID);
                    DataAPI.DataAPI.Read(connection, actionModel, out attachmentProxies);
                }
            }
            catch (Exception ex)
            {
                attachmentProxies = null;
                DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authenticationTicket), ActionLogType.Delete, ReferenceType.Attachments, actionID, "failed to read action attachments", ex);
            }
        }

        /// <summary> Create Action Attachments </summary>
        public static void ReadActionAttachments(FormsAuthenticationTicket authenticationTicket, int ticketID, out AttachmentProxy[] attachmentProxies)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authenticationTicket))
                {
                    TicketModel ticketModel = connection.Ticket(ticketID);
                    DataAPI.DataAPI.Read(connection, ticketModel, out attachmentProxies);
                }
            }
            catch (Exception ex)
            {
                attachmentProxies = null;
                DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authenticationTicket), ActionLogType.Delete, ReferenceType.Attachments, ticketID, "failed to read ticket action attachments", ex);
            }
        }

        #endregion

    }
}
