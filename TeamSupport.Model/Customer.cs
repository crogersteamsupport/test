using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Model
{
    class Customer : UserSession
    {
        public Customer(OrganizationModel organization, int userID) : base(organization, userID)
        {
        }
    }
}
