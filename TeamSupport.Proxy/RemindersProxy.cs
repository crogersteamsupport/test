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
  [KnownType(typeof(ReminderProxy))]
  [Table(Name = "Reminders")]
  public class ReminderProxy
  {
    public ReminderProxy() {}
    [DataMember, Column] public int ReminderID { get; set; }
    [DataMember, Column] public int OrganizationID { get; set; }
    [DataMember, Column] public ReferenceType RefType { get; set; }
    [DataMember, Column] public int RefID { get; set; }
    [DataMember, Column] public string Description { get; set; }
    [DataMember, Column] public DateTime DueDate { get; set; }
    [DataMember, Column] public int UserID { get; set; }
    [DataMember, Column] public bool IsDismissed { get; set; }
    [DataMember, Column] public bool HasEmailSent { get; set; }
    [DataMember, Column] public int CreatorID { get; set; }
    [DataMember, Column] public DateTime DateCreated { get; set; }
          
  }
  
}
