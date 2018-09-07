using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class SearchSorter : BaseItem
  {
    public SearchSorterProxy GetProxy()
    {
      SearchSorterProxy result = new SearchSorterProxy();
      result.Descending = this.Descending;
      result.FieldID = this.FieldID;
      result.TableID = this.TableID;
      result.UserID = this.UserID;
      result.SorterID = this.SorterID;
       
       
       
      return result;
    }	
  }
}
