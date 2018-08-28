using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class TicketReminderNode : IDNode
    {
        public TicketNode Ticket { get; private set; }
        public UserNode User { get; private set; }
        public int ReminderID { get; private set; }

        /// <summary> top down - existing action </summary>
        public TicketReminderNode(TicketNode ticket, int reminderID) : this(ticket, reminderID, true)
        {
        }

        private TicketReminderNode(TicketNode ticket, int reminderID, bool verify) : base(ticket)
        {
            Ticket = ticket;
            ReminderID = reminderID;
            if (verify)
                Verify();
        }

        public static TicketReminderNode[] GetTicketReminders(TicketNode ticket)
        {
            int[] reminderIDs = IDReader.Read(TicketChild.TicketReminders, ticket);
            TicketReminderNode[] reminderModels = new TicketReminderNode[reminderIDs.Length];
            for (int i = 0; i < reminderIDs.Length; ++i)
                reminderModels[i] = new TicketReminderNode(ticket, reminderIDs[i], false);
            return reminderModels;
        }
        public override void Verify()
        {
            int organizationID = Ticket.Organization.OrganizationID;
            Verify($"SELECT ReminderID FROM Reminders WITH (NOLOCK) WHERE ReminderID={ReminderID} AND OrganizationID={organizationID} AND RefID={Ticket.TicketID} AND RefType=17");
        }
    }
}
