using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ReportField : BaseItem
  {
    public ReportFieldProxy GetProxy()
    {
      ReportFieldProxy result = new ReportFieldProxy();
      result.IsCustomField = this.IsCustomField;
      result.LinkedFieldID = this.LinkedFieldID;
      result.ReportID = this.ReportID;
      result.ReportFieldID = this.ReportFieldID;
       
       
       
      return result;
    }	
  }
}
