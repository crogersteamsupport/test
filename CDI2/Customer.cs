using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.CDI
{
    class Customer
    {
        OrganizationAnalysis _organizationAnalysis;
        HashSet<Client> _clients;
        ICDIStrategy _cdiStrategy;

        public Customer(OrganizationAnalysis organizationAnalysis)
        {
            _organizationAnalysis = organizationAnalysis;
            _clients = new HashSet<Client>();
            _cdiStrategy = new CDIPercentileStrategy(_organizationAnalysis.Intervals);
        }

        public void InvokeCDIStrategy()
        {
            _cdiStrategy.CalculateCDI();
        }

    }
}
