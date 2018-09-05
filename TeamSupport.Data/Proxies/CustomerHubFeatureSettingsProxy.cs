using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class CustomerHubFeatureSetting : BaseItem
  {
    public CustomerHubFeatureSettingProxy GetProxy()
    {
      CustomerHubFeatureSettingProxy result = new CustomerHubFeatureSettingProxy();
      result.ChatGroupID = this.ChatGroupID;
      result.EnableProductFamilyFiltering = this.EnableProductFamilyFiltering;
      result.EnableCustomFieldModification = this.EnableCustomFieldModification;
      result.EnableCustomerSpecificKB = this.EnableCustomerSpecificKB;
      result.EnableAnonymousProductAssociation = this.EnableAnonymousProductAssociation;
      result.CommunitySortTypeID = this.CommunitySortTypeID;
      result.KnowledgeBaseSortTypeID = this.KnowledgeBaseSortTypeID;
      result.EnableTicketNameModification = this.EnableTicketNameModification;
      result.RestrictProductVersions = this.RestrictProductVersions;
      result.EnableTicketSeverityModification = this.EnableTicketSeverityModification;
      result.EnableTicketSeverity = this.EnableTicketSeverity;
      result.ModifierID = this.ModifierID;
      result.EnableVideoRecording = this.EnableVideoRecording;
      result.EnableScreenRecording = this.EnableScreenRecording;
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
       
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
