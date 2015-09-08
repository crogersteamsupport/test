using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Ganss.XSS;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(CustomValueProxy))]
  public class CustomValueProxy
  {
    public CustomValueProxy() {}
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
    [DataMember] public ReferenceType RefType { get; set; }
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
  }
  
  public partial class CustomValue : BaseItem
  {
    public CustomValueProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      CustomValueProxy result = new CustomValueProxy();
      result.ImportFileID = this.ImportFileID;
      result.ModifierID = Row["ModifierID"] == DBNull.Value ? -1 : this.ModifierID;
      result.CreatorID = Row["CreatorID"] == DBNull.Value ? -1 : this.CreatorID;
      result.RefID = Row["RefID"] == DBNull.Value ? null : (int?)this.RefID;
      result.CustomFieldID = this.CustomFieldID;
      result.CustomValueID = Row["CustomValueID"] == DBNull.Value ? null : (int?)this.CustomValueID;
       
      result.DateCreated = DateTime.SpecifyKind(Row["DateCreated"] == DBNull.Value ? DateTime.MinValue : this.DateCreated, DateTimeKind.Local);
      result.DateModified = DateTime.SpecifyKind(Row["DateModified"] == DBNull.Value ? DateTime.MinValue : this.DateModified, DateTimeKind.Local);
       
      result.FieldName = sanitizer.Sanitize(this.FieldName);
      result.ApiFieldName = sanitizer.Sanitize(this.ApiFieldName);
      result.ListValues = sanitizer.Sanitize(this.ListValues);
      result.FieldType = this.FieldType;
      result.Name = sanitizer.Sanitize(this.Name);
      result.Description = sanitizer.Sanitize(this.Description);
      result.RefType = this.RefType;
      result.AuxID = this.AuxID;
      result.Position = this.Position;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.IsFirstIndexSelect = this.IsFirstIndexSelect;
      result.IsRequired = this.IsRequired;
      result.OrganizationID = this.OrganizationID;
      result.IsRequiredToClose = this.IsRequiredToClose;
      result.Mask = sanitizer.Sanitize(this.Mask);
      result.CustomFieldCategoryID = this.CustomFieldCategoryID;

      if (this.FieldType == CustomFieldType.DateTime || this.FieldType == CustomFieldType.Date || this.FieldType == CustomFieldType.Time)
      {
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
      }
      else
      {
        result.Value = Row["CustomValue"] == DBNull.Value ? "" : sanitizer.Sanitize(this.Value);
      }

      return result;
    }	
  }
}
