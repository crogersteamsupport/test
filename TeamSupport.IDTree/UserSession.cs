using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics;

namespace TeamSupport.IDTree
{
    /// <summary>
    /// Wrapper for valid UserID
    /// </summary>
    public class UserSession
    {
        public OrganizationNode Organization { get; private set; }
        public UserNode User { get; private set; }
        Proxy.AuthenticationModel _authentication;

        public int UserID { get { return _authentication.UserID; } }

        /// <summary> OrganizationID and UserID come from ConnectionContext.Authentication </summary>
        public UserSession(OrganizationNode organization) : base(organization.Request)
        {
            Organization = organization;
            _authentication = Organization.Request.Authentication;
            //DBReader.VerifyUser(_db, Organization.OrganizationID, UserID);     // connection already verified
        }

        public UserSession(int userID) : base((ClientRequest)null)
        {
        }

        /// <summary>
        /// Trace creator/modifier by having user own tickets...
        /// </summary>
        public TicketNode Ticket(int ticketID)
        {
            return new TicketNode(this, ticketID);
        }

        public bool AllowUserToEditAnyAction() { return IDReader.UserAllowUserToEditAnyAction(Request._db, UserID); }
        public bool CanEdit() { return _authentication.IsSystemAdmin || AllowUserToEditAnyAction(); }

        public override void Verify()
        {
            Verify($"SELECT UserID FROM Users WITH (NOLOCK) WHERE UserID={UserID} AND OrganizationID={Organization.OrganizationID}");
        }
    }
}
