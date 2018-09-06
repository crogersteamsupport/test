using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class SearchCustomFilter : BaseItem
  {
    public SearchCustomFilterProxy GetProxy()
    {
      SearchCustomFilterProxy result = new SearchCustomFilterProxy();
      result.MatchAll = this.MatchAll;
      result.TestValue = this.TestValue;
      result.Measure = this.Measure;
      result.FieldID = this.FieldID;
      result.TableID = this.TableID;
      result.UserID = this.UserID;
      result.CustomFilterID = this.CustomFilterID;
       
       
       
      return result;
    }	
  }
}
