using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CRMLinkTable
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CRMLinkID", "CRMLinkID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("Active", "Active", false, false, false);
      _fieldMap.AddMap("CRMType", "CRMType", false, false, false);
      _fieldMap.AddMap("Username", "Username", false, false, false);
      _fieldMap.AddMap("Password", "Password", false, false, false);
      _fieldMap.AddMap("SecurityToken", "SecurityToken", false, false, false);
      _fieldMap.AddMap("TypeFieldMatch", "TypeFieldMatch", false, false, false);
      _fieldMap.AddMap("LastLink", "LastLink", false, false, false);
      _fieldMap.AddMap("SendBackTicketData", "SendBackTicketData", false, false, false);
      _fieldMap.AddMap("LastProcessed", "LastProcessed", false, false, false);
      _fieldMap.AddMap("LastTicketID", "LastTicketID", false, false, false);
      _fieldMap.AddMap("AllowPortalAccess", "AllowPortalAccess", false, false, false);
      _fieldMap.AddMap("SendWelcomeEmail", "SendWelcomeEmail", false, false, false);
      _fieldMap.AddMap("DefaultSlaLevelID", "DefaultSlaLevelID", false, false, false);
            
    }
  }
  
}
