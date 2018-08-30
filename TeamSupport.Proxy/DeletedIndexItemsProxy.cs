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
    [KnownType(typeof(DeletedIndexItemProxy))]
    public class DeletedIndexItemProxy
    {
        public DeletedIndexItemProxy(ReferenceType referenceType) { }
        [DataMember] public int DeletedIndexID { get; set; }
        [DataMember] public int RefID { get; set; }
        [DataMember] protected References RefType { get; set; }
        [DataMember] public DateTime DateDeleted { get; set; }

        public static DeletedIndexItemProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Tickets: return new TicketsDeletedIndexItemProxy();
                case References.Products: return new ProductsDeletedIndexItemProxy();
                case References.Organizations: return new OrganizationsDeletedIndexItemProxy();
                case References.Notes: return new NotesDeletedIndexItemProxy();
                case References.Asset: return new AssetDeletedIndexItemProxy();
                case References.Contacts: return new ContactsDeletedIndexItemProxy();
                default: throw new Exception("Invalid DeletedIndexItem Reference Type");
            }
        }
        public enum References
        {
            Organizations = ReferenceType.Organizations,
            Contacts = ReferenceType.Contacts,
            Notes = ReferenceType.Notes,
            Asset = ReferenceType.Assets,
            Products = ReferenceType.Products,
            Tickets = ReferenceType.Tickets,
        }

    }

    internal class ContactsDeletedIndexItemProxy : DeletedIndexItemProxy
    {
        public ContactsDeletedIndexItemProxy() : base(ReferenceType.Contacts)
        {
        }
    }

    internal class AssetDeletedIndexItemProxy : DeletedIndexItemProxy
    {
        public AssetDeletedIndexItemProxy() : base(ReferenceType.Assets)
        {
        }
    }

    internal class NotesDeletedIndexItemProxy : DeletedIndexItemProxy
    {
        public NotesDeletedIndexItemProxy() : base(ReferenceType.Notes)
        {
        }
    }

    internal class OrganizationsDeletedIndexItemProxy : DeletedIndexItemProxy
    {
        public OrganizationsDeletedIndexItemProxy() : base(ReferenceType.Organizations)
        {
        }
    }

    internal class ProductsDeletedIndexItemProxy : DeletedIndexItemProxy
    {
        public ProductsDeletedIndexItemProxy() : base(ReferenceType.Products)
        {
        }
    }

    internal class TicketsDeletedIndexItemProxy : DeletedIndexItemProxy
    {
        public TicketsDeletedIndexItemProxy() : base(ReferenceType.Tickets)
        {
        }
    }
}
