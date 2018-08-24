using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class TaskAssociationModel : IdInterface
    {
        public TicketModel Ticket { get; private set; }
        public int TaskID { get; private set; }
        public ConnectionContext Connection { get; private set; }

        public TaskAssociationModel(TicketModel ticket, int taskID) : this(ticket, taskID, true)
        {
        }

        private TaskAssociationModel(TicketModel ticket, int taskID, bool verify)
        {
            Ticket = ticket;
            TaskID = taskID;
            Connection = ticket.Connection;
            if (verify)
                DBReader.VerifyTaskAssociation(Connection._db, ticket.User.Organization.OrganizationID, Ticket.TicketID, TaskID);
        }

        public static TaskAssociationModel[] GetTaskAssociations(TicketModel ticket)
        {
            int[] taskAssociationIDs = DBReader.Read(TicketChild.TaskAssociations, ticket);
            TaskAssociationModel[] taskAssociationModels = new TaskAssociationModel[taskAssociationIDs.Length];
            for (int i = 0; i < taskAssociationIDs.Length; ++i)
                taskAssociationModels[i] = new TaskAssociationModel(ticket, taskAssociationIDs[i], false);
            return taskAssociationModels;
        }

    }
}
