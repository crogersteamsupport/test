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
        public UserNode User { get; private set; }


        /// <summary> top down - existing action </summary>
        public SubscriptionNode(TicketNode ticket, UserNode user) : this(ticket, user, true)
        {
        }

        private SubscriptionNode(TicketNode ticket, UserNode user, bool verify) : base(ticket)
        {
            Ticket = ticket;
            User = user;
            if (verify)
                Verify();
        }

        public static SubscriptionNode[] GetSubscriptions(TicketNode ticket)
        {
            //query = $"SELECT Subscriptions.userid FROM Subscriptions WITH (NOLOCK) " +
            //        $"JOIN Users WITH (NOLOCK) on users.userid = Subscriptions.userid " +
            //        $"WHERE Reftype = 17 and Refid = {ticket.TicketID} and MarkDeleted = 0";
            int[] subscriptionIDs = IDReader.Read(TicketChild.Subscriptions, ticket);
            SubscriptionNode[] subscriptionModels = new SubscriptionNode[subscriptionIDs.Length];
            for (int i = 0; i < subscriptionIDs.Length; ++i)
                subscriptionModels[i] = new SubscriptionNode(ticket, new UserNode(ticket.Connection, subscriptionIDs[i]), false);
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
