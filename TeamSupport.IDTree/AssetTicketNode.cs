using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class AssetTicketNode : IDNode
    {
        public AssetNode Asset { get; private set; }
        public TicketNode Ticket { get; private set; }

        public AssetTicketNode(AssetNode asset, TicketNode ticket) : this(asset, ticket, true)
        {
        }

        private AssetTicketNode(AssetNode asset, TicketNode ticket, bool verify) : base(ticket)
        {
            Asset = asset;
            Ticket = ticket;
            if (verify)
                Verify();
        }

        public static AssetTicketNode[] GetAssetTickets(TicketNode ticket)
        {
            OrganizationNode organization = ticket.Organization;
            //int[] ids = IDReader.Read(TicketChild.Asset, ticket);   // $"SELECT AssetID From AssetTickets WITH (NOLOCK) WHERE TicketID = {ticket.TicketID}"
            string query = $"SELECT AssetID From AssetTickets WITH (NOLOCK) WHERE TicketID = {ticket.TicketID}";
            int[] ids = ticket.Connection._db.ExecuteQuery<int>(query).ToArray();
            AssetTicketNode[] models = new AssetTicketNode[ids.Length];
            for (int i = 0; i < ids.Length; ++i)
                models[i] = new AssetTicketNode(new AssetNode(organization, ids[i]), ticket, false);
            return models;
        }

        public override void Verify()
        {
            Verify($"SELECT AssetID FROM AssetTickets WITH (NOLOCK) WHERE AssetID={Asset.AssetID} AND TicketID={Ticket.TicketID}");
        }
    }
}
