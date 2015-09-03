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
  [KnownType(typeof(CRMLinkErrorProxy))]
  public class CRMLinkErrorProxy
  {
    public CRMLinkErrorProxy() {}
    [DataMember] public int CRMLinkErrorID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string CRMType { get; set; }
    [DataMember] public string Orientation { get; set; }
    [DataMember] public string ObjectType { get; set; }
    [DataMember] public string ObjectID { get; set; }
    [DataMember] public string ObjectFieldName { get; set; }
    [DataMember] public string ObjectData { get; set; }
    [DataMember] public string Exception { get; set; }
    [DataMember] public string OperationType { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
          
  }
  
  public partial class CRMLinkError : BaseItem
  {
    public CRMLinkErrorProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      CRMLinkErrorProxy result = new CRMLinkErrorProxy();
      result.OperationType = sanitizer.Sanitize(this.OperationType);
      result.Exception = sanitizer.Sanitize(this.Exception);
      result.ObjectData = sanitizer.Sanitize(this.ObjectData);
      result.ObjectFieldName = sanitizer.Sanitize(this.ObjectFieldName);
      result.ObjectID = sanitizer.Sanitize(this.ObjectID);
      result.ObjectType = sanitizer.Sanitize(this.ObjectType);
      result.Orientation = sanitizer.Sanitize(this.Orientation);
      result.CRMType = sanitizer.Sanitize(this.CRMType);
      result.OrganizationID = this.OrganizationID;
      result.CRMLinkErrorID = this.CRMLinkErrorID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
