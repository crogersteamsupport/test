using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(CalendarRefItemProxy))]
  public class CalendarRefItemProxy
  {
    public CalendarRefItemProxy() {}
    [DataMember] public int CalendarID { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public CalendarAttachmentType RefType { get; set; }
          
  }
  
  public partial class CalendarRefItem : BaseItem
  {
    public CalendarRefItemProxy GetProxy()
    {
      CalendarRefItemProxy result = new CalendarRefItemProxy();
      result.RefType = this.RefType;
      result.RefID = this.RefID;
      result.CalendarID = this.CalendarID;
       
       
       
      return result;
    }	
  }
}
