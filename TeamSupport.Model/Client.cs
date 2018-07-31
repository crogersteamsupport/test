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

        /// <summary> OrganizationID and UserID come from ConnectionContext.Authentication </summary>
        public Client(OrganizationModel organization) : base(organization)
        {
        }
    }
}
