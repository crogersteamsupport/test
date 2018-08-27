using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class TicketTaskNode : IDNode
    {
        public TicketTaskNode TicketTask { get; private set; }
        public TicketNode Ticket { get; private set; }
        public TicketTaskNode(TicketNode ticket, TicketTaskNode ticketTask) : base(ticket)
        {
            Ticket = ticket;
            TicketTask = TicketTask;
        }

        public override void Verify()
        {

        }
    }
}
