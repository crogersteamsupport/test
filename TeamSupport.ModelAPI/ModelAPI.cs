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
    /// <summary>
    /// The logical model (TeamSupport.Model) assumes correctness and throws exceptions when it is not.
    /// </summary>
    public static class ModelAPI
    {
        // Actions --------------------------
        public static void Create(FormsAuthenticationTicket authentication, ActionProxy actionProxy)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authentication))
                {
                    //if (!CanEditAction(action)) return;
                    DataAPI.DataAPI.Create(connection, connection.Ticket(actionProxy.TicketID), ref actionProxy);
                }
            }
            catch (Exception ex)
            {
                DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authentication), ActionLogType.Insert, ReferenceType.Actions, actionProxy.ActionID, "Unable to insert action", ex);
            }
        }


        /// <summary> Save Action Attachments - Save the uploaded files and insert an action attachment record </summary>
        public static void CreateActionAttachments(FormsAuthenticationTicket authenticationTicket, HttpContext context, int? ticketID, int? actionID)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authenticationTicket))
                {
                    if (!ticketID.HasValue)
                        ticketID = DataAPI.DataAPI.ActionGetTicketID(connection._db, actionID.Value);
                    ActionModel action = connection.Ticket(ticketID.Value).Action(actionID.Value);
                    HttpFileCollection files = context.Request.Files;
                    for (int i = 0; i < files.Count; i++)   // foreach returns strings?
                    {
                        AttachmentFile attachmentFile = action.SaveAttachmentFile(files[i]);
                        if (attachmentFile == null)
                            continue;
                        AttachmentProxy attachmentProxy = attachmentFile.Get(context.Request, connection.Authentication, action.ActionID);
                        DataAPI.DataAPI.Create(connection, action, attachmentProxy);
                    }
                }
            }
            catch (Exception ex)
            {
                DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authenticationTicket), ActionLogType.Insert, ReferenceType.Attachments, ticketID, "Unable to save attachments", ex);
            }
        }

        /*
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
                    connection.Ticket(ticketID.Value).Action(actionID.Value).Attachment(attachmentID).Delete();
                }
            }
            catch (Exception ex)
            {
                DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authenticationTicket), Data.ActionLogType.Delete, Data.ReferenceType.Attachments, attachmentID, "Unable to delete attachments", ex);
            }
        }

        public static AttachmentProxy[] ReadActionAttachments(FormsAuthenticationTicket authentication, int? ticketID, int actionID)
        {
            //try
            //{
            //    using (ConnectionContext connection = new ConnectionContext(authentication))
            //    {
            //        if(!ticketID.HasValue)
            //            ticketID = DataAPI.DataAPI.ActionGetTicketID(connection._db, actionID);
            //        return connection.Ticket(ticketID.Value).Action(actionID).SelectAttachments();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    DataAPI.DataAPI.LogMessage(authentication, Data.ActionLogType.Delete, Data.ReferenceType.Attachments, actionID, "failed to read attachments", ex);
            //    return null;
            //}
            return null;
        }

        public static void MergeTickets(FormsAuthenticationTicket authenticationTicket, int destinationTicketID, int sourceTicketID)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext(authenticationTicket))
                {
                    connection.Ticket(destinationTicketID).Merge(connection.Ticket(sourceTicketID));
                }
            }
            catch (Exception ex)
            {
                DataAPI.DataAPI.LogMessage(new Proxy.AuthenticationModel(authenticationTicket), Data.ActionLogType.Update, Data.ReferenceType.Attachments, destinationTicketID, $"failed to merge {destinationTicketID} <= {sourceTicketID}", ex);
            }
        }
*/
    }
}
