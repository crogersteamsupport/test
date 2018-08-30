using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSupport.Data;

namespace TeamSupport.IDTree
{
    public class UserModel : IDNode
    {
        public OrganizationModel Organization { get; private set; }
        public int UserID { get; private set; }

        public UserModel(ConnectionContext connection) : this(connection.Organization, connection.Authentication.UserID, false)
        {
        }

        private UserModel(OrganizationModel organization, int userID, bool verify) : base(organization)
        {
            Organization = organization;
            UserID = userID;
            if(verify)
                Verify();
        }

        public UserModel(ConnectionContext connection, int userID) : base(connection)
        {
            UserID = userID;
            int organizationID = ExecuteQuery<int>($"SELECT OrganizationID FROM Users WITH (NOLOCK) WHERE UserID={userID}").First();
            Organization = new OrganizationModel(connection, organizationID);
        }

        // slow :(
        public UserProxy UserProxy()
        {
            return ExecuteQuery<UserProxy>($"SELECT * FROM Users WHERE UserID={UserID}").First();
        }

        public override void Verify()
        {
            Verify($"SELECT UserID FROM Users WITH (NOLOCK) WHERE UserID={UserID} AND OrganizationID={Organization.OrganizationID}");
        }

        public bool AllowUserToEditAnyAction()
        {
            return ExecuteQuery<bool>($"SELECT AllowUserToEditAnyAction FROM Users WITH (NOLOCK) WHERE UserID={UserID}").First();
        }

        public bool MarkDeleted()
        {
            return ExecuteQuery<bool>($"SELECT MarkDeleted FROM Users WITH (NOLOCK) WHERE UserID={UserID}").First();
        }

    }
}
