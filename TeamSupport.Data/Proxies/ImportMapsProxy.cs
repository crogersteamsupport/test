using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ImportMap : BaseItem
  {
    public ImportMapProxy GetProxy()
    {
      ImportMapProxy result = new ImportMapProxy();
      result.IsCustom = this.IsCustom;
      result.FieldID = this.FieldID;
      result.SourceName = this.SourceName;
      result.ImportID = this.ImportID;
      result.ImportMapID = this.ImportMapID;
       
       
       
      return result;
    }	
  }
}
