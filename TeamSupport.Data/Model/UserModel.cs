using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace TeamSupport.Data.Model
{
    public class UserModel
    {
        public OrganizationModel Organization { get; private set; }
        public int UserID { get; private set; }
        public DataContext _db { get; private set; }

        public UserModel(OrganizationModel organization, int userID)
        {
            Organization = organization;
            UserID = userID;
            _db = organization._db;

            // exists?
            string query = $"SELECT UserID FROM Users  WITH (NOLOCK) WHERE UserID={UserID} AND OrganizationID={Organization.OrganizationID}";
            IEnumerable<int> x = _db.ExecuteQuery<int>(query);
            if (!x.Any())
                throw new Exception(String.Format($"{query} not found"));
        }

        /// <summary>
        /// Trace creator/modifier by having user own tickets...
        /// </summary>
        public TicketModel Ticket(int ticketID)
        {
            return new TicketModel(this, ticketID);
        }
    }
}
