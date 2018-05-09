using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.CDI
{
    class Client
    {
        OrganizationAnalysis _organizationAnalysis;

        public Client(OrganizationAnalysis organizationAnalysis)
        {
            _organizationAnalysis = organizationAnalysis;
        }

        public override string ToString()
        {
            return _organizationAnalysis.ToString();
        }
    }
}