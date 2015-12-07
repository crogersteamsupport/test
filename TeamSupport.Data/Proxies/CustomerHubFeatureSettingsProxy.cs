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
    [DataMember] public bool ShowKnowledgeBase { get; set; }
    [DataMember] public bool ShowProducts { get; set; }
          
  }
  
  public partial class CustomerHubFeatureSetting : BaseItem
  {
    public CustomerHubFeatureSettingProxy GetProxy()
    {
      CustomerHubFeatureSettingProxy result = new CustomerHubFeatureSettingProxy();
      result.ShowProducts = this.ShowProducts;
      result.ShowKnowledgeBase = this.ShowKnowledgeBase;
      result.CustomerHubID = this.CustomerHubID;
      result.CustomerHubFeatureSettingID = this.CustomerHubFeatureSettingID;
       
       
       
      return result;
    }	
  }
}
