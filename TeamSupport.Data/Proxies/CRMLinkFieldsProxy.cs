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
  [KnownType(typeof(CRMLinkFieldProxy))]
  public class CRMLinkFieldProxy
  {
    public CRMLinkFieldProxy() {}
    [DataMember] public int CRMFieldID { get; set; }
    [DataMember] public int CRMLinkID { get; set; }
    [DataMember] public string CRMObjectName { get; set; }
    [DataMember] public string CRMFieldName { get; set; }
    [DataMember] public int? CustomFieldID { get; set; }
    [DataMember] public string TSFieldName { get; set; }
          
  }
  
  public partial class CRMLinkField : BaseItem
  {
    public CRMLinkFieldProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      CRMLinkFieldProxy result = new CRMLinkFieldProxy();
      result.TSFieldName = sanitizer.Sanitize(this.TSFieldName);
      result.CustomFieldID = this.CustomFieldID;
      result.CRMFieldName = sanitizer.Sanitize(this.CRMFieldName);
      result.CRMObjectName = sanitizer.Sanitize(this.CRMObjectName);
      result.CRMLinkID = this.CRMLinkID;
      result.CRMFieldID = this.CRMFieldID;
       
       
       
      return result;
    }	
  }
}
