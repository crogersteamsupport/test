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
  [KnownType(typeof(TicketRelationshipProxy))]
  [Table(Name = "TicketRelationships")]
  public class TicketRelationshipProxy
  {
    public TicketRelationshipProxy() {}
    [DataMember, Column] public int TicketRelationshipID { get; set; }
    [DataMember, Column] public int OrganizationID { get; set; }
    [DataMember, Column] public int Ticket1ID { get; set; }
    [DataMember, Column] public int Ticket2ID { get; set; }
    [DataMember, Column] public int CreatorID { get; set; }
    [DataMember, Column] public DateTime DateCreated { get; set; }
    [DataMember, Column] public int? ImportFileID { get; set; }
          
  }
}
