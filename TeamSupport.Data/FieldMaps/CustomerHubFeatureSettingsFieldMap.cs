using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CustomerHubFeatureSettings
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CustomerHubFeatureSettingID", "CustomerHubFeatureSettingID", false, false, false);
      _fieldMap.AddMap("CustomerHubID", "CustomerHubID", false, false, false);
      _fieldMap.AddMap("EnableKnowledgeBase", "EnableKnowledgeBase", false, false, false);
      _fieldMap.AddMap("EnableProducts", "EnableProducts", false, false, false);
      _fieldMap.AddMap("EnableTicketCreation", "EnableTicketCreation", false, false, false);
      _fieldMap.AddMap("EnableMyTickets", "EnableMyTickets", false, false, false);
      _fieldMap.AddMap("EnableOrganizationTickets", "EnableOrganizationTickets", false, false, false);
      _fieldMap.AddMap("EnableWiki", "EnableWiki", false, false, false);
      _fieldMap.AddMap("EnableTicketGroupSelection", "EnableTicketGroupSelection", false, false, false);
      _fieldMap.AddMap("EnableTicketProductSelection", "EnableTicketProductSelection", false, false, false);
      _fieldMap.AddMap("EnableTicketProductVersionSelection", "EnableTicketProductVersionSelection", false, false, false);
      _fieldMap.AddMap("DefaultTicketTypeID", "DefaultTicketTypeID", false, false, false);
      _fieldMap.AddMap("DefaultGroupTypeID", "DefaultGroupTypeID", false, false, false);
      _fieldMap.AddMap("EnableCustomerProductAssociation", "EnableCustomerProductAssociation", false, false, false);
      _fieldMap.AddMap("EnableChat", "EnableChat", false, false, false);
      _fieldMap.AddMap("EnableCommunity", "EnableCommunity", false, false, false);
      _fieldMap.AddMap("EnableVideoRecording", "EnableVideoRecording", false, false, false);
      _fieldMap.AddMap("EnableScreenRecording", "EnableScreenRecording", false, false, false);
            
    }
  }
  
}
