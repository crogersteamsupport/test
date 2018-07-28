using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Linq;
using System.IO;
using System.Diagnostics;


namespace TeamSupport.Model
{
    /// <summary>
    /// Model for validating OrganizationID, UserID, TicketID, etc
    /// Centralizes queries for Attachments
    /// </summary>
    public class ConnectionContext : IDisposable
    {

        public const bool Enabled = false;

        public Data.LoginUser _loginUser { get; private set; }
        SqlConnection _connection;
        SqlTransaction _transaction;
        public DataContext _db { get; private set; }
        public OrganizationModel Organization { get; private set; }
        public UserSession User { get; private set; }

        private ConnectionContext(string connectionString, bool useTransaction = false)
        {
            _connection = new SqlConnection(connectionString);  // using
            _connection.Open(); // connection must be open to begin transaction

            _db = new DataContext(_connection);
            _db.ObjectTrackingEnabled = false;  // use linq read-only
            if (!useTransaction)
                return;

            _transaction = _connection.BeginTransaction();
            _db.Transaction = _transaction;
        }

        public ConnectionContext(Data.LoginUser user) : this(user.ConnectionString)
        {
            Organization = new OrganizationModel(this, user.OrganizationID);
            User = new UserSession(Organization, user.UserID);
        }

        public void Commit() { _db.Transaction.Commit(); }
        public void Rollback() { _db.Transaction.Rollback(); }

        //public OrganizationModel Organization(int organizationID)
        //{
        //    return new OrganizationModel(this, organizationID);
        //}

        public TicketModel Ticket(int ticketID)
        {
            return User.Ticket(ticketID);
        }

        public string AttachmentPath(int id)
        {
            string path = _db.ExecuteQuery<string>($"SELECT[Value] FROM FilePaths WITH(NOLOCK) WHERE ID = {id}").FirstOrDefault();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_db != null)
                _db.Dispose();

            if (_transaction != null)
                _transaction.Dispose();

            if (_connection != null)
                _connection.Dispose();
        }

        static bool _IsDebuggerAttached = Debugger.IsAttached;

        public static void LogMessage(Data.LoginUser user, Data.ActionLogType logType, Data.ReferenceType refType, int? ticketID, string message, EventLogEntryType type = EventLogEntryType.Information)
        {
            if (_IsDebuggerAttached)
            {
                Debug.WriteLine(message);   // debug output window (very fast)
                if (type == EventLogEntryType.Error)
                    Debugger.Break();   // something is wrong - fix the code!
            }

            Data.ActionLogs.AddActionLog(user, logType, refType, ticketID.HasValue ? ticketID.Value : 0, message);  // 0 if no TicketID?
        }

        public static void LogMessage(Data.LoginUser user, Data.ActionLogType logType, Data.ReferenceType refType, int? ticketID, string message, Exception e)
        {
            LogMessage(user, logType, refType, ticketID, message + e.ToString() + " ----- STACK: " + e.StackTrace.ToString(), EventLogEntryType.Error);
        }

    }
}
