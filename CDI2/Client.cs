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

        public int? ClientOrganizationID { get { return _organizationAnalysis.ClientOrganizationID; } }

        public void InvokeCDIStrategy(ICDIStrategy customerStrategy)
        {
            _iCdiStrategy = new ClientPercentileStrategy(_organizationAnalysis.Intervals, customerStrategy, _organizationAnalysis.TicketCount);
            _iCdiStrategy.CalculateCDI();
            //if (_iCdiStrategy.CalculateCDI())
            //    Debug.WriteLine(String.Format("\t{0}", ClientOrganizationID.Value));
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
            Organization organization;
            if(Organization.TryGet(clientID, out organization))
                Debug.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}", clientID, organization.Name, _organizationAnalysis.Intervals.Count, organization.CustDisIndex, last.CDI));
        }

        public void WriteItervalData()
        {
            int clientID = _organizationAnalysis.ClientOrganizationID.Value;
            Organization organization;
            if (!Organization.TryGet(clientID, out organization))
                return;

            string clientName = organization.Name;
            foreach (IntervalData interval in _organizationAnalysis.Intervals)
                Debug.WriteLine(String.Format("{0}\t{1}\t{2}", clientID, clientName, interval.ToString()));
        }

    }
}