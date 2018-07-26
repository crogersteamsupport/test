using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Model
{
    class Client : UserSession
    {
        public Client(OrganizationModel organization, int userID) : base(organization, userID)
        {
        }
    }
}
