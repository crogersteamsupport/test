using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Model
{
    public class TaskAssociationModel : IModel
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

        public static TaskAssociationModel[] GetTaskAssociationss(TicketModel ticketModel)
        {
            int[] taskIDs = DBReader.Read(TicketChild.TaskAssociations, ticketModel);
            TaskAssociationModel[] taskModels = new TaskAssociationModel[taskIDs.Length];
            for (int i = 0; i < taskIDs.Length; ++i)
                taskModels[i] = new TaskAssociationModel(ticketModel, taskIDs[i], false);
            return taskModels;
        }
    }
}
