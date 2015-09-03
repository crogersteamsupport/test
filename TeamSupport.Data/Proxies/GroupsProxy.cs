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
          
  }
  
  public partial class Group : BaseItem
  {
    public GroupProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      GroupProxy result = new GroupProxy();
      //result.ModifierID = this.ModifierID;
      //result.CreatorID = this.CreatorID;
      //result.ImportID = this.ImportID;
      result.Description = sanitizer.Sanitize(this.Description);
      result.Name = sanitizer.Sanitize(this.Name);
      //result.OrganizationID = this.OrganizationID;
      result.GroupID = this.GroupID;
      
      Groups groups = new Groups(BaseCollection.LoginUser);
      result.TicketCount = groups.GetGroupCount(this.GroupID);


//      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      //result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
