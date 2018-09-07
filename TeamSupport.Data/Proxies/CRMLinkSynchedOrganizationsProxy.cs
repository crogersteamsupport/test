using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class CRMLinkSynchedOrganization : BaseItem
  {
    public CRMLinkSynchedOrganizationProxy GetProxy()
    {
      CRMLinkSynchedOrganizationProxy result = new CRMLinkSynchedOrganizationProxy();
      result.OrganizationCRMID = this.OrganizationCRMID;
      result.CRMLinkTableID = this.CRMLinkTableID;
      result.CRMLinkSynchedOrganizationsID = this.CRMLinkSynchedOrganizationsID;
       
       
       
      return result;
    }	
  }
}
