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
  [KnownType(typeof(MiscellaneousLogItemProxy))]
  public class MiscellaneousLogItemProxy
  {
    public MiscellaneousLogItemProxy() {}
    [DataMember] public int id { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
    [DataMember] public int? RefType { get; set; }
    [DataMember] public int? RefID { get; set; }
    [DataMember] public int? RefProcess { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
          
  }
  
  public partial class MiscellaneousLogItem : BaseItem
  {
    public MiscellaneousLogItemProxy GetProxy()
    {
      MiscellaneousLogItemProxy result = new MiscellaneousLogItemProxy();
      result.RefProcess = this.RefProcess;
      result.RefID = this.RefID;
      result.RefType = this.RefType;
      result.OrganizationID = this.OrganizationID;
      result.id = this.id;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
