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
    [KnownType(typeof(PhoneNumbersViewItemProxy))]
    public class PhoneNumbersViewItemProxy
    {
        public PhoneNumbersViewItemProxy() { }
        public PhoneNumbersViewItemProxy(ReferenceType referenceType) { }
        [DataMember] public int PhoneID { get; set; }
        [DataMember] public int? PhoneTypeID { get; set; }
        [DataMember] public int RefID { get; set; }
        [DataMember] protected References RefType { get; set; }
        [DataMember] public string PhoneNumber { get; set; }
        [DataMember] public string FormattedPhoneNumber { get; set; }
        [DataMember] public string Extension { get; set; }
        [DataMember] public string OtherTypeName { get; set; }
        [DataMember] public DateTime DateCreated { get; set; }
        [DataMember] public DateTime DateModified { get; set; }
        [DataMember] public int CreatorID { get; set; }
        [DataMember] public int ModifierID { get; set; }
        [DataMember] public string PhoneType { get; set; }
        [DataMember] public string CreatorName { get; set; }
        [DataMember] public string ModifierName { get; set; }

        public static PhoneNumbersViewItemProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Organizations: return new OrganizationsPhoneNumbersViewItemProxy();
                case References.Contacts: return new ContactsPhoneNumbersViewItemProxy();
                case References.Users: return new UsersPhoneNumbersViewItemProxy();
                default: throw new Exception("Invalid PhoneNumbersViewItemProxy Reference Type");
            }
        }
        public enum References
        {
            Organizations = ReferenceType.Organizations,
            Contacts = ReferenceType.Contacts,
            Users = ReferenceType.Users,
        }

    }

    internal class OrganizationsPhoneNumbersViewItemProxy : PhoneNumbersViewItemProxy
    {
        public OrganizationsPhoneNumbersViewItemProxy() : base(ReferenceType.Organizations)
        {
        }
    }
    internal class ContactsPhoneNumbersViewItemProxy : PhoneNumbersViewItemProxy
    {
        public ContactsPhoneNumbersViewItemProxy() : base(ReferenceType.Contacts)
        {
        }
    }

    internal class UsersPhoneNumbersViewItemProxy : PhoneNumbersViewItemProxy
    {
        public UsersPhoneNumbersViewItemProxy() : base(ReferenceType.Users)
        {
        }
    }
}
