using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace TeamSupport.Proxy
{
    [Table(Name = "TicketQueue")]
    public class TicketQueueProxy
    {
        [Column] public int TicketQueueID;
        [Column] public int TicketID;
        [Column] public int UserID;
    }
}
