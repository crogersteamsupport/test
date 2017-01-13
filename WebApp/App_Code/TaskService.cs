using System;
using System.Globalization;
using System.Text;
using System.Web.Script.Services;
using System.Web.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Data.SqlClient;
using System.Linq;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class TaskService : System.Web.Services.WebService
    {
        public TaskService()
        {

        }

        public enum pageTab
        {
            mytasks = 0,
            assigned,
            completed
        }

        [WebMethod]
        public List<ClientTask> GetTasks(int from, int count, pageTab tab)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<string> resultItems = new List<string>();

            Reminders results = new Reminders(loginUser);
            if (tab == pageTab.mytasks)
            {
                results.LoadAssignedToUser(from, count, loginUser.UserID, true, false);
            }
            else if (tab == pageTab.assigned)
            {
                results.LoadCreatedByUser(from, count, loginUser.UserID, true, false);
                //results.LoadCompleted(from, count, loginUser.UserID, false, true);
            }
            else if (tab == pageTab.completed)
            {
                results.LoadCompleted(from, count, loginUser.UserID, false, true);
            }

            return convertToClientTasksList(results.GetReminderProxies(), loginUser);
        }

        public List<ClientTask> convertToClientTasksList(ReminderProxy[] reminderProxies, LoginUser loginUser)
        {
            List<ClientTask> clientTasks = new List<ClientTask>();

            if (reminderProxies.Any())
            {
                for (int x = 0; x < reminderProxies.Length; x++)
                {
                    ClientTask task = new ClientTask();
                    task.SubTasks = new List<ReminderProxy>();

                    task.Task = reminderProxies[x];

                    if (task.Task.UserID.HasValue)
                    {
                        Users userHelper = new Users(loginUser);
                        userHelper.LoadByUserID((int)task.Task.UserID);

                        if (userHelper.Any())
                        {
                            task.AssignedTo = userHelper[0].FirstName + ' ' + userHelper[0].LastName;
                        }
                    }
                    //TaskAssociations taskAssociationHelper = new TaskAssociations(loginUser);
                    //taskAssociationHelper.GetTaskAssociation(loginutask.Task.ReminderID);
                    task.Associations = LoadAssociations(task.Task.ReminderID);

                    if (task.Task.TaskParentID == null)
                    {
                        clientTasks.Add(task);
                    }

                    //add subtasks hook in here later... godspeed
                }

                var subtasks = reminderProxies.Where(m => m.TaskParentID != null).ToList();
                for (int x = 0; x < subtasks.Count; x++)
                {
                    var clientTask = clientTasks.Where(m => m.Task.ReminderID == subtasks[x].TaskParentID).First();
                    clientTask.SubTasks.Add(subtasks[x]);
                }
            }

            return clientTasks;
        }

        //[WebMethod]
        //public FirstLoad GetFirstLoad(int pageSize)
        //{
        //    LoginUser loginUser = TSAuthentication.GetLoginUser();

        //    FirstLoad result = new FirstLoad();
        //    result.AssignedCount = GetAssignedCount(loginUser);
        //    if (result.AssignedCount > 0)
        //    {
        //        //Load Pending
        //        result.AssignedItems = GetTasks(0, pageSize, true, false, false);
        //        if (result.AssignedItems.Count() == 0)
        //        {
        //            //Load Completed
        //            result.AssignedItems = GetTasks(0, 20, false, true, false);
        //        }
        //    }

        //    result.CreatedCount = GetCreatedCount(loginUser);
        //    if (result.CreatedCount > 0)
        //    {
        //        //Load Completed
        //        result.CreatedItems = GetTasks(0, 20, false, true, true);
        //        if (result.CreatedItems.Count() == 0)
        //        {
        //            //Load Pending
        //            result.CreatedItems = GetTasks(0, 20, true, false, true);
        //        }
        //    }

        //    return result;
        //}

        [WebMethod]
        public FirstLoad LoadPage(int start, int pageSize, pageTab tab)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            FirstLoad result = new FirstLoad();


            switch (tab)
            {
                //My Tasks
                case pageTab.mytasks:
                    result.AssignedItems = GetTasks(0, 20, tab);
                    break;
                //Assigned Tasks
                case pageTab.assigned:
                    result.CreatedItems = GetTasks(0, 20, tab);
                    break;
                //Completed Tasks
                case pageTab.completed:
                    result.AssignedItems = GetTasks(0, 20, tab);
                    break;
                default:
                    break;
            }





            //switch (assignedTab)
            //{

            //    case -1:
            //        result.AssignedCount = GetAssignedCount(loginUser);
            //        if (result.AssignedCount > 0)
            //        {
            //            //Load Pending
            //            result.AssignedItems = GetTasks(0, pageSize, true, false, false);
            //            if (result.AssignedItems.Count() == 0)
            //            {
            //                //Load Completed
            //                result.AssignedItems = GetTasks(0, 20, false, true, false);
            //            }
            //        }
            //        break;
            //    case 0:
            //        break;
            //    case 1:
            //        result.AssignedItems = GetTasks(start, pageSize, true, false, false);
            //        break;
            //    case 2:
            //        result.AssignedItems = GetTasks(start, pageSize, false, true, false);
            //        break;
            //    default:
            //        result.AssignedItems = GetTasks(start, pageSize, true, true, false);
            //        break;
            //}


            //switch (createdTab)
            //{
            //    case -1:
            //        result.CreatedCount = GetCreatedCount(loginUser);
            //        if (result.CreatedCount > 0)
            //        {
            //            //Load Completed
            //            result.CreatedItems = GetTasks(0, 20, false, true, true);
            //            if (result.CreatedItems.Count() == 0)
            //            {
            //                //Load Pending
            //                result.CreatedItems = GetTasks(0, 20, true, false, true);
            //            }
            //        }
            //        break;
            //    case 0:
            //        break;
            //    case 1:
            //        result.CreatedItems = GetTasks(start, pageSize, true, false, true);
            //        break;
            //    case 2:
            //        result.CreatedItems = GetTasks(start, pageSize, false, true, true);
            //        break;
            //    default:
            //        result.CreatedItems = GetTasks(start, pageSize, true, true, true);
            //        break;
            //}

            return result;
        }

        [WebMethod]
        public string GetShortNameFromID(int reminderID)
        {
            Reminders tasks = new Reminders(TSAuthentication.GetLoginUser());
            tasks.LoadByReminderID(reminderID);

            if (tasks.IsEmpty) return "N/A";

            string result = tasks[0].ReminderID.ToString();

            if (!String.IsNullOrEmpty(tasks[0].TaskName))
            {
                if (tasks[0].TaskName.Length > 10)
                    result = tasks[0].TaskName.Substring(0, 10).ToString() + "...";
                else
                    result = tasks[0].TaskName.ToString();
            }
            else if (!String.IsNullOrEmpty(tasks[0].Description))
            {
                if (tasks[0].Description.Length > 10)
                    result = tasks[0].Description.Substring(0, 10).ToString() + "...";
                else
                    result = tasks[0].Description.ToString();
            }

            return result;
        }


        [WebMethod]
        public TasksViewItemProxy GetTask(int reminderID)
        {
            TasksViewItem task = TasksView.GetTasksViewItem(TSAuthentication.GetLoginUser(), reminderID);
            if (task.OrganizationID != TSAuthentication.OrganizationID) return null;
            return task.GetProxy();
        }

        [WebMethod]
        public AttachmentProxy[] GetAttachments(int reminderID)
        {
            Attachments attachments = new Attachments(TSAuthentication.GetLoginUser());
            attachments.LoadByReference(ReferenceType.Tasks, reminderID);
            return attachments.GetAttachmentProxies();
        }

        [WebMethod]
        public TaskAssociationsViewItemProxy[] LoadAssociations(int reminderID)
        {
            TaskAssociationsView taskAssociations = new TaskAssociationsView(TSAuthentication.GetLoginUser());
            taskAssociations.LoadByReminderIDOnly(reminderID);
            return taskAssociations.GetTaskAssociationsViewItemProxies();
        }

        [WebMethod]
        public TasksViewItemProxy[] LoadSubtasks(int reminderID)
        {
            TasksView subtasks = new TasksView(TSAuthentication.GetLoginUser());
            subtasks.LoadByParentID(reminderID);

            return subtasks.GetTasksViewItemProxies();
        }

        [WebMethod]
        public TaskLogProxy[] LoadHistory(int reminderID, int start)
        {
            TaskLogs taskLogs = new TaskLogs(TSAuthentication.GetLoginUser());
            taskLogs.LoadByReminderID(reminderID, start);

            return taskLogs.GetTaskLogProxies();
        }

        private int GetAssignedCount(LoginUser loginUser)
        {
            SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Reminders WHERE UserID = @UserID");
            command.Parameters.AddWithValue("UserID", loginUser.UserID);
            return (int)SqlExecutor.ExecuteScalar(loginUser, command);
        }

        private int GetCreatedCount(LoginUser loginUser)
        {
            SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Reminders WHERE CreatorID = @UserID AND UserID <> @UserID");
            command.Parameters.AddWithValue("UserID", loginUser.UserID);
            return (int)SqlExecutor.ExecuteScalar(loginUser, command);
        }

        public string GetDateFormatNormal()
        {
            CultureInfo us = new CultureInfo(TSAuthentication.GetLoginUser().CultureInfo.ToString());
            return us.DateTimeFormat.ShortDatePattern;
        }

        [WebMethod]
        public ReminderProxy NewTask(string data)
        {
            TaskJsonInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<TaskJsonInfo>(data);
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Reminder newTask = (new Reminders(loginUser)).AddNewReminder();

            newTask.TaskParentID = info.TaskParentID;
            newTask.OrganizationID = TSAuthentication.OrganizationID;
            newTask.TaskName = info.TaskName;
            newTask.Description = info.Description;
            newTask.UserID = info.UserID;
            newTask.TaskIsComplete = info.TaskIsComplete;
            newTask.TaskDueDate = info.TaskDueDate;
            newTask.IsDismissed = info.IsDismissed;
            newTask.DueDate = info.DueDate;

            newTask.RefType = ReferenceType.Tasks;
            newTask.RefID = -1;
            newTask.HasEmailSent = false;

            newTask.Collection.Save();

            foreach (int ticketID in info.Tickets)
            {
                AddAssociation(newTask.ReminderID, ticketID, ReferenceType.Tickets);
            }

            foreach (int productID in info.Products)
            {
                AddAssociation(newTask.ReminderID, productID, ReferenceType.Products);
            }

            foreach (int CompanyID in info.Company)
            {
                AddAssociation(newTask.ReminderID, CompanyID, ReferenceType.Organizations);
            }

            foreach (int groupID in info.Groups)
            {
                AddAssociation(newTask.ReminderID, groupID, ReferenceType.Groups);
            }

            foreach (int UserID in info.User)
            {
                AddAssociation(newTask.ReminderID, UserID, ReferenceType.Users);
            }

            string description = String.Format("{0} created task.", TSAuthentication.GetUser(loginUser).FirstLastName);
            TaskLogs.AddTaskLog(loginUser, newTask.ReminderID, description);

            if (newTask.UserID != null && loginUser.UserID != newTask.UserID)
            {
                SendAssignedNotification(loginUser.UserID, newTask.ReminderID);
            }

            return newTask.GetProxy();
        }

        private void SendAssignedNotification(int creatorID, int reminderID)
        {
            TaskEmailPosts existingPosts = new TaskEmailPosts(TSAuthentication.GetLoginUser());
            existingPosts.LoadByReminderID(reminderID);
            if (existingPosts.Count == 0)
            {
                TaskEmailPosts posts = new TaskEmailPosts(TSAuthentication.GetLoginUser());
                TaskEmailPost post = posts.AddNewTaskEmailPost();
                post.TaskEmailPostType = (int)TaskEmailPostType.Assigned;
                post.HoldTime = 120;

                post.CreatorID = creatorID;
                post.ReminderID = reminderID;
                posts.Save();
            }
        }

        [WebMethod]
        public string SetName(int reminderID, string value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Reminder task = Reminders.GetReminder(loginUser, reminderID);
            task.TaskName = value;
            task.Collection.Save();
            string description = String.Format("{0} set task name to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, value);
            TaskLogs.AddTaskLog(loginUser, reminderID, description);

            if (task.UserID != null && loginUser.UserID != task.UserID)
            {
                SendModifiedNotification(loginUser.UserID, task.ReminderID);
            }

            return value != "" ? value : "Empty";
        }

        private void SendModifiedNotification(int creatorID, int reminderID)
        {
            TaskEmailPosts existingPosts = new TaskEmailPosts(TSAuthentication.GetLoginUser());
            existingPosts.LoadByReminderID(reminderID);
            if (existingPosts.Count == 0)
            {
                TaskEmailPosts posts = new TaskEmailPosts(TSAuthentication.GetLoginUser());
                TaskEmailPost post = posts.AddNewTaskEmailPost();
                post.TaskEmailPostType = (int)TaskEmailPostType.Modified;
                post.HoldTime = 120;

                post.CreatorID = creatorID;
                post.ReminderID = reminderID;
                posts.Save();
            }
        }

        [WebMethod]
        public string SetDescription(int reminderID, string value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Reminder task = Reminders.GetReminder(loginUser, reminderID);
            task.Description = value;
            task.Collection.Save();
            string description = String.Format("{0} set task description to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, value);
            TaskLogs.AddTaskLog(loginUser, reminderID, description);

            if (task.UserID != null && loginUser.UserID != task.UserID)
            {
                SendModifiedNotification(loginUser.UserID, task.ReminderID);
            }

            return value != "" ? value : "Empty";
        }

        [WebMethod]
        public int SetUser(int reminderID, int value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Reminder task = Reminders.GetReminder(loginUser, reminderID);
            if (value == -1)
            {
                task.UserID = null;
            }
            else
            {
                task.UserID = value;
            }
            task.Collection.Save();
            User u = Users.GetUser(loginUser, value);
            string description = String.Format("{0} set task user to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, u == null ? "Unassigned" : u.FirstLastName);
            TaskLogs.AddTaskLog(loginUser, reminderID, description);

            if (task.UserID != null && loginUser.UserID != task.UserID)
            {
                SendAssignedNotification(loginUser.UserID, task.ReminderID);
            }

            return value;
        }

        private void SendCompletedNotification(int creatorID, int reminderID)
        {
            TaskEmailPosts existingPosts = new TaskEmailPosts(TSAuthentication.GetLoginUser());
            existingPosts.LoadByReminderID(reminderID);
            if (existingPosts.Count == 0)
            {
                TaskEmailPosts posts = new TaskEmailPosts(TSAuthentication.GetLoginUser());
                TaskEmailPost post = posts.AddNewTaskEmailPost();
                post.TaskEmailPostType = (int)TaskEmailPostType.Complete;
                post.HoldTime = 120;

                post.CreatorID = creatorID;
                post.ReminderID = reminderID;
                posts.Save();
            }
        }

        [WebMethod]
        public bool SetTaskIsCompleted(int reminderID, bool value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Reminder task = Reminders.GetReminder(loginUser, reminderID);
            task.TaskIsComplete = value;
            if (value)
            {
                task.TaskDateCompleted = DateTime.UtcNow;
            }
            else
            {
                task.TaskDateCompleted = null;
            }
            task.Collection.Save();
            string description = String.Format("{0} set task is complete to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, value);
            TaskLogs.AddTaskLog(loginUser, reminderID, description);

            if (task.TaskIsComplete && (loginUser.UserID != task.CreatorID || (task.UserID != null && loginUser.UserID != task.UserID)))
            {
                SendCompletedNotification(loginUser.UserID, task.ReminderID);
            }
            else if (task.UserID != null && loginUser.UserID != task.UserID)
            {
                SendModifiedNotification(loginUser.UserID, task.ReminderID);
            }

            return value;
        }

        [WebMethod]
        public void ClearDueDate(int reminderID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Reminder task = Reminders.GetReminder(loginUser, reminderID);
            StringBuilder description = new StringBuilder();
            description.Append("Changed Due Date to None.");
            task.TaskDueDate = null;
            task.Collection.Save();
            TaskLogs.AddTaskLog(loginUser, reminderID, description.ToString());

            if (task.UserID != null && loginUser.UserID != task.UserID)
            {
                SendModifiedNotification(loginUser.UserID, task.ReminderID);
            }
        }

        [WebMethod]
        public string SetTaskDueDate(int reminderID, object value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Reminder task = Reminders.GetReminder(loginUser, reminderID);
            StringBuilder description = new StringBuilder();
            if (task.TaskDueDate == null)
            {
                description.Append(String.Format("Changed Due Date from \"{0}\" to \"{1}\".", "Unassigned", ((DateTime)value).ToString(GetDateFormatNormal())));
            }
            else
            {
                description.Append(String.Format("Changed Due Date from \"{0}\" to \"{1}\".", ((DateTime)task.TaskDueDate).ToString(GetDateFormatNormal()), ((DateTime)value).ToString(GetDateFormatNormal())));
            }
            task.TaskDueDate = (DateTime)value;
            task.Collection.Save();
            TaskLogs.AddTaskLog(loginUser, reminderID, description.ToString());

            if (task.UserID != null && loginUser.UserID != task.UserID)
            {
                SendModifiedNotification(loginUser.UserID, task.ReminderID);
            }

            return value.ToString() != "" ? value.ToString() : null;
        }

        [WebMethod]
        public bool SetIsDismissed(int reminderID, bool value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Reminder task = Reminders.GetReminder(loginUser, reminderID);
            task.IsDismissed = value;
            task.Collection.Save();
            string description = String.Format("{0} set task is dismissed to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, value);
            TaskLogs.AddTaskLog(loginUser, reminderID, description);

            if (task.UserID != null && loginUser.UserID != task.UserID)
            {
                SendModifiedNotification(loginUser.UserID, task.ReminderID);
            }

            return value;
        }

        [WebMethod]
        public void ClearReminderDate(int reminderID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Reminder task = Reminders.GetReminder(loginUser, reminderID);
            StringBuilder description = new StringBuilder();
            description.Append("Changed Reminder Date to None.");
            task.DueDate = null;
            task.Collection.Save();
            TaskLogs.AddTaskLog(loginUser, reminderID, description.ToString());

            if (task.UserID != null && loginUser.UserID != task.UserID)
            {
                SendModifiedNotification(loginUser.UserID, task.ReminderID);
            }
        }

        [WebMethod]
        public string SetDueDate(int reminderID, object value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Reminder task = Reminders.GetReminder(loginUser, reminderID);
            StringBuilder description = new StringBuilder();
            if (task.DueDate == null)
            {
                description.Append(String.Format("Changed Reminder Date from \"{0}\" to \"{1}\".", "Unassigned", ((DateTime)value).ToString(GetDateFormatNormal())));
            }
            else
            {
                description.Append(String.Format("Changed Reminder Date from \"{0}\" to \"{1}\".", ((DateTime)task.DueDate).ToString(GetDateFormatNormal()), ((DateTime)value).ToString(GetDateFormatNormal())));
            }
            task.DueDate = (DateTime)value;
            task.Collection.Save();
            TaskLogs.AddTaskLog(loginUser, reminderID, description.ToString());

            if (task.UserID != null && loginUser.UserID != task.UserID)
            {
                SendModifiedNotification(loginUser.UserID, task.ReminderID);
            }

            return value.ToString() != "" ? value.ToString() : null;
        }

        [WebMethod]
        public bool AddAssociation(int reminderID, int refID, ReferenceType refType)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            try
            {
                TaskAssociation taskAssociation = (new TaskAssociations(loginUser).AddNewTaskAssociation());
                taskAssociation.ReminderID = reminderID;
                taskAssociation.RefID = refID;
                taskAssociation.RefType = (int)refType;
                taskAssociation.DateCreated = DateTime.UtcNow;
                taskAssociation.CreatorID = loginUser.UserID;
                taskAssociation.Collection.Save();
                string description = String.Format("{0} added task association to {1}.", TSAuthentication.GetUser(loginUser).FirstLastName, Enum.GetName(typeof(ReferenceType), refType));
                TaskLogs.AddTaskLog(loginUser, reminderID, description);

                Reminder task = Reminders.GetReminder(loginUser, reminderID);
                if (task.UserID != null && loginUser.UserID != task.UserID)
                {
                    SendModifiedNotification(loginUser.UserID, task.ReminderID);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [WebMethod(true)]
        public void DeleteAssociation(int reminderID, int refID, ReferenceType refType)
        {
            try
            {
                LoginUser loginUser = TSAuthentication.GetLoginUser();
                TaskAssociations associations = new TaskAssociations(loginUser);
                associations.DeleteAssociation(reminderID, refID, refType);
                string description = String.Format("{0} deleted task association to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, Enum.GetName(typeof(ReferenceType), refType));
                TaskLogs.AddTaskLog(loginUser, reminderID, description);

                Reminder task = Reminders.GetReminder(loginUser, reminderID);
                if (task.UserID != null && loginUser.UserID != task.UserID)
                {
                    SendModifiedNotification(loginUser.UserID, task.ReminderID);
                }
            }
            catch (Exception ex)
            {
                DataUtils.LogException(UserSession.LoginUser, ex);
            }
        }
    }

    [DataContract(Namespace = "http://teamsupport.com/")]
    public class ClientTask
    {
        public ReminderProxy Task { get; set; }
        public List<ReminderProxy> SubTasks { get; set; }
        public TaskAssociationsViewItemProxy[] Associations { get; set; }
        public string AssignedTo { get; set; }
    }

    [DataContract(Namespace = "http://teamsupport.com/")]
    public class FirstLoad
    {
        [DataMember]
        public int AssignedCount { get; set; }
        [DataMember]
        public List<ClientTask> AssignedItems { get; set; }
        [DataMember]
        public int CreatedCount { get; set; }
        [DataMember]
        public List<ClientTask> CreatedItems { get; set; }
    }

    [DataContract(Namespace = "http://teamsupport.com/")]
    public class TaskJsonInfo
    {
        public TaskJsonInfo() { }
        [DataMember]
        public string TaskName { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int? UserID { get; set; }
        [DataMember]
        public bool TaskIsComplete { get; set; }
        [DataMember]
        public DateTime? TaskDueDate { get; set; }
        [DataMember]
        public bool IsDismissed { get; set; }
        [DataMember]
        public DateTime? DueDate { get; set; }
        [DataMember]
        public List<int> Tickets { get; set; }
        [DataMember]
        public List<int> Groups { get; set; }
        [DataMember]
        public List<int> Products { get; set; }
        [DataMember]
        public List<int> Company { get; set; }
        [DataMember]
        public List<int> User { get; set; }
        [DataMember]
        public int? TaskParentID { get; set; }
    }
}