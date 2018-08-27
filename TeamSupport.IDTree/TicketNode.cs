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
        public OrganizationNode Customer { get; private set; }
        public UserNode User { get; private set; }
        public int TicketID { get; private set; }

        int? _ticketNumber;
        public int TicketNumber
        {
            get
            {
                if (!_ticketNumber.HasValue)
                    _ticketNumber = IDReader.TicketNumber(Request._db,TicketID);
                return _ticketNumber.Value;
            }
        }

        /// <summary> top down - existing action </summary>
        public TicketNode(UserNode user, int ticketID) : base(user.Request)
        {
            User = user;
            TicketID = ticketID;
            IDReader.Verify(OrganizationChild.Ticket, Request._db, User.Organization.OrganizationID, TicketID);
        }

        /// <summary> bottom up - existing action </summary>
        public TicketNode(ClientRequest connection, int ticketID) : base(connection)
        {
            User = Request.User;
            TicketID = ticketID;
            IDReader.Verify(OrganizationChild.Ticket, Request._db, Request.Organization.OrganizationID, TicketID);
        }

        /// <summary> Existing Data.Action </summary>
        public ActionNode Action(int actionID)
        {
            return new ActionNode(this, actionID);
        }

        public AssetTicketNode[] AssetTickets() { return AssetTicketNode.GetAssetTickets(this); }
        public UserTicketNode[] Contacts() { return UserTicketNode.GetContacts(this); }
        public OrganizationTicketNode[] Customers() { return OrganizationTicketNode.GetClients(this); }
        public TicketReminderNode[] Reminders() { return TicketReminderNode.GetReminders(this); }
        public SubscriptionNode[] Subscriptions() { return SubscriptionNode.GetSubscriptions(this); }
        public TaskAssociationNode[] TaskAssociations() { return TaskAssociationNode.GetTaskAssociations(this); }

    }
}
