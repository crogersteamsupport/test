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
  [KnownType(typeof(TaskAssociationProxy))]
  public class TaskAssociationProxy
  {
    public TaskAssociationProxy() {}
    [DataMember] public int TaskID { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public int RefType { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
          
  }
  
}
