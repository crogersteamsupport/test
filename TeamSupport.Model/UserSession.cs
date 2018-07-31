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
    public class UserSession
    {
        public OrganizationModel Organization { get; private set; }
        public DataContext _db { get; private set; }

        public AuthenticationModel Authentication { get { return Organization.Connection.Authentication; } }
        public int UserID { get { return Authentication.UserID; } }

        /// <summary> OrganizationID and UserID come from ConnectionContext.Authentication </summary>
        public UserSession(OrganizationModel organization)
        {
            Organization = organization;
            _db = organization._db;
            Verify();
        }

        [Conditional("DEBUG")]
        void Verify()
        {
            string query = $"SELECT UserID FROM Users WITH (NOLOCK) WHERE UserID={UserID} AND OrganizationID={Organization.OrganizationID}";
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

        public bool AllowUserToEditAnyAction() { return _db.ExecuteQuery<bool>($"SELECT AllowUserToEditAnyAction FROM Users WITH (NOLOCK) WHERE UserID={UserID}").First(); }
        public bool CanEdit() { return Authentication.IsSystemAdmin || AllowUserToEditAnyAction(); }

        //FullName _fullName;
        //class FullName
        //{
        //    public string FirstName;
        //    public string LastName;
        //}
        //public string CreatorName
        //{
        //    get
        //    {
        //        if (_fullName == null)
        //        {
        //            string query = $"SELECT FirstName, LastName FROM Users  WITH (NOLOCK) WHERE UserID={UserID} AND OrganizationID={Organization.OrganizationID}";
        //            _fullName = _db.ExecuteQuery<FullName>(query).First();  // throws if it fails
        //        }
        //        return $"{_fullName.FirstName} {_fullName.LastName}";
        //    }
        //}

    }
}
