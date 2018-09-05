using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Data.Linq.Mapping;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(TicketQueueItemProxy))]
  [Table(Name = "TicketQueue")]
  public class TicketQueueItemProxy
  {
    public TicketQueueItemProxy() {}
    [DataMember, Column] public int TicketQueueID { get; set; }
    [DataMember, Column] public int TicketID { get; set; }
    [DataMember, Column] public int UserID { get; set; }
    [DataMember, Column] public decimal? EstimatedDays { get; set; }
    [DataMember, Column] public int Position { get; set; }
    [DataMember, Column] public DateTime DateCreated { get; set; }
    [DataMember, Column] public DateTime DateModified { get; set; }
    [DataMember, Column] public int CreatorID { get; set; }
    [DataMember, Column] public int ModifierID { get; set; }
          
  }
}
