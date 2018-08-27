﻿using System;
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

namespace TeamSupport.IDTree
{
    /// <summary> Action Attachments </summary>
    public class ActionAttachmentNode : IDNode
    {
        public ActionNode Action { get; private set; }
        public int ActionAttachmentID { get; private set; }

        public AttachmentFile File { get; private set; }

        /// <summary> top down - Load existing action attachment /// </summary>
        public ActionAttachmentNode(ActionNode action, int actionAttachmentID) : base(action)
        {
            Action = action;
            ActionAttachmentID = actionAttachmentID;
            Verify();
        }

        /// <summary> bottom up - Load existing action attachment /// </summary>
        public ActionAttachmentNode(ConnectionContext connection, int actionAttachmentID) : base(connection)
        {
            ActionAttachmentID = actionAttachmentID;
            int actionID = GetActionID(connection._db, ActionAttachmentID);
            Action = new ActionNode(Connection, actionID);

            TicketNode ticket = Action.Ticket;
            OrganizationNode organization = ticket.Organization;
            Verify();
        }

        public override void Verify()
        {
            TicketNode ticket = Action.Ticket;
            OrganizationNode organization = ticket.Organization;
            Verify($"SELECT AttachmentID FROM Attachments WITH (NOLOCK) " +
                $"WHERE ActionAttachmentID={ActionAttachmentID} AND OrganizationID={organization.OrganizationID} AND RefID={Action.ActionID} AND RefType=0");
        }
        public static int GetActionID(DataContext db, int attachmentID)
        {
            return db.ExecuteQuery<int>($"SELECT ActionID FROM ActionAttachments WITH(NOLOCK) WHERE ActionAttachmentID = {attachmentID}").Min();
        }

    }
}

