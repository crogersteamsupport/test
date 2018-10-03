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
    public TaskAssociationProxy() { }
    public TaskAssociationProxy(References refType) { RefType = refType; }
    [DataMember, Column] public int TaskID { get; set; }
    [DataMember, Column] public int RefID { get; set; }
    [DataMember, Column] public References RefType { get; private set; }
    [DataMember, Column] public int CreatorID { get; set; }
    [DataMember, Column] public DateTime DateCreated { get; set; }
          
        public static TaskAssociationProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Tickets: return new TicketTaskAssociationProxy();
                case References.Organizations: return new OrganizationTaskAssociationProxy();
                default:
                    if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
                    throw new Exception($"bad ReferenceType {refType}");
            }
        }

        public enum References
        {
            CompanyActivity = ReferenceType.CompanyActivity,
            ContactActivity = ReferenceType.ContactActivity,
            Contacts = ReferenceType.Contacts,
            Groups = ReferenceType.Groups,
            Organizations = ReferenceType.Organizations,
            Products = ReferenceType.Products,
            Tasks = ReferenceType.Tasks,
            Tickets = ReferenceType.Tickets,
            Users = ReferenceType.Users
        }
    }

    public class TicketTaskAssociationProxy : TaskAssociationProxy
    {
        public TicketTaskAssociationProxy() : base(References.Tickets) { }
    }

    public class OrganizationTaskAssociationProxy : TaskAssociationProxy
    {
        public OrganizationTaskAssociationProxy() : base(References.Organizations) { }
    }


}
