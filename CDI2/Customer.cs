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
        ICDIStrategy _iCdiStrategy;

        public Customer(OrganizationAnalysis organizationAnalysis)
        {
            _organizationAnalysis = organizationAnalysis;
            LoadClients();
        }

        public List<IntervalData> Intervals { get { return _organizationAnalysis.Intervals; } }

        void LoadClients()
        {
            TicketJoin[] allTickets = _organizationAnalysis.Tickets.Where(t => t.ClientOrganizationID.HasValue).ToArray();
            if (allTickets.Length == 0)
                return;

            Array.Sort(allTickets, (lhs, rhs) => lhs.ClientOrganizationID.Value.CompareTo(rhs.ClientOrganizationID.Value));
            _clients = new HashSet<Client>();

            // spin through each organization
            int startIndex = 0;
            int startId = allTickets[startIndex].ClientOrganizationID.Value;
            for (int i = 1; i < allTickets.Length; ++i)
            {
                if (allTickets[i].ClientOrganizationID != startId)
                {
                    // not a client of ourselves
                    if(startId != _organizationAnalysis.OrganizationID)
                        _clients.Add(new Client(new OrganizationAnalysis(_organizationAnalysis._dateRange, allTickets, startIndex, i)));
                    startIndex = i;
                    startId = allTickets[startIndex].ClientOrganizationID.Value;
                }
            }

            _clients.Add(new Client(new OrganizationAnalysis(_organizationAnalysis._dateRange, allTickets, startIndex, allTickets.Length)));
        }

        public void InvokeCDIStrategy()
        {
            _iCdiStrategy = new CDIPercentileStrategy(Intervals);
            _iCdiStrategy.CalculateCDI();    // test strategy with customer - TODO

            // run strategy against each client
            foreach (Client client in _clients)
                client.InvokeCDIStrategy(_iCdiStrategy);
        }

        public override string ToString()
        {
            return _organizationAnalysis.ToString();
        }
    }
}
