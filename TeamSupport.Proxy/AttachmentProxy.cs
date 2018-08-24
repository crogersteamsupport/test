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
    public enum AttachmentType
    {
        Actions = ReferenceType.Actions,
        Organizations = ReferenceType.Organizations,
        ProductVersions = ReferenceType.ProductVersions,
        Users = ReferenceType.Users,
        Contacts = ReferenceType.Contacts,
        Assets = ReferenceType.Assets,
        UserPhoto = ReferenceType.UserPhoto,
        WaterCooler = ReferenceType.WaterCooler,
        CustomerHubLogo = ReferenceType.CustomerHubLogo,
        ChatAttachments = ReferenceType.ChatAttachments,
        Tasks = ReferenceType.Tasks,
        CompanyActivity = ReferenceType.CompanyActivity,
        ContactActivity = ReferenceType.ContactActivity,
    };


    [DataContract(Namespace = "http://teamsupport.com/")]
    [KnownType(typeof(AttachmentProxy))]
    public abstract class AttachmentProxy
    {
        protected AttachmentProxy(AttachmentType type, int refID)
        {
            RefType = type;
            RefID = RefID;
        }

        [DataMember] public int AttachmentID { get; set; }
        [DataMember] public int OrganizationID { get; set; }
        [DataMember] public string FileName { get; set; }
        [DataMember] public string FileType { get; set; }
        [DataMember] public long FileSize { get; set; }
        [DataMember] public string Path { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] public DateTime DateCreated { get; set; }
        [DataMember] public DateTime DateModified { get; set; }
        [DataMember] public int CreatorID { get; set; }
        [DataMember] public int ModifierID { get; set; }
        [DataMember] protected AttachmentType RefType { get; set; }
        [DataMember] protected int RefID { get; set; }
        [DataMember] public string CreatorName { get; set; }
        [DataMember] public bool SentToJira { get; set; }
        [DataMember] public int? ProductFamilyID { get; set; }
        [DataMember] public string ProductFamily { get; set; }
        [DataMember] public bool SentToTFS { get; set; }
        [DataMember] public bool SentToSnow { get; set; }
        [DataMember] public int? FilePathID { get; set; }

        public static AttachmentProxy ClassFactory(AttachmentType type, int refID)
        {
            switch(type)
            {
                case AttachmentType.Actions: return new ActionAttachmentProxy(refID);
                case AttachmentType.Assets: return new AssetAttachmentProxy(refID);
                case AttachmentType.ChatAttachments: return new ChatAttachmentProxy(refID);
                case AttachmentType.CompanyActivity: return new CompanyActivityAttachmentProxy(refID);
                case AttachmentType.ContactActivity: return new ContactActivityAttachmentProxy(refID);
                case AttachmentType.Contacts: return new ContactAttachmentProxy(refID);
                case AttachmentType.CustomerHubLogo: return new CustomerHubLogoAttachmentProxy(refID);
                case AttachmentType.Organizations: return new OrganizationAttachmentProxy(refID);
                case AttachmentType.ProductVersions: return new ProductVersionAttachmentProxy(refID);
                case AttachmentType.Tasks: return new TaskAttachmentProxy(refID);
                case AttachmentType.UserPhoto: return new UserPhotoAttachmentProxy(refID);
                case AttachmentType.Users: return new UserAttachmentProxy(refID);
                case AttachmentType.WaterCooler: return new WaterCoolerAttachmentProxy(refID);
                default: throw new Exception("bad AttachmentType");
            }
        }
    }


    public class ActionAttachmentProxy : AttachmentProxy
    {
        public ActionAttachmentProxy(int refID) : base(AttachmentType.Actions, refID) { }
        public int ActionID { get { return RefID; } }
    }

    public class OrganizationAttachmentProxy : AttachmentProxy
    {
        public OrganizationAttachmentProxy(int refID) : base(AttachmentType.Organizations, refID) { }
        //public int OrganizationID { get { return RefID; } }   // Hides base class OrganizationID?
    }

    public class ContactAttachmentProxy : AttachmentProxy
    {
        public ContactAttachmentProxy(int refID) : base(AttachmentType.Contacts, refID) { }
        public int UserID { get { return RefID; } }
    }

    public class WaterCoolerAttachmentProxy : AttachmentProxy
    {
        public WaterCoolerAttachmentProxy(int refID) : base(AttachmentType.WaterCooler, refID) { }
        public int MessageID { get { return RefID; } }
    }

    public class AssetAttachmentProxy : AttachmentProxy
    {
        public AssetAttachmentProxy(int refID) : base(AttachmentType.Assets, refID) { }
        public int AssetID { get { return RefID; } }
    }

    public class ChatAttachmentProxy : AttachmentProxy
    {
        public ChatAttachmentProxy(int refID) : base(AttachmentType.ChatAttachments, refID) { }
        public int ChatID { get { return RefID; } }
    }

    public class CompanyActivityAttachmentProxy : AttachmentProxy
    {
        public CompanyActivityAttachmentProxy(int refID) : base(AttachmentType.CompanyActivity, refID) { }
    }


    public class ContactActivityAttachmentProxy : AttachmentProxy
    {
        public ContactActivityAttachmentProxy(int refID) : base(AttachmentType.ContactActivity, refID) { }
    }


    public class CustomerHubLogoAttachmentProxy : AttachmentProxy
    {
        public CustomerHubLogoAttachmentProxy(int refID) : base(AttachmentType.CustomerHubLogo, refID) { }
    }


    public class ProductVersionAttachmentProxy : AttachmentProxy
    {
        public ProductVersionAttachmentProxy(int refID) : base(AttachmentType.ProductVersions, refID) { }
    }


    public class TaskAttachmentProxy : AttachmentProxy
    {
        public TaskAttachmentProxy(int refID) : base(AttachmentType.Tasks, refID) { }
    }


    public class UserPhotoAttachmentProxy : AttachmentProxy
    {
        public UserPhotoAttachmentProxy(int refID) : base(AttachmentType.UserPhoto, refID) { }
    }


    public class UserAttachmentProxy : AttachmentProxy
    {
        public UserAttachmentProxy(int refID) : base(AttachmentType.Users, refID) { }
    }

}
