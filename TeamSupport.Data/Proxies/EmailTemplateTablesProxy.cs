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
  [KnownType(typeof(EmailTemplateTableProxy))]
  public class EmailTemplateTableProxy
  {
    public EmailTemplateTableProxy() {}
    [DataMember] public int EmailTemplateTableID { get; set; }
    [DataMember] public int EmailTemplateID { get; set; }
    [DataMember] public int ReportTableID { get; set; }
    [DataMember] public string Alias { get; set; }
    [DataMember] public string Description { get; set; }
          
  }
  
  public partial class EmailTemplateTable : BaseItem
  {
    public EmailTemplateTableProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      EmailTemplateTableProxy result = new EmailTemplateTableProxy();
      result.Description = sanitizer.Sanitize(this.Description);
      result.Alias = sanitizer.Sanitize(this.Alias);
      result.ReportTableID = this.ReportTableID;
      result.EmailTemplateID = this.EmailTemplateID;
      result.EmailTemplateTableID = this.EmailTemplateTableID;
       
       
       
      return result;
    }	
  }
}
