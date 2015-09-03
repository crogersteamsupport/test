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
  [KnownType(typeof(ExceptionLogViewItemProxy))]
  public class ExceptionLogViewItemProxy
  {
    public ExceptionLogViewItemProxy() {}
    [DataMember] public int ExceptionLogID { get; set; }
    [DataMember] public string URL { get; set; }
    [DataMember] public string PageInfo { get; set; }
    [DataMember] public string ExceptionName { get; set; }
    [DataMember] public string Message { get; set; }
    [DataMember] public string StackTrace { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public string FirstName { get; set; }
    [DataMember] public string LastName { get; set; }
    [DataMember] public string Name { get; set; }
          
  }
  
  public partial class ExceptionLogViewItem : BaseItem
  {
    public ExceptionLogViewItemProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      ExceptionLogViewItemProxy result = new ExceptionLogViewItemProxy();
      result.Name = sanitizer.Sanitize(this.Name);
      result.LastName = sanitizer.Sanitize(this.LastName);
      result.FirstName = sanitizer.Sanitize(this.FirstName);
      result.CreatorID = this.CreatorID;
      result.StackTrace = sanitizer.Sanitize(this.StackTrace);
      result.Message = sanitizer.Sanitize(this.Message);
      result.ExceptionName = sanitizer.Sanitize(this.ExceptionName);
      result.PageInfo = sanitizer.Sanitize(this.PageInfo);
      result.URL = sanitizer.Sanitize(this.URL);
      result.ExceptionLogID = this.ExceptionLogID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
