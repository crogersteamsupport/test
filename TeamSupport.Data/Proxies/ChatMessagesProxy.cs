using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ChatMessage : BaseItem
  {
    public ChatMessageProxy GetProxy()
    {
      ChatMessageProxy result = new ChatMessageProxy();
      result.PosterType = this.PosterType;
      result.PosterID = this.PosterID;
      result.Message = this.Message;
      result.IsNotification = this.IsNotification;
      result.ChatID = this.ChatID;
      result.ChatMessageID = this.ChatMessageID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
