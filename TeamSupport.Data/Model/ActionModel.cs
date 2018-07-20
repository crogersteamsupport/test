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

namespace TeamSupport.Data.Model
{
    /// <summary>
    /// Wrapper for Valid ActionID
    /// </summary>
    public class ActionModel
    {
        public TicketModel Ticket { get; private set; }
        public int ActionID { get; private set; }
        public DataContext _db { get; private set; }
        //public List<ActionAttachmentModel> Attachments { get; private set; }
        public Action HackDataAction { get; private set; }  // used by TicketPageService

        /// <summary> Root constructor </summary>
        public ActionModel(TicketModel ticket, int actionID)
        {
            Ticket = ticket;
            ActionID = actionID;
            _db = ticket._db;
            Validate();
        }

        /// <summary> New Action </summary>
        public ActionModel(TicketModel ticket, Action action) : this(ticket, action.ActionID)
        {
            HackDataAction = action;  // Keep the Action?
        }

        /// <summary> Add to existing ticket </summary>
        public ActionModel(TicketModel ticket, LoginUser loginUser, ActionProxy proxy) :
            this(ticket, AddAction(loginUser, proxy, ticket._db))
        {
        }

        /// <summary> Add to new ticket</summary>
        public ActionModel(TicketModel ticket, ActionProxy info, Ticket ticketData, User user) :
            this(ticket, AddActionOnNewTicket(ticketData, info, user))
        {
        }

        [Conditional("DEBUG")]
        void Validate()
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

        //C:\Users\sprichard\source\repos\ts-app\TeamSupport.Handlers\UploadUtils.cs
        //public static void SaveFiles(HttpContext context, AttachmentPath.Folder folder, int organizationID, int? itemID)

        //public bool IsAuthenticated(bool isKnowledgeBase)
        //{
        //    string query = $"SELECT a.ActionID FROM Actions a WITH (NOLOCK) JOIN Tickets t on a.TicketID=t.TicketID WHERE ActionID = {ActionID} AND a.IsVisibleOnPortal = 1 AND t.IsVisibleOnPortal = 1";
        //    if(isKnowledgeBase)
        //        query += " AND a.IsKnowledgeBase = 1 AND t.IsKnowledgeBase = 1";

        //    int actionID = _db.ExecuteQuery<int>(query).FirstOrDefault();   // does a record exist satisfying all the conditions?
        //    return actionID == ActionID;
        //}

        /// <summary>
        /// OLD DATA LAYER - extracted from ts-app\WebApp\App_Code\TicketPageService.cs UpdateAction(ActionProxy proxy)
        /// </summary>
        static Action AddAction(LoginUser loginUser, ActionProxy proxy, DataContext db)
        {
            Action action = (new Actions(loginUser)).AddNewAction();
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

        /// <summary>
        /// OLD DATA LAYER - extracted from ts-app\webapp\app_code\ticketservice.cs
        /// </summary>
        static Action AddActionOnNewTicket(Ticket ticket, ActionProxy info, User user)
        {
            TeamSupport.Data.Action action = (new Actions(ticket.Collection.LoginUser)).AddNewAction();
            action.ActionTypeID = null;
            action.Name = "Description";
            action.SystemActionTypeID = SystemActionType.Description;
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

        //public void LoadAttachments()
        //{
        //    try
        //    {
        //        string query = $"SELECT a.*, (u.FirstName + ' ' + u.LastName) AS CreatorName FROM Attachments a WITH (NOLOCK) LEFT JOIN Users u ON u.UserID = a.CreatorID WHERE (RefID = {ActionID}) AND(RefType = {(int)ReferenceType.Actions})";
        //        AttachmentProxy[] proxies = _db.ExecuteQuery<AttachmentProxy>(query).ToArray();
        //        Attachments = new List<ActionAttachmentModel>();
        //        foreach (AttachmentProxy proxy in proxies)
        //            Attachments.Add(new ActionAttachmentModel(this, proxy));
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debugger.Break();
        //    }
        //}

        public List<ActionAttachmentModel> InsertActionAttachments(LoginUser user, HttpRequest request)
        {
            List<ActionAttachmentModel> results = new List<ActionAttachmentModel>();
            HttpFileCollection files = request.Files;
            for (int i = 0; i < files.Count; i++)   // foreach returns strings?
            {
                HttpPostedFile postedFile = files[i];
                if (postedFile.ContentLength == 0)
                    continue;

                if (postedFile.ContentLength == 0)
                    continue;

                results.Add(new ActionAttachmentModel(this, user, postedFile, request));
            }

            return results;
        }

        public static int GetTicketID(DataContext db, int actionID)
        {
            return db.ExecuteQuery<int>($"SELECT TicketID FROM Actions WHERE ActionID = {actionID}").First();
        }

    }
}
