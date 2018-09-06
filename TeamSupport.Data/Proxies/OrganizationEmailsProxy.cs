using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class OrganizationEmail : BaseItem
  {
    public OrganizationEmailProxy GetProxy()
    {
      OrganizationEmailProxy result = new OrganizationEmailProxy();
      result.ProductFamilyID = this.ProductFamilyID;
      result.UseGlobalTemplate = this.UseGlobalTemplate;
      result.IsHtml = this.IsHtml;
      result.Body = (this.Body);
      result.Footer = (this.Footer);
      result.Header = (this.Header);
      result.Subject = (this.Subject);
      result.EmailTemplateID = this.EmailTemplateID;
      result.OrganizationID = this.OrganizationID;
      result.OrganizationEmailID = this.OrganizationEmailID;
       
       
       
      return result;
    }	
  }
}
