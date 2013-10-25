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
        public WatercoolerAttachmentProxy() { }
        [DataMember]
        public int MessageID { get; set; }
        [DataMember]
        public int AttachmentID { get; set; }
        [DataMember]
        public WaterCoolerAttachmentType RefType { get; set; }
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
    }

    public partial class WatercoolerAttachment : BaseItem
    {
        public WatercoolerAttachmentProxy GetProxy()
        {
            WatercoolerAttachmentProxy result = new WatercoolerAttachmentProxy();
            result.CreatorID = this.CreatorID;
            result.RefType = this.RefType;
            result.AttachmentID = this.AttachmentID;
            result.MessageID = this.MessageID;

            result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);

            result.CreatorName = Users.GetUserFullName(BaseCollection.LoginUser, this.CreatorID);

            if (this.RefType == WaterCoolerAttachmentType.Group)
            {
                if (Groups.GetGroup(BaseCollection.LoginUser, this.AttachmentID) != null)
                    result.GroupName = Groups.GetGroup(BaseCollection.LoginUser, this.AttachmentID).Name;
                else
                    return null;
            }

            if (this.RefType == WaterCoolerAttachmentType.Ticket)
            {
                if (Tickets.GetTicketByNumber(BaseCollection.LoginUser, this.AttachmentID) != null)
                    result.TicketName = Tickets.GetTicketByNumber(BaseCollection.LoginUser, this.AttachmentID).Name;
                else
                    return null;
            }

            if (this.RefType == WaterCoolerAttachmentType.Product)
            {
                if (Products.GetProduct(BaseCollection.LoginUser, this.AttachmentID) != null)
                    result.ProductName = Products.GetProduct(BaseCollection.LoginUser, this.AttachmentID).Name;
                else
                    return null;
            }

            if (this.RefType == WaterCoolerAttachmentType.Company)
            {
                if (Organizations.GetOrganization(BaseCollection.LoginUser, this.AttachmentID) != null)
                    result.CompanyName = Organizations.GetOrganization(BaseCollection.LoginUser, this.AttachmentID).Name;
                else
                    return null;
            }

            if (this.RefType == WaterCoolerAttachmentType.User)
            {
                if (Users.GetUserFullName(BaseCollection.LoginUser, this.AttachmentID) != null)
                    result.UserName = Users.GetUserFullName(BaseCollection.LoginUser, this.AttachmentID);
                else
                    return null;
            }

            return result;
        }
    }
}
