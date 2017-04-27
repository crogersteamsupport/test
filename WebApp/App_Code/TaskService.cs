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
using System.Data;

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
        public List<TaskDTO> GetTasks(int from, int count, pageTab tab)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<TaskDTO> taskList = new List<TaskDTO>();

            Tasks tasksHelper = new Tasks(loginUser);
            if (tab == pageTab.mytasks)
            {
                taskList = tasksHelper.LoadMyTasks(from, count, loginUser.UserID, true, false);
            }
            else if (tab == pageTab.assigned)
            {
                taskList = tasksHelper.LoadAssignedTasks(from, count, loginUser.UserID, true, false);
            }
            else if (tab == pageTab.completed)
            {
                taskList = tasksHelper.LoadCompleted(from, count, loginUser.UserID, false, true);
            }

            return convertToClientTasksList(taskList, loginUser);
        }

        [WebMethod]
        public List<TaskDTO> GetCustomerTasks(int from, int count, int organizationID)
        {
            List<TaskDTO> result = new List<TaskDTO>();
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Tasks taskHelper = new Tasks(loginUser);

            result = taskHelper.LoadByCompany(from, count, organizationID);
            return convertToClientTasksList(result, loginUser);
        }

        [WebMethod]
        public List<TaskDTO> GetContactTasks(int from, int count, int contactID)
        {
            List<TaskDTO> result = new List<TaskDTO>();

            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Tasks taskHelper = new Tasks(loginUser);

            result = taskHelper.LoadByContact(from, count, contactID);
            return convertToClientTasksList(result, loginUser);
        }

        [WebMethod]
        public List<TaskDTO> GetUserTasks(int from, int count, int userID)
        {
            List<TaskDTO> result = new List<TaskDTO>();

            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Tasks taskHelper = new Tasks(loginUser);

            result = taskHelper.LoadByUser(from, count, userID);
            return convertToClientTasksList(result, loginUser);
        }

        public List<TaskDTO> GetTasksByTicketID(int ticketID)
        {
            List<TaskDTO> result = new List<TaskDTO>();
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<string> resultItems = new List<string>();

            Tasks taskHelper = new Tasks(loginUser);
            result = taskHelper.LoadByTicketID(ticketID);

            return convertToClientTasksList(result, loginUser);
        }

        public List<TaskDTO> convertToClientTasksList(List<TaskDTO> tasks, LoginUser loginUser)
        {
            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    task.SubTasks = new List<TaskDTO>();

                    if (task.UserID.HasValue)
                    {
                        Users userHelper = new Users(loginUser);
                        userHelper.LoadByUserID((int)task.UserID);

                        if (userHelper.Any())
                        {
                            task.AssignedTo = userHelper[0].FirstName + ' ' + userHelper[0].LastName;
                        }
                    }

                    task.Associations = LoadAssociations(task.TaskID);
                }

            }

            return tasks;
        }

        [WebMethod]
        public TasksModel LoadPage(int start, int pageSize, pageTab tab)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            TasksModel result = new TasksModel();

            switch (tab)
            {
                //My Tasks
                case pageTab.mytasks:
                    result.AssignedItems = GetTasks(start, pageSize, tab);
                    break;
                //Assigned Tasks
                case pageTab.assigned:
                    result.CreatedItems = GetTasks(start, pageSize, tab);
                    break;
                //Completed Tasks
                case pageTab.completed:
                    result.CompletedItems = GetTasks(start, pageSize, tab);
                    break;
                default:
                    break;
            }

            return result;
        }

        [WebMethod]
        public string GetShortNameFromID(int taskID)
        {
            Tasks tasks = new Tasks(TSAuthentication.GetLoginUser());
            tasks.LoadByTaskID(taskID);

            if (tasks.IsEmpty) return "N/A";

            string result = tasks[0].ReminderID.ToString();

            if (!String.IsNullOrEmpty(tasks[0].Name))
            {
                if (tasks[0].Name.Length > 10)
                    result = tasks[0].Name.Substring(0, 10).ToString() + "...";
                else
                    result = tasks[0].Name.ToString();
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
        public TasksViewItemProxy GetTask(int taskID)
        {
            TasksViewItem task = TasksView.GetTasksViewItem(TSAuthentication.GetLoginUser(), taskID);
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
        public TaskAssociationsViewItemProxy[] LoadAssociations(int taskID)
        {
            TaskAssociationsView taskAssociations = new TaskAssociationsView(TSAuthentication.GetLoginUser());
            taskAssociations.LoadByTaskIDOnly(taskID);
            return taskAssociations.GetTaskAssociationsViewItemProxies();
        }

        [WebMethod]
        public TasksViewItemProxy[] LoadSubtasks(int taskID)
        {
            TasksView subtasks = new TasksView(TSAuthentication.GetLoginUser());
            subtasks.LoadByParentID(taskID);

            return subtasks.GetTasksViewItemProxies();
        }

        [WebMethod]
        public TaskLogProxy[] LoadHistory(int taskID, int start)
        {
            TaskLogs taskLogs = new TaskLogs(TSAuthentication.GetLoginUser());
            taskLogs.LoadByTaskID(taskID, start);

            return taskLogs.GetTaskLogProxies();
        }

        private int GetAssignedCount(LoginUser loginUser)
        {
            SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Tasks WHERE UserID = @UserID");
            command.Parameters.AddWithValue("UserID", loginUser.UserID);
            return (int)SqlExecutor.ExecuteScalar(loginUser, command);
        }

        private int GetCreatedCount(LoginUser loginUser)
        {
            SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Tasks WHERE CreatorID = @UserID AND UserID <> @UserID");
            command.Parameters.AddWithValue("UserID", loginUser.UserID);
            return (int)SqlExecutor.ExecuteScalar(loginUser, command);
        }

        public string GetDateFormatNormal()
        {
            CultureInfo us = new CultureInfo(TSAuthentication.GetLoginUser().CultureInfo.ToString());
            return us.DateTimeFormat.ShortDatePattern;
        }

        private Reminder CreateReminder(LoginUser loginUser, int taskID, string taskName, DateTime reminderDate, bool isDismissed)
        {
            Reminders reminderHelper = new Reminders(loginUser);
            Reminder reminder = reminderHelper.AddNewReminder();

            reminder.DateCreated = DateTime.UtcNow;
            reminder.Description = taskName;
            DateTime reminderDueDate = DateTime.Now;
            if (reminderDate != null)
            {
                reminderDueDate = (DateTime)reminderDate;
            }
            reminder.DueDate = reminderDueDate;
            reminder.IsDismissed = isDismissed;
            reminder.RefType = ReferenceType.Tasks;
            reminder.RefID = taskID;
            reminder.HasEmailSent = false;
            reminder.UserID = loginUser.UserID;
            reminder.CreatorID = loginUser.UserID;
            reminder.OrganizationID = loginUser.OrganizationID;

            reminderHelper.Save();

            return reminder;
        }

        [WebMethod]
        public TaskProxy NewTask(string data)
        {
            TaskJsonInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<TaskJsonInfo>(data);
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            Task newTask = (new Tasks(loginUser)).AddNewTask();

            newTask.ParentID = info.ParentID;
            newTask.OrganizationID = TSAuthentication.OrganizationID;
            newTask.Name = info.Name;
            newTask.Description = info.Description;
            newTask.UserID = info.UserID;
            newTask.IsComplete = info.IsComplete;

            if (newTask.IsComplete) newTask.DateCompleted = DateTime.UtcNow;

            newTask.DueDate = DataUtils.DateToUtc(loginUser, info.DueDate);

            newTask.Collection.Save();

            if (info.Reminder != null)
            {
                Reminder reminder = CreateReminder(loginUser, newTask.TaskID, info.Name, DataUtils.DateToUtc(loginUser, (DateTime)info.Reminder), false);
                if (reminder != null)
                {
                    Tasks taskHelper = new Tasks(loginUser);
                    taskHelper.LoadByTaskID(newTask.TaskID);

                    if (taskHelper.Any())
                    {
                        taskHelper[0].ReminderID = reminder.ReminderID;
                        taskHelper.Save();
                    }
                }
            }

            foreach (int ticketID in info.Tickets)
            {
                AddAssociation(newTask.TaskID, ticketID, ReferenceType.Tickets);
            }

            foreach (int productID in info.Products)
            {
                AddAssociation(newTask.TaskID, productID, ReferenceType.Products);
            }

            foreach (int CompanyID in info.Company)
            {
                AddAssociation(newTask.TaskID, CompanyID, ReferenceType.Organizations);
            }

            foreach (int UserID in info.Contacts)
            {
                AddAssociation(newTask.TaskID, UserID, ReferenceType.Contacts);
            }

            foreach (int groupID in info.Groups)
            {
                AddAssociation(newTask.TaskID, groupID, ReferenceType.Groups);
            }

            foreach (int UserID in info.User)
            {
                AddAssociation(newTask.TaskID, UserID, ReferenceType.Users);
            }

            string description = String.Format("{0} created task.", TSAuthentication.GetUser(loginUser).FirstLastName);
            TaskLogs.AddTaskLog(loginUser, newTask.TaskID, description);

            if (newTask.UserID != null && loginUser.UserID != newTask.UserID)
            {
                SendAssignedNotification(loginUser.UserID, newTask.TaskID);
            }

            return newTask.GetProxy();
        }

        private void SendOldUserNotification(int creatorID, int oldUserID, int taskID)
        {
            TaskEmailPosts existingPosts = new TaskEmailPosts(TSAuthentication.GetLoginUser());
            existingPosts.LoadByTaskIDIDAndPostType(taskID, TaskEmailPostType.OldUser);
            if (existingPosts.Count == 0)
            {
                TaskEmailPosts posts = new TaskEmailPosts(TSAuthentication.GetLoginUser());
                TaskEmailPost post = posts.AddNewTaskEmailPost();
                post.TaskEmailPostType = (int)TaskEmailPostType.OldUser;
                post.HoldTime = 120;

                post.CreatorID = creatorID;
                post.TaskID = taskID;
                post.OldUserID = oldUserID;
                posts.Save();
            }
        }

        private void SendAssignedNotification(int creatorID, int taskID)
        {
            TaskEmailPosts existingPosts = new TaskEmailPosts(TSAuthentication.GetLoginUser());
            existingPosts.LoadByTaskID(taskID);
            if (existingPosts.Count == 0)
            {
                TaskEmailPosts posts = new TaskEmailPosts(TSAuthentication.GetLoginUser());
                TaskEmailPost post = posts.AddNewTaskEmailPost();
                post.TaskEmailPostType = (int)TaskEmailPostType.Assigned;
                post.HoldTime = 120;

                post.CreatorID = creatorID;
                post.TaskID = taskID;
                posts.Save();
            }
        }

        [WebMethod]
        public string SetName(int taskID, string value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Task task = Tasks.GetTask(loginUser, taskID);
            task.Name = value;

            task.Collection.Save();
            string description = String.Format("{0} set task name to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, value);
            TaskLogs.AddTaskLog(loginUser, taskID, description);

            if (task.UserID != null && loginUser.UserID != task.UserID)
            {
                SendModifiedNotification(loginUser.UserID, task.TaskID);
            }

            return value != "" ? value : "Empty";
        }

        private void SendModifiedNotification(int creatorID, int taskID)
        {
            TaskEmailPosts existingPosts = new TaskEmailPosts(TSAuthentication.GetLoginUser());
            existingPosts.LoadByTaskID(taskID);
            if (existingPosts.Count == 0)
            {
                TaskEmailPosts posts = new TaskEmailPosts(TSAuthentication.GetLoginUser());
                TaskEmailPost post = posts.AddNewTaskEmailPost();
                post.TaskEmailPostType = (int)TaskEmailPostType.Modified;
                post.HoldTime = 120;

                post.CreatorID = creatorID;
                post.TaskID = taskID;
                posts.Save();
            }
        }

        [WebMethod]
        public string SetDescription(int taskID, string value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Task task = Tasks.GetTask(loginUser, taskID);
            task.Description = value;
            task.Collection.Save();
            string description = String.Format("{0} set task description to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, value);
            TaskLogs.AddTaskLog(loginUser, taskID, description);

            if (task.UserID != null && loginUser.UserID != task.UserID)
            {
                SendModifiedNotification(loginUser.UserID, task.TaskID);
            }

            return value != "" ? value : "Empty";
        }

        [WebMethod]
        public int SetUser(int taskID, int value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Task task = Tasks.GetTask(loginUser, taskID);

            Reminder reminder = Reminders.GetReminderByTaskID(loginUser, taskID);

            if (task.UserID != null && loginUser.UserID != task.UserID && value != task.UserID)
            {
                SendOldUserNotification(loginUser.UserID, (int)task.UserID, task.TaskID);
            }

            if (value != -1 && loginUser.UserID != value && value != task.UserID)
            {
                SendAssignedNotification(loginUser.UserID, task.TaskID);
            }

            //User is being set to unassigned
            if (value == -1)
            {
                if (reminder != null)
                {
                    reminder.Delete();
                    reminder.Collection.Save();
                    task.ReminderID = null;
                }
                task.UserID = null;
            }
            else
            {
                if (reminder != null)
                {
                    reminder.UserID = value;
                    reminder.Collection.Save();
                }
                task.UserID = value;
            }

            task.Collection.Save();
            
            User u = Users.GetUser(loginUser, value);
            string description = String.Format("{0} set task user to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, u == null ? "Unassigned" : u.FirstLastName);
            TaskLogs.AddTaskLog(loginUser, taskID, description);

            return value;
        }

        private void SendCompletedNotification(int creatorID, int taskID)
        {
            TaskEmailPosts existingPosts = new TaskEmailPosts(TSAuthentication.GetLoginUser());
            existingPosts.LoadByTaskID(taskID);
            if (existingPosts.Count == 0)
            {
                TaskEmailPosts posts = new TaskEmailPosts(TSAuthentication.GetLoginUser());
                TaskEmailPost post = posts.AddNewTaskEmailPost();
                post.TaskEmailPostType = (int)TaskEmailPostType.Complete;
                post.HoldTime = 120;

                post.CreatorID = creatorID;
                post.TaskID = taskID;
                posts.Save();
            }
        }

        [WebMethod]
        public TaskCompletionStatus SetTaskIsCompleted(int taskID, bool value)
        {
            TaskCompletionStatus result = new TaskCompletionStatus(false, value);

            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Task task = Tasks.GetTask(loginUser, taskID);
            task.IsComplete = value;

            //if a user is attempting to complete a task check for incomplete subtasks first
            if (value)
            {
                if (GetIncompleteSubtasks(taskID))
                {
                    result.IncompleteSubtasks = true;
                    result.Value = !value;
                    return result;
                }
                task.DateCompleted = DateTime.UtcNow;
            }
            else
            {
                result.IncompleteSubtasks = false;
                result.Value = value;
                task.DateCompleted = null;
            }

            task.Collection.Save();
            string description = String.Format("{0} set task is complete to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, value);
            TaskLogs.AddTaskLog(loginUser, taskID, description);

            if (task.IsComplete && (loginUser.UserID != task.CreatorID || (task.UserID != null && loginUser.UserID != task.UserID)))
            {
                SendCompletedNotification(loginUser.UserID, task.TaskID);
            }
            else if (task.UserID != null && loginUser.UserID != task.UserID)
            {
                SendModifiedNotification(loginUser.UserID, task.TaskID);
            }

            return result;
        }

        [WebMethod]
        public bool GetIncompleteSubtasks(int taskID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Tasks incompleteSubtasks = new Tasks(loginUser);
            incompleteSubtasks.LoadIncompleteByParentID(taskID);
            bool result = false;
            if (incompleteSubtasks.Count > 0)
            {
                result = true;
            }
            return result;
        }

        [WebMethod]
        public void ClearDueDate(int taskID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Task task = Tasks.GetTask(loginUser, taskID);
            StringBuilder description = new StringBuilder();
            description.Append("Changed Due Date to None.");
            task.DueDate = null;
            task.Collection.Save();
            TaskLogs.AddTaskLog(loginUser, taskID, description.ToString());

            if (task.UserID != null && loginUser.UserID != task.UserID)
            {
                SendModifiedNotification(loginUser.UserID, task.TaskID);
            }
        }

        [WebMethod]
        public ReminderProxy SetReminderDueDate(int taskID, string data)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Reminder reminder = Reminders.GetReminderByTaskID(loginUser, taskID);
            StringBuilder description = new StringBuilder();
            Task task = Tasks.GetTask(loginUser, taskID);
            TaskJsonInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<TaskJsonInfo>(data);
            DateTime reminderDueDate = (DateTime)info.Reminder;
            
            if (reminder != null)
            {
                if (reminder.DueDate == null)
                {
                    description.Append(String.Format("Changed Reminder from \"{0}\" to \"{1}\".", "Unassigned", (reminderDueDate).ToString()));
                }
                else
                {
                    description.Append(String.Format("Changed Reminder from \"{0}\" to \"{1}\".", ((DateTime)reminder.DueDate).ToString(), (reminderDueDate).ToString()));
                }
                reminder.DueDate = DataUtils.DateToUtc(loginUser, reminderDueDate);
                reminder.IsDismissed = false;
                reminder.HasEmailSent = false;
                reminder.Collection.Save();
                TaskLogs.AddTaskLog(loginUser, taskID, description.ToString());
            }
            else
            {
                if (reminder == null)
                {
                    reminder = CreateReminder(loginUser, taskID, task.Name, DataUtils.DateToUtc(loginUser, reminderDueDate), false);
                    task.ReminderID = reminder.ReminderID;
                    task.Collection.Save();
                }
            }

            if (task.UserID != null && loginUser.UserID != task.UserID)
            {
                SendModifiedNotification(loginUser.UserID, task.TaskID);
            }

            return reminder.GetProxy();
        }

        [WebMethod]
        public void ClearReminderDate(int taskID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Reminder reminder = Reminders.GetReminderByTaskID(loginUser, taskID);
            StringBuilder description = new StringBuilder();
            description.Append("Changed Reminder Date to None.");
            reminder.Delete();
            reminder.Collection.Save();
            TaskLogs.AddTaskLog(loginUser, taskID, "Reminder deleted");

            SendModifiedNotification(loginUser.UserID, taskID);
        }

        [WebMethod]
        public TaskProxy SetDueDate(int taskID, string data)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Task task = Tasks.GetTask(loginUser, taskID);
            StringBuilder description = new StringBuilder();
            TaskJsonInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<TaskJsonInfo>(data);
            DateTime dueDate = (DateTime)info.DueDate;

            if (task.DueDate == null)
            {
                description.Append(String.Format("Changed Due Date from \"{0}\" to \"{1}\".", "Unassigned", (dueDate).ToString()));
            }
            else
            {
                description.Append(String.Format("Changed Due Date from \"{0}\" to \"{1}\".", ((DateTime)task.DueDate).ToString(), (dueDate).ToString()));
            }
            task.DueDate = DataUtils.DateToUtc(loginUser, dueDate);
            task.Collection.Save();
            TaskLogs.AddTaskLog(loginUser, taskID, description.ToString());

            if (task.UserID != null && loginUser.UserID != task.UserID)
            {
                SendModifiedNotification(loginUser.UserID, task.TaskID);
            }

            return task.GetProxy();
        }

        [WebMethod]
        public bool AddAssociation(int taskID, int refID, ReferenceType refType)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            try
            {
                TaskAssociation taskAssociation = (new TaskAssociations(loginUser).AddNewTaskAssociation());
                taskAssociation.TaskID = taskID;
                taskAssociation.RefID = refID;
                taskAssociation.RefType = (int)refType;
                taskAssociation.DateCreated = DateTime.UtcNow;
                taskAssociation.CreatorID = loginUser.UserID;
                taskAssociation.Collection.Save();
                string description = String.Format("{0} added task association to {1}.", TSAuthentication.GetUser(loginUser).FirstLastName, Enum.GetName(typeof(ReferenceType), refType));
                TaskLogs.AddTaskLog(loginUser, taskID, description);

                Reminder task = Reminders.GetReminder(loginUser, taskID);
                if (task.UserID != null && loginUser.UserID != task.UserID)
                {
                    SendModifiedNotification(loginUser.UserID, task.ReminderID);
                }

                if (refType == ReferenceType.Contacts)
                {
                    TeamSupport.Data.User user = Users.GetUser(loginUser, refID);
                    taskAssociation = (new TaskAssociations(loginUser).AddNewTaskAssociation());
                    taskAssociation.TaskID = taskID;
                    taskAssociation.RefID = user.OrganizationID;
                    taskAssociation.RefType = (int)ReferenceType.Organizations;
                    taskAssociation.DateCreated = DateTime.UtcNow;
                    taskAssociation.CreatorID = loginUser.UserID;
                    try
                    {
                        taskAssociation.Collection.Save();
                    }
                    catch (Exception e)
                    {
                        //TaskAssociation do not allow duplicates. This could happen when the company is already associated with the task.
                    }
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
        public TaskDTO Task { get; set; }
        public ReminderProxy Reminder { get; set; }
        public List<TaskDTO> SubTasks { get; set; }
        public TaskAssociationsViewItemProxy[] Associations { get; set; }
        public string AssignedTo { get; set; }
    }

    [DataContract(Namespace = "http://teamsupport.com/")]
    public class TasksModel
    {
        [DataMember]
        public int AssignedCount { get; set; }
        [DataMember]
        public int CreatedCount { get; set; }
        [DataMember]
        public List<TaskDTO> AssignedItems { get; set; }
        [DataMember]
        public List<TaskDTO> CreatedItems { get; set; }
        [DataMember]
        public List<TaskDTO> CompletedItems { get; set; }
    }

    [DataContract(Namespace = "http://teamsupport.com/")]
    public class TaskJsonInfo
    {
        public TaskJsonInfo() { }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int? UserID { get; set; }
        [DataMember]
        public bool IsComplete { get; set; }
        [DataMember]
        public DateTime? DueDate { get; set; }
        [DataMember]
        public bool IsDismissed { get; set; }
        [DataMember]
        public DateTime? Reminder { get; set; }
        [DataMember]
        public List<int> Tickets { get; set; }
        [DataMember]
        public List<int> Groups { get; set; }
        [DataMember]
        public List<int> Products { get; set; }
        [DataMember]
        public List<int> Company { get; set; }
        [DataMember]
        public List<int> Contacts { get; set; }
        [DataMember]
        public List<int> User { get; set; }
        [DataMember]
        public int? ParentID { get; set; }
    }

    public class TaskCompletionStatus
    {
        public TaskCompletionStatus() { }
        public TaskCompletionStatus(bool incompleteSubtasks, bool value)
        {
            IncompleteSubtasks = incompleteSubtasks;
            Value = value;
        }

        [DataMember]
        public bool IncompleteSubtasks { get; set; }

        [DataMember]
        public bool Value { get; set; }
    }
}