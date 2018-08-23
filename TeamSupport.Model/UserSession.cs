using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics;

namespace TeamSupport.Model
{
    /// <summary>
    /// Wrapper for valid UserID
    /// </summary>
    public class UserSession : IModel
    {
        public OrganizationModel Organization { get; private set; }
        public ConnectionContext Connection { get; private set; }
        Proxy.AuthenticationModel _authentication;

        public int UserID { get { return _authentication.UserID; } }

        /// <summary> OrganizationID and UserID come from ConnectionContext.Authentication </summary>
        public UserSession(OrganizationModel organization)
        {
            Organization = organization;
            Connection = organization.Connection;
            _authentication = Organization.Connection.Authentication;
            //DBReader.VerifyUser(_db, Organization.OrganizationID, UserID);     // connection already verified
        }

        /// <summary>
        /// Trace creator/modifier by having user own tickets...
        /// </summary>
        public TicketModel Ticket(int ticketID)
        {
            return new TicketModel(this, ticketID);
        }

        public bool AllowUserToEditAnyAction() { return DBReader.UserAllowUserToEditAnyAction(Connection._db, UserID); }
        public bool CanEdit() { return _authentication.IsSystemAdmin || AllowUserToEditAnyAction(); }

    }
}
