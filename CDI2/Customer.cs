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
        OrganizationAnalysis _organizationAnalysis;
        HashSet<Client> _clients;
        ICDIStrategy _iCdiStrategy;

        public Customer(OrganizationAnalysis organizationAnalysis)
        {
            _organizationAnalysis = organizationAnalysis;
            _clients = new HashSet<Client>();
            LoadClients();
        }

        public List<IntervalData> Intervals { get { return _organizationAnalysis.Intervals; } }
        public int OrganizationID { get { return _organizationAnalysis.OrganizationID; } }
        public void WriteItervals() { _organizationAnalysis.WriteItervals(); }

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
                client.GenerateIntervals();
        }

        public void InvokeCDIStrategy()
        {
            _iCdiStrategy = new CustomerPercentileStrategy(Intervals, Callback, _organizationAnalysis.TicketCount);
            _iCdiStrategy.CalculateCDI();    // test strategy with customer - TODO
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
            Console.WriteLine("------------------------------------------");
            Console.WriteLine(_organizationAnalysis.OrganizationID);
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("ClientID\tClient\tActiveWeeks\tCDI\tCDI-2");
            foreach (Client client in _clients)
                client.WriteCdiByOrganization(_organizationAnalysis.Intervals[0]._timeStamp);
        }

        public void WriteIntervals()
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine(_organizationAnalysis.OrganizationID);
            Console.WriteLine("------------------------------------------");
            _organizationAnalysis.WriteItervals();
        }

        public void WriteIntervals(int clientID)
        {
            Debug.Write("ClientID\tClient\t");
            IntervalData.WriteHeader();

            Client client = _clients.Where(c => c.ClientOrganizationID.HasValue && (c.ClientOrganizationID == clientID)).FirstOrDefault();
            if (client != null)
                client.WriteIntervals();
        }

    }
}
