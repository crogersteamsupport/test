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
  [KnownType(typeof(EMailAlternateInboundItemProxy))]
  public class EMailAlternateInboundItemProxy
  {
    public EMailAlternateInboundItemProxy() {}
    [DataMember] public Guid SystemEMailID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int? GroupToAssign { get; set; }
    [DataMember] public int? DefaultTicketType { get; set; }
    [DataMember] public int? ProductID { get; set; }
          
  }
  
  public partial class EMailAlternateInboundItem : BaseItem
  {
    public EMailAlternateInboundItemProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      EMailAlternateInboundItemProxy result = new EMailAlternateInboundItemProxy();
      result.ProductID = this.ProductID;
      result.DefaultTicketType = this.DefaultTicketType;
      result.GroupToAssign = this.GroupToAssign;
      result.Description = sanitizer.Sanitize(this.Description);
      result.OrganizationID = this.OrganizationID;
      result.SystemEMailID = this.SystemEMailID;
       
       
       
      return result;
    }	
  }
}
