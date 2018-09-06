using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TicketTemplate : BaseItem
  {
    public TicketTemplateProxy GetProxy()
    {
      TicketTemplateProxy result = new TicketTemplateProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.TemplateText = this.TemplateText;
      result.TriggerText = this.TriggerText;
      result.TicketTypeID = this.TicketTypeID;
      result.IsEnabled = this.IsEnabled;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.TemplateType = this.TemplateType;
      result.OrganizationID = this.OrganizationID;
      result.TicketTemplateID = this.TicketTemplateID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
