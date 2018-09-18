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

    [DataContract(Namespace = "http://teamsupport.com/")]
    [KnownType(typeof(AttachmentProxy))]
    [Table(Name = "Attachments")]
    [InheritanceMapping(Code = AttachmentProxy.References.Actions, Type = typeof(ActionAttachmentProxy), IsDefault = true)]
    public abstract class AttachmentProxy
    {
        public AttachmentProxy() { }
        protected AttachmentProxy(References type, int refID)
        {
            RefType = type;
            RefID = RefID;
        }

        [DataMember, Column(DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int AttachmentID { get; set; }
        [DataMember, Column] public int OrganizationID { get; set; }
        [DataMember, Column] public string FileName { get; set; }
        [DataMember, Column] public string FileType { get; set; }
        [DataMember, Column] public long FileSize { get; set; }
        [DataMember, Column] public string Path { get; set; }
        [DataMember, Column] public string Description { get; set; }
        [DataMember, Column] public DateTime DateCreated { get; set; }
        [DataMember, Column] public DateTime DateModified { get; set; }
        [DataMember, Column] public int CreatorID { get; set; }
        [DataMember, Column] public int ModifierID { get; set; }
        [DataMember, Column(IsDiscriminator = true)] public References RefType { get; set; }
        [DataMember, Column] public int RefID { get; set; }
        [DataMember] public string CreatorName { get; set; }
        [DataMember, Column] public bool SentToJira { get; set; }
        [DataMember, Column] public int? ProductFamilyID { get; set; }
        [DataMember] public string ProductFamily { get; set; }
        [DataMember, Column] public bool SentToTFS { get; set; }
        [DataMember, Column] public bool SentToSnow { get; set; }
        [DataMember, Column] public int? FilePathID { get; set; }

        public const int AttachmentPathIndex = 3;

        public static AttachmentProxy ClassFactory(References refType, int refID)
        {
            switch (refType)
            {
                case References.Actions: return new ActionAttachmentProxy(refID);
                case References.Assets: return new AssetAttachmentProxy(refID);
                case References.ChatAttachments: return new ChatAttachmentProxy(refID);
                case References.CompanyActivity: return new CompanyActivityAttachmentProxy(refID);
                case References.ContactActivity: return new ContactActivityAttachmentProxy(refID);
                case References.Contacts: return new ContactAttachmentProxy(refID);
                case References.CustomerHubLogo: return new CustomerHubLogoAttachmentProxy(refID);
                case References.Organizations: return new OrganizationAttachmentProxy(refID);
                case References.ProductVersions: return new ProductVersionAttachmentProxy(refID);
                case References.Tasks: return new TaskAttachmentProxy(refID);
                case References.UserPhoto: return new UserPhotoAttachmentProxy(refID);
                case References.Users: return new UserAttachmentProxy(refID);
                case References.WaterCooler: return new WaterCoolerAttachmentProxy(refID);
                case References.Imports: return new ImportsAttachmentProxy(refID);
                case References.None: return new NoneAttachmentProxy(refID);
                default:
                    if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
                    throw new Exception($"bad ReferenceType {refType}");
            }
        }

        public enum References
        {
            None = ReferenceType.None,
            Actions = ReferenceType.Actions,
            Assets = ReferenceType.Assets,
            ChatAttachments = ReferenceType.ChatAttachments,
            CompanyActivity = ReferenceType.CompanyActivity,
            ContactActivity = ReferenceType.ContactActivity,
            Contacts = ReferenceType.Contacts,
            CustomerHubLogo = ReferenceType.CustomerHubLogo,
            Organizations = ReferenceType.Organizations,
            ProductVersions = ReferenceType.ProductVersions,
            Tasks = ReferenceType.Tasks,
            UserPhoto = ReferenceType.UserPhoto,
            Users = ReferenceType.Users,
            WaterCooler = ReferenceType.WaterCooler,

            Imports = ReferenceType.Imports,
        };

    }

    public class ActionAttachmentProxy : AttachmentProxy
    {
        public ActionAttachmentProxy() { }
        public ActionAttachmentProxy(int refID) : base(References.Actions, refID) { }
        public int ActionID { get { return RefID; } }
    }

    public class OrganizationAttachmentProxy : AttachmentProxy
    {
        public OrganizationAttachmentProxy(int refID) : base(References.Organizations, refID) { }
        //public int OrganizationID { get { return RefID; } }   // Hides base class OrganizationID?
    }

    public class ContactAttachmentProxy : AttachmentProxy
    {
        public ContactAttachmentProxy(int refID) : base(References.Contacts, refID) { }
        public int UserID { get { return RefID; } }
    }

    public class WaterCoolerAttachmentProxy : AttachmentProxy
    {
        public WaterCoolerAttachmentProxy(int refID) : base(References.WaterCooler, refID) { }
        public int MessageID { get { return RefID; } }
    }

    public class AssetAttachmentProxy : AttachmentProxy
    {
        public AssetAttachmentProxy(int refID) : base(References.Assets, refID) { }
        public int AssetID { get { return RefID; } }
    }

    public class ChatAttachmentProxy : AttachmentProxy
    {
        public ChatAttachmentProxy(int refID) : base(References.ChatAttachments, refID) { }
        public int ChatID { get { return RefID; } }
    }

    public class CompanyActivityAttachmentProxy : AttachmentProxy
    {
        public CompanyActivityAttachmentProxy(int refID) : base(References.CompanyActivity, refID) { }
    }


    public class ContactActivityAttachmentProxy : AttachmentProxy
    {
        public ContactActivityAttachmentProxy(int refID) : base(References.ContactActivity, refID) { }
    }


    public class CustomerHubLogoAttachmentProxy : AttachmentProxy
    {
        public CustomerHubLogoAttachmentProxy(int refID) : base(References.CustomerHubLogo, refID) { }
    }


    public class ProductVersionAttachmentProxy : AttachmentProxy
    {
        public ProductVersionAttachmentProxy(int refID) : base(References.ProductVersions, refID) { }
    }


    public class TaskAttachmentProxy : AttachmentProxy
    {
        public TaskAttachmentProxy(int refID) : base(References.Tasks, refID) { }
    }


    public class UserPhotoAttachmentProxy : AttachmentProxy
    {
        public UserPhotoAttachmentProxy(int refID) : base(References.UserPhoto, refID) { }
    }


    public class UserAttachmentProxy : AttachmentProxy
    {
        public UserAttachmentProxy(int refID) : base(References.Users, refID) { }
    }

    public class ImportsAttachmentProxy : AttachmentProxy
    {
        public ImportsAttachmentProxy(int refID) : base(References.Imports, refID) { }
    }

    public class NoneAttachmentProxy : AttachmentProxy
    {
        public NoneAttachmentProxy(int refID) : base(References.None, refID) { }
    }

}
