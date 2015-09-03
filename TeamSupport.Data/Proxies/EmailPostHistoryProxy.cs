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
  [KnownType(typeof(EmailPostHistoryItemProxy))]
  public class EmailPostHistoryItemProxy
  {
    public EmailPostHistoryItemProxy() {}
    [DataMember] public int EmailPostID { get; set; }
    [DataMember] public int EmailPostType { get; set; }
    [DataMember] public int HoldTime { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public string Param1 { get; set; }
    [DataMember] public string Param2 { get; set; }
    [DataMember] public string Param3 { get; set; }
    [DataMember] public string Param4 { get; set; }
    [DataMember] public string Param5 { get; set; }
    [DataMember] public string Param6 { get; set; }
    [DataMember] public string Param7 { get; set; }
    [DataMember] public string Param8 { get; set; }
    [DataMember] public string Param9 { get; set; }
    [DataMember] public string Param10 { get; set; }
    [DataMember] public string Text1 { get; set; }
    [DataMember] public string Text2 { get; set; }
    [DataMember] public string Text3 { get; set; }
          
  }
  
  public partial class EmailPostHistoryItem : BaseItem
  {
    public EmailPostHistoryItemProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      EmailPostHistoryItemProxy result = new EmailPostHistoryItemProxy();
      result.Text3 = sanitizer.Sanitize(this.Text3);
      result.Text2 = sanitizer.Sanitize(this.Text2);
      result.Text1 = sanitizer.Sanitize(this.Text1);
      result.Param10 = sanitizer.Sanitize(this.Param10);
      result.Param9 = sanitizer.Sanitize(this.Param9);
      result.Param8 = sanitizer.Sanitize(this.Param8);
      result.Param7 = sanitizer.Sanitize(this.Param7);
      result.Param6 = sanitizer.Sanitize(this.Param6);
      result.Param5 = sanitizer.Sanitize(this.Param5);
      result.Param4 = sanitizer.Sanitize(this.Param4);
      result.Param3 = sanitizer.Sanitize(this.Param3);
      result.Param2 = sanitizer.Sanitize(this.Param2);
      result.Param1 = sanitizer.Sanitize(this.Param1);
      result.CreatorID = this.CreatorID;
      result.HoldTime = this.HoldTime;
      result.EmailPostType = this.EmailPostType;
      result.EmailPostID = this.EmailPostID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
