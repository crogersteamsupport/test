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
    class ActionModel
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
            DataAPI.DataAPI.VerifyAction(_db, ticket.User.Organization.OrganizationID, Ticket.TicketID, ActionID);
        }

        /// <summary> new action on existing ticket </summary>
        public ActionModel(TicketModel ticket, ActionProxy proxy)
        {
            ActionProxy result = DataAPI.DataAPI.InsertAction(ticket.User.Authentication.OrganizationUser, proxy, ticket._db);
            Ticket = ticket;
            ActionID = result.ActionID;
            _db = ticket._db;
        }

        ///// <summary> new action on new ticket </summary>
        //public ActionModel(TicketModel ticket, ActionProxy proxy, Data.Ticket ticketData, Data.User user)
        //{
        //    ActionProxy result = DataAPI.DataAPI.InsertAction(ticketData, proxy, user);
        //    Ticket = ticket;
        //    ActionID = result.ActionID;
        //    _db = ticket._db;
        //}

        public int CreatorID() { return DataAPI.DataAPI.ActionCreatorID(_db, ActionID); }

        public ActionAttachment Attachment(int actionAttachmentID)
        {
            return new ActionAttachment(this, actionAttachmentID);
        }

        public string AttachmentPath
        {
            get
            {
                string path = Ticket.User.Organization.AttachmentPath(3);
                path = Path.Combine(path, "Actions");   // see AttachmentPath.GetFolderName(AttachmentPath.Folder.Actions);
                path = Path.Combine(path, ActionID.ToString());
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        // this is very slow...
        public ActionAttachment[] Attachments()
        {
            string query = $"SELECT AttachmentID FROM ActionAttachments WITH (NOLOCK) WHERE ActionID={ActionID} AND OrganizationID={Ticket.User.Organization.OrganizationID}";
            int[] actionAttachmentIDs = _db.ExecuteQuery<int>(query).ToArray();
            return actionAttachmentIDs.Select(id => new ActionAttachment(this, id)).ToArray();
        }

        public List<ActionAttachment> InsertActionAttachments(HttpRequest request)
        {
            List<ActionAttachment> results = new List<ActionAttachment>();
            //Data.OrganizationUser user = Ticket.User.Authentication.OrganizationUser;
            //HttpFileCollection files = request.Files;
            //for (int i = 0; i < files.Count; i++)   // foreach returns strings?
            //{
            //    HttpPostedFile postedFile = files[i];
            //    if (postedFile.ContentLength == 0)
            //        continue;

            //    results.Add(new ActionAttachment(this, user, postedFile, request));
            //}

            return results;
        }

        public bool CanEdit() { return Ticket.User.CanEdit() || (Ticket.User.UserID == CreatorID()); }

        public AttachmentProxy[] SelectAttachments() { return DataAPI.DataAPI.GetActionAttachmentProxies(_db, ActionID); }
    }
}
