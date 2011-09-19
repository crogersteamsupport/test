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
  [KnownType(typeof(ActionProxy))]
  public class ActionProxy
  {
    public ActionProxy() {}
    [DataMember] public int ActionID { get; set; }
    [DataMember] public int? ActionTypeID { get; set; }
    [DataMember] public SystemActionType SystemActionTypeID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public int? TimeSpent { get; set; }
    [DataMember] public DateTime? DateStarted { get; set; }
    [DataMember] public bool IsVisibleOnPortal { get; set; }
    [DataMember] public bool IsKnowledgeBase { get; set; }
    [DataMember] public string ImportID { get; set; }
    [DataMember] public DateTime? DateCreated { get; set; }
    [DataMember] public DateTime? DateModified { get; set; }
    [DataMember] public int? CreatorID { get; set; }
    [DataMember] public int? ModifierID { get; set; }
    [DataMember] public int TicketID { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public string DisplayName { get; set; }
          
  }
  
  public partial class Action : BaseItem
  {
    public ActionProxy GetProxy()
    {
      ActionProxy result = new ActionProxy();
      result.Description = this.Description;
      result.TicketID = this.TicketID;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.ImportID = this.ImportID;
      result.IsKnowledgeBase = this.IsKnowledgeBase;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.TimeSpent = this.TimeSpent;
      result.Name = this.Name;
      result.SystemActionTypeID = this.SystemActionTypeID;
      result.ActionTypeID = this.ActionTypeID;
      result.ActionID = this.ActionID;
      result.DisplayName = this.DisplayName;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      result.DateStarted = this.DateStartedUtc == null ? this.DateStartedUtc : DateTime.SpecifyKind((DateTime)this.DateStartedUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
