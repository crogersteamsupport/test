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

namespace TeamSupport.IDTree
{
    // interface to model class that supports attachments 
    public interface IAttachmentParent
    {
        string AttachmentPath { get; }
        IDNode AsIDNode { get; }    // back door to map class to IDNode at compile time
    }

    public abstract class AttachmentModel : IDNode
    {
        public AttachmentModel(IDNode node) : this(node.Connection) { }
        public AttachmentModel(ConnectionContext connection) : base(connection)
        {
        }

        public int ActionAttachmentID { get; protected set; }
        public AttachmentFile File { get; protected set; }
    }

    /// <summary> Action Attachments </summary>
    public class ActionAttachmentModel : AttachmentModel
    {
        public ActionModel Action { get; private set; }
        public string AttachmentPath { get { return Action.AttachmentPath; } }


        /// <summary> top down - Load existing action attachment /// </summary>
        public ActionAttachmentModel(ActionModel action, int actionAttachmentID) : base(action)
        {
            Action = action;
            ActionAttachmentID = actionAttachmentID;
            Verify();
        }

        /// <summary> bottom up - Load existing action attachment /// </summary>
        public ActionAttachmentModel(ConnectionContext connection, int actionAttachmentID) : base(connection)
        {
            ActionAttachmentID = actionAttachmentID;
            int actionID = GetActionID(connection._db, ActionAttachmentID);
            Action = new ActionModel(Connection, actionID);
            Verify();
        }

        public override void Verify()
        {
            TicketModel ticket = Action.Ticket;
            OrganizationModel organization = ticket.Organization;
            Verify($"SELECT AttachmentID FROM Attachments WITH (NOLOCK) " +
                $"WHERE AttachmentID={ActionAttachmentID} AND OrganizationID={organization.OrganizationID} AND RefID={Action.ActionID} AND RefType=0");
        }
        public static int GetActionID(DataContext db, int attachmentID)
        {
            return db.ExecuteQuery<int>($"SELECT RefID FROM Attachments WITH(NOLOCK) WHERE AttachmentID = {attachmentID}").Min();
        }

    }
}

