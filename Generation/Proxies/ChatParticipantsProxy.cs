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
  [KnownType(typeof(ChatParticipantProxy))]
  public class ChatParticipantProxy
  {
    public ChatParticipantProxy() {}
    [DataMember] public int ChatParticipantID { get; set; }
    [DataMember] public int ChatID { get; set; }
    [DataMember] public int ParticipantID { get; set; }
    [DataMember] public ChatParticipantType ParticipantType { get; set; }
    [DataMember] public string IPAddress { get; set; }
    [DataMember] public int LastMessageID { get; set; }
    [DataMember] public int LastPreviewedMessageID { get; set; }
    [DataMember] public DateTime LastTyped { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateJoined { get; set; }
    [DataMember] public DateTime? DateLeft { get; set; }
          
  }
  
  public partial class ChatParticipant : BaseItem
  {
    public ChatParticipantProxy GetProxy()
    {
      ChatParticipantProxy result = new ChatParticipantProxy();
      result.LastPreviewedMessageID = this.LastPreviewedMessageID;
      result.LastMessageID = this.LastMessageID;
      result.IPAddress = this.IPAddress;
      result.ParticipantType = this.ParticipantType;
      result.ParticipantID = this.ParticipantID;
      result.ChatID = this.ChatID;
      result.ChatParticipantID = this.ChatParticipantID;
       
      result.LastTyped = DateTime.SpecifyKind(this.LastTyped, DateTimeKind.Local);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreated, DateTimeKind.Local);
      result.DateJoined = DateTime.SpecifyKind(this.DateJoined, DateTimeKind.Local);
       
      result.DateLeft = this.DateLeft == null ? this.DateLeft : DateTime.SpecifyKind((DateTime)this.DateLeft, DateTimeKind.Local); 
       
      return result;
    }	
  }
}
