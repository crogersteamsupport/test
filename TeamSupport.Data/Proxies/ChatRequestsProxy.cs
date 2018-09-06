using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ChatRequest : BaseItem
  {
    public ChatRequestProxy GetProxy()
    {
      ChatRequestProxy result = new ChatRequestProxy();
      result.IsAccepted = this.IsAccepted;
      result.RequestType = this.RequestType;
      result.GroupID = this.GroupID;
      result.Message = this.Message;
      result.TargetUserID = this.TargetUserID;
      result.RequestorType = this.RequestorType;
      result.RequestorID = this.RequestorID;
      result.ChatID = this.ChatID;
      result.OrganizationID = this.OrganizationID;
      result.ChatRequestID = this.ChatRequestID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
