using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics;
using TeamSupport.Data;

namespace TeamSupport.IDTree
{
    /// <summary>
    /// Wrapper for valid TicketID
    /// </summary>
    public class TicketNode : IDNode
    {
        public OrganizationNode Organization { get; private set; }
        public int TicketID { get; private set; }

        int? _ticketNumber;
        public int TicketNumber
        {
            get
            {
                if (!_ticketNumber.HasValue)
                    _ticketNumber = Connection._db.ExecuteQuery<int>($"SELECT TicketNumber FROM Tickets WITH(NOLOCK) WHERE TicketId = {TicketID}").First();
                return _ticketNumber.Value;
            }
        }

        /// <summary> top down - existing ticket </summary>
        public TicketNode(OrganizationNode organization, int ticketID) : base(organization)
        {
            Organization = organization;   // user session on customer
            TicketID = ticketID;
            Verify();
        }

        /// <summary> bottom up - existing action </summary>
        public TicketNode(ConnectionContext connection, int ticketID) : base(connection)
        {
            TicketID = ticketID;
            Verify();
        }

        public override void Verify()
        {
            Verify($"SELECT TicketID FROM Tickets WITH (NOLOCK) WHERE TicketID={TicketID} AND OrganizationID={Organization.OrganizationID}");
        }

        /// <summary> Existing Data.Action </summary>
        public ActionNode Action(int actionID)
        {
            return new ActionNode(this, actionID);
        }

        public ActionNode[] Actions() { return ActionNode.GetActions(this); }
        public AssetTicketNode[] AssetTickets() { return AssetTicketNode.GetAssetTickets(this); }
        public UserTicketNode[] UserTickets() { return UserTicketNode.GetUserTickets(this); }
        public OrganizationTicketNode[] OrganizationTickets() { return OrganizationTicketNode.GetOrganizationTickets(this); }
        public TicketReminderNode[] Reminders() { return TicketReminderNode.GetTicketReminders(this); }
        public SubscriptionNode[] Subscriptions() { return SubscriptionNode.GetSubscriptions(this); }
        public TaskAssociationNode[] TaskAssociations() { return TaskAssociationNode.GetTaskAssociations(this); }

        public TicketNode[] ChildTickets()
        {
            int[] ticketIDs = IDReader.Read(TicketChild.Children, this);
            TicketNode[] childTickets = new TicketNode[ticketIDs.Length];
            for (int i = 0; i < ticketIDs.Length; ++i)
                childTickets[i] = new TicketNode(Organization, ticketIDs[i]);
            return childTickets;
        }

    }
}
