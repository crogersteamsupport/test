using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Model
{
    public class ReminderModel
    {
        public TicketModel Ticket { get; private set; }
        public int ReminderID { get; private set; }
        public ConnectionContext Connection { get; private set; }

        /// <summary> top down - existing action </summary>
        public ReminderModel(TicketModel ticket, int reminderID) : this(ticket, reminderID, true)
        {
        }

        private ReminderModel(TicketModel ticket, int reminderID, bool verify)
        {
            Ticket = ticket;
            ReminderID = reminderID;
            Connection = ticket.Connection;
            if(verify)
                DBReader.VerifyReminder(Connection._db, ticket.User.Organization.OrganizationID, Ticket.TicketID, ReminderID);
        }

        public static ReminderModel[] GetReminders(TicketModel ticket)
        {
            int[] reminderIDs = DBReader.Read(TicketChild.Reminders, ticket);
            ReminderModel[] reminderModels = new ReminderModel[reminderIDs.Length];
            for (int i = 0; i < reminderIDs.Length; ++i)
                reminderModels[i] = new ReminderModel(ticket, reminderIDs[i], false);
            return reminderModels;
        }
    }
}
