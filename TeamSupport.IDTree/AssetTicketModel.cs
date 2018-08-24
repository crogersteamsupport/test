using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class AssetTicketModel : IdInterface
    {
        public TicketModel Ticket { get; private set; }
        public int AssetID { get; private set; }
        public ConnectionContext Connection { get; private set; }

        public AssetTicketModel(TicketModel ticket, int taskID) : this(ticket, taskID, true)
        {
        }

        private AssetTicketModel(TicketModel ticket, int taskID, bool verify)
        {
            Ticket = ticket;
            AssetID = taskID;
            Connection = ticket.Connection;
            if (verify)
                DBReader.VerifyAssetTicket(Connection._db, ticket.User.Organization.OrganizationID, Ticket.TicketID, AssetID);
        }

        public static AssetTicketModel[] GetAssetTickets(TicketModel ticket)
        {
            int[] ids = DBReader.Read(TicketChild.AssetTickets, ticket);
            AssetTicketModel[] models = new AssetTicketModel[ids.Length];
            for (int i = 0; i < ids.Length; ++i)
                models[i] = new AssetTicketModel(ticket, ids[i], false);
            return models;
        }
    }
}
