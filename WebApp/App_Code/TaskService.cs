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

}