using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class TagNode : IDNode
    {
        public OrganizationNode Organization { get; private set; }
        public int TagID { get; private set; }
        public TagNode(OrganizationNode organization, int tagID) : base(organization)
        {

        }

        public override void Verify()
        {
            string query = $"SELECT TagID FROM Tags WITH (NOLOCK) WHERE TagID={TagID} && OrganizationID={Organization.OrganizationID}";
        }
    }
}
    