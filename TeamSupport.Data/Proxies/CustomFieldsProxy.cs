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
  [KnownType(typeof(CustomFieldProxy))]
  public class CustomFieldProxy
  {
    public CustomFieldProxy() {}
    [DataMember] public int CustomFieldID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int? CustomFieldCategoryID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string ApiFieldName { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
    [DataMember] public CustomFieldType FieldType { get; set; }
    [DataMember] public int AuxID { get; set; }
    [DataMember] public int Position { get; set; }
    [DataMember] public string ListValues { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public bool? IsVisibleOnPortal { get; set; }
    [DataMember] public bool IsFirstIndexSelect { get; set; }
    [DataMember] public bool IsRequired { get; set; }
    /*
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    */      
    [DataMember] public bool IsRequiredToClose { get; set; }
    [DataMember] public string Mask { get; set; }
    [DataMember] public int? ParentCustomFieldID { get; set; }
    [DataMember] public string ParentCustomValue { get; set; }
    [DataMember] public int? ParentProductID { get; set; }
          
  }
  
  public partial class CustomField : BaseItem
  {
    public CustomFieldProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      CustomFieldProxy result = new CustomFieldProxy();
      result.ParentProductID = this.ParentProductID;
      result.ParentCustomValue = sanitizer.Sanitize(this.ParentCustomValue);
      result.ParentCustomFieldID = this.ParentCustomFieldID;
      result.Mask = sanitizer.Sanitize(this.Mask);
      result.IsRequiredToClose = this.IsRequiredToClose;
      result.IsRequired = this.IsRequired;
      result.IsFirstIndexSelect = this.IsFirstIndexSelect;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.Description = sanitizer.Sanitize(this.Description);
      result.ListValues = sanitizer.Sanitize(this.ListValues);
      result.Position = this.Position;
      result.AuxID = this.AuxID;
      result.FieldType = this.FieldType;
      result.RefType = this.RefType;
      result.ApiFieldName = sanitizer.Sanitize(this.ApiFieldName);
      result.Name = sanitizer.Sanitize(this.Name);
      result.OrganizationID = this.OrganizationID;
      result.CustomFieldID = this.CustomFieldID;
      result.CustomFieldCategoryID = this.CustomFieldCategoryID;

      /*
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       */
       
       
      return result;
    }	
  }
}
