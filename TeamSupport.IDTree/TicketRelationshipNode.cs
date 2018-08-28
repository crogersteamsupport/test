using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class TicketRelationshipNode : IDNode
    {
        TicketNode[] Ticket;
        public int TicketRelationshipID { get; private set; }

        public TicketRelationshipNode(TicketNode ticket1, TicketNode ticket2, int ticketRelationshipID) : base(ticket1)
        {
            Ticket = new TicketNode[2] { ticket1, ticket2 };
            TicketRelationshipID = ticketRelationshipID;
            Verify();
        }

        public override void Verify()
        {
            Verify($"SELECT TicketRelationshipID FROM TicketRelationships WITH(NOLOCK) " +
                $"WHERE TicketRelationshipID={TicketRelationshipID} AND Ticket1ID={Ticket[0].TicketID} AND Ticket2ID={Ticket[1].TicketID}");
        }
    }
}
