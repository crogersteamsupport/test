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
        public OrganizationAnalysis _organizationAnalysis { get; private set; }
        ICDIStrategy _iCdiStrategy;

        public Client(OrganizationAnalysis organizationAnalysis)
        {
            _organizationAnalysis = organizationAnalysis;
        }

        public void GenerateIntervals()
        {
            //List<IntervalData> intervals = _organizationAnalysis.Intervals;
            _organizationAnalysis.GenerateIntervals();
        }

        public IntervalData GetRawData()
        {
            IntervalData last = _organizationAnalysis.Intervals.Last();
            IntervalData value = new IntervalData()
            {
                _timeStamp = last._timeStamp,
                _totalTicketsCreated = last._totalTicketsCreated, // 1. Total Tickets Created
                _newCount = last._newCount, // 2. Tickets Created in Last 30
                _openCount = last._openCount,   // 3. Number of Tickets Currently Open
                _medianDaysOpen = last._medianDaysOpen, // 4. Average Time Tickets have been open
                _medianDaysToClose = IntervalData.MedianTotalDaysToClose(_organizationAnalysis.Tickets),  // 5. Average Time tickets took to close
            };

            return value;
        }

        public void CalculateCDI(IntervalPercentiles allClientsPercentiles)
        {
            allClientsPercentiles.CalculateCDI(_organizationAnalysis.Intervals.Last());
        }

        public void InvokeCDIStrategy(ICDIStrategy customerStrategy)
        {
            _iCdiStrategy = new ClientPercentileStrategy(_organizationAnalysis.Intervals, customerStrategy);
            _iCdiStrategy.CalculateCDI();
            //if (_iCdiStrategy.CalculateCDI())
            //    CDIEventLog.WriteLine(String.Format("\t{0}", ClientOrganizationID.Value));
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
            if (Organization.TryGet(clientID, out organization))
            {
                //CDIEventLog.Write("ClientID\tClient\tCreatedLast30\tTotalTicketsCreated\tCDI\tnew\ttotal\tCDI-2\t");
                CDIEventLog.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", clientID, organization.Name, 
                    organization.CreatedLast30, organization.TotalTicketsCreated, organization.CustDisIndex, 
                    last._newCount, last._totalTicketsCreated, last.CDI));
                //CDIEventLog.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", clientID, organization.Name, _organizationAnalysis.Intervals.Count, organization.CustDisIndex, last.CDI, last.ToString()));
            }
        }

        public void WriteIntervals()
        {
            int clientID = _organizationAnalysis.ClientOrganizationID.Value;
            Organization organization;
            if (!Organization.TryGet(clientID, out organization))
                return;

            string clientName = organization.Name;
            foreach (IntervalData interval in _organizationAnalysis.Intervals)
                CDIEventLog.WriteLine(String.Format("{0}\t{1}\t{2}", clientID, clientName, interval.ToString()));
        }

    }
}