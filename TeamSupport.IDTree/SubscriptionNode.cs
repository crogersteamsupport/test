using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class SubscriptionNode : IDNode
    {
        public TicketNode Ticket { get; private set; }
        public int UserID { get; private set; }


        /// <summary> top down - existing action </summary>
        public SubscriptionNode(TicketNode ticket, int userID) : this(ticket, userID, true)
        {
        }

        private SubscriptionNode(TicketNode ticket, int userID, bool verify) : base(ticket.Request)
        {
            Ticket = ticket;
            UserID = userID;
            if (verify)
                Verify();
        }

        public static SubscriptionNode[] GetSubscriptions(TicketNode ticket)
        {
            int[] subscriptionIDs = IDReader.Read(TicketChild.Subscriptions, ticket);
            SubscriptionNode[] subscriptionModels = new SubscriptionNode[subscriptionIDs.Length];
            for (int i = 0; i < subscriptionIDs.Length; ++i)
                subscriptionModels[i] = new SubscriptionNode(ticket, subscriptionIDs[i], false);
            return subscriptionModels;
        }

        public override void Verify()
        {
            Verify($"SELECT Subscriptions.userid FROM Subscriptions WITH (NOLOCK) " +
                    $"JOIN Users WITH (NOLOCK) ON users.userid = Subscriptions.userid " +
                    $"WHERE Subscriptions.userid = userID AND Reftype = 17 AND Refid = {Ticket.TicketID} AND MarkDeleted = 0");
        }

    }
}
