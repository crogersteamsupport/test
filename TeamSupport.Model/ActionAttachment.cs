using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.IO;
using System.Web;
using System.Diagnostics;
using TeamSupport.Proxy;
using TeamSupport.Data;
using System.Web.Security;

namespace TeamSupport.Model
{
    public class ModelBase { }

    /// <summary> Action Attachments </summary>
    public class ActionAttachment : ModelBase
    {
        public ActionModel Action { get; private set; }
        public int ActionAttachmentID { get; private set; }
        public ConnectionContext Connection { get; private set; }

        public static int GetActionID(DataContext db, int attachmentID)
        {
            return db.ExecuteQuery<int>($"SELECT ActionID FROM ActionAttachments WITH(NOLOCK) WHERE ActionAttachmentID = {attachmentID}").Min();
        }

        /// <summary> top down - Load existing action attachment /// </summary>
        public ActionAttachment(ActionModel action, int actionAttachmentID)
        {
            Action = action;
            ActionAttachmentID = actionAttachmentID;
            Connection = action.Connection;

            TicketModel ticket = Action.Ticket;
            OrganizationModel organization = ticket.User.Organization;
            DBReader.VerifyActionAttachment(Connection._db, organization.OrganizationID, ticket.TicketID, Action.ActionID, ActionAttachmentID);
        }

        /// <summary> bottom up - Load existing action attachment /// </summary>
        public ActionAttachment(ConnectionContext connection, int actionAttachmentID)
        {
            Connection = connection;
            ActionAttachmentID = actionAttachmentID;
            int actionID = GetActionID(connection._db, ActionAttachmentID);
            Action = new ActionModel(Connection, actionID);

            TicketModel ticket = Action.Ticket;
            OrganizationModel organization = ticket.User.Organization;
            DBReader.VerifyActionAttachment(Connection._db, organization.OrganizationID, ticket.TicketID, Action.ActionID, ActionAttachmentID);
        }

    }
}

