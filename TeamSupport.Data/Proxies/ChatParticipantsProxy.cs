using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
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
       
      result.LastTyped = DateTime.SpecifyKind(this.LastTypedUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateJoined = DateTime.SpecifyKind(this.DateJoinedUtc, DateTimeKind.Utc);
       
      result.DateLeft = this.DateLeftUtc == null ? this.DateLeftUtc : DateTime.SpecifyKind((DateTime)this.DateLeftUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
