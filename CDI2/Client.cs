using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TeamSupport.CDI.linq;

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

        public void InvokeCDIStrategy(ICDIStrategy customerStrategy)
        {
            _iCdiStrategy = new ClientPercentileStrategy(_organizationAnalysis.Intervals, customerStrategy);
            _iCdiStrategy.CalculateCDI();
        }

        public override string ToString()
        {
            return _organizationAnalysis.ToString();
        }


        public void WriteCdiByOrganization(DateTime timestamp)
        {
            if (!_organizationAnalysis.ClientOrganizationID.HasValue)
                return;

            IntervalData last = _organizationAnalysis.Intervals.Last();
            if (last._timeStamp < timestamp.AddDays(-90))
                return; // too old...

            int clientID = _organizationAnalysis.ClientOrganizationID.Value;
            Debug.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}", clientID, Organization.GetOrganizationName(clientID), _organizationAnalysis.Intervals.Count, last.CDI));
        }
    }
}