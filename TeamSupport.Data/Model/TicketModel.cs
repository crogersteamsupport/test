using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace TeamSupport.Data.Model
{
    public class TicketModel
    {
        public UserModel User { get; private set; }
        public int TicketID { get; private set; }
        public DataContext _db { get; private set; }

        public TicketModel(UserModel user, int ticketID)
        {
            User = user;
            _db = User._db;
            TicketID = ticketID;

            // exists?
            string query = $"SELECT TicketID FROM Tickets  WITH (NOLOCK) WHERE TicketID={ticketID} AND OrganizationID={User.Organization.OrganizationID}";
            IEnumerable<int> x = _db.ExecuteQuery<int>(query);
            if (!x.Any())
                throw new Exception(String.Format($"{query} not found"));
        }

        /// <summary> Existing Action </summary>
        public ActionModel Action(int actionID)
        {
            return new ActionModel(this, actionID);
        }

        /// <summary> Create new Action on an existing ticket </summary>
        public ActionModel InsertAction(LoginUser loginUser, ActionProxy proxy)
        {
            return new ActionModel(this, loginUser, proxy);
        }

        /// <summary> Create new Action on new ticket </summary>
        public ActionModel InsertAction(ActionProxy info, Ticket ticketData, User user)
        {
            return new ActionModel(this, info, ticketData, user);
        }

    }
}
