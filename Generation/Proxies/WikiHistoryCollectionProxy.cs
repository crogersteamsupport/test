using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(WikiHistoryProxy))]
  public class WikiHistoryProxy
  {
    public WikiHistoryProxy() {}
    [DataMember] public int HistoryID { get; set; }
    [DataMember] public int ArticleID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string ArticleName { get; set; }
    [DataMember] public string Body { get; set; }
    [DataMember] public int? Version { get; set; }
    [DataMember] public int? CreatedBy { get; set; }
    [DataMember] public DateTime? CreatedDate { get; set; }
    [DataMember] public int? ModifiedBy { get; set; }
    [DataMember] public DateTime? ModifiedDate { get; set; }
          
  }
  
  public partial class WikiHistory : BaseItem
  {
    public WikiHistoryProxy GetProxy()
    {
      WikiHistoryProxy result = new WikiHistoryProxy();
      result.ModifiedBy = this.ModifiedBy;
      result.CreatedBy = this.CreatedBy;
      result.Version = this.Version;
      result.Body = this.Body;
      result.ArticleName = this.ArticleName;
      result.OrganizationID = this.OrganizationID;
      result.ArticleID = this.ArticleID;
      result.HistoryID = this.HistoryID;
       
       
      result.ModifiedDate = this.ModifiedDate == null ? this.ModifiedDate : DateTime.SpecifyKind((DateTime)this.ModifiedDate, DateTimeKind.Local); 
      result.CreatedDate = this.CreatedDate == null ? this.CreatedDate : DateTime.SpecifyKind((DateTime)this.CreatedDate, DateTimeKind.Local); 
       
      return result;
    }	
  }
}
