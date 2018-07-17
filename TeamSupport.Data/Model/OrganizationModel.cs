using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace TeamSupport.Data.Model
{
    public class OrganizationModel
    {
        Model _model;
        public int OrganizationID { get; private set; }
        public DataContext _db { get; private set; }

        //TicketTypeModel[] _ticketTypes;
        //TicketStatusModel[] _ticketStatuses;

        public OrganizationModel(Model user, int organizationID)
        {
            _model = user;
            OrganizationID = organizationID;
            _db = user._db;

            // exists?
            string query = $"SELECT OrganizationID FROM Organizations  WITH (NOLOCK) WHERE OrganizationID={OrganizationID}";
            IEnumerable<int> x = _db.ExecuteQuery<int>(query);
            if (!x.Any())
                throw new Exception(String.Format($"{query} not found"));
        }

        public UserModel User(int userID)
        {
            return new UserModel(this, userID);
        }
    }
}
