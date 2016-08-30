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
  [KnownType(typeof(CustomerHubFeatureSettingProxy))]
  public class CustomerHubFeatureSettingProxy
  {
    public CustomerHubFeatureSettingProxy() {}
    [DataMember] public int CustomerHubFeatureSettingID { get; set; }
    [DataMember] public int CustomerHubID { get; set; }
    [DataMember] public bool EnableKnowledgeBase { get; set; }
    [DataMember] public bool EnableProducts { get; set; }
    [DataMember] public bool EnableTicketCreation { get; set; }
    [DataMember] public bool EnableMyTickets { get; set; }
    [DataMember] public bool EnableOrganizationTickets { get; set; }
    [DataMember] public bool EnableWiki { get; set; }
    [DataMember] public bool EnableTicketGroupSelection { get; set; }
    [DataMember] public bool EnableTicketProductSelection { get; set; }
    [DataMember] public bool EnableTicketProductVersionSelection { get; set; }
    [DataMember] public int? DefaultTicketTypeID { get; set; }
    [DataMember] public int? DefaultGroupTypeID { get; set; }
    [DataMember] public bool EnableCustomerProductAssociation { get; set; }
    [DataMember] public bool EnableChat { get; set; }
    [DataMember] public bool EnableCommunity { get; set; }
    [DataMember] public bool EnableVideoRecording { get; set; }
    [DataMember] public bool EnableScreenRecording { get; set; }
          
  }
  
  public partial class CustomerHubFeatureSetting : BaseItem
  {
    public CustomerHubFeatureSettingProxy GetProxy()
    {
      CustomerHubFeatureSettingProxy result = new CustomerHubFeatureSettingProxy();
      result.EnableScreenRecording = this.EnableScreenRecording;
      result.EnableVideoRecording = this.EnableVideoRecording;
      result.EnableCommunity = this.EnableCommunity;
      result.EnableChat = this.EnableChat;
      result.EnableCustomerProductAssociation = this.EnableCustomerProductAssociation;
      result.DefaultGroupTypeID = this.DefaultGroupTypeID;
      result.DefaultTicketTypeID = this.DefaultTicketTypeID;
      result.EnableTicketProductVersionSelection = this.EnableTicketProductVersionSelection;
      result.EnableTicketProductSelection = this.EnableTicketProductSelection;
      result.EnableTicketGroupSelection = this.EnableTicketGroupSelection;
      result.EnableWiki = this.EnableWiki;
      result.EnableOrganizationTickets = this.EnableOrganizationTickets;
      result.EnableMyTickets = this.EnableMyTickets;
      result.EnableTicketCreation = this.EnableTicketCreation;
      result.EnableProducts = this.EnableProducts;
      result.EnableKnowledgeBase = this.EnableKnowledgeBase;
      result.CustomerHubID = this.CustomerHubID;
      result.CustomerHubFeatureSettingID = this.CustomerHubFeatureSettingID;
       
       
       
      return result;
    }	
  }
}
