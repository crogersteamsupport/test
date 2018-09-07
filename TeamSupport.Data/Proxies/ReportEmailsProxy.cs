using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ReportEmail : BaseItem
  {
    public ReportEmailProxy GetProxy()
    {
      ReportEmailProxy result = new ReportEmailProxy();
      result.ReportID = this.ReportID;
      result.UserID = this.UserID;
      result.ReportEmailID = this.ReportEmailID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
      result.DateSent = this.DateSentUtc == null ? this.DateSentUtc : DateTime.SpecifyKind((DateTime)this.DateSentUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
