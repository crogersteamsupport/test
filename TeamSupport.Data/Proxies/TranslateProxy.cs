using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TranslateItem : BaseItem
  {
    public TranslateItemProxy GetProxy()
    {
      TranslateItemProxy result = new TranslateItemProxy();
      result.Portugese = this.Portugese;
      result.Spanish = this.Spanish;
      result.German = this.German;
      result.Italian = this.Italian;
      result.French = this.French;
      result.English = this.English;
      result.PhraseID = this.PhraseID;
       
       
       
      return result;
    }	
  }
}
