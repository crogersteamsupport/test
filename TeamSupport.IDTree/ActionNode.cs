using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO;
using System.Diagnostics;
using System.Web;
using TeamSupport.Data;

namespace TeamSupport.IDTree
{
    /// <summary>
    /// Wrapper for Valid ActionID
    /// </summary>
    public class ActionNode : IDNode
    {
        public TicketNode Ticket { get; private set; }
        public int ActionID { get; private set; }

        public static int GetTicketID(DataContext db, int actionID)
        {
            return db.ExecuteQuery<int>($"SELECT TicketID FROM Actions WITH (NOLOCK) WHERE ActionID = {actionID}").Min();
        }

        /// <summary> top down - existing action </summary>
        public ActionNode(TicketNode ticket, int actionID) : base(ticket)
        {
            Ticket = ticket;
            ActionID = actionID;
            Verify();
        }

        /// <summary> bottom up  - existing action </summary>
        public ActionNode(ConnectionContext connection, int actionID) : base(connection)
        {
            ActionID = actionID;
            int ticketID = GetTicketID(Connection._db, actionID);
            Ticket = new TicketNode(Connection, ticketID);
            Verify();
        }

        /// <summary> existing action attachment </summary>
        public ActionAttachmentNode ActionAttachment(int actionAttachmentID)
        {
            return new ActionAttachmentNode(this, actionAttachmentID);
        }

        public bool CanEdit() { return Connection.CanEdit() || (Connection.User.UserID == IDReader.CreatorID(Connection._db, ActionID)); }

        public const int ActionPathIndex = 3;
        public string AttachmentPath
        {
            get
            {
                string path = Ticket.Organization.AttachmentPath(ActionPathIndex);
                path = Path.Combine(path, "Actions");   // see AttachmentPath.GetFolderName(AttachmentPath.Folder.Actions);
                path = Path.Combine(path, ActionID.ToString());
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        public override void Verify()
        {
            Verify($"SELECT ActionID FROM Actions WITH (NOLOCK) WHERE ActionID={ActionID} AND TicketID={Ticket.TicketID}");
        }

        public int CreatorID()
        {
            return Connection._db.ExecuteQuery<int>($"SELECT CreatorID FROM Actions WITH (NOLOCK) WHERE ActionID={ActionID}").Min();
        }

        public int TicketID()
        {
            return Connection._db.ExecuteQuery<int>($"SELECT TicketID FROM Actions WITH (NOLOCK) WHERE ActionID = {ActionID}").Min();
        }


    }
}
