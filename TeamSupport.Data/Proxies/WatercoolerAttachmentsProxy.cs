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
    [KnownType(typeof(WatercoolerAttachmentProxy))]
    public class WatercoolerAttachmentProxy
    {
        public WatercoolerAttachmentProxy(ReferenceType referenceType) { }
        [DataMember]
        public int MessageID { get; set; }
        [DataMember]
        public int AttachmentID { get; set; }
        [DataMember]
        protected References RefType { get; set; }
        [DataMember]
        public int CreatorID { get; set; }
        [DataMember]
        public string CreatorName { get; set; }
        [DataMember]
        public DateTime DateCreated { get; set; }
        [DataMember]
        public string GroupName { get; set; }
        [DataMember]
        public string TicketName { get; set; }
        [DataMember]
        public string ProductName { get; set; }
        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public AttachmentType ActivityRefType { get; set; }
        [DataMember]
        public string ActivityTitle { get; set; }
        [DataMember]
        public int ActivityRefID { get; set; }

        public static WatercoolerAttachmentProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Ticket: return new TicketWatercoolerAttachmentProxy();
                case References.Product: return new ProductWatercoolerAttachmentProxy();
                case References.Company: return new CompanyWatercoolerAttachmentProxy();
                case References.User: return new UserWatercoolerAttachmentProxy();
                case References.Group: return new GroupWatercoolerAttachmentProxy();
                case References.Activities: return new ActivitiesWatercoolerAttachmentProxy();


                default: throw new Exception("Invalid WatercoolerAttachment Reference Type");
            }
        }

        public enum References
        {
            Ticket = WaterCoolerAttachmentType.Ticket,
            Product = WaterCoolerAttachmentType.Product,
            Company = WaterCoolerAttachmentType.Company,
            User = WaterCoolerAttachmentType.User,
            Group = WaterCoolerAttachmentType.Group,
            Activities = WaterCoolerAttachmentType.Activities
        }

    }

    public class TicketWatercoolerAttachmentProxy : WatercoolerAttachmentProxy
    {
        public TicketWatercoolerAttachmentProxy() : base(ReferenceType.Users)
        {
        }
    }
    public class ProductWatercoolerAttachmentProxy : WatercoolerAttachmentProxy
    {
        public ProductWatercoolerAttachmentProxy() : base(ReferenceType.Users)
        {
        }
    }
    public class CompanyWatercoolerAttachmentProxy : WatercoolerAttachmentProxy
    {
        public CompanyWatercoolerAttachmentProxy() : base(ReferenceType.Users)
        {
        }
    }
    public class UserWatercoolerAttachmentProxy : WatercoolerAttachmentProxy
    {
        public UserWatercoolerAttachmentProxy() : base(ReferenceType.Users)
        {
        }
    }
    public class GroupWatercoolerAttachmentProxy : WatercoolerAttachmentProxy
    {
        public GroupWatercoolerAttachmentProxy() : base(ReferenceType.Users)
        {
        }
    }
    public class ActivitiesWatercoolerAttachmentProxy : WatercoolerAttachmentProxy
    {
        public ActivitiesWatercoolerAttachmentProxy() : base(ReferenceType.Users)
        {
        }
    }
    public partial class WatercoolerAttachment : BaseItem
    {
        public WatercoolerAttachmentProxy GetProxy()
        {
            WatercoolerAttachmentProxy result = WatercoolerAttachmentProxy.ClassFactory((WatercoolerAttachmentProxy.References)this.RefType);
            result.CreatorID = this.CreatorID;
            result.AttachmentID = this.AttachmentID;
            result.MessageID = this.MessageID;

            result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);

            result.CreatorName = Users.GetUserFullName(BaseCollection.LoginUser, this.CreatorID);

            if (this.RefType == WaterCoolerAttachmentType.Group)
            {
                var group = Groups.GetGroup(BaseCollection.LoginUser, this.AttachmentID);
                if (group != null)
                    result.GroupName = group.Name;
                else
                    return null;
            }

            if (this.RefType == WaterCoolerAttachmentType.Ticket)
            {
                var ticket = Tickets.GetTicketByNumber(BaseCollection.LoginUser, this.AttachmentID);
                if (ticket != null)
                    result.TicketName = ticket.Name;
                else
                    return null;
            }

            if (this.RefType == WaterCoolerAttachmentType.Product)
            {
                var product = Products.GetProduct(BaseCollection.LoginUser, this.AttachmentID);
                if (product != null)
                    result.ProductName = product.Name;
                else
                    return null;
            }

            if (this.RefType == WaterCoolerAttachmentType.Company)
            {
                var organization = Organizations.GetOrganization(BaseCollection.LoginUser, this.AttachmentID);
                if (organization != null)
                    result.CompanyName = organization.Name;
                else
                    return null;
            }

            if (this.RefType == WaterCoolerAttachmentType.User)
            {
                var user = Users.GetUserFullName(BaseCollection.LoginUser, this.AttachmentID);
                if (user != null)
                    result.UserName = user;
                else
                    return null;
            }

            if (this.RefType == WaterCoolerAttachmentType.Activities)
            {
                var activity = Notes.GetNote(BaseCollection.LoginUser, this.AttachmentID);
                if (activity != null)
                {
                    result.ActivityRefType = activity.RefType;
                    result.ActivityTitle = activity.Title;
                    result.ActivityRefID = activity.RefID;
                }
                else
                    return null;
            }

            return result;
        }
    }
}
