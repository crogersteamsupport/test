using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class TicketTaskNode : IDNode
    {
        public TicketNode Ticket { get; private set; }
        public int TaskID { get; private set; }
        public TicketTaskNode(TicketNode ticket, int taskID) : base(ticket)
        {
            Ticket = ticket;
            TaskID = taskID;
        }

        public override void Verify()
        {
            Verify($"SELECT TaskID FROM TaskAssociations WITH (NOLOCK) WHERE TaskID={TaskID} AND Refid={Ticket.TicketID} and RefType = 17");
        }
    }
}
