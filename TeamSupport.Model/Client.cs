using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Model
{
    public class Client : UserSession
    {
        public Customer Customer { get; private set; }
        public Client(OrganizationModel organization, int userID) : base(organization, userID)
        {
        }
    }
}
