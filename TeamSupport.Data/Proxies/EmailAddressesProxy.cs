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
    [KnownType(typeof(EmailAddressProxy))]
    public abstract class EmailAddressProxy
    {
        public EmailAddressProxy(ReferenceType referenceType) { }
        [DataMember] public int Id { get; set; }
        [DataMember] public int RefID { get; set; }
        [DataMember] protected References RefType { get; set; }
        [DataMember] public string Email { get; set; }
        [DataMember] public DateTime DateCreated { get; set; }
        [DataMember] public DateTime DateModified { get; set; }
        [DataMember] public int CreatorID { get; set; }
        [DataMember] public int ModifierID { get; set; }
        [DataMember] public int? ImportFileID { get; set; }

        public static EmailAddressProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Users:
                    return new UserEmailAddressProxy();
                default: throw new Exception("Invalid EmailAddressProxy Reference Type");
            }
        }
        public enum References
        {
            Users = ReferenceType.Users
        }
    }

    public class UserEmailAddressProxy : EmailAddressProxy
    {
        public UserEmailAddressProxy() : base(ReferenceType.Users)
        {

        }
    }

    public partial class EmailAddress : BaseItem
    {
        public EmailAddressProxy GetProxy()
        {
            EmailAddressProxy result = EmailAddressProxy.ClassFactory((EmailAddressProxy.References)this.RefType); ;
            result.ImportFileID = this.ImportFileID;
            result.ModifierID = this.ModifierID;
            result.CreatorID = this.CreatorID;
            result.Email = this.Email;
            result.RefID = this.RefID;
            result.Id = this.Id;

            result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
            result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);


            return result;
        }
    }
}
