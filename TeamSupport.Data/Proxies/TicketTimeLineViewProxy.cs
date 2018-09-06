using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TicketTimeLineViewItem : BaseItem
  {
    public TicketTimeLineViewItemProxy GetProxy()
    {
      TicketTimeLineViewItemProxy result = new TicketTimeLineViewItemProxy();
      result.WCUserID = this.WCUserID;
      result.IsPinned = this.IsPinned;
      result.IsKnowledgeBase = this.IsKnowledgeBase;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.CreatorName = this.CreatorName;
      result.CreatorID = this.CreatorID;
      result.OrganizationID = this.OrganizationID;
      result.Message = this.Message;
      result.MessageType = this.MessageType == null ? "Description" : this.MessageType;
      result.ActionTypeID = this.ActionTypeID;
      result.IsWC = this.IsWC;
      result.RefID = this.RefID;
      result.TicketNumber = this.TicketNumber;
      result.TicketID = this.TicketID;
      result.TimeSpent = this.TimeSpent;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateStarted = this.DateStartedUtc == null ? this.DateStartedUtc : DateTime.SpecifyKind((DateTime)this.DateStartedUtc, DateTimeKind.Utc); 

       
      return result;
    }	
  }
}
