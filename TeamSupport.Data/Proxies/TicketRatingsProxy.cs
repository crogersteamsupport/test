using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TicketRating : BaseItem
  {
    public TicketRatingProxy GetProxy()
    {
      TicketRatingProxy result = new TicketRatingProxy();
      result.ThumbsDown = this.ThumbsDown;
      result.ThumbsUp = this.ThumbsUp;
      result.Views = this.Views;
      result.Rating = this.Rating;
      result.Votes = this.Votes;
      result.TicketType = this.TicketType;
      result.TicketID = this.TicketID;
       
       
      result.LastViewed = this.LastViewedUtc == null ? this.LastViewedUtc : DateTime.SpecifyKind((DateTime)this.LastViewedUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
