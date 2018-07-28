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
        public Data.Action DataLayerAction { get; private set; }  // back door used by TicketPageService

        /// <summary> Root constructor </summary>
        public ActionModel(TicketModel ticket, int actionID)
        {
            Ticket = ticket;
            ActionID = actionID;
            _db = ticket._db;
            Verify();
        }

        /// <summary> load action </summary>
        public ActionModel(TicketModel ticket, Data.Action action) : this(ticket, action.ActionID)
        {
            DataLayerAction = action;  // Keep the Data.Action?
        }

        /// <summary> new action on existing ticket </summary>
        public ActionModel(TicketModel ticket, Data.LoginUser loginUser, Data.ActionProxy proxy) :
            this(ticket, AddAction(loginUser, proxy, ticket._db))
        {
        }

        /// <summary> new action on new ticket </summary>
        public ActionModel(TicketModel ticket, Data.ActionProxy info, Data.Ticket ticketData, Data.User user) :
            this(ticket, AddActionOnNewTicket(ticketData, info, user))
        {
        }

        [Conditional("DEBUG")]
        void Verify()
        {
            string query = $"SELECT ActionID FROM Actions WITH (NOLOCK) WHERE ActionID={ActionID} AND TicketID={Ticket.TicketID}";
            IEnumerable<int> x = _db.ExecuteQuery<int>(query);
            if (!x.Any())
                throw new Exception(String.Format($"{query} not found"));
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

        /// <summary> extracted from ts-app\WebApp\App_Code\TicketPageService.cs UpdateAction(ActionProxy proxy) </summary>
        static Data.Action AddAction(Data.LoginUser loginUser, Data.ActionProxy proxy, DataContext db)
        {
            Data.Action action = (new Data.Actions(loginUser)).AddNewAction();
            action.TicketID = proxy.TicketID;
            action.CreatorID = loginUser.UserID;
            action.Description = proxy.Description;

            // add signature?
            string signature = db.ExecuteQuery<string>($"SELECT [Signature] FROM Users  WITH (NOLOCK) WHERE UserID={action.CreatorID}").FirstOrDefault();    // 175915
            if (!string.IsNullOrWhiteSpace(signature) && proxy.IsVisibleOnPortal && !proxy.IsKnowledgeBase && proxy.ActionID == -1 &&
                (!proxy.Description.Contains(signature)))
            {
                action.Description += "<br/><br/>" + signature;
            }

            action.ActionTypeID = proxy.ActionTypeID;
            action.DateStarted = proxy.DateStarted;
            action.TimeSpent = proxy.TimeSpent;
            action.IsKnowledgeBase = proxy.IsKnowledgeBase;
            action.IsVisibleOnPortal = proxy.IsVisibleOnPortal;
            action.Collection.Save();
            return action;
        }

        /// <summary> extracted from ts-app\webapp\app_code\ticketservice.cs </summary>
        static Data.Action AddActionOnNewTicket(Data.Ticket ticket, Data.ActionProxy info, Data.User user)
        {
            TeamSupport.Data.Action action = (new Data.Actions(ticket.Collection.LoginUser)).AddNewAction();
            action.ActionTypeID = null;
            action.Name = "Description";
            action.SystemActionTypeID = Data.SystemActionType.Description;
            action.ActionSource = ticket.TicketSource;
            action.Description = info.Description;


            if (!string.IsNullOrWhiteSpace(user.Signature) && info.IsVisibleOnPortal)
            {
                action.Description = action.Description + "<br/><br/>" + user.Signature;
            }

            action.IsVisibleOnPortal = ticket.IsVisibleOnPortal;
            action.IsKnowledgeBase = ticket.IsKnowledgeBase;
            action.TicketID = ticket.TicketID;
            action.TimeSpent = info.TimeSpent;
            action.DateStarted = info.DateStarted;
            action.Collection.Save();
            return action;
        }

        // this is very slow...
        public ActionAttachment[] Attachments()
        {
            string query = $"SELECT AttachmentID FROM ActionAttachments WHERE OrganizationID={Ticket.User.Organization.OrganizationID} AND ActionID={ActionID}";
            int[] actionAttachmentIDs = _db.ExecuteQuery<int>(query).ToArray();
            return actionAttachmentIDs.Select(id => new ActionAttachment(this, id)).ToArray();
        }

        public List<ActionAttachment> InsertActionAttachments(HttpRequest request)
        {
            Data.LoginUser user = Ticket.User.Organization.ConnectionModel._loginUser;
            List<ActionAttachment> results = new List<ActionAttachment>();
            HttpFileCollection files = request.Files;
            for (int i = 0; i < files.Count; i++)   // foreach returns strings?
            {
                HttpPostedFile postedFile = files[i];
                if (postedFile.ContentLength == 0)
                    continue;

                if (postedFile.ContentLength == 0)
                    continue;

                results.Add(new ActionAttachment(this, user, postedFile, request));
            }

            return results;
        }

        public static int GetTicketID(DataContext db, int actionID)
        {
            return db.ExecuteQuery<int>($"SELECT TicketID FROM Actions WHERE ActionID = {actionID}").First();
        }

    }
}
