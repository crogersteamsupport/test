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
    [DataMember] public string SecurityToken { get; set; }
    [DataMember] public string TypeFieldMatch { get; set; }
    [DataMember] public DateTime? LastLink { get; set; }
    [DataMember] public bool SendBackTicketData { get; set; }
    [DataMember] public DateTime LastProcessed { get; set; }
    [DataMember]    public int LastTicketID { get; set; }
    [DataMember]    public bool AllowPortalAccess { get; set; }
      [DataMember] public bool SendWelcomeEmail {get; set;}
  }
  
  public partial class CRMLinkTableItem : BaseItem
  {
    public CRMLinkTableItemProxy GetProxy()
    {
      CRMLinkTableItemProxy result = new CRMLinkTableItemProxy();
      result.LastTicketID = this.LastTicketID;
      result.SendBackTicketData = this.SendBackTicketData;
      result.TypeFieldMatch = this.TypeFieldMatch;
      result.SecurityToken = this.SecurityToken;
      result.Password = this.Password;
      result.Username = this.Username;
      result.CRMType = this.CRMType;
      result.Active = this.Active;
      result.OrganizationID = this.OrganizationID;
      result.CRMLinkID = this.CRMLinkID;
      result.AllowPortalAccess = this.AllowPortalAccess;
      result.SendWelcomeEmail = this.SendWelcomeEmail;

      result.LastProcessed = DateTime.SpecifyKind(this.LastProcessed, DateTimeKind.Local);
       
      result.LastLink = this.LastLink == null ? this.LastLink : DateTime.SpecifyKind((DateTime)this.LastLink, DateTimeKind.Local); 
       
      return result;
    }	
  }
}
