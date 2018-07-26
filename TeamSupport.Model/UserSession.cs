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
        public int UserID { get; private set; }
        public DataContext _db { get; private set; }

        public UserSession(OrganizationModel organization, int userID)
        {
            Organization = organization;
            UserID = userID;
            _db = organization._db;
            Verify();
        }

        [Conditional("DEBUG")]
        void Verify()
        {
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


        /// <summary> Log that this user did something... </summary>
        public void AddActionLog(Data.ActionLogType actionLogType, Data.ReferenceType refType, int refID, string description)
        {
            string query = String.Empty;
            switch (actionLogType)
            {
                case Data.ActionLogType.Insert:
                    query = "INSERT INTO ActionLogs(OrganizationID, RefType, RefID, Data.ActionLogType, [Description], DateCreated, CreatorID)" +
                        $"VALUES ({Organization.OrganizationID}, {refType}, {refID}, {(int)actionLogType}, {0}, {DateTime.UtcNow}, {UserID})";
                    break;
                case Data.ActionLogType.Update:
                case Data.ActionLogType.Delete:
                    query = "INSERT INTO ActionLogs(OrganizationID, RefType, RefID, Data.ActionLogType, [Description], DateModified, ModifierID)" +
                        $"VALUES ({Organization.OrganizationID}, {refType}, {refID}, {(int)actionLogType}, {0}, {DateTime.UtcNow}, {UserID})";
                    break;
            }
            _db.ExecuteCommand(query, description); // sql injection checking of description
        }
    }
}
