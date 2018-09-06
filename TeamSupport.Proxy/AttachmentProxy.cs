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
    public class AttachmentProxy
    {
        public AttachmentProxy() { }
        protected AttachmentProxy(ReferenceType type, int refID)
        {
            RefType = type;
            RefID = RefID;
        }

        [DataMember, Column] public int AttachmentID { get; set; }
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
        [DataMember, Column] public ReferenceType RefType { get; set; }
        [DataMember, Column] public int RefID { get; set; }
        [DataMember, Column] public string CreatorName { get; set; }
        [DataMember, Column] public bool SentToJira { get; set; }
        [DataMember, Column] public int? ProductFamilyID { get; set; }
        [DataMember, Column] public string ProductFamily { get; set; }
        [DataMember, Column] public bool SentToTFS { get; set; }
        [DataMember, Column] public bool SentToSnow { get; set; }
        [DataMember, Column] public int? FilePathID { get; set; }

        public static AttachmentProxy ClassFactory(ReferenceType type, int refID)
        {
            switch(type)
            {
                case ReferenceType.Actions: return new ActionAttachmentProxy(refID);
                case ReferenceType.Assets: return new AssetAttachmentProxy(refID);
                case ReferenceType.ChatAttachments: return new ChatAttachmentProxy(refID);
                case ReferenceType.CompanyActivity: return new CompanyActivityAttachmentProxy(refID);
                case ReferenceType.ContactActivity: return new ContactActivityAttachmentProxy(refID);
                case ReferenceType.Contacts: return new ContactAttachmentProxy(refID);
                case ReferenceType.CustomerHubLogo: return new CustomerHubLogoAttachmentProxy(refID);
                case ReferenceType.Organizations: return new OrganizationAttachmentProxy(refID);
                case ReferenceType.ProductVersions: return new ProductVersionAttachmentProxy(refID);
                case ReferenceType.Tasks: return new TaskAttachmentProxy(refID);
                case ReferenceType.UserPhoto: return new UserPhotoAttachmentProxy(refID);
                case ReferenceType.Users: return new UserAttachmentProxy(refID);
                case ReferenceType.WaterCooler: return new WaterCoolerAttachmentProxy(refID);
                default: throw new Exception("bad ReferenceType");
            }
        }

        public enum References
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
    }


    public class ActionAttachmentProxy : AttachmentProxy
    {
        public ActionAttachmentProxy(int refID) : base(ReferenceType.Actions, refID) { }
        public int ActionID { get { return RefID; } }
    }

    public class OrganizationAttachmentProxy : AttachmentProxy
    {
        public OrganizationAttachmentProxy(int refID) : base(ReferenceType.Organizations, refID) { }
        //public int OrganizationID { get { return RefID; } }   // Hides base class OrganizationID?
    }

    public class ContactAttachmentProxy : AttachmentProxy
    {
        public ContactAttachmentProxy(int refID) : base(ReferenceType.Contacts, refID) { }
        public int UserID { get { return RefID; } }
    }

    public class WaterCoolerAttachmentProxy : AttachmentProxy
    {
        public WaterCoolerAttachmentProxy(int refID) : base(ReferenceType.WaterCooler, refID) { }
        public int MessageID { get { return RefID; } }
    }

    public class AssetAttachmentProxy : AttachmentProxy
    {
        public AssetAttachmentProxy(int refID) : base(ReferenceType.Assets, refID) { }
        public int AssetID { get { return RefID; } }
    }

    public class ChatAttachmentProxy : AttachmentProxy
    {
        public ChatAttachmentProxy(int refID) : base(ReferenceType.ChatAttachments, refID) { }
        public int ChatID { get { return RefID; } }
    }

    public class CompanyActivityAttachmentProxy : AttachmentProxy
    {
        public CompanyActivityAttachmentProxy(int refID) : base(ReferenceType.CompanyActivity, refID) { }
    }


    public class ContactActivityAttachmentProxy : AttachmentProxy
    {
        public ContactActivityAttachmentProxy(int refID) : base(ReferenceType.ContactActivity, refID) { }
    }


    public class CustomerHubLogoAttachmentProxy : AttachmentProxy
    {
        public CustomerHubLogoAttachmentProxy(int refID) : base(ReferenceType.CustomerHubLogo, refID) { }
    }


    public class ProductVersionAttachmentProxy : AttachmentProxy
    {
        public ProductVersionAttachmentProxy(int refID) : base(ReferenceType.ProductVersions, refID) { }
    }


    public class TaskAttachmentProxy : AttachmentProxy
    {
        public TaskAttachmentProxy(int refID) : base(ReferenceType.Tasks, refID) { }
    }


    public class UserPhotoAttachmentProxy : AttachmentProxy
    {
        public UserPhotoAttachmentProxy(int refID) : base(ReferenceType.UserPhoto, refID) { }
    }


    public class UserAttachmentProxy : AttachmentProxy
    {
        public UserAttachmentProxy(int refID) : base(ReferenceType.Users, refID) { }
    }

}
