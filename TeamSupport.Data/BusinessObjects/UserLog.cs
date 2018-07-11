using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Diagnostics;

namespace TeamSupport.Data
{
    [Table(Name = "UserLogs")]
    public class UserLog
    {
#pragma warning disable CS0649  // Field is never assigned to in code (assigned by linq to sql)
        int _userLogID;
        [Column(Storage = "_userLogID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int UserLogID { get { return _userLogID; } }

        [Column]
        DateTime DateCreated;
        [Column]
        public int? OrganizationID;
        [Column]
        public int TicketID;
        [Column]
        public int UserID;
        [Column]
        public int LogType;
        [Column]
        public string Description;
#pragma warning restore CS0649

        static HashSet<UserLog> _queue = new HashSet<UserLog>();

        public static void AddLog(LoginUser loginUser, ActionLogType type, int ticketID, string description)
        {
            UserLog userLog = new UserLog()
            {
                DateCreated = DateTime.UtcNow,
                OrganizationID = loginUser.OrganizationID < 0 ? null : (int?)loginUser.OrganizationID,
                TicketID = ticketID,
                UserID = loginUser.UserID,
                LogType = (int)type,
                Description = description
            };
            _queue.Add(userLog);
            if (_queue.Count > 100)
            {
                HashSet<UserLog> tmp = _queue;
                _queue = new HashSet<UserLog>();
                System.Threading.Tasks.Task.Run(() => SaveTask(tmp)); // background task
            }
        }

        public static void SaveTask(HashSet<UserLog> userLogs)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(LoginUser.GetConnectionString(-1)))
                using (DataContext db = new DataContext(connection))
                {
                    Table<UserLog> table = db.GetTable<UserLog>();
                    foreach (UserLog userLog in userLogs)
                        table.InsertOnSubmit(userLog);
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("Application", "UserLog Save Failed" + e.Message + " ----- STACK: " + e.StackTrace.ToString());
                Console.WriteLine(e.ToString());
            }
        }
    }
}
