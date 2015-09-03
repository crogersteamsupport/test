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
  [KnownType(typeof(ArticleStatProxy))]
  public class ArticleStatProxy
  {
    public ArticleStatProxy() {}
    [DataMember] public int ArticleViewID { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
    [DataMember] public int? ArticleID { get; set; }
    [DataMember] public DateTime? ViewDateTime { get; set; }
    [DataMember] public string ViewIP { get; set; }
    [DataMember] public int? UserID { get; set; }
          
  }
  
  public partial class ArticleStat : BaseItem
  {
    public ArticleStatProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      ArticleStatProxy result = new ArticleStatProxy();
      result.UserID = this.UserID;
      result.ViewIP = sanitizer.Sanitize(this.ViewIP);
      result.ArticleID = this.ArticleID;
      result.OrganizationID = this.OrganizationID;
      result.ArticleViewID = this.ArticleViewID;
       
       
      result.ViewDateTime = this.ViewDateTimeUtc == null ? this.ViewDateTimeUtc : DateTime.SpecifyKind((DateTime)this.ViewDateTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
