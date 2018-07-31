using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Model
{
    public class Customer : UserSession
    {
        /// <summary> OrganizationID and UserID come from ConnectionContext.Authentication </summary>
        public Customer(OrganizationModel organization) : base(organization)
        {
        }
    }
}
