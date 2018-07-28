using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TeamSupport.Model
{
    public static class API
    {
        /// <summary> Insert Action </summary>
        public static Data.Action InsertAction(Data.LoginUser loginUser, Data.ActionProxy actionProxy)
        {
            TeamSupport.Data.Action dataAction = null;
            try
            {
                using (TeamSupport.Model.ConnectionContext connection = new ConnectionContext(loginUser))
                {
                    TeamSupport.Model.ActionModel action = connection.Ticket(actionProxy.TicketID).InsertAction(loginUser, actionProxy);
                    dataAction = action.DataLayerAction;
                }
            }
            catch (Exception ex)
            {
                TeamSupport.Model.ConnectionContext.LogMessage(loginUser, Data.ActionLogType.Insert, Data.ReferenceType.Actions, actionProxy.TicketID, "Unable to insert action", ex);
                Data.ExceptionLogs.LogException(loginUser, ex, "NewAction", "TicketPageService.NewAction");
            }
            return dataAction;
        }

        /// <summary> Insert Action </summary>
        public static TeamSupport.Model.ActionModel InsertAction(Data.LoginUser loginUser, Data.ActionProxy proxy, Data.Ticket ticket, Data.User user)
        {
            try
            {
                using (TeamSupport.Model.ConnectionContext connection = new ConnectionContext(loginUser))
                {
                    return connection.Ticket(ticket.TicketID).InsertAction(proxy, ticket, user);
                }
            }
            catch (Exception ex)
            {
                TeamSupport.Model.ConnectionContext.LogMessage(loginUser, Data.ActionLogType.Insert, Data.ReferenceType.Actions, ticket.TicketID, "Unable to insert action", ex);
                return null;
            }
        }

        /// <summary> SaveActionAttachments </summary>
        public static List<Model.ActionAttachment> SaveActionAttachments(Data.LoginUser loginUser, HttpContext context, int? ticketID, int? actionID)
        {
            try
            {
                if (!actionID.HasValue)
                    return new List<Model.ActionAttachment>();

                using (Model.ConnectionContext connection = new ConnectionContext(loginUser))
                {
                    if (!ticketID.HasValue)
                        ticketID = Model.ActionModel.GetTicketID(connection._db, actionID.Value);

                    // add the attachments to the action
                    return connection.Ticket(ticketID.Value).Action(actionID.Value).InsertActionAttachments(context.Request);
                }
            }
            catch (Exception ex)
            {
                Model.ConnectionContext.LogMessage(loginUser, Data.ActionLogType.Insert, Data.ReferenceType.Attachments, ticketID, "Unable to save attachments", ex);
                return new List<Model.ActionAttachment>();
            }
        }

    }
}
