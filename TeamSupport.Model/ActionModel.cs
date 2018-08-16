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

namespace TeamSupport.Model
{
    /// <summary>
    /// Wrapper for Valid ActionID
    /// </summary>
    public class ActionModel
    {
        public TicketModel Ticket { get; private set; }
        public int ActionID { get; private set; }
        public ConnectionContext Connection { get; private set; }

        public static int GetTicketID(DataContext db, int actionID)
        {
            return db.ExecuteQuery<int>($"SELECT TicketID FROM Actions WITH (NOLOCK) WHERE ActionID = {actionID}").Min();
        }

        /// <summary> top down - existing action </summary>
        public ActionModel(TicketModel ticket, int actionID)
        {
            Ticket = ticket;
            ActionID = actionID;
            Connection = ticket.Connection;
            DBReader.VerifyAction(Connection._db, ticket.User.Organization.OrganizationID, Ticket.TicketID, ActionID);
        }

        /// <summary> bottom up  - existing action </summary>
        public ActionModel(ConnectionContext connection, int actionID)
        {
            ActionID = actionID;
            Connection = connection;
            int ticketID = GetTicketID(Connection._db, actionID);
            Ticket = new TicketModel(Connection, ticketID);
            DBReader.VerifyAction(Connection._db, Connection.Organization.OrganizationID, Ticket.TicketID, ActionID);
        }

        /// <summary> existing action attachment </summary>
        public ActionAttachment ActionAttachment(int actionAttachmentID)
        {
            return new ActionAttachment(this, actionAttachmentID);
        }

        public bool CanEdit() { return Ticket.User.CanEdit() || (Ticket.User.UserID == DBReader.CreatorID(Connection._db, ActionID)); }

        public const int ActionPathIndex = 3;
        public string AttachmentPath
        {
            get
            {
                string path = Ticket.User.Organization.AttachmentPath(ActionPathIndex);
                path = Path.Combine(path, "Actions");   // see AttachmentPath.GetFolderName(AttachmentPath.Folder.Actions);
                path = Path.Combine(path, ActionID.ToString());
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

    }
}
