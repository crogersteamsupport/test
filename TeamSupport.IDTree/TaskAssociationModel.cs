using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class TaskAssociationModel : IDNode
    {
        public TicketModel Ticket { get; private set; }
        public TaskNode Task { get; private set; }

        public TaskAssociationModel(TicketModel ticket, TaskNode taskID) : this(ticket, taskID, true)
        {
        }

        private TaskAssociationModel(TicketModel ticket, TaskNode task, bool verify) : base(ticket)
        {
            Ticket = ticket;
            Task = task;
            if (verify)
                Verify();
        }

        public static TaskAssociationModel[] GetTaskAssociations(TicketModel ticket)
        {
            int[] taskAssociationIDs = IDReader.Read(TicketChild.TaskAssociations, ticket);
            TaskAssociationModel[] taskAssociationModels = new TaskAssociationModel[taskAssociationIDs.Length];
            for (int i = 0; i < taskAssociationIDs.Length; ++i)
                taskAssociationModels[i] = new TaskAssociationModel(ticket, new TaskNode(ticket.Organization, taskAssociationIDs[i]), false);
            return taskAssociationModels;
        }

        public override void Verify()
        {
            Verify($"SELECT TaskID FROM TaskAssociations WITH (NOLOCK) WHERE TaskID={Task.TaskID} AND Refid={Ticket.TicketID} and RefType = 17");
        }

    }
}
