using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class EmailTemplateTable : BaseItem
  {
    public EmailTemplateTableProxy GetProxy()
    {
      EmailTemplateTableProxy result = new EmailTemplateTableProxy();
      result.Description = this.Description;
      result.Alias = this.Alias;
      result.ReportTableID = this.ReportTableID;
      result.EmailTemplateID = this.EmailTemplateID;
      result.EmailTemplateTableID = this.EmailTemplateTableID;
       
       
       
      return result;
    }	
  }
}
