using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(TagLinkProxy))]
  public class TagLinkProxy
  {
    public TagLinkProxy() {}
    public TagLinkProxy(References refType) { RefType = (ReferenceType)refType; }
    [DataMember] public int TagLinkID { get; set; }
    [DataMember] public int TagID { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int CreatorID { get; set; }
          

        public static TagLinkProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Tickets: return new TicketTagLinkProxy();
                default:
                    if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
                    throw new Exception($"Invalid AssetAssignmentsViewItem Reference Type  {refType}");
  }
  
        }

        public enum References
        {
            Tickets
        }
    }

    public class TicketTagLinkProxy : TagLinkProxy
    {
        public TicketTagLinkProxy() : base(References.Tickets) { }
    }
}
