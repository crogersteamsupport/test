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
  [KnownType(typeof(CRMLinkTableItemProxy))]
  public class CRMLinkTableItemProxy
  {
    public CRMLinkTableItemProxy() {}
    [DataMember] public int CRMLinkID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public bool Active { get; set; }
    [DataMember] public string CRMType { get; set; }
    [DataMember] public string Username { get; set; }
    [DataMember] public string Password { get; set; }
    [DataMember] public string SecurityToken1 { get; set; }
    [DataMember] public string TypeFieldMatch { get; set; }
    [DataMember] public DateTime? LastLink { get; set; }
    [DataMember] public bool SendBackTicketData { get; set; }
    [DataMember] public DateTime LastProcessed { get; set; }
    [DataMember] public int LastTicketID { get; set; }
    [DataMember] public bool AllowPortalAccess { get; set; }
    [DataMember] public bool SendWelcomeEmail { get; set; }
    [DataMember] public int? DefaultSlaLevelID { get; set; }
    [DataMember] public bool? PushTicketsAsCases { get; set; }
    [DataMember] public bool? PullCasesAsTickets { get; set; }
    [DataMember] public bool? PullCustomerProducts { get; set; }
    [DataMember] public int? ActionTypeIDToPush { get; set; }
    [DataMember] public string HostName { get; set; }
    [DataMember] public string DefaultProject { get; set; }
    [DataMember] public bool? UpdateStatus { get; set; }
    [DataMember] public bool MatchAccountsByName { get; set; }
          
  }
  
  public partial class CRMLinkTableItem : BaseItem
  {
    public CRMLinkTableItemProxy GetProxy()
    {
      CRMLinkTableItemProxy result = new CRMLinkTableItemProxy();
      result.MatchAccountsByName = this.MatchAccountsByName;
      result.UpdateStatus = this.UpdateStatus;
      result.DefaultProject = this.DefaultProject;
      result.HostName = this.HostName;
      result.ActionTypeIDToPush = this.ActionTypeIDToPush;
      result.PullCustomerProducts = this.PullCustomerProducts;
      result.PullCasesAsTickets = this.PullCasesAsTickets;
      result.PushTicketsAsCases = this.PushTicketsAsCases;
      result.SendWelcomeEmail = this.SendWelcomeEmail;
      result.AllowPortalAccess = this.AllowPortalAccess;
      result.LastTicketID = this.LastTicketID;
      result.SendBackTicketData = this.SendBackTicketData;
      result.TypeFieldMatch = this.TypeFieldMatch;
      result.SecurityToken1 = this.SecurityToken1;
      result.Password = this.Password;
      result.Username = this.Username;
      result.CRMType = this.CRMType;
      result.Active = this.Active;
      result.OrganizationID = this.OrganizationID;
      result.CRMLinkID = this.CRMLinkID;
      result.DefaultSlaLevelID = this.DefaultSlaLevelID;
       
      result.LastProcessed = DateTime.SpecifyKind(this.LastProcessedUtc, DateTimeKind.Utc);
       
      result.LastLink = this.LastLinkUtc == null ? this.LastLinkUtc : DateTime.SpecifyKind((DateTime)this.LastLinkUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
