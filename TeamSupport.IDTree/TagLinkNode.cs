using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class TagLinkNode : IDNode
    {
        public TicketNode Ticket { get; private set; }
        public TagNode Tag { get; private set; }
        public int TagLinkID { get; private set; }

        public TagLinkNode(TicketNode ticket, TagNode tag, int tagLinkID) : base(ticket)
        {
            Ticket = ticket;
            Tag = tag;
            TagLinkID = tagLinkID;
            Verify();
        }

        public override void Verify()
        {
            Verify($"SELECT TagLinkID FROM TagLinks WITH (NOLOCK) WHERE TagLinkID={TagLinkID} AND RefID={Ticket.TicketID} AND RefType=17");
        }

    }
}
