using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ReportView : BaseItem
  {
    public ReportViewProxy GetProxy()
    {
      ReportViewProxy result = new ReportViewProxy();
      result.ErrorMessage = this.ErrorMessage;
      result.SQLExecuted = this.SQLExecuted;
      result.DurationToLoad = this.DurationToLoad;
      result.UserID = this.UserID;
      result.ReportID = this.ReportID;
      result.ReportViewID = this.ReportViewID;
       
      result.DateViewed = DateTime.SpecifyKind(this.DateViewedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
