using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Model
{
    public class Contact : IModel
    {
        public TicketModel Ticket { get; private set; }
        public int UserID { get; private set; }
        public ConnectionContext Connection { get; private set; }

        public Contact(TicketModel ticket, int userID) : this(ticket, userID, true)
        {
        }

        private Contact(TicketModel ticket, int userID, bool verify)
        {
            Ticket = ticket;
            UserID = userID;
            Connection = ticket.Connection;
            //if (verify)
            //    DBReader.VerifySubscription(Connection._db, ticket.User.Organization.OrganizationID, Ticket.TicketID, UserID);
        }

        public static Contact[] GetContacts(TicketModel ticketModel)
        {
            int[] contactIDs = DBReader.Read(TicketChild.Contacts, ticketModel);
            Contact[] contacts = new Contact[contactIDs.Length];
            for (int i = 0; i < contactIDs.Length; ++i)
                contacts[i] = new Contact(ticketModel, contactIDs[i], false);
            return contacts;
        }
    }
}
