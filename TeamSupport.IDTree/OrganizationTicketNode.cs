using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class OrganizationTicketNode : IDNode
    {
        public TicketNode Ticket { get; private set; }
        public int OrganizationID { get; private set; }
        public OrganizationNode Organization { get; private set; }

        public OrganizationTicketNode(TicketNode ticket, int userID) : this(ticket, userID, true)
        {
        }

        private OrganizationTicketNode(TicketNode ticket, int organizationID, bool verify) : base(ticket.Request)
        {
            Ticket = ticket;
            OrganizationID = organizationID;
            //if (verify)
            //    DBReader.VerifySubscription(Connection._db, ticket.User.Organization.OrganizationID, Ticket.TicketID, UserID);
        }

        public static OrganizationTicketNode[] GetClients(TicketNode ticketModel)
        {
            int[] customerIDs = IDReader.Read(TicketChild.Customers, ticketModel);
            OrganizationTicketNode[] customers = new OrganizationTicketNode[customerIDs.Length];
            for (int i = 0; i < customerIDs.Length; ++i)
                customers[i] = new OrganizationTicketNode(ticketModel, customerIDs[i], false);
            return customers;
        }

        public override void Verify()
        {
            Verify($"SELECT OrganizationID FROM OrganizationTickets WITH (NOLOCK) WHERE TicketID={Ticket.TicketID} AND OrganizationID={OrganizationID}");
        }
    }
}
