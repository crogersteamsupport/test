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
  [KnownType(typeof(ReminderProxy))]
  public class ReminderProxy
  {
    public ReminderProxy() {}
    [DataMember] public int ReminderID { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public DateTime DueDate { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public bool IsDismissed { get; set; }
    [DataMember] public bool HasEmailSent { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public string TaskName { get; set; }
    [DataMember] public DateTime? TaskDueDate { get; set; }
    [DataMember] public bool TaskIsComplete { get; set; }
          
  }
  
  public partial class Reminder : BaseItem
  {
    public ReminderProxy GetProxy()
    {
      ReminderProxy result = new ReminderProxy();
      result.TaskIsComplete = this.TaskIsComplete;
      result.TaskName = this.TaskName;
      result.CreatorID = this.CreatorID;
      result.HasEmailSent = this.HasEmailSent;
      result.IsDismissed = this.IsDismissed;
      result.UserID = this.UserID;
      result.Description = (this.Description);
      result.RefID = this.RefID;
      result.RefType = this.RefType;
      result.ReminderID = this.ReminderID;
       
      result.DueDate = DateTime.SpecifyKind(this.DueDateUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
      result.TaskDueDate = this.TaskDueDateUtc == null ? this.TaskDueDateUtc : DateTime.SpecifyKind((DateTime)this.TaskDueDateUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
