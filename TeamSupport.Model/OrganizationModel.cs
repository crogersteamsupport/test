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
        public ConnectionModel ConnectionModel { get; private set; }
        public int OrganizationID { get; private set; }
        public DataContext _db { get; private set; }
        //TicketTypeModel[] _ticketTypes; // contains TicketStatusModel[]

        public OrganizationModel(ConnectionModel user, int organizationID)
        {
            ConnectionModel = user;
            OrganizationID = organizationID;
            _db = user._db;
            Verify();
        }

        [Conditional("DEBUG")]
        void Verify()
        {
            string query = $"SELECT OrganizationID FROM Organizations  WITH (NOLOCK) WHERE OrganizationID={OrganizationID}";
            IEnumerable<int> x = _db.ExecuteQuery<int>(query);
            if (!x.Any())
                throw new Exception(String.Format($"{query} not found"));
        }

        public UserSession UserSession(int userID)
        {
            return new UserSession(this, userID);
        }

        public string AttachmentPath(int id)
        {
            string path = ConnectionModel.AttachmentPath(id);
            path = Path.Combine(Path.Combine(path, "Organizations"), OrganizationID.ToString());
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

    }
}
