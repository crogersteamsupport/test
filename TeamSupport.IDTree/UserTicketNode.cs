using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    /// <summary> Link contacts and tickets </summary>
    public class UserTicketNode : IDNode
    {
        public TicketNode Ticket { get; private set; }
        public UserNode Contact { get; private set; }

        public UserTicketNode(TicketNode ticket, UserNode contact) : this(ticket, contact, true)
        {
        }

        private UserTicketNode(TicketNode ticket, UserNode contact, bool verify) : base(ticket)
        {
            Ticket = ticket;
            Contact = contact;
            if (verify)
                Verify();
        }

        public static UserTicketNode[] GetContacts(TicketNode ticket)
        {
            int[] contactIDs = IDReader.Read(TicketChild.Contacts, ticket);
            UserTicketNode[] contacts = new UserTicketNode[contactIDs.Length];
            for (int i = 0; i < contactIDs.Length; ++i)
                contacts[i] = new UserTicketNode(ticket, new UserNode(ticket.Connection, contactIDs[i]), false);
            return contacts;
        }

        public override void Verify()
        {
            
        }
    }
}
