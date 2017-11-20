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
  [KnownType(typeof(GroupProxy))]
  public class GroupProxy
  {
    public GroupProxy() {}
    [DataMember] public int GroupID { get; set; }
    //[DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember]
    public int TicketCount { get; set; }
    //[DataMember] public string ImportID { get; set; }
    //[DataMember] public DateTime DateCreated { get; set; }
    //[DataMember] public DateTime DateModified { get; set; }
    //[DataMember] public int CreatorID { get; set; }
    //[DataMember] public int ModifierID { get; set; }
    [DataMember] public int? ProductFamilyID { get; set; }
          
  }
  
  public partial class Group : BaseItem
  {
    public GroupProxy GetProxy()
    {
      GroupProxy result = new GroupProxy();
      result.ProductFamilyID = this.ProductFamilyID;
      result.Description = this.Description;
      result.Name = this.Name;
      result.GroupID = this.GroupID;
      
      Groups groups = new Groups(BaseCollection.LoginUser);
	  result.TicketCount = this.TicketCount;
       
      return result;
    }	
  }
}
