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
        Dictionary<int, Client> _clients;
        ICDIStrategy _cdiStrategy;

        public Customer(OrganizationAnalysis organizationAnalysis)
        {
            _organizationAnalysis = organizationAnalysis;
            AnalyzeClients();
        }

        void AnalyzeClients()
        {
            TicketJoin[] allTickets = _organizationAnalysis.Tickets.Where(t => t.ClientID.HasValue).ToArray();

            Array.Sort(allTickets, (lhs, rhs) => lhs.ClientID.Value.CompareTo(rhs.ClientID.Value));
            _clients = new Dictionary<int, Client>();

            // spin through each organization
            int startIndex = 0;
            int startId = allTickets[startIndex].ClientID.Value;
            for (int i = 1; i < allTickets.Length; ++i)
            {
                if (allTickets[i].ClientID != startId)
                {
                    // not a client of ourselves
                    if(startId != _organizationAnalysis.OrganizationID)
                        _clients[startId] = new Client(new OrganizationAnalysis(_organizationAnalysis._dateRange, allTickets, startIndex, i));
                    startIndex = i;
                    startId = allTickets[startIndex].ClientID.Value;
                }
            }

            _clients[startId] = new Client(new OrganizationAnalysis(_organizationAnalysis._dateRange, allTickets, startIndex, allTickets.Length));
        }

        public void InvokeCDIStrategy()
        {
            _cdiStrategy = new CDIPercentileStrategy(_organizationAnalysis.Intervals);
            _cdiStrategy.CalculateCDI();
        }

        public override string ToString()
        {
            return _organizationAnalysis.ToString();
        }
    }
}
