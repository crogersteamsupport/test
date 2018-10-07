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
  [KnownType(typeof(TagLinkProxy)), Table(Name = "TagLinks")]
  public class TagLinkProxy
  {
    public TagLinkProxy() {}
    [DataMember, Column] public int TagLinkID { get; set; }
    [DataMember, Column] public int TagID { get; set; }
    [DataMember, Column] public ReferenceType RefType { get; set; }
    [DataMember, Column] public int RefID { get; set; }
    [DataMember, Column] public DateTime DateCreated { get; set; }
    [DataMember, Column] public int CreatorID { get; set; }
          
  }
  
}
