using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ReportTableField : BaseItem
  {
    public ReportTableFieldProxy GetProxy()
    {
      ReportTableFieldProxy result = new ReportTableFieldProxy();
      result.LookupTableID = this.LookupTableID;
      result.Description = (this.Description);
      result.IsVisible = this.IsVisible;
      result.Size = this.Size;
      result.DataType = this.DataType;
      result.Alias = (this.Alias);
      result.FieldName = (this.FieldName);
      result.ReportTableID = this.ReportTableID;
      result.ReportTableFieldID = this.ReportTableFieldID;
       
       
       
      return result;
    }	
  }
}
