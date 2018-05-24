using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Configuration;

namespace TeamSupport.CDI
{
    class Customer
    {
        OrganizationAnalysis _organizationAnalysis;
        HashSet<Client> _clients;
        IntervalPercentiles _percentiles;
        //ICDIStrategy _iCdiStrategy;

        public Customer(OrganizationAnalysis organizationAnalysis)
        {
            _organizationAnalysis = organizationAnalysis;
            _clients = new HashSet<Client>();
            LoadClients();
        }

        public int OrganizationID { get { return _organizationAnalysis.ParentID; } }


        /// <summary>
        /// ParentID = Customer, OrganizationID = Client
        /// </summary>
        void LoadClients()
        {
            TicketJoin[] allTickets = _organizationAnalysis.Tickets;//.Where(t => t.OrganizationID).ToArray();
            if (allTickets.Length == 0)
                return;

            Array.Sort(allTickets, (lhs, rhs) => lhs.OrganizationID.CompareTo(rhs.OrganizationID));

            // spin through each organization
            int startIndex = 0;
            int startId = allTickets[startIndex].OrganizationID;
            for (int i = 1; i < allTickets.Length; ++i)
            {
                if (allTickets[i].OrganizationID != startId)
                {
                    // not a client of ourselves
                    if (startId != _organizationAnalysis.OrganizationID)
                        _clients.Add(new Client(new OrganizationAnalysis(_organizationAnalysis._dateRange, allTickets, startIndex, i)));
                    startIndex = i;
                    startId = allTickets[startIndex].OrganizationID;
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

        linq.CDI_Settings _weights;

        public linq.CDI_Settings GetCDISettings()
        {
            if (_weights != null)
                return _weights;

            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    Table<linq.CDI_Settings> table = db.GetTable<linq.CDI_Settings>();
                    _weights = Enumerable.ToArray(table).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                CDIEventLog.WriteEntry("CDI_Settings Read failed", e);
            }

            if (_weights == null)
            {
                const float equalWeight = 1/5;
                _weights = new linq.CDI_Settings()
                {
                    TotalTicketsWeight = equalWeight,
                    OpenTicketsWeight = equalWeight,
                    Last30Weight = equalWeight,
                    AvgDaysOpenWeight = equalWeight,
                    AvgDaysToCloseWeight = equalWeight
                };
            }
            return _weights;
        }

        public void InvokeCDIStrategy()
        {
            // calculate the percentiles for the client metrics
            List<IntervalData> clientIntervals = new List<IntervalData>();
            foreach (Client client in _clients)
            {
                IntervalData current = client.GetCDIIntervalData();
                if(current != null)
                    clientIntervals.Add(current);
            }
            _percentiles = new IntervalPercentiles(clientIntervals);

            // construct the customer strategy
            //_iCdiStrategy = new CustomerPercentileStrategy(clientIntervals, Callback);
            //_iCdiStrategy.CalculateCDI();    // customer CDI not used

            // calculate the CDI for each client
            foreach (Client client in _clients)
                client.InvokeCDIStrategy(_percentiles, GetCDISettings());
        }

        //public void Callback()
        //{
        //    if (_clients == null)
        //        return;

        //    // run strategy against each client
        //    foreach (Client client in _clients)
        //        client.InvokeCDIStrategy(_iCdiStrategy);
        //}

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
            //CDIEventLog.Write("ClientID\tClient\t");
            //IntervalData.WriteHeader();

            //Client client = _clients.Where(c => c.ClientOrganizationID.HasValue && (c.ClientOrganizationID == clientID)).FirstOrDefault();
            //if (client != null)
            //    client.WriteIntervals();
        }

        public void WriteClients()
        {
            CDIEventLog.WriteLine("ClientID\tClient\tTotalTicketsCreated\tTicketsOpen\tCreatedLast30\tAvgTimeOpen\tAvgTimeToClose\tCustDisIndex\tDate\tTotalTicketsCreated\tTicketsOpen\tCreatedLast30\tAvgTimeOpen\tAvgTimeToClose\tCustDisIndex-2" +
                "CDI-2");
            foreach (Client client in _clients)
                client.Write();
        }

        public void Save(linq.CDI[] cdi2s, Table<linq.CDI> table)
        {
            foreach (Client client in _clients)
            {
                linq.CDI cdi2 = cdi2s.Where(c => c.OrganizationID == client.OrganizationID).FirstOrDefault();
                client.Save(cdi2, table);
            }
        }

    }
}
