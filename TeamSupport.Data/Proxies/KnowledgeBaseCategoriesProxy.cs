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
  [KnownType(typeof(KnowledgeBaseCategoryProxy))]
  public class KnowledgeBaseCategoryProxy
  {
    public KnowledgeBaseCategoryProxy() {}
    [DataMember] public int CategoryID { get; set; }
    [DataMember] public int ParentID { get; set; }
    [DataMember] public string CategoryName { get; set; }
    [DataMember] public string CategoryDesc { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int? Position { get; set; }
    [DataMember] public bool? VisibleOnPortal { get; set; }
          
  }
  
  public partial class KnowledgeBaseCategory : BaseItem
  {
    public KnowledgeBaseCategoryProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      KnowledgeBaseCategoryProxy result = new KnowledgeBaseCategoryProxy();
      result.VisibleOnPortal = this.VisibleOnPortal;
      result.Position = this.Position;
      result.OrganizationID = this.OrganizationID;
      result.CategoryDesc = sanitizer.Sanitize(this.CategoryDesc);
      result.CategoryName = sanitizer.Sanitize(this.CategoryName);
      result.ParentID = this.ParentID;
      result.CategoryID = this.CategoryID;
       
       
       
      return result;
    }	
  }
}
