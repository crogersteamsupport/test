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
        public UserNode User { get; private set; }

        public UserTicketNode(TicketNode ticket, UserNode contact) : this(ticket, contact, true)
        {
        }

        private UserTicketNode(TicketNode ticket, UserNode contact, bool verify) : base(ticket)
        {
            Ticket = ticket;
            User = contact;
            if (verify)
                Verify();
        }

        public static UserTicketNode[] GetUserTickets(TicketNode ticket)
        {
            //query = $"SELECT Users.userid FROM Users WITH (NOLOCK)" +
            //    $"JOIN UserTickets WITH (NOLOCK) on UserTickets.userid = Users.UserID" +
            //    $" WHERE UserTickets.TicketID = {ticket.TicketID} AND (Users.MarkDeleted = 0)";

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
