using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics;
using TeamSupport.Data;

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

        public TicketModel(UserSession user, int ticketID)
        {
            User = user;
            Connection = User.Connection;
            TicketID = ticketID;
            DBReader.VerifyTicket(Connection._db, User.Organization.OrganizationID, TicketID);
        }

        /// <summary> Existing Data.Action </summary>
        public ActionModel Action(int actionID)
        {
            return new ActionModel(this, actionID);
        }

    }
}
