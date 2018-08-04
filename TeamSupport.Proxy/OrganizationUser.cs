using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Proxy
{
    public class OrganizationUser
    {
        public int UserID;
        public int OrganizationID;

        public OrganizationUser(int userID, int organizationID)
        {
            UserID = userID;
            OrganizationID = organizationID;
        }
    }
}
