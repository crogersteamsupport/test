using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Model
{
    public class SubscriptionModel : IModel
    {
        public TicketModel Ticket { get; private set; }
        public int UserID { get; private set; }
        public ConnectionContext Connection { get; private set; }


        /// <summary> top down - existing action </summary>
        public SubscriptionModel(TicketModel ticket, int userID) : this(ticket, userID, true)
        {
        }

        private SubscriptionModel(TicketModel ticket, int userID, bool verify)
        {
            Ticket = ticket;
            UserID = userID;
            Connection = ticket.Connection;
            if(verify)
                DBReader.VerifySubscription(Connection._db, ticket.User.Organization.OrganizationID, Ticket.TicketID, UserID);
        }

        public static SubscriptionModel[] GetSubscriptions(TicketModel ticket)
        {
            int[] subscriptionIDs = DBReader.Read(TicketChild.Subscriptions, ticket);
            SubscriptionModel[] subscriptionModels = new SubscriptionModel[subscriptionIDs.Length];
            for (int i = 0; i < subscriptionIDs.Length; ++i)
                subscriptionModels[i] = new SubscriptionModel(ticket, subscriptionIDs[i], false);
            return subscriptionModels;
        }

    }
}
