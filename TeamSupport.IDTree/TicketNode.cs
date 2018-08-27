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
                    _ticketNumber = IDReader.TicketNumber(Connection._db,TicketID);
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

        /// <summary> Existing Data.Action </summary>
        public ActionNode Action(int actionID)
        {
            return new ActionNode(this, actionID);
        }

        public AssetTicketNode[] AssetTickets() { return AssetTicketNode.GetAssetTickets(this); }
        public UserTicketNode[] Contacts() { return UserTicketNode.GetContacts(this); }
        public OrganizationTicketNode[] Customers() { return OrganizationTicketNode.GetOrganizationTickets(this); }
        public TicketReminderNode[] Reminders() { return TicketReminderNode.GetReminders(this); }
        public SubscriptionNode[] Subscriptions() { return SubscriptionNode.GetSubscriptions(this); }
        public TaskAssociationNode[] TaskAssociations() { return TaskAssociationNode.GetTaskAssociations(this); }

        public override void Verify()
        {
            Verify($"SELECT TicketID FROM Tickets WITH (NOLOCK) WHERE TicketID={TicketID} AND OrganizationID={Organization.OrganizationID}");
        }
    }
}
