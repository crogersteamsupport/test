using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Ganss.XSS;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(CRMLinkSynchedOrganizationProxy))]
  public class CRMLinkSynchedOrganizationProxy
  {
    public CRMLinkSynchedOrganizationProxy() {}
    [DataMember] public int CRMLinkSynchedOrganizationsID { get; set; }
    [DataMember] public int CRMLinkTableID { get; set; }
    [DataMember] public string OrganizationCRMID { get; set; }
          
  }
  
  public partial class CRMLinkSynchedOrganization : BaseItem
  {
    public CRMLinkSynchedOrganizationProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      CRMLinkSynchedOrganizationProxy result = new CRMLinkSynchedOrganizationProxy();
      result.OrganizationCRMID = sanitizer.Sanitize(this.OrganizationCRMID);
      result.CRMLinkTableID = this.CRMLinkTableID;
      result.CRMLinkSynchedOrganizationsID = this.CRMLinkSynchedOrganizationsID;
       
       
       
      return result;
    }	
  }
}
