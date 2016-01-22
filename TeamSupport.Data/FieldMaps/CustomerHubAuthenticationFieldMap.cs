using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CustomerHubAuthentication
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CustomerHubAuthenticationID", "CustomerHubAuthenticationID", false, false, false);
      _fieldMap.AddMap("CustomerHubID", "CustomerHubID", false, false, false);
      _fieldMap.AddMap("EnableSelfRegister", "EnableSelfRegister", false, false, false);
      _fieldMap.AddMap("EnableRequestAccess", "EnableRequestAccess", false, false, false);
      _fieldMap.AddMap("EnableSSO", "EnableSSO", false, false, false);
      _fieldMap.AddMap("RequestTicketType", "RequestTicketType", false, false, false);
      _fieldMap.AddMap("AnonymousWikiAccess", "AnonymousWikiAccess", false, false, false);
      _fieldMap.AddMap("AnonymousKBAccess", "AnonymousKBAccess", false, false, false);
            
    }
  }
  
}
