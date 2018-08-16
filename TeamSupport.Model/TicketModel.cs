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
        public DataContext _db { get; private set; }

        int? _ticketNumber;
        public int TicketNumber
        {
            get
            {
                if (!_ticketNumber.HasValue)
                    _ticketNumber = DBReader.TicketNumber(_db,TicketID);
                return _ticketNumber.Value;
            }
        }

        public TicketModel(UserSession user, int ticketID)
        {
            User = user;
            _db = User._db;
            TicketID = ticketID;
            DBReader.VerifyTicket(_db, User.Organization.OrganizationID, TicketID);
        }

        /// <summary> Existing Data.Action </summary>
        public ActionModel Action(int actionID)
        {
            return new ActionModel(this, actionID);
        }

    }
}
