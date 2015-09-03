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
  [KnownType(typeof(ApiLogProxy))]
  public class ApiLogProxy
  {
    public ApiLogProxy() {}
    [DataMember] public int ApiLogID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string IPAddress { get; set; }
    [DataMember] public string Url { get; set; }
    [DataMember] public string Verb { get; set; }
    [DataMember] public int StatusCode { get; set; }
    [DataMember] public string RequestBody { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
          
  }
  
  public partial class ApiLog : BaseItem
  {
    public ApiLogProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      ApiLogProxy result = new ApiLogProxy();
      result.RequestBody = sanitizer.Sanitize(this.RequestBody);
      result.StatusCode = this.StatusCode;
      result.Verb = sanitizer.Sanitize(this.Verb);
      result.Url = sanitizer.Sanitize(this.Url);
      result.IPAddress = sanitizer.Sanitize(this.IPAddress);
      result.OrganizationID = this.OrganizationID;
      result.ApiLogID = this.ApiLogID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
