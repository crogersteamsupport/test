using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Linq;

namespace TeamSupport.Data.Model
{
    public class Model : IDisposable
    {
        SqlConnection _connection;
        public DataContext _db { get; private set; }

        public Model(string connectionString)
        {
            _connection = new SqlConnection(connectionString);  // using
            _connection.Open(); // connection must be open to begin transaction
            _db = new DataContext(_connection);
        }

        public OrganizationModel Organization(int organizationID)
        {
            return new OrganizationModel(this, organizationID);
        }

        public void SubmitChanges() { _db.SubmitChanges(); }

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
            {
                SubmitChanges();
                _db.Dispose();
            }

            if (_connection != null)
                _connection.Dispose();
        }
    }
}
