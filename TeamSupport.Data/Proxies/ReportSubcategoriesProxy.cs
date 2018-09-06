using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ReportSubcategory : BaseItem
  {
    public ReportSubcategoryProxy GetProxy()
    {
      ReportSubcategoryProxy result = new ReportSubcategoryProxy();
      result.BaseQuery = this.BaseQuery;
      result.ReportTableID = this.ReportTableID;
      result.ReportCategoryTableID = this.ReportCategoryTableID;
      result.ReportSubcategoryID = this.ReportSubcategoryID;
       
       
       
      return result;
    }	
  }
}
