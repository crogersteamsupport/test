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
  [KnownType(typeof(TaskAssociationProxy))]
  [Table(Name = "TaskAssociations")]
  public class TaskAssociationProxy
  {
    public TaskAssociationProxy() {}
    [DataMember, Column] public int TaskID { get; set; }
    [DataMember, Column] public int RefID { get; set; }
    [DataMember, Column] public int RefType { get; set; }
    [DataMember, Column] public int CreatorID { get; set; }
    [DataMember, Column] public DateTime DateCreated { get; set; }
          
  }
  
}
