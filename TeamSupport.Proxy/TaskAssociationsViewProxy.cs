using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
    [DataContract(Namespace = "http://teamsupport.com/")]
    [KnownType(typeof(TaskAssociationsViewItemProxy))]
  public class TaskAssociationsViewItemProxy
    {
    public TaskAssociationsViewItemProxy() {}
        [DataMember] public int TaskID { get; set; }
        [DataMember] public int RefID { get; set; }
    [DataMember] public int RefType { get; set; }
        [DataMember] public int? TicketNumber { get; set; }
        [DataMember] public string TicketName { get; set; }
        [DataMember] public string User { get; set; }
        [DataMember] public string Company { get; set; }
        [DataMember] public string Group { get; set; }
        [DataMember] public string Product { get; set; }
        [DataMember] public string Contact { get; set; }
        [DataMember] public string Activity { get; set; }
        [DataMember] public int ActivityID { get; set; }
        [DataMember] public int ActivityRefID { get; set; }

        public static TaskAssociationsViewItemProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Organizations: return new OrganizationsTaskAssociationsViewItemProxy();
                case References.Contacts: return new ContactsTaskAssociationsViewItemProxy();
                case References.CompanyActivity: return new CompanyActivityTaskAssociationsViewItemProxy();
                case References.Groups: return new GroupsTaskAssociationsViewItemProxy();
                case References.ContactActivity: return new ContactActivityTaskAssociationsViewItemProxy();
                case References.Tasks: return new TasksTaskAssociationsViewItemProxy();
                case References.Products: return new ProductsTaskAssociationsViewItemProxy();
                case References.Users: return new UsersTaskAssociationsViewItemProxy();
                case References.Tickets: return new TicketsTaskAssociationsViewItemProxy();
                default: throw new Exception("Invalid TaskAssociationsViewItem Reference Type");
            }
        }
        public enum References
        {
            Organizations = ReferenceType.Organizations,
            Contacts = ReferenceType.Contacts,
            CompanyActivity = ReferenceType.CompanyActivity,
            Groups = ReferenceType.Groups,
            ContactActivity = ReferenceType.ContactActivity,
            Tasks = ReferenceType.Tasks,
            Products = ReferenceType.Products,
            Users = ReferenceType.Users,
            Tickets = ReferenceType.Tickets,
        }

    }

    internal class OrganizationsTaskAssociationsViewItemProxy : TaskAssociationsViewItemProxy
    {
        public OrganizationsTaskAssociationsViewItemProxy() : base(ReferenceType.Organizations)
        {
        }
    }
    internal class ContactsTaskAssociationsViewItemProxy : TaskAssociationsViewItemProxy
    {
        public ContactsTaskAssociationsViewItemProxy() : base(ReferenceType.Contacts)
        {
        }
    }
    internal class CompanyActivityTaskAssociationsViewItemProxy : TaskAssociationsViewItemProxy
    {
        public CompanyActivityTaskAssociationsViewItemProxy() : base(ReferenceType.CompanyActivity)
        {
        }
    }
    internal class GroupsTaskAssociationsViewItemProxy : TaskAssociationsViewItemProxy
    {
        public GroupsTaskAssociationsViewItemProxy() : base(ReferenceType.Groups)
        {
        }
    }
    internal class ContactActivityTaskAssociationsViewItemProxy : TaskAssociationsViewItemProxy
    {
        public ContactActivityTaskAssociationsViewItemProxy() : base(ReferenceType.Tasks)
        {
        }
    }
    internal class TasksTaskAssociationsViewItemProxy : TaskAssociationsViewItemProxy
    {
        public TasksTaskAssociationsViewItemProxy() : base(ReferenceType.Tasks)
        {
        }
    }
    internal class ProductsTaskAssociationsViewItemProxy : TaskAssociationsViewItemProxy
    {
        public ProductsTaskAssociationsViewItemProxy() : base(ReferenceType.Products)
        {
        }
    }
    internal class UsersTaskAssociationsViewItemProxy : TaskAssociationsViewItemProxy
    {
        public UsersTaskAssociationsViewItemProxy() : base(ReferenceType.Users)
        {
        }
    }
    internal class TicketsTaskAssociationsViewItemProxy : TaskAssociationsViewItemProxy
    {
        public TicketsTaskAssociationsViewItemProxy() : base(ReferenceType.Tickets)
        {
        }
    }
}
