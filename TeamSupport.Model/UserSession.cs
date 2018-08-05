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
    class UserSession
    {
        public OrganizationModel Organization { get; private set; }
        public DataContext _db { get; private set; }

        public Proxy.AuthenticationModel Authentication { get { return Organization.Connection.Authentication; } }
        public int UserID { get { return Authentication.UserID; } }

        /// <summary> OrganizationID and UserID come from ConnectionContext.Authentication </summary>
        public UserSession(OrganizationModel organization)
        {
            Organization = organization;
            _db = organization._db;
            DataAPI.DataAPI.VerifyUser(_db, Organization.OrganizationID, UserID);
        }

        /// <summary>
        /// Trace creator/modifier by having user own tickets...
        /// </summary>
        public TicketModel Ticket(int ticketID)
        {
            return new TicketModel(this, ticketID);
        }

        public bool AllowUserToEditAnyAction() { return DataAPI.DataAPI.UserAllowUserToEditAnyAction(_db, UserID); }
        public bool CanEdit() { return Authentication.IsSystemAdmin || AllowUserToEditAnyAction(); }

    }
}
