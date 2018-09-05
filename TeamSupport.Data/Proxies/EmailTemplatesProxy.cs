using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class EmailTemplate : BaseItem
  {
    public EmailTemplateProxy GetProxy()
    {
      EmailTemplateProxy result = new EmailTemplateProxy();
      result.Body = this.Body;
      result.UseGlobalTemplate = this.UseGlobalTemplate;
      result.IsEmail = this.IsEmail;
      result.IncludeDelimiter = this.IncludeDelimiter;
      result.IsHtml = this.IsHtml;
      result.Footer = this.Footer;
      result.Header = this.Header;
      result.Subject = this.Subject;
      result.IsTSOnly = this.IsTSOnly;
      result.Description = this.Description;
      result.Name = this.Name;
      result.Position = this.Position;
      result.EmailTemplateID = this.EmailTemplateID;
       
       
       
      return result;
    }	
  }
}
