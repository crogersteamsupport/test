using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TeamSupport.CDI
{
    class Customer
    {
        public OrganizationAnalysis _organizationAnalysis { get; private set; }
        HashSet<Client> _clients;
        ICDIStrategy _iCdiStrategy;
        List<IntervalData> _clientIntervals;

        public Customer(OrganizationAnalysis organizationAnalysis)
        {
            _organizationAnalysis = organizationAnalysis;
            _clients = new HashSet<Client>();
            _clientIntervals = new List<IntervalData>();

            LoadClients();
        }

        void LoadClients()
        {
            TicketJoin[] allTickets = _organizationAnalysis.Tickets.Where(t => t.ClientOrganizationID.HasValue).ToArray();
            if (allTickets.Length == 0)
                return;

            Array.Sort(allTickets, (lhs, rhs) => lhs.ClientOrganizationID.Value.CompareTo(rhs.ClientOrganizationID.Value));

            // spin through each organization
            int startIndex = 0;
            int startId = allTickets[startIndex].ClientOrganizationID.Value;
            for (int i = 1; i < allTickets.Length; ++i)
            {
                if (allTickets[i].ClientOrganizationID != startId)
                {
                    // not a client of ourselves
                    if (startId != _organizationAnalysis.ClientOrganizationID)
                        _clients.Add(new Client(new OrganizationAnalysis(_organizationAnalysis._dateRange, allTickets, startIndex, i)));
                    startIndex = i;
                    startId = allTickets[startIndex].ClientOrganizationID.Value;
                }
            }

            _clients.Add(new Client(new OrganizationAnalysis(_organizationAnalysis._dateRange, allTickets, startIndex, allTickets.Length)));
        }

        public void GenerateIntervals()
        {
            _organizationAnalysis.GenerateIntervals();
            foreach (Client client in _clients)
            {
                client.GenerateIntervals();
            }

            // roll up the clients into the customer distribution

        }

        public void InvokeCDIStrategy()
        {
            foreach (Client client in _clients)
            {
                IntervalData interval = client.GetRawData();
                if (interval._timeStamp > _organizationAnalysis._dateRange.EndDate.AddDays(-90)) // ignore data older than 90 days
                    _clientIntervals.Add(interval);
            }

            IntervalPercentiles clientPercentiles = new IntervalPercentiles(_clientIntervals);

            _iCdiStrategy = new CustomerPercentileStrategy(_clientIntervals, Callback);
            //_iCdiStrategy.CalculateCDI();    // test strategy with customer - TODO

            foreach(Client client in _clients)
            {
                client.CalculateCDI(clientPercentiles);
            }
        }

        public void Callback()
        {
            if (_clients == null)
                return;

            // run strategy against each client
            foreach (Client client in _clients)
                client.InvokeCDIStrategy(_iCdiStrategy);
        }

        public override string ToString()
        {
            return _organizationAnalysis.ToString();
        }

        public void WriteCdiByOrganization()
        {
            CDIEventLog.WriteLine("------------------------------------------");
            CDIEventLog.WriteLine(_organizationAnalysis.OrganizationID.ToString());
            CDIEventLog.WriteLine("------------------------------------------");
            //CDIEventLog.Write("ClientID\tClient\tActiveWeeks\tCDI\tCDI-2\t");

            CDIEventLog.Write("ClientID\tClient\tCreatedLast30\tTotalTicketsCreated\tCDI\tnew\ttotal\tCDI-2\t");

            IntervalData.WriteHeader();
            foreach (Client client in _clients)
                client.WriteCdiByOrganization(_organizationAnalysis.Intervals[0]._timeStamp);
        }

        public void WriteIntervals()
        {
            CDIEventLog.WriteLine("------------------------------------------");
            CDIEventLog.WriteLine(_organizationAnalysis.OrganizationID.ToString());
            CDIEventLog.WriteLine("------------------------------------------");
            _organizationAnalysis.WriteItervals();
        }

        public void WriteIntervals(int clientID)
        {
            CDIEventLog.Write("ClientID\tClient\t");
            IntervalData.WriteHeader();

            Client client = _clients.Where(c => c._organizationAnalysis.ClientOrganizationID.HasValue && (c._organizationAnalysis.ClientOrganizationID == clientID)).FirstOrDefault();
            if (client != null)
                client.WriteIntervals();
        }

        public void WriteClients()
        {
            foreach (Client client in _clients)
            {
                IntervalData interval = client._organizationAnalysis.Intervals.First();
            }
        }

    }
}
