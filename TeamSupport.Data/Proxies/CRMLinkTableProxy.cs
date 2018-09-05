using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class CRMLinkTableItem : BaseItem
  {
    public CRMLinkTableItemProxy GetProxy()
    {
      CRMLinkTableItemProxy result = new CRMLinkTableItemProxy();
      result.WebHookTokenId = this.WebHookTokenId;
	  result.WebHookTokenFullUrl = this.WebHookTokenFullUrl;
	  result.UseNetworkCredentials = this.UseNetworkCredentials;
      result.IncludeIssueNonRequired = this.IncludeIssueNonRequired;
      result.ExcludedTicketStatusUpdate = this.ExcludedTicketStatusUpdate;
      result.InstanceName = this.InstanceName;
      result.UpdateTicketType = this.UpdateTicketType;
      result.RestrictedToTicketTypes = this.RestrictedToTicketTypes;
      result.AlwaysUseDefaultProjectKey = this.AlwaysUseDefaultProjectKey;
      result.UseSandBoxServer = this.UseSandBoxServer;
      result.MatchAccountsByName = this.MatchAccountsByName;
      result.DefaultProject = this.DefaultProject;
      result.HostName = this.HostName;
      result.ActionTypeIDToPush = this.ActionTypeIDToPush;
      result.UpdateStatus = this.UpdateStatus;
      result.PullCustomerProducts = this.PullCustomerProducts;
      result.PushTicketsAsCases = this.PushTicketsAsCases;
      result.PullCasesAsTickets = this.PullCasesAsTickets;
      result.DefaultSlaLevelID = this.DefaultSlaLevelID;
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
       
      result.LastProcessed = DateTime.SpecifyKind(this.LastProcessedUtc, DateTimeKind.Utc);
       
      result.LastLink = this.LastLinkUtc == null ? this.LastLinkUtc : DateTime.SpecifyKind((DateTime)this.LastLinkUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
