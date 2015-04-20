using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
namespace TeamSupport.Data
{
    public partial class Action
    {
        public Attachments GetAttachments()
        {
            Attachments attachments = new Attachments(BaseCollection.LoginUser);
            attachments.LoadByActionID(ActionID);
            return attachments;
        }
        public ActionsViewItem GetActionView()
        {
            return ActionsView.GetActionsViewItem(BaseCollection.LoginUser, ActionID);
        }
        public string ActionTypeName
        {
            get
            {
                if (Row.Table.Columns.Contains("ActionTypeName") && Row["ActionTypeName"] != DBNull.Value)
                {
                    return (string)Row["ActionTypeName"];
                }
                else return "";
            }

        }
        public string DisplayName
        {
            get
            {
                string title = "";

                switch (SystemActionTypeID)
                {
                    case SystemActionType.Description: title = "Description"; break;
                    case SystemActionType.Resolution: title = "Resolution"; break;
                    case SystemActionType.Email: title = "Email: " + Name; break;
                    case SystemActionType.UpdateRequest: title = "Update Request"; break;
                    case SystemActionType.Chat: title = "Chat"; break;
                    case SystemActionType.Reminder: title = "Reminder"; break;
                    default:
                        title = ActionTypeName == "" ? "[No Action Type]" : ActionTypeName;
                        if (!string.IsNullOrEmpty(Name)) title += ": " + Name;
                        break;
                }
                return title;
            }

        }
    }
    public partial class Actions
    {
        private bool _updateChildTickets = true;
        private string _actionLogInstantMessage = null;
        public string ActionLogInstantMessage
        {
            get
            {
                return _actionLogInstantMessage;
            }
            set
            {
                _actionLogInstantMessage = value;
            }
        }
        partial void BeforeRowDelete(int actionID)
        {
            Action action = (Action)Actions.GetAction(LoginUser, actionID);
            string description = "Deleted action '" + action.Name + "' from " + Tickets.GetTicketLink(LoginUser, action.TicketID);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Tickets, action.TicketID, description);
        }
        partial void BeforeRowEdit(Action action)
        {
            action.Description = HtmlUtility.FixScreenRFrame((action.Row["Description"] == DBNull.Value) ? string.Empty : action.Description);
            string actionNumber = GetActionNumber(action.TicketID, action.ActionID);
            string description = "Modified action #" + actionNumber + " on " + Tickets.GetTicketLink(LoginUser, action.TicketID);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, action.TicketID, description);
        }
        private string GetActionNumber(int ticketID, int actionID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM Actions WHERE TicketID = @TicketID AND ActionID <= @ActionID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                command.Parameters.AddWithValue("@ActionID", actionID);
                return ExecuteScalar(command, "Tickets").ToString();
            }
        }
        partial void BeforeRowInsert(Action action)
        {
            action.Description = HtmlUtility.FixScreenRFrame((action.Row["Description"] == DBNull.Value) ? string.Empty : action.Description);
        }
        partial void AfterRowInsert(Action action)
        {
            string description = "Added action ";
            if (_actionLogInstantMessage != null)
            {
                description = _actionLogInstantMessage;
            }
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, action.TicketID, description + action.Name + " to " + Tickets.GetTicketLink(LoginUser, action.TicketID));
            if (_updateChildTickets) AddChildActions(action);
        }
        public void AddChildActions(Action action)
        {
            Tickets tickets = new Tickets(LoginUser);
            tickets.LoadChildren(action.TicketID);
            Actions actions = new Actions(LoginUser);
            actions._updateChildTickets = false;
            foreach (Ticket ticket in tickets)
            {
                Action newAction = actions.AddNewAction();
                newAction.ActionTypeID = action.ActionTypeID;
                newAction.SystemActionTypeID = action.SystemActionTypeID;
                newAction.Name = action.Name;
                newAction.ActionSource = action.ActionSource;
                newAction.Description = action.Description;// +"<p>This action was added by the parent ticket.</p>";
                newAction.TimeSpent = action.TimeSpent;
                newAction.Row["DateStarted"] = action.Row["DateStarted"];
                newAction.IsVisibleOnPortal = action.IsVisibleOnPortal;
                newAction.IsKnowledgeBase = action.IsKnowledgeBase;
                newAction.ImportID = action.ImportID;
                newAction.TicketID = ticket.TicketID;
            }
            actions.Save();
        }
        /// <summary>
        /// Loads actions for a ticket and orders them by the latest date created first
        /// </summary>
        /// <param name="ticketID"></param>
        public void LoadByTicketID(int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT a.*, u.FirstName + ' ' + u.LastName AS UserName, at.Name AS ActionTypeName FROM Actions a LEFT JOIN Users u ON a.CreatorID = u.UserID LEFT JOIN ActionTypes at ON a.ActionTypeID = at.ActionTypeID WHERE a.TicketID = @TicketID ORDER BY a.DateCreated DESC";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command);
            }
        }
        /// <summary>
        /// Load a range of Actions Ordered by CreateDate by TicketID, From, and To
        /// </summary>
        /// <param name="ticketID"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void LoadByRange(int ticketNumber, int orgID, int userID, int from, int to)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                                    WITH BaseQuery AS(
                                        select *
                                      from [ActionTimeLineTest]
                                      WHERE ticketnumber = @TicketNumber and organizationid = @OrganizationID and (wcuserid IS NULL or  wcuserid = @UserID)
                                    ),
                                    RowQuery AS (
                                        SELECT BaseQuery.*, ROW_NUMBER() OVER (ORDER BY DateCreated DESC) AS 'RowNum' FROM BaseQuery
                                    ),
                                    PageQuery AS (
                                        SELECT  * FROM RowQuery WHERE RowNum BETWEEN @From AND @To
                                    )
                                    SELECT PageQuery.RowNum, *
                                    FROM PageQuery
                                    ORDER BY PageQuery.RowNum
                                    ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketNumber", ticketNumber);
                command.Parameters.AddWithValue("@OrganizationID", orgID);
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@From", from);
                command.Parameters.AddWithValue("@To", to);
                Fill(command);
            }
        }
        public void LoadLatestByTicket(int ticketID, bool onlyPublic)
        {
            using (SqlCommand command = new SqlCommand())
            {
                if (onlyPublic)
                    command.CommandText = "SELECT TOP 1 a.* FROM Actions a WHERE a.TicketID = @TicketID AND IsVisibleOnPortal = 1 ORDER BY a.DateCreated DESC";
                else
                    command.CommandText = "SELECT TOP 1 a.* FROM Actions a WHERE a.TicketID = @TicketID ORDER BY a.DateCreated DESC";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command);
            }
        }
        public void LoadByActionIDs(int[] actionIDs)
        {
            if (actionIDs.Length < 1) return;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < actionIDs.Length; i++)
            {
                if (i != 0) builder.Append(",");
                builder.Append(actionIDs[i]);
            }
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Actions WHERE ActionID IN (" + builder.ToString() + ") ORDER BY DateCreated DESC";
                command.CommandType = CommandType.Text;
                Fill(command);
            }
        }
        public static Action GetTicketDescription(LoginUser loginUser, int ticketID)
        {
            Actions actions = new Actions(loginUser);
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT a.*, u.FirstName + ' ' + u.LastName AS UserName, at.Name AS ActionTypeName FROM Actions a LEFT JOIN Users u ON a.CreatorID = u.UserID LEFT JOIN ActionTypes at ON a.ActionTypeID = at.ActionTypeID WHERE a.TicketID = @TicketID AND a.SystemActionTypeID = 1 ORDER BY a.DateCreated";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                actions.Fill(command);
            }
            if (actions.IsEmpty)
                return null;
            else
                return actions[0];
        }
        public void LoadAll()
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT a.* FROM Actions a left join tickets t on a.ticketid = t.ticketid where organizationid = 1360";
                command.CommandType = CommandType.Text;
                Fill(command);
            }
        }
        public Action FindByImportID(string importID)
        {
            foreach (Action action in this)
            {
                if (action.ImportID == importID)
                {
                    return action;
                }
            }
            return null;
        }

        public void LoadByOrganizationID(int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT a.* FROM Actions a LEFT JOIN Tickets t on a.TicketID = t.TicketID WHERE OrganizationID = @OrganizationID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }
        public void LoadModifiedByCRMLinkItem(CRMLinkTableItem item)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText =
                @"
          SELECT 
            a.* 
          FROM 
            Actions a 
            JOIN Tickets t 
              ON a.TicketID = t.TicketID
            LEFT JOIN TicketStatuses ts
              ON t.TicketStatusID = ts.TicketStatusID
          WHERE 
            a.SystemActionTypeID <> 1 
            AND a.DateModified > @DateModified
            AND 
            (
              a.DateModifiedBySalesForceSync IS NULL
              OR a.DateModified > DATEADD(s, 2, a.DateModifiedBySalesForceSync)
            )
            AND t.OrganizationID = @OrgID 
            AND 
            (
              @DateModified > '1753-01-01'
              OR ts.IsClosed = 0
            )
          ORDER BY 
            a.DateCreated DESC
        ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrgID", item.OrganizationID);
                command.Parameters.AddWithValue("@DateModified", item.LastLink == null ? new DateTime(1753, 1, 1) : item.LastLinkUtc.Value.AddHours(-1));
                using (SqlConnection connection = new SqlConnection(this.LoginUser.ConnectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandTimeout = 300;
                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    this.Table.Load(reader);
                }
            }
        }
        public void ReplaceActionType(int oldID, int newID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE Actions SET ActionTypeID = @newID WHERE (ActionTypeID = @oldID)";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@oldID", oldID);
                command.Parameters.AddWithValue("@newID", newID);
                ExecuteNonQuery(command, "Actions");
            }
        }
        public void UpdateSalesForceSync(string salesForceID, DateTime dateModifiedBySalesForceSync, int actionID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE Actions SET SalesForceID = @salesForceID, DateModifiedBySalesForceSync = @dateModifiedBySalesForceSync WHERE ActionID = @actionID";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();
                SqlParameter salesForceIDParameter = new SqlParameter("@salesForceID", DBNull.Value);
                if (salesForceID != null)
                {
                    salesForceIDParameter.Value = salesForceID;
                }
                command.Parameters.Add(salesForceIDParameter);
                command.Parameters.AddWithValue("@dateModifiedBySalesForceSync", dateModifiedBySalesForceSync);
                command.Parameters.AddWithValue("@actionID", actionID);
                ExecuteNonQuery(command, "Actions");
            }
        }
        public static Action GetActionByID(LoginUser loginUser, int actionID)
        {
            Actions actions = new Actions(loginUser);
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT a.*, u.FirstName + ' ' + u.LastName AS UserName, at.Name AS ActionTypeName FROM Actions a LEFT JOIN Users u ON a.CreatorID = u.UserID LEFT JOIN ActionTypes at ON a.ActionTypeID = at.ActionTypeID WHERE a.ActionID = @ActionID ORDER BY a.DateCreated DESC";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ActionID", actionID);
                actions.Fill(command);
            }
            if (actions.IsEmpty)
                return null;
            else
                return actions[0];
        }
        public void LoadBySalesForceID(string salesForceID, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Actions a JOIN Tickets t ON a.TicketID = t.TicketID WHERE a.SalesForceID = @SalesForceID AND t.OrganizationID = @OrganizationID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@SalesForceID", salesForceID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }
        //Changes to this method needs to be applied to ActionLinkToJira.LoadToPushToJira also.
        public void LoadToPushToJira(CRMLinkTableItem item, int ticketID)
        {
            string actionTypeFilter = "1 = 1";
            if (item.ActionTypeIDToPush != null)
            {
                actionTypeFilter = "a.ActionTypeID = @actionTypeID";
            }
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText =
                @"
          SELECT 
            a.* 
          FROM 
            Actions a
            LEFT JOIN ActionLinkToJira j
              ON a.ActionID = j.ActionID
          WHERE 
            a.SystemActionTypeID <> 1 
            AND a.TicketID = @ticketID
            AND " + actionTypeFilter + @"
            AND 
            (
              j.DateModifiedByJiraSync IS NULL
              OR a.DateModified > DATEADD(s, 2, j.DateModifiedByJiraSync)
            )
          ORDER BY 
            a.DateCreated ASC
        ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ticketID", ticketID);
                command.Parameters.AddWithValue("@DateModified", item.LastLink == null ? new DateTime(1753, 1, 1) : item.LastLinkUtc.Value.AddHours(-1));
                command.Parameters.AddWithValue("@actionTypeID", item.ActionTypeIDToPush == null ? -1 : item.ActionTypeIDToPush);
                Fill(command, "Actions");
            }
        }
        public static int GetActionPosition(LoginUser loginUser, int actionID)
        {
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"
          SELECT 
            COUNT(*) 
          FROM
            Actions 
          WHERE 
            ActionID <= @ActionID 
            AND TicketID = (SELECT TicketID FROM Actions WHERE ActionID = @ActionID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ActionID", actionID);
                int result = (int)command.ExecuteScalar();
                connection.Close();
                return result;
            }
        }
        public static int GetTicketActionCount(LoginUser loginUser, int ticketID)
        {
            int cnt = 0;
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM ACTIONS WHERE TICKETID = @TicketID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                object o = SqlExecutor.ExecuteScalar(loginUser, command);
                if (o == DBNull.Value) return -1;
                cnt = (int)o;
            }
            return cnt;
        }
    }
}