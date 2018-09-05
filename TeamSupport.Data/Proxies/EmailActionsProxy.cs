using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class EmailAction : BaseItem
  {
    public EmailActionProxy GetProxy()
    {
      EmailActionProxy result = new EmailActionProxy();
      result.OrganizationID = this.OrganizationID;
      result.TicketID = this.TicketID;
      result.Status = this.Status;
      result.ActionAdded = this.ActionAdded;
      result.OrganizationGUID = this.OrganizationGUID;
      result.EMailBody = this.EMailBody;
      result.EMailSubject = this.EMailSubject;
      result.EMailTo = this.EMailTo;
      result.EMailFrom = this.EMailFrom;
      result.EMailActionID = this.EMailActionID;
       
       
      //result.DateTime = this.DateTimeUtc == null ? this.DateTimeUtc : DateTime.SpecifyKind((DateTime)this.DateTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
