using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class TaskNode : IDNode
    {
        public OrganizationModel Organization { get; private set; }
        public int TaskID { get; private set; }

        public TaskNode(OrganizationModel organization, int taskID) : base(organization)
        {
            Organization = organization;
            TaskID = taskID;
        }

        public override void Verify()
        {
            Verify($"SELECT TaskID FROM Tasks WITH (NOLOCK) WHERE TaskID={TaskID} AND OrganizationID={Organization.OrganizationID}");
        }
    }
}
