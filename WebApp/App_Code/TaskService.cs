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

        [WebMethod]
        public ReminderProxy[] GetTasks(int from, int count, bool searchPending, bool searchComplete, bool searchCreated)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<string> resultItems = new List<string>();

            Reminders results = new Reminders(loginUser);
            if (searchCreated)
            {
                results.LoadCreatedByUser(from, count, loginUser.UserID, searchPending, searchComplete);
            }
            else
            {
                results.LoadAssignedToUser(from, count, loginUser.UserID, searchPending, searchComplete);
            }
            return results.GetReminderProxies();
        }

        [WebMethod]
        public FirstLoad GetFirstLoad(int pageSize)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            FirstLoad result = new FirstLoad();
            result.AssignedCount = GetAssignedCount(loginUser);
            if (result.AssignedCount > 0)
            {
                //Load Pending
                result.AssignedItems = GetTasks(0, pageSize, true, false, false);
                if (result.AssignedItems.Length == 0)
                {
                    //Load Completed
                    result.AssignedItems = GetTasks(0, 20, false, true, false);
                }
            }

            result.CreatedCount = GetCreatedCount(loginUser);
            if (result.CreatedCount > 0)
            {
                //Load Completed
                result.CreatedItems = GetTasks(0, 20, false, true, true);
                if (result.CreatedItems.Length == 0)
                {
                    //Load Pending
                    result.CreatedItems = GetTasks(0, 20, true, false, true);
                }
            }

            return result;
        }

        [WebMethod]
        public FirstLoad LoadPage(int start, int pageSize, int assignedTab, int createdTab)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            FirstLoad result = new FirstLoad();
            switch (assignedTab)
            {
                case -1:
                    result.AssignedCount = GetAssignedCount(loginUser);
                    if (result.AssignedCount > 0)
                    {
                        //Load Pending
                        result.AssignedItems = GetTasks(0, pageSize, true, false, false);
                        if (result.AssignedItems.Length == 0)
                        {
                            //Load Completed
                            result.AssignedItems = GetTasks(0, 20, false, true, false);
                        }
                    }
                    break;
                case 0:
                    break;
                case 1:
                    result.AssignedItems = GetTasks(start, pageSize, true, false, false);
                    break;
                case 2:
                    result.AssignedItems = GetTasks(start, pageSize, false, true, false);
                    break;
                default:
                    result.AssignedItems = GetTasks(start, pageSize, true, true, false);
                    break;
            }


            switch (createdTab)
            {
                case -1:
                    result.CreatedCount = GetCreatedCount(loginUser);
                    if (result.CreatedCount > 0)
                    {
                        //Load Completed
                        result.CreatedItems = GetTasks(0, 20, false, true, true);
                        if (result.CreatedItems.Length == 0)
                        {
                            //Load Pending
                            result.CreatedItems = GetTasks(0, 20, true, false, true);
                        }
                    }
                    break;
                case 0:
                    break;
                case 1:
                    result.CreatedItems = GetTasks(start, pageSize, true, false, true);
                    break;
                case 2:
                    result.CreatedItems = GetTasks(start, pageSize, false, true, true);
                    break;
                default:
                    result.CreatedItems = GetTasks(start, pageSize, true, true, true);
                    break;
            }

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
        public ReminderProxy[] LoadSubtasks(int reminderID)
        {
            Reminders subtasks = new Reminders(TSAuthentication.GetLoginUser());
            subtasks.LoadByParentID(reminderID);

            return subtasks.GetReminderProxies();
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

            return newTask.GetProxy();
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
            return value != "" ? value : "Empty";
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
            return value != "" ? value : "Empty";
        }

        [WebMethod]
        public int SetUser(int reminderID, int value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Reminder task = Reminders.GetReminder(loginUser, reminderID);
            task.UserID = value;
            task.Collection.Save();
            User u = Users.GetUser(loginUser, value);
            string description = String.Format("{0} set task user to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, u == null ? "Unassigned" : u.FirstLastName);
            TaskLogs.AddTaskLog(loginUser, reminderID, description);
            return value;
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
            return value;
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
            return value;
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
                TaskAssociations associations = new TaskAssociations(UserSession.LoginUser);
                associations.DeleteAssociation(reminderID, refID, refType);
                string description = String.Format("{0} deleted task association to {1} ", TSAuthentication.GetUser(UserSession.LoginUser).FirstLastName, Enum.GetName(typeof(ReferenceType), refType));
                TaskLogs.AddTaskLog(UserSession.LoginUser, reminderID, description);
            }
            catch (Exception ex)
            {
                DataUtils.LogException(UserSession.LoginUser, ex);
            }
        }
    }



    [DataContract(Namespace = "http://teamsupport.com/")]
    public class FirstLoad
    {
        [DataMember]
        public int AssignedCount { get; set; }
        [DataMember]
        public ReminderProxy[] AssignedItems { get; set; }
        [DataMember]
        public int CreatedCount { get; set; }
        [DataMember]
        public ReminderProxy[] CreatedItems { get; set; }
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
        public int UserID { get; set; }
        [DataMember]
        public bool TaskIsComplete { get; set; }
        [DataMember]
        public DateTime TaskDueDate { get; set; }
        [DataMember]
        public bool IsDismissed { get; set; }
        [DataMember]
        public DateTime DueDate { get; set; }
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
    }
}