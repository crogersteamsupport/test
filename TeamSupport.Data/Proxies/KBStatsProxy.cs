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
  [KnownType(typeof(KBStatProxy))]
  public class KBStatProxy
  {
    public KBStatProxy() {}
    [DataMember] public int KBViewID { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
    [DataMember] public int? KBArticleID { get; set; }
    [DataMember] public DateTime? ViewDateTime { get; set; }
    [DataMember] public string ViewIP { get; set; }
    [DataMember] public string SearchTerm { get; set; }
          
  }
  
  public partial class KBStat : BaseItem
  {
    public KBStatProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      KBStatProxy result = new KBStatProxy();
      result.SearchTerm = sanitizer.Sanitize(this.SearchTerm);
      result.ViewIP = sanitizer.Sanitize(this.ViewIP);
      result.KBArticleID = this.KBArticleID;
      result.OrganizationID = this.OrganizationID;
      result.KBViewID = this.KBViewID;
       
       
      result.ViewDateTime = this.ViewDateTimeUtc == null ? this.ViewDateTimeUtc : DateTime.SpecifyKind((DateTime)this.ViewDateTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
