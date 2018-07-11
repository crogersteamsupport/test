using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Diagnostics;

namespace TeamSupport.Data
{
    [Table(Name = "TicketLogs")]
    public class TicketLog
    {
#pragma warning disable CS0649  // Field is never assigned to in code (assigned by linq to sql)
        int _ticketLogID;
        [Column(Storage = "_ticketLogID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int TicketLogID { get { return _ticketLogID; } }

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

        static HashSet<TicketLog> _queue = new HashSet<TicketLog>();

        public static void AddLog(LoginUser loginUser, ActionLogType type, int ticketID, string description)
        {
            TicketLog ticketLog = new TicketLog()
            {
                DateCreated = DateTime.UtcNow,
                OrganizationID = loginUser.OrganizationID < 0 ? null : (int?)loginUser.OrganizationID,
                TicketID = ticketID,
                UserID = loginUser.UserID,
                LogType = (int)type,
                Description = description
            };
            _queue.Add(ticketLog);
            if (_queue.Count > 100)
            {
                HashSet<TicketLog> tmp = _queue;
                _queue = new HashSet<TicketLog>();
                System.Threading.Tasks.Task.Run(() => SaveTask(tmp)); // background task
            }
        }

        public static void SaveTask(HashSet<TicketLog> ticketLogs)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(LoginUser.GetConnectionString(-1)))
                using (DataContext db = new DataContext(connection))
                {
                    Table<TicketLog> table = db.GetTable<TicketLog>();
                    foreach(TicketLog ticketLog in ticketLogs)
                        table.InsertOnSubmit(ticketLog);
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("Application", "TicketLog Save Failed" + e.Message + " ----- STACK: " + e.StackTrace.ToString());
                Console.WriteLine(e.ToString());
            }
        }
    }
}
