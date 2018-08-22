using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Model
{
    public class TagLinkModel
    {
        public TicketModel Ticket { get; private set; }
        public int TagLinkID { get; private set; }
        public ConnectionContext Connection { get; private set; }

        public TagLinkModel(TicketModel ticket, int tagLinkID)
        {
            Ticket = ticket;
            TagLinkID = tagLinkID;
            Connection = ticket.Connection;

            DBReader.VerifyTagLink(Connection._db, Connection.Organization.OrganizationID, ticket.TicketID, TagLinkID);
        }


    }
}
