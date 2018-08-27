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
        public int UserID { get; private set; }
        public UserNode User { get; private set; }

        public UserTicketNode(TicketNode ticket, int userID) : this(ticket, userID, true)
        {
        }

        private UserTicketNode(TicketNode ticket, int userID, bool verify) : base(ticket.Request)
        {
            Ticket = ticket;
            UserID = userID;
            //if (verify)
            //    DBReader.VerifySubscription(Connection._db, ticket.User.Organization.OrganizationID, Ticket.TicketID, UserID);
        }

        public static UserTicketNode[] GetContacts(TicketNode ticketModel)
        {
            int[] contactIDs = IDReader.Read(TicketChild.Contacts, ticketModel);
            UserTicketNode[] contacts = new UserTicketNode[contactIDs.Length];
            for (int i = 0; i < contactIDs.Length; ++i)
                contacts[i] = new UserTicketNode(ticketModel, contactIDs[i], false);
            return contacts;
        }
    }
}
