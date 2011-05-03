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
  [KnownType(typeof(SlaNotificationProxy))]
  public class SlaNotificationProxy
  {
    public SlaNotificationProxy() {}
    [DataMember] public int TicketID { get; set; }
    [DataMember] public DateTime? TimeClosedViolationDate { get; set; }
    [DataMember] public DateTime? LastActionViolationDate { get; set; }
    [DataMember] public DateTime? InitialResponseViolationDate { get; set; }
    [DataMember] public DateTime? TimeClosedWarningDate { get; set; }
    [DataMember] public DateTime? LastActionWarningDate { get; set; }
    [DataMember] public DateTime? InitialResponseWarningDate { get; set; }
          
  }
  
  public partial class SlaNotification : BaseItem
  {
    public SlaNotificationProxy GetProxy()
    {
      SlaNotificationProxy result = new SlaNotificationProxy();
      result.TicketID = this.TicketID;
       
       
      result.InitialResponseWarningDate = this.InitialResponseWarningDate == null ? this.InitialResponseWarningDate : DateTime.SpecifyKind((DateTime)this.InitialResponseWarningDate, DateTimeKind.Local); 
      result.LastActionWarningDate = this.LastActionWarningDate == null ? this.LastActionWarningDate : DateTime.SpecifyKind((DateTime)this.LastActionWarningDate, DateTimeKind.Local); 
      result.TimeClosedWarningDate = this.TimeClosedWarningDate == null ? this.TimeClosedWarningDate : DateTime.SpecifyKind((DateTime)this.TimeClosedWarningDate, DateTimeKind.Local); 
      result.InitialResponseViolationDate = this.InitialResponseViolationDate == null ? this.InitialResponseViolationDate : DateTime.SpecifyKind((DateTime)this.InitialResponseViolationDate, DateTimeKind.Local); 
      result.LastActionViolationDate = this.LastActionViolationDate == null ? this.LastActionViolationDate : DateTime.SpecifyKind((DateTime)this.LastActionViolationDate, DateTimeKind.Local); 
      result.TimeClosedViolationDate = this.TimeClosedViolationDate == null ? this.TimeClosedViolationDate : DateTime.SpecifyKind((DateTime)this.TimeClosedViolationDate, DateTimeKind.Local); 
       
      return result;
    }	
  }
}
