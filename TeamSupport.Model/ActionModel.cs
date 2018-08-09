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
        public DataContext _db { get; private set; }

        /// <summary> existing action </summary>
        public ActionModel(TicketModel ticket, int actionID)
        {
            Ticket = ticket;
            ActionID = actionID;
            _db = ticket._db;
            DBReader.VerifyAction(_db, ticket.User.Organization.OrganizationID, Ticket.TicketID, ActionID);
        }

        /// <summary> existing action attachment </summary>
        public ActionAttachment Attachment(int actionAttachmentID)
        {
            return new ActionAttachment(this, actionAttachmentID);
        }

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

        public AttachmentFile CreateAttachmentFile(HttpPostedFile postedFile)
        {
            if (postedFile.ContentLength == 0)
                return null;

            return new AttachmentFile(AttachmentPath, postedFile);
        }

        public bool CanEdit() { return Ticket.User.CanEdit() || (Ticket.User.UserID == DBReader.CreatorID(_db, ActionID)); }
    }
}
