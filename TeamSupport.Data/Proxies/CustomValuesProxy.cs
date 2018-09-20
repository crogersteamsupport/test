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
    [KnownType(typeof(CustomValueProxy))]
    public class CustomValueProxy
    {
        public CustomValueProxy(ReferenceType referenceType) { }
        [DataMember] public int? CustomValueID { get; set; }
        [DataMember] public int CustomFieldID { get; set; }
        [DataMember] public int? RefID { get; set; }
        [DataMember] public object Value { get; set; }
        [DataMember] public DateTime DateCreated { get; set; }
        [DataMember] public DateTime DateModified { get; set; }
        [DataMember] public int CreatorID { get; set; }
        [DataMember] public int ModifierID { get; set; }
        [DataMember] public int? ImportFileID { get; set; }

        /* Custom Fields Info */
        [DataMember] public string FieldName { get; set; }
        [DataMember] public string ApiFieldName { get; set; }
        [DataMember] public string ListValues { get; set; }
        [DataMember] public CustomFieldType FieldType { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] protected References RefType { get; set; }
        [DataMember] public int? AuxID { get; set; }
        [DataMember] public int Position { get; set; }
        [DataMember] public bool IsVisibleOnPortal { get; set; }
        [DataMember] public bool IsFirstIndexSelect { get; set; }
        [DataMember] public bool IsRequired { get; set; }
        [DataMember] public int OrganizationID { get; set; }
        [DataMember] public bool IsRequiredToClose { get; set; }
        [DataMember] public string Mask { get; set; }
        [DataMember]
        public int CustomFieldCategoryID { get; set; }

        public static CustomValueProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Actions: return new ActionsCustomValueProxy();
                case References.OrganizationProducts: return new ActionsCustomValueProxy();
                case References.Organizations: return new ActionsCustomValueProxy();
                case References.Tickets: return new ActionsCustomValueProxy();
                case References.Contacts: return new ActionsCustomValueProxy();
                case References.Assets: return new ActionsCustomValueProxy();
                case References.Products: return new ProductCustomValueProxy();
                default: throw new Exception("Invalid CustomValueProxy Reference Type");
            }
        }
        public enum References
        {
            Actions = ReferenceType.Actions,
            OrganizationProducts = ReferenceType.OrganizationProducts,
            Organizations = ReferenceType.Organizations,
            Tickets = ReferenceType.Tickets,
            Contacts = ReferenceType.Contacts,
            Assets = ReferenceType.Assets,
            Products = ReferenceType.Products
        }
    }



    public class ActionsCustomValueProxy : CustomValueProxy
    {
        public ActionsCustomValueProxy() : base(ReferenceType.Actions){}
    }
    public class OrganizationProductsCustomValueProxy : CustomValueProxy
    {
        public OrganizationProductsCustomValueProxy() : base(ReferenceType.OrganizationProducts) { }
    }
    public class OrganizationsCustomValueProxy : CustomValueProxy
    {
        public OrganizationsCustomValueProxy() : base(ReferenceType.Organizations) { }
    }
    public class TicketsCustomValueProxy : CustomValueProxy
    {
        public TicketsCustomValueProxy() : base(ReferenceType.Tickets) { }
    }
    public class ContactsCustomValueProxy : CustomValueProxy
    {
        public ContactsCustomValueProxy() : base(ReferenceType.Contacts) { }
    }
    public class AssetsCustomValueProxy : CustomValueProxy
    {
        public AssetsCustomValueProxy() : base(ReferenceType.Assets) { }
    }

    public class ProductCustomValueProxy : CustomValueProxy
    {
        public ProductCustomValueProxy() : base(ReferenceType.Products) { }
    }

    public partial class CustomValue : BaseItem
    {
        public CustomValueProxy GetProxy()
        {
            CustomValueProxy result = CustomValueProxy.ClassFactory((CustomValueProxy.References)this.RefType); ; ;
            result.ImportFileID = this.ImportFileID;
            result.ModifierID = Row["ModifierID"] == DBNull.Value ? -1 : this.ModifierID;
            result.CreatorID = Row["CreatorID"] == DBNull.Value ? -1 : this.CreatorID;
            result.RefID = Row["RefID"] == DBNull.Value ? null : (int?)this.RefID;
            result.CustomFieldID = this.CustomFieldID;
            result.CustomValueID = Row["CustomValueID"] == DBNull.Value ? null : (int?)this.CustomValueID;

            result.DateCreated = DateTime.SpecifyKind(Row["DateCreated"] == DBNull.Value ? DateTime.MinValue : this.DateCreated, DateTimeKind.Local);
            result.DateModified = DateTime.SpecifyKind(Row["DateModified"] == DBNull.Value ? DateTime.MinValue : this.DateModified, DateTimeKind.Local);

            result.FieldName = this.FieldName;
            result.ApiFieldName = this.ApiFieldName;
            result.ListValues = this.ListValues;
            result.FieldType = this.FieldType;
            result.Name = this.Name;
            result.Description = this.Description;
            result.AuxID = this.AuxID;
            result.Position = this.Position;
            result.IsVisibleOnPortal = this.IsVisibleOnPortal;
            result.IsFirstIndexSelect = this.IsFirstIndexSelect;
            result.IsRequired = this.IsRequired;
            result.OrganizationID = this.OrganizationID;
            result.IsRequiredToClose = this.IsRequiredToClose;
            result.Mask = this.Mask;
            result.CustomFieldCategoryID = this.CustomFieldCategoryID;

            switch (this.FieldType)
            {
                case CustomFieldType.DateTime:
                case CustomFieldType.Time:
                    result.Value = null;
                    if (Row["CustomValue"] != DBNull.Value)
                    {
                        DateTime date;
                        if (DateTime.TryParse(this.Value, out date))
                        {
                            date = DateTime.SpecifyKind(DateTime.Parse(this.Value), DateTimeKind.Utc);
                            result.Value = date;
                        }
                    }
                    break;
                case CustomFieldType.Date:
                    result.Value = null;
                    if (Row["CustomValue"] != DBNull.Value)
                    {
                        DateTime date;
                        if (DateTime.TryParse(this.Value, out date))
                        {
                            result.Value = this.Value;
                        }
                    }
                    break;
                default:
                    result.Value = Row["CustomValue"] == DBNull.Value ? "" : this.Value;
                    break;
            }

            return result;
        }
    }
}
