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
  [KnownType(typeof(ExceptionLogProxy))]
  public class ExceptionLogProxy
  {
    public ExceptionLogProxy() {}
    [DataMember] public int ExceptionLogID { get; set; }
    [DataMember] public string URL { get; set; }
    [DataMember] public string PageInfo { get; set; }
    [DataMember] public string ExceptionName { get; set; }
    [DataMember] public string Message { get; set; }
    [DataMember] public string StackTrace { get; set; }
    [DataMember] public string Browser { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
          
  }
  
  public partial class ExceptionLog : BaseItem
  {
    public ExceptionLogProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      ExceptionLogProxy result = new ExceptionLogProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.Browser = sanitizer.Sanitize(this.Browser);
      result.StackTrace = sanitizer.Sanitize(this.StackTrace);
      result.Message = sanitizer.Sanitize(this.Message);
      result.ExceptionName = sanitizer.Sanitize(this.ExceptionName);
      result.PageInfo = sanitizer.Sanitize(this.PageInfo);
      result.URL = sanitizer.Sanitize(this.URL);
      result.ExceptionLogID = this.ExceptionLogID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
