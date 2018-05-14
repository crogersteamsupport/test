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
        ICDIStrategy _iCdiStrategy;

        public Client(OrganizationAnalysis organizationAnalysis)
        {
            _organizationAnalysis = organizationAnalysis;
        }

        public void InvokeCDIStrategy(ICDIStrategy iCdiStrategy)
        {
            _iCdiStrategy = iCdiStrategy;
        }

        public override string ToString()
        {
            return _organizationAnalysis.ToString();
        }
    }
}