using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace TeamSupport.Data.Model
{
    public class ActionModel
    {
        public TicketModel Ticket { get; private set; }
        public int ActionID { get; private set; }
        public DataContext _db { get; private set; }

        public ActionModel(TicketModel ticket, int actionID)
        {
            Ticket = ticket;
            ActionID = actionID;
            _db = ticket._db;

            // Actions do not have OrganizationID so we assume
            string query = $"SELECT ActionID from Actions WHERE ActionID={ActionID} AND TicketID={Ticket.TicketID}";
            IEnumerable<int> x = _db.ExecuteQuery<int>(query);
            if (!x.Any())
                throw new Exception(String.Format($"{query} not found"));
        }

        /// <summary>Add action to existing ticket</summary>
        public ActionModel(TicketModel ticket, LoginUser loginUser, ActionProxy proxy) : 
            this(ticket, AddAction(loginUser, proxy, ticket._db))
        {
        }

        /// <summary>Create new action on new ticket</summary>
        public ActionModel(TicketModel ticket, ActionProxy info, Ticket ticketData, User user) :
            this(ticket, AddActionOnNewTicket(ticketData, info, user))
        {
        }

        /// <summary>
        /// OLD DATA LAYER - extracted from ts-app\WebApp\App_Code\TicketPageService.cs UpdateAction(ActionProxy proxy)
        /// </summary>
        static int AddAction(LoginUser loginUser, ActionProxy proxy, DataContext db)
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
            return action.ActionID;
        }

        /// <summary>
        /// OLD DATA LAYER - extracted from ts-app\webapp\app_code\ticketservice.cs
        /// </summary>
        static int AddActionOnNewTicket(Ticket ticket, ActionProxy info, User user)
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
            return action.ActionID;
        }

        //public AttachmentModel[] Attachments()
        //{
        //    //return _db.ExecuteQuery<AttachmentProxy>($"SELECT a.*, (u.FirstName + ' ' + u.LastName) AS CreatorName FROM Attachments a LEFT JOIN Users u ON u.UserID = a.CreatorID WHERE(RefID = @RefID) AND(RefType = 0)").ToArray();
        //    //Attachments attachments = new Attachments(loginUser);
        //    //attachments.LoadByActionID(ActionID);
        //    //item.Attachments = GetActionAttachments(action.ActionID, loginUser);
        //}

        public AttachmentProxy[] GetActionAttachments()
        {
            AttachmentProxy[] attachments = null;
            try
            {
                const ReferenceType actionType = ReferenceType.Actions;
                string query = $"SELECT a.*, (u.FirstName + ' ' + u.LastName) AS CreatorName FROM Attachments a WITH (NOLOCK) LEFT JOIN Users u ON u.UserID = a.CreatorID WHERE (RefID = {ActionID}) AND(RefType = {(int)actionType})";
                attachments = _db.ExecuteQuery<AttachmentProxy>(query).ToArray();
                return attachments;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debugger.Break();
            }
            return null;
        }
    }
}
