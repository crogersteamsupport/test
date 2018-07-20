using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Linq;
using System.IO;

namespace TeamSupport.Data.Model
{
    /// <summary>
    /// Model for validating OrganizationID, UserID, TicketID, etc
    /// Centralizes queries for Attachments
    /// </summary>
    public class ConnectionModel : IDisposable
    {
        SqlConnection _connection;
        public DataContext _db { get; private set; }

        public ConnectionModel(string connectionString)
        {
            _connection = new SqlConnection(connectionString);  // using
            _connection.Open(); // connection must be open to begin transaction
            _db = new DataContext(_connection);
            _db.ObjectTrackingEnabled = false;  // use linq read-only
        }

        public OrganizationModel Organization(int organizationID)
        {
            return new OrganizationModel(this, organizationID);
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

            if (_connection != null)
                _connection.Dispose();
        }


    }
}
