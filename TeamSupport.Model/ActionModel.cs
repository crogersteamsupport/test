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
            Data.DataAPI.VerifyAction(_db, ticket.User.Organization.OrganizationID, Ticket.TicketID, ActionID);
        }

        /// <summary> new action on existing ticket </summary>
        public ActionModel(TicketModel ticket, Data.ActionProxy proxy)
        {
            Data.ActionProxy result = Data.DataAPI.InsertAction(ticket.User.Authentication.LoginUser, proxy, ticket._db);
            Ticket = ticket;
            ActionID = result.ActionID;
            _db = ticket._db;
        }

        /// <summary> new action on new ticket </summary>
        public ActionModel(TicketModel ticket, Data.ActionProxy proxy, Data.Ticket ticketData, Data.User user)
        {
            Data.ActionProxy result = Data.DataAPI.InsertAction(ticketData, proxy, user);
            Ticket = ticket;
            ActionID = result.ActionID;
            _db = ticket._db;
        }

        public int CreatorID {  get { return _db.ExecuteQuery<int>($"SELECT CreatorID FROM Actions WITH (NOLOCK) WHERE ActionID={ActionID}").Min(); } }

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

        //// this is very slow...
        //public ActionAttachment[] Attachments()
        //{
        //    string query = $"SELECT AttachmentID FROM ActionAttachments WITH (NOLOCK) WHERE ActionID={ActionID} AND OrganizationID={Ticket.User.Organization.OrganizationID}";
        //    int[] actionAttachmentIDs = _db.ExecuteQuery<int>(query).ToArray();
        //    return actionAttachmentIDs.Select(id => new ActionAttachment(this, id)).ToArray();
        //}

        public List<ActionAttachment> InsertActionAttachments(HttpRequest request)
        {
            Data.LoginUser user = Ticket.User.Authentication.LoginUser;
            List<ActionAttachment> results = new List<ActionAttachment>();
            HttpFileCollection files = request.Files;
            for (int i = 0; i < files.Count; i++)   // foreach returns strings?
            {
                HttpPostedFile postedFile = files[i];
                if (postedFile.ContentLength == 0)
                    continue;

                results.Add(new ActionAttachment(this, user, postedFile, request));
            }

            return results;
        }

        public static int GetTicketID(DataContext db, int actionID)
        {
            return db.ExecuteQuery<int>($"SELECT TicketID FROM Actions WITH (NOLOCK) WHERE ActionID = {actionID}").Min();
        }

        public bool CanEdit()
        {
            return (Ticket.User.UserID == CreatorID) || Ticket.User.CanEdit();
        }

        public Data.AttachmentProxy[] SelectAttachments()
        {
            string query = $"SELECT a.*, (u.FirstName + ' ' + u.LastName) AS CreatorName FROM Attachments a WITH (NOLOCK) LEFT JOIN Users u ON u.UserID = a.CreatorID WHERE (RefID = {ActionID}) AND (RefType = 0)";
            return _db.ExecuteQuery<Data.AttachmentProxy>(query).ToArray();
        }

    }
}
