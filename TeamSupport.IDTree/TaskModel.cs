using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class TaskModel : IDNode
    {
        public TaskAssociationModel TaskAssociation { get; private set; }
        public int TaskID { get; private set; }

        public TaskModel(TaskAssociationModel taskAssociation, int taskID, bool verify) : base(taskAssociation)
        {
            TaskAssociation = taskAssociation;
            if (verify)
                Verify();
        }

        public TaskModel(TaskAssociationModel task, int taskID) : this(task, taskID, true)
        {
        }

        /// <summary> bottom up  - existing task </summary>
        //public TaskModel(ConnectionContext connection, int taskID) : base(connection)
        //{
        //    TaskID = taskID;
        //    int ticketID = Connection._db.ExecuteQuery<int>($"SELECT TicketID FROM Actions WITH (NOLOCK) WHERE ActionID = {taskID}").Min();
        //    }

        //    Ticket = new TicketModel(Connection, ticketID);
        //    Verify();
        //}

        public override void Verify()
        {
            //Verify($"SELECT TaskID FROM Tasks WITH (NOLOCK) WHERE TaskID={TaskID} AND OrganizationID={Ticket.Connection.OrganizationID}");
        }


    }
}
