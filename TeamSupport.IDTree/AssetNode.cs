using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class AssetNode : IDNode
    {
        public OrganizationNode Organization { get; private set; }
        public int AssetID { get; private set; }
        public AssetNode(OrganizationNode organization, int assetID) : base(organization)
        {
            Organization = organization;
            AssetID = assetID;
            Verify();
        }

        public override void Verify()
        {
            Verify($"SELECT AssetID FROM Assets WITH (NOLOCK) WHERE AssetID={AssetID} AND OrganizationID={Organization.OrganizationID}");
        }

    }
}
