using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class Customer : IdInterface
    {
        public TicketModel Ticket { get; private set; }
        public int OrganizationID { get; private set; }
        public ConnectionContext Connection { get; private set; }

        public Customer(TicketModel ticket, int userID) : this(ticket, userID, true)
        {
        }

        private Customer(TicketModel ticket, int organizationID, bool verify)
        {
            Ticket = ticket;
            OrganizationID = organizationID;
            Connection = ticket.Connection;
            //if (verify)
            //    DBReader.VerifySubscription(Connection._db, ticket.User.Organization.OrganizationID, Ticket.TicketID, UserID);
        }

        public static Customer[] GetCustomers(TicketModel ticketModel)
        {
            int[] customerIDs = DBReader.Read(TicketChild.Customers, ticketModel);
            Customer[] customers = new Customer[customerIDs.Length];
            for (int i = 0; i < customerIDs.Length; ++i)
                customers[i] = new Customer(ticketModel, customerIDs[i], false);
            return customers;
        }

    }
}
