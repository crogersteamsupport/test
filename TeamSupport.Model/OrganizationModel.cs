using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO;
using System.Diagnostics;

namespace TeamSupport.Model
{
    /// <summary>
    /// Wrapper for valid OrganizationID
    /// </summary>
    public class OrganizationModel
    {
        public ConnectionContext Connection { get; private set; }
        public DataContext _db { get; private set; }

        public int OrganizationID { get { return Connection.Authentication.OrganizationID; } }

        /// <summary> OrganizationID and UserID come from ConnectionContext.Authentication </summary>
        public OrganizationModel(ConnectionContext connection)
        {
            Connection = connection;
            _db = connection._db;
            Data.DataAPI.VerifyOrganization(_db, OrganizationID);
        }

        /// <summary> UserID comes from ConnectionContext.Authentication </summary>
        public UserSession User()
        {
            return new UserSession(this);
        }

        public string AttachmentPath(int id)
        {
            string path = Connection.AttachmentPath(id);
            path = Path.Combine(Path.Combine(path, "Organizations"), OrganizationID.ToString());
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

    }
}
