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
    [KnownType(typeof(CustomFieldCategoryProxy))]
    public abstract class CustomFieldCategoryProxy
    {

        public CustomFieldCategoryProxy(ReferenceType referenceType) { }
        [DataMember] public int CustomFieldCategoryID { get; set; }
        [DataMember] public int OrganizationID { get; set; }
        [DataMember] public string Category { get; set; }
        [DataMember] public int Position { get; set; }
        [DataMember] protected References RefType { get; set; }
        [DataMember] public int? AuxID { get; set; }
        [DataMember] public int? ProductFamilyID { get; set; }
        [DataMember] public string ProductFamilyName { get; set; }

        public static CustomFieldCategoryProxy ClassFactory(References refType)
        {
            switch(refType)
            {
                case References.Assets:
                    return new AssetCustomFieldCategoryProxy();
                case References.Contacts:
                    return new ContactsCustomFieldCategoryProxy();
                case References.Organizations:
                    return new OrganizationsCustomFieldCategoryProxy();
                case References.Tickets:
                    return new TicketsCustomFieldCategoryProxy();
                default: throw new Exception("Invalid CustomFieldCategory Reference Type");
            }
        }

        public enum References
        {
            Organizations = ReferenceType.Organizations,
            Tickets = ReferenceType.Tickets,
            Contacts = ReferenceType.Contacts,
            Assets = ReferenceType.Assets
        }
    }

    public class AssetCustomFieldCategoryProxy : CustomFieldCategoryProxy
    {
        public AssetCustomFieldCategoryProxy() : base(ReferenceType.Assets)
        {

        }
    }

    public class ContactsCustomFieldCategoryProxy : CustomFieldCategoryProxy
    {
        public ContactsCustomFieldCategoryProxy() : base(ReferenceType.Contacts)
        {

        }
    }


    public class OrganizationsCustomFieldCategoryProxy : CustomFieldCategoryProxy
    {
        public OrganizationsCustomFieldCategoryProxy() : base(ReferenceType.Organizations)
        {

        }
    }


    public class TicketsCustomFieldCategoryProxy : CustomFieldCategoryProxy
    {
        public TicketsCustomFieldCategoryProxy() : base(ReferenceType.Tickets)
        {

        }
    }

    public partial class CustomFieldCategory : BaseItem
    {
        public CustomFieldCategoryProxy GetProxy()
        {
            CustomFieldCategoryProxy result = CustomFieldCategoryProxy.ClassFactory((CustomFieldCategoryProxy.References)this.RefType);
            result.ProductFamilyID = this.ProductFamilyID;
            result.AuxID = this.AuxID;
//            result.RefType = this.RefType;
            result.Position = this.Position;
            result.Category = this.Category;
            result.OrganizationID = this.OrganizationID;
            result.CustomFieldCategoryID = this.CustomFieldCategoryID;
            result.ProductFamilyName = this.ProductFamilyName;

            return result;
        }



    }
}
