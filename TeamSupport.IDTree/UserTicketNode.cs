using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class UserTicketNode : IDNode
    {
        public TicketNode Ticket { get; private set; }
        public UserSession User { get; private set; }

        public UserTicketNode(TicketNode ticket, UserSession user) : this(ticket, user, true)
        {
        }

        private UserTicketNode(TicketNode ticket, UserSession user, bool verify) : base(ticket.Request)
        {
            Ticket = ticket;
            User = user;
            if (verify)
                Verify();
        }

        public static UserTicketNode[] GetContacts(TicketNode ticketModel)
        {
            int[] contactIDs = IDReader.Read(TicketChild.Contacts, ticketModel);
            UserTicketNode[] contacts = new UserTicketNode[contactIDs.Length];
            for (int i = 0; i < contactIDs.Length; ++i)
                contacts[i] = new UserTicketNode(ticketModel, new UserSession(contactIDs[i]), false);
            return contacts;
        }

        public override void Verify()
        {
            
        }
    }
}
