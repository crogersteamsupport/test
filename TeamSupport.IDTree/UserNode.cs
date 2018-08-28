using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class UserNode : IDNode
    {
        public OrganizationNode Organization { get; private set; }
        public int UserID { get; private set; }

        public UserNode(OrganizationNode organization, int userID) : base(organization)
        {
            Organization = organization;
            UserID = userID;
            Verify();
        }

        public UserNode(ConnectionContext connection, int userID) : base(connection)
        {
            UserID = userID;
            int organizationID = Connection._db.ExecuteQuery<int>($"SELECT OrganizationID FROM Users WITH (NOLOCK) WHERE UserID={userID}").First();
            Organization = new OrganizationNode(connection, organizationID);
        }

        public override void Verify()
        {
            Verify($"SELECT UserID FROM Users WITH (NOLOCK) WHERE UserID={UserID} AND OrganizationID={Organization.OrganizationID}");
        }

        public bool AllowUserToEditAnyAction()
        {
            return Connection._db.ExecuteQuery<bool>($"SELECT AllowUserToEditAnyAction FROM Users WITH (NOLOCK) WHERE UserID={UserID}").First();
        }

    }
}
