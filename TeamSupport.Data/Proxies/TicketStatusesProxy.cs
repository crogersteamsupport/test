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
  [KnownType(typeof(TicketStatusProxy))]
  public class TicketStatusProxy
  {
    public TicketStatusProxy() {}
    [DataMember] public int TicketStatusID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int Position { get; set; }
    [DataMember] public int TicketTypeID { get; set; }
    [DataMember] public bool IsClosed { get; set; }
    [DataMember] public bool IsClosedEmail { get; set; }
    [DataMember] public bool IsEmailResponse { get; set; }
    //[DataMember] public int OrganizationID { get; set; }
    //[DataMember] public DateTime DateCreated { get; set; }
    //[DataMember] public DateTime DateModified { get; set; }
    //[DataMember] public int CreatorID { get; set; }
    //[DataMember] public int ModifierID { get; set; }
          
  }
  
  public partial class TicketStatus : BaseItem
  {
    public TicketStatusProxy GetProxy()
    {
      TicketStatusProxy result = new TicketStatusProxy();
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");
      //result.ModifierID = this.ModifierID;
      //result.CreatorID = this.CreatorID;
      //result.OrganizationID = this.OrganizationID;
      result.IsEmailResponse = this.IsEmailResponse;
      result.IsClosedEmail = this.IsClosedEmail;
      result.IsClosed = this.IsClosed;
      result.TicketTypeID = this.TicketTypeID;
      result.Position = this.Position;
      result.Description = (this.Description);
      result.Name = (this.Name);
      result.TicketStatusID = this.TicketStatusID;
       
       
       
      return result;
    }	
  }
}
