using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    /// <summary> Link contact organizations and tickets </summary>
    public class OrganizationTicketNode : IDNode
    {
        public TicketNode Ticket { get; private set; }
        public OrganizationNode Organization { get; private set; }

        public OrganizationTicketNode(TicketNode ticket, OrganizationNode organization) : this(ticket, organization, true)
        {
        }

        private OrganizationTicketNode(TicketNode ticket, OrganizationNode organization, bool verify) : base(ticket)
        {
            Ticket = ticket;
            Organization = organization;
            if (verify)
                Verify();
        }

        public static OrganizationTicketNode[] GetOrganizationTickets(TicketNode ticket)
        {
            //$"Select Organizationid From OrganizationTickets WITH (NOLOCK) WHERE TicketId = {ticket.TicketID}"
            int[] customerIDs = IDReader.Read(TicketChild.Customers, ticket);
            OrganizationTicketNode[] organizationTickets = new OrganizationTicketNode[customerIDs.Length];
            for (int i = 0; i < customerIDs.Length; ++i)
                organizationTickets[i] = new OrganizationTicketNode(ticket, new OrganizationNode(ticket.Connection, customerIDs[i]), false);
            return organizationTickets;
        }

        public override void Verify()
        {
            Verify($"SELECT OrganizationID FROM OrganizationTickets WITH (NOLOCK) WHERE TicketID={Ticket.TicketID} AND OrganizationID={Organization.OrganizationID}");
        }
    }
}
