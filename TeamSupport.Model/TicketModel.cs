using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics;
using TeamSupport.Data;
using System.Web;

namespace TeamSupport.Model
{
    /// <summary>
    /// Wrapper for valid TicketID
    /// </summary>
    public class TicketModel
    {
        public UserSession User { get; private set; }
        public int TicketID { get; private set; }
        public ConnectionContext Connection { get; private set; }

        int? _ticketNumber;
        public int TicketNumber
        {
            get
            {
                if (!_ticketNumber.HasValue)
                    _ticketNumber = DBReader.TicketNumber(Connection._db,TicketID);
                return _ticketNumber.Value;
            }
        }

        public TicketModel(UserSession user, int ticketID)
        {
            User = user;
            Connection = User.Connection;
            TicketID = ticketID;
            DBReader.VerifyTicket(Connection._db, User.Organization.OrganizationID, TicketID);
        }

        /// <summary> bottom up - existing action </summary>
        public TicketModel(ConnectionContext connection, int ticketID)
        {
            Connection = connection;
            User = Connection.User;
            TicketID = ticketID;
            DBReader.VerifyTicket(Connection._db, Connection.Organization.OrganizationID, TicketID);
        }

        /// <summary> Existing Data.Action </summary>
        public ActionModel Action(int actionID)
        {
            return new ActionModel(this, actionID);
        }

        //public Data.TicketProxy AsTicketProxy(HttpRequest request, TicketModel ticketModel)
        //{
           
        //}

    }
}
