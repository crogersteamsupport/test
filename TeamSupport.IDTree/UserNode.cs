using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class UserNode : IDNode
    {
        public int UserID { get; private set; }

        public UserNode(ConnectionContext connection, int userID) : base(connection)
        {
            UserID = userID;
            Verify();
        }

        public override void Verify()
        {
            //Verify($"SELECT UserID FROM Users WITH (NOLOCK) WHERE UserID={UserID} AND OrganizationID={Organization.OrganizationID}");
        }

        public bool AllowUserToEditAnyAction() { return IDReader.UserAllowUserToEditAnyAction(Connection._db, UserID); }

    }
}
