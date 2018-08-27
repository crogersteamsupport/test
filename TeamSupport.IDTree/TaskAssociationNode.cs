using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class TaskAssociationNode : IDNode
    {
        public TicketNode Ticket { get; private set; }
        public TicketTaskNode Task { get; private set; }

        public TaskAssociationNode(TicketNode ticket, TicketTaskNode taskID) : this(ticket, taskID, true)
        {
        }

        private TaskAssociationNode(TicketNode ticket, TicketTaskNode task, bool verify) : base(ticket.Request)
        {
            Ticket = ticket;
            Task = task;
            if (verify)
                Verify();
        }

        public static TaskAssociationNode[] GetTaskAssociations(TicketNode ticket)
        {
            int[] taskAssociationIDs = IDReader.Read(TicketChild.TaskAssociations, ticket);
            TaskAssociationNode[] taskAssociationModels = new TaskAssociationNode[taskAssociationIDs.Length];
            for (int i = 0; i < taskAssociationIDs.Length; ++i)
                taskAssociationModels[i] = new TaskAssociationNode(ticket, new TicketTaskNode(ticket, taskAssociationIDs[i]), false);
            return taskAssociationModels;
        }

        public override void Verify()
        {
            Verify($"SELECT TaskID FROM TaskAssociations WITH (NOLOCK) WHERE TaskID={Task.TaskID} AND Refid={Ticket.TicketID} and RefType = 17");
        }

    }
}
